using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MaskMYDrama.Cards;

namespace MaskMYDrama.Core
{
    /// <summary>
    /// Manages available cards for roguelike selection and starting deck.
    /// 
    /// Based on CSV: Cards are separated into:
    /// - Starting Pool (开局 in card pool): Cards that start in the deck
    /// - Roguelike Pool: Cards that can be added through roguelike selection
    /// </summary>
    [CreateAssetMenu(fileName = "CardDatabaseList", menuName = "MaskMYDrama/Card CardDatabaseList")]
    public class CardDatabaseList : ScriptableObject
    {
        [Header("Starting CardDatabase (开局 in CardDatabase pool)")]
        [Tooltip("CardDatabase")]
        public List<CardDatabase> cardDatabaseList;
        
        public CardDatabase GetCardDatabase(int level)
        {
            if (cardDatabaseList == null || cardDatabaseList.Count == 0)
            {
                Debug.LogWarning($"CardDatabaseList is empty or null! Cannot get database for level {level}.");
                return null;
            }
            
            // Clamp level to valid range (0 to count-1)
            // If level exceeds available databases, use the last one
            int index = Mathf.Clamp(level, 0, cardDatabaseList.Count - 1);
            
            if (index != level)
            {
                Debug.LogWarning($"Level {level} is out of range (0-{cardDatabaseList.Count - 1}). Using database at index {index} instead.");
            }
            
            return cardDatabaseList[index];
        }

    }
}

