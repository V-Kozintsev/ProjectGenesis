# Sprint 030 - Elite Quest And Merchant Placeholder

## Goal

Connect the first elite encounter to a small quest and add a village merchant placeholder without starting the economy system.

## Scope

- Add a reusable `QuestDefinition` for defeating the Wolf Alpha.
- Make the Guard Captain offer the elite quest only after `Кабанья угроза` is completed.
- Keep the elite objective tied to the Wolf Alpha target id, not to ordinary wolves.
- Add a selectable and talkable `Деревенский торговец` NPC in the starter village.
- Show a clear placeholder dialogue for NPCs that do not currently offer a quest.
- Add deterministic validation for quest data, prerequisite flow, objective matching, merchant scene wiring, and relevant regressions.

## Acceptance Criteria

- `Project Genesis > Sprint 030 > Apply Elite Quest And Merchant To Existing Scene` adds the quest data and merchant to the existing scene.
- `Project Genesis > Sprint 030 > Validate Elite Quest And Merchant` passes.
- `Project Genesis > Sprint 030 > Validate Relevant Regression Suite` passes.
- In Play Mode, the Guard Captain still offers `Кабанья угроза` first.
- After turning in `Кабанья угроза`, the Guard Captain offers `Вожак стаи`.
- Defeating an ordinary wolf does not progress `Вожак стаи`.
- Defeating the level-3 `Вожак стаи` completes the elite objective and lets the player return to the Guard Captain.
- The merchant can be selected and talked to, but does not open a shop yet.

## Out Of Scope

- Real buy/sell economy, currency, prices, repair, vendor stock, or item sinks.
- New items, enemies, zones, shops, UI art, animations, VFX, or sound.
- Networked chat, server economy, auction house, player trading, or global marketplace.
