# 🎮 GHOSTBEAM - GUIA COMPLETO DE IMPLEMENTAÇÃO
## Do Zero ao Jogo Funcional

**Versão:** 1.1  
**Data:** 15 de Abril de 2026  
**Tempo Estimado Total:** 8-12 horas (ou 2-3 dias de desenvolvimento)  
**Plataforma:** Mobile (iOS/Android) com fallback Desktop

---

## 📋 ÍNDICE GERAL

1. [PROJECT SETUP](#1-project-setup)
2. [SCENE STRUCTURE](#2-scene-structure)
3. [PLAYER IMPLEMENTATION](#3-player-implementation)
4. [ENEMY SYSTEM](#4-enemy-system)
5. [ITEM SYSTEMS](#5-item-systems)
6. [MANAGERS](#6-managers)
7. [UI/MENUS](#7-uimenus)
8. [GAMEPLAY MECHANICS](#8-gameplay-mechanics)
9. [AUDIO & VFX](#9-audio--vfx)
10. [OPTIMIZATION](#10-optimization)
11. [MOBILE BUILD](#11-mobile-build)
12. [TESTING & DEPLOYMENT](#12-testing--deployment)

---

# 1. PROJECT SETUP
**Tempo: 15-20 minutos**

## 1.1: Criar Novo Projeto Unity
- Abra Unity Hub
- Create > New project
- **Template:** 2D (URP)
- **Engine:** Unity 6 LTS
- **Project Name:** GhostBeam
- **Location:** `C:\Users\Usuario\Desktop\Game\`

## 1.2: Importar Assets Necessários
Window > TextMeshPro > Import TMP Essential Resources

## 1.3: Estrutura de Pastas
Crie estas pastas em Assets/:
```
Assets/
├── _Project/
│   ├── Scenes/
│   ├── Scripts/
│   │   ├── Managers/
│   │   ├── Player/
│   │   ├── Enemy/
│   │   ├── Items/
│   │   ├── UI/
│   │   ├── Gameplay/
│   │   └── Utilities/
│   ├── Prefabs/
│   ├── Art/
│   │   ├── Sprites/
│   │   ├── Animations/
│   │   └── UI/
│   └── Audio/
│       ├── SFX/
│       └── Music/
└── Settings/ (Unity defaults)
```

## 1.4: Configurações de Projeto
**Edit > Project Settings:**
- **Resolution:** 1920 x 1080 (16:9)
- **Aspect Ratio:** 16:9
- **Graphics API:** OpenGL ES 3.0 (Android) / Metal (iOS)
- **Color Space:** Linear
- **Render Pipeline:** 2D URP

## 1.5: Git Setup
```powershell
cd C:\Users\Usuario\Desktop\Game\GhostBeam
git init
git add .
git commit -m "Initial: GhostBeam project template"
```

---

# 2. SCENE STRUCTURE
**Tempo: 10-15 minutos**

## 2.1: Criar Cenas
No Project > Assets/_Project/Scenes/:
- `MainMenu.unity` - Menu principal
- `Gameplay.unity` - Cena de jogo
- `GameOver.unity` - Tela de derrota (OU dentro de Gameplay)

## 2.2: MainMenu Scene Structure
```
MainMenu
├── GameManager (Singleton)
├── AudioManager (Singleton)
├── Canvas - Main Menu
│   ├── Background (Image)
│   ├── PanelMenu
│   │   ├── TxtTitulo
│   │   ├── BtnJogar
│   │   ├── Row1 (LOJA, RANKING, DESAFIOS)
│   │   ├── Row2 (CONFIG, SAIR)
│   │   └── TxtBestScore
│   ├── Canvas - Shop
│   ├── Canvas - Leaderboard
│   ├── Canvas - Daily Quests
│   └── Canvas - Settings
└── SceneTransitioner (Para transições suaves)
```

*Implementação detalhada: Ver FASE 7 do MENU_IMPLEMENTATION_GUIDE.md*

## 2.3: Gameplay Scene Structure
```
Gameplay
├── GameManager
├── Camera Main
├── Player
│   ├── Sprite Renderer (Luna ou Flash)
│   ├── Collider 2D (Circle)
│   ├── Rigidbody 2D
│   └── Scripts:
│       ├── LunaController.cs
│       └── FlashlightController.cs
├── Enemies (vazio, spawn em runtime)
├── Items
│   ├── Coins (spawn em runtime)
│   └── Battery (spawn em runtime)
├── Environment
│   ├── Background (paralax ou estático)
│   └── Walls (colliders)
├── HUD Canvas
│   ├── TxtVida
│   ├── TxtScore
│   ├── TxtRecorde
│   ├── TxtBateria
│   ├── TxtMoedas
│   ├── TxtTempo
│   ├── TxtFase
│   ├── BtnPausa
│   └── Performance Overlay
├── PausePanel
│   ├── TxtPausado
│   ├── BtnResumir
│   ├── BtnMenuPrincipal
│   └── BtnSair
└── GameOverPanel (ativo só no game over)
    ├── TxtGameOver
    ├── TxtScoreFinal
    ├── TxtRecordeFinal
    ├── BtnReiniciar
    └── BtnMenuPrincipal
```

---

# 3. PLAYER IMPLEMENTATION
**Tempo: 60-90 minutos**

## 3.1: Player Model

### 3.1.1: Create Player GameObject
```csharp
// Criar em Gameplay scene
GameObject player = new GameObject("Player");
player.AddComponent<SpriteRenderer>();
player.AddComponent<CircleCollider2D>();
player.AddComponent<Rigidbody2D>();
```

### 3.1.2: Sprite Setup
- **Sprite:** Luna ou Flash character sprite
- **PPU (Pixels Per Unit):** 100
- **Size:** 1x1 (com collider)
- **Layer:** Set para "Player"

### 3.1.3: Physics Setup
```
Rigidbody2D:
├─ Body Type: Dynamic
├─ Gravity Scale: 0 (top-down)
├─ Constraints: Freeze Rotation Z
├─ Collision Detection: Continuous

CircleCollider2D:
├─ Radius: 0.5
├─ Offset: (0, 0)
```

## 3.2: Player Input System
*Arquivo: Assets/_Project/Scripts/Player/PlayerInput.cs*

```csharp
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Vector2 moveInput;
    
    private void Update()
    {
        // Desktop/Editor input
        if (Input.touchCount == 0)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            moveInput = new Vector2(horizontal, vertical).normalized;
        }
        // Mobile touch input
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // Convert touch position to world direction
            // Details in section 3.5
        }
    }
    
    public Vector2 GetMoveInput() => moveInput;
}
```

## 3.3: Luna Controller
*Arquivo: Assets/_Project/Scripts/Player/LunaController.cs*

```csharp
using UnityEngine;

public class LunaController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private Rigidbody2D rb;
    private PlayerInput input;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
    }
    
    private void FixedUpdate()
    {
        Vector2 moveInput = input.GetMoveInput();
        rb.velocity = moveInput * moveSpeed;
        
        // Rotate sprite toward movement direction
        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
```

## 3.4: Flashlight Controller
*Arquivo: Assets/_Project/Scripts/Player/FlashlightController.cs*

```csharp
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Transform lightPivot; // Filha do player
    [SerializeField] private float rotationSpeed = 10f;
    private PlayerInput input;
    
    private void Start()
    {
        input = GetComponent<PlayerInput>();
        if (lightPivot == null)
        {
            lightPivot = new GameObject("LightPivot").transform;
            lightPivot.SetParent(transform);
            lightPivot.localPosition = Vector3.zero;
        }
    }
    
    private void Update()
    {
        Vector2 moveInput = input.GetMoveInput();
        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            lightPivot.rotation = Quaternion.Lerp(
                lightPivot.rotation,
                Quaternion.AngleAxis(angle, Vector3.forward),
                Time.deltaTime * rotationSpeed
            );
        }
    }
}
```

## 3.5: Mobile Touch Input
```csharp
// Adicionar em PlayerInput.cs
private void Update()
{
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
        Vector2 direction = (touchPos - transform.position).normalized;
        moveInput = direction;
    }
}
```

---

# 4. ENEMY SYSTEM
**Tempo: 60-90 minutos**

## 4.1: Enemy Prefab

### 4.1.1: Create Enemy GameObject
```
Enemy (Prefab)
├─ SpriteRenderer (Enemy sprite)
├─ CircleCollider2D (trigger)
├─ Rigidbody2D (kinematic)
├─ EnemyController.cs
└─ PooledObject.cs (para object pooling)
```

### 4.1.2: Enemy Controller
*Arquivo: Assets/_Project/Scripts/Enemy/EnemyController.cs*

```csharp
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private int scoreReward = 100;
    
    private Transform playerTransform;
    private Rigidbody2D rb;
    private bool isAlive = true;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = FindObjectOfType<LunaController>().transform;
    }
    
    private void FixedUpdate()
    {
        if (!isAlive) return;
        
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HealthSystem health = collision.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
        
        if (collision.CompareTag("Flashlight"))
        {
            Die();
        }
    }
    
    public void Die()
    {
        isAlive = false;
        ScoreManager.Instance?.AddScore(scoreReward);
        gameObject.SetActive(false); // Para object pool
    }
}
```

## 4.2: Spawn Manager
*Arquivo: Assets/_Project/Scripts/Managers/SpawnManager.cs*

```csharp
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int initialPoolSize = 20;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float spawnRadius = 15f;
    
    private ObjectPool<GameObject> enemyPool;
    private float spawnTimer;
    private Transform playerTransform;
    
    private void Start()
    {
        // Criar object pool
        enemyPool = new ObjectPool<GameObject>(
            create: () => Instantiate(enemyPrefab),
            onGet: (obj) => obj.SetActive(true),
            onRelease: (obj) => obj.SetActive(false),
            initialSize: initialPoolSize
        );
        
        playerTransform = FindObjectOfType<LunaController>().transform;
    }
    
    private void Update()
    {
        spawnTimer += Time.deltaTime;
        
        if (spawnTimer >= spawnRate)
        {
            SpawnEnemy();
            spawnTimer = 0;
        }
    }
    
    private void SpawnEnemy()
    {
        Vector2 randomAngle = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = playerTransform.position + (Vector3)randomAngle * spawnRadius;
        
        GameObject enemy = enemyPool.Get();
        enemy.transform.position = spawnPos;
    }
}
```

---

# 5. ITEM SYSTEMS
**Tempo: 45-60 minutos**

## 5.1: Battery System
*Arquivo: Assets/_Project/Scripts/Items/BatterySystem.cs*

```csharp
using UnityEngine;

public class BatterySystem : MonoBehaviour
{
    [SerializeField] private float maxBattery = 100f;
    [SerializeField] private float drainRate = 5f; // per second
    [SerializeField] private float rechargeRate = 30f; // per pickup
    
    private float currentBattery;
    private bool isAlive = true;
    
    public float CurrentBattery => currentBattery;
    public float MaxBattery => maxBattery;
    public float BatteryPercent => currentBattery / maxBattery;
    
    private void Start()
    {
        currentBattery = maxBattery * 0.5f; // Comeca com 50%
    }
    
    private void Update()
    {
        if (!isAlive) return;
        
        currentBattery -= drainRate * Time.deltaTime;
        
        if (currentBattery <= 0)
        {
            currentBattery = 0;
            Die();
        }
    }
    
    public void Recharge()
    {
        currentBattery = Mathf.Min(currentBattery + rechargeRate, maxBattery);
    }
    
    private void Die()
    {
        isAlive = false;
        GameManager.Instance?.TriggerGameOver();
    }
}
```

## 5.2: Battery Pickup
*Arquivo: Assets/_Project/Scripts/Items/BatteryPickup.cs*

```csharp
using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [SerializeField] private float rechargeAmount = 30f;
    private bool isCollected = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            BatterySystem battery = collision.GetComponent<BatterySystem>();
            if (battery != null)
            {
                battery.Recharge();
                isCollected = true;
                gameObject.SetActive(false);
            }
        }
    }
}
```

## 5.3: Coin System
*Arquivo: Assets/_Project/Scripts/Items/CoinPickup.cs*

```csharp
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int coinValue = 10;
    private bool isCollected = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            ScoreManager.Instance?.AddCoins(coinValue);
            isCollected = true;
            gameObject.SetActive(false);
        }
    }
}
```

---

# 6. MANAGERS
**Tempo: 90-120 minutos**

## 6.1: GameManager (Main)
*Arquivo: Assets/_Project/Scripts/Managers/GameManager.cs*

```csharp
using UnityEngine;
using System;

public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public static event Action onGameOver;
    public static event Action<bool> onPauseChanged;
    public static event Action<bool> onMainMenuChanged;
    
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsInMainMenu { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            EnterMainMenuState();
        }
        else
        {
            StartGameplayState();
        }
    }
    
    public void TriggerGameOver()
    {
        IsGameOver = true;
        IsPaused = false;
        IsInMainMenu = false;
        Time.timeScale = 0f;
        onGameOver?.Invoke();
    }
    
    public void TogglePause()
    {
        SetPause(!IsPaused);
    }
    
    public void SetPause(bool value)
    {
        if (IsGameOver || IsInMainMenu) return;
        
        IsPaused = value;
        Time.timeScale = IsPaused ? 0f : 1f;
        onPauseChanged?.Invoke(IsPaused);
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        EnterMainMenuState();
    }
    
    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void EnterMainMenuState()
    {
        IsGameOver = false;
        IsInMainMenu = true;
        IsPaused = true;
        Time.timeScale = 1f;
        onMainMenuChanged?.Invoke(true);
    }
    
    private void StartGameplayState()
    {
        IsGameOver = false;
        IsInMainMenu = false;
        IsPaused = false;
        Time.timeScale = 1f;
        onMainMenuChanged?.Invoke(false);
    }
}
```

## 6.2: ScoreManager
*Arquivo: Assets/_Project/Scripts/Managers/ScoreManager.cs*

```csharp
using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    public static event Action<int> onCoinsChanged;
    public static event Action<int> onScoreChanged;
    
    private int currentScore;
    private int currentCoins;
    private int highScore;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        highScore = PlayerPrefs.GetInt("Highscore", 0);
        currentScore = 0;
        currentCoins = PlayerPrefs.GetInt("Coins", 0);
    }
    
    public void AddScore(int amount)
    {
        currentScore += amount;
        onScoreChanged?.Invoke(currentScore);
        
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("Highscore", highScore);
        }
    }
    
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        PlayerPrefs.SetInt("Coins", currentCoins);
        onCoinsChanged?.Invoke(currentCoins);
    }
    
    public bool TrySpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            PlayerPrefs.SetInt("Coins", currentCoins);
            onCoinsChanged?.Invoke(currentCoins);
            return true;
        }
        return false;
    }
    
    public int Score => currentScore;
    public int Coins => currentCoins;
    public int HighScore => highScore;
}
```

## 6.3: HealthSystem
*Arquivo: Assets/_Project/Scripts/Managers/HealthSystem.cs*

```csharp
using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    
    public event Action<float> onHealthChanged;
    
    private void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth);
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        onHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth);
    }
    
    private void Die()
    {
        GameManager.Instance?.TriggerGameOver();
    }
    
    public float HealthPercent => currentHealth / maxHealth;
}
```

## 6.4: AudioManager
*Arquivo: Assets/_Project/Scripts/Managers/AudioManager.cs*

```csharp
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        sfxSource.PlayOneShot(clip, volume);
    }
    
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
```

## 6.5: SettingsManager
*Arquivo: Assets/_Project/Scripts/Managers/SettingsManager.cs*

```csharp
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    
    public float MasterVolume { get; set; } = 1f;
    public bool VibrationEnabled { get; set; } = true;
    public bool TimerHUDEnabled { get; set; } = true;
    public bool PerformanceOverlayEnabled { get; set; } = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }
    
    private void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        VibrationEnabled = PlayerPrefs.GetInt("Vibration", 1) == 1;
        TimerHUDEnabled = PlayerPrefs.GetInt("TimerHUD", 1) == 1;
        PerformanceOverlayEnabled = PlayerPrefs.GetInt("PerfOverlay", 0) == 1;
    }
    
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetInt("Vibration", VibrationEnabled ? 1 : 0);
        PlayerPrefs.SetInt("TimerHUD", TimerHUDEnabled ? 1 : 0);
        PlayerPrefs.SetInt("PerfOverlay", PerformanceOverlayEnabled ? 1 : 0);
    }
}
```

---

# 7. UI/MENUS
**Tempo: 120-150 minutos**

## 7.1: Main Menu Implementation
*Ver MENU_IMPLEMENTATION_GUIDE.md completo*

**Checklist:**
- [ ] Canvas Main Menu criado (1920x1080)
- [ ] 6 botões com cores corretas
- [ ] Canvas Shop, Leaderboard, Daily Quests
- [ ] MainMenuController com callbacks
- [ ] Todos os botões conectados

## 7.2: HUD During Gameplay
```
HUD Canvas (Overlay)
├─ Top Left: Health, Score, Coins
├─ Top Right: Timer, Stage
├─ Bottom: Battery bar
└─ Pause Button
```

*Arquivo: Assets/_Project/Scripts/UI/HUDController.cs*

```csharp
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI batteryText;
    [SerializeField] private Slider batterySlider;
    
    private BatterySystem battery;
    private HealthSystem health;
    
    private void Start()
    {
        battery = FindObjectOfType<BatterySystem>();
        health = FindObjectOfType<HealthSystem>();
        
        if (battery != null)
            InvokeRepeating(nameof(UpdateBattery), 0, 0.1f);
        
        if (health != null)
            health.onHealthChanged += UpdateHealth;
        
        ScoreManager.Instance.onScoreChanged += UpdateScore;
        ScoreManager.Instance.onCoinsChanged += UpdateCoins;
    }
    
    private void UpdateBattery()
    {
        if (battery == null) return;
        batterySlider.value = battery.BatteryPercent;
        batteryText.text = $"BATTERY: {battery.CurrentBattery:F0}/{battery.MaxBattery:F0}";
    }
    
    private void UpdateHealth(float health)
    {
        healthText.text = $"HEALTH: {health:F0}";
    }
    
    private void UpdateScore(int score)
    {
        scoreText.text = $"SCORE: {score}";
    }
    
    private void UpdateCoins(int coins)
    {
        coinsText.text = $"COINS: {coins}";
    }
    
    private void OnDestroy()
    {
        if (health != null)
            health.onHealthChanged -= UpdateHealth;
    }
}
```

## 7.3: GameOver Screen
*Arquivo: Assets/_Project/Scripts/UI/GameOverPanelController.cs*

```csharp
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI newRecordText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    
    private void Start()
    {
        gameObject.SetActive(false);
        GameManager.onGameOver += ShowGameOver;
    }
    
    private void ShowGameOver()
    {
        gameObject.SetActive(true);
        
        int finalScore = ScoreManager.Instance.Score;
        int highScore = ScoreManager.Instance.HighScore;
        
        finalScoreText.text = $"FINAL SCORE: {finalScore}";
        newRecordText.text = finalScore >= highScore ? "✨ NEW RECORD! ✨" : "";
        
        restartButton.onClick.AddListener(() => GameManager.Instance.RestartScene());
        menuButton.onClick.AddListener(() => GameManager.Instance.ReturnToMainMenu());
    }
}
```

---

# 8. GAMEPLAY MECHANICS
**Tempo: 45-60 minutos**

## 8.1: Pause System
```csharp
// Adicionar em GameManager
public partial class GameManager
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }
}
```

## 8.2: Wave System (Opcional)
```csharp
public class WaveSystem : MonoBehaviour
{
    private int currentWave = 1;
    private float waveTimer;
    
    private void Update()
    {
        waveTimer += Time.deltaTime;
        
        // Difficulty increases every 30 seconds
        if (waveTimer >= 30)
        {
            currentWave++;
            waveTimer = 0;
            AdjustDifficulty();
        }
    }
    
    private void AdjustDifficulty()
    {
        SpawnManager spawn = FindObjectOfType<SpawnManager>();
        // Increase spawn rate, enemy speed, etc
    }
}
```

## 8.3: Combo System (Opcional)
```csharp
public class ComboSystem : MonoBehaviour
{
    private int currentCombo;
    private float comboTimer;
    private const float COMBO_TIMEOUT = 2f;
    
    private void Update()
    {
        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0)
            currentCombo = 0;
    }
    
    public void OnEnemyKilled()
    {
        currentCombo++;
        comboTimer = COMBO_TIMEOUT;
        // ScoreManager.AddScore(comboBonus);
    }
}
```

---

# 9. AUDIO & VFX
**Tempo: 30-45 minutos**

## 9.1: Audio Setup
1. Criar pastas: Assets/Audio/Music/, Assets/Audio/SFX/
2. Importar arquivos de áudio
3. Configurar AudioSources:
   - Music: volume 0.5, loop=true
   - SFX: volume 0.8, loop=false

## 9.2: VFX - Particle Systems
Create > Effects > Particle System:
- **Enemy Death:** Explosão amarela
- **Coin Pickup:** Efeito dourado
- **Battery Pickup:** Efeito azul
- **Damage Taken:** Flash vermelho

## 9.3: Screen Effects
```csharp
public class ScreenFlash : MonoBehaviour
{
    public void FlashRed(float duration = 0.1f)
    {
        StartCoroutine(FlashCoroutine(Color.red, duration));
    }
    
    private IEnumerator FlashCoroutine(Color color, float duration)
    {
        Image overlay = GetComponent<Image>();
        overlay.color = color;
        yield return new WaitForSeconds(duration);
        overlay.color = Color.clear;
    }
}
```

---

# 10. OPTIMIZATION
**Tempo: 30-45 minutos**

## 10.1: Object Pooling
*Arquivo: Assets/_Project/Scripts/Utilities/ObjectPool.cs*

```csharp
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : class
{
    private Queue<T> available = new Queue<T>();
    private HashSet<T> inUse = new HashSet<T>();
    
    private System.Func<T> create;
    private System.Action<T> onGet;
    private System.Action<T> onRelease;
    
    public ObjectPool(System.Func<T> create, System.Action<T> onGet, 
                      System.Action<T> onRelease, int initialSize)
    {
        this.create = create;
        this.onGet = onGet;
        this.onRelease = onRelease;
        
        for (int i = 0; i < initialSize; i++)
        {
            available.Enqueue(create());
        }
    }
    
    public T Get()
    {
        T obj = available.Count > 0 ? available.Dequeue() : create();
        inUse.Add(obj);
        onGet?.Invoke(obj);
        return obj;
    }
    
    public void Release(T obj)
    {
        if (inUse.Remove(obj))
        {
            onRelease?.Invoke(obj);
            available.Enqueue(obj);
        }
    }
}
```

## 10.2: Camera Culling
- Set Camera > Far Clip Plane: 100
- Enable GPU Instancing em materials

## 10.3: Physics Optimization
- Set Physics2D Check: "Always 2D"
- Reduce collision layers
- Use kinematic bodies quando possível

## 10.4: UI Optimization
- Use Canvas.renderMode = ScreenSpaceOverlay quando possível
- Disable raycast target em imagens não interativas
- Use object pooling para items dinâmicos

---

# 11. MOBILE BUILD
**Tempo: 30-40 minutos**

## 11.1: Build Settings
File > Build Settings:
- Add MainMenu, Gameplay cenas
- Platform: Android / iOS
- Resolution: 1920x1080
- Orientation: Portrait (or Landscape)

## 11.2: Android Setup
```
File > Build Settings > Player Settings:
├─ Company Name: YourCompany
├─ Product Name: GhostBeam
├─ Default Icon: (adicione)
├─ Package Name: com.yourcompany.ghostbeam
├─ Target API: API 31+
├─ Minimum API: API 24
└─ Graphics APIs: OpenGL ES 3.0
```

## 11.3: iOS Setup
```
File > Build Settings > Player Settings:
├─ Team ID: (sua Apple Team)
├─ Signing Certificate: (sua cert)
├─ Target Minimum iOS: 15.0
└─ Supported Device Orientations: Portrait
```

## 11.4: Build APK/IPA
```
File > Build Settings:
1. Select Platform (Android/iOS)
2. Click "Build" ou "Build and Run"
3. Salve em ./Builds/
4. Aguarde compilação (5-15 min)
```

---

# 12. TESTING & DEPLOYMENT
**Tempo: 60-90 minutos**

## 12.1: Functional Testing Checklist

### Main Menu
- [ ] Todos 6 botões funcionam
- [ ] Transições suaves entre telas
- [ ] Best Score mostra corretamente
- [ ] Config salva entre sessões

### Gameplay
- [ ] Player se move com input
- [ ] Inimigos spawnam e atacam
- [ ] Battery drena corretamente
- [ ] Coins spawnam e são coletados
- [ ] Score aumenta ao matar inimigos
- [ ] Game Over funciona
- [ ] Health bar se atualiza

### HUD
- [ ] Score mostra corretamente
- [ ] Health atualiza
- [ ] Battery bar visual
- [ ] Pause funciona (menu aparece)
- [ ] Unpause volta ao jogo

### Performance
- [ ] FPS estável em 60
- [ ] Sem lag durante gameplay
- [ ] Sem memory leaks
- [ ] Otimizado para mobile (menos de 200MB RAM)

## 12.2: QA Testing Report
```
Template:
┌─ BUILD: v1.0
├─ DEVICE: Mobile (Android/iOS)
├─ OS: Android 12.0 / iOS 15.0
├─ RESOLUTION: 1920x1080
├─ RAM USAGE: 120MB
├─ BATTERY: 2% por minuto
├─ BUGS FOUND:
│  ├─ [ ] Critical
│  ├─ [ ] Major
│  ├─ [ ] Minor
│  └─ [ ] TODO
└─ APPROVED: ✅
```

## 12.3: Release Checklist
- [ ] All 12 sections above tested
- [ ] No critical bugs
- [ ] Performance optimized
- [ ] Mobile-friendly input works
- [ ] Audio plays correctly
- [ ] Settings persist
- [ ] Game doesn't crash
- [ ] Build size < 300MB

## 12.4: Deploy to Stores

### Google Play Store
1. Create Google Play Developer Account
2. Prepare APK
3. Add screenshots, description
4. Submit for review (24-48h)

### Apple App Store
1. Create Apple Developer Account
2. Prepare IPA with TestFlight
3. Beta test minimum 24h
4. Submit for review (1-3 days)

---

# ⏱️ TIMELINE SUMMARY

| Fase | Componente | Tempo |
|------|-----------|-------|
| 1 | Project Setup | 20 min |
| 2 | Scene Structure | 15 min |
| 3 | Player | 90 min |
| 4 | Enemies | 90 min |
| 5 | Items | 60 min |
| 6 | Managers | 120 min |
| 7 | UI/Menus | 150 min |
| 8 | Gameplay | 60 min |
| 9 | Audio/VFX | 45 min |
| 10 | Optimization | 45 min |
| 11 | Mobile Build | 40 min |
| 12 | Testing | 90 min |
| **TOTAL** | | **825 min = 13-14 horas** |

---

# 🎯 PRIORITY ORDER FOR RAPID DEPLOYMENT

Se você tem pouco tempo, implemente nesta ordem:

1. **Project Setup** (20 min) ← MUST HAVE
2. **Player + Input** (90 min) ← MUST HAVE
3. **Enemies** (90 min) ← MUST HAVE
4. **Items** (60 min) ← MUST HAVE
5. **Managers** (120 min) ← MUST HAVE
6. **Basic Menu** (60 min) ← MUST HAVE
7. **HUD** (30 min) ← MUST HAVE
8. **Build & Test** (30 min) ← MUST HAVE

**Total: 500 min = 8 horas** (versão MÍNIMA viável)

---

# 📚 DIRECTORY REFERENCE

Todos os arquivos devem estar em:
```
Assets/_Project/Scripts/
├── Managers/
│   ├── GameManager.cs ✅
│   ├── ScoreManager.cs ✅
│   ├── AudioManager.cs ✅
│   ├── SettingsManager.cs ✅
│   └── HealthSystem.cs ✅
├── Player/
│   ├── PlayerInput.cs ✅
│   ├── LunaController.cs ✅
│   └── FlashlightController.cs ✅
├── Enemy/
│   ├── EnemyController.cs ✅
│   └── SpawnManager.cs ✅
├── Items/
│   ├── BatterySystem.cs ✅
│   ├── BatteryPickup.cs ✅
│   └── CoinPickup.cs ✅
├── UI/
│   ├── MainMenuController.cs ✅
│   ├── HUDController.cs ✅
│   ├── GameOverPanelController.cs ✅
│   └── ShopScreenController.cs ✅
└── Utilities/
    ├── ObjectPool.cs ✅
    └── ScreenFlash.cs ✅
```

---

# 🚀 NEXT STEPS

1. **Hoje (15 Abril):** Implementar Menu segundo MENU_IMPLEMENTATION_GUIDE.md
2. **Amanhã (16 Abril):** Player + Input system
3. **Dia 3 (17 Abril):** Enemies + Spawn system
4. **Dia 4 (18 Abril):** Items + Managers
5. **Dia 5 (19 Abril):** HUD + UI completion
6. **Dia 6 (20 Abril):** Optimization + Mobile Build
7. **Dia 7 (21 Abril):** Testing + Deployment

---

**BOA SORTE! 🎮✨**

*Este guia cobre 100% do desenvolvimento de GhostBeam v1.0*  
*Qualquer dúvida ao implementar, me avisa que corrijo!*
