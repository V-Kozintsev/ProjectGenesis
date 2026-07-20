# Sprint 015 - Skills Foundation

Status: completed and accepted in Play Mode on 2026-07-20.

## Sprint Goal

Create a minimal reusable active-skill foundation and prove it with one warrior skill.

## Task 1 - Add Skill Data

Acceptance criteria:

- the project contains a reusable `SkillDefinition` ScriptableObject type;
- skill data includes a stable id, display name, future class requirement, target type, damage, range, and cooldown;
- `SO_Skill_HeavyStrike` exists under `Assets/ProjectGenesis/Data/Skills`;
- Heavy Strike is authored as a warrior enemy-target skill with 22 damage, 1.55 range, and a 4 second cooldown.

## Task 2 - Apply One Active Warrior Skill

Acceptance criteria:

- the player has a reusable `PlayerSkillController`;
- pressing the Heavy Strike hotbar button uses the currently selected hostile target;
- if the selected enemy is outside skill range, the player automatically approaches with the current NavMesh setup;
- the skill temporarily stops the current autoattack action while approaching and applying;
- after a successful skill hit, the player resumes basic autoattacks against the same living target;
- left-clicking the ground, clicking another action target, or using WASD cancels the pending skill action;
- basic autoattack selection and repeated-click attack behavior continue to work.

## Task 3 - Add Minimal Skill Feedback

Acceptance criteria:

- the starter UI contains a small temporary hotbar with one Heavy Strike button;
- the button is visibly unavailable during cooldown and shows the remaining seconds;
- the UI shows short feedback for success, missing target, cooldown, invalid data, dead player, or too-distant target;
- the UI remains temporary blockout UI, with no final art, animation, VFX, sound, or icon work.

## Task 4 - Keep The Feature Verifiable

Acceptance criteria:

- the starter-village builder creates the Heavy Strike asset, player skill controller, quick slot, hotbar, and scene wiring;
- an editor validator checks skill data, player prefab wiring, hotbar presence, and preserved enemy spawner count;
- the project compiles and the Sprint 015 validator passes in Unity batch mode;
- existing movement, camera, NPC dialogue, target selection, autoattack, recovery, death, experience, loot, quest, and save flows are not intentionally changed.

## Initial Prototype Tuning

- Heavy Strike target: selected enemy;
- Heavy Strike damage: 22 direct damage;
- Heavy Strike range: 1.55;
- Heavy Strike cooldown: 4 seconds;
- quick slot count: 1.

Sprint 018 replaces the original fixed 22 direct damage with a 1.7 attack-power multiplier. The unarmed level-1 result against the current wolf remains 22, while equipment and levels can now strengthen the skill.

## Not In This Sprint

- cave, new zones, or decorative world rebuilds;
- new NPCs, quests, enemies, or items;
- full skill tree, multiple class kits, mana, rage, stamina, or other resources;
- final models, animations, VFX, sound, icons, or UI art;
- character creation;
- server, multiplayer, or networked persistence.
