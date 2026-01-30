using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MaskMYDrama.Cards;
using MaskMYDrama.Core;

namespace MaskMYDrama.UI
{
    /// <summary>
    /// UI for roguelike card selection screen.
    /// 
    /// Implements the roguelike card selection system from CSV:
    /// - Player picks up one card within 3 cards to add one new into the card pool
    /// - Displays 3 random cards from CardDatabase
    /// - Player selects one card
    /// - Selected card is added to DeckManager's card pool
    /// 
    /// Based on CSV: "Roguelike - player pick up one card within 3 cards to add one new into the card pool"
    /// </summary>
    public class CardSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject selectionPanel;
        public Transform cardOptionsParent;
        public GameObject cardOptionPrefab;
        public TextMeshProUGUI titleText;
        public Button confirmButton;
        
        [Header("Managers")]
        public DeckManager deckManager;
        public CardDatabase cardDatabase;
        
        private List<Card> cardOptions = new List<Card>();
        private Card selectedCard;
        
        public System.Action<Card> OnCardSelected;
        
        private void Start()
        {
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(false);
            }
            
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmSelection);
            }
        }
        
        public void ShowCardSelection()
        {
            if (cardDatabase == null)
            {
                Debug.LogError("CardDatabase is not assigned!");
                return;
            }
            
            // Get 3 random cards
            cardOptions = cardDatabase.GetRandomCards(3);
            
            if (cardOptions.Count == 0)
            {
                Debug.LogWarning("No cards available in database!");
                return;
            }
            
            // Display cards
            DisplayCardOptions();
            
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(true);
            }
            
            selectedCard = null;
            UpdateConfirmButton();
        }
        
        private void DisplayCardOptions()
        {
            // Clear existing options
            foreach (Transform child in cardOptionsParent)
            {
                Destroy(child.gameObject);
            }
            
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
                    }
                }
            }
        }
        
        private void OnCardOptionSelected(int index)
        {
            if (index >= 0 && index < cardOptions.Count)
            {
                selectedCard = cardOptions[index];
                UpdateConfirmButton();
            }
        }
        
        private void UpdateConfirmButton()
        {
            if (confirmButton != null)
            {
                confirmButton.interactable = selectedCard != null;
            }
        }
        
        private void OnConfirmSelection()
        {
            if (selectedCard != null && deckManager != null)
            {
                deckManager.AddCardToPool(selectedCard);
                OnCardSelected?.Invoke(selectedCard);
                
                if (selectionPanel != null)
                {
                    selectionPanel.SetActive(false);
                }
            }
        }
        
        public void HideSelection()
        {
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(false);
            }
        }
    }
}

