# 🎮 GhostBeam - Complete Requirements Checklist
## Everything the Game Should Have (According to Documentation)

**Created:** April 20, 2026  
**Based on:** README.md, ARCHITECTURE.md, GAME_DESIGN.md, PROJECT_STATUS.md, SETUP_COMPLETO.md, GHOSTBEAM_COMPLETE_GUIDE.md, GUIA_IMPLEMENTACAO_UNITY.md, HIERARQUIA_PASSO_A_PASSO.md

---

## 📊 MANAGERS (Core Game Systems)

### Global Managers (Singleton Pattern - DontDestroyOnLoad)
- [ ] **GameManager**
  - [ ] Scene flow management (MainMenu, Gameplay, GameOver, Pause)
  - [ ] Event system for onGameOver, onPauseChanged, onMainMenuChanged
  - [ ] Pause/Resume toggle functionality
  - [ ] Scene transitions
  - [ ] Main game state controller
  
- [ ] **AudioManager**
  - [ ] Background music playback control
  - [ ] Sound effects management
  - [ ] Master volume control (0-1 range)
  - [ ] SFX volume slider
  - [ ] Music volume slider
  - [ ] Singleton with DontDestroyOnLoad
  
- [ ] **SettingsManager**
  - [ ] Master Volume property (saved in PlayerPrefs)
  - [ ] Vibration Enable/Disable toggle
  - [ ] Timer HUD toggle
  - [ ] Performance Overlay toggle
  - [ ] Save/Load settings from PlayerPrefs
  - [ ] Singleton with DontDestroyOnLoad

### Scene-based Managers
- [ ] **ScoreManager**
  - [ ] Track score points
  - [ ] Track collected coins
  - [ ] Survival time tracking
  - [ ] Highscore persistence (PlayerPrefs)
  - [ ] Coins persistence (PlayerPrefs)
  - [ ] Events: onScoreChanged, onCoinsChanged, onHighscoreUpdated
  - [ ] AddScore(int amount)
  - [ ] AddCoins(int amount)
  - [ ] TrySpendCoins(int amount)
  - [ ] GetHighScore()
  - [ ] GetCoins()
  - [ ] GetCurrentScore()

- [ ] **SpawnManager**
  - [ ] Enemy pooling system
  - [ ] Progressive difficulty stages (Stage 1, 2, 3)
  - [ ] Dynamic spawn rate adjustment
  - [ ] Max simultaneous enemies limit
  - [ ] Enemy type distribution per stage
  - [ ] Spawn position calculation (away from player)
  - [ ] Spawn margin safety (8 units from Luna)
  - [ ] Stage-based AI behavior variations
  - [ ] Stage 1: 0-35s (Penado 70%, Ictericia 30%)
  - [ ] Stage 2: 35-125s (All types distributed)
  - [ ] Stage 3: 125s+ (Random distribution, max pressure)

- [ ] **BatteryPickupSpawner**
  - [ ] Spawn battery pickups on enemy death
  - [ ] Apply multipliers per enemy type (1.5x-2.5x)
  - [ ] Object pooling for pickups
  - [ ] Lifetime management (despawn after 8 seconds)
  - [ ] Multipliers: Penado(1.5x), Ictericia(1.5x), Ectogangue(2.0x), Tita(2.5x), Espectro(2.0x)

- [ ] **CoinPickupSpawner**
  - [ ] Spawn coin pickups on enemy death
  - [ ] Apply multipliers per enemy type (1.0x-1.5x)
  - [ ] Object pooling for pickups
  - [ ] Lifetime management (despawn after 15 seconds)
  - [ ] Multipliers: Penado(1.0x), Ictericia(1.1x), Ectogangue(1.0x), Tita(1.5x), Espectro(1.2x)

- [ ] **UpgradeManager**
  - [ ] Track purchased upgrades
  - [ ] Save/Load upgrade state (PlayerPrefs)
  - [ ] Apply upgrade effects to gameplay

- [ ] **SkinManager** (DEBUG ONLY)
  - [ ] Character skin selection (for future content)

---

## 🎨 UI COMPONENTS & MENUS

### Main Menu UI
- [ ] **Main Menu Canvas**
  - [ ] Menu panel container
  - [ ] Title/Logo display
  - [ ] **Play Button** (green) → Starts gameplay
  - [ ] **Shop Button** (yellow) → Opens shop panel
  - [ ] **Challenges/Daily Button** (orange) → Opens daily quests
  - [ ] **Settings Button** (blue) → Opens settings panel
  - [ ] **Quit Button** (red) → Application.Quit()
  - [ ] Best Score display
  - [ ] Canvas Scaler (responsive)
  - [ ] EventSystem

### In-Game HUD Canvas
- [ ] **HUD Controller** Component
  - [ ] Health display (TxtHealth/TxtVida)
  - [ ] Score display (TxtScore)
  - [ ] Highscore display (TxtRecorde)
  - [ ] Battery display (TxtBateria)
  - [ ] Coins display (TxtMoedas)
  - [ ] Timer display (TxtTempo - MM:SS format)
  - [ ] Wave/Stage display (TxtFase)
  - [ ] Battery visual bar (Slider component)
  - [ ] Health bar visual (dynamic color: green→yellow→red)
  - [ ] Pause button (top-right corner)
  - [ ] Performance Overlay toggle (FPS, memory)
  - [ ] Safe area aware positioning
  - [ ] Real-time value synchronization

### Pause Menu Canvas
- [ ] **Pause Panel**
  - [ ] Pause indicator text ("PAUSADO" / "PAUSED")
  - [ ] **Resume Button** (green) → Unpause
  - [ ] **Settings Button** (blue) → Settings from pause
  - [ ] **Main Menu Button** (red) → Return to main menu
  - [ ] Semi-transparent overlay (darkens gameplay)
  - [ ] Block input to gameplay while paused

### Game Over Canvas
- [ ] **Game Over Panel**
  - [ ] "GAME OVER" title
  - [ ] Final Score display
  - [ ] Final Highscore display
  - [ ] Coins earned this session
  - [ ] **Restart Button** → Reloads scene
  - [ ] **Main Menu Button** → Returns to menu
  - [ ] Fade-in animation

### Settings Menu Canvas
- [ ] **Settings Panel**
  - [ ] Master Volume slider (0-1)
  - [ ] SFX Volume slider (0-1)
  - [ ] Music Volume slider (0-1)
  - [ ] Vibration toggle (mobile)
  - [ ] Timer HUD toggle
  - [ ] Performance Overlay toggle
  - [ ] Language selection (Portuguese PT-BR at minimum)
  - [ ] Apply/Save button
  - [ ] Close/Back button
  - [ ] Apply settings immediately

### Shop/Upgrade Panel
- [ ] **Shop Controller**
  - [ ] **Upgrade 1: Lanterna Melhorada (Improved Flashlight)**
    - [ ] Cost: 750 / 1250 / 1850 coins
    - [ ] Effect: 2x intensidade no tier 3
    - [ ] Max level: 3
    - [ ] Display cost and current level
  - [ ] **Upgrade 2: Bateria Melhorada (Improved Battery)**
    - [ ] Cost: 1000 / 1600 / 2300 coins
    - [ ] Effect: 2x capacidade no tier 3
    - [ ] Max level: 3
    - [ ] Display cost and current level
  - [ ] **Upgrade 3: Vida Extra (Extra Life)**
    - [ ] Cost: 150 / 150 / 150 coins
    - [ ] Effect: +1 vida por tier (max 6)
    - [ ] Max level: 3
    - [ ] Display cost and current level
  - [ ] Buy button for each upgrade
  - [ ] Coin balance display
  - [ ] "Not enough coins" message
  - [ ] Close/Back button

### Leaderboard/Ranking Panel
- [ ] Highscore display
- [ ] Local leaderboard (top scores)
- [ ] Close/Back button

### Daily Quests Panel
- [ ] Daily challenges placeholder (not fully implemented)
- [ ] Reward preview
- [ ] Close/Back button

### UI Utility Scripts
- [ ] **UIBootstrapper**
  - [ ] Auto-link canvas elements by name
  - [ ] Auto-apply layout configurations
  - [ ] Remove placeholder text
  - [ ] Configure canvas scaler
  - [ ] Safe area handling
  - [ ] EventSystem creation if missing

- [ ] **MainMenuUIBuilder** (MEGA SCRIPT)
  - [ ] Create 5 Canvases automatically
  - [ ] Manage menu navigation
  - [ ] Update HUD in real-time
  - [ ] Detect Game Over automatically
  - [ ] Handle ESC key pause
  - [ ] Event subscriptions to GameManager

---

## ⚙️ SYSTEMS & GAMEPLAY FEATURES

### Player Character System
- [ ] **Luna Controller** (Player Character)
  - [ ] Sprite Renderer with Luna/Flash sprite
  - [ ] Circle Collider 2D (radius 0.5)
  - [ ] Rigidbody 2D (Dynamic, Gravity Scale 0, Constraints: Freeze Rotation Z)
  - [ ] 8-directional movement (N, NE, E, SE, S, SW, W, NW)
  - [ ] Movement speed: 8 units/sec
  - [ ] Tag: "Player"
  - [ ] Position bounds checking (stay within 10x15 area)
  - [ ] Sprite rotation toward movement direction

- [ ] **FlashlightController**
  - [ ] Light 2D Spot component attached
  - [ ] Light properties:
    - [ ] Light Type: Spot
    - [ ] Intensity: 1.0
    - [ ] Outer Radius: 15 units
    - [ ] Inner Radius: 0
    - [ ] Outer Angle: 70 degrees
    - [ ] Color: White
    - [ ] Mode: Additive
    - [ ] Shadows: Enabled
    - [ ] Sorting Order: 0
  - [ ] Rotation speed: 10 multiplier
  - [ ] Rotation follows right joystick input
  - [ ] Continuous rotation update

### Health System
- [ ] **Health System**
  - [ ] Initial health: 3 lives
  - [ ] Damage on contact: -1 HP
  - [ ] TakeDamage() method
  - [ ] Event: onHealthDepleted
  - [ ] Event: onHealthChanged
  - [ ] Display current health
  - [ ] Game Over trigger when health ≤ 0

### Battery System
- [ ] **Battery System**
  - [ ] Maximum capacity: 2x no tier 3
  - [ ] Drain rate: 2 pts/sec when illuminating
  - [ ] No drain when flashlight off
  - [ ] Event: onBatteryDepleted
  - [ ] Event: onBatteryChanged
  - [ ] Event: onBlackout (lantern turns off)
  - [ ] Blackout mechanics: Lantern off for 3 seconds if battery reaches 0
  - [ ] Game Over if blackout continues after 3 seconds
  - [ ] Recharge only via pickup (no natural regeneration)
  - [ ] Upgrade effect: +50 capacity per level (100-200 max)

- [ ] **Battery Pickup System**
  - [ ] Pickup value: +100 battery points
  - [ ] Attraction range: 3 units (magnetic pull to player)
  - [ ] Lifetime on ground: 8 seconds (then disappears)
  - [ ] Visual: Yellow circle sprite
  - [ ] Collider: Circle, radius 0.2, is trigger
  - [ ] Touch detection with Luna
  - [ ] Spawn on enemy death with multipliers

- [ ] **Battery Pickup Spawner**
  - [ ] Object pooling integration
  - [ ] Spawn triggers on enemy defeat
  - [ ] Multiplier application: 1.5x-2.5x per enemy type
  - [ ] Spawn position relative to enemy death point

### Scoring System
- [ ] **Score Points**
  - [ ] +5 points per second survived
  - [ ] +15 points per enemy killed
  - [ ] +25 bonus points per combo x2
  - [ ] Stage bonuses:
    - [ ] Stage 1: +0% (baseline)
    - [ ] Stage 2: +25% (1.25x multiplier)
    - [ ] Stage 3: +50% (1.50x multiplier)
  - [ ] Unlimited maximum per session
  - [ ] Saved as Highscore in PlayerPrefs

- [ ] **Coin Economy**
  - [ ] +1 base coin per enemy killed
  - [ ] Enemy type multipliers: 1.0x-1.5x
  - [ ] 100% drop rate on enemy death
  - [ ] Pickup lifetime: 15 seconds
  - [ ] Attraction range: 3 units
  - [ ] Persistent storage in PlayerPrefs
  - [ ] Used for upgrades in shop
  - [ ] Visual: Orange circle sprite

- [ ] **Coin Pickup System**
  - [ ] Spawn on enemy death
  - [ ] Object pooling
  - [ ] Magnetic attraction to player
  - [ ] Multiplier application per enemy type
  - [ ] Collider: Circle, radius 0.15, is trigger

### Enemy System
- [ ] **Enemy Controller**
  - [ ] Base AI that follows Luna
  - [ ] Light detection using mathematical calculation:
    - [ ] Distance check: < 15 units
    - [ ] Angle check: < 35 degrees cone
    - [ ] Vector2.Angle() calculation
  - [ ] Takes damage when illuminated
  - [ ] Triggers object pooling on death
  - [ ] Spawns pickups on death
  - [ ] Adds score on death
  - [ ] Collision damage: -1 HP to Luna on contact
  - [ ] Sprite direcional por arquétipo

- [ ] **Enemy Types (5 Total)**
  1. **PENADO (Basic)**
     - [ ] Difficulty: ⭐☆☆☆☆
     - [ ] Time to kill: 2.0 seconds
     - [ ] Speed: 1.5 u/sec
     - [ ] Health: 100 HP
     - [ ] Coin multiplier: 1.0x
     - [ ] Battery multiplier: 1.5x
     - [ ] Spawn frequency: 40%
     - [ ] Behavior: Straight line toward player
     - [ ] Attack: Contact damage (-1 HP)
     - [ ] Appearance: Basic sprite

  2. **ICTERICIA (Fast)**
     - [ ] Difficulty: ⭐⭐☆☆☆
     - [ ] Time to kill: 2.0 seconds
     - [ ] Speed: 1.9 u/sec
     - [ ] Health: 100 HP
     - [ ] Coin multiplier: 1.1x
     - [ ] Battery multiplier: 1.5x
     - [ ] Spawn frequency: 30%
     - [ ] Behavior: Faster pursuit, attempts interception
     - [ ] Attack: Contact damage
     - [ ] Appearance: Faster variant sprite
     - [ ] First appears: Stage 2+

  3. **ECTOGANGUE (Group)**
     - [ ] Difficulty: ⭐⭐☆☆☆
     - [ ] Time to kill: 2.8 seconds
     - [ ] Speed: 1.9 u/sec
     - [ ] Health: 140 HP (more tanky)
     - [ ] Coin multiplier: 1.0x
     - [ ] Battery multiplier: 2.0x
     - [ ] Spawn frequency: 20%
     - [ ] Behavior: Random + directional toward player
     - [ ] Attack: Contact damage
     - [ ] Appearance: Group variant sprite
     - [ ] First appears: Stage 2+ (pairs/trios)

  4. **TITA (Heavy Tank)**
     - [ ] Difficulty: ⭐⭐⭐⭐☆
     - [ ] Time to kill: 4.6 seconds
     - [ ] Speed: 0.95 u/sec
     - [ ] Health: 230 HP (very tanky)
     - [ ] Coin multiplier: 1.5x
     - [ ] Battery multiplier: 2.5x
     - [ ] Spawn frequency: 5%
     - [ ] Behavior: Slow, persistent, high pressure
     - [ ] Attack: Contact damage
     - [ ] Appearance: Heavy/large sprite
     - [ ] First appears: Stage 3+ only

  5. **ESPECTRO (Fragile Fast)**
     - [ ] Difficulty: ⭐⭐⭐☆☆
     - [ ] Time to kill: 0.95 seconds
     - [ ] Speed: 3.15 u/sec (very fast)
     - [ ] Health: 47 HP (very fragile)
     - [ ] Coin multiplier: 1.2x
     - [ ] Battery multiplier: 2.0x
     - [ ] Spawn frequency: 5%
     - [ ] Behavior: Chaotic + fast movement
     - [ ] Attack: Contact damage
     - [ ] Appearance: Fragile/ghost-like sprite
     - [ ] First appears: Stage 2+ rarely, Stage 3 common

### Difficulty Progression (Stages)
- [ ] **Stage 1: Presentation (0-35s)**
  - [ ] Spawn interval: 2.8s → 1.6s (gradual acceleration 0.018/sec)
  - [ ] Max simultaneous: 6 enemies
  - [ ] Enemy types: Penado (70%), Ictericia (30%)
  - [ ] Score multiplier: +0% (baseline)
  - [ ] Pickup multiplier: +0% (baseline)
  - [ ] Objective: Tutorial/introduction, low pressure

- [ ] **Stage 2: Test (35-125s)**
  - [ ] Spawn interval: 1.4s → 1.0s (acceleration 0.020/sec)
  - [ ] Max simultaneous: 12 enemies
  - [ ] Enemy types: All types (Penado 40%, Ictericia 30%, Ectogangue 20%, Espectro 5%, Tita 5%)
  - [ ] Score multiplier: +25% (1.25x)
  - [ ] Pickup multiplier: +25%
  - [ ] Objective: Test player skills, moderate pressure

- [ ] **Stage 3: Climax (125s+)**
  - [ ] Spawn interval: 0.95s → 0.75s (acceleration 0.025/sec)
  - [ ] Max simultaneous: 18 enemies
  - [ ] Enemy types: Random distribution of all types
  - [ ] Score multiplier: +50% (1.50x)
  - [ ] Pickup multiplier: +50%
  - [ ] Objective: Extreme survival, nearly impossible after 180s

### Gameplay Mechanics
- [ ] **Game Loop**
  - [ ] Movement input processing
  - [ ] Flashlight rotation update
  - [ ] Enemy AI updates
  - [ ] Light detection calculations
  - [ ] Damage application
  - [ ] Pickup attraction
  - [ ] Score accumulation
  - [ ] Battery drain
  - [ ] HUD updates
  - [ ] Win condition: N/A (endless)
  - [ ] Lose condition: Health ≤ 0 or blackout > 3s

- [ ] **Object Pooling System**
  - [ ] Generic ObjectPool<T> class
  - [ ] Enemy pooling (initial size: 30)
  - [ ] Battery pickup pooling
  - [ ] Coin pickup pooling
  - [ ] Lifecycle: Get(), Release()
  - [ ] OnGet callback (reset state)
  - [ ] OnRelease callback (clean up)

### Input System
- [ ] **Input Method: Classic Input Manager** (NOT new Input System)
  - [ ] Keyboard input fallback (PC/Editor)
  - [ ] Touch input (mobile)
  - [ ] ESC key for pause
  - [ ] Space for menu navigation

---

## 📱 MOBILE UI & TOUCH CONTROLS

### Virtual Joysticks (Landscape Only)
- [ ] **Left Joystick (Movement)**
  - [ ] Position: Bottom-left corner
  - [ ] Safe area aware
  - [ ] Detection radius: 100px
  - [ ] Output: Vector2 (-1 to +1 normalized)
  - [ ] 8-directional input (cardinal + diagonals)
  - [ ] Latency: <50ms
  - [ ] Visual indicator (optional)

- [ ] **Right Joystick (Aiming/Flashlight Rotation)**
  - [ ] Position: Bottom-right corner
  - [ ] Safe area aware
  - [ ] Detection radius: 100px
  - [ ] Output: Angle (0-360°) or Vector2
  - [ ] Continuous rotation following touch
  - [ ] Latency: <50ms
  - [ ] Visual indicator (optional)

### Safe Area Handling
- [ ] Safe area aware positioning for notches (iPhone X+)
- [ ] Handle home indicators (bottom)
- [ ] Landscape-only orientation lock
- [ ] Aspect ratio: 16:9 (1920x1080)
- [ ] Resolution scaling (Canvas Scaler)
- [ ] Padding around edges for virtual joysticks

### Mobile-Specific Features
- [ ] Vibration feedback support (toggle in settings)
- [ ] Haptic feedback on damage
- [ ] Haptic feedback on kill
- [ ] Touch responsiveness optimization
- [ ] Screen size adaptation
- [ ] Device orientation lock: Landscape

---

## 🎵 AUDIO SYSTEM & REQUIREMENTS

### Audio Manager Features
- [ ] Background music playback
- [ ] Sound effects playback
- [ ] Master volume control
- [ ] Music volume slider
- [ ] SFX volume slider
- [ ] Mute functionality
- [ ] Fade in/out effects

### Music Tracks Required
- [ ] **Background Music (Gameplay)**
  - [ ] Looping, ambient/tense
  - [ ] Fits dark theme
  - [ ] Adjustable BPM for intensity

- [ ] **Main Menu Music**
  - [ ] Looping, welcoming
  - [ ] Lower intensity than gameplay

- [ ] **Game Over Music**
  - [ ] Optional, short loop or one-shot

### Sound Effects Required
- [ ] **Flashlight activation** - Light on/off sound
- [ ] **Enemy eliminated** - Death/defeat sound
- [ ] **Damage taken** - Luna hurt sound
- [ ] **Pickup collected** - Coin pickup sound
- [ ] **Battery pickup** - Battery pickup sound
- [ ] **Menu button click** - UI interaction sound
- [ ] **Pause activated** - Pause menu sound
- [ ] **Resume game** - Resume sound
- [ ] **Upgrade purchased** - Shop purchase sound
- [ ] **Insufficient coins** - Error/fail sound
- [ ] **Blackout warning** - Low battery warning beep
- [ ] **Battery depleted** - Battery empty alarm

### Audio Implementation
- [ ] AudioListener component (single instance, NOT duplicated)
- [ ] AudioSource for music (looping)
- [ ] AudioSource for SFX (pooled or multiple)
- [ ] Volume persistence (PlayerPrefs)
- [ ] Mute all functionality
- [ ] Individual track volume adjustment

---

## 🎁 PREFABS THAT NEED TO BE CREATED

### Character Prefabs
- [ ] **Luna (Player Prefab)**
  - [ ] Sprite: Luna/Flash character
  - [ ] CircleCollider2D
  - [ ] Rigidbody2D (Dynamic, Gravity Scale 0)
  - [ ] LunaController script
  - [ ] FlashlightController script
  - [ ] HealthSystem component
  - [ ] Flashlight child object (Light 2D Spot)
  - [ ] Tag: Player
  - [ ] Layer: Player
  - [ ] Rotation constraints

### Enemy Prefabs
- [ ] **Enemy (Generic/Base Enemy)**
  - [ ] Sprite Renderer (generic/placeholder)
  - [ ] CircleCollider2D (trigger)
  - [ ] Rigidbody2D (Kinematic or Dynamic with Gravity 0)
  - [ ] EnemyController script
  - [ ] PooledObject component
  - [ ] Variable data container for enemy type
  - [ ] Animation controller (if using animations)
  - [ ] Separate prefabs OR variants for each enemy type
    - [ ] Penado variant
    - [ ] Ictericia variant
    - [ ] Ectogangue variant
    - [ ] Tita variant
    - [ ] Espectro variant

### Pickup Prefabs
- [ ] **Battery Pickup**
  - [ ] Sprite: Yellow circle or battery icon
  - [ ] CircleCollider2D (trigger, radius 0.2)
  - [ ] BatteryPickup script
  - [ ] PooledObject component
  - [ ] Visual indicator (glow/rotation)
  - [ ] Audio clip reference

- [ ] **Coin Pickup**
  - [ ] Sprite: Orange circle or coin icon
  - [ ] CircleCollider2D (trigger, radius 0.15)
  - [ ] CoinPickup script
  - [ ] PooledObject component
  - [ ] Visual indicator (rotation/scale)
  - [ ] Audio clip reference

### VFX/Effect Prefabs
- [ ] **Enemy Death Effect**
  - [ ] Particle system (dissolve/explosion)
  - [ ] Fade animation
  - [ ] Lifetime: 0.5-1 second

- [ ] **Damage Flash Effect** (on Luna)
  - [ ] Screen flash (white overlay)
  - [ ] Camera shake
  - [ ] Duration: 0.2-0.3 seconds

- [ ] **Pickup Collection Effect**
  - [ ] Particle system (collect animation)
  - [ ] Scale animation (pop)
  - [ ] Duration: 0.3-0.5 seconds

---

## 🎬 SCENES NEEDED

### Scene 1: MainMenu
- [ ] **Purpose:** Main menu with game options
- [ ] **Contents:**
  - [ ] GameManager singleton
  - [ ] AudioManager singleton
  - [ ] Main Menu Canvas (Play, Shop, Settings, Challenges, Quit)
  - [ ] Shop Canvas (upgrades)
  - [ ] Settings Canvas (audio, vibration, etc.)
  - [ ] Leaderboard Canvas (scores)
  - [ ] Daily Quests Canvas (placeholder)
  - [ ] Background/UI decorations
  - [ ] EventSystem

### Scene 2: Gameplay
- [ ] **Purpose:** Main gameplay loop
- [ ] **Contents:**
  - [ ] GameManager reference
  - [ ] Main Camera (Orthographic, size 7.5, position 0,0,-10)
  - [ ] Global Light 2D (Intensity 0)
  - [ ] Background sprite (blue-dark, scale 100x100, sorting -10)
  - [ ] Luna (Player) with Flashlight
  - [ ] Enemies container (initially empty, spawn at runtime)
  - [ ] Items container (coins/batteries, spawn at runtime)
  - [ ] Systems container:
    - [ ] GameManager
    - [ ] ScoreManager
    - [ ] SpawnManager
    - [ ] BatteryPickupSpawner
    - [ ] CoinPickupSpawner
    - [ ] AudioManager
    - [ ] SettingsManager
    - [ ] UpgradeManager
    - [ ] UIBootstrapper
  - [ ] UI Container:
    - [ ] Canvas HUD (health, score, battery, coins, timer, wave, pause button)
    - [ ] Canvas Pause Menu
    - [ ] Canvas Game Over
  - [ ] EventSystem
  - [ ] Orthographic projection setup

### Scene 3: GameOver (Optional, can be part of Gameplay)
- [ ] **Purpose:** Game over screen
- [ ] **Contents:**
  - [ ] Game Over Canvas
  - [ ] Score display
  - [ ] Highscore comparison
  - [ ] Coins earned
  - [ ] Restart button
  - [ ] Main Menu button
  - [ ] (Can be integrated into Gameplay scene as inactive canvas)

---

## 🎮 OTHER GAME SYSTEMS & FEATURES

### Performance & Optimization
- [ ] **Performance Overlay**
  - [ ] FPS counter display (toggleable)
  - [ ] Memory usage display
  - [ ] Frame time display
  - [ ] Toggle via settings menu
  - [ ] Minimal performance impact when disabled

- [ ] **Optimization Features**
  - [ ] Object pooling (enemies, pickups)
  - [ ] Spatial partitioning (if needed)
  - [ ] Efficient light detection (math-based, not physics)
  - [ ] No redundant object searches
  - [ ] Efficient enemy AI loops
  - [ ] Reduced draw calls via batching
  - [ ] 60 FPS target stable on mobile

### Gameplay Features from Documentation
- [ ] **Pause System**
  - [ ] ESC key trigger
  - [ ] UI button trigger
  - [ ] Pause menu overlay
  - [ ] Time.timeScale = 0 when paused
  - [ ] Time.timeScale = 1 when resumed
  - [ ] Pause state persists across menus

- [ ] **Main Game Loop**
  - [ ] Play scene
  - [ ] Pause scene
  - [ ] Game Over scene
  - [ ] Return to Menu
  - [ ] Restart gameplay

- [ ] **Floating Text System** (Mentioned in docs)
  - [ ] Score popup when collecting coins
  - [ ] Score popup when killing enemies
  - [ ] Damage popup on Luna
  - [ ] Position: Spawns at event location
  - [ ] Animation: Float upward + fade out
  - [ ] Duration: 1-2 seconds

- [ ] **Combo System** (Score bonus)
  - [ ] Track consecutive kills
  - [ ] +25 bonus per combo x2
  - [ ] Visual indicator (on-screen counter)
  - [ ] Reset on damage taken

### Mobile Build Specifications
- [ ] **Target Platforms:**
  - [ ] Android (API 21+ / Lollipop)
  - [ ] iOS (11.0+)
  - [ ] Landscape ONLY (no portrait)

- [ ] **Resolution & Display:**
  - [ ] Target resolution: 1920 x 1080 (16:9)
  - [ ] Graphics API: OpenGL ES 3.0 (Android), Metal (iOS)
  - [ ] Color Space: Linear
  - [ ] Render Pipeline: URP 2D
  - [ ] Performance: 60 FPS stable

- [ ] **Build Requirements:**
  - [ ] Compile with 0 errors, 0 warnings
  - [ ] TextMesh Pro configured
  - [ ] All prefabs assigned in scene
  - [ ] All audio clips referenced
  - [ ] Canvas scaler configured
  - [ ] Safe area implemented
  - [ ] Input system configured
  - [ ] Orientation locked to landscape

### Accessibility & Localization
- [ ] **Localization Support**
  - [ ] Portuguese (PT-BR) - Primary language
  - [ ] English - Secondary (optional)
  - [ ] UI text externalized
  - [ ] Font support for special characters

- [ ] **Accessibility Features**
  - [ ] Font size adjustable
  - [ ] Color contrast sufficient
  - [ ] Button sizes suitable for touch
  - [ ] Text readability in all UI

### Testing & Validation
- [ ] **Functionality Testing**
  - [ ] All buttons clickable and functional
  - [ ] All sliders work correctly
  - [ ] Game logic: spawn, damage, death
  - [ ] Pause/Resume functionality
  - [ ] Menu navigation
  - [ ] Settings persistence

- [ ] **Platform-Specific Testing**
  - [ ] Android device testing
  - [ ] iOS device testing
  - [ ] Safe area handling on notched devices
  - [ ] Home indicator handling
  - [ ] Touch input latency <50ms
  - [ ] Performance on target devices (60 FPS)

- [ ] **Edge Cases**
  - [ ] Rapid pause/unpause
  - [ ] Memory stress (many kills rapid)
  - [ ] Battery depletion scenarios
  - [ ] Blackout recovery
  - [ ] Upgrade max levels reached
  - [ ] Insufficient coins for upgrades

---

## 📋 IMPLEMENTATION CHECKLIST BY PHASE

### Phase 1: Core Gameplay (FOUNDATION)
- [ ] Unity project setup with URP 2D
- [ ] Folder structure created
- [ ] 21 scripts implemented (from rebuild)
- [ ] Scene Gameplay created with all systems
- [ ] Luna with movement and flashlight
- [ ] Basic enemy spawning
- [ ] Object pooling functional
- [ ] Health system working
- [ ] Battery system working
- [ ] Score tracking working
- [ ] Compilation: 0 errors, 0 warnings

### Phase 2: Content & Prefabs
- [ ] All 5 enemy types implemented with sprites
- [ ] Luna character sprite finalized
- [ ] Enemy prefabs created and tested
- [ ] Pickup prefabs created
- [ ] VFX prefabs created
- [ ] Animations (if used) implemented

### Phase 3: UI & Polish
- [ ] Main Menu UI complete
- [ ] HUD UI complete and synced
- [ ] Game Over screen
- [ ] Pause menu
- [ ] Settings menu
- [ ] Shop menu with upgrades
- [ ] All UI responsive to different resolutions
- [ ] Safe area implemented
- [ ] Virtual joystick input working

### Phase 4: Audio
- [ ] Background music implemented
- [ ] All SFX implemented and triggered
- [ ] Volume controls working
- [ ] Audio persists between scenes
- [ ] Mute functionality

### Phase 5: Mobile Build & Testing
- [ ] Android build created and tested
- [ ] iOS build created and tested
- [ ] All features tested on real devices
- [ ] Performance validation (60 FPS)
- [ ] Touch input testing (<50ms latency)
- [ ] Safe area validation on notched devices

### Phase 6: Polish & Optimization
- [ ] Performance profiling complete
- [ ] Memory optimization
- [ ] Framerate stability verified
- [ ] Visual polish (particles, effects)
- [ ] Screen effects (damage flash, etc.)
- [ ] UI animations
- [ ] Feedback systems (haptic, audio, visual)

---

## 🎯 SUCCESS CRITERIA

From PROJECT_STATUS.md - v1.1 (April 14, 2026):

**Development Status:**
- Core Gameplay: 100% ✅
- 5 Enemy Types: 100% ✅
- Items & Economy: 100% ✅
- UI & Menus: 100% ✅
- Audio: 80% (⏳ SFX base, music placeholder)
- Mobile Touch Controls: 100% ✅
- Polish & Optimization: 60% ⏳
- Device Testing: 20% ⏳

**Performance Targets:**
- 60 FPS stable on mobile
- <50ms input latency
- Load time: <3 seconds
- Memory: <200MB on target devices

**Quality Gates:**
- 0 Critical bugs
- 0 Major bugs
- 2 Known minor bugs (screen flicker when pause rapidly, memory spike on rapid kills)
- 5 Polish improvements pending

**Session Duration Targets:**
- Stage 1 success rate: >98% (too easy)
- Stage 2 success rate: ~75% (balanced)
- Stage 3 success rate: ~25% (challenging)
- Average session: 40-60 seconds
- Player accessibility: +40% vs v1.0

---

## 📝 NOTES FROM DOCUMENTATION

### Technical Specifications
- Engine: Unity 6 LTS (minimum 2023.2)
- Render Pipeline: URP 2D
- Input: Classic Input Manager (NOT new Input System)
- UI: TextMesh Pro (TMP)
- Platforms: Android/iOS Landscape ONLY
- Resolution: 1920x1080 (16:9)

### Architecture Principles
- Singleton pattern for global managers (with DontDestroyOnLoad)
- Object pooling for performance-critical objects
- Event-driven communication between systems
- Mathematical light detection (NOT physics-based)
- PlayerPrefs for persistence
- Namespaced scripts (GhostBeam.Managers, .Player, .Enemy, etc.)

### Known Limitations
- No portrait mode support
- No multiplayer/online features
- No controller support (touch only)
- Local leaderboard only (no cloud)
- Placeholder daily quests system
- Shop placeholder (non-functional)

---

**Created:** April 20, 2026  
**Last Updated:** April 20, 2026  
**Status:** Complete Requirements Documentation  
**Scope:** 100% of documented game features
