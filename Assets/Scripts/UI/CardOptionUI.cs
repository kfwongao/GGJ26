using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MaskMYDrama.Cards;

namespace MaskMYDrama.UI
{
    /// <summary>
    /// Individual card option display in the card selection screen.
    /// 
    /// Shows a card that can be selected in the roguelike card selection.
    /// Provides visual feedback when selected (color change).
    /// 
    /// Used by CardSelectionUI to display the 3 card options.
    /// </summary>
    public class CardOptionUI : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI References")]
        public TextMeshProUGUI cardNameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI energyCostText;
        public Image cardBackground;
        
        [Header("Selection")]
        public Color normalColor = Color.white;
        public Color selectedColor = Color.green;
        
        private Card card;
        private EncoreCardAction encoreAction;
        private int index;
        private bool isSelected = false;
        private bool isEncoreMode = false;
        
        public System.Action<int> OnCardSelected;
        
        public void SetupCard(Card cardData, int cardIndex)
        {
            card = cardData;
            encoreAction = null;
            index = cardIndex;
            isSelected = false;
            isEncoreMode = false;
            
            if (cardNameText != null)
                cardNameText.text = card.cardName;
            
            if (descriptionText != null)
            {
                string desc = card.description;
                if (card.attackValue > 0)
                    desc += $"\n攻击: {card.attackValue}";
                if (card.defenceValue > 0)
                    desc += $"\n防御: {card.defenceValue}";
                if (card.strengthValue > 0)
                    desc += $"\n力量: +{card.strengthValue}";
                descriptionText.text = desc;
            }
            
            if (energyCostText != null)
                energyCostText.text = card.energyCost.ToString();
            
            UpdateVisualState();
        }
        
        /// <summary>
        /// Sets up the UI to display an Encore card action
        /// </summary>
        public void SetupEncoreAction(EncoreCardAction action, int actionIndex)
        {
            encoreAction = action;
            card = null;
            index = actionIndex;
            isSelected = false;
            isEncoreMode = true;
            
            if (cardNameText != null)
                cardNameText.text = encoreAction.GetActionName();
            
            if (descriptionText != null)
                descriptionText.text = encoreAction.GetDescription();
            
            // Hide or clear energy cost for Encore actions (they don't have energy costs)
            if (energyCostText != null)
                energyCostText.text = ""; // Or you could hide the text component
            
            UpdateVisualState();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            // Always select (don't toggle) - only one card can be selected
            if (!isSelected)
            {
                SetSelected(true);
                OnCardSelected?.Invoke(index);
            }
        }
        
        /// <summary>
        /// Sets the selected state of this card
        /// </summary>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateVisualState();
        }
        
        private void UpdateVisualState()
        {
            if (cardBackground != null)
            {
                cardBackground.color = isSelected ? selectedColor : normalColor;
            }
        }
    }
}

