# MaskMYDrama - Scripts Documentation

## Project Structure

### Core Systems
- **CardType.cs**: Enums for card types and rarities
- **CombatEntity.cs**: Base class for player and enemy
- **CombatManager.cs**: Manages turn-based combat flow
- **DeckManager.cs**: Handles card pool (50 slots), hand (5 cards), and abandoned pile
- **GameManager.cs**: Overall game state management
- **CardDatabase.cs**: Manages available cards for roguelike selection

### Cards
- **Card.cs**: ScriptableObject for card data
- **CardInstance.cs**: Runtime instance of a card

### Combat
- **Player.cs**: Player entity with energy management
- **Enemy.cs**: Enemy entity with attack patterns

### UI
- **CombatUI.cs**: Main combat interface
- **CardUI.cs**: Individual card display in hand
- **CardSelectionUI.cs**: Roguelike card selection (pick 1 from 3)
- **CardOptionUI.cs**: Card option in selection screen
- **HealthBar.cs**: Health display component
- **EnergyDisplay.cs**: Energy counter display

## Setup Instructions

### 1. Create Sample Cards
1. In Unity Editor, go to menu: `MaskMYDrama > Create Sample Cards`
2. This will create sample card ScriptableObjects in `Assets/Data/Cards/`

### 2. Create Card Database
1. Right-click in Project window: `Create > MaskMYDrama > Card Database`
2. Name it "CardDatabase"
3. Assign all created cards to the `All Cards` array

### 3. Setup Scene
1. Create an empty GameObject named "GameManager"
2. Add `GameManager` component
3. Create empty GameObject "CombatManager"
4. Add `CombatManager`, `DeckManager` components
5. Create Player and Enemy GameObjects
6. Add `Player` and `Enemy` components respectively
7. Assign references in CombatManager

### 4. Setup UI
1. Create Canvas
2. Create CombatUI GameObject under Canvas
3. Add `CombatUI` component
4. Create UI elements:
   - Player Health Bar (with HealthBar component)
   - Enemy Health Bar (with HealthBar component)
   - Energy Display (with EnergyDisplay component)
   - Card Hand Parent (Horizontal Layout Group)
   - End Turn Button
   - Pool/Abandoned count texts

### 5. Create Card UI Prefab
1. Create UI Image for card background
2. Add TextMeshPro components for name, description, energy cost
3. Add `CardUI` component
4. Save as prefab
5. Assign to CombatUI's `cardUIPrefab` field

## Gameplay Flow (Based on CSV)

1. **Turn Start**: Player draws 5 cards from pool
2. **Player Turn**: 
   - Player can play cards if energy >= card cost
   - Cards are moved to abandoned pile after use
   - Unused cards go to abandoned at end of turn
3. **Enemy Turn**: Enemy attacks player
4. **Win Condition**: All enemies' life = 0
5. **Lose Condition**: Player's life = 0
6. **Card Pool**: 
   - 50 slots total (5x10)
   - 12-15 cards in demo
   - When pool runs out, reshuffle abandoned pile into pool

## Energy System
- Player starts with 3/4/5 energy per turn (configurable)
- Energy is consumed when playing cards
- Energy resets at start of each player turn

## Card Types (From CSV)
- **Attack**: Basic (0), Junior (1), Senior (2-3 energy)
- **Defence**: Basic (0), Junior (1), Senior (2-3 energy)
- **Strength**: Increases attack power
- **Function**: Special effects

## Next Steps
- Implement level progression (3 levels)
- Add roguelike card selection between levels
- Create game start page and menu
- Add pause/restart functionality
- Implement Mask theme visuals

