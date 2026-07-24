# Sprint 032 - Merchant Shop Foundation

## Goal

Add the first minimal buy/sell loop through the existing village merchant without starting full economy balancing.

## Scope

- Add persistent player gold.
- Add temporary buy and sell prices to item definitions.
- Give the Village Merchant a small data-driven starter stock.
- Add a simple movable merchant shop window.
- Allow buying affordable stocked items into the bag.
- Allow selling ordinary bag items for their sell price.
- Publish local system messages for successful shop actions.
- Add deterministic validation for wallet, prices, shop wiring, buy, sell, and relevant regressions.

## Acceptance Criteria

- `Project Genesis > Sprint 032 > Apply Merchant Shop To Existing Scene` adds the wallet, shop UI, item prices, and merchant stock to the current scene.
- `Project Genesis > Sprint 032 > Validate Merchant Shop Foundation` passes.
- `Project Genesis > Sprint 032 > Validate Relevant Regression Suite` passes.
- In Play Mode, the Village Merchant can be selected with one click.
- Re-clicking the selected Village Merchant nearby, or after auto-approach, opens the shop window.
- The shop shows player gold, starter stock, and sellable bag items.
- The inventory window also shows current player gold.
- Shop buy and sell actions require a second click confirmation before currency or items change.
- Buying refuses full bags or insufficient gold without taking currency.
- Selling removes the exact item instance from the bag and adds gold.
- Bought items, sold items, remaining inventory, equipment, quests, level, position, and gold persist through restart.
- Existing movement, combat, Heavy Strike, loot pickup, inventory, equipment, map, NPC dialogue, quests, death, and chat remain functional.

## Out Of Scope

- Final economy balance, taxes, auction, player trading, repair, durability, stack sizes, limited vendor inventory, buyback, or server-authoritative currency.
- Final shop UI art, icons, tooltips, and drag-preview polish.
- Full NPC interaction menu with dialogue choices such as quests, trading, lore, and close.
- New merchant quests, new NPCs, new zones, or new item models.
