using MaskMYDrama.Cards;

namespace MaskMYDrama.Cards
{
    /// <summary>
    /// Runtime instance of a card in the game.
    /// 
    /// This wraps a Card ScriptableObject and represents a specific instance
    /// of that card in the player's deck/hand. Allows for per-instance modifications
    /// like upgrades without affecting the base card data.
    /// 
    /// Used by DeckManager to track cards in pool, hand, and abandoned pile.
    /// </summary>
    public class CardInstance
    {
        /// <summary>The base card data (ScriptableObject)</summary>
        public Card cardData;
        
        /// <summary>Whether this instance has been upgraded</summary>
        public bool isUpgraded;
        
        /// <summary>
        /// Creates a new card instance from a card data asset.
        /// </summary>
        /// <param name="card">The card ScriptableObject to create an instance of</param>
        public CardInstance(Card card)
        {
            cardData = card;
            isUpgraded = false;
        }
        
        /// <summary>
        /// Gets the energy cost of this card instance.
        /// </summary>
        public int GetEnergyCost()
        {
            return cardData.energyCost;
        }
        
        /// <summary>
        /// Gets the attack value of this card instance.
        /// </summary>
        public int GetAttackValue()
        {
            return cardData.attackValue;
        }
        
        /// <summary>
        /// Gets the defence value of this card instance.
        /// </summary>
        public int GetDefenceValue()
        {
            return cardData.defenceValue;
        }
    }
}

