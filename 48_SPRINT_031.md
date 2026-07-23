# Sprint 031 - Map Foundation

## Goal

Add a readable prototype map layer for orientation in the starter village, north wilds, and Wolf Alpha clearing.

## Scope

- Add a compact mini-map pinned to the upper-right screen edge.
- Show the player marker on the mini-map and rotate it with the character direction.
- Show the current authored zone name on the mini-map.
- Add a larger map window toggled with `M`.
- Draw simple readable areas for the starter village, north wilds, Wolf Alpha clearing, and main roads.
- Keep the implementation as temporary UI, not final map art.
- Add deterministic validation for map wiring and coordinate conversion.

## Acceptance Criteria

- `Project Genesis > Sprint 031 > Apply Map Foundation To Existing Scene` adds the map to the existing scene.
- `Project Genesis > Sprint 031 > Validate Map Foundation` passes.
- `Project Genesis > Sprint 031 > Validate Relevant Regression Suite` passes.
- In Play Mode, a mini-map is visible in the upper-right corner.
- The mini-map is pinned to the right screen edge rather than floating near the center.
- The mini-map marker follows the player and rotates with the character direction.
- The mini-map zone label changes between village and north-wilds data as the player moves.
- Pressing `M` opens and closes a larger map window.
- The larger map window can be dragged by its title area like the other temporary windows.
- Typing in chat does not toggle the map with `M`.

## Out Of Scope

- Final illustrated map art.
- Fog of war, quest markers, pins, icons, pathfinding routes, zoom, drag-pan, or discovered-area persistence.
- Trading, currency, prices, vendor stock, and economy balance.
