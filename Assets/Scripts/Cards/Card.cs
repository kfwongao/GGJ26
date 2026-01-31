using UnityEngine;
using MaskMYDrama.Core;
using MaskMYDrama.Effects;
using UnityEngine.UI;

namespace MaskMYDrama.Cards
{
    /// <summary>
    /// Card data ScriptableObject. Represents a card's properties and effects.
    /// 
    /// This is the data container for cards. Cards are created as ScriptableObjects
    /// so they can be easily edited in the Unity Inspector without code changes.
    /// 
    /// Based on CSV requirements:
    /// - Card types: Attack, Defence, Strength, Function
    /// - Energy costs: 0 (Basic), 1 (Junior), 2-3 (Senior)
    /// - Effects: Attack value, Defence value, Strength value
    /// 
    /// Usage: Create via menu "MaskMYDrama/Card" or use CardDataCreator editor tool.
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "MaskMYDrama/Card")]
    public class Card : ScriptableObject
    {
        [Header("Card Info")]
        [Tooltip("Display name of the card")]
        public string cardName;
        
        [TextArea(3, 5)]
        [Tooltip("Description of what the card does")]
        public string description;
        
        [Tooltip("Type of card: Attack, Defence, Strength, or Function")]
        public CardType cardType;
        
        [Tooltip("Rarity level: Basic (0 energy), Junior (1 energy), Senior (2-3 energy)")]
        public CardRarity rarity;
        
        [Header("Cost")]
        [Tooltip("Energy cost to play this card (0-3 typically)")]
        public int energyCost;
        
        [Header("Effects")]
        [Tooltip("Damage dealt when played (for Attack cards)")]
        public int attackValue;
        
        [Tooltip("Defence/block points granted (for Defence cards)")]
        public int defenceValue;
        
        [Tooltip("Attack power increase (for Strength cards)")]
        public int strengthValue; // For strength cards
        
        [Header("Special Properties")]
        [Tooltip("If true, card is permanently removed after use (not sent to abandoned pile)")]
        public bool isExhaust; // Card is removed after use
        
        [Tooltip("Number of cards to draw when this card is played (0 = no draw)")]
        public int drawCardCount = 0; // Number of cards to draw when played
        
        [Header("Card Pool")]
        [Tooltip("Which pool this card belongs to: StartingPool or RoguelikePool")]
        public CardPoolType poolType = CardPoolType.StartingPool;
        
        [Header("Advanced Effects (Version 2.0)")]
        [Tooltip("Optional: Card effect configuration for complex effects. If set, this overrides simple attackValue/defenceValue.")]
        public CardEffect cardEffect;
        
        [Tooltip("If true, use cardEffect instead of legacy attackValue/defenceValue system")]
        public bool useAdvancedEffects = false;

        [Header("Card Image")]
        [Tooltip("The Image Sprite of that card")]
        public Sprite image; // Card is removed after use

        /// <summary>
        /// Returns the display name of the card.
        /// </summary>
        public string GetDisplayName()
        {
            return cardName;
        }
        
        /// <summary>
        /// Returns the card's description text.
        /// </summary>
        public string GetDescription()
        {
            return description;
        }
        
        /// <summary>
        /// Checks if the card can be played with the given energy amount.
        /// </summary>
        /// <param name="currentEnergy">Player's current energy</param>
        /// <returns>True if energy is sufficient</returns>
        public bool CanPlay(int currentEnergy)
        {
            return currentEnergy >= energyCost;
        }
    }
}

