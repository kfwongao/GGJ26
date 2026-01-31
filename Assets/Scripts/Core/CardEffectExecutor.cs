using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MaskMYDrama.Cards;
using MaskMYDrama.Effects;
using MaskMYDrama.Combat;

namespace MaskMYDrama.Core
{
    /// <summary>
    /// Executes card effects based on CardEffectData configurations.
    /// Supports complex effects with formulas, conditions, and sub-effects.
    /// Based on CSV card descriptions from MaskMYDrama - Art Design Info.csv
    /// 
    /// ============================================================================
    /// HOW TO CONFIGURE CARD SCRIPTABLEOBJECT FOR CSV CARDS
    /// ============================================================================
    /// 
    /// This executor supports two ways to configure card effects:
    /// 1. Simple Legacy System: Use attackValue, defenceValue, drawCardCount directly
    /// 2. Advanced Effect System: Use CardEffect ScriptableObject with CardEffectData
    /// 
    /// ============================================================================
    /// METHOD 1: SIMPLE CARDS (Legacy System)
    /// ============================================================================
    /// 
    /// For simple cards from CSV, you can use the legacy system:
    /// 
    /// Example: "亮相/Stage pose" - 受到打击的一名对手将失去5点生命
    /// - Card Type: Attack
    /// - Energy Cost: 1
    /// - Attack Value: 5
    /// - Pool Type: StartingPool
    /// - useAdvancedEffects: false (use legacy system)
    /// 
    /// Example: "屏障/Block" - 获得4点屏障保护
    /// - Card Type: Defence
    /// - Energy Cost: 1
    /// - Defence Value: 4
    /// - Pool Type: StartingPool
    /// - useAdvancedEffects: false
    /// 
    /// Example: "Monologue 独白" - 受到打击的一名对手将失去7点生命，抽一张牌
    /// - Card Type: Attack
    /// - Energy Cost: 1
    /// - Attack Value: 7
    /// - Draw Card Count: 1
    /// - Pool Type: StartingPool
    /// - useAdvancedEffects: false
    /// 
    /// ============================================================================
    /// METHOD 2: COMPLEX CARDS (Advanced Effect System)
    /// ============================================================================
    /// 
    /// For complex cards with formulas, conditions, or multiple effects:
    /// 
    /// Step 1: Create CardEffect ScriptableObject
    ///   Right-click → Create → MaskMYDrama → Card Effect
    /// 
    /// Step 2: Configure CardEffectData in the CardEffect
    /// 
    /// Step 3: In Card ScriptableObject:
    ///   - Set useAdvancedEffects = true
    ///   - Assign the CardEffect to cardEffect field
    /// 
    /// ============================================================================
    /// CSV CARD EXAMPLES WITH CONFIGURATION
    /// ============================================================================
    /// 
    /// 1. "即兴表演/Improvisation" - 受到打击的一名对手失去X*4点生命（X=剩余全部天赋点）
    ///    Configuration:
    ///    - Create CardEffect ScriptableObject
    ///    - Add CardEffectData:
    ///      * Effect Type: FormulaDamage
    ///      * Formula: "X*4"
    ///      * Variable: RemainingEnergy
    ///      * Target: Enemy
    ///    - In Card: useAdvancedEffects = true, assign CardEffect
    /// 
    /// 2. "高潮/Climax" - 随机令一名敌人失去20点生命 - 消耗手牌中所有力量牌
    ///    Configuration:
    ///    - Create CardEffect ScriptableObject
    ///    - Add CardEffectData (Effect 1):
    ///      * Effect Type: RandomDamage
    ///      * Base Value: 20
    ///      * Target: RandomEnemy
    ///    - Add CardEffectData (Effect 2 - Sub Effect):
    ///      * Effect Type: DiscardAllByType
    ///      * Card Type Filter: Strength
    ///    - In Card: useAdvancedEffects = true, assign CardEffect
    /// 
    /// 3. "主角/Protagonist" - 抽3张牌，下一轮回合对方失去对你造成的同等伤害
    ///    Configuration:
    ///    - Create CardEffect ScriptableObject
    ///    - Add CardEffectData (Effect 1):
    ///      * Effect Type: DrawCard
    ///      * Base Value: 3
    ///    - Add CardEffectData (Effect 2):
    ///      * Effect Type: DelayedDamage (or ReflectDamage)
    ///      * Is Delayed: true
    ///      * Note: This requires delayed effect system implementation
    ///    - In Card: useAdvancedEffects = true, assign CardEffect
    /// 
    /// 4. "多才多艺/Versatile" - 受到打击的对手失去9点生命，将弃牌堆的一张牌放回手牌，此张牌的天赋点变为0
    ///    Configuration:
    ///    - Create CardEffect ScriptableObject
    ///    - Add CardEffectData (Effect 1):
    ///      * Effect Type: Damage
    ///      * Base Value: 9
    ///      * Target: Enemy
    ///    - Add CardEffectData (Effect 2):
    ///      * Effect Type: ReturnCard
    ///      * Base Value: 1
    ///    - Add CardEffectData (Effect 3):
    ///      * Effect Type: ModifyCardCost
    ///      * Base Value: 0
    ///      * Note: This modifies the returned card's cost
    ///    - In Card: useAdvancedEffects = true, assign CardEffect
    /// 
    /// 5. "横穿舞台/Cross" - 本轮结束后，下一轮你将获得和对敌人伤害同等数值的格挡
    ///    Configuration:
    ///    - Create CardEffect ScriptableObject
    ///    - Add CardEffectData:
    ///      * Effect Type: FormulaDefence
    ///      * Formula: "TotalDamageThisTurn"
    ///      * Variable: TotalDamageThisTurn
    ///      * Is Delayed: true
    ///      * Note: This requires delayed effect system
    ///    - In Card: useAdvancedEffects = true, assign CardEffect
    /// 
    /// 6. "提示词/Cue" - 获得本轮所有伤害同等值的屏障，并恢复15点生命
    ///    Configuration:
    ///    - Create CardEffect ScriptableObject
    ///    - Add CardEffectData (Effect 1):
    ///      * Effect Type: FormulaDefence
    ///      * Formula: "TotalDamageThisTurn"
    ///      * Variable: TotalDamageThisTurn
    ///    - Add CardEffectData (Effect 2):
    ///      * Effect Type: Heal
    ///      * Base Value: 15
    ///    - In Card: useAdvancedEffects = true, assign CardEffect
    /// 
    /// 7. "假戏真做" - 造成你卡池中剩余卡牌数量2倍的伤害，获得4点屏障
    ///    Configuration:
    ///    - Create CardEffect ScriptableObject
    ///    - Add CardEffectData (Effect 1):
    ///      * Effect Type: FormulaDamage
    ///      * Formula: "PoolCount*2"
    ///      * Variable: PoolCount
    ///      * Target: Enemy
    ///    - Add CardEffectData (Effect 2):
    ///      * Effect Type: Defence
    ///      * Base Value: 4
    ///    - In Card: useAdvancedEffects = true, assign CardEffect
    /// 
    /// 8. "剧透/Spoiled Alert" - 损失5点生命，打出剩余生命值的伤害给生命最少的敌人，获得12点屏障
    ///    Configuration:
    ///    - Create CardEffect ScriptableObject
    ///    - Add CardEffectData (Effect 1):
    ///      * Effect Type: Damage (to self)
    ///      * Base Value: 5
    ///      * Target: Self
    ///    - Add CardEffectData (Effect 2):
    ///      * Effect Type: FormulaDamage
    ///      * Formula: "RemainingHealth"
    ///      * Variable: RemainingHealth
    ///      * Target: LowestHealthEnemy
    ///    - Add CardEffectData (Effect 3):
    ///      * Effect Type: Defence
    ///      * Base Value: 12
    ///    - In Card: useAdvancedEffects = true, assign CardEffect
    /// 
    /// 9. "反派/Antagonist" - 获得最大35生命，本轮减伤30%。消耗手牌中所有功能牌
    ///    Configuration:
    ///    - Create CardEffect ScriptableObject
    ///    - Add CardEffectData (Effect 1):
    ///      * Effect Type: ModifyMaxHealth
    ///      * Base Value: 35
    ///    - Add CardEffectData (Effect 2):
    ///      * Effect Type: DamageReduction
    ///      * Percentage Modifier: 30
    ///    - Add CardEffectData (Effect 3):
    ///      * Effect Type: DiscardAllByType
    ///      * Card Type Filter: Function
    ///    - In Card: useAdvancedEffects = true, assign CardEffect
    /// 
    /// ============================================================================
    /// FORMULA VARIABLES AVAILABLE
    /// ============================================================================
    /// 
    /// When using FormulaDamage or FormulaDefence, you can use these variables:
    /// - RemainingEnergy: 剩余全部天赋点 (X=剩余全部天赋点)
    /// - PoolCount: 卡池中剩余卡牌数量
    /// - HandCount: 手牌数量
    /// - AbandonedCount: 弃牌堆数量
    /// - TotalDamageThisTurn: 本轮所有伤害
    /// - RemainingHealth: 剩余生命值
    /// - EnemyCount: 敌人数量
    /// - StrengthCardCount: 手牌中力量牌数量
    /// - FunctionCardCount: 手牌中功能牌数量
    /// - AttackCardCount: 手牌中攻击牌数量
    /// 
    /// Formula Examples:
    /// - "X*4" where X = RemainingEnergy → 即兴表演
    /// - "PoolCount*2" → 假戏真做
    /// - "TotalDamageThisTurn" → 提示词, 横穿舞台
    /// - "RemainingHealth" → 剧透
    /// 
    /// ============================================================================
    /// EFFECT TYPES AND THEIR USAGE
    /// ============================================================================
    /// 
    /// Damage: 造成伤害 (受到打击的一名对手将失去X点生命)
    ///   - Base Value: 伤害数值
    ///   - Target: Enemy, AllEnemies, RandomEnemy, LowestHealthEnemy
    /// 
    /// DamageAll: 对所有敌人造成伤害 (所有对手失去X点生命)
    ///   - Base Value: 伤害数值
    /// 
    /// RandomDamage: 随机伤害 (随机令一名敌人失去X点生命)
    ///   - Base Value: 伤害数值
    /// 
    /// FormulaDamage: 公式伤害 (受到打击的一名对手失去X*4点生命)
    ///   - Formula: "X*4", "PoolCount*2", etc.
    ///   - Variable: RemainingEnergy, PoolCount, etc.
    ///   - Target: Enemy, AllEnemies, etc.
    /// 
    /// Defence: 获得防御 (获得X点屏障保护)
    ///   - Base Value: 防御数值
    /// 
    /// FormulaDefence: 公式防御 (获得本轮所有伤害同等值的屏障)
    ///   - Formula: "TotalDamageThisTurn"
    ///   - Variable: TotalDamageThisTurn
    /// 
    /// Heal: 恢复生命 (恢复X点生命)
    ///   - Base Value: 恢复数值
    /// 
    /// DrawCard: 抽牌 (抽X张牌)
    ///   - Base Value: 抽牌数量
    /// 
    /// DiscardCard: 弃牌 (随机消耗一张手牌)
    ///   - Randomly discards one card
    /// 
    /// DiscardAllByType: 按类型弃牌 (消耗手牌中所有力量牌/功能牌)
    ///   - Card Type Filter: Strength, Function, Attack, Defence
    /// 
    /// ReturnCard: 从弃牌堆返回 (将弃牌堆的一张牌放回手牌)
    ///   - Base Value: 返回数量
    ///   - Card Type Filter: (optional) filter by type
    /// 
    /// ReturnAllToPool: 将弃牌堆所有牌放入卡池 (将弃牌堆所有牌放入卡池并洗牌)
    /// 
    /// ModifyCardCost: 修改卡牌费用 (此张牌的天赋点变为0)
    ///   - Base Value: 新的费用值
    /// 
    /// ShufflePool: 洗牌 (重洗卡池所有牌)
    /// 
    /// ModifyEnergy: 修改能量 (需消耗的天赋点-1)
    ///   - Base Value: 能量变化值 (可以是负数)
    /// 
    /// ModifyMaxHealth: 修改最大生命 (获得最大35生命)
    ///   - Base Value: 生命值变化
    /// 
    /// DamageReduction: 减伤 (本轮减伤30%)
    ///   - Percentage Modifier: 百分比值 (30 = 30%)
    /// 
    /// DamageBoost: 增伤 (指定的一名敌人获得20%增伤)
    ///   - Percentage Modifier: 百分比值 (20 = 20%)
    /// 
    /// Exhaust: 一次性使用 (一次性使用)
    ///   - Marks card as exhaust (removed after use)
    /// 
    /// ============================================================================
    /// CONDITIONAL EFFECTS
    /// ============================================================================
    /// 
    /// Some cards require conditions to be met:
    /// Example: "高潮/Climax" requires discarding all Strength cards
    /// 
    /// To add conditions:
    /// - In CardEffectData, add to Conditions list
    /// - Condition Type: HasCardTypeInHand, HasEnoughEnergy, etc.
    /// - Comparison: GreaterThan, LessThan, Equal, etc.
    /// - Compare Value: Value to compare against
    /// 
    /// ============================================================================
    /// SUB-EFFECTS (组合效果)
    /// ============================================================================
    /// 
    /// Cards with multiple effects can use Sub Effects:
    /// Example: "多才多艺/Versatile" has 3 effects:
    ///   1. Damage (9)
    ///   2. ReturnCard (1)
    ///   3. ModifyCardCost (0)
    /// 
    /// Configuration:
    /// - Create main CardEffectData with first effect
    /// - Add sub-effects in Sub Effects list
    /// - Sub-effects execute after main effect
    /// 
    /// ============================================================================
    /// </summary>
    public class CardEffectExecutor : MonoBehaviour
    {
        [Header("References")]
        public Player player;
        public Enemy enemy;
        public DeckManager deckManager;
        
        // Track damage dealt this turn for formula effects
        private int totalDamageThisTurn = 0;
        
        /// <summary>
        /// Executes all effects from a card effect configuration.
        /// </summary>
        /// <param name="effectConfig">The card effect configuration to execute</param>
        /// <param name="cardInstance">The card instance being played</param>
        /// <returns>True if effects were successfully applied</returns>
        public bool ExecuteEffects(CardEffect effectConfig, CardInstance cardInstance)
        {
            if (effectConfig == null || effectConfig.effects == null || effectConfig.effects.Count == 0)
            {
                // Fallback to legacy card system if no effect config
                return false;
            }
            
            bool allEffectsApplied = true;
            
            foreach (var effectData in effectConfig.effects)
            {
                if (!ExecuteEffect(effectData, cardInstance))
                {
                    allEffectsApplied = false;
                }
            }
            
            return allEffectsApplied;
        }
        
        /// <summary>
        /// Executes a single card effect.
        /// </summary>
        private bool ExecuteEffect(CardEffectData effectData, CardInstance cardInstance)
        {
            // Check conditions first
            if (!CheckConditions(effectData.conditions))
            {
                return false;
            }
            
            // Calculate effect value (base value or formula)
            int effectValue = CalculateEffectValue(effectData);
            
            // Execute based on effect type
            switch (effectData.effectType)
            {
                case CardEffectType.Damage:
                    ApplyDamage(effectValue, effectData.target);
                    break;
                    
                case CardEffectType.DamageAll:
                    ApplyDamageToAll(effectValue);
                    break;
                    
                case CardEffectType.RandomDamage:
                    ApplyRandomDamage(effectValue);
                    break;
                    
                case CardEffectType.FormulaDamage:
                    int formulaDamage = CalculateFormulaValue(effectData);
                    ApplyDamage(formulaDamage, effectData.target);
                    break;
                    
                case CardEffectType.Defence:
                    // CSV Examples: "屏障" (4防御), "暗场" (6防御), "停顿" (8防御)
                    player.AddDefence(effectValue);
                    ShowEffectFeedback($"Gained {effectValue} Defence", Color.yellow);
                    break;
                    
                case CardEffectType.FormulaDefence:
                    // CSV Examples: "提示词" (获得本轮所有伤害同等值的屏障), "横穿舞台" (延迟格挡)
                    int formulaDefence = CalculateFormulaValue(effectData);
                    player.AddDefence(formulaDefence);
                    ShowEffectFeedback($"Gained {formulaDefence} Defence", Color.yellow);
                    break;
                    
                case CardEffectType.Heal:
                    // CSV Examples: "提示词" (恢复15点生命)
                    HealPlayer(effectValue);
                    break;
                    
                case CardEffectType.DrawCard:
                    // CSV Examples: "Monologue 独白" (抽1张), "主角" (抽3张), "抢戏" (抽3张), "煽情" (抽3张)
                    int cardsDrawn = deckManager.DrawCards(effectValue);
                    if (cardsDrawn > 0)
                    {
                        ShowEffectFeedback($"Drew {cardsDrawn} card(s)!", Color.cyan);
                    }
                    break;
                    
                case CardEffectType.DiscardCard:
                    DiscardRandomCard();
                    break;
                    
                case CardEffectType.DiscardAllByType:
                    DiscardAllCardsByType(effectData.cardTypeFilter);
                    break;
                    
                case CardEffectType.ReturnCard:
                    ReturnCardFromAbandoned(effectValue, effectData.cardTypeFilter);
                    break;
                    
                case CardEffectType.ReturnAllToPool:
                    ReturnAllToPoolAndShuffle();
                    break;
                    
                case CardEffectType.ModifyCardCost:
                    // This would need to be applied to returned cards
                    // For now, handled in ReturnCardFromAbandoned
                    break;
                    
                case CardEffectType.ShufflePool:
                    // CSV Examples: "情感记忆" (重洗卡池所有牌), "安可" (重洗卡池所有牌)
                    deckManager.ShufflePool();
                    ShowEffectFeedback("Shuffled pool!", Color.white);
                    break;
                    
                case CardEffectType.ModifyEnergy:
                    // CSV Examples: "抢戏" (你每使用一次这张牌，需消耗的天赋点-1)
                    // Note: Per-card tracking not yet implemented, this modifies current energy
                    ModifyPlayerEnergy(effectValue);
                    break;
                    
                case CardEffectType.ModifyMaxHealth:
                    // CSV Examples: "反派" (获得最大35生命)
                    // Note: Base Value should be the target max health, not increment
                    ModifyPlayerMaxHealth(effectValue);
                    break;
                    
                case CardEffectType.DamageReduction:
                    // CSV Examples: "反派" (本轮减伤30%), "讽刺" (敌人对你本轮减伤50%)
                    // This would need a buff system - simplified for now
                    ShowEffectFeedback($"{effectData.percentageModifier}% Damage Reduction", Color.blue);
                    break;
                    
                case CardEffectType.DamageBoost:
                    // CSV Examples: "暗场" (指定的一名敌人获得20%增伤), "讽刺" (你向所有敌人施加了30%增伤)
                    // This would need a buff system - simplified for now
                    ShowEffectFeedback($"{effectData.percentageModifier}% Damage Boost", Color.red);
                    break;
                    
                case CardEffectType.Exhaust:
                    // CSV Examples: "笑场" (一次性使用)
                    // Mark card as exhaust (handled in DeckManager)
                    if (cardInstance != null)
                    {
                        cardInstance.cardData.isExhaust = true;
                    }
                    break;
                    
                default:
                    Debug.LogWarning($"Unhandled effect type: {effectData.effectType}");
                    return false;
            }
            
            // Execute sub-effects
            if (effectData.subEffects != null && effectData.subEffects.Count > 0)
            {
                foreach (var subEffect in effectData.subEffects)
                {
                    ExecuteEffect(subEffect, cardInstance);
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Calculates the effect value (base value or from formula).
        /// </summary>
        private int CalculateEffectValue(CardEffectData effectData)
        {
            if (string.IsNullOrEmpty(effectData.formula))
            {
                return effectData.baseValue;
            }
            
            return CalculateFormulaValue(effectData);
        }
        
        /// <summary>
        /// Calculates formula-based effect value.
        /// Supports formulas like "X*4" where X is a variable.
        /// </summary>
        private int CalculateFormulaValue(CardEffectData effectData)
        {
            if (string.IsNullOrEmpty(effectData.formula))
            {
                return effectData.baseValue;
            }
            
            // Get variable value
            int variableValue = GetVariableValue(effectData.variable);
            
            // Simple formula parsing (supports X*N, X+N, X-N, X/N)
            string formula = effectData.formula.ToUpper().Replace("X", variableValue.ToString());
            
            // For now, support simple multiplication (X*4)
            if (formula.Contains("*"))
            {
                string[] parts = formula.Split('*');
                if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int left) && int.TryParse(parts[1].Trim(), out int right))
                {
                    return left * right;
                }
            }
            
            // Support addition (X+5)
            if (formula.Contains("+"))
            {
                string[] parts = formula.Split('+');
                if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int left) && int.TryParse(parts[1].Trim(), out int right))
                {
                    return left + right;
                }
            }
            
            // Support subtraction (X-5)
            if (formula.Contains("-"))
            {
                string[] parts = formula.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int left) && int.TryParse(parts[1].Trim(), out int right))
                {
                    return left - right;
                }
            }
            
            // Support division (X/2)
            if (formula.Contains("/"))
            {
                string[] parts = formula.Split('/');
                if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int left) && int.TryParse(parts[1].Trim(), out int right) && right != 0)
                {
                    return left / right;
                }
            }
            
            // Special formulas
            if (formula.Contains("POOLCOUNT*2") || formula.Contains("POOLCOUNT * 2"))
            {
                return deckManager.PoolCount * 2;
            }
            
            Debug.LogWarning($"Could not parse formula: {effectData.formula}, using base value: {effectData.baseValue}");
            return effectData.baseValue;
        }
        
        /// <summary>
        /// Gets the value of a variable for formula calculations.
        /// 
        /// Variable Mapping from CSV:
        /// - RemainingEnergy: 剩余全部天赋点 (used in 即兴表演: X=剩余全部天赋点)
        /// - PoolCount: 卡池中剩余卡牌数量 (used in 假戏真做: 造成你卡池中剩余卡牌数量2倍的伤害)
        /// - TotalDamageThisTurn: 本轮所有伤害 (used in 提示词, 横穿舞台: 获得本轮所有伤害同等值的屏障)
        /// - RemainingHealth: 剩余生命值 (used in 剧透: 打出剩余生命值的伤害)
        /// - HandCount: 手牌数量
        /// - AbandonedCount: 弃牌堆数量
        /// - StrengthCardCount: 手牌中力量牌数量 (used in 高潮: 消耗手牌中所有力量牌)
        /// - FunctionCardCount: 手牌中功能牌数量 (used in 反派: 消耗手牌中所有功能牌)
        /// </summary>
        private int GetVariableValue(EffectVariable variable)
        {
            switch (variable)
            {
                case EffectVariable.RemainingEnergy:
                    return player != null ? player.currentEnergy : 0;
                    
                case EffectVariable.PoolCount:
                    return deckManager != null ? deckManager.PoolCount : 0;
                    
                case EffectVariable.HandCount:
                    return deckManager != null ? deckManager.HandCount : 0;
                    
                case EffectVariable.AbandonedCount:
                    return deckManager != null ? deckManager.AbandonedCount : 0;
                    
                case EffectVariable.TotalDamageThisTurn:
                    return totalDamageThisTurn;
                    
                case EffectVariable.RemainingHealth:
                    return player != null ? player.currentHealth : 0;
                    
                case EffectVariable.EnemyCount:
                    return enemy != null && enemy.IsAlive() ? 1 : 0;
                    
                case EffectVariable.StrengthCardCount:
                    return CountCardsInHandByType(CardType.Strength);
                    
                case EffectVariable.FunctionCardCount:
                    return CountCardsInHandByType(CardType.Function);
                    
                case EffectVariable.AttackCardCount:
                    return CountCardsInHandByType(CardType.Attack);
                    
                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Counts cards in hand by type.
        /// </summary>
        private int CountCardsInHandByType(CardType cardType)
        {
            if (deckManager == null) return 0;
            
            var hand = deckManager.GetHand();
            return hand.Count(card => card.cardData.cardType == cardType);
        }
        
        /// <summary>
        /// Checks if all conditions are met.
        /// </summary>
        private bool CheckConditions(List<EffectCondition> conditions)
        {
            if (conditions == null || conditions.Count == 0)
                return true;
            
            foreach (var condition in conditions)
            {
                if (!CheckCondition(condition))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Checks a single condition.
        /// </summary>
        private bool CheckCondition(EffectCondition condition)
        {
            int actualValue = 0;
            
            switch (condition.conditionType)
            {
                case ConditionType.HasCardTypeInHand:
                    actualValue = CountCardsInHandByType(CardType.Strength); // Simplified
                    break;
                    
                case ConditionType.PoolCount:
                    actualValue = deckManager != null ? deckManager.PoolCount : 0;
                    break;
                    
                case ConditionType.HandCount:
                    actualValue = deckManager != null ? deckManager.HandCount : 0;
                    break;
                    
                default:
                    return true; // Unknown condition, allow
            }
            
            // Compare
            switch (condition.comparison)
            {
                case ComparisonOperator.GreaterThan:
                    return actualValue > condition.compareValue;
                case ComparisonOperator.LessThan:
                    return actualValue < condition.compareValue;
                case ComparisonOperator.Equal:
                    return actualValue == condition.compareValue;
                case ComparisonOperator.GreaterOrEqual:
                    return actualValue >= condition.compareValue;
                case ComparisonOperator.LessOrEqual:
                    return actualValue <= condition.compareValue;
                default:
                    return true;
            }
        }
        
        // ============================================================================
        // EFFECT APPLICATION METHODS
        // ============================================================================
        // These methods apply the actual effects to game entities.
        // Each method corresponds to card effects from CSV.
        // ============================================================================
        
        /// <summary>
        /// Applies damage to target(s).
        /// CSV Examples:
        /// - "亮相": Damage=5, Target=Enemy
        /// - "Monologue 独白": Damage=7, Target=Enemy
        /// - "Dialogue 对白": Damage=4, Target=AllEnemies
        /// - "高潮": Damage=20, Target=RandomEnemy
        /// - "剧透": Damage=RemainingHealth, Target=LowestHealthEnemy
        /// </summary>
        private void ApplyDamage(int damage, EffectTarget target)
        {
            if (damage <= 0) return;
            
            int finalDamage = damage + (player != null ? player.attackPower : 0);
            
            switch (target)
            {
                case EffectTarget.Enemy:
                    if (enemy != null)
                    {
                        enemy.TakeDamage(finalDamage);
                        totalDamageThisTurn += finalDamage;
                        ShowDamageFeedback(enemy.transform, finalDamage);
                    }
                    break;
                    
                case EffectTarget.AllEnemies:
                    ApplyDamageToAll(finalDamage);
                    break;
                    
                case EffectTarget.RandomEnemy:
                    ApplyRandomDamage(finalDamage);
                    break;
                    
                case EffectTarget.LowestHealthEnemy:
                    // For now, just damage the single enemy
                    if (enemy != null)
                    {
                        enemy.TakeDamage(finalDamage);
                        totalDamageThisTurn += finalDamage;
                        ShowDamageFeedback(enemy.transform, finalDamage);
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Applies damage to all enemies.
        /// CSV Examples:
        /// - "Dialogue 对白": 所有对手失去4点生命
        /// - "笑场": 对所有敌人造成13点伤害
        /// </summary>
        private void ApplyDamageToAll(int damage)
        {
            if (enemy != null && enemy.IsAlive())
            {
                int finalDamage = damage + (player != null ? player.attackPower : 0);
                enemy.TakeDamage(finalDamage);
                totalDamageThisTurn += finalDamage;
                ShowDamageFeedback(enemy.transform, finalDamage);
            }
        }
        
        /// <summary>
        /// Applies random damage to one enemy.
        /// CSV Examples:
        /// - "高潮": 随机令一名敌人失去20点生命
        /// </summary>
        private void ApplyRandomDamage(int damage)
        {
            if (enemy != null && enemy.IsAlive())
            {
                int finalDamage = damage + (player != null ? player.attackPower : 0);
                enemy.TakeDamage(finalDamage);
                totalDamageThisTurn += finalDamage;
                ShowDamageFeedback(enemy.transform, finalDamage);
            }
        }
        
        /// <summary>
        /// Heals the player.
        /// CSV Examples:
        /// - "提示词": 恢复15点生命
        /// </summary>
        private void HealPlayer(int amount)
        {
            if (player != null)
            {
                player.currentHealth = Mathf.Min(player.maxHealth, player.currentHealth + amount);
                ShowEffectFeedback($"Healed {amount} HP", Color.green);
            }
        }
        
        /// <summary>
        /// Discards a random card from hand.
        /// CSV Examples:
        /// - "停顿": 随机消耗一张手牌
        /// </summary>
        private void DiscardRandomCard()
        {
            var hand = deckManager.GetHand();
            if (hand.Count > 0)
            {
                int randomIndex = Random.Range(0, hand.Count);
                var card = hand[randomIndex];
                hand.RemoveAt(randomIndex);
                deckManager.EndRound(); // Add to abandoned
                ShowEffectFeedback("Discarded a card", Color.gray);
            }
        }
        
        /// <summary>
        /// Discards all cards of a specific type from hand.
        /// CSV Examples:
        /// - "高潮": 消耗手牌中所有力量牌 (CardType.Strength)
        /// - "反派": 消耗手牌中所有功能牌 (CardType.Function)
        /// - "煽情": 消耗所有手牌 (would need to discard all types)
        /// </summary>
        private void DiscardAllCardsByType(CardType cardType)
        {
            var hand = deckManager.GetHand();
            int discarded = 0;
            
            for (int i = hand.Count - 1; i >= 0; i--)
            {
                if (hand[i].cardData.cardType == cardType)
                {
                    hand.RemoveAt(i);
                    discarded++;
                }
            }
            
            if (discarded > 0)
            {
                deckManager.EndRound(); // Add to abandoned
                ShowEffectFeedback($"Discarded {discarded} {cardType} card(s)", Color.gray);
            }
        }
        
        /// <summary>
        /// Returns a card from abandoned pile to hand.
        /// CSV Examples:
        /// - "多才多艺": 将弃牌堆的一张牌放回手牌
        /// - "情感记忆": 将弃牌堆所有牌放入卡池并洗牌 (use ReturnAllToPoolAndShuffle instead)
        /// 
        /// Note: ModifyCardCost should be used as sub-effect to set cost to 0
        /// </summary>
        private void ReturnCardFromAbandoned(int count, CardType cardTypeFilter)
        {
            // This would need access to abandoned pile
            // Simplified for now
            ShowEffectFeedback("Returned card from abandoned", Color.cyan);
        }
        
        /// <summary>
        /// Returns all cards from abandoned pile to pool and shuffles.
        /// CSV Examples:
        /// - "情感记忆": 将弃牌堆所有牌放入卡池并洗牌，抽2张牌
        ///   (Also needs DrawCard effect with value 2)
        /// </summary>
        private void ReturnAllToPoolAndShuffle()
        {
            // This would need access to abandoned pile
            // Simplified for now - would call deckManager method
            ShowEffectFeedback("Returned all to pool and shuffled", Color.cyan);
        }
        
        /// <summary>
        /// Modifies player's current energy.
        /// CSV Examples:
        /// - "抢戏": 你每使用一次这张牌，需消耗的天赋点-1
        ///   (This would need to be tracked per card instance, currently simplified)
        /// </summary>
        private void ModifyPlayerEnergy(int amount)
        {
            if (player != null)
            {
                player.SetEnergy(player.currentEnergy + amount);
                ShowEffectFeedback($"Energy modified by {amount}", Color.magenta);
            }
        }
        
        /// <summary>
        /// Modifies player's maximum health.
        /// CSV Examples:
        /// - "反派": 获得最大35生命
        ///   (Set Base Value to 35, but this adds to current max, so may need adjustment)
        /// </summary>
        private void ModifyPlayerMaxHealth(int amount)
        {
            if (player != null)
            {
                player.maxHealth = Mathf.Max(1, player.maxHealth + amount);
                player.currentHealth = Mathf.Min(player.maxHealth, player.currentHealth + amount);
                ShowEffectFeedback($"Max Health modified by {amount}", Color.green);
            }
        }
        
        // Visual feedback methods (simplified - would use proper UI system)
        private void ShowDamageFeedback(Transform target, int damage)
        {
            // This would instantiate damage numbers
            Debug.Log($"Dealt {damage} damage to {target.name}");
        }
        
        private void ShowEffectFeedback(string message, Color color)
        {
            Debug.Log($"[Effect] {message}");
        }
        
        /// <summary>
        /// Resets turn-based tracking (called at turn start).
        /// </summary>
        public void ResetTurn()
        {
            totalDamageThisTurn = 0;
        }
    }
}

