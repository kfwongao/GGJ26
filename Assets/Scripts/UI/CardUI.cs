using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MaskMYDrama.Cards;
using DG.Tweening;

namespace MaskMYDrama.UI
{
    /// <summary>
    /// Individual card display component for cards in hand.
    /// 
    /// Implements card interaction:
    /// - Visual display (name, description, energy cost)
    /// - Hover effects (highlight, scale up)
    /// - Click to select: Card is selected on mouse click and brought to front
    /// - Drag to play: Drag selected card to left (player) or right (enemy) side of screen center
    /// - Playable/unplayable state visualization
    /// - Graceful selection animations (move up, enlarge, shader effects)
    /// 
    /// Based on CSV: Cards in hand are displayed and clickable.
    /// Player can only play cards if energy >= card cost.
    /// </summary>
    public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IEndDragHandler
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
        public Color selectedColor = new Color(1f, 0.8f, 0.2f, 1f); // Golden highlight for selected card
        
        [Header("Animation Settings")]
        [Tooltip("How much the card moves up when selected (in local Y units)")]
        public float selectedYOffset = 50f;
        [Tooltip("Scale multiplier when card is selected")]
        public float selectedScale = 1.25f;
        [Tooltip("Hover scale multiplier")]
        public float hoverScale = 1.1f;
        [Tooltip("Animation duration for selection")]
        public float selectionDuration = 0.3f;
        [Tooltip("Animation easing type")]
        public Ease selectionEase = Ease.OutBack;
        
        [Header("Drag Settings")]
        [Tooltip("Canvas reference for screen space calculations (auto-detected if null)")]
        public Canvas canvas;
        [Tooltip("Offset from cursor position when dragging")]
        public Vector2 dragOffset = Vector2.zero;
        [Tooltip("Dead zone around screen center (0.0 = no dead zone, 0.1 = 10% of screen width)")]
        [Range(0f, 0.2f)]
        public float centerDeadZone = 0.05f;
        
        [Header("Shader Settings")]
        [Tooltip("Material with shader that supports UV time speed (optional)")]
        public Material cardMaterial;
        [Tooltip("Shader property name for UV time speed (e.g., _Main_UV, _Tex_2_UV)")]
        public string uvSpeedPropertyName = "_Main_UV";
        [Tooltip("Normal UV speed (Vector4: x, y, z, w)")]
        public Vector4 normalUVSpeed = Vector4.zero;
        [Tooltip("Selected UV speed (faster animation when selected)")]
        public Vector4 selectedUVSpeed = new Vector4(0.5f, 0.5f, 0f, 0f);
        
        private CardInstance cardInstance;
        private int handIndex;
        private bool isPlayable;
        private bool isSelected = false;
        private bool isDragging = false;
        
        // Store original state
        private Vector3 originalPosition;
        private Vector3 originalScale;
        private Material originalMaterial;
        private Material instanceMaterial;
        
        // Drag state
        private RectTransform rectTransform;
        private Canvas parentCanvas;
        private Transform originalParent;
        private int originalSiblingIndex;
        
        // DOTween sequences
        private Sequence selectionSequence;
        private Tween hoverTween;
        
        public System.Action<int> OnCardClicked;
        public System.Action<int> OnCardSelected;
        
        private void Awake()
        {
            // Get RectTransform
            rectTransform = GetComponent<RectTransform>();
            
            // Find canvas if not assigned
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }
            parentCanvas = canvas;
            
            // Create material instance if material is assigned
            if (cardBackground != null && cardBackground.material != null)
            {
                originalMaterial = cardBackground.material;
                instanceMaterial = new Material(originalMaterial);
                cardBackground.material = instanceMaterial;
                cardMaterial = instanceMaterial;
            }
            else if (cardMaterial != null)
            {
                instanceMaterial = new Material(cardMaterial);
                if (cardBackground != null)
                {
                    cardBackground.material = instanceMaterial;
                }
            }
        }
        
        private void Start()
        {
            // Store original state after layout group has positioned the card
            // Use a coroutine to wait one frame for layout to complete
            StartCoroutine(StoreOriginalStateDelayed());
        }
        
        private System.Collections.IEnumerator StoreOriginalStateDelayed()
        {
            // Wait for layout (arc layout or horizontal layout) to position the card
            // Wait 2 frames to ensure arc layout has positioned the card
            yield return null;
            yield return null;
            originalPosition = transform.localPosition;
            originalScale = transform.localScale;
        }
        
        private void OnDestroy()
        {
            // Clean up tweens
            if (selectionSequence != null && selectionSequence.IsActive())
            {
                selectionSequence.Kill();
            }
            if (hoverTween != null && hoverTween.IsActive())
            {
                hoverTween.Kill();
            }
            
            // Clean up material instance
            if (instanceMaterial != null)
            {
                Destroy(instanceMaterial);
            }
        }
        
        public void SetupCard(CardInstance card, int index, bool playable)
        {
            cardInstance = card;
            handIndex = index;
            isPlayable = playable;
            
            // Store original state if not already stored (in case SetupCard is called before Start)
            if (originalScale == Vector3.zero)
            {
                originalPosition = transform.localPosition;
                originalScale = transform.localScale;
            }
            else
            {
                // Reset to original state
                ResetToOriginalState();
            }
            
            UpdateDisplay();
            UpdatePlayableState(playable);
        }
        
        private void ResetToOriginalState()
        {
            // Kill any active animations
            if (selectionSequence != null && selectionSequence.IsActive())
            {
                selectionSequence.Kill();
            }
            if (hoverTween != null && hoverTween.IsActive())
            {
                hoverTween.Kill();
            }
            
            // Reset transform
            transform.localPosition = originalPosition;
            transform.localScale = originalScale;
            
            // Reset material shader parameters
            if (instanceMaterial != null && !string.IsNullOrEmpty(uvSpeedPropertyName))
            {
                if (instanceMaterial.HasProperty(uvSpeedPropertyName))
                {
                    instanceMaterial.SetVector(uvSpeedPropertyName, normalUVSpeed);
                }
            }
            
            // Reset color
            if (cardBackground != null)
            {
                cardBackground.color = isPlayable ? normalColor : unplayableColor;
            }
            
            isSelected = false;
            isDragging = false;
        }
        
        private void UpdateDisplay()
        {
            if (cardInstance == null || cardInstance.cardData == null)
                return;
            
            if(cardBackground != null)
            {
                cardBackground.sprite = cardInstance.cardData.image;
            }

            if (cardTypeIcon != null)
            {
                cardTypeIcon.sprite = cardInstance.cardData.image;
            }

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
            // Only show hover effect if not selected and card is playable
            if (!isSelected && isPlayable)
            {
                if (cardBackground != null)
                {
                    cardBackground.color = highlightColor;
                }
                
                // Animate hover scale
                if (hoverTween != null && hoverTween.IsActive())
                {
                    hoverTween.Kill();
                }
                hoverTween = transform.DOScale(originalScale * hoverScale, 0.2f)
                    .SetEase(Ease.OutQuad);
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            // Only reset hover if not selected
            if (!isSelected)
            {
                if (cardBackground != null)
                {
                    cardBackground.color = isPlayable ? normalColor : unplayableColor;
                }
                
                // Animate back to original scale
                if (hoverTween != null && hoverTween.IsActive())
                {
                    hoverTween.Kill();
                }
                hoverTween = transform.DOScale(originalScale, 0.2f)
                    .SetEase(Ease.OutQuad);
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            // PC-based card selection: Click to select/deselect card
            if (!isPlayable)
                return;
            
            if (isSelected)
            {
                // If already selected, deselect it
                DeselectCard();
                OnCardSelected?.Invoke(-1); // -1 indicates deselection
            }
            else
            {
                // Select this card and bring it to front
                SelectCard();
                OnCardSelected?.Invoke(handIndex);
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            // Only allow dragging if this card is selected and playable
            if (!isSelected || !isPlayable) return;
            
            isDragging = true;
            
            // Kill any position animations while dragging
            if (selectionSequence != null && selectionSequence.IsActive())
            {
                selectionSequence.Kill();
            }
            
            // Convert screen position to world position for the card
            Vector3 worldPoint;
            RectTransform parentRect = parentCanvas.transform as RectTransform;
            
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                parentRect,
                eventData.position + dragOffset,
                parentCanvas.worldCamera,
                out worldPoint))
            {
                // Move card to follow cursor
                transform.position = worldPoint;
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            // Only process if this card was actually being dragged and is selected
            if (!isDragging || !isSelected) return;
            
            isDragging = false;
            
            // Check if card is dropped in playable area (left or right of center)
            // Use eventData.position directly to ensure accuracy even after resolution changes
            DropArea dropArea = GetDropArea(eventData.position);
            if (dropArea != DropArea.None)
            {
                // Card is dropped in a valid play area - play it
                OnCardClicked?.Invoke(handIndex);
            }
            else
            {
                // Card is dropped in invalid area - return to original position
                DeselectCard();
            }
        }
        
        /// <summary>
        /// Determines which drop area the card is currently in based on screen position.
        /// </summary>
        /// <param name="screenPosition">The screen position where the drop occurred (from eventData.position)</param>
        /// <returns>DropArea enum indicating player area (left), enemy area (right), or none</returns>
        private DropArea GetDropArea(Vector2 screenPosition)
        {
            // Use current screen dimensions (always up-to-date, even after resolution changes)
            float screenCenterX = Screen.width * 0.5f;
            float deadZoneWidth = Screen.width * centerDeadZone;
            
            // Check if drop position is in dead zone (center area)
            float leftBoundary = screenCenterX - deadZoneWidth;
            float rightBoundary = screenCenterX + deadZoneWidth;
            
            if (screenPosition.x >= leftBoundary && screenPosition.x <= rightBoundary)
            {
                // Card is in dead zone (center) - don't play
                return DropArea.None;
            }
            else if (screenPosition.x < leftBoundary)
            {
                // Card is left of center - player area
                return DropArea.Player;
            }
            else
            {
                // Card is right of center - enemy area
                return DropArea.Enemy;
            }
        }
        
        /// <summary>
        /// Enum for drop areas on the screen.
        /// </summary>
        private enum DropArea
        {
            None,   // Center dead zone or invalid area
            Player, // Left of center
            Enemy   // Right of center
        }
        
        private void SelectCard()
        {
            if (isSelected) return;
            
            isSelected = true;
            
            // Store parent info for restoring position later
            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex();
            
            // Move card to canvas root to allow free dragging and bring to front (top of deck)
            if (parentCanvas != null)
            {
                transform.SetParent(parentCanvas.transform, true);
                transform.SetAsLastSibling(); // Bring to front (top of deck)
            }
            
            // Kill any existing animations
            if (selectionSequence != null && selectionSequence.IsActive())
            {
                selectionSequence.Kill();
            }
            if (hoverTween != null && hoverTween.IsActive())
            {
                hoverTween.Kill();
            }
            
            // Create selection animation sequence
            selectionSequence = DOTween.Sequence();
            
            // Calculate target position in world space
            // First get the world position of the original position
            Vector3 originalWorldPos = originalParent != null 
                ? originalParent.TransformPoint(originalPosition) 
                : originalPosition;
            
            // Calculate offset in world space (convert local up to world up)
            Vector3 worldUp = originalParent != null 
                ? originalParent.TransformDirection(Vector3.up) 
                : Vector3.up;
            Vector3 targetPosition = originalWorldPos + worldUp * selectedYOffset;
            
            // Move card up (but we'll allow dragging to override this)
            selectionSequence.Append(transform.DOMove(targetPosition, selectionDuration)
                .SetEase(selectionEase));
            
            // Enlarge card
            selectionSequence.Join(transform.DOScale(originalScale * selectedScale, selectionDuration)
                .SetEase(selectionEase));
            
            // Change color to selected color
            if (cardBackground != null)
            {
                selectionSequence.Join(cardBackground.DOColor(selectedColor, selectionDuration)
                    .SetEase(Ease.OutQuad));
            }
            
            // Modify shader UV time speed
            if (instanceMaterial != null && !string.IsNullOrEmpty(uvSpeedPropertyName))
            {
                if (instanceMaterial.HasProperty(uvSpeedPropertyName))
                {
                    selectionSequence.Join(DOTween.To(
                        () => instanceMaterial.GetVector(uvSpeedPropertyName),
                        x => instanceMaterial.SetVector(uvSpeedPropertyName, x),
                        selectedUVSpeed,
                        selectionDuration
                    ).SetEase(Ease.OutQuad));
                }
            }
            
            selectionSequence.Play();
        }
        
        public void DeselectCard()
        {
            if (!isSelected) return;
            
            isSelected = false;
            isDragging = false;
            
            // Kill any existing animations
            if (selectionSequence != null && selectionSequence.IsActive())
            {
                selectionSequence.Kill();
            }
            if (hoverTween != null && hoverTween.IsActive())
            {
                hoverTween.Kill();
            }
            
            // Create deselection animation sequence
            selectionSequence = DOTween.Sequence();
            
            // Return card to original parent first (before animating)
            if (originalParent != null)
            {
                transform.SetParent(originalParent, true);
                transform.SetSiblingIndex(originalSiblingIndex);
            }
            
            // Move card back to original position (now in local space of original parent)
            selectionSequence.Append(transform.DOLocalMove(originalPosition, selectionDuration)
                .SetEase(Ease.InBack));
            
            // Scale back to original
            selectionSequence.Join(transform.DOScale(originalScale, selectionDuration)
                .SetEase(Ease.InBack));
            
            // Reset color
            if (cardBackground != null)
            {
                selectionSequence.Join(cardBackground.DOColor(
                    isPlayable ? normalColor : unplayableColor,
                    selectionDuration)
                    .SetEase(Ease.OutQuad));
            }
            
            // Reset shader UV time speed
            if (instanceMaterial != null && !string.IsNullOrEmpty(uvSpeedPropertyName))
            {
                if (instanceMaterial.HasProperty(uvSpeedPropertyName))
                {
                    selectionSequence.Join(DOTween.To(
                        () => instanceMaterial.GetVector(uvSpeedPropertyName),
                        x => instanceMaterial.SetVector(uvSpeedPropertyName, x),
                        normalUVSpeed,
                        selectionDuration
                    ).SetEase(Ease.OutQuad));
                }
            }
            
            selectionSequence.Play();
        }
        
        public bool IsSelected()
        {
            return isSelected;
        }
    }
}

