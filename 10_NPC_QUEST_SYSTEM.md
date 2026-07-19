# NPC And Quest System

## NPC Roles

Initial NPCs:

- Village Elder - first quest giver.
- Guard Captain - combat tutorial and enemy quests.
- Trader - buy/sell later.
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

## Quest Rule

Quest logic should be objective-based. Do not write one custom class for every quest unless a quest truly has unique behavior.
