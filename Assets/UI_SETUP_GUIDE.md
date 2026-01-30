# MaskMYDrama - UI Setup Guide

## Quick Setup (Automated)

### Option 1: Use the Automated Setup Script (Recommended)

1. **Open Unity Editor**
2. **Go to menu:** `MaskMYDrama > Setup Combat UI`
3. **Done!** All UI GameObjects will be created automatically

The script will create:
- ✅ Canvas with proper settings
- ✅ EventSystem
- ✅ All UI elements (Top Bar, Combat Area, Card Hand, Bottom Bar)
- ✅ Card UI Prefab
- ✅ Manager GameObjects
- ✅ All component references linked

**After running the script:**
1. Assign the Card Prefab: In CombatUI component, drag `Assets/Prefabs/CardUIPrefab.prefab` to the `Card UIPrefab` field
2. Assign Starting Cards: In DeckManager component, create some Card ScriptableObjects and assign them to `Starting Cards` array
3. Configure Player/Enemy: Set maxHealth, maxEnergy, baseAttackDamage in Inspector

---

## Manual Setup (Step-by-Step)

If you prefer to set up manually or need to customize the UI:

### Step 1: Create Canvas

1. **Right-click in Hierarchy** → `UI > Canvas`
2. **Select Canvas** → In Inspector:
   - **Render Mode:** Screen Space - Overlay
   - **Canvas Scaler:**
     - **UI Scale Mode:** Scale With Screen Size
     - **Reference Resolution:** 1920 x 1080
     - **Match:** 0.5 (Width/Height)

### Step 2: Create EventSystem

1. **Right-click in Hierarchy** → `UI > Event System`
   - This is automatically created with Canvas, but verify it exists

### Step 3: Create CombatUI GameObject

1. **Right-click on Canvas** → `Create Empty`
2. **Rename** to `CombatUI`
3. **Add Component:** `CombatUI` (MaskMYDrama.UI)

### Step 4: Create Top Bar

1. **Right-click on Canvas** → `UI > Panel`
2. **Rename** to `TopBar`
3. **RectTransform Settings:**
   - **Anchor:** Top-Stretch (click top anchor preset)
   - **Height:** 80
   - **Position Y:** 0
4. **Image Component:**
   - **Color:** Dark gray (0.2, 0.2, 0.2, 0.8)

#### Top Bar - Player Info (Left Side)

1. **Right-click on TopBar** → `Create Empty`
2. **Rename** to `PlayerInfo`
3. **RectTransform:**
   - **Anchor:** Left-Stretch
   - **Width:** 30% (0 to 0.3)
   - **Stretch:** Full height

**Player Name:**
1. **Right-click on PlayerInfo** → `UI > Text - TextMeshPro`
2. **Rename** to `PlayerNameText`
3. **RectTransform:**
   - **Anchor:** Top-Stretch
   - **Height:** 50%
   - **Position:** Top half
4. **TextMeshProUGUI:**
   - **Text:** "Player"
   - **Font Size:** 24
   - **Color:** White
   - **Alignment:** Left

**Level Text:**
1. **Right-click on PlayerInfo** → `UI > Text - TextMeshPro`
2. **Rename** to `LevelText`
3. **RectTransform:**
   - **Anchor:** Bottom-Stretch
   - **Height:** 50%
   - **Position:** Bottom half
4. **TextMeshProUGUI:**
   - **Text:** "Level 1"
   - **Font Size:** 18
   - **Color:** White

#### Top Bar - Player Health Bar (Right Side)

1. **Right-click on TopBar** → `Create Empty`
2. **Rename** to `PlayerHealthBar`
3. **Add Component:** `HealthBar` (MaskMYDrama.UI)
4. **RectTransform:**
   - **Anchor:** Right-Stretch
   - **Width:** 30% (0.7 to 1.0)
   - **Padding:** 10 on all sides

**Health Slider:**
1. **Right-click on PlayerHealthBar** → `UI > Slider`
2. **Rename** to `HealthSlider`
3. **RectTransform:** Full stretch (fill parent)
4. **Slider Settings:**
   - **Min Value:** 0
   - **Max Value:** 100
   - **Value:** 100

**Health Text:**
1. **Right-click on PlayerHealthBar** → `UI > Text - TextMeshPro`
2. **Rename** to `HealthText`
3. **RectTransform:** Full stretch (fill parent)
4. **TextMeshProUGUI:**
   - **Text:** "100/100"
   - **Font Size:** 20
   - **Color:** White
   - **Alignment:** Center

**Link HealthBar Component:**
- Drag `HealthSlider` to `Health Slider` field
- Drag `HealthText` to `Health Text` field

### Step 5: Create Combat Area

1. **Right-click on Canvas** → `Create Empty`
2. **Rename** to `CombatArea`
3. **RectTransform:**
   - **Anchor:** Middle-Stretch
   - **Top:** 20% (0.2)
   - **Bottom:** 20% (0.8)
   - **Stretch:** Full width

#### Combat Area - Player Area (Left)

1. **Right-click on CombatArea** → `Create Empty`
2. **Rename** to `PlayerArea`
3. **RectTransform:**
   - **Anchor:** Left-Stretch
   - **Width:** 50% (0 to 0.5)

**Player GameObject:**
1. **Right-click on PlayerArea** → `UI > Image`
2. **Rename** to `Player`
3. **Add Component:** `Player` (MaskMYDrama.Combat)
4. **RectTransform:**
   - **Width:** 200
   - **Height:** 300
   - **Anchor:** Bottom-Center
   - **Position Y:** 50
5. **Image Component:**
   - **Color:** Light blue (placeholder - replace with art later)
6. **Player Component Settings:**
   - **Max Health:** 100
   - **Max Energy:** 3

#### Combat Area - Enemy Area (Right)

1. **Right-click on CombatArea** → `Create Empty`
2. **Rename** to `EnemyArea`
3. **RectTransform:**
   - **Anchor:** Right-Stretch
   - **Width:** 50% (0.5 to 1.0)

**Enemy GameObject:**
1. **Right-click on EnemyArea** → `UI > Image`
2. **Rename** to `Enemy`
3. **Add Component:** `Enemy` (MaskMYDrama.Combat)
4. **RectTransform:**
   - **Width:** 200
   - **Height:** 300
   - **Anchor:** Bottom-Center
   - **Position Y:** 50
5. **Image Component:**
   - **Color:** Light red (placeholder - replace with art later)
6. **Enemy Component Settings:**
   - **Max Health:** 100
   - **Base Attack Damage:** 10

**Enemy Health Bar:**
1. **Right-click on EnemyArea** → `Create Empty`
2. **Rename** to `EnemyHealthBar`
3. **Add Component:** `HealthBar` (MaskMYDrama.UI)
4. **RectTransform:**
   - **Width:** 300
   - **Height:** 40
   - **Anchor:** Bottom-Center
   - **Position Y:** 10

**Enemy Health Slider:**
1. **Right-click on EnemyHealthBar** → `UI > Slider`
2. **Rename** to `HealthSlider`
3. **RectTransform:** Full stretch
4. **Slider Settings:** Same as player health slider

**Enemy Health Text:**
1. **Right-click on EnemyHealthBar** → `UI > Text - TextMeshPro`
2. **Rename** to `HealthText`
3. **RectTransform:** Full stretch
4. **TextMeshProUGUI:** Same settings as player health text

**Link Enemy HealthBar Component:**
- Drag `HealthSlider` to `Health Slider` field
- Drag `HealthText` to `Health Text` field

### Step 6: Create Card Hand

1. **Right-click on Canvas** → `Create Empty`
2. **Rename** to `CardHand`
3. **RectTransform:**
   - **Anchor:** Bottom-Stretch
   - **Height:** 20% (0 to 0.2)
   - **Stretch:** Full width
4. **Add Component:** `Horizontal Layout Group`
   - **Spacing:** 20
   - **Padding:** 20 (all sides)
   - **Child Alignment:** Middle Center
   - **Child Control Width:** Unchecked
   - **Child Control Height:** Unchecked
   - **Child Force Expand:** Both unchecked

### Step 7: Create Bottom Bar

1. **Right-click on Canvas** → `UI > Panel`
2. **Rename** to `BottomBar`
3. **RectTransform:**
   - **Anchor:** Bottom-Stretch
   - **Height:** 20% (0 to 0.2)
   - **Stretch:** Full width
4. **Image Component:**
   - **Color:** Dark gray (0.2, 0.2, 0.2, 0.8)

#### Bottom Bar - Energy Display (Left)

1. **Right-click on BottomBar** → `Create Empty`
2. **Rename** to `EnergyDisplay`
3. **Add Component:** `EnergyDisplay` (MaskMYDrama.UI)
4. **RectTransform:**
   - **Anchor:** Left-Stretch
   - **Width:** 20% (0 to 0.2)
   - **Padding:** 20 left, 10 right/top/bottom

**Energy Text:**
1. **Right-click on EnergyDisplay** → `UI > Text - TextMeshPro`
2. **Rename** to `EnergyText`
3. **RectTransform:** Full stretch
4. **TextMeshProUGUI:**
   - **Text:** "3/3"
   - **Font Size:** 32
   - **Color:** Cyan
   - **Alignment:** Center

**Link EnergyDisplay Component:**
- Drag `EnergyText` to `Energy Text` field

#### Bottom Bar - Deck Counts (Left Center)

1. **Right-click on BottomBar** → `Create Empty`
2. **Rename** to `DeckCounts`
3. **RectTransform:**
   - **Anchor:** Middle-Left
   - **Width:** 20% (0.2 to 0.4)
   - **Padding:** 10 all sides
4. **Add Component:** `Vertical Layout Group`
   - **Spacing:** 5
   - **Child Control Width:** Checked

**Pool Count Text:**
1. **Right-click on DeckCounts** → `UI > Text - TextMeshPro`
2. **Rename** to `PoolCount`
3. **TextMeshProUGUI:**
   - **Text:** "Pool: 12"
   - **Font Size:** 18
   - **Color:** White

**Abandoned Count Text:**
1. **Right-click on DeckCounts** → `UI > Text - TextMeshPro`
2. **Rename** to `AbandonedCount`
3. **TextMeshProUGUI:**
   - **Text:** "Abandoned: 0"
   - **Font Size:** 18
   - **Color:** White

#### Bottom Bar - End Turn Button (Right)

1. **Right-click on BottomBar** → `UI > Button - TextMeshPro`
2. **Rename** to `EndTurnButton`
3. **RectTransform:**
   - **Anchor:** Right-Stretch
   - **Width:** 20% (0.8 to 1.0)
   - **Padding:** 10 left/top/bottom, 20 right
4. **Button Component:**
   - **Color:** Green (0.3, 0.6, 0.3)
5. **Button Text:**
   - **Text:** "End Turn"
   - **Font Size:** 24
   - **Color:** White
   - **Alignment:** Center

### Step 8: Create Card UI Prefab

1. **Right-click in Project** → `Create > Folder` → Name: `Prefabs`
2. **Right-click on Prefabs** → `Create > UI > Image`
3. **Rename** to `CardUIPrefab`
4. **RectTransform:**
   - **Width:** 200
   - **Height:** 280
5. **Image Component:**
   - **Color:** Light gray (0.9, 0.9, 0.9)

**Card Name:**
1. **Right-click on CardUIPrefab** → `UI > Text - TextMeshPro`
2. **Rename** to `CardName`
3. **RectTransform:**
   - **Anchor:** Top-Stretch
   - **Height:** 20% (0.8 to 1.0)
   - **Padding:** 10 left/right, 0 top/bottom
4. **TextMeshProUGUI:**
   - **Text:** "Card Name"
   - **Font Size:** 20
   - **Color:** Black
   - **Font Style:** Bold

**Card Description:**
1. **Right-click on CardUIPrefab** → `UI > Text - TextMeshPro`
2. **Rename** to `CardDescription`
3. **RectTransform:**
   - **Anchor:** Middle-Stretch
   - **Top:** 80% (0.8)
   - **Bottom:** 30% (0.3)
   - **Padding:** 10 all sides
4. **TextMeshProUGUI:**
   - **Text:** "Card Description"
   - **Font Size:** 14
   - **Color:** Black
   - **Alignment:** Top Left

**Energy Cost:**
1. **Right-click on CardUIPrefab** → `UI > Text - TextMeshPro`
2. **Rename** to `EnergyCost`
3. **RectTransform:**
   - **Anchor:** Top-Right
   - **Width:** 20%
   - **Height:** 20%
   - **Position:** Top-right corner
4. **TextMeshProUGUI:**
   - **Text:** "1"
   - **Font Size:** 24
   - **Color:** Yellow
   - **Alignment:** Center
   - **Font Style:** Bold

**Add CardUI Component:**
1. **Select CardUIPrefab**
2. **Add Component:** `CardUI` (MaskMYDrama.UI)
3. **Link References:**
   - Drag `CardName` to `Card Name Text`
   - Drag `CardDescription` to `Description Text`
   - Drag `EnergyCost` to `Energy Cost Text`
   - Drag main `Image` to `Card Background`

**Save as Prefab:**
1. **Drag CardUIPrefab from Hierarchy to Prefabs folder**
2. **Delete from Hierarchy** (keep prefab in Project)

### Step 9: Create Manager GameObjects

**GameManager:**
1. **Right-click in Hierarchy** → `Create Empty`
2. **Rename** to `GameManager`
3. **Add Component:** `GameManager` (MaskMYDrama.Core)

**CombatManager:**
1. **Right-click in Hierarchy** → `Create Empty`
2. **Rename** to `CombatManager`
3. **Add Component:** `CombatManager` (MaskMYDrama.Core)
4. **Add Component:** `DeckManager` (MaskMYDrama.Core)

### Step 10: Link All References

**CombatUI Component:**
1. **Select CombatUI GameObject**
2. **In CombatUI Component, link:**
   - **Combat Manager:** Drag `CombatManager` from Hierarchy
   - **Deck Manager:** Drag `CombatManager` (has DeckManager component)
   - **Player:** Drag `Player` from CombatArea
   - **Enemy:** Drag `Enemy` from CombatArea
   - **Player Health Bar:** Drag `PlayerHealthBar` from TopBar
   - **Enemy Health Bar:** Drag `EnemyHealthBar` from EnemyArea
   - **Energy Display:** Drag `EnergyDisplay` from BottomBar
   - **Card Hand Parent:** Drag `CardHand` from Canvas
   - **Card UI Prefab:** Drag `CardUIPrefab` from Prefabs folder
   - **End Turn Button:** Drag `EndTurnButton` from BottomBar
   - **Pool Count Text:** Drag `PoolCount` from BottomBar/DeckCounts
   - **Abandoned Count Text:** Drag `AbandonedCount` from BottomBar/DeckCounts
   - **Player Name Text:** Drag `PlayerNameText` from TopBar/PlayerInfo
   - **Level Text:** Drag `LevelText` from TopBar/PlayerInfo

**CombatManager Component:**
1. **Select CombatManager GameObject**
2. **In CombatManager Component, link:**
   - **Player:** Drag `Player` from CombatArea
   - **Enemy:** Drag `Enemy` from CombatArea
   - **Deck Manager:** Drag `CombatManager` (itself)

**DeckManager Component:**
1. **Select CombatManager GameObject**
2. **In DeckManager Component:**
   - **Pool Size:** 50
   - **Initial Card Count:** 12
   - **Starting Cards:** Create Card ScriptableObjects and assign them here

### Step 11: Configure Player and Enemy

**Player Component:**
1. **Select Player GameObject**
2. **Set values:**
   - **Max Health:** 100
   - **Current Health:** 100 (auto-set)
   - **Max Energy:** 3
   - **Current Energy:** 3 (auto-set)

**Enemy Component:**
1. **Select Enemy GameObject**
2. **Set values:**
   - **Enemy Name:** "Enemy"
   - **Max Health:** 100
   - **Current Health:** 100 (auto-set)
   - **Base Attack Damage:** 10
   - **Attack Multiplier:** 1

### Step 12: Create Sample Cards

1. **Go to menu:** `MaskMYDrama > Create Sample Cards`
2. **This creates 8 sample cards in `Assets/Data/Cards/`**

### Step 13: Assign Starting Cards

1. **Select CombatManager GameObject**
2. **In DeckManager Component:**
   - **Starting Cards:** Set size to 5-8
   - **Drag cards from `Assets/Data/Cards/`** to the array slots
   - Recommended: Mix of Attack_Basic, Defence_Basic, and Strength_Basic

### Step 14: Test the Scene

1. **Press Play**
2. **Cards should appear in hand**
3. **Click cards to play** (if energy allows)
4. **Click "End Turn" button**
5. **Enemy should attack automatically**
6. **Repeat until win/lose**

---

## UI Hierarchy Structure

```
Canvas
├── CombatUI (CombatUI component)
├── TopBar
│   ├── PlayerInfo
│   │   ├── PlayerNameText
│   │   └── LevelText
│   └── PlayerHealthBar (HealthBar component)
│       ├── HealthSlider
│       └── HealthText
├── CombatArea
│   ├── PlayerArea
│   │   └── Player (Player component)
│   └── EnemyArea
│       ├── Enemy (Enemy component)
│       └── EnemyHealthBar (HealthBar component)
│           ├── HealthSlider
│           └── HealthText
├── CardHand (Horizontal Layout Group)
└── BottomBar
    ├── EnergyDisplay (EnergyDisplay component)
    │   └── EnergyText
    ├── DeckCounts (Vertical Layout Group)
    │   ├── PoolCount
    │   └── AbandonedCount
    └── EndTurnButton
        └── Text

Hierarchy (Non-UI):
├── GameManager (GameManager component)
└── CombatManager
    ├── CombatManager component
    └── DeckManager component
```

---

## Replacing Placeholder Art

### Player/Enemy Sprites
1. **Select Player or Enemy GameObject**
2. **Replace Image component** with your sprite
3. **Or add SpriteRenderer** if using 2D sprites

### Card Art
1. **Select CardUIPrefab**
2. **Replace Card Background Image** with card frame sprite
3. **Add card artwork** as child Image component

### UI Graphics
1. **Replace health bar fill** with custom sprite
2. **Replace button graphics** with custom sprites
3. **Add background images** to panels

### Mask Theme
1. **Apply mask-themed sprites** to all visual elements
2. **Update colors** to match theme
3. **Add mask-themed backgrounds**

---

## Troubleshooting

### Cards Not Appearing
- Check Card Hand Parent is assigned in CombatUI
- Check Card UI Prefab is assigned
- Check Starting Cards are assigned in DeckManager

### UI Not Updating
- Check all references are linked in CombatUI component
- Check CombatManager has Player/Enemy/DeckManager assigned
- Check EventSystem exists in scene

### Buttons Not Working
- Check EventSystem exists
- Check Button component is properly configured
- Check Canvas has GraphicRaycaster component

### Health Bars Not Updating
- Check HealthBar components have Slider and Text assigned
- Check Player/Enemy have CombatEntity components
- Check CombatUI has health bars assigned

---

## Next Steps

After UI is set up:
1. ✅ Create Card ScriptableObjects
2. ✅ Assign starting cards
3. ✅ Test combat flow
4. ✅ Replace placeholder art with final assets
5. ✅ Add animations
6. ✅ Add sound effects
7. ✅ Implement level progression

---

**Note:** All placeholder colors and sizes can be adjusted. The automated script creates functional UI that you can customize and replace with artist assets later.

