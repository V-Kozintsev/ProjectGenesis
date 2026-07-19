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

## Visual Production Rule

Blockout geometry exists to test scale, navigation, combat space, and camera behavior. Do not spend a gameplay sprint polishing blockout objects into final art.

Visual development follows these stages:

1. collect focused references for the world and interface;
2. approve a small style guide for shape, material, color, architecture, characters, and UI;
3. make an asset list and choose whether each asset is temporary, self-made, purchased, or commissioned;
4. build reusable modular environment kits and character pipelines;
5. add materials, lighting, animation, effects, sound, and UI presentation;
6. polish only after the related playable flow is stable.

Unity is the assembly, lighting, interaction, and runtime environment. External tools or licensed asset packs can be chosen later per asset; the project should not commit to a final art pipeline before the visual direction and production budget are clear.
