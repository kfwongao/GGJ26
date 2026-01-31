# MaskMYDrama Version 2.0 - Design Document

## Goal (目标)

### Primary Objectives
1. **可配置的卡牌效果系统**: 将 CSV 中的复杂卡牌效果（数学公式）转换为可编辑的 ScriptableObject 系统
2. **Roguelike 关卡过渡**: 将"下一关"按钮改为 Roguelike 卡牌选择流程
3. **向后兼容**: 确保新功能不影响现有功能，无 bug 和崩溃

### Key Features
- 支持复杂卡牌效果（如"X*4点生命（X=剩余全部天赋点）"）
- 支持条件效果（如"消耗手牌中所有力量牌"）
- Roguelike 选择系统集成到关卡过渡
- 所有效果可配置，无需硬编码

---

## Solution (解决方案)

### 1. 卡牌效果系统架构

#### 1.1 CardEffect ScriptableObject
创建可配置的效果系统，支持：
- **基础效果**: 伤害、防御、抽牌等
- **公式效果**: 基于变量的计算（如 X*4，X=剩余能量）
- **条件效果**: 需要满足条件才能触发
- **组合效果**: 多个效果组合

#### 1.2 效果类型枚举
```csharp
public enum CardEffectType
{
    Damage,              // 造成伤害
    Defence,             // 获得防御
    DrawCard,            // 抽牌
    Heal,                // 恢复生命
    ModifyEnergy,        // 修改能量
    DiscardCard,         // 弃牌
    ReturnCard,          // 从弃牌堆返回
    Shuffle,             // 洗牌
    ModifyCardCost,      // 修改卡牌费用
    DamageAll,           // 对所有敌人造成伤害
    RandomDamage,        // 随机伤害
    FormulaDamage,       // 公式伤害（X*4）
    ConditionalEffect    // 条件效果
}
```

#### 1.3 效果数据类
```csharp
[System.Serializable]
public class CardEffectData
{
    public CardEffectType effectType;
    public int baseValue;              // 基础值
    public string formula;             // 公式字符串（如 "X*4"）
    public EffectVariable variable;    // 变量类型（如剩余能量）
    public List<CardEffectCondition> conditions; // 条件列表
    public List<CardEffectData> subEffects;      // 子效果
}
```

### 2. Roguelike 关卡过渡系统

#### 2.1 流程设计
1. 玩家胜利后，显示"下一关"按钮
2. 点击按钮 → 显示 Roguelike 卡牌选择界面
3. 从 Roguelike Pool 随机抽取 3 张卡牌
4. 玩家选择 1 张 → 添加到卡池
5. 进入下一关

#### 2.2 集成点
- `CombatManager.Go_Next_Level()` → 改为触发 Roguelike 选择
- `CardSelectionUI` → 复用现有选择界面
- 选择完成后 → 调用原来的关卡切换逻辑

### 3. CSV 数据映射

#### 3.1 卡牌效果解析表

| CSV 描述 | 效果类型 | 参数 |
|---------|---------|------|
| "受到打击的一名对手将失去5点生命" | Damage | baseValue=5 |
| "受到打击的一名对手将失去7点生命，抽一张牌" | Damage + DrawCard | baseValue=7, drawCount=1 |
| "所有对手失去4点生命" | DamageAll | baseValue=4 |
| "受到打击的一名对手失去X*4点生命（X=剩余全部天赋点）" | FormulaDamage | formula="X*4", variable=RemainingEnergy |
| "随机令一名敌人失去20点生命 - 消耗手牌中所有力量牌" | RandomDamage + ConditionalDiscard | baseValue=20, condition=DiscardAllStrength |
| "抽3张牌，下一轮回合对方失去对你造成的同等伤害" | DrawCard + DelayedDamage | drawCount=3, delayedEffect=ReflectDamage |
| "将弃牌堆的一张牌放回手牌，此张牌的天赋点变为0" | ReturnCard + ModifyCost | returnCount=1, newCost=0 |
| "获得本轮所有伤害同等值的屏障" | FormulaDefence | formula="TotalDamageThisTurn" |
| "对所有敌人造成13点伤害，一次性使用" | DamageAll + Exhaust | baseValue=13, isExhaust=true |
| "造成你卡池中剩余卡牌数量2倍的伤害" | FormulaDamage | formula="PoolCount*2" |

---

## Best Practices (最佳实践)

### 1. 代码组织
- **单一职责**: 每个类只负责一个功能
- **可扩展性**: 使用接口和抽象类，便于添加新效果类型
- **数据驱动**: 所有效果通过 ScriptableObject 配置，不硬编码

### 2. Unity 最佳实践
- **ScriptableObject**: 用于卡牌数据和效果配置
- **事件系统**: 使用 UnityEvent 或 C# Action 进行解耦
- **编辑器工具**: 提供编辑器脚本方便配置
- **性能优化**: 避免在 Update 中频繁计算

### 3. 向后兼容
- **保持现有接口**: 不修改现有公共 API
- **渐进式迁移**: 新系统与旧系统并存
- **默认值**: 新字段提供合理默认值
- **测试覆盖**: 确保现有功能不受影响

### 4. 错误处理
- **空值检查**: 所有引用类型检查 null
- **边界检查**: 数组索引、数值范围验证
- **日志记录**: 关键操作记录 Debug.Log
- **优雅降级**: 效果解析失败时使用默认行为

---

## Test Plan (测试计划)

### 1. 单元测试
- [ ] CardEffect 解析测试
- [ ] 公式计算测试（X*4, PoolCount*2 等）
- [ ] 条件效果测试
- [ ] 组合效果测试

### 2. 集成测试
- [ ] Roguelike 选择流程测试
- [ ] 关卡过渡测试
- [ ] 卡牌效果应用测试
- [ ] UI 更新测试

### 3. 回归测试
- [ ] 现有卡牌功能测试
- [ ] 战斗流程测试
- [ ] 能量系统测试
- [ ] 手牌管理测试

### 4. 边界测试
- [ ] 空卡池情况
- [ ] 手牌为空情况
- [ ] 能量不足情况
- [ ] 公式变量为 0 或负数

---

## Points to Aware (注意事项)

### 1. 性能考虑
- **公式解析**: 避免在运行时解析字符串公式，使用预编译的表达式
- **效果缓存**: 复杂计算的结果可以缓存
- **UI 更新**: 批量更新 UI，避免每帧更新

### 2. 数据一致性
- **ScriptableObject 引用**: 确保所有卡牌数据正确引用
- **卡池同步**: Roguelike 选择后正确更新卡池
- **状态管理**: 关卡切换时正确保存/恢复状态

### 3. 用户体验
- **视觉反馈**: 卡牌效果应用时提供清晰的视觉反馈
- **错误提示**: 效果无法应用时给出明确提示
- **加载时间**: Roguelike 选择界面快速加载

### 4. 扩展性
- **新效果类型**: 系统应易于添加新效果类型
- **公式系统**: 支持更复杂的公式（未来可能）
- **多语言**: 考虑效果描述的多语言支持

### 5. 调试支持
- **日志系统**: 详细的效果应用日志
- **编辑器预览**: 在编辑器中预览效果结果
- **测试模式**: 提供测试模式快速验证效果

---

## Implementation Checklist (实现清单)

### Phase 1: 卡牌效果系统
- [ ] 创建 `CardEffectType` 枚举
- [ ] 创建 `CardEffectData` 类
- [ ] 创建 `CardEffectExecutor` 类
- [ ] 创建效果配置 ScriptableObject
- [ ] 实现基础效果（伤害、防御、抽牌）
- [ ] 实现公式效果（X*4, PoolCount*2）
- [ ] 实现条件效果
- [ ] 更新 `Card.cs` 集成效果系统
- [ ] 更新 `CombatManager` 应用效果

### Phase 2: Roguelike 关卡过渡
- [ ] 修改 `CombatManager.Go_Next_Level()` 
- [ ] 集成 `CardSelectionUI` 到关卡过渡
- [ ] 实现 Roguelike 选择流程
- [ ] 更新 UI 按钮逻辑
- [ ] 测试关卡切换

### Phase 3: CSV 数据迁移
- [ ] 解析 CSV 数据
- [ ] 创建所有卡牌的 ScriptableObject
- [ ] 配置卡牌效果
- [ ] 验证效果正确性

### Phase 4: 测试与优化
- [ ] 运行所有测试
- [ ] 修复发现的 bug
- [ ] 性能优化
- [ ] 代码审查
- [ ] 文档更新

---

## File Structure (文件结构)

```
Assets/Scripts/
├── Cards/
│   ├── Card.cs (更新 - 添加效果引用)
│   ├── CardInstance.cs
│   └── CardEffect.cs (新建 - 效果 ScriptableObject)
├── Core/
│   ├── CombatManager.cs (更新 - 集成效果系统)
│   ├── DeckManager.cs
│   ├── CardDatabase.cs
│   └── CardEffectExecutor.cs (新建 - 效果执行器)
├── Effects/
│   ├── CardEffectType.cs (新建 - 效果类型枚举)
│   ├── CardEffectData.cs (新建 - 效果数据类)
│   └── CardEffectConditions.cs (新建 - 条件系统)
└── UI/
    ├── CombatUI.cs
    └── CardSelectionUI.cs (更新 - 关卡过渡集成)
```

---

## Risk Assessment (风险评估)

### 高风险
1. **公式解析复杂性**: 需要可靠的公式解析系统
   - 缓解: 使用预定义公式类型，避免字符串解析

2. **向后兼容性**: 可能破坏现有功能
   - 缓解: 充分测试，渐进式迁移

### 中风险
1. **性能影响**: 复杂效果可能影响性能
   - 缓解: 优化计算，使用缓存

2. **状态管理**: 关卡切换时状态同步
   - 缓解: 明确的状态管理流程

### 低风险
1. **UI 更新**: UI 可能需要调整
   - 缓解: 复用现有 UI 组件

---

## Conclusion (结论)

Version 2.0 将显著增强游戏的可配置性和扩展性，同时保持与现有系统的兼容性。通过 ScriptableObject 驱动的效果系统，设计师可以在不修改代码的情况下创建复杂的卡牌效果。Roguelike 关卡过渡将提升游戏的策略深度和重玩价值。

