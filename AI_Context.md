# AI Context Map: Project Manatite

> **Goal**: Minimal token usage, fast code discovery.
> **Architecture Patterns**: Event Bus (`GameEvents`), Interfaces (`IDamageable`, `IEquippable`), ScriptableObject Data Containers, Object Pooling (`PoolManager`).

### Core & Utilities (`Assets/Scripts/Core/`)
| Class | Path | Purpose |
|-------|------|---------|
| `GameEvents` | `Events/GameEvents.cs` | Global static event bus (e.g., `SaveRequested`, `EnemyDied`). |
| `IDamageable` | `Interfaces/IDamageable.cs` | Health, damage taking, and death events. |
| `IEquippable` | `Interfaces/IEquippable.cs` | Upgradeable targets (e.g., turrets). |
| `PoolManager` | `Pooling/PoolManager.cs` | Generic O(1) prefab object pool. |
| `CameraCache` | `Utilities/CameraCache.cs` | Cached `Camera.main` replacement (perf). |
| `Physics2DBuffers`| `Utilities/Physics2DBuffers.cs`| Shared buffers for non-allocating physics overlaps (perf). |

### Battle & Entities (`Assets/Scripts/Battle/`)
| Class | Path | Purpose |
|-------|------|---------|
| `Enemy` | `Enemy/Enemy.cs` | Base enemy. Implements `IDamageable`, `IPoolable`. |
| `Player` | `Player/Player.cs` | Player ship. Implements `IDamageable`. |
| `WeaponBehaviour`| `Weapons/WeaponBehaviour.cs` | Turret. Implements `IEquippable`. Fires projectiles. |
| `ProjectileBehaviour`| `Projectiles/ProjectileBehaviour.cs`| Pooled homing projectile. |
| `ExplosionModifier`| `Weapons/Modifiers/ExplosionModifier.cs`| Damages `IDamageable`s in radius. |
| `EnemySpawner`| `Spawning/EnemySpawner.cs` | Spawns enemies via `PoolManager`. |
| `WinLevel` | `State/WinLevel.cs` | Tracks kills, triggers win sequence. |

### Upgrades & Chips
| Class | Path | Purpose |
|-------|------|---------|
| `Chip` | `Assets/Scripts/Battle/Chips/Chip.cs` | Loot pickup. |
| `Inventory` | `Assets/Scripts/Battle/Chips/Inventory.cs` | Player chip storage. |
| `SlotBehaviour`| `Assets/Scripts/UI/Battle/SlotBehaviour.cs` | Turret slot connecting `ChipSO` stats to `WeaponBehaviour`. |

### Data / ScriptableObjects
| Class | Path | Purpose |
|-------|------|---------|
| `EnemySO` | `Assets/Scripts/Battle/Enemy/EnemySO.cs` | Enemy base stats & drop table. |
| `WeaponSO` | `Assets/Scripts/Battle/Weapons/WeaponSO.cs` | Weapon base stats. |
| `ChipSO` | `Assets/Scripts/Battle/Chips/ChipSO.cs` | Chip modifiers (dmg%, speed%, etc). |
| `SpawnTableSO`| `Assets/Scripts/Battle/Spawning/SpawnTableSO.cs`| Enemy spawner weights. |
| `GameConfigSO`| `Assets/Scripts/Core/Config/GameConfigSO.cs`| Global magic numbers. Includes `ResolutionRevertDuration` (default 15s). Asset: `Assets/ScriptableObjects/Config/GameConfig.asset`. |

### Event Channel Assets (`Assets/ScriptableObjects/Events/`)
| Asset | Type | Purpose |
|-------|------|---------|
| `MasterVolumeChannel.asset` | `FloatEventChannelSO` | Broadcasts master volume (0–1) on change. |
| `MusicVolumeChannel.asset` | `FloatEventChannelSO` | Broadcasts effective music volume (master × music). |
| `SFXVolumeChannel.asset` | `FloatEventChannelSO` | Broadcasts effective SFX volume (master × sfx). |
| `SettingsAppliedChannel.asset` | `VoidEventChannelSO` | Raised when user confirms a resolution change via [Keep]. |

### UI & Flow (`Assets/Scripts/UI/`)
| Class | Path | Purpose |
|-------|------|---------|
| `Coins` | `Battle/Coins.cs` | Currency tracker HUD. |
| `DamageTextBehaviour`| `Battle/DamageTextBehaviour.cs`| Floating damage text. |
| `TriggerMovement`| `Menu/TriggerMovement.cs` | Menu reveal animations orchestrator. |
| `SelectSystem`| `WorldMap/SelectSystem.cs` | World map level selector. |
| `SettingsMenu` | `Menu/SettingsMenu.cs` | Settings panel: master/music/SFX volume (via AudioMixer + FloatEventChannelSO), VSync toggle, safe resolution apply (15s revert timer using WaitCache). Wired in MainMenu and EntreMapas. |

### Save System (`Assets/Scripts/Save/`)
| Class | Path | Purpose |
|-------|------|---------|
| `SaveLoader` | `SaveLoader.cs` | JSON serialization for stats/inventory. |
| `EquippedChips`| `SaveLoader.cs` | Struct for chip assignments. |
