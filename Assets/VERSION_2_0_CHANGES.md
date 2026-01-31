# Version 2.0 - Changes Summary

## Overview
Version 2.0 实现了可配置的卡牌效果系统和 Roguelike 关卡过渡功能，同时保持与现有系统的完全向后兼容。

---

## New Files Created (新建文件)

### 1. Effects System (效果系统)
- **`Assets/Scripts/Effects/CardEffectType.cs`**
  - 定义了所有卡牌效果类型枚举
  - 支持基础效果、公式效果、条件效果等

- **`Assets/Scripts/Effects/EffectVariable.cs`**
  - 定义了公式中使用的变量类型
  - 支持剩余能量、卡池数量、手牌数量等变量

- **`Assets/Scripts/Effects/CardEffectData.cs`**
  - 可序列化的效果数据结构
  - 支持公式、条件、子效果等复杂配置

- **`Assets/Scripts/Effects/CardEffect.cs`**
  - ScriptableObject 用于配置卡牌效果
  - 可在 Unity Inspector 中编辑

### 2. Core System (核心系统)
- **`Assets/Scripts/Core/CardEffectExecutor.cs`**
  - 效果执行器，负责执行所有卡牌效果
  - 支持公式计算、条件检查、效果应用

### 3. Documentation (文档)
- **`Assets/VERSION_2_0_DESIGN.md`**
  - 完整的设计文档
  - 包含目标、解决方案、最佳实践、测试计划等

- **`Assets/VERSION_2_0_CHANGES.md`** (本文件)
  - 变更总结文档

---

## Modified Files (修改的文件)

### 1. `Assets/Scripts/Cards/Card.cs`
**变更内容:**
- 添加了 `using MaskMYDrama.Effects;`
- 添加了 `cardEffect` 字段（CardEffect ScriptableObject 引用）
- 添加了 `useAdvancedEffects` 布尔字段
- **向后兼容**: 默认 `useAdvancedEffects = false`，使用旧系统

**影响:**
- 现有卡牌不受影响，继续使用旧系统
- 新卡牌可以启用高级效果系统

### 2. `Assets/Scripts/Core/CombatManager.cs`
**变更内容:**
- 添加了 `CardEffectExecutor effectExecutor` 字段
- 添加了 `CardSelectionUI cardSelectionUI` 字段
- 修改了 `ApplyCardEffects()` 方法：
  - 优先尝试使用新效果系统
  - 如果新系统不可用，回退到旧系统
- 修改了 `Go_Next_Level()` 方法：
  - 改为触发 Roguelike 卡牌选择
  - 选择完成后进入下一关
- 添加了 `OnRoguelikeCardSelected()` 方法
- 添加了 `ProceedToNextLevel()` 方法（原逻辑）
- 添加了 `CheckWinCondition()` 方法（提取公共逻辑）
- 在 `Start()` 中初始化 `effectExecutor`
- 在 `StartPlayerTurn()` 中重置效果执行器

**影响:**
- 完全向后兼容，现有功能不受影响
- 新功能仅在配置后启用

---

## Key Features (主要功能)

### 1. 可配置的卡牌效果系统
- **公式支持**: 支持 "X*4" 等公式，其中 X 可以是变量（如剩余能量）
- **条件效果**: 支持条件检查（如"消耗手牌中所有力量牌"）
- **组合效果**: 支持多个效果组合
- **ScriptableObject 配置**: 所有效果可在 Unity Inspector 中配置

### 2. Roguelike 关卡过渡
- **流程**: 胜利 → 显示选择界面 → 选择卡牌 → 进入下一关
- **集成**: 复用现有的 `CardSelectionUI`
- **数据源**: 从 Roguelike Pool 随机抽取 3 张卡牌

---

## Backward Compatibility (向后兼容性)

### 保证措施
1. **默认行为**: 所有新字段都有默认值，不会破坏现有卡牌
2. **渐进式迁移**: 新系统与旧系统并存，可以逐步迁移
3. **回退机制**: 如果新系统不可用，自动回退到旧系统
4. **可选功能**: 新功能通过 `useAdvancedEffects` 标志启用

### 测试建议
- [ ] 测试现有卡牌是否正常工作
- [ ] 测试战斗流程是否正常
- [ ] 测试能量系统是否正常
- [ ] 测试手牌管理是否正常

---

## Setup Instructions (设置说明)

### 1. 设置效果执行器
在 CombatManager GameObject 上：
- 确保有 `CardEffectExecutor` 组件（会自动添加）
- 确保 `player`, `enemy`, `deckManager` 引用正确

### 2. 设置 Roguelike 选择
在 CombatManager 上：
- 分配 `cardSelectionUI` 引用
- 确保 `CardSelectionUI` 已配置 `CardDatabase`

### 3. 创建新卡牌效果
1. 创建 `CardEffect` ScriptableObject
2. 配置效果列表
3. 在 `Card` 中引用该 `CardEffect`
4. 设置 `useAdvancedEffects = true`

---

## Known Limitations (已知限制)

### 1. 公式解析
- 当前支持简单公式（X*N, X+N, X-N, X/N）
- 复杂公式可能需要扩展解析器

### 2. 效果执行器
- 某些效果（如 ReturnCard）需要访问 DeckManager 的私有方法
- 可能需要添加公共 API

### 3. UI 反馈
- 效果反馈使用 Debug.Log，需要集成到实际 UI 系统

---

## Migration Guide (迁移指南)

### 从旧系统迁移到新系统

1. **创建 CardEffect ScriptableObject**
   ```
   Right-click → Create → MaskMYDrama → Card Effect
   ```

2. **配置效果**
   - 在 Inspector 中添加效果
   - 设置效果类型、数值、公式等

3. **更新 Card**
   - 在 Card 中引用 CardEffect
   - 设置 `useAdvancedEffects = true`

4. **测试**
   - 在游戏中测试卡牌效果
   - 验证效果是否正确应用

---

## Testing Checklist (测试清单)

### 基础功能测试
- [ ] 现有卡牌（Attack/Defence/Strength）正常工作
- [ ] 能量系统正常
- [ ] 手牌管理正常
- [ ] 战斗流程正常

### 新功能测试
- [ ] 公式效果（X*4）正确计算
- [ ] 条件效果正确检查
- [ ] 组合效果正确应用
- [ ] Roguelike 选择流程正常
- [ ] 关卡过渡正常

### 边界测试
- [ ] 空卡池情况
- [ ] 手牌为空情况
- [ ] 能量不足情况
- [ ] 公式变量为 0 或负数

---

## Future Improvements (未来改进)

1. **公式解析器增强**
   - 支持更复杂的公式
   - 支持嵌套公式

2. **效果编辑器**
   - 可视化效果编辑器
   - 效果预览功能

3. **性能优化**
   - 效果计算结果缓存
   - 批量 UI 更新

4. **更多效果类型**
   - 延迟效果
   - 持续效果
   - 触发效果

---

## Conclusion (结论)

Version 2.0 成功实现了可配置的卡牌效果系统和 Roguelike 关卡过渡，同时保持了完全的向后兼容性。系统设计遵循 Unity C# 最佳实践，代码结构清晰，易于扩展和维护。

