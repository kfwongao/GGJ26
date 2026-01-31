using UnityEngine;
using MaskMYDrama.Core;

namespace MaskMYDrama.Cards
{
    /// <summary>
    /// Encore Card Action ScriptableObject.
    /// Represents one of the three special Encore actions that can be selected after achieving Encore status.
    /// 
    /// The three Encore actions are:
    /// 1. Randomly add 1 new card to draw pile
    /// 2. Shuffle entire draw pile
    /// 3. Add a copy of a random card from discard pile to draw pile
    /// </summary>
    [CreateAssetMenu(fileName = "New Encore Card Action", menuName = "MaskMYDrama/Encore Card Action")]
    public class EncoreCardAction : ScriptableObject
    {
        [Header("Action Info")]
        [Tooltip("Display name of the action (Chinese)")]
        public string actionName_CN;
        
        [Tooltip("Display name of the action (English)")]
        public string actionName_EN;
        
        [TextArea(3, 5)]
        [Tooltip("Description of what the action does (Chinese)")]
        public string description_CN;
        
        [TextArea(3, 5)]
        [Tooltip("Description of what the action does (English)")]
        public string description_EN;
        
        [Header("Action Type")]
        [Tooltip("Type of Encore action")]
        public EncoreActionType actionType;
        
        /// <summary>
        /// Gets the localized action name based on current language setting
        /// </summary>
        public string GetActionName()
        {
            // For now, return English. Can be extended to support localization
            return actionName_EN;
        }
        
        /// <summary>
        /// Gets the localized description based on current language setting
        /// </summary>
        public string GetDescription()
        {
            // For now, return English. Can be extended to support localization
            return description_EN;
        }
    }
    
    /// <summary>
    /// Types of Encore actions available
    /// </summary>
    public enum EncoreActionType
    {
        AddRandomNewCard,      // Randomly add 1 new card to draw pile
        ShuffleDrawPile,       // Shuffle entire draw pile
        CopyFromDiscardPile    // Add a copy of a random card from discard pile to draw pile
    }
}

