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

- defeat 3 wolves near Old North Road.

Reward:

- experience;
- basic weapon or armor;
- small amount of gold later.

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

## Quest Rule

Quest logic should be objective-based. Do not write one custom class for every quest unless a quest truly has unique behavior.

