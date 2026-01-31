using UnityEngine;
using MaskMYDrama.Core;

namespace MaskMYDrama.Combat
{
    /// <summary>
    /// Enemy entity with attack patterns.
    /// 
    /// Implements enemy combat behavior:
    /// - Base attack damage that can be modified
    /// - Attack multiplier for scaling difficulty
    /// - Executes attacks during enemy turn
    /// 
    /// Based on CSV: Enemy attacks player during enemy turn.
    /// Enemy's attack point is dynamically calculated.
    /// </summary>
    public class Enemy : CombatEntity
    {
        [Header("Enemy Info")]
        [Tooltip("Name of the enemy")]
        public string enemyName;
        
        [Header("Attack Pattern")]
        [Tooltip("Base damage dealt per attack")]
        public int baseAttackDamage = 10;
        
        [Tooltip("Multiplier for attack damage (for difficulty scaling)")]
        public int attackMultiplier = 1;
        
        /// <summary>
        /// Calculates the attack damage this enemy will deal.
        /// </summary>
        /// <returns>Total attack damage</returns>
        public int GetAttackDamage()
        {
            return baseAttackDamage * attackMultiplier;
        }
        
        /// <summary>
        /// Executes an attack against the player.
        /// Called during enemy turn by CombatManager.
        /// </summary>
        /// <param name="target">The player to attack</param>
        public int ExecuteAttack(Player target)
        {
            int damage = GetAttackDamage();
            target.TakeDamage(damage);

            return damage;
        }
    }
}

