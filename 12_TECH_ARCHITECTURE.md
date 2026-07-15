# Technical Architecture

## Unity Target

Use a modern Unity LTS version when the actual Unity project is created.

Recommended:

- URP for visuals;
- Input System package;
- Cinemachine for camera;
- ScriptableObjects for authored data.

## Folder Structure

Recommended project folders:

```text
Assets/
  ProjectGenesis/
    Art/
    Audio/
    Data/
      Classes/
      Enemies/
      Items/
      Quests/
      Races/
      Skills/
    Materials/
    Prefabs/
      Characters/
      Enemies/
      UI/
      World/
    Scenes/
    Scripts/
      Core/
      Gameplay/
      UI/
      Data/
      Saving/
      Tools/
    Settings/
```

## Code Layers

### Data Layer

Contains ScriptableObjects and serializable definitions.

Examples:

- ItemDefinition;
- EnemyDefinition;
- QuestDefinition;
- ClassDefinition;
- RaceDefinition;
- SkillDefinition.

### Runtime Layer

Contains active runtime state.

Examples:

- PlayerState;
- Inventory;
- QuestLog;
- CombatStats;

### Unity Adapter Layer

Contains MonoBehaviours that connect Unity objects to runtime systems.

Examples:

- PlayerController;
- EnemyController;
- InteractionTrigger;
- InventoryView;

## Saving

Save only stable runtime state:

- player position;
- level and experience;
- equipped items by id;
- inventory items by id and quantity;
- quest states;
- world flags later.

Do not serialize entire scene objects.

## Multiplayer Later

Do not build the first prototype around multiplayer assumptions. Keep systems deterministic and data-oriented enough that multiplayer can be evaluated later.

