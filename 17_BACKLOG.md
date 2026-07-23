# Backlog

## Idea Intake Rule

New ideas are recorded here even when they are not ready for implementation. Each idea should eventually answer three questions: what player problem it solves, which systems it depends on, and at which roadmap stage it belongs. An idea moves into a sprint only during explicit priority selection or when it unblocks current work.

## Gameplay Ideas


- Expand character creation with race and class choices after more options are authored.
- Harden character names before online profiles: 3-20 Unicode letters, single internal spaces, hyphens, or apostrophes; no digits, edge separators, repeated separators, control characters, or mixed-script lookalikes. Keep profanity, reserved-name, and uniqueness checks server-authoritative later.
- Campfire rest system.
- Rare enemy variants.
- Tune final experience and level-loss rates against the future economy and PvP rules.
- First dungeon-like cave.
- Reputation with village factions.
- Simple crafting.
- Merchant economy: currency, buy and sell prices, vendor stock, item sinks, repair or service costs, and save/server authority rules.
- Gathering nodes.
- Mounts much later.

## Combat Ideas

- Skill cooldowns.
- Class resource systems.
- Blocking and dodging.
- Enemy telegraph attacks.
- Boss phases.
- Damage types and resistances.

## World Ideas

- Abandoned shrine.
- Ruined watchtower.
- Merchant caravan.
- Old battlefield.
- Hidden cave chest.
- Seamless physical entrances for caves and interior spaces.
- Long-distance teleports as world services rather than replacements for nearby travel.

## Story And Visual Ideas

- Create a concise world-history and faction bible before expanding quest content.
- Define the starter region's main conflict and the player's place in it.
- Build a visual-reference board and style guide before replacing blockout art.
- Plan a modular village, wilderness, character, animation, effects, audio, and UI asset pipeline.
- During the final inventory UI pass, add item icons, a drag preview that follows the pointer, destination highlighting, and a short move, swap, or snap-back animation.
- Grow the temporary character equipment view into a full paper-doll layout only after the slot model is stable; keep equipped slots visually separate from bag storage and allow compatible drag-and-drop between them.

## Communication Ideas

- Build one typed message stream with `System`, `Loot`, `Combat`, `LocalChat`, and `Announcement` categories instead of letting gameplay systems write directly into UI text.
- Let the local prototype publish private messages such as collected items and recovery results before any network chat exists.
- Add a movable, resizable lower-left log with tabs or filters; keep short urgent errors and maintenance warnings visible even when ordinary chat is filtered.
- Add real general, nearby, party, clan, and private player chat only with server transport, moderation hooks, rate limits, mute/block/report controls, and persisted user filters.
- Treat server maintenance and exceptional world events as authenticated server announcements, not player-authored chat messages.
- Decide rare-item global announcements from rarity and world-economy rules later; avoid broadcasting ordinary loot and exposing private drops by default.

## Death And Resurrection Ideas

- Add later resurrection-in-place options through an explicit item, skill, party action, or service rather than making them free by default.
- Keep death penalties, respawn destinations, resurrection costs, and protected-zone rules data-driven for future server authority.
- Do not persist a character as permanently dead when a local session closes; restore at a validated safe point until online death-state ownership exists.

## Multiplayer Ideas

- Local prototype first.
- Later evaluate co-op.
- Later evaluate authoritative server.
- Do not start here.

## Documentation Ideas

- Expand full GDD with UI mockups.
- Add economy document.
- Add save format specification.
- Add technical risk register.
