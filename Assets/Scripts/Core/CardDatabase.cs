using System.Collections.Generic;
using UnityEngine;
using MaskMYDrama.Cards;

namespace MaskMYDrama.Core
{
    /// <summary>
    /// Manages available cards for roguelike selection
    /// </summary>
    [CreateAssetMenu(fileName = "CardDatabase", menuName = "MaskMYDrama/Card Database")]
    public class CardDatabase : ScriptableObject
    {
        [Header("Available Cards")]
        public Card[] allCards;
        
        public List<Card> GetRandomCards(int count)
        {
            List<Card> selected = new List<Card>();
            List<Card> available = new List<Card>(allCards);
            
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
    }
}

