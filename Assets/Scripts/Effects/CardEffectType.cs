namespace MaskMYDrama.Effects
{
    /// <summary>
    /// Defines all possible card effect types.
    /// Based on CSV card descriptions from MaskMYDrama - Art Design Info.csv
    /// </summary>
    public enum CardEffectType
    {
        // Basic Effects
        Damage,              // 造成伤害 (受到打击的一名对手将失去X点生命)
        DamageAll,           // 对所有敌人造成伤害 (所有对手失去X点生命)
        RandomDamage,        // 随机伤害 (随机令一名敌人失去X点生命)
        Defence,             // 获得防御 (获得X点屏障保护)
        Heal,                // 恢复生命 (恢复X点生命)
        DrawCard,            // 抽牌 (抽X张牌)
        
        // Formula Effects
        FormulaDamage,       // 公式伤害 (受到打击的一名对手失去X*4点生命，X=剩余全部天赋点)
        FormulaDefence,      // 公式防御 (获得本轮所有伤害同等值的屏障)
        
        // Card Manipulation
        DiscardCard,         // 弃牌 (随机消耗一张手牌)
        DiscardAllByType,    // 按类型弃牌 (消耗手牌中所有力量牌/功能牌)
        ReturnCard,          // 从弃牌堆返回 (将弃牌堆的一张牌放回手牌)
        ReturnAllToPool,     // 将弃牌堆所有牌放入卡池 (将弃牌堆所有牌放入卡池并洗牌)
        ModifyCardCost,      // 修改卡牌费用 (此张牌的天赋点变为0)
        ShufflePool,         // 洗牌 (重洗卡池所有牌)
        
        // Special Effects
        DelayedDamage,       // 延迟伤害 (下一轮回合对方失去对你造成的同等伤害)
        ReflectDamage,       // 反射伤害 (下一轮回合对方失去对你造成的同等伤害)
        ModifyEnergy,        // 修改能量 (需消耗的天赋点-1)
        ModifyMaxHealth,     // 修改最大生命 (获得最大35生命)
        DamageReduction,    // 减伤 (本轮减伤30%)
        DamageBoost,         // 增伤 (指定的一名敌人获得20%增伤)
        Exhaust,             // 一次性使用 (一次性使用)
        
        // Conditional Effects
        ConditionalEffect    // 条件效果 (需要满足条件才能触发)
    }
}

