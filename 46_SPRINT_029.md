# Sprint 029 - Combat Readability Layer

## Goal

Make the existing combat prototype easier to read without starting a final UI-art pass or expanding world content.

## Scope

- Keep the selected-target HUD clean for ordinary enemies.
- Keep elite enemies identified through the existing rank label instead of permanent explanatory text.
- During an elite telegraphed attack, show a short countdown and a clear retreat hint.
- Keep existing click selection, autoattack, Heavy Strike, movement, loot, quests, death, respawn, and saves intact.
- Add deterministic editor validation for the HUD wiring and elite-warning data.

## Acceptance Criteria

- Selecting a regular wolf still shows only name, level, and health in the target panel.
- Selecting the Wolf Alpha shows `Элита` in the name without a permanent extra status line.
- While the Wolf Alpha prepares the special attack, the target panel shows a countdown/hint instead of only relying on body color.
- NPC selection still shows the target panel without an enemy health bar.
- The current scene and full scene rebuild both create the same temporary-warning wiring.
- `Project Genesis > Sprint 029 > Validate Combat Readability` passes.
- The Sprint 029 relevant regression suite passes without breaking older systems.

## Out of Scope

- Final UI art, icons, animations, sound, VFX, floating combat text, damage numbers above heads, or full boss frames.
- New enemies, quests, loot, zones, classes, resources, or skills.
- Multiplayer/global combat announcements.
