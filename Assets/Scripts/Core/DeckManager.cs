using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MaskMYDrama.Cards;

namespace MaskMYDrama.Core
{
    /// <summary>
    /// Manages the card pool system as specified in CSV gameplay design.
    /// 
    /// Implements the card loop system:
    /// - Card Pool: 5x10 = 50 slots total, filled with 12-15 cards in demo
    /// - Hand: 5 cards per round (designed + randomly drawn)
    /// - Abandoned Pile: Used cards + unused cards from hand
    /// 
    /// Card Flow:
    /// 1. Cards start in Pool
    /// 2. 5 cards drawn to Hand at turn start
    /// 3. Played cards go to Abandoned
    /// 4. Unused cards go to Abandoned at end of round
    /// 5. When Pool runs out, Abandoned reshuffles into Pool
    /// 
    /// Based on CSV: "card in hand: 5, design + randomly"
    /// "card pool: in total, when it's runout - rewash all card in abandoned and fill in it"
    /// </summary>
    public class DeckManager : MonoBehaviour
    {
        [Header("Card Pool Settings")]
        [Tooltip("Total card pool size (5x10 = 50 slots as per CSV)")]
        public int poolSize = 50; // 5x10 grid
        
        [Tooltip("Initial number of cards in pool (12-15 in demo as per CSV)")]
        public int initialCardCount = 12; // 12-15 cards in demo
        
        // Card pools
        /// <summary>Main card pool - cards available to draw</summary>
        private List<CardInstance> cardPool = new List<CardInstance>(); // Main pool
        
        /// <summary>Current hand - 5 cards player can play this turn</summary>
        private List<CardInstance> hand = new List<CardInstance>(); // Current hand (5 cards)
        
        /// <summary>Abandoned pile - used cards that will reshuffle into pool</summary>
        private List<CardInstance> abandonedPile = new List<CardInstance>(); // Used cards
        
        [Header("Starting Cards")]
        public Card[] startingCards;
        
        public int HandCount => hand.Count;
        public int PoolCount => cardPool.Count;
        public int AbandonedCount => abandonedPile.Count;
        
        public List<CardInstance> GetHand() => hand;
        
        private void Awake()
        {
            InitializeDeck();
        }
        
        public void InitializeDeck()
        {
            cardPool.Clear();
            hand.Clear();
            abandonedPile.Clear();
            
            // Add starting cards
            if (startingCards != null && startingCards.Length > 0)
            {
                foreach (var card in startingCards)
                {
                    if (card != null)
                    {
                        cardPool.Add(new CardInstance(card));
                    }
                }
            }
            
            // Ensure we have at least initialCardCount cards
            while (cardPool.Count < initialCardCount && startingCards != null && startingCards.Length > 0)
            {
                // Duplicate some basic cards if needed
                foreach (var card in startingCards)
                {
                    if (cardPool.Count >= initialCardCount) break;
                    if (card != null)
                    {
                        cardPool.Add(new CardInstance(card));
                    }
                }
            }
            
            ShufflePool();
        }
        
        public void ShufflePool()
        {
            // Fisher-Yates shuffle
            for (int i = cardPool.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                CardInstance temp = cardPool[i];
                cardPool[i] = cardPool[randomIndex];
                cardPool[randomIndex] = temp;
            }
        }
        
        /// <summary>
        /// Draw 5 cards into hand (designed + randomly)
        /// </summary>
        public void DrawHand()
        {
            hand.Clear();
            
            // If pool is empty, reshuffle abandoned pile into pool
            if (cardPool.Count == 0)
            {
                ReshuffleAbandonedPile();
            }
            
            // Draw up to 5 cards
            int cardsToDraw = Mathf.Min(5, cardPool.Count);
            for (int i = 0; i < cardsToDraw; i++)
            {
                if (cardPool.Count > 0)
                {
                    hand.Add(cardPool[0]);
                    cardPool.RemoveAt(0);
                }
            }
        }
        
        /// <summary>
        /// Play a card from hand
        /// </summary>
        public CardInstance PlayCard(int handIndex)
        {
            if (handIndex < 0 || handIndex >= hand.Count)
                return null;
            
            CardInstance card = hand[handIndex];
            hand.RemoveAt(handIndex);
            
            // Add to abandoned pile (unless it's exhaust)
            if (!card.cardData.isExhaust)
            {
                abandonedPile.Add(card);
            }
            
            return card;
        }
        
        /// <summary>
        /// End of round: All unused cards in hand go to abandoned pile
        /// </summary>
        public void EndRound()
        {
            // All remaining cards in hand go to abandoned
            while (hand.Count > 0)
            {
                abandonedPile.Add(hand[0]);
                hand.RemoveAt(0);
            }
        }
        
        /// <summary>
        /// When pool runs out, reshuffle all cards from abandoned pile into pool
        /// </summary>
        private void ReshuffleAbandonedPile()
        {
            cardPool.AddRange(abandonedPile);
            abandonedPile.Clear();
            ShufflePool();
        }
        
        /// <summary>
        /// Add a new card to the pool (from roguelike selection)
        /// </summary>
        public void AddCardToPool(Card card)
        {
            if (cardPool.Count < poolSize)
            {
                cardPool.Add(new CardInstance(card));
            }
        }
        
        /// <summary>
        /// Get 3 random cards for roguelike selection
        /// </summary>
        public List<Card> GetRandomCardOptions(int count = 3)
        {
            // This would typically come from a card database
            // For now, return placeholder - will be implemented with card database
            return new List<Card>();
        }
    }
}

