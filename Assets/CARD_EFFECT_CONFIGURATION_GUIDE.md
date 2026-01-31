# Card Effect Configuration Guide

## 概述

本指南说明如何根据 `MaskMYDrama - Art Design Info.csv` 配置卡牌的 ScriptableObject。

---

## 两种配置方式

### 方式 1: 简单卡牌（Legacy 系统）

适用于基础效果的卡牌，直接在 Card ScriptableObject 中设置数值。

**适用卡牌:**
- 亮相/Stage pose
- 屏障/Block
- Monologue 独白
- Dialogue 对白
- 暗场/Fade out
- 停顿/Stop
- 潜台词Subtext
- 情感记忆/Emotional Memory
- 讽刺/Irony

**配置步骤:**
1. 创建 Card ScriptableObject
2. 设置基本属性（名称、类型、能量消耗）
3. 设置效果数值（attackValue, defenceValue, drawCardCount）
4. 设置 `useAdvancedEffects = false`
5. 设置 `poolType`（StartingPool 或 RoguelikePool）

### 方式 2: 复杂卡牌（Advanced Effect 系统）

适用于有公式、条件、多效果的卡牌，使用 CardEffect ScriptableObject。

**适用卡牌:**
- 即兴表演/Improvisation
- 高潮/Climax
- 主角/Protagonist
- 多才多艺/Versatile
- 横穿舞台/Cross
- 提示词/Cue
- 假戏真做
- 剧透/Spoiled Alert
- 反派/Antagonist
- 所有 Function 卡牌

**配置步骤:**
1. 创建 CardEffect ScriptableObject
2. 在 CardEffect 中添加 CardEffectData
3. 配置每个效果的参数
4. 在 Card ScriptableObject 中：
   - 设置 `useAdvancedEffects = true`
   - 分配 CardEffect 到 `cardEffect` 字段

---

## 详细配置示例

### 1. 即兴表演/Improvisation
**CSV 描述:** 受到打击的一名对手失去X*4点生命（X=剩余全部天赋点）

**配置:**
```
CardEffect ScriptableObject:
  - Effect 1:
    * Effect Type: FormulaDamage
    * Formula: "X*4"
    * Variable: RemainingEnergy
    * Target: Enemy
    * Base Value: 0 (not used, formula takes precedence)
```

### 2. 高潮/Climax
**CSV 描述:** 随机令一名敌人失去20点生命 - 消耗手牌中所有力量牌

**配置:**
```
CardEffect ScriptableObject:
  - Effect 1:
    * Effect Type: RandomDamage
    * Base Value: 20
    * Target: RandomEnemy
  - Effect 2 (Sub Effect):
    * Effect Type: DiscardAllByType
    * Card Type Filter: Strength
```

### 3. 主角/Protagonist
**CSV 描述:** 抽3张牌，下一轮回合对方失去对你造成的同等伤害

**配置:**
```
CardEffect ScriptableObject:
  - Effect 1:
    * Effect Type: DrawCard
    * Base Value: 3
  - Effect 2:
    * Effect Type: DelayedDamage (需要实现延迟效果系统)
    * Is Delayed: true
    * Note: 延迟反射伤害功能需要额外实现
```

### 4. 多才多艺/Versatile
**CSV 描述:** 受到打击的对手失去9点生命，将弃牌堆的一张牌放回手牌，此张牌的天赋点变为0

**配置:**
```
CardEffect ScriptableObject:
  - Effect 1:
    * Effect Type: Damage
    * Base Value: 9
    * Target: Enemy
  - Effect 2 (Sub Effect):
    * Effect Type: ReturnCard
    * Base Value: 1
  - Effect 3 (Sub Effect):
    * Effect Type: ModifyCardCost
    * Base Value: 0
```

### 5. 假戏真做
**CSV 描述:** 造成你卡池中剩余卡牌数量2倍的伤害，获得4点屏障

**配置:**
```
CardEffect ScriptableObject:
  - Effect 1:
    * Effect Type: FormulaDamage
    * Formula: "PoolCount*2"
    * Variable: PoolCount
    * Target: Enemy
  - Effect 2:
    * Effect Type: Defence
    * Base Value: 4
```

### 6. 提示词/Cue
**CSV 描述:** 获得本轮所有伤害同等值的屏障，并恢复15点生命

**配置:**
```
CardEffect ScriptableObject:
  - Effect 1:
    * Effect Type: FormulaDefence
    * Formula: "TotalDamageThisTurn"
    * Variable: TotalDamageThisTurn
  - Effect 2:
    * Effect Type: Heal
    * Base Value: 15
```

### 7. 剧透/Spoiled Alert
**CSV 描述:** 损失5点生命，打出剩余生命值的伤害给生命最少的敌人，获得12点屏障

**配置:**
```
CardEffect ScriptableObject:
  - Effect 1:
    * Effect Type: Damage
    * Base Value: 5
    * Target: Self
  - Effect 2:
    * Effect Type: FormulaDamage
    * Formula: "RemainingHealth"
    * Variable: RemainingHealth
    * Target: LowestHealthEnemy
  - Effect 3:
    * Effect Type: Defence
    * Base Value: 12
```

### 8. 反派/Antagonist
**CSV 描述:** 获得最大35生命，本轮减伤30%。消耗手牌中所有功能牌

**配置:**
```
CardEffect ScriptableObject:
  - Effect 1:
    * Effect Type: ModifyMaxHealth
    * Base Value: 35
  - Effect 2:
    * Effect Type: DamageReduction
    * Percentage Modifier: 30
  - Effect 3:
    * Effect Type: DiscardAllByType
    * Card Type Filter: Function
```

---

## 公式变量参考

| 变量 | CSV 对应 | 使用示例 |
|------|---------|---------|
| RemainingEnergy | 剩余全部天赋点 | 即兴表演: X*4 |
| PoolCount | 卡池中剩余卡牌数量 | 假戏真做: PoolCount*2 |
| TotalDamageThisTurn | 本轮所有伤害 | 提示词, 横穿舞台 |
| RemainingHealth | 剩余生命值 | 剧透 |
| HandCount | 手牌数量 | - |
| AbandonedCount | 弃牌堆数量 | - |
| StrengthCardCount | 手牌中力量牌数量 | 高潮条件检查 |
| FunctionCardCount | 手牌中功能牌数量 | 反派条件检查 |

---

## 效果类型快速参考

| 效果类型 | CSV 示例 | 关键参数 |
|---------|---------|---------|
| Damage | 亮相, Monologue | Base Value, Target |
| DamageAll | Dialogue, 笑场 | Base Value |
| RandomDamage | 高潮 | Base Value |
| FormulaDamage | 即兴表演, 假戏真做 | Formula, Variable, Target |
| Defence | 屏障, 暗场 | Base Value |
| FormulaDefence | 提示词, 横穿舞台 | Formula, Variable |
| Heal | 提示词 | Base Value |
| DrawCard | Monologue, 主角 | Base Value |
| DiscardCard | 停顿 | - |
| DiscardAllByType | 高潮, 反派 | Card Type Filter |
| ReturnCard | 多才多艺 | Base Value, Card Type Filter |
| ModifyCardCost | 多才多艺 | Base Value |
| ModifyMaxHealth | 反派 | Base Value |
| DamageReduction | 反派, 讽刺 | Percentage Modifier |
| DamageBoost | 暗场, 讽刺 | Percentage Modifier |
| Exhaust | 笑场 | - |

---

## 注意事项

1. **延迟效果**: 某些卡牌（如"主角"、"横穿舞台"）需要延迟效果系统，目前需要额外实现
2. **条件检查**: 某些卡牌需要条件检查（如"高潮"需要手牌中有力量牌），使用 Conditions 列表
3. **子效果**: 多效果卡牌使用 Sub Effects 列表，按顺序执行
4. **公式解析**: 当前支持简单公式（X*N, X+N, X-N, X/N），复杂公式可能需要扩展
5. **百分比效果**: 增伤/减伤效果需要 Buff 系统支持，目前仅显示反馈

---

## 完整配置清单

### Starting Pool 卡牌（开局卡池）
- [x] 亮相/Stage pose - 简单卡牌
- [x] Monologue 独白 - 简单卡牌（带抽牌）
- [x] Dialogue 对白 - 简单卡牌（所有敌人）
- [x] 屏障/Block - 简单卡牌
- [x] 暗场/Fade out - 简单卡牌（带抽牌）
- [x] 停顿/Stop - 简单卡牌（带弃牌）
- [x] 潜台词Subtext - 简单卡牌
- [x] 情感记忆/Emotional Memory - 需要 ReturnAllToPool + DrawCard
- [x] 讽刺/Irony - 需要 DamageBoost + DamageReduction

### Roguelike Pool 卡牌
- [x] 即兴表演/Improvisation - 公式伤害
- [x] 高潮/Climax - 随机伤害 + 弃牌
- [x] 主角/Protagonist - 抽牌 + 延迟反射
- [x] 多才多艺/Versatile - 伤害 + 返回 + 修改费用
- [x] 横穿舞台/Cross - 延迟公式防御
- [x] 提示词/Cue - 公式防御 + 恢复
- [x] 笑场/Corpsing - 所有敌人伤害 + 一次性
- [x] 抢戏/Upstaging - 抽牌 + 费用递减
- [x] 魅惑 - 需要实现
- [x] 煽情 - 抽牌 + 修改费用 + 弃牌
- [x] 假戏真做 - 公式伤害 + 防御
- [x] 剧透/Spoiled Alert - 自伤 + 公式伤害 + 防御
- [x] 反派/Antagonist - 修改生命 + 减伤 + 弃牌

---

## 快速配置工具

使用菜单 `MaskMYDrama > Create All Cards from CSV` 可以自动创建所有卡牌的基础版本。

复杂效果需要手动创建 CardEffect ScriptableObject 并配置。

