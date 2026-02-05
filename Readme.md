# PR: Cute Survivors
Links:

[Itch.io - CuteSurvivors by Burbuu:](https://burbuu.itch.io/cute-survivors)

[Full gameplay of the first level (6 mins):](https://youtu.be/1DezpT9gl8g)

[Gameplay - Edited version (2min):](https://youtu.be/RHxEfWJAbWA)



## Introduction:
In this project, I decided to develop a **clone** of the game **Vampire Survivors**.
The objective of this game is to **survive for a certain amount of time** against endless waves of enemies.

What defines this game (and the genre that precedes it) is that **the player has no direct control over the attack**: this action occurs automatically. Thus, the player must focus on **collecting experience** to level up and improve their statistics, weapons, and abilities to survive as long as possible.

![](Doc/Caos.png)

---
## Platforms:
- Windows
- Linux
- Browser

## Controls:

**Keyboard**:
- **WASD** or **Arrows** to move.

**Controller**:
- **Left Joystick** or **D-Pad** to move horizontally.

 ----

## Game Mechanics:

The objective of the game is to survive for a certain amount of time against endless waves of enemies.

### Enemy Waves:

Every minute of the game, the difficulty increases progressively.

The game generates waves of different types of enemies that the player must dodge and defeat.

This variable difficulty aims to **pressure the player into collecting experience** to improve the character and adapt to continue playing.

### Leveling System (player):

The player must collect experience to level up.

Each time they level up, the game pauses and offers several **weapon and accessory options** to acquire or improve. In this case, I decided that each level offers 3 options to choose from.

To survive, it is necessary to **make strategic decisions** about which weapons and accessories to improve at any given time.

### Statistics System:

The game is implemented to offer various characters with different abilities and different starting weapons.

Thus, the set of classes used is oriented towards defining **flexible data structures**, allowing for easy modification and expansion of statistics. This structure facilitates the creation of new characters, accessories, enemies, etc.

### Level and Item Unlocking System:

In this work, a data saving system has been developed, designed to implement a level, character, and item unlocking system in the future.

This part of the game is not yet fully implemented, but the base structure is already prepared.

### Weapons and Accessories:
The player has 3 slots for weapons and 3 slots for accessories. Once equipped, they cannot be removed.
Accessories provide passive abilities (stat improvements such as damage, speed, number of projectiles, etc.), and each weapon has a unique projectile pattern.

The most notable feature of Vampire Survivors is that the player does not directly control the use of weapons.
Weapons instantiate projectiles automatically, attacking enemies at regular intervals.
The player cannot activate them manually; instead, they must optimize their build and wait for them to execute.

Accessories are improved through the upgrade options upon leveling up.

I have implemented three types of weapons and two types of accessories. However, the game is prepared to include many more.


### Levels:
I have implemented two different levels with different enemy wave patterns. And a menu to select them before starting the game.
![SeleccioDeNivells.png](Doc/SeleccioDeNivells.png)

---

## Game Systems:

These are the main systems of the game and the classes that implement them:

### `GameManager`:
Class responsible for managing persistent game data. It handles scene transitions (with fade in/out) and stores the data necessary to start a level. It acts as a bridge between other game systems and scenes.

### `GameplayManager`:
Class responsible for managing the **gameplay** state. It contains a **state machine** to control the game flow, with the following states: `ActiveState`, `LevelUpState`, `PauseState`, and `FinishState`. Each of these states manages its own entry and exit flow with `Enter` and `Exit` methods.

![End of the level, represented by the `FinishState`.](Doc/StageClear.png)

### Audio:
For audio, I used the class I developed in previous assignments: `AudioManager`. I made a change to allow adjusting the volume of the clips being played. I use **Scriptable Objects** to define a dictionary of clips, making it easy to reference them in the code.


### Enemies:

The classes that manage enemies are: `EnemyController`, `EnemyManager`, and `EnemySpawner`. Additionally, I defined enemy statistics using **Scriptable Objects** and data classes, following a **data-driven** approach.
To manage dozens or hundreds of enemies simultaneously, I implemented an Object Pooling system where enemy instances are created at the start of the game and reused throughout its execution. Enemy data (statistics and animations) are oriented toward this system.
Furthermore, enemies update their pathfinding through a *staggered updates* system.

I decided to implement this system to experiment with these techniques, regardless of whether they were strictly necessary for the game's scale.

- `EnemyManager`:
This class holds references to all enemy instances and manages them. It provides public `Spawn` and `Despawn` methods, which allow reading each enemy's data and assigning them a position within the scene.
Additionally, it provides access to active enemy positions for the weapon system. Finally, it is also responsible for coordinating **pathfinding updates, processing a fixed number of enemies per Update**.

- `SpawnManager`:
Responsible for reading the level's `StageData` and spawning enemies according to the patterns defined in that data. It passes the necessary data to spawn enemies to the Enemy Manager.
I had planned to include what I called **SpawnEvents**, which would instantiate multiple enemies simultaneously following specific patterns. However, this feature is not finished and was not included in the game (although it is present in the source code).

- `EnemyController`:

The Enemy Controller handles combat and movement. For pathfinding, I created a public method called `UpdatePathfinding`. It does not use Update to prevent pathfinding from being updated every frame.
When an enemy dies, it notifies the Enemy Manager (and other systems that keep track) so it can be removed from the active enemy list.
Upon death, enemies instantiate experience (and sometimes extra health), which the player must collect. These objects are instantiated by the `EnemyController`.

### Player:
The player's logic is structured around a set of **data classes** that **separate state from behavior**, following a **data-driven** approach.

The base statistics are defined in the static class `BaseStats`, which acts as a starting point. To these base values, bonuses defined in `StatsData` (which contains the bonuses) and `CharacterData` (the final **ScriptableObject** that defines a character) are added.

The runtime behavior is divided into two classes:
- `PlayerController`: Class responsible for movement, animation management, data input, and character death. It contains a static `Instance` variable, which acts as a bridge with other game systems, such as for enemy pathfinding.

- `PlayerStats`: Manages character statistics such as health, level, experience, and all other data. It is responsible for notifying other systems of important events, such as a change in combat statistics or character death.

The decision to structure this system this way comes from facilitating the creation of different characters with different statistics and creating a **flexible RPG system**.



### Accessories, Weapons, and Inventory:

The accessory and weapon system is also designed following a **data-driven** approach, aiming to separate data from runtime behavior.

The base item data is defined with ScriptableObjects derived from `ItemData`:
- `AccessoryData`
- `WeaponData`

**Inventory**:
The inventory system is centralized in the `Inventory` class, which manages equipped weapons and accessories.

When an item is added:
- Level 1 bonuses are applied directly to `PlayerStats`.
- Events (`OnWeaponAdded`, `OnAccessoryAdded`) are triggered to notify other systems.

**Weapons**
Weapons are represented by the `Weapon` class, which contains the combat logic and the use of data from `WeaponData`. These weapons do not require player input; their behavior is automatic.

**Accessories**
Accessories are represented by the `Accessory` class and have no active behavior; they only pass data to `PlayerStats`.

### LevelUp:
Upon leveling up, this system reads the data of equipped items and provides several options for improvement or acquisition of new items, taking into account their maximum level and available slots. When the game is in this state, time is paused until an option is chosen.
- `Inventory` class: Contains information about equipped items and allows reading them, as well as adding new items.
- `ILevelUpChoice` interface: Represents the different types of options to choose from: upgrade/acquisition for a weapon or accessory.
- `LevelUpState` state: Manages the entry and exit flow of the state. It creates the upgrade/acquisition options and passes them to the UI.
- `LevelUpUI` class: Displays the options and processes the result of the choice.
![Level up choices](Doc/LevelChoices.png)

### SaveSystem:

The game includes a persistent save system implemented through the classes `SaveManager`, `SaveData`, and `RunData`.
This system is intended as a base for recording global statistics, records, and player progress.
Although the system is implemented and functional, in practice, it is not extensively used in the game. Due to a lack of content and time, I could not create an unlock system for characters, weapons, etc. But this system would function as the base.

### UI:
For the UI, I used my own resources. I made the sprites with Aseprite and used them with the **9-slice** technique.

During the level, relevant health information and the status of equipped items are displayed.

![UI during the level.](Doc/ui_gameplay.png)

### Options Menu:
The options menu allows modifying game audio and video settings using `PlayerPrefs`.

Audio options:
- Master volume
- Music volume
- Sound effects volume

Video options:
- Full screen
- Resolutions

### FX:
I have implemented two visual effects regarding combat:
- A particle effect representing the enemy's blood.
- An effect to show the damage dealt in an attack. For this system, I used a World Space Canvas.

Additionally, I implemented animations for damage, both for enemies and the player.

![Display of attack visual effects.](Doc/Effectes.png)
----

## Credits / Assets

- **Tilemaps**
  [Forest and Beach - Creation Pier](https://creationpier.itch.io/tilemaps-florest-and-beach)

- **Playable Characters**
  [72 Cute Pixel Character - BDragon1727](https://bdragon1727.itch.io/72-cute-pixel-character)

- **Projectiles and Effects**
  [Free Effect Bullet Impact & Explosion (32x32) - BDragon1727](https://bdragon1727.itch.io/free-effect-bullet-impact-explosion-32x32)

- **Enemies**
  [Tiny RPG Character Asset Pack - Zerie](https://zerie.itch.io/tiny-rpg-character-asset-pack)

- **Font**
  [Free Pixel Font: Thaleah - Unity Asset Store](https://assetstore.unity.com/packages/2d/fonts/free-pixel-font-thaleah-140059)

- **Music**
[Vampire's Bit (Demo 4) - VampireDev](https://vampiredev.itch.io/vampires-bit-demo)

[Vampire's Bit Fanart Music - Framed-Mimic-Triptune](https://famed-mimic-triptune.itch.io/vampires-bit-fanart-music)

- **SFX**

  [8 Bit sfx - IvoryRed](https://ivoryred.itch.io/8-bit-sfx)

----
## Use of AI
During the programming process, GitHub Copilot was used to accelerate code writing, taking advantage of suggestions when they fit the desired solution. In this case, no specific prompts were used.
