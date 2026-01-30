# MaskMYDrama - Game Design Document

## Goal

Create a Unity prototype of a deck-building roguelike game inspired by **Slay the Spire** (杀戮尖塔) with a **Mask** theme. The game will feature:

- Turn-based card combat system
- Deck building and card collection mechanics
- Energy-based action system
- Roguelike progression with multiple floors/acts
- Mask-themed narrative and visual design
- Character progression with relics and upgrades

## Solution Overview

### Core Gameplay Loop

1. **Combat Phase**: Player faces enemies in turn-based combat using cards
2. **Map Navigation**: Player chooses paths through a branching map
3. **Card Acquisition**: Player adds new cards to their deck
4. **Upgrade/Shop**: Player upgrades cards or purchases items
5. **Boss Encounter**: Player faces challenging boss at end of act
6. **Repeat**: Progress through multiple acts with increasing difficulty

### Key Systems

#### 1. Card System
- **Card Types**:
  - **Attack Cards**: Deal damage to enemies
  - **Skill Cards**: Provide block, buffs, or utility effects
  - **Power Cards**: Provide persistent effects for the combat
- **Card Properties**:
  - Energy Cost
  - Damage/Block values
  - Special effects (status effects, card draw, etc.)
  - Upgradeable versions (+ versions)

#### 2. Combat System
- **Turn Structure**:
  - Player Turn: Draw cards, play cards using energy
  - Enemy Turn: Enemies execute intents (attacks, buffs, debuffs)
- **Energy System**: Player has limited energy per turn (typically 3-4)
- **Block System**: Temporary damage mitigation
- **Status Effects**: Buffs (Strength, Dexterity, Focus) and Debuffs (Vulnerable, Weak, Frail)

#### 3. Enemy System
- **Enemy Types**: Various enemies with unique attack patterns
- **Intent System**: Enemies show what they will do next turn
- **Health Bars**: Visual representation of enemy health
- **Boss Encounters**: Special challenging enemies at act ends

#### 4. Deck Management
- **Deck Building**: Start with basic cards, add/remove cards throughout run
- **Draw Pile**: Cards to be drawn
- **Discard Pile**: Cards used this turn
- **Exhaust Pile**: Cards permanently removed from combat
- **Hand Management**: Limited hand size (typically 5-10 cards)

#### 5. Progression System
- **Relics**: Permanent passive bonuses (similar to Slay the Spire)
- **Gold**: Currency for shops
- **Floor/Act Progression**: Track player progress
- **Ascension Levels**: Difficulty modifiers for replayability

#### 6. Mask Theme Integration
- **Mask Mechanics**: Masks could provide unique abilities or character classes
- **Visual Design**: Mask-themed UI, characters, and enemies
- **Narrative**: Story elements centered around masks and identity

### UI Layout (Based on Reference Images)

**Top Bar:**
- Player name and character icon
- Health display (current/max)
- Gold counter
- Relic icons (with tooltips)
- Floor/Act number
- Map and Settings buttons

**Combat Area:**
- Player character (left side)
- Enemy character(s) (right side)
- Health bars for all combatants
- Status effect icons
- Enemy intent indicators

**Bottom Bar:**
- Energy orb (current/max)
- Draw pile count
- Discard pile count
- Exhaust pile count (if applicable)
- End Turn button

**Card Hand:**
- Cards displayed horizontally at bottom
- Card tooltips on hover
- Card highlighting for playable cards
- Energy cost indicators

## Best Practices

### Unity Architecture

1. **ScriptableObject-Based Design**
   - Cards as ScriptableObjects for easy data management
   - Enemies as ScriptableObjects
   - Relics as ScriptableObjects
   - Card effects as modular components

2. **Event-Driven Architecture**
   - Use Unity Events or C# events for combat actions
   - Decouple UI from game logic
   - Enable easy testing and debugging

3. **State Management**
   - Combat state machine (PlayerTurn, EnemyTurn, Victory, Defeat)
   - Game state manager for overall game flow
   - Scene management for different game phases

4. **Component-Based Design**
   - Modular card effects as components
   - Reusable UI components
   - Status effect system as components

5. **Data-Driven Design**
   - Externalize game data (cards, enemies, relics) to ScriptableObjects
   - Easy to balance and modify without code changes
   - Support for modding potential

### Code Organization

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── CombatManager.cs
│   │   └── DeckManager.cs
│   ├── Cards/
│   │   ├── Card.cs (ScriptableObject)
│   │   ├── CardData.cs
│   │   └── CardEffect.cs
│   ├── Combat/
│   │   ├── CombatEntity.cs (base class)
│   │   ├── Player.cs
│   │   ├── Enemy.cs
│   │   └── StatusEffect.cs
│   ├── UI/
│   │   ├── CardUI.cs
│   │   ├── HealthBar.cs
│   │   ├── EnergyDisplay.cs
│   │   └── CombatUI.cs
│   └── Data/
│       ├── Relic.cs (ScriptableObject)
│       └── EnemyData.cs (ScriptableObject)
├── Data/
│   ├── Cards/
│   ├── Enemies/
│   └── Relics/
└── Prefabs/
    ├── CardPrefab.prefab
    ├── EnemyPrefab.prefab
    └── UI/
```

### Performance Considerations

1. **Object Pooling**: Pool card UI objects to avoid instantiation overhead
2. **UI Optimization**: Use Canvas Groups and efficient layout groups
3. **Memory Management**: Proper cleanup of event subscriptions
4. **Asset Management**: Use Addressables for large assets if needed

### Testing Strategy

1. **Unit Tests**: Test card effects, combat calculations, deck management
2. **Integration Tests**: Test combat flow, state transitions
3. **Manual Testing**: Playtest combat scenarios, edge cases

## Test Plan

### Phase 1: Core Combat System
- [ ] Card data structure and ScriptableObject setup
- [ ] Basic combat turn flow (Player -> Enemy)
- [ ] Energy system implementation
- [ ] Health and damage calculations
- [ ] Block system

### Phase 2: Card System
- [ ] Card drawing and hand management
- [ ] Card playing and energy consumption
- [ ] Discard pile mechanics
- [ ] Card effects (damage, block, status effects)
- [ ] Card upgrades

### Phase 3: Enemy System
- [ ] Enemy data structure
- [ ] Enemy intent system
- [ ] Enemy turn execution
- [ ] Multiple enemy encounters
- [ ] Boss encounters

### Phase 4: UI Implementation
- [ ] Card UI with hover effects
- [ ] Health bars for player and enemies
- [ ] Energy display
- [ ] Status effect icons
- [ ] Enemy intent display
- [ ] Top bar (health, gold, relics, floor)

### Phase 5: Deck Management
- [ ] Starting deck
- [ ] Card acquisition
- [ ] Deck building mechanics
- [ ] Card removal

### Phase 6: Progression System
- [ ] Relic system
- [ ] Gold and shop system
- [ ] Floor/Act progression
- [ ] Map navigation

### Phase 7: Mask Theme Integration
- [ ] Mask-themed visuals
- [ ] Mask-based character classes or abilities
- [ ] Thematic narrative elements

## Points to Be Aware Of

### Technical Challenges

1. **Card Effect System Complexity**
   - Cards can have complex interactions
   - Need flexible system for combining effects
   - Consider using a command pattern or effect chain system

2. **UI Responsiveness**
   - Card animations should feel responsive
   - Smooth transitions between states
   - Clear visual feedback for all actions

3. **State Management**
   - Combat state can be complex with many edge cases
   - Need to handle interruptions (e.g., death during card play)
   - Save/load system for runs

4. **Balance and Tuning**
   - Card costs and effects need careful balancing
   - Enemy difficulty scaling
   - Progression curve

### Design Considerations

1. **Slay the Spire Inspiration vs. Originality**
   - Use core mechanics but add unique Mask theme elements
   - Avoid direct copying of card designs
   - Create unique identity while maintaining familiar feel

2. **Accessibility**
   - Clear tooltips and descriptions
   - Visual clarity for all game elements
   - Easy to understand combat flow

3. **Scalability**
   - Design systems to easily add new cards, enemies, relics
   - Modular architecture for future expansion
   - Consider modding support

4. **Performance on Lower-End Devices**
   - Optimize UI rendering
   - Efficient card effect calculations
   - Consider mobile platform requirements if applicable

### Development Workflow

1. **Iterative Development**
   - Start with minimal viable prototype
   - Add features incrementally
   - Test frequently

2. **Data-Driven Approach**
   - Use ScriptableObjects for easy iteration
   - Separate data from logic
   - Enable designers to modify without code

3. **Version Control**
   - Proper .gitignore for Unity
   - Use Git LFS for large assets
   - Regular commits with clear messages

## Implementation Priority

### MVP (Minimum Viable Prototype)
1. Basic combat system (player vs. single enemy)
2. Simple card system (attack, defend, block)
3. Energy system
4. Basic UI (health bars, cards, energy)
5. Turn flow

### Phase 2
1. Multiple card types and effects
2. Status effects system
3. Multiple enemies
4. Deck building basics
5. Enemy intents

### Phase 3
1. Relic system
2. Card upgrades
3. Map navigation
4. Shop system
5. Boss encounters

### Phase 4
1. Mask theme integration
2. Advanced card effects
3. Multiple acts/floors
4. Polish and balance

## Review Checklist

Before implementing code, review:

- [ ] Architecture aligns with best practices
- [ ] Data structures are well-defined
- [ ] UI layout matches reference images
- [ ] Systems are modular and extensible
- [ ] Performance considerations addressed
- [ ] Test plan is comprehensive
- [ ] Mask theme is properly integrated

---

**Next Steps**: Review this document, then proceed with implementation starting with the core combat system and card framework.

