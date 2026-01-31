# Encore (安可) System Implementation

## Goal

Implement the Encore (安可) feature that triggers special card selection when the player achieves a perfect combat (no damage taken and kills an enemy within X rounds). The Encore system provides three special actions instead of the normal roguelike card selection, and automatically draws a roguelike card when entering subsequent levels.

## Solution

### Overview

The Encore system consists of:
1. **Encore Tracking**: Monitors combat performance (rounds, damage taken, enemy killed)
2. **Encore Status Persistence**: Stores Encore status in PlayerData singleton across scenes
3. **Encore Card Actions**: Three ScriptableObject-based actions that replace normal card selection
4. **Encore Card Selection UI**: Modified CardSelectionUI to support Encore actions
5. **Automatic Roguelike Card Drawing**: Draws one roguelike card when entering subsequent levels

### Components

#### 1. EncoreCardAction ScriptableObject
- **Location**: `Assets/Scripts/Cards/EncoreCardAction.cs`
- **Purpose**: Defines the three Encore actions as ScriptableObjects
- **Actions**:
  1. `AddRandomNewCard`: Randomly add 1 new card to draw pile
  2. `ShuffleDrawPile`: Shuffle entire draw pile
  3. `CopyFromDiscardPile`: Add a copy of a random card from discard pile to draw pile
- **Usage**: Create three EncoreCardAction assets in Unity and assign them to CombatManager's `encoreCardActions` array

#### 2. PlayerData Singleton Enhancement
- **Location**: `Assets/Scripts/Singletons/Player/PlayerData.cs`
- **Changes**:
  - Added `isEncoreActive` boolean to track Encore status
  - Added `encoreRoundLimit` integer (default: 3 rounds) to configure Encore requirement
- **Persistence**: Status persists across scenes using the singleton pattern

#### 3. CombatManager Enhancements
- **Location**: `Assets/Scripts/Core/CombatManager.cs`
- **New Features**:
  - **Encore Tracking Variables**:
    - `combatRoundCount`: Tracks number of rounds elapsed
    - `playerInitialHealth`: Stores player health at combat start
    - `totalDamageTaken`: Tracks total damage taken during combat
    - `enemyKilled`: Tracks if enemy was killed
  - **Encore Checking**: `CheckEncoreCondition()` method validates Encore requirements
  - **Encore Card Selection**: Modified `Go_Next_Level()` to show Encore selection when active
  - **Encore Action Execution**: `ExecuteEncoreAction()` handles the three action types
  - **Roguelike Card Drawing**: `DrawRoguelikeCardOnLevelStart()` draws one card when entering levels > 1

#### 4. CardSelectionUI Enhancements
- **Location**: `Assets/Scripts/UI/CardSelectionUI.cs`
- **New Features**:
  - `ShowEncoreCardSelection()`: Displays Encore actions instead of regular cards
  - `OnEncoreCardActionSelected` event: Fires when player selects an Encore action
  - Modified `OnConfirmSelection()` to handle both regular and Encore selection modes

#### 5. DeckManager Enhancements
- **Location**: `Assets/Scripts/Core/DeckManager.cs`
- **New Methods**:
  - `GetRandomCardFromDiscardPile()`: Returns a random card from discard pile (for Encore action)
  - `GetDiscardPile()`: Returns the discard pile for external access

## Best Practices

### 1. Singleton Pattern
- Used existing `PlayerData` singleton to persist Encore status across scenes
- Follows the existing singleton pattern in the codebase

### 2. ScriptableObject Pattern
- Encore actions are ScriptableObjects, allowing easy configuration in Unity Inspector
- Supports localization (Chinese/English) with separate fields

### 3. Event-Driven Architecture
- Uses C# events (`System.Action`) for decoupled communication
- CardSelectionUI fires events that CombatManager subscribes to

### 4. Separation of Concerns
- CombatManager handles combat logic and Encore checking
- CardSelectionUI handles UI display and user interaction
- DeckManager handles card pool management
- Each component has a single responsibility

### 5. Error Handling
- Null checks for all references
- Validation of Encore action arrays (must have exactly 3 actions)
- Fallback behavior when components are missing

### 6. Code Documentation
- XML documentation comments for all public methods
- Inline comments explaining complex logic
- Clear variable names that describe their purpose

## Test Plan

### Unit Tests

1. **Encore Condition Checking**
   - Test: No damage taken + enemy killed within round limit → Encore active
   - Test: Damage taken → Encore not active
   - Test: Enemy killed after round limit → Encore not active
   - Test: Enemy not killed → Encore not active

2. **Encore Action Execution**
   - Test: `AddRandomNewCard` adds one card to pool
   - Test: `ShuffleDrawPile` shuffles the draw pile
   - Test: `CopyFromDiscardPile` copies a card from discard to pool
   - Test: Empty discard pile → Copy action handles gracefully

3. **Roguelike Card Drawing**
   - Test: Level 1 → No card drawn
   - Test: Level 2+ → One roguelike card drawn
   - Test: Empty roguelike pool → Handles gracefully

### Integration Tests

1. **Combat Flow**
   - Complete combat with no damage → Encore triggers
   - Complete combat with damage → Normal card selection
   - Encore selection → Action executes → Next level loads

2. **Scene Persistence**
   - Achieve Encore in level 1 → Status persists to level 2
   - Use Encore in level 2 → Status resets
   - Status persists across scene loads

3. **UI Flow**
   - Encore active → Shows Encore card selection UI
   - Encore not active → Shows normal card selection UI
   - Selection → UI hides → Level proceeds

### Manual Testing Checklist

- [ ] Start new game, achieve Encore (no damage, kill enemy in 3 rounds)
- [ ] Verify Encore card selection appears instead of normal selection
- [ ] Select each of the three Encore actions and verify they work correctly
- [ ] Verify Encore status resets after use
- [ ] Enter level 2+ and verify one roguelike card is automatically drawn
- [ ] Test with damage taken → Normal card selection appears
- [ ] Test killing enemy after round limit → Normal card selection appears
- [ ] Test final level (level 4) → Encore should not trigger (normal selection)

## Points to Be Aware

### 1. EncoreCardAction Setup
- **Critical**: Must create three `EncoreCardAction` ScriptableObjects in Unity
- **Location**: Create via menu `MaskMYDrama/Encore Card Action`
- **Assignment**: Assign all three to `CombatManager.encoreCardActions` array in Inspector
- **Configuration**: Set action names and descriptions (both Chinese and English)

### 2. Round Counting
- Rounds are counted at the start of each player turn
- Round 1 = First player turn
- Encore must be achieved within `PlayerData.Instance.encoreRoundLimit` rounds (default: 3)

### 3. Damage Tracking
- Damage is tracked by comparing initial health to current health
- Defence points reduce damage but don't prevent Encore (if health doesn't decrease)
- Health restoration during combat may affect Encore checking

### 4. Final Level Behavior
- Level 4 is the final level (shows end game message)
- Encore does NOT trigger on final level (normal card selection instead)
- This prevents Encore from triggering when there's no next level

### 5. CardSelectionUI Limitations
- Current implementation reuses `CardOptionUI` for Encore actions
- **Future Enhancement**: Consider creating `EncoreActionOptionUI` component for better display
- Encore actions are displayed as simple buttons with text (may need visual polish)

### 6. Roguelike Card Drawing
- Only draws one card per level (not per combat)
- Only draws in levels > 1 (first level uses starting deck only)
- Requires `CardDatabase` to be assigned to `CombatManager`

### 7. Encore Status Reset
- Encore status is reset after being used (in `OnRoguelikeCardSelected` and `OnEncoreCardActionSelected`)
- Status persists across scenes until used
- Status is NOT reset when starting a new combat (only when used)

### 8. Localization
- EncoreCardAction supports both Chinese and English text
- Current implementation uses English by default
- Can be extended to use `GameSettingDataSingleton.Instance.localization_index` for proper localization

### 9. Error Handling
- If `CardDatabase` is not assigned, Encore actions that require it will fail gracefully
- If discard pile is empty, `CopyFromDiscardPile` action does nothing
- If roguelike pool is empty, `AddRandomNewCard` action does nothing

### 10. Performance Considerations
- Encore checking happens once per combat (when enemy is killed)
- No performance impact during normal gameplay
- Card drawing on level start is a one-time operation

## Changes Summary

### New Files
1. `Assets/Scripts/Cards/EncoreCardAction.cs` - Encore action ScriptableObject

### Modified Files
1. `Assets/Scripts/Singletons/Player/PlayerData.cs` - Added Encore status tracking
2. `Assets/Scripts/Core/CombatManager.cs` - Added Encore tracking, checking, and execution
3. `Assets/Scripts/UI/CardSelectionUI.cs` - Added Encore card selection support
4. `Assets/Scripts/Core/DeckManager.cs` - Added discard pile access methods

### Unity Setup Required
1. Create three `EncoreCardAction` ScriptableObjects:
   - Action 1: "随机获得1张新卡牌到卡池" / "Randomly add 1 new card to your draw pile" (AddRandomNewCard)
   - Action 2: "重洗卡池所有牌" / "Shuffle your entire draw pile" (ShuffleDrawPile)
   - Action 3: "复制一张弃牌里的卡牌堆到卡池" / "Add a copy of a random card from your discard pile to your draw pile" (CopyFromDiscardPile)
2. Assign the three actions to `CombatManager.encoreCardActions` array in Inspector
3. Ensure `CardDatabase` is assigned to `CombatManager` for roguelike card drawing

## Review Notes

### Code Quality
- ✅ Follows existing codebase patterns and conventions
- ✅ Uses singleton pattern for persistence (consistent with existing code)
- ✅ ScriptableObject pattern for data configuration (consistent with Card system)
- ✅ Event-driven architecture for decoupling
- ✅ Comprehensive XML documentation

### Potential Improvements
- Consider creating `EncoreActionOptionUI` component for better visual display
- Add visual feedback when Encore is achieved (particle effects, sound, etc.)
- Add localization support using existing localization system
- Add analytics tracking for Encore achievement rate
- Consider making `encoreRoundLimit` configurable per difficulty level

### Testing Recommendations
- Test with various combat scenarios (quick kills, long combats, etc.)
- Test edge cases (empty pools, missing references, etc.)
- Test across multiple levels to verify persistence
- Test on different platforms if applicable

## Conclusion

The Encore system has been successfully implemented following Unity C# best practices. The system is modular, well-documented, and integrates seamlessly with the existing codebase. The implementation provides a rewarding gameplay mechanic that encourages skillful play while maintaining code quality and maintainability.

