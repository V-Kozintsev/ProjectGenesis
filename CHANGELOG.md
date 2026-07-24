# Changelog

## 2026-07-23

- Started Sprint 033 NPC Interaction Hub with documented scope and acceptance criteria.
- Added a temporary NPC action window so quests and merchant trade open from explicit choices rather than immediate repeated-click actions.

- Started Sprint 032 Merchant Shop Foundation with documented scope and acceptance criteria.
- Added persistent player gold, temporary item buy/sell prices, merchant stock, a movable shop window, and deterministic buy/sell validation.

- Started Sprint 031 Map Foundation with documented scope and acceptance criteria.
- Added a prototype mini-map and larger M map before starting merchant economy work.

- Started Sprint 030 Elite Quest And Merchant Placeholder with documented scope and acceptance criteria.
- Added the data-driven `Вожак стаи` follow-up quest after `Кабанья угроза`, tied to the Wolf Alpha objective id rather than ordinary wolves.
- Added a selectable village merchant placeholder with readable no-shop dialogue while leaving economy, currency, prices, and vendor stock for a later sprint.
- Started Sprint 029 Combat Readability Layer with documented scope and acceptance criteria.
- Added a temporary selected-target warning line for active elite attack countdowns while keeping regular enemy target panels clean.
- Started Sprint 028 First Elite Encounter with documented scope and acceptance criteria.
- Added explicit common, elite, and boss enemy ranks without coupling rank to authored balance values.
- Added a reusable telegraphed enemy attack with warning feedback, defense-aware damage, and movement-based avoidance.
- Added a level-3 Wolf Alpha prefab, its guaranteed existing-item loot table, a connected east-side clearing, separate territory, and slower respawn.
- Extended deterministic validation for attack resolution, avoidance, elite data, loot, scene wiring, and older zone and enemy-variety regressions.

## 2026-07-22

- Started Sprint 025 Reusable Quest Content Definitions with documented scope and acceptance criteria.
- Added a reusable `QuestDefinition` asset containing stable identity, dialogue, explicit defeat-target objective data, and experience reward data.
- Moved the Village Elder from concrete serialized wolf-quest fields to one quest-definition reference while preserving self-contained saved progress and the existing field-based regression API.
- Rebuilt the starter village and passed the deterministic Sprint 008-025 relevant regression suite; the user's Play Mode acceptance remains pending.
- Accepted Sprint 025 after the user's Play Mode check of the existing Village Elder quest flow.
- Started Sprint 026 Parallel Quests And Second Quest with documented scope and acceptance criteria.
- Added a Guard Captain quest giver and the data-driven `Кабанья угроза` quest for defeating two boars and receiving 100 experience.
- Kept the wolf and boar quests simultaneously active, independently progressed, persisted, and turnable in either order; the compact tracker now summarizes multiple active quests.
- Restored a bold visible close symbol on the selected-target panel and added deterministic scene coverage for it.
- Passed the Sprint 008-026 relevant regression suite and accepted Sprint 026 after the user's Play Mode check of simultaneous quests, independent progress, persistence, turn-in, and target-panel close control.
- Started Sprint 027 Zone Rules Foundation with documented scope and acceptance criteria.
- Added reusable `WorldZoneDefinition` assets and prioritized `WorldZoneVolume` scene bounds for the peaceful starter village and combat-enabled north wilds.
- Centralized combat authorization in `PlayerZoneController`; normal attacks, Heavy Strike, and hostile AI now respect peaceful protection while enemy selection remains available.
- Published authored zone-transition and combat-refusal messages through the existing local feed and refreshed the resolved zone immediately after village resurrection.
- Passed the deterministic Sprint 008-027 relevant regression suite and accepted Sprint 027 after the user's Play Mode check of peaceful combat refusal, zone transitions, wilderness combat, enemy return, and safe resurrection.
- Started Sprint 023 Character Equipment View with documented scope and acceptance criteria.
- Started Sprint 024 Local Message Feed Foundation with documented scope and acceptance criteria.
- Added a bounded typed session-local stream for system, loot, combat, local-chat, and announcement events without adding message history to character saves.
- Published useful pickup, drop, destroy, equipment, consumable, Heavy Strike, ordinary outgoing and incoming damage, death, and respawn results after successful operations.
- Added a movable lower-left feed with category filters, hide and reopen controls, the new Input System `L` shortcut, a visible draggable scrollbar, and a length-limited local chat input with blinking caret; focused text entry now blocks gameplay and camera controls, while real network player chat remains intentionally absent.
- Added a deterministic Sprint 024 validator, passed the Sprint 008-024 relevant regression suite, accepted the user's Play Mode check, and set Sprint 025 Reusable Quest Content Definitions as the next proposed step.
- Separated exact equipped weapon and body-armor instances from the eight bag positions, including safe replacement and full-bag unequip refusal.
- Combined the eight-slot bag and main-hand/body equipment presentation in the movable inventory window, with selected-item comparison and equip, replace, and unequip feedback.
- Added exact-instance world dropping plus click-or-drag trash with centered confirmation for permanent selected-item removal; both are blocked while dead.
- Added explicit independent unequip buttons for main-hand and body slots so a selected replacement item cannot hide the remove action.
- Added profile version 7 with separate equipment records and version-1 through version-6 compatibility.
- Added a deterministic Sprint 023 validator and passed the Sprint 008-023 relevant regression suite.
- Accepted Sprint 023 after the user's Play Mode check and set Sprint 024 Local Message Feed Foundation as the next proposed implementation step.
- Started Sprint 022 Death State And Respawn Choice with documented scope and acceptance criteria.
- Added PlayerDeathController and DeathRespawnView so death applies the existing penalty once, blocks gameplay, keeps the character at the death position, and resurrects in the village only after confirmation.
- Stopped pending combat, skill, NPC, loot, movement, consumable, and regeneration actions while dead.
- Added a deterministic Sprint 022 validator and passed the Sprint 008-022 relevant regression suite.
- Accepted Sprint 022 after the user's Play Mode check and set Sprint 023 Character Equipment View as the next proposed implementation step.
- Started Sprint 021 Equipment Slots And First Consumable with documented scope and acceptance criteria.
- Chose one body-armor slot, a +3 Worn Leather Armor, and a 30-health Minor Healing Potion as the smallest reusable equipment-and-consumable proof.
- Generalized item data for weapons, armor, and consumables; added body equipment defense and exact-instance consumable removal.
- Added profile version 6 with body-instance persistence while retaining version-1 through version-5 migration.
- Added armor and potion loot to existing enemies, temporary inventory feedback, builder wiring, a deterministic Sprint 021 validator, and a Play Mode preparation command.
- Passed the Sprint 008-021 relevant regression suite, including the fixed-seed wolf sword result of 9.86%.
- Passed the user's main Sprint 021 Play Mode check and moved the equipped marker before long item names so armor state remains visible in narrow temporary slots.
- Accepted Sprint 021 after the user's visual check and set Sprint 022 Death State And Respawn Choice as the next implementation step.
- Set explicit death and respawn choice, separate equipment presentation, and a typed local message feed as the next proposed foundation sequence; real player chat and global announcements remain server-owned later work.

## 2026-07-21

- Started Sprint 020 Inventory Rearrangement And Weapon Variety with documented scope and acceptance criteria.
- Added fixed inventory positions, drag-and-drop movement and swapping, and profile-version-5 slot persistence with legacy migration.
- Added a +7 Worn Axe definition, reusable boar loot, and deterministic comparison against the +4 Rusty Sword.
- Accepted Sprint 020 after deterministic validation and the user's Play Mode check of movement, swapping, weapon stat changes, exact equipment, and persistence.
- Recorded polished drag feedback for the future inventory UI pass and set equipment slots plus one consumable as the next proposed implementation.
- Accepted Sprint 019 after deterministic validation and the user's Play Mode check of separate inventory slots and weapon instances.
- Set inventory-slot rearrangement and a second weapon with contrasting stats as the next proposed implementation.

## 2026-07-20

- Started Sprint 019 Item Instances And Inventory Selection with documented scope and acceptance criteria.
- Added stable runtime item instances, profile-version-4 persistence, and migration from legacy item-id lists.
- Added eight explicit inventory slot controls and exact-instance equip and unequip actions.
- Accepted Sprint 018 after deterministic validation and the user's Play Mode checks for character stats, weapon-scaled Heavy Strike, hover tooltip, and movable windows.
- Set stable item instances and explicit inventory selection as the proposed Sprint 019 direction.
- Started Sprint 018 Character Stats with documented scope and acceptance criteria.
- Added authored warrior health and attack bonuses while preserving the existing level-1 totals.
- Unified attack composition across base, class, level, and weapon contributions.
- Changed Heavy Strike from fixed direct damage to a 1.7 current-attack multiplier that respects target defense.
- Added a temporary live character-stats window and deterministic validation for exact health, attack, and skill results.
- Simplified player-facing stat explanations, made stats, inventory, and quest windows draggable, and added a data-driven Heavy Strike hover tooltip.
- Accepted Sprint 017 after deterministic validation and the user's Play Mode checks for creation, selection, persisted identity, and persisted level.
- Recorded stricter fantasy-character naming rules as a separate pre-online hardening task.
- Started Sprint 017 Character Creation And Selection with documented scope and acceptance criteria.
- Added profile version 3 with explicit first-character creation state and compatibility for version-1 and version-2 profiles.
- Added a blocking local creation and selection flow for the first named human warrior, with gameplay input disabled until `Играть` is pressed.
- Added starter-village builder wiring and deterministic validation for lifecycle rules, name rules, scene wiring, and preserved skill and identity systems.
- Accepted Sprint 016 after Unity compilation, deterministic validators, and the user's Play Mode visual check.
- Set Sprint 017 Character Creation And Selection as the next proposed implementation step.
- Documented Sprint 016 Character Identity scope and acceptance criteria.
- Added reusable race and class data definitions plus runtime player identity.
- Added version-2 identity persistence with backward compatibility for version-1 local profiles.
- Added temporary character name, race, and class feedback to the inventory window.
- Added Sprint 016 builder wiring and deterministic identity validation.
- Accepted Sprint 015 after Unity compilation, deterministic validators, and the user's Play Mode checks.
- Set Sprint 016 Character Identity as the next proposed implementation step.
- Recorded weapon- and class-based skill scaling as part of the future Character Stats work.

## 2026-07-19

- Documented Sprint 015 scope and acceptance criteria.
- Added reusable active-skill data with a warrior Heavy Strike skill asset.
- Added selected-target skill execution with NavMesh approach, cooldown tracking, temporary hotbar feedback, and cancellation on movement or competing actions.
- Continued basic autoattacks against the same living target after Heavy Strike resolves.
- Updated the starter-village builder and added deterministic Sprint 015 validation for skill data, player wiring, hotbar presence, and preserved enemy population.
- Replaced the premature cave-entrance Sprint 015 direction with a dependency-based Russian development plan.
- Set Skills Foundation as the next proposed sprint and separated completed sprint history from future delivery order.
- Documented seamless connected-world goals, allowed teleport use, and technical gates before building a real cave.
- Added explicit lore-bible and visual-production stages so content and art follow an approved direction.
- Added idea-intake, focus, user-testing, and automatic Codex task-handoff rules.
- Documented Sprint 014 scope and acceptance criteria.
- Added a distinct reusable level-2 forest-boar prefab with authored health, combat, movement, recovery, and experience values.
- Replaced the northern wolf spawn with one boar while keeping the combat-area population at three enemies.
- Reused shared enemy AI, territory, selection, combat, recovery, death, cleanup, respawn, and level-scaled experience behavior.
- Kept boars out of the wolf loot table and wolf-tail quest progress.
- Added deterministic validation for boar prefab defaults and the two-wolf, one-boar scene population.
- Documented Sprint 013 scope and acceptance criteria.
- Added Inspector-authored enemy display names, levels, and base experience rewards.
- Displayed enemy level in the selected-target panel.
- Scaled kill experience by enemy-versus-player level difference with configurable weaker-enemy penalties, stronger-enemy bonuses, and multiplier bounds.
- Kept fixed quest rewards separate from enemy-kill scaling.
- Added deterministic validation for equal, weaker, stronger, minimum, maximum, and zero-reward cases.
- Documented Sprint 012 scope and acceptance criteria.
- Added configurable percentage and minimum experience loss on player death.
- Added experience removal across level boundaries with level 1 and zero experience as hard floors.
- Recalculated level-based health and attack bonuses after deleveling without reviving the player before respawn.
- Preserved inventory, equipment, quests, automatic persistence, and the existing full-health village respawn.
- Added deterministic validation for prefab tuning, level crossing, exact-zero behavior, the level-1 floor, and disabled penalties.
- Fixed player and selected-enemy health bars so their visible width follows current health instead of updating only the numeric label.
- Documented Sprint 011 scope and acceptance criteria.
- Added a reusable `EnemyTerritory` with Inspector-authored dimensions and a selected Scene gizmo.
- Added idle wolf roaming with configurable radius and random pause range.
- Restricted wolf detection and pursuit to the northern combat territory so enemies return before entering the peaceful village.
- Passed territory references through all three spawners and their respawned enemies.
- Added deterministic validation for roaming defaults, territory dimensions, and spawner assignments.
- Documented Sprint 010 scope and acceptance criteria.
- Added reusable delayed, interval-based `HealthRegeneration` with damage-delay resets and death safety.
- Changed returning wolves to keep their current health instead of healing fully on reaching home.
- Anchored each wolf's home point to its actual NavMesh placement and allowed close players to re-engage it during return.
- Prevented an idle wolf from repeatedly restarting `Return` while the player remains far away, allowing its recovery delay to finish.
- Measured leash distance from the wolf instead of the fleeing player, allowing a visible pursuit before returning home.
- Kept recovering wolves responsive to nearby players; proximity does not pause healing, while each new hit restarts its delay.
- Added five-second delayed wolf recovery at 3 health per second and interruption on renewed aggro.
- Added eight-second delayed player recovery at 2 health per second after active combat ends.
- Kept full restoration for player death and village respawn.
- Exposed recovery values on player and wolf prefabs and added deterministic editor validation.
- Rebuilt the starter village and kept leash, corpse cleanup, and respawn timing independently tunable.
- Documented Sprint 009 scope and acceptance criteria.
- Added reusable `LootTableDefinition` assets with item, rarity, direct chance, no-drop remainder, and one-result-per-roll behavior.
- Created a shared `LT_Wolf` asset and reduced the `Rusty Sword` chance from 35% to 10%.
- Changed `EnemyLootDrop` to roll regular world loot from its assigned table while keeping quest-trophy rolls separate.
- Updated the starter-village builder so an existing loot table and later Inspector tuning are preserved during rebuilds.
- Added deterministic table validation and a fixed-seed 100,000-roll simulation; the configured table produced a 9.86% observed rate.
- Rebuilt the starter village and verified a clean Play Mode startup with the existing saved profile.
- Kept Sprint 009 scoped away from guaranteed drops, pity counters, economy systems, randomized stats, final loot visuals, and server authority.

## 2026-07-18

- Documented Sprint 008 scope and acceptance criteria.
- Extended persisted quest progress with title, description, objective label, quest giver, and reward metadata.
- Added a `J` quest journal with dynamically generated active and completed tabs plus detailed quest information.
- Added a two-step abandon confirmation for active and ready-to-turn-in quests; retaking an abandoned quest starts at zero progress.
- Added short objective-progress notifications that trigger only for new progress, not restored profile data.
- Prevented the quest journal and NPC dialogue from remaining open on top of each other.
- Added an editor validator for acceptance, progress, abandon, retake, ready-to-turn-in, completion, and terminal-state rules.
- Rebuilt the starter village scene and visually verified the journal, persisted metadata, and abandon flow in Play mode.
- Kept Sprint 008 scoped away from new quests, map markers, dialogue trees, content-authoring tools, final UI art, and server authority.

- Documented Sprint 007 scope and acceptance criteria.
- Replaced the single fixed wolf encounter with three distributed wolf spawners in the northern combat area.
- Added six-second corpse cleanup and twelve-second enemy respawn.
- Added independent chances: 35% for a world `Rusty Sword` drop and 70% for direct wolf-tail objective progress while the collection quest is active and incomplete.
- Changed the Village Elder's objective from one guaranteed kill to collecting five chance-based wolf tails.
- Kept quest trophies out of the world and normal inventory, stored them as serializable objective progress, and capped them at the required amount.
- Added a compact upper-right quest tracker for active progress and return-to-NPC readiness.
- Tuned wolf detection, leash distance, and spawn positions so encounters can be approached individually.
- Verified quest acceptance and tracker display, wolf defeat and experience, random non-quest loot, corpse cleanup, and enemy respawn in Play mode.
- Kept Sprint 007 scoped away from additional enemy types, multiple quests, generalized loot tables, polished presentation, and server authority.

- Documented Sprint 004 scope and acceptance criteria.
- Added reusable health, combat stats, distance checks, and player progression components.
- Added click-to-target combat: the player approaches a selected enemy and performs timed basic attacks automatically.
- Unified mouse interaction: one click selects an NPC or enemy, while a double-click starts dialogue or combat with that selected target.
- Kept selected targets while moving away, separated stopping an action from clearing a target, and added `Esc` plus a target-panel close button for explicit deselection.
- Removed click-timing dependence: clicking an already selected NPC or enemy performs its action regardless of how long ago it was selected.
- Reused the top target panel for selected NPCs as well as enemies.
- Expanded the classic MMORPG camera controls: mouse-wheel click toggles the front/rear view, while a short right-click returns the camera behind the character.
- Added a first wolf enemy with idle, chase, attack, return, and death states.
- Added player death, delayed respawn at the village spawn point, and full-health restoration.
- Added a combat HUD for player health, selected-enemy health, level, and experience.
- Added 20 experience as the first combat reward.
- Added a blockout wolf and combat target marker to the starter village scene.
- Kept Sprint 004 scoped away from loot, inventory, equipment, quest completion, skills, and save/load.
- Documented Sprint 005 scope and acceptance criteria.
- Opened a north village gate and added a visually distinct combat area beyond it.
- Moved the wolf outside the village and shortened its leash so it returns before entering the peaceful area.
- Added an authored `Rusty Sword` item definition and a visible wolf loot drop.
- Added an adjustable corpse lifetime so the defeated wolf disappears after six seconds without removing its loot.
- Added click-to-approach loot collection using the existing NavMesh movement.
- Added an eight-slot player inventory and one main-hand equipment slot.
- Added a compact inventory window opened by `I` or its on-screen button.
- Made the equipped sword increase attack power from 14 to 18, with immediate equip and unequip feedback.
- Kept Sprint 005 scoped away from general zone rules, armor, trading, crafting, quest rewards, enemy respawn, and save/load.
- Documented Sprint 006 scope and acceptance criteria.
- Completed the first quest loop: accept the Village Elder's task, defeat the wolf, return, and turn it in once.
- Added serializable quest objective progress, ready-to-turn-in and completed states, and an 80 experience quest reward.
- Added level thresholds and level-up stat growth; the first full loop raises the player to level 2 with 110 maximum health and higher attack power.
- Extended the HUD and dialogue window with experience thresholds, objective progress, reward details, and completed-state feedback.
- Added automatic persistence for the player's last valid position, level, experience, quest state, inventory, and equipped weapon.
- Kept persistence behind `IPlayerPersistence`; local JSON is only a temporary offline substitute for a future authoritative online service.
- Added `Project Genesis > Development > Clear Local Prototype Profile` for clean development playthroughs.
- Verified the full loop and restored state across two Play mode sessions, then cleared the local test profile.
- Kept Sprint 006 scoped away from a manual save UI, authentication, databases, a real multiplayer backend, enemy respawn, world flags, multiple quests, and additional equipment systems.

## 2026-07-15

- Added initial project documentation foundation.
- Added game vision, design direction, technical architecture, roadmap, sprint plan, and Codex working rules.
- Completed Sprint 001 foundation setup: created `Assets/ProjectGenesis/` folder structure, starter scene, prototype player prefab, third-person camera follow, and basic movement.
- Made left-click movement the primary control style and kept keyboard movement as an optional fallback.
- Added independent right-mouse camera orbit and mouse-wheel zoom.
- Kept Sprint 001 scoped to foundation work only; combat, quests, inventory, multiplayer, and character creation UI were not started.
- Documented Sprint 002 scope and acceptance criteria.
- Built a starter village blockout with simple houses, props, road shapes, playable-area boundaries, and a minimal UI placeholder.
- Replaced direct click movement with NavMeshAgent movement that routes around obstacles and rejects unreachable destinations.
- Added runtime NavMesh building for the prototype scene so the scene stays text-serializable and easy to review.
- Added camera collision protection so the follow camera does not pass through village blockers.
- Kept Sprint 002 scoped away from combat, quests, NPC interaction, and inventory.
- Documented Sprint 003 scope and acceptance criteria.
- Added the first Village Elder NPC prototype to the starter village.
- Added range-limited click interaction with the Village Elder, a simple dialogue window, and an interaction prompt.
- Kept `E` as a temporary fallback while making mouse interaction the primary NPC flow.
- Added click-to-approach NPC interaction: single-click moves toward the NPC, while double-click opens dialogue when close enough.
- Exposed interaction range tuning through `PlayerInteractionController` inspector fields.
- Delayed enabling `NavMeshAgent` until after the runtime NavMesh is built to avoid startup NavMesh errors.
- Split NPC interaction tuning into close dialogue radius, approach distance, maximum click-to-approach distance, and double-click timing.
- Added minimal quest state handling for `Wolves Near The Road`, including `NotStarted` and `Active`.
- Kept Sprint 003 scoped away from combat, enemies, quest completion, rewards, and inventory.
