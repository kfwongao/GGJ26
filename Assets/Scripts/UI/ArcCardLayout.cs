using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MaskMYDrama.UI
{
    /// <summary>
    /// Arranges card UI elements in a parabolic curve layout.
    /// Cards are positioned along a parabolic curve with spacing between them.
    /// </summary>
    public class ArcCardLayout : MonoBehaviour
    {
        [Header("Parabolic Curve Settings")]
        [Tooltip("Spacing between cards horizontally")]
        public float cardSpacing = 120f;
        
        [Tooltip("Height of the parabolic curve (how much the center cards are raised)")]
        public float curveHeight = 60f;
        
        [Tooltip("Width factor for the curve (controls how wide the curve spreads)")]
        [Range(0.1f, 2f)]
        public float curveWidth = 2f;
        
        [Tooltip("Vertical offset from center (positive = up, negative = down)")]
        public float verticalOffset = 0f;
        
        [Tooltip("Horizontal offset from center (positive = right, negative = left)")]
        public float horizontalOffset = 0f;
        
        [Header("Card Rotation")]
        [Tooltip("Should cards rotate to follow the curve tangent?")]
        public bool rotateCards = true;
        
        [Tooltip("Maximum rotation angle in degrees (cards at edges will rotate more)")]
        [Range(0f, 45f)]
        public float maxRotation = 15f;
        
        [Tooltip("Additional rotation offset in degrees")]
        public float rotationOffset = 0f;
        
        [Header("Animation")]
        [Tooltip("Animate cards into position when layout changes")]
        public bool animateLayout = true;
        
        [Tooltip("Animation duration in seconds")]
        public float animationDuration = 0.3f;
        
        private List<RectTransform> cardTransforms = new List<RectTransform>();
        private Coroutine layoutCoroutine;
        
        /// <summary>
        /// Updates the layout of all child cards in an arc formation.
        /// </summary>
        /// <param name="immediate">If true, positions are set immediately without animation</param>
        public void UpdateLayout(bool immediate = false)
        {
            // Stop any running animation coroutines to prevent accessing destroyed objects
            if (layoutCoroutine != null)
            {
                StopCoroutine(layoutCoroutine);
                layoutCoroutine = null;
            }
            
            // Get all card transforms
            cardTransforms.Clear();
            foreach (Transform child in transform)
            {
                RectTransform rect = child.GetComponent<RectTransform>();
                if (rect != null)
                {
                    cardTransforms.Add(rect);
                }
            }
            
            if (cardTransforms.Count == 0)
                return;
            
            // Calculate positions
            if (immediate)
            {
                CalculateArcPositionsImmediate();
            }
            else
            {
                CalculateArcPositions();
            }
        }
        
        /// <summary>
        /// Calculates and applies parabolic curve positions to all cards.
        /// </summary>
        private void CalculateArcPositions()
        {
            int cardCount = cardTransforms.Count;
            
            if (cardCount == 0)
                return;
            
            if (cardCount == 1)
            {
                // Single card: place at center
                Vector3 centerPos = new Vector3(horizontalOffset, verticalOffset + curveHeight, 0);
                SetCardPosition(0, centerPos, rotationOffset);
                return;
            }
            
            // Calculate total width needed
            float totalWidth = (cardCount - 1) * cardSpacing;
            float halfWidth = totalWidth * 0.5f;
            
            // Calculate the parabolic curve coefficient
            // Using y = curveHeight - a * x^2, where a = curveHeight / (halfWidth^2)
            // This creates an inverted parabola with peak at center (x=0)
            float curveCoefficient = curveHeight / (halfWidth * halfWidth * curveWidth);
            
            // Position each card along the parabolic curve
            for (int i = 0; i < cardCount; i++)
            {
                // Skip if card has been destroyed
                if (i >= cardTransforms.Count || cardTransforms[i] == null)
                    continue;
                
                // Calculate horizontal position (centered around 0)
                float normalizedPos = (float)i / (cardCount - 1); // 0 to 1
                float x = Mathf.Lerp(-halfWidth, halfWidth, normalizedPos);
                
                // Calculate vertical position using parabolic equation: y = curveHeight - a * x^2
                // This makes center cards higher (y = curveHeight) and edge cards lower (y approaches 0)
                float y = curveHeight - (curveCoefficient * x * x);
                
                Vector3 position = new Vector3(
                    x + horizontalOffset,
                    y + verticalOffset,
                    0
                );
                
                // Calculate rotation based on curve tangent
                float rotation = 0f;
                if (rotateCards)
                {
                    // Calculate derivative at this point: dy/dx = -2 * a * x
                    // This gives us the slope of the tangent line
                    float slope = -2f * curveCoefficient * x;
                    // Convert slope to angle (atan gives angle in radians)
                    float angleRad = Mathf.Atan(slope);
                    float angleDeg = angleRad * Mathf.Rad2Deg;
                    
                    // Clamp rotation to maxRotation
                    rotation = Mathf.Clamp(angleDeg, -maxRotation, maxRotation);
                }
                rotation += rotationOffset;
                
                SetCardPosition(i, position, rotation);
            }
        }
        
        /// <summary>
        /// Calculates and immediately applies parabolic curve positions without animation.
        /// </summary>
        private void CalculateArcPositionsImmediate()
        {
            int cardCount = cardTransforms.Count;
            
            if (cardCount == 0)
                return;
            
            if (cardCount == 1)
            {
                // Single card: place at center
                Vector3 centerPos = new Vector3(horizontalOffset, verticalOffset + curveHeight, 0);
                if (cardTransforms[0] != null)
                {
                    cardTransforms[0].localPosition = centerPos;
                    cardTransforms[0].localRotation = Quaternion.Euler(0, 0, rotationOffset);
                }
                return;
            }
            
            // Calculate total width needed
            float totalWidth = (cardCount - 1) * cardSpacing;
            float halfWidth = totalWidth * 0.5f;
            
            // Calculate the parabolic curve coefficient
            // Using y = curveHeight - a * x^2, where a = curveHeight / (halfWidth^2)
            // This creates an inverted parabola with peak at center (x=0)
            float curveCoefficient = curveHeight / (halfWidth * halfWidth * curveWidth);
            
            // Position each card along the parabolic curve
            for (int i = 0; i < cardCount; i++)
            {
                if (cardTransforms[i] == null) continue;
                
                // Calculate horizontal position (centered around 0)
                float normalizedPos = (float)i / (cardCount - 1); // 0 to 1
                float x = Mathf.Lerp(-halfWidth, halfWidth, normalizedPos);
                
                // Calculate vertical position using parabolic equation: y = curveHeight - a * x^2
                // This makes center cards higher (y = curveHeight) and edge cards lower (y approaches 0)
                float y = curveHeight - (curveCoefficient * x * x);
                
                Vector3 position = new Vector3(
                    x + horizontalOffset,
                    y + verticalOffset,
                    0
                );
                
                // Calculate rotation based on curve tangent
                float rotation = 0f;
                if (rotateCards)
                {
                    // Calculate derivative at this point: dy/dx = -2 * a * x
                    // This gives us the slope of the tangent line
                    float slope = -2f * curveCoefficient * x;
                    // Convert slope to angle (atan gives angle in radians)
                    float angleRad = Mathf.Atan(slope);
                    float angleDeg = angleRad * Mathf.Rad2Deg;
                    
                    // Clamp rotation to maxRotation
                    rotation = Mathf.Clamp(angleDeg, -maxRotation, maxRotation);
                }
                rotation += rotationOffset;
                
                cardTransforms[i].localPosition = position;
                cardTransforms[i].localRotation = Quaternion.Euler(0, 0, rotation);
            }
        }
        
        /// <summary>
        /// Sets the position and rotation of a card.
        /// </summary>
        private void SetCardPosition(int index, Vector3 position, float rotation)
        {
            if (index < 0 || index >= cardTransforms.Count)
                return;
            
            RectTransform cardRect = cardTransforms[index];
            
            // Check if card has been destroyed
            if (cardRect == null)
                return;
            
            if (animateLayout && Application.isPlaying)
            {
                // Animate to position
                if (layoutCoroutine != null)
                {
                    StopCoroutine(layoutCoroutine);
                }
                layoutCoroutine = StartCoroutine(AnimateCardToPosition(cardRect, position, rotation));
            }
            else
            {
                // Set immediately (with null check)
                if (cardRect != null)
                {
                    cardRect.localPosition = position;
                    cardRect.localRotation = Quaternion.Euler(0, 0, rotation);
                }
            }
        }
        
        /// <summary>
        /// Animates a card to its target position and rotation.
        /// </summary>
        private IEnumerator AnimateCardToPosition(RectTransform cardRect, Vector3 targetPosition, float targetRotation)
        {
            // Check if card is already destroyed
            if (cardRect == null)
                yield break;
            
            Vector3 startPosition = cardRect.localPosition;
            Quaternion startRotation = cardRect.localRotation;
            Quaternion targetRotationQuat = Quaternion.Euler(0, 0, targetRotation);
            
            float elapsed = 0f;
            
            while (elapsed < animationDuration)
            {
                // Check if card has been destroyed during animation
                if (cardRect == null)
                    yield break;
                
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                
                // Smooth interpolation (ease out)
                t = 1f - Mathf.Pow(1f - t, 3f);
                
                // Double-check before accessing (card might be destroyed between checks)
                if (cardRect != null)
                {
                    cardRect.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                    cardRect.localRotation = Quaternion.Lerp(startRotation, targetRotationQuat, t);
                }
                else
                {
                    yield break;
                }
                
                yield return null;
            }
            
            // Ensure final position (with null check)
            if (cardRect != null)
            {
                cardRect.localPosition = targetPosition;
                cardRect.localRotation = targetRotationQuat;
            }
        }
        
        /// <summary>
        /// Called when a child is added or removed.
        /// </summary>
        private void OnTransformChildrenChanged()
        {
            UpdateLayout();
        }
        
        /// <summary>
        /// Called in editor to preview layout changes.
        /// </summary>
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateLayout();
            }
        }
    }
}

