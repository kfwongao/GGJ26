# TextMeshPro 字体替换指南

## 问题描述

TextMeshPro 组件默认使用 Liberation Sans 字体，该字体不支持简体中文字符。需要将所有 TextMeshPro 组件的字体替换为 Noto Sans SC 以正确显示中文。

## 解决方案

### 方法 1: 使用字体替换工具（推荐）

1. **打开字体替换工具**
   - 菜单栏：`MaskMYDrama > Replace TextMeshPro Font to Noto Sans SC`
   - 工具会自动查找并加载 Noto Sans SC 字体

2. **选择替换范围**
   - ✅ **场景中的对象**：替换当前场景中所有 TextMeshPro 组件
   - ✅ **预制体**：替换选中的预制体
   - ⬜ **项目中的所有预制体**：替换项目中所有预制体（谨慎使用）

3. **点击"开始替换"**
   - 工具会自动替换所有匹配的 TextMeshPro 组件
   - 显示替换数量确认对话框

### 方法 2: 快速替换当前场景

- 菜单栏：`MaskMYDrama > Quick Replace Font in Scene`
- 一键替换当前场景中所有 TextMeshPro 组件的字体

### 方法 3: 手动替换（单个组件）

1. 在 Hierarchy 或 Scene 中选择包含 TextMeshPro 组件的对象
2. 在 Inspector 中找到 TextMeshPro 组件
3. 在 `Font Asset` 字段中，点击圆形选择按钮
4. 搜索并选择 `NotoSansSC-VariableFont_wght SDF`

## 字体资源位置

Noto Sans SC 字体资源位于：
```
Assets/Fonts/Noto_Sans_SC/NotoSansSC-VariableFont_wght SDF.asset
```

## 自动分配

使用 `UISetupHelper` 创建的新 UI 元素会自动分配 Noto Sans SC 字体：
- 所有通过 `MaskMYDrama > Setup Combat UI` 创建的 UI
- 所有通过 `MaskMYDrama > Create Card UI Prefab` 创建的卡牌预制体
- 所有通过 `MaskMYDrama > Create Card Option Prefab` 创建的卡牌选项预制体

## 验证字体是否正确

替换后，检查以下内容：
1. 场景中的中文文本是否正常显示
2. 预制体中的中文文本是否正常显示
3. 运行时中文文本是否正常显示

如果仍有问题：
- 检查字体资源是否存在
- 确认字体资源已正确导入
- 检查 TextMeshPro 的 Font Asset 字段是否正确分配

## 注意事项

1. **预制体修改**：如果修改了预制体，记得保存（Ctrl+S）
2. **场景保存**：修改场景后记得保存场景
3. **批量操作**：替换项目中的所有预制体可能需要较长时间，请耐心等待
4. **备份**：在进行大规模替换前，建议先备份项目

## 常见问题

### Q: 工具找不到 Noto Sans SC 字体？
A: 检查字体资源是否存在于 `Assets/Fonts/Noto_Sans_SC/` 目录下。如果不存在，需要先导入字体资源。

### Q: 替换后中文仍然不显示？
A: 
1. 确认字体资源已正确导入
2. 检查 TextMeshPro 的 Font Asset 字段是否已更新
3. 尝试重新生成字体图集（在字体资源的 Inspector 中点击 "Generate Font Atlas"）

### Q: 如何为新的 TextMeshPro 组件自动使用 Noto Sans SC？
A: 在 TextMeshPro 设置中设置默认字体：
1. 菜单栏：`Window > TextMeshPro > Font Asset Creator`（如果需要创建字体）
2. 菜单栏：`Window > TextMeshPro > Settings`
3. 在 `Default Font Asset` 中选择 Noto Sans SC

