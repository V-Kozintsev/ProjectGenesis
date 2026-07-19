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
      LootTables/
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
- LootTableDefinition;
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
- Health and HealthRegeneration;

### Unity Adapter Layer

Contains MonoBehaviours that connect Unity objects to runtime systems.

Examples:

- PlayerController;
- EnemyController;
- InteractionTrigger;
- InventoryView;

`Health` owns health boundaries, damage, healing, and death events. `HealthRegeneration` is a reusable timing adapter that listens for damage and applies bounded healing ticks only while its owner allows recovery. Player and enemy controllers decide when combat state permits regeneration; they do not duplicate health arithmetic.

`EnemyTerritory` owns reusable world-space bounds for hostile AI. `EnemySpawner` assigns the scene territory to each spawned `EnemyBrain`; the brain uses it for detection, leash return, and validated NavMesh roaming without coupling player movement to zone rules.

`PlayerProgression` owns experience arithmetic in both directions, including death penalties and level-boundary changes. `PlayerCombatController` only decides when one penalty is applied, then continues the existing respawn flow.

Enemy identity, level, and base experience are authored by `EnemyBrain`. `PlayerProgression` owns the configurable level-difference multiplier because it converts an enemy reward into player progression; fixed quest rewards continue to bypass enemy scaling.

## Saving

Save only stable runtime state:

- player position;
- level and experience;
- equipped items by id;
- inventory items by id and quantity;
- quest states;
- world flags later.

Do not serialize entire scene objects.

The current prototype saves automatically through `IPlayerPersistence`. `LocalJsonPlayerPersistence` is an offline development implementation, not the final ownership model. It writes a small versioned profile containing only stable ids and values, while `PlayerPersistenceController` captures and restores Unity runtime components.

There is no player-facing save/load button. The intended online flow is automatic, server-authoritative persistence: the server stores character progress and a validated last position, then restores that state after the player logs in again. Replacing local JSON with a server adapter should not require gameplay systems to know where the profile is stored.

## Multiplayer Later

Do not build networking before the local gameplay loop is stable. Keep systems deterministic and data-oriented, use stable ids for persisted content, and keep persistence behind an interface so server authority can replace the local prototype implementation later.
