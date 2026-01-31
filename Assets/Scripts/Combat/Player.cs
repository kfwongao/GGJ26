using UnityEngine;
using MaskMYDrama.Core;

namespace MaskMYDrama.Combat
{
    /// <summary>
    /// Player entity with energy management system.
    /// 
    /// Implements the energy system from CSV:
    /// - Energy balls: 3/4/5 per round (configurable via maxEnergy)
    /// - Energy is consumed when playing cards
    /// - Energy resets at the start of each player turn
    /// - Player can only play cards if energy >= card cost
    /// 
    /// Extends CombatEntity to add energy functionality.
    /// </summary>
    [System.Serializable]
    public class Player : CombatEntity
    {
        [Header("Player Stats")]
        [Tooltip("Current available energy")]
        [SerializeField] public int currentEnergy = 3;
        
        [Tooltip("Maximum energy per turn (3/4/5 as per CSV)")]
        [SerializeField] public int maxEnergy = 3;
        
        /// <summary>
        /// Sets energy to a specific value (clamped to 0-maxEnergy).
        /// </summary>
        /// <param name="energy">Energy value to set</param>
        public void SetEnergy(int energy)
        {
            currentEnergy = Mathf.Clamp(energy, 0, maxEnergy);
        }
        
        /// <summary>
        /// Consumes energy when playing a card.
        /// </summary>
        /// <param name="amount">Energy amount to consume</param>
        public void ConsumeEnergy(int amount)
        {
            currentEnergy = Mathf.Max(0, currentEnergy - amount);
        }
        
        /// <summary>
        /// Resets energy to maximum (called at start of player turn).
        /// </summary>
        public void ResetEnergy()
        {
            currentEnergy = maxEnergy;
        }
        
        /// <summary>
        /// Checks if player has enough energy to play a card.
        /// Used to determine if cards are playable.
        /// </summary>
        /// <param name="cost">Card energy cost</param>
        /// <returns>True if energy >= cost</returns>
        public bool HasEnoughEnergy(int cost)
        {
            return currentEnergy >= cost;
        }
        
        /// <summary>
        /// Resets turn stats including energy.
        /// Called at the start of each player turn.
        /// </summary>
        public override void ResetTurn()
        {
            base.ResetTurn();
            ResetEnergy();
        }
    }
}

