# Gameplay Systems

## Core Systems

The first prototype needs these systems:

- character controller;
- camera;
- interaction;
- health;
- combat;
- experience;
- inventory;
- equipment;
- quests;
- saving.

## System Boundaries

### Character System

Owns:

- movement state;
- race/class identity;
- level;
- current stats;
- equipment-derived modifiers.

Does not own:

- quest definitions;
- item definitions;
- UI layout.

### Combat System

Owns:

- damage calculation;
- hit validation;
- health changes;
- death flow;
- skill execution.

Does not own:

- inventory storage;
- quest text;
- save file format.

### Inventory System

Owns:

- item stacks;
- equipment slots;
- add/remove/equip/unequip rules.

Does not own:

- visual item icons beyond references;
- enemy loot table evaluation, except receiving generated items.

### Quest System

Owns:

- quest state;
- objective progress;
- reward claiming.

Does not own:

- combat logic;
- enemy AI;
- player movement.

## Data Direction

Use data assets for:

- races;
- classes;
- items;
- enemies;
- quests;
- skills;
- loot tables.

