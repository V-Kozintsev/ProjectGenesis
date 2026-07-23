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

Basic Damage = Attacker Attack Power - Target Defense

Then clamp to a minimum value.

The current Heavy Strike uses the same rule with an authored skill coefficient:

Heavy Strike Damage = round(Attacker Attack Power x 1.7) - Target Defense

Attack Power already includes base, class, level, and equipped-weapon contributions, so equipping a weapon strengthens both basic attacks and Heavy Strike.

The temporary hotbar exposes player-facing skill data on hover: purpose, current damage or attack-power percentage, range, and cooldown. The description comes from the skill asset so future classes can reuse the same tooltip path.

Later additions:

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

## Zone Rules

Combat permission belongs to the player's resolved world zone rather than to UI buttons or enemy placement.

- peaceful zones allow selection but refuse basic attacks and active combat skills before movement, damage, or cooldown begins;
- entering a peaceful zone stops an active player attack and causes hostile AI to return instead of damaging the protected player;
- combat zones preserve the normal selection, approach, skill, damage, and autoattack loop;
- overlapping world-zone volumes use explicit priority so a village, sanctuary, or interior can override a broader region;
- unmapped starter-region space uses an authored combat fallback so gaps cannot accidentally become protected;
- future PvP, faction, duel, guard, and crime rules must extend this authority instead of bypassing it.

## Death

Player death in prototype:

- the current implementation applies its experience penalty once when health reaches zero;
- the character stays dead at the death position until the player confirms a resurrection choice;
- movement, combat, skills, NPC interaction, loot pickup, consumables, and regeneration are blocked while dead;
- the first centered death window offers only `Воскреснуть в деревне`;
- lose current combat state;
- no harsh penalty at first.

Resurrection at the corpse is a later item, skill, party, or service rule. The experience penalty is applied once on death, never again when the player confirms a destination.

Enemy death:

- grant experience;
- roll loot;
- notify quest objectives;
- play death feedback.

## Combat Recovery

Leaving combat and restoring health are separate phases.

- enemies keep their current health while returning to their home point;
- enemy recovery starts only after they reach home and wait through a configurable delay;
- an enemy's leash is measured from its own position to its home point, allowing a visible pursuit before return;
- a recovering enemy still detects, pursues, and attacks nearby players;
- proximity alone does not pause enemy recovery, while each new hit restarts its recovery delay;
- the player recovers only after active combat ends and a configurable no-damage delay passes;
- recovery happens in small configurable ticks, never exceeds maximum health, and never revives a dead character;
- death and respawn may still use an explicit full-health restoration independently of regeneration.
