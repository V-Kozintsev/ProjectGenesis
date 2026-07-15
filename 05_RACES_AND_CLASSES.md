# Races And Classes

## Initial Races

Races define starting flavor and minor stat tendencies. They should not make classes unplayable.

### Human

Role: balanced starter race.

Strengths:

- flexible stats;
- easy for first implementation;
- suitable for all classes.

### Elf

Role: agile and magical.

Strengths:

- movement or dexterity tendency;
- magic affinity;
- lower durability.

### Dark Elf

Role: high-risk damage.

Strengths:

- critical damage or offensive magic tendency;
- fragile or reputation complications later.

### Orc

Role: physical strength and survivability.

Strengths:

- high health;
- melee damage;
- lower magic affinity.

### Dwarf

Role: durability and crafting/economy future.

Strengths:

- armor;
- stamina;
- crafting bonuses later.

## Initial Classes

Start with one fully playable class before implementing all classes.

### Warrior

Recommended first class.

Core:

- melee attack;
- high health;
- simple skill rotation;
- easy to test combat with.

### Mage

Core:

- ranged spell attacks;
- mana;
- burst damage;
- lower defense.

### Rogue

Core:

- fast attacks;
- critical hits;
- mobility;
- positioning later.

### Tank

Core:

- defense;
- threat generation later;
- group content role;
- not urgent before multiplayer.

## Implementation Rule

Class data should be authored as data. Combat code should not contain hardcoded checks like "if class is Warrior."

