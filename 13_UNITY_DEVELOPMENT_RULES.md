# Unity Development Rules

## Scene Rules

Each scene should have a clear purpose.

Initial scenes:

- Bootstrap;
- CharacterCreation later;
- StarterVillage;
- CombatTest optional.

## Prefab Rules

Prefabs should be reusable and clearly named.

Naming examples:

- `PF_Player_Warrior`
- `PF_Enemy_Wolf`
- `PF_NPC_VillageElder`
- `PF_UI_InventoryPanel`

## Script Naming

Use clear C# names:

- `PlayerController`
- `Health`
- `CombatStats`
- `Inventory`
- `QuestLog`
- `EnemyBrain`
- `ItemDefinition`

## Asset Naming

Use consistent prefixes when helpful:

- `SO_` for ScriptableObjects;
- `PF_` for prefabs;
- `MAT_` for materials;
- `SFX_` for sound effects;
- `UI_` for UI assets.

## Inspector Rule

Inspector fields should be private with `[SerializeField]` unless they truly need to be public API.

## Dependency Rule

Prefer explicit references and constructor-like initialization methods over global lookups. Use singletons only when there is a strong reason.

