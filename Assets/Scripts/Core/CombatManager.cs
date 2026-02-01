using MaskMYDrama.Cards;
using MaskMYDrama.Combat;
using MaskMYDrama.Core;
using MaskMYDrama.Effects;
using MaskMYDrama.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

namespace MaskMYDrama.Core
{
    /// <summary>
    /// Combat state machine states.
    /// Controls the flow of combat turns and outcomes.
    /// </summary>
    public enum CombatState
    {
        PlayerTurn,  // Player can play cards
        EnemyTurn,   // Enemy executes attack
        Victory,     // All enemies defeated
        Defeat       // Player health reached 0
    }
    
    /// <summary>
    /// Manages turn-based combat flow as specified in CSV gameplay design.
    /// 
    /// Implements the combat system:
    /// - Turn-based: Player first, then enemy (as per CSV: "Player and enemy move in turn in each round; Player first, then enemy")
    /// - Energy system: Player can only play cards if energy >= card cost
    /// - Win condition: All enemies' life bar go to 0
    /// - Lose condition: Player's life bar go to 0
    /// - Auto-end turn: When player has no enough energy for any card in hand
    /// 
    /// Based on CSV requirements:
    /// - "Player's move: Can, energy balls > the chosen card"
    /// - "Enemy's Turn: Player has no enough energy for any card in 手牌"
    /// - "Player's win: In each round, all enemies life bar go to 0"
    /// - "Player's lose: In each round, player's life bar go to 0"
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Combat Entities")]
        public Player player;
        public Enemy enemy;
        
        [Header("Managers")]
        public DeckManager deckManager;
        public CardEffectExecutor effectExecutor;
        public CardDatabase cardDatabase;
        public CardDatabaseList cardDatabaseList;

        [Header("Roguelike Selection")]
        [Tooltip("Card selection UI for roguelike card selection between levels")]
        public CardSelectionUI cardSelectionUI;
        
        private CombatState currentState = CombatState.PlayerTurn;
        
        public CombatState CurrentState => currentState;
        
        // Events
        public System.Action<CombatState> OnStateChanged;
        public System.Action OnPlayerTurnStart;
        public System.Action OnEnemyTurnStart;
        public System.Action OnCombatEnd;


        public GameObject EnemySpawnGameObjectPool;
        public GameObject PlayerSpawnGameObjectPool;
        public BufferInfoHolder bufferInfoHolder;
        public DamageInfoHolder damageInfoHolder;

        public GameObject CombatInfoPool;
        public CombatInfoHolder combatInfoHolder;
        public EndCombatInfoHolder endCombatInfoHolder;

        public Dictionary<string, List<GameObject>> ObjectPool; // Implement if have time left

        public bool canGoNextRound = true;
        public Button go_next_level;
        public Button re_try_current;
        
        [Header("Encore System")]
        [Tooltip("Encore card actions for selection (3 actions)")]
        public EncoreCardAction[] encoreCardActions = new EncoreCardAction[3];
        
        // Encore tracking variables
        private int combatRoundCount = 0;
        private int playerInitialHealth = 0;
        private int totalDamageTaken = 0;
        private bool enemyKilled = false;

        public AudioSource audioSrc;

        public AudioClip audioClip_attack;
        public AudioClip audioClip_booking;
        public AudioClip audioClip_cheer;
        public AudioClip audioClip_tapping_the_ground;
        public AudioClip audioClip_enqure;
        public AudioClip audioClip_slide_card;
        public AudioClip audioClip_stage_battle;

        public GameObject endGO;

        private void Start()
        {
            ObjectPool = new Dictionary<string, List<GameObject>>();
            go_next_level.gameObject.SetActive(false);
            re_try_current.gameObject.SetActive(false);

            string[] splitStr = MapsDataSingleton.Instance.MapName.Split('_');
            if (splitStr.Length > 1)
            {
                int level = int.Parse(splitStr[1]);
                
            }

            if(endGO != null)
            {
                endGO.SetActive(false);
            }

            // Initialize effect executor if not assigned
            if (effectExecutor == null)
            {
                effectExecutor = GetComponent<CardEffectExecutor>();
                if (effectExecutor == null)
                {
                    effectExecutor = gameObject.AddComponent<CardEffectExecutor>();
                }
            }
            
            // Setup effect executor references
            if (effectExecutor != null)
            {
                effectExecutor.player = player;
                effectExecutor.enemy = enemy;
                effectExecutor.deckManager = deckManager;
            }
            
            StartCombat();
        }
        
        public void StartCombat()
        {
            // Reset Encore tracking
            combatRoundCount = 0;
            playerInitialHealth = player.currentHealth;
            totalDamageTaken = 0;
            enemyKilled = false;
            
            // Draw one roguelike card from card pool when entering a level (if not first level)
            DrawRoguelikeCardOnLevelStart();
            
            currentState = CombatState.PlayerTurn;
            StartPlayerTurn();
        }
        
        /// <summary>
        /// Draws one roguelike card from card pool and adds it to draw pile when entering a level.
        /// This happens in subsequent levels (not the first level).
        /// </summary>
        private void DrawRoguelikeCardOnLevelStart()
        {
            // Check if this is not the first level
            string[] splitStr = MapsDataSingleton.Instance.MapName.Split('_');
            if (splitStr.Length > 1)
            {
                int level = int.Parse(splitStr[1]);
                
                // Only draw roguelike card if not first level (level > 1)
                if (level > 1 && deckManager != null)
                {
                    List<Card> randomCards = null;
                    
                    // Try to get card from CardDatabaseList first, then fallback to CardDatabase
                    if (cardDatabaseList != null)
                    {
                        CardDatabase levelDatabase = cardDatabaseList.GetCardDatabase(level);
                        if (levelDatabase != null)
                        {
                            randomCards = levelDatabase.GetRandomCards(1);
                        }
                    }
                    
                    // Fallback to single CardDatabase if CardDatabaseList didn't work
                    if ((randomCards == null || randomCards.Count == 0) && cardDatabase != null)
                    {
                        randomCards = cardDatabase.GetRandomCards(1);
                    }
                    
                    if (randomCards != null && randomCards.Count > 0)
                    {
                        // Add to draw pile (card pool)
                        deckManager.AddCardToPool(randomCards[0]);
                        Debug.Log($"Level {level}: Added roguelike card {randomCards[0]} to draw pile");
                    }
                    else
                    {
                        Debug.LogWarning($"Level {level}: No roguelike cards available to add to draw pile.");
                    }
                }
            }
        }
        
        public void StartPlayerTurn()
        {
            currentState = CombatState.PlayerTurn;
            player.ResetTurn();
            deckManager.DrawHand();
            
            // Increment round count at start of player turn
            combatRoundCount++;
            
            OnPlayerTurnStart?.Invoke();
            OnStateChanged?.Invoke(currentState);
        }
        
        public bool TryPlayCard(int handIndex)
        {
            if (currentState != CombatState.PlayerTurn)
                return false;
            
            if (handIndex < 0 || handIndex >= deckManager.HandCount)
                return false;
            
            var cardInstance = deckManager.GetHand()[handIndex];
            
            // Check energy
            if (!player.HasEnoughEnergy(cardInstance.GetEnergyCost()))
                return false;
            
            // Play the card
            CardInstance playedCard = deckManager.PlayCard(handIndex);
            if (playedCard == null)
                return false;
            
            // Consume energy
            player.ConsumeEnergy(playedCard.GetEnergyCost());
            
            // Apply card effects (this may trigger draw card effects)
            ApplyCardEffects(playedCard);
            
            // Handle draw card effects (similar to Slay the Spire)
            if (playedCard.cardData.drawCardCount > 0)
            {
                int cardsDrawn = deckManager.DrawCards(playedCard.cardData.drawCardCount);
                if (cardsDrawn > 0)
                {
                    // Create visual feedback for drawing cards
                    CombatInfoHolder drawInfo = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    drawInfo.Init($"Drew {cardsDrawn} card(s)!", Color.cyan, -1); // msg goes down
                    
                    // Notify UI to update hand display (will be handled by CombatUI update)
                }
            }
            
            // Check if player can still play cards
            if (!CanPlayerPlayAnyCard())
            {
                // Auto end turn if no cards can be played
                EndPlayerTurn();
            }
            
            return true;
        }
        
        private void ApplyCardEffects(CardInstance card)
        {
            // Version 2.0: Try advanced effect system first
            if (card.cardData.useAdvancedEffects && card.cardData.cardEffect != null && effectExecutor != null)
            {
                bool effectApplied = effectExecutor.ExecuteEffects(card.cardData.cardEffect, card);
                if (effectApplied)
                {
                    // Show card name feedback
                    CombatInfoHolder cardNameFeedback = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    cardNameFeedback.Init($"Using {card.cardData.cardName}...", Color.white);
                    
                    // Check win condition after effect
                    CheckWinCondition();
                    return;
                }
            }
            
            // Fallback to legacy system for backward compatibility
            switch (card.cardData.cardType)
            {
                case CardType.Attack:
                    int attackDamage = card.GetAttackValue() + player.attackPower;
                    enemy.TakeDamage(attackDamage);

                    // Play Spell skill anim, display card skill name at player holder, play hit anim for enemy, display damage value at enemy holder

                    CombatInfoHolder attackCardName = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    attackCardName.Init($"Using {card.cardData.cardName}...", Color.white);

                    DamageInfoHolder holder = Instantiate(damageInfoHolder, EnemySpawnGameObjectPool.transform);
                    holder.Init($"-{attackDamage}", Color.red);

                    if(audioSrc != null)
                    {
                        audioSrc.PlayOneShot(audioClip_attack);
                    }
                    break;
                    
                case CardType.Defence:
                    player.AddDefence(card.GetDefenceValue());

                    CombatInfoHolder defenceCardName = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    defenceCardName.Init($"Using {card.cardData.cardName}...", Color.white);


                    BufferInfoHolder defence = Instantiate(bufferInfoHolder, PlayerSpawnGameObjectPool.transform);
                    defence.Init($"+{card.GetDefenceValue()}", Color.yellow);

                    // Play Spell skill anim, display card skill name at player holder, play incease value at player holder

                    if (audioSrc != null)
                    {
                        audioSrc.PlayOneShot(audioClip_booking);
                    }
                    break;
                    
                case CardType.Strength:
                    player.AddAttackPower(card.cardData.strengthValue);

                    CombatInfoHolder strengthCardName = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    strengthCardName.Init($"Using {card.cardData.cardName}...", Color.white);


                    BufferInfoHolder powering = Instantiate(bufferInfoHolder, PlayerSpawnGameObjectPool.transform);
                    powering.Init($"+{card.cardData.strengthValue}", Color.cyan);

                    // Play Spell skill anim, display card skill name at player holder, play incease value at player holder

                    if (audioSrc != null)
                    {
                        audioSrc.PlayOneShot(audioClip_tapping_the_ground);
                    }
                    break;
                    
                case CardType.Function:
                    // Special function cards - draw card effects are handled after ApplyCardEffects
                    CombatInfoHolder functionCardName = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    functionCardName.Init($"Using {card.cardData.cardName}...", Color.white);

                    if (audioSrc != null)
                    {
                        audioSrc.PlayOneShot(audioClip_enqure);
                    }
                    break;
            }
            
            // Check win condition
            CheckWinCondition();
        }
        
        private void CheckWinCondition()
        {
            if (!enemy.IsAlive())
            {
                enemyKilled = true;

                if (audioSrc != null)
                {
                    audioSrc.PlayOneShot(audioClip_cheer);
                }

                // Check Encore condition: no damage taken and killed enemy within X rounds
                CheckEncoreCondition();
                
                EndCombatInfoHolder playerIsNotAlive = Instantiate(endCombatInfoHolder, CombatInfoPool.transform);
                playerIsNotAlive.Init($"Please choose next level to continue...", Color.yellow);

                currentState = CombatState.Victory;
                OnStateChanged?.Invoke(currentState);
                OnCombatEnd?.Invoke();

                go_next_level.gameObject.SetActive(true);
                re_try_current.gameObject.SetActive(false);
                canGoNextRound = true;
            }
        }
        
        /// <summary>
        /// Checks if Encore condition is met: no damage taken and killed enemy within X rounds
        /// </summary>
        private void CheckEncoreCondition()
        {
            if (enemyKilled && player.IsAlive())
            {
                // Check if no damage was taken (current health equals or exceeds initial health)
                // Note: We track damage taken separately in case health is restored
                bool noDamageTaken = (totalDamageTaken == 0) || (player.currentHealth >= playerInitialHealth);
                
                // Check if killed within round limit
                bool withinRoundLimit = combatRoundCount <= PlayerData.Instance.encoreRoundLimit;
                
                if (noDamageTaken && withinRoundLimit)
                {
                    PlayerData.Instance.isEncoreActive = true;
                    Debug.Log($"Encore achieved! No damage taken in {combatRoundCount} rounds.");

                    if (audioSrc != null)
                    {
                        audioSrc.PlayOneShot(audioClip_enqure);
                    }
                }
            }
        }

        /// <summary>
        /// Version 2.0: Go to next level with roguelike card selection or Encore card selection.
        /// If Encore is active and not final level, show Encore card selection.
        /// Otherwise, show normal roguelike card selection.
        /// </summary>
        public void Go_Next_Level()
        {
            // Hide the button first
            go_next_level.gameObject.SetActive(false);
            
            // Check if it's the final level
            string[] splitStr = MapsDataSingleton.Instance.MapName.Split('_');
            bool isFinalLevel = false;
            if (splitStr.Length > 1)
            {
                int level = int.Parse(splitStr[1]);
                isFinalLevel = (level >= 4); // Level 4 is final level
            }
            
            // If Encore is active and not final level, show Encore card selection
            if (PlayerData.Instance.isEncoreActive && !isFinalLevel && cardSelectionUI != null)
            {
                // Validate Encore actions are assigned
                if (encoreCardActions == null || encoreCardActions.Length != 3)
                {
                    Debug.LogError("EncoreCardActions array is not properly set up! It must contain exactly 3 EncoreCardAction ScriptableObjects. Please assign them in the CombatManager Inspector.");
                    // Fallback to regular card selection
                    cardSelectionUI.OnCardSelected += OnRoguelikeCardSelected;
                    cardSelectionUI.ShowCardSelection();
                    return;
                }
                
                // Check for null actions
                bool hasNullAction = false;
                for (int i = 0; i < encoreCardActions.Length; i++)
                {
                    if (encoreCardActions[i] == null)
                    {
                        Debug.LogError($"EncoreCardAction at index {i} is null! Please assign all 3 EncoreCardAction ScriptableObjects in the CombatManager Inspector.");
                        hasNullAction = true;
                    }
                }
                
                if (hasNullAction)
                {
                    // Fallback to regular card selection
                    cardSelectionUI.OnCardSelected += OnRoguelikeCardSelected;
                    cardSelectionUI.ShowCardSelection();
                    return;
                }
                
                // Show Encore card selection
                cardSelectionUI.OnEncoreCardActionSelected += OnEncoreCardActionSelected;
                cardSelectionUI.ShowEncoreCardSelection(encoreCardActions);
            }
            else if (cardSelectionUI != null)
            {
                // Show normal roguelike card selection
                cardSelectionUI.OnCardSelected += OnRoguelikeCardSelected;
                cardSelectionUI.ShowCardSelection();
            }
            else
            {
                // Fallback: if no card selection UI, go directly to next level
                Debug.LogWarning("CardSelectionUI not assigned, proceeding directly to next level");
                ProceedToNextLevel();
            }
        }
        
        /// <summary>
        /// Called when player selects a card from roguelike selection.
        /// </summary>
        private void OnRoguelikeCardSelected(Card selectedCard)
        {
            if (selectedCard != null && deckManager != null)
            {
                // Add selected card to pool
                deckManager.AddCardToPool(selectedCard);
                
                // Show feedback
                if (CombatInfoPool != null && combatInfoHolder != null)
                {
                    CombatInfoHolder feedback = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    feedback.Init($"Added {selectedCard.cardName} to deck!", Color.green);
                }
            }
            
            // Unsubscribe from event
            if (cardSelectionUI != null)
            {
                cardSelectionUI.OnCardSelected -= OnRoguelikeCardSelected;
            }
            
            // Reset Encore status after using it
            PlayerData.Instance.isEncoreActive = false;

            if (audioSrc != null)
            {
                audioSrc.PlayOneShot(audioClip_tapping_the_ground);
            }

            // Proceed to next level
            ProceedToNextLevel();
        }
        
        /// <summary>
        /// Called when player selects an Encore card action.
        /// </summary>
        private void OnEncoreCardActionSelected(EncoreCardAction selectedAction)
        {
            if (selectedAction != null && deckManager != null)
            {
                // Execute the selected Encore action
                ExecuteEncoreAction(selectedAction);
                
                // Show feedback
                if (CombatInfoPool != null && combatInfoHolder != null)
                {
                    CombatInfoHolder feedback = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    feedback.Init($"Encore: {selectedAction.GetActionName()}!", Color.yellow);
                }
            }
            
            // Unsubscribe from event
            if (cardSelectionUI != null)
            {
                cardSelectionUI.OnEncoreCardActionSelected -= OnEncoreCardActionSelected;
            }

            if (audioSrc != null)
            {
                audioSrc.PlayOneShot(audioClip_tapping_the_ground);
            }

            // Reset Encore status after using it
            PlayerData.Instance.isEncoreActive = false;
            
            // Proceed to next level
            ProceedToNextLevel();
        }
        
        /// <summary>
        /// Executes the selected Encore action.
        /// </summary>
        private void ExecuteEncoreAction(EncoreCardAction action)
        {
            switch (action.actionType)
            {
                case EncoreActionType.AddRandomNewCard:
                    if(cardDatabaseList != null)
                    {
                        // Check if this is not the first level
                        string[] splitStr = MapsDataSingleton.Instance.MapName.Split('_');
                        if (splitStr.Length > 1)
                        {
                            int level = int.Parse(splitStr[1]);
                            List<Card> randomCards = cardDatabaseList.GetCardDatabase(level).GetRandomCards(1);
                            if (randomCards.Count > 0)
                            {
                                deckManager.AddCardToPool(randomCards[0]);
                                Debug.Log($"Encore: Added random card {randomCards[0].cardName} to draw pile");
                                return;
                            }
                        }

                    }

                    // Randomly add 1 new card to draw pile
                    if (cardDatabase != null)
                    {
                        List<Card> randomCards = cardDatabase.GetRandomCards(1);
                        if (randomCards.Count > 0)
                        {
                            deckManager.AddCardToPool(randomCards[0]);
                            Debug.Log($"Encore: Added random card {randomCards[0].cardName} to draw pile");
                        }
                    }
                    break;
                    
                case EncoreActionType.ShuffleDrawPile:
                    // Shuffle entire draw pile
                    deckManager.ShufflePool();
                    Debug.Log("Encore: Shuffled entire draw pile");
                    break;
                    
                case EncoreActionType.CopyFromDiscardPile:
                    // Add a copy of a random card from discard pile to draw pile
                    CardInstance randomDiscardCard = deckManager.GetRandomCardFromDiscardPile();
                    if (randomDiscardCard != null)
                    {
                        deckManager.AddCardToPool(randomDiscardCard.cardData);
                        Debug.Log($"Encore: Copied {randomDiscardCard.cardData.cardName} from discard pile to draw pile");
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Proceeds to the next level (original logic).
        /// </summary>
        private void ProceedToNextLevel()
        {
            string[] splitStr = MapsDataSingleton.Instance.MapName.Split('_');
            if (splitStr.Length > 1)
            {
                int level = int.Parse(splitStr[1]);
                if (canGoNextRound == true)
                {
                    ++level;
                }
                switch (level)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        MapsDataSingleton.Instance.MapName = $"level_{level}";
                        MapsDataSingleton.Instance.LocationAreaName = $"level_{level}";
                        initSceneManager.Instance.InitScene($"level_{level}");

                        break;
                    case 5:
                        EndCombatInfoHolder endgamemsg = Instantiate(endCombatInfoHolder, CombatInfoPool.transform);
                        endgamemsg.Init($"You Win, Cheer。", Color.yellow);

                        if(endGO != null)
                        {
                            endGO.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        private bool CanPlayerPlayAnyCard()
        {
            foreach (var card in deckManager.GetHand())
            {
                if (player.HasEnoughEnergy(card.GetEnergyCost()))
                    return true;
            }
            return false;
        }
        
        public void EndPlayerTurn()
        {
            if (currentState != CombatState.PlayerTurn)
                return;
            
            // Move unused cards to abandoned
            deckManager.EndRound();
            
            // Check lose condition
            if (!player.IsAlive())
            {

                EndCombatInfoHolder playerIsNotAlive = Instantiate(endCombatInfoHolder, CombatInfoPool.transform);
                playerIsNotAlive.Init($"You Lose.....", Color.red);

                go_next_level.gameObject.SetActive(false);
                re_try_current.gameObject.SetActive(true);
                canGoNextRound = false;


                currentState = CombatState.Defeat;
                OnStateChanged?.Invoke(currentState);
                OnCombatEnd?.Invoke();
                return;
            }
            
            // Start enemy turn
            StartCoroutine(EnemyTurnCoroutine());
        }
        
        private IEnumerator EnemyTurnCoroutine()
        {
            currentState = CombatState.EnemyTurn;
            OnEnemyTurnStart?.Invoke();
            OnStateChanged?.Invoke(currentState);
            
            // Wait a moment for visual feedback
            yield return new WaitForSeconds(0.5f);
            
            // Enemy attacks
            int damageVal = enemy.ExecuteAttack(player);
            
            // Track damage taken for Encore checking
            int actualDamage = playerInitialHealth - player.currentHealth;
            totalDamageTaken = actualDamage;

            // Play attack anim for enemy

            // display attack skill name at enemy holder
            CombatInfoHolder attackSkillName = Instantiate(combatInfoHolder, CombatInfoPool.transform);
            attackSkillName.Init($"Attacking Player...", Color.white);
            yield return new WaitForSeconds(0.5f);


            // //play hit anim for player,

            // display damage value at player holder
            DamageInfoHolder hittingInfo = Instantiate(damageInfoHolder, PlayerSpawnGameObjectPool.transform);
            hittingInfo.Init($"-{damageVal}", Color.red);

            // Wait for attack animation
            yield return new WaitForSeconds(1f);
            
            // Check lose condition
            if (!player.IsAlive())
            {
                EndCombatInfoHolder playerIsNotAlive = Instantiate(endCombatInfoHolder, CombatInfoPool.transform);
                playerIsNotAlive.Init($"You Lose.....", Color.red);

                go_next_level.gameObject.SetActive(false);
                re_try_current.gameObject.SetActive(true);
                canGoNextRound = false;

                currentState = CombatState.Defeat;
                OnStateChanged?.Invoke(currentState);
                OnCombatEnd?.Invoke();
                yield break;
            }
            
            // Reset enemy for next turn
            enemy.ResetTurn();
            
            // Start next player turn
            yield return new WaitForSeconds(0.5f);
            StartPlayerTurn();
        }
        
        public bool IsPlayerTurn()
        {
            return currentState == CombatState.PlayerTurn;
        }
        
        public bool IsCombatActive()
        {
            return currentState == CombatState.PlayerTurn || currentState == CombatState.EnemyTurn;
        }
    }
}

