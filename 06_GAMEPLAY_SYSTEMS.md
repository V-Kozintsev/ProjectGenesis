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

The current player attack total is composed from base, class, level, and equipped-weapon contributions. Player maximum health is composed from base, class, and level contributions. These values remain runtime state; UI reads them but does not recalculate them independently.

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

- stable identities for collected item instances;
- inventory capacity and concrete-slot selection data exposed to UI;
- equipment slots;
- add/remove/equip/unequip rules.

Does not own:

- visual item icons beyond references;
- enemy loot table evaluation, except receiving generated items.

`ItemDefinition` is shared authored data. `ItemInstance` represents one physical collected copy and owns its stable instance id. Equipment references an `ItemInstance`, so two items created from one definition remain distinguishable in inventory and persistence.

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
