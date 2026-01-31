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
            return cardDatabaseList[level];
        }

    }
}

