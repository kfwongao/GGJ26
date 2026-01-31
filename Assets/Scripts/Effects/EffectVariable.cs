namespace MaskMYDrama.Effects
{
    /// <summary>
    /// Defines variables that can be used in formula-based card effects.
    /// Based on CSV formulas like "X*4点生命（X=剩余全部天赋点）"
    /// </summary>
    public enum EffectVariable
    {
        None,                    // 无变量
        RemainingEnergy,         // 剩余全部天赋点 (X=剩余全部天赋点)
        PoolCount,               // 卡池中剩余卡牌数量 (造成你卡池中剩余卡牌数量2倍的伤害)
        HandCount,               // 手牌数量
        AbandonedCount,          // 弃牌堆数量
        TotalDamageThisTurn,     // 本轮所有伤害 (获得本轮所有伤害同等值的屏障)
        RemainingHealth,         // 剩余生命值 (打出剩余生命值的伤害)
        EnemyCount,              // 敌人数量
        StrengthCardCount,      // 手牌中力量牌数量
        FunctionCardCount,       // 手牌中功能牌数量
        AttackCardCount          // 手牌中攻击牌数量
    }
}

