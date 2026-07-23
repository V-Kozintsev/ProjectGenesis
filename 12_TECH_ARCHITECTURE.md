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

`WorldZoneDefinition` owns reusable authored identity, type, and player feedback for a world zone. `WorldZoneVolume` maps definitions to prioritized world-space bounds without depending on trigger timing. `PlayerZoneController` resolves the current zone once per movement change and is the shared combat-permission authority used by player combat, skills, hostile AI, and resurrection refresh. `EnemyTerritory` remains responsible only for where enemies may roam.

`PlayerProgression` owns experience arithmetic in both directions, including death penalties and level-boundary changes. `PlayerDeathController` listens to player `Health.Died`, applies the death penalty once, stops gameplay actions, disables regeneration, shows `DeathRespawnView`, and performs the confirmed village resurrection. `PlayerCombatController` owns only target selection and basic attack state.

`CombatStats` owns attack and defense composition plus damage formulas. Base, class, progression, and equipment attack contributions remain separately readable; base defense and body-armor defense produce one total defense. `PlayerProgression` applies authored class and level growth to `Health` and `CombatStats`; `PlayerEquipment` owns the exact equipped weapon and body-armor instances and supplies their contributions. `SkillDefinition` stores an attack-power multiplier and player-facing description, and `PlayerSkillController` asks `CombatStats` for the final defense-aware skill damage. `CharacterStatsView`, `CharacterEquipmentView`, and `SkillTooltipView` only present or request operations from these runtime systems. `DraggableWindow` is a reusable UI adapter attached to window headers and keeps moved prototype windows reachable on screen.

`ItemDefinition` remains immutable authored content and separates weapon attack, armor defense, and consumable healing values. Each collected `ItemInstance` carries a stable unique id and references its definition. `PlayerInventory` owns only bag instances in eight fixed positions, `PlayerEquipment` owns exact main-hand and body instances outside the bag, and `PlayerItemUseController` coordinates health validation with exact-instance removal. `PlayerItemDropController` transfers a selected bag instance into a `WorldLootPickup` or permanently removes it after UI confirmation; a dropped pickup retains the same instance id when collected again. Equipping atomically exchanges a selected bag item with the previous compatible equipment item; unequipping requires bag space. `InventoryView` and `CharacterEquipmentView` share one movable window. Profile version 7 persists bag positions and equipped instances separately. Versions 1-3 migrate definition-id lists into unique instances; version 4 preserves instance ids; version 5 preserves fixed positions; version 6 transfers its referenced equipped instances out of their old bag positions.

Enemy identity, level, and base experience are authored by `EnemyBrain`. `PlayerProgression` owns the configurable level-difference multiplier because it converts an enemy reward into player progression; fixed quest rewards continue to bypass enemy scaling.

`QuestDefinition` is immutable authored content for quest identity, dialogue, an explicit objective type, target, count, and reward. `InteractableNpc` references that content, while `QuestLog` owns any number of simultaneous runtime quest states and copies persistent fields into `QuestProgressData` when each quest is accepted. Objective events update every matching active entry independently. `QuestTrackerView` summarizes active entries from the log instead of depending on one NPC, and existing saves do not need a profile migration.

Enemy prefabs compose the shared `EnemyBrain`, `Health`, `HealthRegeneration`, `CombatStats`, and `NavMeshAgent` behavior with independently authored values. Optional reward components such as `EnemyLootDrop` are attached only when that enemy actually participates in those loot or quest rules.

## Saving

Save only stable runtime state:

- character name, race id, and class id;
- player position;
- level and experience;
- equipped item instance ids by slot;
- inventory instance ids, definition ids, and slot positions;
- quest states;
- world flags later.

Do not serialize entire scene objects.

The current prototype saves automatically through `IPlayerPersistence`. `LocalJsonPlayerPersistence` is an offline development implementation, not the final ownership model. It writes a small versioned profile containing only stable ids and values, while `PlayerPersistenceController` captures and restores Unity runtime components.

Profile version 2 added character identity while continuing to load version-1 files. Profile version 3 adds an explicit first-character-created flag. Version-1 and version-2 profiles are migrated as existing characters, while a new version-3 profile remains in creation state until a valid name is submitted. Missing identity fields still fall back to the authored player-prefab defaults.

`CharacterEntryView` owns only the local prototype entry flow. It reads lifecycle state from `PlayerPersistenceController`, blocks gameplay controllers until selection, and does not own race, class, progression, inventory, or combat rules.

There is no player-facing save/load button. The intended online flow is automatic, server-authoritative persistence: the server stores character progress and a validated last position, then restores that state after the player logs in again. Replacing local JSON with a server adapter should not require gameplay systems to know where the profile is stored.

## Multiplayer Later

Do not build networking before the local gameplay loop is stable. Keep systems deterministic and data-oriented, use stable ids for persisted content, and keep persistence behind an interface so server authority can replace the local prototype implementation later.
