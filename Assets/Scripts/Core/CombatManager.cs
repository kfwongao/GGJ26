using System.Collections;
using UnityEngine;
using MaskMYDrama.Combat;
using MaskMYDrama.Cards;
using MaskMYDrama.Core;
using System.Collections.Generic;

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




        private void Start()
        {
            ObjectPool = new Dictionary<string, List<GameObject>>();
            StartCombat();
        }
        
        public void StartCombat()
        {
            currentState = CombatState.PlayerTurn;
            StartPlayerTurn();
        }
        
        public void StartPlayerTurn()
        {
            currentState = CombatState.PlayerTurn;
            player.ResetTurn();
            deckManager.DrawHand();
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
            
            // Apply card effects
            ApplyCardEffects(playedCard);
            
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
                    break;
                    
                case CardType.Defence:
                    player.AddDefence(card.GetDefenceValue());

                    CombatInfoHolder defenceCardName = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    defenceCardName.Init($"Using {card.cardData.cardName}...", Color.white);


                    BufferInfoHolder defence = Instantiate(bufferInfoHolder, PlayerSpawnGameObjectPool.transform);
                    defence.Init($"+{card.GetDefenceValue()}", Color.yellow);

                    // Play Spell skill anim, display card skill name at player holder, play incease value at player holder
                    break;
                    
                case CardType.Strength:
                    player.AddAttackPower(card.cardData.strengthValue);

                    CombatInfoHolder strengthCardName = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                    strengthCardName.Init($"Using {card.cardData.cardName}...", Color.white);


                    BufferInfoHolder powering = Instantiate(bufferInfoHolder, PlayerSpawnGameObjectPool.transform);
                    powering.Init($"+{card.cardData.strengthValue}", Color.cyan);

                    // Play Spell skill anim, display card skill name at player holder, play incease value at player holder
                    break;
                    
                case CardType.Function:
                    // Special function cards
                    if (card.cardData.drawCard)
                    {
                        // Draw additional card (simplified - would need proper implementation)
                        // animation vfx

                        CombatInfoHolder drawCardName = Instantiate(combatInfoHolder, CombatInfoPool.transform);
                        drawCardName.Init($"Using {card.cardData.cardName}...", Color.white);
                    }
                    break;
            }
            
            // Check win condition
            if (!enemy.IsAlive())
            {
                EndCombatInfoHolder playerIsNotAlive = Instantiate(endCombatInfoHolder, CombatInfoPool.transform);
                playerIsNotAlive.Init($"Victory, Cheer.....", Color.yellow);

                currentState = CombatState.Victory;
                OnStateChanged?.Invoke(currentState);
                OnCombatEnd?.Invoke();
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

            // Play attack anim for enemy

            // display attack skill name at enemy holder
            DamageInfoHolder attackSkillName = Instantiate(damageInfoHolder, CombatInfoPool.transform);
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

