# MaskMYDrama MVP Implementation Summary

## Overview
MVP implementation based on the CSV gameplay design document. Core systems are implemented and ready for Unity scene setup.

## Implemented Systems

### ✅ Core Data Structures
- **Card.cs**: ScriptableObject for card data (Attack, Defence, Strength, Function types)
- **CardInstance.cs**: Runtime card instance
- **CardType.cs**: Enums for card types and rarities
- **CombatEntity.cs**: Base class for combat entities
- **Player.cs**: Player with energy management (3/4/5 per turn)
- **Enemy.cs**: Enemy with attack patterns

### ✅ Card System (Based on CSV)
- **DeckManager.cs**: 
  - Card Pool: 50 slots (5x10), filled with 12-15 cards in demo
  - Hand: 5 cards per round (designed + randomly)
  - Abandoned Pile: Used cards + unused cards from hand
  - Auto-reshuffle when pool runs out
- **CardDatabase.cs**: Manages available cards for roguelike selection

### ✅ Combat System
- **CombatManager.cs**:
  - Turn-based: Player first, then enemy
  - Energy system: Player consumes energy to play cards
  - Win condition: All enemies' life = 0
  - Lose condition: Player's life = 0
  - Auto-end turn when no playable cards
- **Attack/Defence System**:
  - Attack cards deal damage
  - Defence cards add defence points
  - Damage calculation: enemy attack - player defence
  - Defence resets each turn

### ✅ UI System
- **CombatUI.cs**: Main combat interface manager
- **CardUI.cs**: Individual card display with hover/click
- **HealthBar.cs**: Health display (current/max)
- **EnergyDisplay.cs**: Energy counter (current/max)
- **CardSelectionUI.cs**: Roguelike selection (pick 1 from 3)
- **CardOptionUI.cs**: Card option in selection screen

### ✅ Game Management
- **GameManager.cs**: Game state management (MainMenu, InLevel, Paused, Victory, GameOver)
- **CardDataCreator.cs**: Editor utility to create sample cards

## CSV Requirements Implementation Status

| Requirement | Status | Implementation |
|------------|--------|----------------|
| Card Pool (5x10 = 50 slots) | ✅ | DeckManager with poolSize = 50 |
| 12-15 cards in demo | ✅ | initialCardCount = 12 |
| 5 cards in hand per round | ✅ | DrawHand() draws 5 cards |
| Energy 3/4/5 per round | ✅ | Player.maxEnergy configurable |
| Card types (Attack/Defence/Strength/Function) | ✅ | CardType enum + Card ScriptableObject |
| Card rarities (Basic/Junior/Senior) | ✅ | CardRarity enum |
| Card loop (Pool → Hand → Abandoned → Pool) | ✅ | Full cycle implemented |
| Turn-based (Player → Enemy) | ✅ | CombatManager with state machine |
| Life bars | ✅ | HealthBar component |
| Energy balls display | ✅ | EnergyDisplay component |
| Attack/Defence calculation | ✅ | CombatEntity.TakeDamage() |
| Win/Lose conditions | ✅ | CombatState.Victory/Defeat |
| Roguelike card selection | ✅ | CardSelectionUI (pick 1 from 3) |

## Setup Instructions

### 1. Create Sample Cards
```
Unity Menu: MaskMYDrama > Create Sample Cards
```
This creates sample cards in `Assets/Data/Cards/`:
- Attack_Basic (0 energy, 6 damage)
- Attack_Junior (1 energy, 10 damage)
- Attack_Senior (2 energy, 18 damage)
- Defence_Basic (0 energy, 5 block)
- Defence_Junior (1 energy, 8 block)
- Defence_Senior (2 energy, 12 block)
- Strength_Basic (1 energy, +2 attack)
- Function_Draw (1 energy, draw card)

### 2. Create Card Database
1. Right-click: `Create > MaskMYDrama > Card Database`
2. Name: "CardDatabase"
3. Assign all cards to `All Cards` array

### 3. Scene Setup
1. Create empty GameObject "GameManager"
   - Add `GameManager` component
2. Create empty GameObject "CombatManager"
   - Add `CombatManager` component
   - Add `DeckManager` component
   - Assign starting cards array
3. Create Player GameObject
   - Add `Player` component
   - Set maxHealth, maxEnergy
4. Create Enemy GameObject
   - Add `Enemy` component
   - Set maxHealth, baseAttackDamage
5. Link references in CombatManager

### 4. UI Setup
1. Create Canvas
2. Create CombatUI GameObject
   - Add `CombatUI` component
   - Assign all manager references
3. Create UI elements:
   - Player Health Bar (Slider + TextMeshPro)
   - Enemy Health Bar (Slider + TextMeshPro)
   - Energy Display (TextMeshPro)
   - Card Hand Parent (Horizontal Layout Group)
   - End Turn Button
   - Pool/Abandoned count texts
4. Create Card UI Prefab:
   - UI Image (card background)
   - TextMeshPro (name, description, energy cost)
   - Add `CardUI` component
   - Assign to CombatUI's cardUIPrefab

### 5. Test
1. Play scene
2. Cards should appear in hand
3. Click cards to play (if energy allows)
4. Click "End Turn" to proceed
5. Enemy attacks automatically
6. Repeat until win/lose

## Next Steps (Not in MVP)

### Level System
- [ ] Level 1 (Tutorial)
- [ ] Level 2
- [ ] Level 3
- [ ] Level progression logic

### Game States
- [ ] Main menu scene
- [ ] Game start page (New Game, Load Game, Exit)
- [ ] Pause menu (Resume, Restart, Back to Menu)
- [ ] Victory/Defeat screens

### Roguelike Features
- [ ] Card selection between levels
- [ ] Card selection UI integration
- [ ] Level completion rewards

### Polish
- [ ] Mask theme visuals
- [ ] Animations
- [ ] Sound effects
- [ ] Particle effects

## File Structure
```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── CardType.cs
│   │   ├── CombatEntity.cs
│   │   ├── CombatManager.cs
│   │   ├── DeckManager.cs
│   │   ├── GameManager.cs
│   │   ├── CardDatabase.cs
│   │   └── CardDataCreator.cs (Editor)
│   ├── Cards/
│   │   ├── Card.cs
│   │   └── CardInstance.cs
│   ├── Combat/
│   │   ├── Player.cs
│   │   └── Enemy.cs
│   └── UI/
│       ├── CombatUI.cs
│       ├── CardUI.cs
│       ├── CardSelectionUI.cs
│       ├── CardOptionUI.cs
│       ├── HealthBar.cs
│       └── EnergyDisplay.cs
├── Data/
│   └── Cards/ (created by CardDataCreator)
└── MaskMYDrama_Design.md
```

## Notes
- All systems follow Unity best practices (ScriptableObjects, Events, Component-based)
- Code is modular and extensible
- Ready for scene setup and testing
- CSV requirements fully implemented in core systems

