# 🛠️ Ghost Beam - Technical Architecture

**Versão:** 1.1  
**Data de Última Atualização:** 14 de Abril de 2026  
**Status:** Framework Consolidado

---

## 1. Stack Técnico

### 1.1 Versões
| Componente | Versão | Status |
|------------|--------|--------|
| **Engine** | Unity 6 LTS | ✅ |
| **Render Pipeline** | Universal Render Pipeline (URP) 2D | ✅ |
| **Input System** | Input Manager (Clássico) | ✅ |
| **UI Framework** | TextMesh Pro (TMP) | ✅ |
| **Target Platform** | Android/iOS (Mobile Landscape Only) | ✅ |

### 1.2 Requisitos Mínimos

#### Android
- **API Mínima:** 21 (Android 5.0 Lollipop)
- **Orientação:** LANDSCAPE (obrigatório)
- **RAM Mínima:** 1 GB
- **Espaço em Disco:** 150 MB
- **Processador:** ARM v7
- **Tela:** 4.5" a 6.5"

#### iOS
- **Versão Mínima:** iOS 11.0
- **Orientação:** LANDSCAPE (obrigatório)
- **RAM Mínima:** 1 GB
- **Tela:** iPhone 6S+, iPad (landscape)
- **A-series:** A9 ou superior
- **Safe Area:** Handles notches (iPhone X+) e home indicators

#### Desenvolvimento (Editor Only)
- **SO:** Windows 7+ / macOS 10.12+ / Linux
- **RAM:** 4 GB
- **GPU:** Dedicada +1GB VRAM
- **Build:** Apenas para criar builds Android/iOS

---

## 2. Arquitetura de Cena

### 2.1 Hierarquia Padrão (Gameplay)

```
[Scene Root]
├─── Main Camera
│    └─── Canvas Scaler (responsivo)
│
├─── Global Light 2D
│    ├─ Intensity: 0 (cena escura)
│    └─ Mode: Additive
│
├─── Background Sprite
│    ├─ Sorting Order: -10
│    └─ Paralax (opcional)
│
├─── Gameplay Container
│    ├─ Luna (Personagem Principal)
│    │  ├─ Sprite Renderer
│    │  ├─ Circle Collider 2D (isTrigger: false)
│    │  ├─ Rigidbody 2D
│    │  │  ├─ Body Type: Dynamic
│    │  │  ├─ Gravity Scale: 0
│    │  │  ├─ Constraints: Freeze Rotation Z
│    │  │  └─ Collision Detection: Continuous
│    │  ├─ LunaController.cs
│    │  └─ Flashlight (Light 2D - Spot)
│    │     ├─ Light Type: Spot
│    │     ├─ Intensity: 1.0
│    │     ├─ Outer Radius: 15u
│    │     ├─ Outer Angle: 70°
│    │     ├─ Shadows: Enabled
│    │     ├─ Sorting Order: 0
│    │     └─ Rotation: acompanha input
│    │
│    ├─ Enemies Container (spawn aqui)
│    │  └─ [Dinâmico] Inimigos via Object Pool
│    │
│    └─ Items Container (pickups)
│       ├─ Battery Pickups (via BatteryPickupSpawner)
│       └─ Coin Pickups (via CoinPickupSpawner)
│
├─── Systems Container
│    ├─ GameManager (Singleton)
│    ├─ SpawnManager
│    ├─ ScoreManager (Singleton)
│    ├─ BatterySystem
│    ├─ BatteryPickupSpawner
│    ├─ CoinPickupSpawner
│    ├─ AudioManager (Singleton, DontDestroyOnLoad)
│    ├─ SettingsManager (Singleton, DontDestroyOnLoad)
│    ├─ UpgradeManager
│    ├─ SkinManager (DEBUG ONLY)
│    └─ UIBootstrapper
│
└─── UI Container
     └─ Canvas HUD
        ├─ Rendermode: Screen Space - Overlay
        ├─ TxtScore (top-left)
        ├─ TxtCoins (top-right)
        ├─ TxtHealth (bottom-left)
        ├─ TxtBattery (bottom-right)
        ├─ BatteryBar (slider visual)
        ├─ BtnPause (pause button)
        └─ PausePanel (inactive até pausar)
```

---

## 3. Sistemas Principais

### 3.1 GameManager
**Arquivo:** `Assets/_Project/Scripts/Managers/GameManager.cs`

**Responsabilidades:**
- Gerenciar estado global do jogo (Menu, Gameplay, GameOver, Pause)
- Broadcast de eventos (onGameOver, onPauseChanged, onMainMenuChanged)
- Scene management e transições
- Singleton padrão (DontDestroyOnLoad)

**Métodos Públicos:**
```csharp
public static void TriggerGameOver()        // Finaliza jogo
public static void TogglePause()            // Alterna pausa
public static void SetPause(bool value)     // Define pausa
public static void ReturnToMainMenu()       // Volta menu
public static void RestartScene()           // Reinicia cena
```

**Eventos:**
```csharp
public static event Action onGameOver;
public static event Action<bool> onPauseChanged;
public static event Action<bool> onMainMenuChanged;
```

---

### 3.2 SpawnManager
**Arquivo:** `Assets/_Project/Scripts/Managers/SpawnManager.cs`

**Responsabilidades:**
- Controlar spawn de inimigos
- Aplicar curva de dificuldade (stages)
- Usar Object Pooling para performance

**Parâmetros Principais:**
```csharp
[SerializeField] private GameObject enemyPrefab;
[SerializeField] private int poolSize = 30;
[SerializeField] private float spawnRate = 2.0f;
[SerializeField] private float spawnRadius = 15f;
[SerializeField] private int maxSimultaneous = 6;
```

**Lógica de Stage:**
```csharp
private void AdjustForStage()
{
    float gameTime = Time.timeSinceLevelLoad;
    
    if (gameTime < 35f)         // Stage 1
        spawnRate = 2.8f - (gameTime * 0.018f);
    else if (gameTime < 125f)   // Stage 2
        spawnRate = 1.4f - ((gameTime - 35) * 0.020f);
    else                         // Stage 3
        spawnRate = 0.95f - ((gameTime - 125) * 0.025f);
}
```

---

### 3.3 ScoreManager
**Arquivo:** `Assets/_Project/Scripts/Managers/ScoreManager.cs`

**Responsabilidades:**
- Rastrear pontos e moedas
- Persistência com PlayerPrefs
- Broadcast de eventos de mudança

**Métodos Públicos:**
```csharp
public void AddScore(int amount)
public void AddCoins(int amount)
public bool TrySpendCoins(int amount)
public int GetHighScore()
public int GetCoins()
```

**Persistência:**
```csharp
PlayerPrefs.SetInt("Highscore", score);
PlayerPrefs.SetInt("Coins", coins);
```

---

### 3.4 BatterySystem
**Arquivo:** `Assets/_Project/Scripts/Items/BatterySystem.cs`

**Responsabilidades:**
- Rastrear energia da lanterna
- Drenar quando Light está active
- Recarregar via pickup

**Valores:**
```csharp
private float maxBattery = 150f;
private float drainRate = 10f;      // pts/seg
private float rechargeAmount = 100f; // por pickup
```

**Lógica:**
```csharp
private void Update()
{
    if (isLighting)
        currentBattery -= drainRate * Time.deltaTime;
    
    if (currentBattery <= 0)
        OnBatteryDepleted();
}

public void Recharge()
{
    currentBattery = Mathf.Min(currentBattery + rechargeAmount, maxBattery);
}
```

---

### 3.5 BatteryPickupSpawner
**Arquivo:** `Assets/_Project/Scripts/Items/BatteryPickupSpawner.cs`

**Responsabilidades:**
- Spawnar pickups de bateria ao matar inimigo
- Aplicar multiplicadores por tipo de inimigo

**Multiplicadores:**
```csharp
private Dictionary<EnemyType, float> batteryMultiplier = new()
{
    { EnemyType.Penado, 1.5f },
    { EnemyType.Ictericia, 1.5f },
    { EnemyType.Ectogangue, 2.0f },
    { EnemyType.Tita, 2.5f },
    { EnemyType.Espectro, 2.0f }
};
```

---

### 3.6 CoinPickupSpawner
**Arquivo:** `Assets/_Project/Scripts/Items/CoinPickupSpawner.cs`

**Responsabilidades:**
- Spawnar pickups de moeda ao matar inimigo
- Aplicar multiplicadores por tipo

**Multiplicadores:**
```csharp
private Dictionary<EnemyType, float> coinMultiplier = new()
{
    { EnemyType.Penado, 1.0f },
    { EnemyType.Ictericia, 1.1f },
    { EnemyType.Ectogangue, 1.0f },
    { EnemyType.Tita, 1.5f },
    { EnemyType.Espectro, 1.2f }
};
```

---

### 3.7 AudioManager
**Arquivo:** `Assets/_Project/Scripts/Managers/AudioManager.cs`

**Responsabilidades:**
- Gerenciar música de fundo
- Reproduzir efeitos sonoros
- Controlar volume
- Singleton com DontDestroyOnLoad

**Métodos Públicos:**
```csharp
public void PlayMusic(AudioClip clip, bool loop = true)
public void PlaySFX(AudioClip clip, float volume = 1f)
public void SetMasterVolume(float volume)
public void SetSFXVolume(float volume)
```

---

### 3.8 SettingsManager
**Arquivo:** `Assets/_Project/Scripts/Managers/SettingsManager.cs`

**Responsabilidades:**
- Salvar preferências do jogador
- Aplicar configurações
- Singleton com DontDestroyOnLoad

**Configurações:**
```csharp
public float MasterVolume { get; set; } = 1f;
public bool VibrationEnabled { get; set; } = true;
public bool TimerHUDEnabled { get; set; } = true;
public bool PerformanceOverlayEnabled { get; set; } = false;
```

---

### 3.9 EnemyController
**Arquivo:** `Assets/_Project/Scripts/Enemy/EnemyController.cs`

**Responsabilidades:**
- Gerenciar movimento de inimigo
- Detectar iluminação (cálculo matemático)
- Lidar com morte
- Spawnar pickups e score

**Detecção de Luz (Fórmula Correta):**
```csharp
private bool IsIlluminated()
{
    Vector2 toEnemy = (Vector2)(transform.position - flashlight.position);
    float distance = toEnemy.magnitude;
    float angle = Vector2.Angle(flashlight.up, toEnemy);
    
    return distance < 15f && angle < 35f;
}
```

**Movimento:**
```csharp
private void Move()
{
    Vector2 direction = (playerPos - transform.position).normalized;
    transform.position = Vector2.MoveTowards(
        transform.position, 
        playerPos, 
        speed * Time.deltaTime
    );
}
```

---

### 4. Input System (Mobile Touch Native)

### 4.1 Configuração Mobile
**Plataforma:** Android/iOS Landscape ONLY

```
Virtual Joystick Esquerda (Movimento)
├─ Posição: Bottom-Left (safe area aware)
├─ Raio de Detecção: 100px
├─ Output: Vector2 (-1 a +1, -1 a +1)
└─ Latência: <50ms

Virtual Joystick Direita (Mira)
├─ Posição: Bottom-Right (safe area aware)
├─ Raio de Detecção: 100px
├─ Output: Ângulo (0-360°)
└─ Latência: <50ms
```

### 4.2 Leitura do Input (Touch Mobile)
```csharp
// Movimento (Joystick esquerdo)
if (Input.touchCount > 0)
{
    Touch touch = Input.GetTouch(0);
    Vector2 screenPos = touch.position;
    
    // Detectar joystick esquerdo OU direito
    if (screenPos.x < Screen.width / 2)  // Esquerda
        moveInput = GetJoystickInput(screenPos);
    else  // Direita
        aimInput = GetAimInput(screenPos);
}
```

### 4.3 Safe Area Handling (Landscape)
```csharp
RectTransform canvasRect = GetComponent<RectTransform>();
Rect safeArea = Screen.safeArea;

// Adjust joystick positions
leftJoystick.anchoredPosition = new Vector2(
    safeArea.xMin + padding,
    safeArea.yMin + padding
);

rightJoystick.anchoredPosition = new Vector2(
    -safeArea.xMax + padding,
    safeArea.yMin + padding
);
```

---

## 5. Lighting System (URP 2D)

### 5.1 Setup Correto

#### Global Light 2D
```
Type: Global
Intensity: 0 (cena escura)
Color: White (255, 255, 255)
Mode: Additive (aditivo, sem subtrair)
Normals: Disabled
```

#### Player Flashlight (Left Pivot)
```
Type: Spot
Intensity: 1.0
Outer Radius: 15 (alcance)
Inner Radius: 0
Outer Angle: 70 (abertura)
Color: White (255, 255, 255)
Mode: Additive
Shadows: Enabled (para sombras)
Sorting Order: 0
```

#### Enemies (sem light source, só recebem)
```
Sprite Renderer:
├─ Material: Default Sprite Material
└─ Receives Light: Enabled
```

### 5.2 Detecção de Luz (Não Usar Physics Collider!)
```csharp
// ✅ CORRETO: Cálculo matemático
private bool CheckLightIntersection()
{
    Vector2 toEnemy = (Vector2)(transform.position - flashlight.position);
    float distance = toEnemy.magnitude;
    float angle = Vector2.Angle(flashlight.up, toEnemy);
    
    return distance < 15f && angle < 35f;
}

// ❌ ERRADO: Usar collider com física
// Physics2D.OverlapCircle(...) - causa lag e bugs
```

---

## 6. Object Pooling

### 6.1 Pool Generic
**Arquivo:** `Assets/_Project/Scripts/Utilities/ObjectPool.cs`

```csharp
public class ObjectPool<T> where T : class
{
    private Queue<T> available = new();
    private HashSet<T> inUse = new();
    
    public ObjectPool(Func<T> create, Action<T> onGet, 
                      Action<T> onRelease, int initialSize)
    {
        for (int i = 0; i < initialSize; i++)
            available.Enqueue(create());
    }
    
    public T Get()
    {
        T obj = available.Count > 0 
            ? available.Dequeue() 
            : create();
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

### 6.2 Uso em SpawnManager
```csharp
private ObjectPool<GameObject> enemyPool;

private void Start()
{
    enemyPool = new ObjectPool<GameObject>(
        create: () => Instantiate(enemyPrefab),
        onGet: (obj) => obj.SetActive(true),
        onRelease: (obj) => obj.SetActive(false),
        initialSize: 30
    );
}

private void SpawnEnemy()
{
    GameObject enemy = enemyPool.Get();
    enemy.transform.position = spawnPos;
}

private void OnEnemyDeath(GameObject enemy)
{
    enemyPool.Release(enemy);
}
```

---

## 7. Physics Configuration

### 7.1 Rigidbody 2D (Luna apenas)
```
Body Type: Dynamic
Mass: 1
Gravity Scale: 0
Drag: 0
Angular Drag: 0
Constraints: Freeze Rotation Z
Collision Detection: Continuous
Sleeping Mode: Never Sleep
```

### 7.2 Colliders
```
Luna (Circle Collider)
├─ Radius: 0.5
├─ Offset: (0, 0)
└─ Is Trigger: false (detecta colisões físicas)

Enemies (Circle Trigger)
├─ Radius: 0.5
├─ Offset: (0, 0)
└─ Is Trigger: true (apenas trigger, não física)
```

### 7.3 Movimento Preferencial
```csharp
// ✅ CORRETO: MoveTowards
position = Vector2.MoveTowards(position, target, speed * dt);

// ⚠️ ACEITÁVEL: Rigidbody velocity
rigidbody.velocity = direction * speed;

// ❌ EVITAR: AddForce (causa comportamento imprevisível com luz)
rigidbody.AddForce(direction * force);
```

---

## 8. Padrões de Código

### 8.1 Naming Convention
```csharp
// Classes - PascalCase
public class EnemyController : MonoBehaviour { }

// Métodos - PascalCase
public void TakeDamage(float damage) { }

// Variáveis públicas - camelCase
public float speed = 5f;

// Variáveis privadas - _camelCase ou camelCase
private float _timer = 0f;
private int damageAmount = 10;

// Constantes - UPPER_SNAKE_CASE
private const float MAX_SPEED = 10f;
private const int DEFAULT_POOL_SIZE = 30;

// Eventos - OnXXX ou onXXX
public event Action onEnemyDeath;
public static event Action<int> onScoreChanged;
```

### 8.2 Estrutura Padrão de Script
```csharp
using UnityEngine;
using System;

// Namespace para organização
namespace GhostBeam.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        // 1. Serialized Fields (Inspector)
        [SerializeField] private float speed = 5f;
        
        // 2. Private Fields
        private Transform playerTransform;
        private Vector2 moveDirection;
        
        // 3. Properties
        public float Speed => speed;
        
        // 4. Events
        public event Action onDeath;
        
        // 5. Lifecycle
        private void Start()
        {
            playerTransform = FindObjectOfType<LunaController>().transform;
        }
        
        private void Update()
        {
            UpdateMovement();
        }
        
        // 6. Public Methods
        public void TakeDamage(float damage)
        {
            // Lógica
        }
        
        // 7. Private Methods
        private void UpdateMovement()
        {
            // Lógica
        }
    }
}
```

---

## 9. Regras Críticas (Core Rules)

### 9.1 Input System
✅ Usar Virtual Joysticks (native touch)
✅ Joystick esquerdo: Movimento
✅ Joystick direito: Mira/Rotação lanterna
✅ Safe area aware (notches, home indicators)
✅ Touch latência <50ms
✅ MOBILE LANDSCAPE ONLY
❌ NUNCA usar Input Manager clássico (WASD/mouse)
❌ NUNCA adicionar suporte a teclado/mouse
### 9.2 Lanterna
✅ Usar Light 2D tipo Spot  
✅ Ranio máximo: 15 unidades  
✅ Ângulo: 70 graus  
✅ Usar transform.up para direção  
❌ NUNCA usar Point Light ou Directional  

### 9.3 Iluminação Inimigos
✅ Usar cálculo matemático (distance + angle)  
❌ NUNCA usar Physics Collider para detectar luz  

### 9.4 Hierarquia Luna
✅ Luna com Flashlight como filha (transform local = 0,0,0)  
✅ Flashlight rotaciona mas não se move  
❌ NUNCA separar Luna de Flashlight  

### 9.5 Física dos Inimigos
✅ Usar Rigidbody2D kinematic OU sem rigidbody  
✅ Usar Vector2.MoveTowards para movimento  
❌ NUNCA usar AddForce ou AddVelocity nos inimigos  

---

## 10. Build & Deployment

### 10.1 Android Build
```
File > Build Settings:
├─ Platform: Android
├─ Player Settings:
│  ├─ Company Name: YourCompany
│  ├─ Product Name: GhostBeam
│  ├─ Package Name: com.yourcompany.ghostbeam
│  ├─ Target API: 31+
│  ├─ Minimum API: 21
│  └─ Graphics APIs: OpenGL ES 3.0
├─ Resolution: 1920x1080
└─ Build: APK ou AAB
```

### 10.2 iOS Build
```
File > Build Settings:
├─ Platform: iOS
├─ Player Settings:
│  ├─ Team ID: [sua Apple Team]
│  ├─ Signing Certificate: [sua cert]
│  ├─ Target Minimum iOS: 11.0
│  └─ Supported Device Orientations: Portrait/Landscape
├─ Resolution: 1920x1080
└─ Build: Xcode Project
```

---

## 11. Performance Optimization

### 11.1 Culling & Rendering
```
Camera:
├─ Far Clip Plane: 100
├─ Rendering Path: Forward
└─ HDR: Disabled (mobile não aguenta)

Materials:
├─ GPU Instancing: Enabled
└─ Shader: Mobile-Optimized
```

### 11.2 Physics
```
Edit > Project Settings > Physics2D:
├─ Gravity: (0, 0)
├─ Default Material: Friction 0, Bounce 0
├─ Solver Iterations: 2
└─ Body Sleep: Enabled
```

### 11.3 Memory Management
```csharp
// Usar Object Pool, não Instantiate/Destroy
private void Cleanup()
{
    // ✅ Correto
    enemyPool.Release(enemy);
    
    // ❌ Evitar
    Destroy(enemy);
}
```

---

## 12. Debugging Tips

### 12.1 Debug Draw Flashlight
```csharp
private void OnDrawGizmos()
{
    // Draw flashlight range
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, 15f);
    
    // Draw flashlight angle
    Vector3 direction = transform.up * 15f;
    Gizmos.DrawRay(transform.position, direction);
}
```

### 12.2 Debug Logs
```csharp
// Rastreie iluminação
if (IsIlluminated())
    Debug.Log($"Enemy {name} iluminado por {timer:F2}s");

// Rastreie spawn
Debug.Log($"[Spawn] Stage {currentStage}, rate {spawnRate:F2}");

// Rastreie bateria
Debug.Log($"[Battery] {current:F0}/{max:F0} ({percent:P})");
```

### 12.3 Profiler
```
Window > Analysis > Profiler
├─ CPU Usage: Target <20ms por frame (60 FPS)
├─ Memory: Target <150MB no mobile
├─ GPU: Target <16ms (60 FPS)
└─ Rendering: Draw calls < 50
```

---

**_Documento v1.1 - Aprovado para Desenvolvimento 🚀_**
