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
        [Tooltip("Starting cards (can be set directly or via CardDatabase)")]
        public Card[] startingCards;
        
        [Header("Card Database")]
        [Tooltip("Card database for roguelike selection and starting cards")]
        public CardDatabase cardDatabase;
        
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
            
            // Get starting cards from CardDatabase if available, otherwise use startingCards array
            List<Card> cardsToAdd = new List<Card>();
            
            if (cardDatabase != null)
            {
                // Use CardDatabase to get starting cards
                cardsToAdd = cardDatabase.GetStartingCards();
            }
            else if (startingCards != null && startingCards.Length > 0)
            {
                // Fallback to startingCards array
                cardsToAdd = new List<Card>(startingCards);
            }
            
            // Add starting cards to pool
            foreach (var card in cardsToAdd)
            {
                if (card != null && card.poolType == CardPoolType.StartingPool)
                {
                    cardPool.Add(new CardInstance(card));
                }
            }
            
            // Ensure we have at least initialCardCount cards
            while (cardPool.Count < initialCardCount && cardsToAdd.Count > 0)
            {
                // Duplicate some basic cards if needed
                foreach (var card in cardsToAdd)
                {
                    if (cardPool.Count >= initialCardCount) break;
                    if (card != null && card.poolType == CardPoolType.StartingPool)
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
        /// Similar to Slay the Spire: Draw cards from pool to hand
        /// </summary>
        public void DrawHand()
        {
            hand.Clear();
            
            // Draw 5 cards
            DrawCards(5);
        }
        
        /// <summary>
        /// Draw a specific number of cards from pool to hand.
        /// Similar to Slay the Spire draw card logic.
        /// </summary>
        /// <param name="count">Number of cards to draw</param>
        /// <returns>Number of cards actually drawn</returns>
        public int DrawCards(int count)
        {
            int cardsDrawn = 0;
            
            for (int i = 0; i < count; i++)
            {
                // If pool is empty, reshuffle abandoned pile into pool
                if (cardPool.Count == 0)
                {
                    ReshuffleAbandonedPile();
                    
                    // If still empty after reshuffle, can't draw more
                    if (cardPool.Count == 0)
                        break;
                }
                
                // Draw one card from pool to hand
                if (cardPool.Count > 0)
                {
                    hand.Add(cardPool[0]);
                    cardPool.RemoveAt(0);
                    cardsDrawn++;
                }
            }
            
            return cardsDrawn;
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
        
        /// <summary>
        /// Get current hand size (for UI display)
        /// </summary>
        public int GetHandSize()
        {
            return hand.Count;
        }
        
        /// <summary>
        /// Get maximum hand size (typically 5, but can be modified by effects)
        /// </summary>
        public int GetMaxHandSize()
        {
            return 5; // Default hand size as per CSV
        }
    }
}

