# Combat System

## Combat Goal

Combat should be understandable, responsive, and expandable.

The first version should favor clarity over complexity.

## Initial Combat Model

Recommended first model:

- target or proximity-based melee attack;
- attack cooldown;
- health and damage;
- enemy retaliation;
- death and reward event.

## Core Stats

Initial stats:

- Health;
- Attack Power;
- Defense;
- Move Speed;
- Attack Speed;
- Critical Chance later;
- Magic Power later;
- Mana later.

## Damage Formula

Start simple:

Damage = Attacker Attack Power - Target Defense

Then clamp to a minimum value.

Later additions:

- skill coefficient;
- random variance;
- critical hit;
- armor type;
- resistances.

## Skills

Warrior first skills:

- Basic Attack;
- Heavy Strike;
- Guard or Battle Shout later.

Mage first skills:

- Fire Bolt;
- Arcane Shield later.

Rogue first skills:

- Quick Slash;
- Backstab later.

## Enemy Rules

Enemies should telegraph danger through:

- level;
- health bar;
- name color later;
- attack speed;
- size or visual category.

## Death

Player death in prototype:

- respawn at village;
- lose current combat state;
- no harsh penalty at first.

Enemy death:

- grant experience;
- roll loot;
- notify quest objectives;
- play death feedback.

