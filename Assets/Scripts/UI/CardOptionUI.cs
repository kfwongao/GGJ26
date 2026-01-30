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
        private int index;
        private bool isSelected = false;
        
        public System.Action<int> OnCardSelected;
        
        public void SetupCard(Card cardData, int cardIndex)
        {
            card = cardData;
            index = cardIndex;
            isSelected = false;
            
            if (cardNameText != null)
                cardNameText.text = card.cardName;
            
            if (descriptionText != null)
            {
                string desc = card.description;
                if (card.attackValue > 0)
                    desc += $"\n攻击: {card.attackValue}";
                if (card.defenceValue > 0)
                    desc += $"\n防御: {card.defenceValue}";
                descriptionText.text = desc;
            }
            
            if (energyCostText != null)
                energyCostText.text = card.energyCost.ToString();
            
            UpdateVisualState();
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            isSelected = !isSelected;
            UpdateVisualState();
            OnCardSelected?.Invoke(index);
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

