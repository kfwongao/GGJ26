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
        
        [Header("Bottom UI")]
        public Button endTurnButton;
        public TextMeshProUGUI poolCountText;
        public TextMeshProUGUI abandonedCountText;
        
        [Header("Top Bar")]
        public TextMeshProUGUI playerNameText;
        public TextMeshProUGUI levelText;
        
        private List<CardUI> cardUIList = new List<CardUI>();
        
        private void Start()
        {
            InitializeUI();
            SubscribeToEvents();
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
            
            UpdateAllUI();
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
                        cardUIList.Add(cardUI);
                    }
                }
            }
        }
        
        private void OnCardClicked(int handIndex)
        {
            if (combatManager.TryPlayCard(handIndex))
            {
                // Card was played successfully
                UpdateCardHand();
                UpdateAllUI();
            }
        }
        
        private void OnEndTurnClicked()
        {
            if (combatManager.IsPlayerTurn())
            {
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
                poolCountText.text = $"Pool: {deckManager.PoolCount}";
            }
            
            if (abandonedCountText != null)
            {
                abandonedCountText.text = $"Abandoned: {deckManager.AbandonedCount}";
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

