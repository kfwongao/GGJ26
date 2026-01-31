using System;
using System.Collections.Generic;
using UnityEngine;
using MaskMYDrama.Core;

namespace MaskMYDrama.Effects
{
    /// <summary>
    /// Serializable data structure for card effects.
    /// Supports complex effects with formulas, conditions, and sub-effects.
    /// </summary>
    [Serializable]
    public class CardEffectData
    {
        [Header("Effect Type")]
        [Tooltip("Type of effect to apply")]
        public CardEffectType effectType = CardEffectType.Damage;
        
        [Header("Base Values")]
        [Tooltip("Base value for the effect (e.g., damage amount, defence points)")]
        public int baseValue = 0;
        
        [Tooltip("Secondary value (e.g., number of cards to draw, heal amount)")]
        public int secondaryValue = 0;
        
        [Header("Formula")]
        [Tooltip("Formula string (e.g., 'X*4' where X is a variable). Leave empty for simple baseValue effects.")]
        public string formula = "";
        
        [Tooltip("Variable to use in formula (e.g., RemainingEnergy, PoolCount)")]
        public EffectVariable variable = EffectVariable.None;
        
        [Header("Target")]
        [Tooltip("Target type: Self, Enemy, AllEnemies, RandomEnemy, LowestHealthEnemy")]
        public EffectTarget target = EffectTarget.Enemy;
        
        [Header("Conditions")]
        [Tooltip("Conditions that must be met for this effect to trigger")]
        public List<EffectCondition> conditions = new List<EffectCondition>();
        
        [Header("Sub Effects")]
        [Tooltip("Additional effects that trigger after this effect")]
        public List<CardEffectData> subEffects = new List<CardEffectData>();
        
        [Header("Special Properties")]
        [Tooltip("If true, this effect is applied next turn instead of this turn")]
        public bool isDelayed = false;
        
        [Tooltip("If true, this card is exhausted (removed) after use")]
        public bool exhaustCard = false;
        
        [Tooltip("Card type filter for discard/return effects (e.g., Strength, Function)")]
        public CardType cardTypeFilter = CardType.Attack;
        
        [Tooltip("Percentage modifier (e.g., 20 for 20% damage boost)")]
        public float percentageModifier = 0f;
    }
    
    /// <summary>
    /// Target types for card effects
    /// </summary>
    public enum EffectTarget
    {
        Self,                // 自己
        Enemy,               // 单个敌人
        AllEnemies,          // 所有敌人
        RandomEnemy,         // 随机敌人
        LowestHealthEnemy    // 生命最少的敌人
    }
    
    /// <summary>
    /// Condition types for conditional effects
    /// </summary>
    [Serializable]
    public class EffectCondition
    {
        [Tooltip("Type of condition to check")]
        public ConditionType conditionType = ConditionType.None;
        
        [Tooltip("Value to compare against")]
        public int compareValue = 0;
        
        [Tooltip("Comparison operator")]
        public ComparisonOperator comparison = ComparisonOperator.GreaterThan;
    }
    
    /// <summary>
    /// Types of conditions
    /// </summary>
    public enum ConditionType
    {
        None,
        HasCardTypeInHand,      // 手牌中有特定类型卡牌
        HasEnoughEnergy,        // 有足够能量
        HasEnoughHealth,        // 有足够生命
        PoolCount,              // 卡池数量
        HandCount,              // 手牌数量
        EnemyCount              // 敌人数量
    }
    
    /// <summary>
    /// Comparison operators
    /// </summary>
    public enum ComparisonOperator
    {
        GreaterThan,        // >
        LessThan,          // <
        Equal,             // ==
        GreaterOrEqual,    // >=
        LessOrEqual        // <=
    }
}

