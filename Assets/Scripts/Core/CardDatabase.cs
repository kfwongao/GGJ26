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
    [CreateAssetMenu(fileName = "CardDatabase", menuName = "MaskMYDrama/Card Database")]
    public class CardDatabase : ScriptableObject
    {
        [Header("Starting Cards (开局 in card pool)")]
        [Tooltip("Cards that start in the player's deck")]
        public Card[] startingCards;
        
        [Header("Roguelike Pool Cards")]
        [Tooltip("Cards that can be added through roguelike selection")]
        public Card[] roguelikePoolCards;
        
        [Header("All Cards (Legacy - use startingCards and roguelikePoolCards instead)")]
        [Tooltip("All cards in database (for backward compatibility)")]
        public Card[] allCards;
        
        /// <summary>
        /// Get random cards from roguelike pool for selection
        /// </summary>
        /// <param name="count">Number of cards to return (default 3)</param>
        /// <returns>List of random cards from roguelike pool</returns>
        public List<Card> GetRandomCards(int count = 3)
        {
            List<Card> selected = new List<Card>();
            
            // Use roguelike pool if available, otherwise fall back to allCards
            List<Card> available = new List<Card>();
            if (roguelikePoolCards != null && roguelikePoolCards.Length > 0)
            {
                available = new List<Card>(roguelikePoolCards);
            }
            else if (allCards != null && allCards.Length > 0)
            {
                // Fallback: filter allCards by pool type
                available = allCards.Where(c => c != null && c.poolType == CardPoolType.RoguelikePool).ToList();
            }
            
            if (available.Count == 0)
                return selected;
            
            // Shuffle
            for (int i = available.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                Card temp = available[i];
                available[i] = available[randomIndex];
                available[randomIndex] = temp;
            }
            
            // Select up to count cards
            for (int i = 0; i < Mathf.Min(count, available.Count); i++)
            {
                selected.Add(available[i]);
            }
            
            return selected;
        }
        
        /// <summary>
        /// Get all starting cards (开局 in card pool)
        /// </summary>
        /// <returns>List of starting cards</returns>
        public List<Card> GetStartingCards()
        {
            List<Card> starting = new List<Card>();
            
            if (startingCards != null && startingCards.Length > 0)
            {
                starting = new List<Card>(startingCards);
            }
            else if (allCards != null && allCards.Length > 0)
            {
                // Fallback: filter allCards by pool type
                starting = allCards.Where(c => c != null && c.poolType == CardPoolType.StartingPool).ToList();
            }
            
            return starting;
        }
        
        /// <summary>
        /// Get all roguelike pool cards
        /// </summary>
        /// <returns>List of roguelike pool cards</returns>
        public List<Card> GetRoguelikePoolCards()
        {
            List<Card> roguelike = new List<Card>();
            
            if (roguelikePoolCards != null && roguelikePoolCards.Length > 0)
            {
                roguelike = new List<Card>(roguelikePoolCards);
            }
            else if (allCards != null && allCards.Length > 0)
            {
                // Fallback: filter allCards by pool type
                roguelike = allCards.Where(c => c != null && c.poolType == CardPoolType.RoguelikePool).ToList();
            }
            
            return roguelike;
        }
    }
}

