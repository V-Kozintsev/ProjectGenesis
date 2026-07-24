# Sprint 033 - NPC Interaction Hub

## Goal

Add a minimal reusable NPC action window so NPCs do not immediately open a quest or shop on repeated click.

## Scope

- Add a temporary NPC interaction window with NPC name, short greeting, and action buttons.
- Route quest NPCs through a `Задание` action that opens the existing quest dialogue.
- Route merchant NPCs through a `Торговля` action that opens the existing shop.
- Keep `Поговорить` as a placeholder for later lore, services, rumors, and achievements.
- Preserve one-click NPC selection, repeated-click interaction, and auto-approach.
- Add deterministic validation for scene wiring and relevant regressions.

## Acceptance Criteria

- `Project Genesis > Sprint 033 > Apply NPC Interaction Hub To Existing Scene` adds the hub to the current scene.
- `Project Genesis > Sprint 033 > Validate NPC Interaction Hub` passes.
- `Project Genesis > Sprint 033 > Validate Relevant Regression Suite` passes.
- In Play Mode, clicking an already selected Village Elder opens the NPC action window first.
- Pressing `Задание` opens the existing quest dialogue and keeps quest accept/turn-in behavior.
- Clicking an already selected Village Merchant opens the NPC action window first.
- Pressing `Торговля` opens the existing shop window.
- Walking away, death, target clearing, or selecting a different NPC closes the temporary hub.

## Out Of Scope

- Final dialogue UI art.
- Branching dialogue trees, voice, portraits, reputation, achievements, services, and lore database.
- Player-to-player trade, auction, final economy balance, or item tooltip polish.
