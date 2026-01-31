using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MaskMYDrama.Core;
using MaskMYDrama.Combat;
using MaskMYDrama.Cards;
using DG.Tweening;

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
        
        [Header("Sprite Highlighting")]
        [Tooltip("Player sprite/image component for highlighting")]
        public Image playerSprite;
        [Tooltip("Enemy sprite/image component for highlighting")]
        public Image enemySprite;
        [Tooltip("Color to use when highlighting sprites")]
        public Color highlightColor = new Color(1f, 1f, 0.5f, 1f); // Yellow tint
        [Tooltip("Duration for highlight animation")]
        public float highlightDuration = 0.3f;
        
        private List<CardUI> cardUIList = new List<CardUI>();
        private HorizontalLayoutGroup horizontalLayoutGroup;
        
        // Highlighting state
        private Image currentlyHighlightedSprite = null;
        private Color originalPlayerColor = Color.white;
        private Color originalEnemyColor = Color.white;
        private bool originalPlayerColorStored = false;
        private bool originalEnemyColorStored = false;
        private Tween highlightTween = null;
        
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
            
            // Store original sprite colors
            if (playerSprite != null)
            {
                originalPlayerColor = playerSprite.color;
                originalPlayerColorStored = true;
            }
            if (enemySprite != null)
            {
                originalEnemyColor = enemySprite.color;
                originalEnemyColorStored = true;
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
            
            // Highlight sprite based on card type
            if (handIndex >= 0 && handIndex < cardUIList.Count)
            {
                // Verify the card is actually selected
                CardUI selectedCard = cardUIList[handIndex];
                if (selectedCard != null && selectedCard.IsSelected())
                {
                    var hand = deckManager.GetHand();
                    if (handIndex < hand.Count && hand[handIndex] != null && hand[handIndex].cardData != null)
                    {
                        CardType cardType = hand[handIndex].cardData.cardType;
                        
                        // If card type is Attack, highlight enemy sprite
                        // Otherwise, highlight player sprite (default)
                        if (cardType == CardType.Attack)
                        {
                            HighlightSprite(enemySprite, ref originalEnemyColor);
                        }
                        else
                        {
                            HighlightSprite(playerSprite, ref originalPlayerColor);
                        }
                    }
                }
                else
                {
                    // Card was deselected, remove highlight
                    RemoveHighlight();
                }
            }
            else
            {
                // Invalid index, remove highlight
                RemoveHighlight();
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
            
            // Remove highlighting when card is played
            RemoveHighlight();
            
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
                
                // Check if any card is selected, remove highlight if none are selected
                bool anyCardSelected = false;
                foreach (var cardUI in cardUIList)
                {
                    if (cardUI != null && cardUI.IsSelected())
                    {
                        anyCardSelected = true;
                        break;
                    }
                }
                
                // If no cards are selected but we have a highlight, remove it
                if (!anyCardSelected && currentlyHighlightedSprite != null)
                {
                    RemoveHighlight();
                }
            }
        }
        
        /// <summary>
        /// Highlights a sprite based on card type selection.
        /// </summary>
        private void HighlightSprite(Image sprite, ref Color originalColor)
        {
            // Remove previous highlight first
            RemoveHighlight();
            
            if (sprite == null)
                return;
            
            // Store original color if not already stored
            if (sprite == playerSprite && !originalPlayerColorStored)
            {
                originalPlayerColor = sprite.color;
                originalPlayerColorStored = true;
                originalColor = originalPlayerColor;
            }
            else if (sprite == enemySprite && !originalEnemyColorStored)
            {
                originalEnemyColor = sprite.color;
                originalEnemyColorStored = true;
                originalColor = originalEnemyColor;
            }
            
            currentlyHighlightedSprite = sprite;
            
            // Kill any existing highlight tween
            if (highlightTween != null && highlightTween.IsActive())
            {
                highlightTween.Kill();
            }
            
            // Animate to highlight color
            highlightTween = sprite.DOColor(highlightColor, highlightDuration)
                .SetEase(Ease.OutQuad);
        }
        
        /// <summary>
        /// Removes highlighting from the currently highlighted sprite.
        /// </summary>
        private void RemoveHighlight()
        {
            if (currentlyHighlightedSprite == null)
                return;
            
            // Kill any existing highlight tween
            if (highlightTween != null && highlightTween.IsActive())
            {
                highlightTween.Kill();
            }
            
            // Restore original color
            Color colorToRestore = Color.white;
            if (currentlyHighlightedSprite == playerSprite)
            {
                colorToRestore = originalPlayerColor;
            }
            else if (currentlyHighlightedSprite == enemySprite)
            {
                colorToRestore = originalEnemyColor;
            }
            
            // Animate back to original color
            highlightTween = currentlyHighlightedSprite.DOColor(colorToRestore, highlightDuration)
                .SetEase(Ease.OutQuad);
            
            currentlyHighlightedSprite = null;
        }
        
        private void OnDestroy()
        {
            if (combatManager != null)
            {
                combatManager.OnPlayerTurnStart -= OnPlayerTurnStart;
                combatManager.OnEnemyTurnStart -= OnEnemyTurnStart;
                combatManager.OnStateChanged -= OnCombatStateChanged;
            }
            
            // Clean up highlight tween
            if (highlightTween != null && highlightTween.IsActive())
            {
                highlightTween.Kill();
            }
        }
    }
}

