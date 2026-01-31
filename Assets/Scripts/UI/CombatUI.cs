using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MaskMYDrama.Core;
using MaskMYDrama.Combat;
using MaskMYDrama.Cards;

namespace MaskMYDrama.UI
{
    /// <summary>
    /// Main combat UI controller. Manages all UI elements during combat.
    /// 
    /// Implements the UI layout based on reference images (ref_game_001.jpg, ref_game_002.jpg):
    /// - Top Bar: Player name, health, level
    /// - Combat Area: Player and enemy health bars
    /// - Bottom Bar: Energy display, deck counts, end turn button
    /// - Card Hand: 5 cards displayed horizontally
    /// 
    /// Subscribes to CombatManager events for real-time updates.
    /// Handles card interactions and turn management.
    /// </summary>
    public class CombatUI : MonoBehaviour
    {
        [Header("Managers")]
        public CombatManager combatManager;
        public DeckManager deckManager;
        public Player player;
        public Enemy enemy;
        
        [Header("Health Bars")]
        public HealthBar playerHealthBar;
        public HealthBar enemyHealthBar;
        
        [Header("Energy Display")]
        public EnergyDisplay energyDisplay;
        
        [Header("Card Hand UI")]
        public Transform cardHandParent;
        public GameObject cardUIPrefab;
        public ArcCardLayout arcCardLayout;
        
        [Header("Bottom UI")]
        public Button endTurnButton;
        public TextMeshProUGUI poolCountText;
        public TextMeshProUGUI abandonedCountText;
        
        [Header("Top Bar")]
        public TextMeshProUGUI playerNameText;
        public TextMeshProUGUI levelText;
        
        private List<CardUI> cardUIList = new List<CardUI>();
        private HorizontalLayoutGroup horizontalLayoutGroup;
        
        private void Awake()
        {
            // Subscribe to events early to catch any events that fire during Start()
            SubscribeToEvents();
            
            // Setup arc layout if not assigned
            if (arcCardLayout == null && cardHandParent != null)
            {
                arcCardLayout = cardHandParent.GetComponent<ArcCardLayout>();
                if (arcCardLayout == null)
                {
                    arcCardLayout = cardHandParent.gameObject.AddComponent<ArcCardLayout>();
                }
            }
            
            // Disable horizontal layout group if it exists (arc layout will handle positioning)
            if (cardHandParent != null)
            {
                horizontalLayoutGroup = cardHandParent.GetComponent<HorizontalLayoutGroup>();
                if (horizontalLayoutGroup != null)
                {
                    horizontalLayoutGroup.enabled = false;
                }
            }
        }
        
        private void Start()
        {
            InitializeUI();
            // Ensure cards are displayed if combat has already started
            StartCoroutine(InitializeCardsDelayed());
        }
        
        private IEnumerator InitializeCardsDelayed()
        {
            // Wait one frame to ensure all Start() methods have completed
            yield return null;
            
            // Check if combat is active and cards should be displayed
            if (combatManager != null && combatManager.IsCombatActive() && 
                deckManager != null && deckManager.HandCount > 0)
            {
                UpdateCardHand();
            }
        }
        
        private void InitializeUI()
        {
            if (player != null && playerHealthBar != null)
            {
                playerHealthBar.Initialize(player.maxHealth);
            }
            
            if (enemy != null && enemyHealthBar != null)
            {
                enemyHealthBar.Initialize(enemy.maxHealth);
            }
            
            if (endTurnButton != null)
            {
                endTurnButton.onClick.AddListener(OnEndTurnClicked);
            }

            if(levelText != null)
            {
                levelText.text = MapsDataSingleton.Instance.MapName;
            }
            
            UpdateAllUI();
            
            // If cards are already available (combat started before UI initialized), display them
            if (deckManager != null && deckManager.HandCount > 0)
            {
                UpdateCardHand();
            }
        }
        
        private void SubscribeToEvents()
        {
            if (combatManager != null)
            {
                combatManager.OnPlayerTurnStart += OnPlayerTurnStart;
                combatManager.OnEnemyTurnStart += OnEnemyTurnStart;
                combatManager.OnStateChanged += OnCombatStateChanged;
            }
        }
        
        private void OnPlayerTurnStart()
        {
            UpdateCardHand();
            UpdateAllUI();
            // Enable card interaction during player turn
            SetCardInteractionEnabled(true);
        }
        
        private void OnEnemyTurnStart()
        {
            // Disable card interaction during enemy turn
            SetCardInteractionEnabled(false);
        }
        
        private void OnCombatStateChanged(CombatState state)
        {
            if (state == CombatState.Victory)
            {
                ShowVictory();
            }
            else if (state == CombatState.Defeat)
            {
                ShowDefeat();
            }
        }
        
        private void UpdateCardHand()
        {
            // Clear existing cards
            foreach (var cardUI in cardUIList)
            {
                if (cardUI != null)
                    Destroy(cardUI.gameObject);
            }
            cardUIList.Clear();
            
            // Create card UIs
            var hand = deckManager.GetHand();
            for (int i = 0; i < hand.Count; i++)
            {
                if (cardUIPrefab != null && cardHandParent != null)
                {
                    GameObject cardObj = Instantiate(cardUIPrefab, cardHandParent);
                    CardUI cardUI = cardObj.GetComponent<CardUI>();
                    
                    if (cardUI != null)
                    {
                        bool playable = player.HasEnoughEnergy(hand[i].GetEnergyCost());
                        cardUI.SetupCard(hand[i], i, playable);
                        cardUI.OnCardClicked += OnCardClicked;
                        cardUI.OnCardSelected += OnCardSelected;
                        cardUIList.Add(cardUI);
                    }
                }
            }
            
            // Update arc layout after cards are created
            if (arcCardLayout != null)
            {
                // Update layout immediately (no animation) for initial positioning
                // Cards will store their arc positions in their Start() methods
                StartCoroutine(UpdateArcLayoutDelayed());
            }
        }
        
        private IEnumerator UpdateArcLayoutDelayed()
        {
            // Wait one frame to ensure all cards are instantiated
            yield return null;
            if (arcCardLayout != null)
            {
                // Set positions immediately for initial layout (no animation)
                arcCardLayout.UpdateLayout(immediate: true);
            }
        }
        
        private void OnCardSelected(int handIndex)
        {
            // Deselect all other cards when one is selected
            for (int i = 0; i < cardUIList.Count; i++)
            {
                if (cardUIList[i] != null && i != handIndex && cardUIList[i].IsSelected())
                {
                    cardUIList[i].DeselectCard();
                }
            }
        }
        
        private void OnCardClicked(int handIndex)
        {
            // Verify that the card at this index is actually selected before playing
            if (handIndex < 0 || handIndex >= cardUIList.Count)
                return;
            
            CardUI clickedCard = cardUIList[handIndex];
            if (clickedCard == null || !clickedCard.IsSelected())
            {
                // Card is not selected, ignore the click
                return;
            }
            
            // Store the card that was clicked (before it's removed from hand)
            int clickedHandIndex = handIndex;
            
            if (combatManager.TryPlayCard(handIndex))
            {
                // Card was played successfully - deselect all cards
                for (int i = 0; i < cardUIList.Count; i++)
                {
                    if (cardUIList[i] != null && cardUIList[i].IsSelected())
                    {
                        cardUIList[i].DeselectCard();
                    }
                }
                
                // Update hand display (this will refresh after draw card effects)
                // Use a coroutine to wait a frame for draw card effects to complete
                StartCoroutine(UpdateHandAfterCardPlay());
                UpdateAllUI();
            }
        }
        
        private System.Collections.IEnumerator UpdateHandAfterCardPlay()
        {
            // Wait one frame to allow draw card effects to complete
            yield return null;
            UpdateCardHand();
            UpdateAllUI();
        }
        
        private void OnEndTurnClicked()
        {
            if (combatManager.IsPlayerTurn())
            {
                // Only end turn if player can't play any more cards
                combatManager.EndPlayerTurn();
            }
        }
        
        private void SetCardInteractionEnabled(bool enabled)
        {
            var hand = deckManager.GetHand();
            for (int i = 0; i < cardUIList.Count && i < hand.Count; i++)
            {
                if (cardUIList[i] != null)
                {
                    bool playable = enabled && player.HasEnoughEnergy(hand[i].GetEnergyCost());
                    cardUIList[i].UpdatePlayableState(playable);
                }
            }
            
            if (endTurnButton != null)
            {
                endTurnButton.interactable = enabled;
            }
        }
        
        private void UpdateAllUI()
        {
            // Update health bars
            if (player != null && playerHealthBar != null)
            {
                playerHealthBar.SetHealth(player.currentHealth);
            }
            
            if (enemy != null && enemyHealthBar != null)
            {
                enemyHealthBar.SetHealth(enemy.currentHealth);
            }
            
            // Update energy
            if (player != null && energyDisplay != null)
            {
                energyDisplay.UpdateEnergy(player.currentEnergy, player.maxEnergy);
            }
            
            // Update deck counts
            if (poolCountText != null)
            {
                poolCountText.text = $"{deckManager.PoolCount}";
            }
            
            if (abandonedCountText != null)
            {
                abandonedCountText.text = $"{deckManager.AbandonedCount}";
            }
            
            // Update card playability
            UpdateCardPlayability();
        }
        
        private void UpdateCardPlayability()
        {
            var hand = deckManager.GetHand();
            for (int i = 0; i < cardUIList.Count && i < hand.Count; i++)
            {
                if (cardUIList[i] != null)
                {
                    bool playable = player.HasEnoughEnergy(hand[i].GetEnergyCost());
                    cardUIList[i].UpdatePlayableState(playable);
                }
            }
        }
        
        private void ShowVictory()
        {
            Debug.Log("Victory! All enemies defeated.");
            // TODO: Show victory UI
        }
        
        private void ShowDefeat()
        {
            Debug.Log("Defeat! Player life bar reached 0.");
            // TODO: Show defeat UI
        }
        
        private void Update()
        {
            // Update UI every frame (could be optimized with events)
            if (combatManager != null && combatManager.IsCombatActive())
            {
                UpdateAllUI();
            }
        }
        
        private void OnDestroy()
        {
            if (combatManager != null)
            {
                combatManager.OnPlayerTurnStart -= OnPlayerTurnStart;
                combatManager.OnEnemyTurnStart -= OnEnemyTurnStart;
                combatManager.OnStateChanged -= OnCombatStateChanged;
            }
        }
    }
}

