using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MaskMYDrama.Cards;

namespace MaskMYDrama.UI
{
    /// <summary>
    /// Individual card display component for cards in hand.
    /// 
    /// Implements card interaction:
    /// - Visual display (name, description, energy cost)
    /// - Hover effects (highlight, scale up)
    /// - Click to play
    /// - Playable/unplayable state visualization
    /// 
    /// Based on CSV: Cards in hand are displayed and clickable.
    /// Player can only play cards if energy >= card cost.
    /// </summary>
    public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("UI References")]
        public TextMeshProUGUI cardNameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI energyCostText;
        public Image cardBackground;
        public Image cardTypeIcon;
        
        [Header("Visual States")]
        public Color normalColor = Color.white;
        public Color highlightColor = Color.yellow;
        public Color unplayableColor = Color.gray;
        
        private CardInstance cardInstance;
        private int handIndex;
        private bool isPlayable;
        
        public System.Action<int> OnCardClicked;
        
        public void SetupCard(CardInstance card, int index, bool playable)
        {
            cardInstance = card;
            handIndex = index;
            isPlayable = playable;
            
            UpdateDisplay();
            UpdatePlayableState(playable);
        }
        
        private void UpdateDisplay()
        {
            if (cardInstance == null || cardInstance.cardData == null)
                return;
            
            if (cardNameText != null)
                cardNameText.text = cardInstance.cardData.cardName;
            
            if (descriptionText != null)
            {
                string desc = cardInstance.cardData.description;
                // Add effect values to description
                if (cardInstance.cardData.attackValue > 0)
                    desc += $"\n攻击: {cardInstance.cardData.attackValue}";
                if (cardInstance.cardData.defenceValue > 0)
                    desc += $"\n防御: {cardInstance.cardData.defenceValue}";
                if (cardInstance.cardData.strengthValue > 0)
                    desc += $"\n力量: +{cardInstance.cardData.strengthValue}";
                descriptionText.text = desc;
            }
            
            if (energyCostText != null)
                energyCostText.text = cardInstance.GetEnergyCost().ToString();
        }
        
        public void UpdatePlayableState(bool playable)
        {
            isPlayable = playable;
            if (cardBackground != null)
            {
                cardBackground.color = playable ? normalColor : unplayableColor;
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (cardBackground != null && isPlayable)
            {
                cardBackground.color = highlightColor;
            }
            // Show tooltip or enlarge card
            transform.localScale = Vector3.one * 1.1f;
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (cardBackground != null)
            {
                cardBackground.color = isPlayable ? normalColor : unplayableColor;
            }
            transform.localScale = Vector3.one;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isPlayable)
            {
                OnCardClicked?.Invoke(handIndex);
            }
        }
    }
}

