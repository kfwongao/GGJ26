# Card Selection UI 设置指南

## 快速设置（推荐）

### 方法 1: 使用自动化工具（最简单）

1. **打开 Unity Editor**
2. **菜单栏选择**: `MaskMYDrama > Setup Combat UI`
   - 这会创建完整的战斗 UI，包括 Card Selection UI
3. **或者只创建 Card Selection UI**: `MaskMYDrama > Setup Card Selection UI Only`

### 方法 2: 手动设置

如果场景中已有 Canvas，可以手动添加 Card Selection UI：

1. **在 Hierarchy 中找到 Canvas**
2. **右键 Canvas** → `Create Empty`
3. **重命名**为 `CardSelectionPanel`
4. **添加 Component**: `CardSelectionUI` (MaskMYDrama.UI)
5. **按照下面的结构创建子对象**

---

## UI 结构

```
Canvas
└── CardSelectionPanel (CardSelectionUI component)
    ├── BackgroundOverlay (Image - 半透明黑色背景)
    └── ContentContainer
        ├── TitleText (TextMeshProUGUI - "选择一张卡牌加入牌组")
        ├── CardOptionsParent (HorizontalLayoutGroup - 卡牌容器)
        │   └── (3个 CardOptionPrefab 会在这里动态生成)
        └── ConfirmButton (Button - "确认选择")
```

---

## 组件配置

### CardSelectionUI 组件设置

在 Inspector 中配置以下字段：

#### UI References (UI 引用)
- **Selection Panel**: 拖入 `CardSelectionPanel` GameObject
- **Card Options Parent**: 拖入 `ContentContainer/CardOptionsParent` Transform
- **Card Option Prefab**: 拖入 `Assets/Prefabs/CardOptionPrefab.prefab`
- **Title Text**: 拖入 `ContentContainer/TitleText` TextMeshProUGUI
- **Confirm Button**: 拖入 `ContentContainer/ConfirmButton` Button
- **Background Overlay**: 拖入 `BackgroundOverlay` Image

#### Layout Settings (布局设置)
- **Card Spacing**: 300 (卡牌间距)
- **Normal Card Scale**: 1.0 (未选中时的缩放)
- **Selected Card Scale**: 1.2 (选中时的缩放)
- **Selection Animation Duration**: 0.3 (动画时长)

#### Managers (管理器)
- **Deck Manager**: 拖入场景中的 `DeckManager` 组件
- **Card Database**: 拖入 `CardDatabase` ScriptableObject

---

## 预制体设置

### CardOptionPrefab 预制体

如果使用自动化工具，预制体会自动创建在 `Assets/Prefabs/CardOptionPrefab.prefab`

**预制体结构:**
```
CardOptionUI (CardOptionUI component)
├── CardName (TextMeshProUGUI)
├── CardDescription (TextMeshProUGUI)
├── EnergyCost (TextMeshProUGUI)
└── (Image component on root - 作为背景和按钮)
```

**CardOptionUI 组件字段:**
- **Card Name Text**: 卡牌名称文本
- **Description Text**: 卡牌描述文本
- **Energy Cost Text**: 能量消耗文本
- **Card Background**: 卡牌背景 Image
- **Normal Color**: 未选中颜色 (白色)
- **Selected Color**: 选中颜色 (绿色)

---

## 与 CombatManager 连接

### 自动连接（使用工具时）

如果使用 `Setup Combat UI` 工具，连接会自动完成。

### 手动连接

1. **选择 CombatManager GameObject**
2. **在 CombatManager 组件中找到 "Roguelike Selection" 部分**
3. **将 Card Selection UI** 拖入 `Card Selection UI` 字段

---

## 测试

### 测试步骤

1. **确保 CardDatabase 已配置 Roguelike Pool 卡牌**
2. **运行游戏**
3. **在战斗中击败敌人**
4. **点击 "下一关" 按钮**
5. **应该看到 Card Selection UI 显示 3 张卡牌**
6. **点击选择一张卡牌**
7. **点击 "确认选择" 按钮**
8. **应该进入下一关**

### 常见问题

**问题**: UI 不显示
- **解决**: 检查 `selectionPanel` 是否被正确分配
- **解决**: 检查 Canvas 是否存在且启用

**问题**: 卡牌不显示
- **解决**: 检查 `cardOptionPrefab` 是否分配
- **解决**: 检查 `CardDatabase` 是否有 Roguelike Pool 卡牌

**问题**: 点击无反应
- **解决**: 检查 EventSystem 是否存在
- **解决**: 检查 CardOptionUI 组件是否正确配置

**问题**: 选择后不跳转关卡
- **解决**: 检查 CombatManager 的 `cardSelectionUI` 是否分配
- **解决**: 检查 `OnCardSelected` 事件是否正确连接

---

## 自定义样式

### 修改卡牌外观

1. **打开** `Assets/Prefabs/CardOptionPrefab.prefab`
2. **修改背景颜色**: 选择根对象，修改 Image 组件的 Color
3. **修改文本样式**: 选择各个 TextMeshProUGUI，修改字体、大小、颜色等
4. **调整尺寸**: 修改 RectTransform 的 Size Delta

### 修改布局

在 `CardSelectionUI` 组件中：
- **Card Spacing**: 调整卡牌间距
- **Normal/Selected Scale**: 调整选中/未选中的缩放比例
- **Animation Duration**: 调整动画速度

### 修改背景

选择 `BackgroundOverlay` GameObject：
- **修改颜色**: 调整 Image 组件的 Color（Alpha 值控制透明度）
- **修改大小**: 调整 RectTransform（应该覆盖整个屏幕）

---

## 完成！

现在 Card Selection UI 已经设置完成。当玩家胜利后，会自动显示卡牌选择界面，玩家选择一张卡牌后进入下一关。

