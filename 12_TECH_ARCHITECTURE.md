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

`CombatStats` owns attack composition and damage formulas. Base, class, progression, and equipment contributions remain separately readable but produce one total attack power. `PlayerProgression` applies authored class and level growth to `Health` and `CombatStats`; `PlayerEquipment` supplies only the equipped-weapon contribution. `SkillDefinition` stores an attack-power multiplier and player-facing description, and `PlayerSkillController` asks `CombatStats` for the final defense-aware skill damage. `CharacterStatsView` and `SkillTooltipView` only present these runtime values. `DraggableWindow` is a reusable UI adapter attached to window headers and keeps moved prototype windows reachable on screen.

Enemy identity, level, and base experience are authored by `EnemyBrain`. `PlayerProgression` owns the configurable level-difference multiplier because it converts an enemy reward into player progression; fixed quest rewards continue to bypass enemy scaling.

Enemy prefabs compose the shared `EnemyBrain`, `Health`, `HealthRegeneration`, `CombatStats`, and `NavMeshAgent` behavior with independently authored values. Optional reward components such as `EnemyLootDrop` are attached only when that enemy actually participates in those loot or quest rules.

## Saving

Save only stable runtime state:

- character name, race id, and class id;
- player position;
- level and experience;
- equipped items by id;
- inventory items by id and quantity;
- quest states;
- world flags later.

Do not serialize entire scene objects.

The current prototype saves automatically through `IPlayerPersistence`. `LocalJsonPlayerPersistence` is an offline development implementation, not the final ownership model. It writes a small versioned profile containing only stable ids and values, while `PlayerPersistenceController` captures and restores Unity runtime components.

Profile version 2 added character identity while continuing to load version-1 files. Profile version 3 adds an explicit first-character-created flag. Version-1 and version-2 profiles are migrated as existing characters, while a new version-3 profile remains in creation state until a valid name is submitted. Missing identity fields still fall back to the authored player-prefab defaults.

`CharacterEntryView` owns only the local prototype entry flow. It reads lifecycle state from `PlayerPersistenceController`, blocks gameplay controllers until selection, and does not own race, class, progression, inventory, or combat rules.

There is no player-facing save/load button. The intended online flow is automatic, server-authoritative persistence: the server stores character progress and a validated last position, then restores that state after the player logs in again. Replacing local JSON with a server adapter should not require gameplay systems to know where the profile is stored.

## Multiplayer Later

Do not build networking before the local gameplay loop is stable. Keep systems deterministic and data-oriented, use stable ids for persisted content, and keep persistence behind an interface so server authority can replace the local prototype implementation later.
