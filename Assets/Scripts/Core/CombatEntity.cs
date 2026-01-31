using UnityEngine;

namespace MaskMYDrama.Core
{
    /// <summary>
    /// Base class for all combat entities (Player and Enemy).
    /// 
    /// Implements the core combat mechanics as specified in CSV:
    /// - Life bars (health system)
    /// - Defence points (accumulated per turn, reduce incoming damage)
    /// - Attack power (dynamically calculated)
    /// - Damage calculation: actual damage = incoming damage - defence
    /// 
    /// Defence points reset at the start of each turn.
    /// </summary>
    [System.Serializable]
    public class CombatEntity : MonoBehaviour
    {
        [Header("Stats")]
        [Tooltip("Maximum health points")]
        [SerializeField] public int maxHealth = 100;
        
        [Tooltip("Current health points")]
        [SerializeField] public int currentHealth;
        
        [Tooltip("Defence points accumulated this turn (reduces incoming damage)")]
        [SerializeField] public int currentDefence = 0; // Defence points accumulated this turn
        
        [Header("Combat")]
        [Tooltip("Dynamically calculated attack power (modified by strength cards)")]
        [SerializeField] public int attackPower = 0; // Dynamically calculated attack
        
        /// <summary>
        /// Initialize health to maximum on awake.
        /// </summary>
        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }
        
        /// <summary>
        /// Takes damage, applying defence reduction.
        /// 
        /// Formula from CSV: actual damage = incoming damage - defence
        /// Defence is consumed when blocking damage.
        /// </summary>
        /// <param name="damage">Incoming damage amount</param>
        public virtual void TakeDamage(int damage)
        {
            // Defence reduces damage, but is also consumed
            int actualDamage = Mathf.Max(0, damage - currentDefence);
            currentHealth = Mathf.Max(0, currentHealth - actualDamage);
            currentDefence = Mathf.Max(0, currentDefence - damage);
        }
        
        /// <summary>
        /// Adds defence points (from Defence cards).
        /// </summary>
        /// <param name="defence">Defence points to add</param>
        public virtual void AddDefence(int defence)
        {
            currentDefence += defence;
        }
        
        /// <summary>
        /// Adds attack power (from Strength cards).
        /// </summary>
        /// <param name="attack">Attack power to add</param>
        public virtual void AddAttackPower(int attack)
        {
            attackPower += attack;
        }
        
        /// <summary>
        /// Resets defence points to 0 (called at turn start).
        /// </summary>
        public virtual void ResetDefence()
        {
            currentDefence = 0;
        }
        
        /// <summary>
        /// Checks if the entity is still alive.
        /// </summary>
        /// <returns>True if health > 0</returns>
        public virtual bool IsAlive()
        {
            return currentHealth > 0;
        }
        
        /// <summary>
        /// Resets turn-based stats (defence and attack power).
        /// Called at the start of each turn.
        /// </summary>
        public virtual void ResetTurn()
        {
            ResetDefence();
            attackPower = 0;
        }
    }
}

