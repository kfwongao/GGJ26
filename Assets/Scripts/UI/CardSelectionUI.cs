using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MaskMYDrama.Cards;
using MaskMYDrama.Core;
using DG.Tweening;

namespace MaskMYDrama.UI
{
    /// <summary>
    /// UI for roguelike card selection screen.
    /// 
    /// Implements the roguelike card selection system from CSV:
    /// - Player picks up one card within 3 cards to add one new into the card pool
    /// - Displays 3 random cards from CardDatabase in the center of screen
    /// - Player selects one card
    /// - Selected card is added to DeckManager's card pool
    /// - Then proceeds to next level
    /// 
    /// Based on CSV: "Roguelike - player pick up one card within 3 cards to add one new into the card pool"
    /// </summary>
    public class CardSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Main panel that contains all selection UI")]
        public GameObject selectionPanel;
        
        [Tooltip("Parent transform for card options (should be centered on screen)")]
        public Transform cardOptionsParent;
        
        [Tooltip("Prefab for individual card option display")]
        public GameObject cardOptionPrefab;
        
        [Tooltip("Title text (e.g., 'Choose a Card')")]
        public TextMeshProUGUI titleText;
        
        [Tooltip("Confirm button to proceed after selection")]
        public Button confirmButton;
        
        [Tooltip("Background overlay to dim the screen")]
        public Image backgroundOverlay;
        
        [Header("Layout Settings")]
        [Tooltip("Spacing between cards")]
        public float cardSpacing = 300f;
        
        [Tooltip("Scale of cards when not selected")]
        public float normalCardScale = 1f;
        
        [Tooltip("Scale of cards when selected")]
        public float selectedCardScale = 1.2f;
        
        [Tooltip("Animation duration for card selection")]
        public float selectionAnimationDuration = 0.3f;
        
        [Header("Managers")]
        public DeckManager deckManager;
        public CardDatabase cardDatabase;
        
        private List<Card> cardOptions = new List<Card>();
        private List<CardOptionUI> cardOptionUIs = new List<CardOptionUI>();
        private Card selectedCard;
        private int selectedIndex = -1;
        
        // Encore card action selection
        private List<EncoreCardAction> encoreCardActions = new List<EncoreCardAction>();
        private EncoreCardAction selectedEncoreAction;
        private int selectedEncoreIndex = -1;
        
        public System.Action<Card> OnCardSelected;
        public System.Action<EncoreCardAction> OnEncoreCardActionSelected;
        
        private void Start()
        {
            // Initialize UI state
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(false);
            }
            
            if (backgroundOverlay != null)
            {
                backgroundOverlay.gameObject.SetActive(false);
            }
            
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmSelection);
                confirmButton.interactable = false;
            }
            
            // Setup card options parent layout if needed
            SetupCardOptionsParent();
        }
        
        /// <summary>
        /// Sets up the card options parent with proper layout for center display
        /// </summary>
        private void SetupCardOptionsParent()
        {
            if (cardOptionsParent == null) return;
            
            // Add HorizontalLayoutGroup if not present
            HorizontalLayoutGroup layoutGroup = cardOptionsParent.GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup == null)
            {
                layoutGroup = cardOptionsParent.gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            
            layoutGroup.spacing = cardSpacing;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            
            // Center the parent on screen
            RectTransform rectTransform = cardOptionsParent.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        
        /// <summary>
        /// Shows the card selection UI with 3 random cards from Roguelike Pool
        /// </summary>
        public void ShowCardSelection()
        {
            if (cardDatabase == null)
            {
                Debug.LogError("CardDatabase is not assigned!");
                return;
            }
            
            // Clear Encore actions to ensure we're in regular card selection mode
            encoreCardActions.Clear();
            
            // Get 3 random cards from Roguelike Pool
            cardOptions = cardDatabase.GetRandomCards(3);
            
            if (cardOptions.Count == 0)
            {
                Debug.LogWarning("No cards available in Roguelike Pool! Proceeding to next level without selection.");
                // Still trigger the event so level can proceed
                OnCardSelected?.Invoke(null);
                return;
            }
            
            // Reset selection state
            selectedCard = null;
            selectedIndex = -1;
            
            // Show UI
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(true);
            }
            
            if (backgroundOverlay != null)
            {
                backgroundOverlay.gameObject.SetActive(true);
                // Fade in background
                backgroundOverlay.color = new Color(0, 0, 0, 0);
                backgroundOverlay.DOFade(0.7f, 0.3f);
            }
            
            // Update title
            if (titleText != null)
            {
                titleText.text = "选择一张卡牌加入牌组";
            }
            
            // Display cards with animation
            StartCoroutine(DisplayCardOptionsAnimated());
            
            UpdateConfirmButton();
        }
        
        /// <summary>
        /// Displays card options with entrance animation
        /// </summary>
        private IEnumerator DisplayCardOptionsAnimated()
        {
            // Clear existing options
            foreach (var optionUI in cardOptionUIs)
            {
                if (optionUI != null)
                {
                    Destroy(optionUI.gameObject);
                }
            }
            cardOptionUIs.Clear();
            
            // Wait a frame for cleanup
            yield return null;
            
            // Create card option UIs
            for (int i = 0; i < cardOptions.Count; i++)
            {
                if (cardOptionPrefab != null && cardOptions[i] != null)
                {
                    GameObject cardObj = Instantiate(cardOptionPrefab, cardOptionsParent);
                    
                    // Setup card display
                    CardOptionUI optionUI = cardObj.GetComponent<CardOptionUI>();
                    if (optionUI != null)
                    {
                        optionUI.SetupCard(cardOptions[i], i);
                        optionUI.OnCardSelected += OnCardOptionSelected;
                        cardOptionUIs.Add(optionUI);
                        
                        // Animate card entrance
                        RectTransform cardRect = cardObj.GetComponent<RectTransform>();
                        if (cardRect != null)
                        {
                            cardRect.localScale = Vector3.zero;
                            cardRect.DOScale(normalCardScale, 0.3f)
                                .SetDelay(i * 0.1f)
                                .SetEase(DG.Tweening.Ease.OutBack);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when a card option is clicked
        /// </summary>
        private void OnCardOptionSelected(int index)
        {
            if (index < 0 || index >= cardOptions.Count)
                return;
            
            // Deselect previous card
            if (selectedIndex >= 0 && selectedIndex < cardOptionUIs.Count)
            {
                CardOptionUI previousUI = cardOptionUIs[selectedIndex];
                if (previousUI != null)
                {
                    previousUI.SetSelected(false);
                    // Animate scale down
                    RectTransform prevRect = previousUI.GetComponent<RectTransform>();
                    if (prevRect != null)
                    {
                        prevRect.DOScale(normalCardScale, selectionAnimationDuration);
                    }
                }
            }
            
            // Select new card
            selectedIndex = index;
            selectedCard = cardOptions[index];
            
            // Animate selected card
            if (selectedIndex < cardOptionUIs.Count)
            {
                CardOptionUI selectedUI = cardOptionUIs[selectedIndex];
                if (selectedUI != null)
                {
                    selectedUI.SetSelected(true);
                    // Animate scale up
                    RectTransform selectedRect = selectedUI.GetComponent<RectTransform>();
                    if (selectedRect != null)
                    {
                        selectedRect.DOScale(selectedCardScale, selectionAnimationDuration)
                            .SetEase(DG.Tweening.Ease.OutBack);
                    }
                }
            }
            
            UpdateConfirmButton();
        }
        
        /// <summary>
        /// Called when confirm button is clicked (handles both regular and Encore selection)
        /// </summary>
        private void OnConfirmSelection()
        {
            // Check if we're in Encore mode
            if (encoreCardActions.Count > 0)
            {
                if (selectedEncoreAction == null)
                {
                    Debug.LogWarning("No Encore action selected!");
                    return;
                }
                
                // Hide UI with animation
                StartCoroutine(HideSelectionAnimated());
                
                // Trigger Encore action event
                OnEncoreCardActionSelected?.Invoke(selectedEncoreAction);
            }
            else
            {
                // Regular card selection
                if (selectedCard == null)
                {
                    Debug.LogWarning("No card selected!");
                    return;
                }
                
                // Hide UI with animation
                StartCoroutine(HideSelectionAnimated());
                
                // Trigger event (CombatManager will handle adding card to pool and level transition)
                OnCardSelected?.Invoke(selectedCard);
            }
        }
        
        /// <summary>
        /// Hides the selection UI with exit animation
        /// </summary>
        private IEnumerator HideSelectionAnimated()
        {
            // Animate cards out
            foreach (var optionUI in cardOptionUIs)
            {
                if (optionUI != null)
                {
                    RectTransform rect = optionUI.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        rect.DOScale(0f, 0.2f);
                    }
                }
            }
            
            // Fade out background
            if (backgroundOverlay != null)
            {
                backgroundOverlay.DOFade(0f, 0.3f);
            }
            
            yield return new WaitForSeconds(0.3f);
            
            // Hide panel
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(false);
            }
            
            if (backgroundOverlay != null)
            {
                backgroundOverlay.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Hides the selection UI immediately (no animation)
        /// </summary>
        public void HideSelection()
        {
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(false);
            }
            
            if (backgroundOverlay != null)
            {
                backgroundOverlay.gameObject.SetActive(false);
            }
            
            // Clear card options
            foreach (var optionUI in cardOptionUIs)
            {
                if (optionUI != null)
                {
                    Destroy(optionUI.gameObject);
                }
            }
            cardOptionUIs.Clear();
            
            // Clear selection state
            cardOptions.Clear();
            encoreCardActions.Clear();
            selectedCard = null;
            selectedEncoreAction = null;
            selectedIndex = -1;
            selectedEncoreIndex = -1;
        }
        
        /// <summary>
        /// Shows the Encore card action selection UI with 3 Encore actions.
        /// </summary>
        public void ShowEncoreCardSelection(EncoreCardAction[] actions)
        {
            if (actions == null)
            {
                Debug.LogError("EncoreCardActions array is null! Please assign 3 EncoreCardAction ScriptableObjects in CombatManager.");
                return;
            }
            
            if (actions.Length != 3)
            {
                Debug.LogError($"EncoreCardActions array must contain exactly 3 actions! Current length: {actions.Length}");
                return;
            }
            
            // Check if any actions are null
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] == null)
                {
                    Debug.LogError($"EncoreCardAction at index {i} is null! Please assign all 3 EncoreCardAction ScriptableObjects in CombatManager.");
                    return;
                }
            }
            
            // Store Encore actions
            encoreCardActions = new List<EncoreCardAction>(actions);
            
            // Reset selection state
            selectedEncoreAction = null;
            selectedEncoreIndex = -1;
            
            // Show UI
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(true);
            }
            
            if (backgroundOverlay != null)
            {
                backgroundOverlay.gameObject.SetActive(true);
                // Fade in background
                backgroundOverlay.color = new Color(0, 0, 0, 0);
                backgroundOverlay.DOFade(0.7f, 0.3f);
            }
            
            // Update title for Encore
            if (titleText != null)
            {
                titleText.text = "安可/Encore\n选择其中一张卡牌加入卡池\nChoose one card and add it to your draw pile.";
            }
            
            // Display Encore actions with animation
            StartCoroutine(DisplayEncoreActionsAnimated());
            
            UpdateConfirmButton();
        }
        
        /// <summary>
        /// Displays Encore card actions with entrance animation
        /// </summary>
        private IEnumerator DisplayEncoreActionsAnimated()
        {
            // Clear existing options
            foreach (var optionUI in cardOptionUIs)
            {
                if (optionUI != null)
                {
                    Destroy(optionUI.gameObject);
                }
            }
            cardOptionUIs.Clear();
            
            // Clear regular card options to ensure we're in Encore mode
            cardOptions.Clear();
            
            // Wait a frame for cleanup
            yield return null;
            
            // Create Encore action option UIs
            for (int i = 0; i < encoreCardActions.Count; i++)
            {
                if (cardOptionPrefab == null)
                {
                    Debug.LogError("CardOptionPrefab is not assigned in CardSelectionUI!");
                    continue;
                }
                
                if (encoreCardActions[i] == null)
                {
                    Debug.LogError($"EncoreCardAction at index {i} is null! Skipping display.");
                    continue;
                }
                
                GameObject actionObj = Instantiate(cardOptionPrefab, cardOptionsParent);
                
                // Setup Encore action display using CardOptionUI
                CardOptionUI optionUI = actionObj.GetComponent<CardOptionUI>();
                if (optionUI != null)
                {
                    // Use the new SetupEncoreAction method
                    optionUI.SetupEncoreAction(encoreCardActions[i], i);
                    optionUI.OnCardSelected += OnEncoreActionSelected;
                    cardOptionUIs.Add(optionUI);
                    
                    // Animate card entrance
                    RectTransform actionRect = actionObj.GetComponent<RectTransform>();
                    if (actionRect != null)
                    {
                        actionRect.localScale = Vector3.zero;
                        actionRect.DOScale(normalCardScale, 0.3f)
                            .SetDelay(i * 0.1f)
                            .SetEase(DG.Tweening.Ease.OutBack);
                    }
                }
                else
                {
                    Debug.LogError($"CardOptionUI component not found on prefab {cardOptionPrefab.name}! Make sure the prefab has a CardOptionUI component.");
                }
            }
            
            // Log if no cards were created
            if (cardOptionUIs.Count == 0)
            {
                Debug.LogError("No Encore action cards were created! Check that cardOptionPrefab is assigned and has CardOptionUI component.");
            }
            else
            {
                Debug.Log($"Successfully created {cardOptionUIs.Count} Encore action cards for selection.");
            }
        }
        
        /// <summary>
        /// Called when an Encore action is clicked
        /// </summary>
        private void OnEncoreActionSelected(int index)
        {
            if (index < 0 || index >= encoreCardActions.Count)
                return;
            
            // Deselect previous action
            if (selectedEncoreIndex >= 0 && selectedEncoreIndex < cardOptionUIs.Count)
            {
                CardOptionUI previousUI = cardOptionUIs[selectedEncoreIndex];
                if (previousUI != null)
                {
                    previousUI.SetSelected(false);
                    // Animate scale down
                    RectTransform prevRect = previousUI.GetComponent<RectTransform>();
                    if (prevRect != null)
                    {
                        prevRect.DOScale(normalCardScale, selectionAnimationDuration);
                    }
                }
            }
            
            // Select new action
            selectedEncoreIndex = index;
            selectedEncoreAction = encoreCardActions[index];
            
            // Animate selected action
            if (selectedEncoreIndex < cardOptionUIs.Count)
            {
                CardOptionUI selectedUI = cardOptionUIs[selectedEncoreIndex];
                if (selectedUI != null)
                {
                    selectedUI.SetSelected(true);
                    // Animate scale up
                    RectTransform selectedRect = selectedUI.GetComponent<RectTransform>();
                    if (selectedRect != null)
                    {
                        selectedRect.DOScale(selectedCardScale, selectionAnimationDuration)
                            .SetEase(DG.Tweening.Ease.OutBack);
                    }
                }
            }
            
            UpdateConfirmButton();
        }
        
        /// <summary>
        /// Updates the confirm button state (for both regular and Encore selection)
        /// </summary>
        private void UpdateConfirmButton()
        {
            if (confirmButton != null)
            {
                // Check if we're in Encore mode or regular mode
                if (encoreCardActions.Count > 0)
                {
                    confirmButton.interactable = selectedEncoreAction != null;
                }
                else
                {
                    confirmButton.interactable = selectedCard != null;
                }
            }
        }
    }
}

