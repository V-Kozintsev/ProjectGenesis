# NPC And Quest System

## NPC Roles

Initial NPCs:

- Village Elder - first quest giver.
- Guard Captain - boar-hunt quest giver and follow-up combat-oriented quests such as the Wolf Alpha hunt.
- Village Merchant - selectable and talkable placeholder; buy/sell comes later with economy rules.
- Healer - future respawn and consumable hook.

## Quest Types

Start with:

- kill quest;
- fetch quest.

Add later:

- talk quest;
- escort quest;
- boss quest;
- exploration quest;
- chain quest.

## First Quest

Working title: Wolves Near The Road.

Objective:

- collect 5 wolf-tail trophies from wolves near Old North Road;
- each eligible wolf defeat has a chance to advance the objective;
- trophy progress is abstract quest data, not a world or inventory item.

Reward:

- 80 experience in the current prototype;
- equipment and currency remain separate future rewards.

Purpose:

- teach combat;
- teach quest tracking;
- teach return-to-NPC loop.

## Quest State

Required states:

- NotStarted;
- Active;
- ReadyToTurnIn;
- Completed.

Active and ready-to-turn-in quests may be abandoned. Abandoning removes the quest from the log and resets its objective, so accepting it again starts from zero. Completed quests are terminal and cannot be abandoned or progressed again.

## Quest Journal

The journal is a view over `QuestLog`, not a separate source of quest state. It displays dynamically generated active and completed lists plus persisted title, description, objective, giver, state, and reward metadata.

Objective notifications are event-driven and appear only when progress actually increases. Restoring saved state must not replay old progress notifications.

Authored quest content lives in reusable `QuestDefinition` assets. A definition owns the stable id, presentation, dialogue, explicit objective type and target, required count, and reward data. An `InteractableNpc` references a definition; it does not duplicate a concrete quest's fields.

Accepted progress remains a self-contained `QuestProgressData` snapshot. This keeps existing saves readable and prevents later wording or balance edits to an asset from corrupting the state already stored for a character.

`QuestLog` supports multiple active quests at the same time. Objective events advance every matching active quest independently, turning in one quest does not change another, and the shared tracker summarizes multiple active entries while the journal remains the detailed view.

An `InteractableNpc` may also hold follow-up quest definitions. Follow-ups use prerequisite quest ids, so a combat NPC can offer the next task only after the earlier task is completed.

## Quest Rule

Quest logic should be objective-based. Do not write one custom class for every quest unless a quest truly has unique behavior.
