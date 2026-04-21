# 🎮 GUIA PASSO-A-PASSO: MONTANDO A HIERARQUIA

**Tempo: ~45 minutos**  
**Dificuldade: Iniciante**

---

## ✅ CHECKLIST RÁPIDO

- [ ] Scene Gameplay criada
- [ ] Camera configurada
- [ ] Global Light 2D criada
- [ ] Luna com Flashlight montada
- [ ] Systems (managers) criados
- [ ] Enemy prefab salvo
- [ ] BatteryPickup prefab salvo
- [ ] CoinPickup prefab salvo
- [ ] Canvas HUD criada com textos
- [ ] Referências linkadas no Inspector

---

## PASSO 1: CRIAR SCENE GAMEPLAY

### 1.1 Nova Scene
```
Ctrl + N (ou File > New Scene)
```

### 1.2 Salvar Scene
```
Ctrl + S → Assets/_Project/Scenes/Gameplay.unity
```

**Resultado esperado:**
```
Scene: Gameplay
Hierarchy:
├── Main Camera
├── Canvas (default)
└── (vazio)
```

### 1.3 Deletar Canvas padrão
```
Right-click Canvas → Delete
```

---

## PASSO 2: CONFIGURAR MAIN CAMERA

### 2.1 Selecionar Main Camera
```
Na Hierarchy, click em "Main Camera"
```

### 2.2 Propriedades (Inspector)
```
Transform:
  Position: X=0, Y=0, Z=-10
  Rotation: X=0, Y=0, Z=0
  Scale: X=1, Y=1, Z=1

Camera:
  Projection: Orthographic ✓
  Size: 7.5
  Near: 0.3
  Far: 1000
  
Background:
  Color: (0, 0, 0, 255) - Preto
```

---

## PASSO 3: CRIAR GLOBAL LIGHT 2D

### 3.1 Create Light 2D
```
Hierarchy > Right-click > 2D Object > Light > Global Light 2D
```

### 3.2 Rename
```
Rename para "Global Light 2D"
```

### 3.3 Propriedades
```
Light 2D:
  Light Type: Global
  Intensity: 0
  Blend Style: Default
```

**Resultado esperado:**
Scene escura (sem luz)

---

## PASSO 4: CRIAR BACKGROUND

### 4.1 Create Sprite
```
Hierarchy > Right-click > 2D Object > Sprite > Square
```

### 4.2 Setup
```
Name: Background
Transform:
  Position: X=0, Y=0, Z=0
  Scale: X=100, Y=100, Z=1

Sprite Renderer:
  Color: (30, 30, 50, 255) - Azul escuro
  Sorting Order: -10
```

---

## PASSO 5: MONTAR LUNA (PLAYER) ⭐

Este é o passo MAIS IMPORTANTE!

### 5.1 Create Empty
```
Hierarchy > Right-click > Create Empty
Name: Luna
Position: X=0, Y=0, Z=0
```

### 5.2 Add Sprite Renderer
```
Inspector > Add Component > Sprite Renderer
  Color: (200, 100, 255, 255) - Roxo
  Sprite: (leave empty, usará default)
```

### 5.3 Add Rigidbody 2D
```
Inspector > Add Component > Physics 2D > Rigidbody 2D

Propriedades:
  Body Type: Dynamic
  Mass: 1
  Gravity Scale: 0
  Drag: 0
  Angular Drag: 0
  
Constraints:
  ✓ Freeze Rotation Z
  
Collision Detection: Continuous
```

### 5.4 Add Circle Collider 2D
```
Inspector > Add Component > Physics 2D > Circle Collider 2D

Propriedades:
  Radius: 0.5
  Offset: X=0, Y=0
  Is Trigger: FALSE ✓
```

### 5.5 Add Tag "Player"
```
Inspector > Tag dropdown > (top) "Add Tag"
  New tag: "Player"

Depois volte e assign Tag: Player
```

### 5.6 Add Scripts
```
Inspector > Add Component > Health System (procure em Scripts)
Inspector > Add Component > Luna Controller
```

**Sua Luna agora pode se mover!**

---

## PASSO 6: CRIAR FLASHLIGHT (LUZ DA LANTERNA)

### 6.1 Create Light 2D (filha de Luna)
```
Luna > Right-click > 2D Object > Light > Spot Light 2D
Name: Flashlight
Position: X=0, Y=0, Z=0
```

### 6.2 Configurar Light 2D
```
Light 2D:
  Light Type: Spot
  Intensity: 1.0
  Outer Radius: 15
  Inner Radius: 0
  Outer Angle: 70
  
Color: White (255, 255, 255, 255)
Blend Style: Default
  
Shadows:
  ✓ Enabled
  
Sorting Order: 0
```

### 6.3 Add FlashlightController
```
Luna > Add Component > Flashlight Controller

Inspector:
  Flashlight: (Drag Flashlight aqui)
  Rotation Speed: 10
```

**Resultado esperado:**
- Luna tem um cone de luz branco
- Rotação suave com mouse

---

## PASSO 7: CRIAR SYSTEMS (MANAGERS)

### 7.1 Create Empty Container
```
Hierarchy > Right-click > Create Empty
Name: Systems
Position: X=0, Y=0, Z=0
```

### 7.2 Add Managers (como GameObjects filhos)

Para cada um abaixo:
```
Systems > Right-click > Create Empty
Name: [Manager Name]
Add Component > [Script correspondente]
```

**Criar estes:**
1. GameManager → Add Component > Game Manager
2. ScoreManager → Add Component > Score Manager
3. AudioManager → Add Component > Audio Manager
4. SettingsManager → Add Component > Settings Manager
5. BatterySystem → Add Component > Battery System
6. SpawnManager → Add Component > Spawn Manager
7. BatteryPickupSpawner → Add Component > Battery Pickup Spawner
8. CoinPickupSpawner → Add Component > Coin Pickup Spawner

**Resultado esperado:**
```
Systems/
├── GameManager
├── ScoreManager
├── AudioManager
├── SettingsManager
├── BatterySystem
├── SpawnManager
├── BatteryPickupSpawner
└── CoinPickupSpawner
```

---

## PASSO 8: CRIAR PREFABS

### 8.1 Enemy Prefab

#### 8.1.1 Create
```
Hierarchy > Right-click > 2D Object > Sprite > Circle
Name: Enemy
Position: X=5, Y=5, Z=0
Scale: X=0.8, Y=0.8, Z=1
```

#### 8.1.2 Setup
```
Sprite Renderer:
  Color: (100, 255, 100, 255) - Verde

Circle Collider 2D:
  Radius: 0.4
  Is Trigger: TRUE ✓

Add Component > Enemy Controller
  Speed: 3
  Time To Kill: 3
  Damage On Contact: 1
```

#### 8.1.3 Save as Prefab
```
Drag "Enemy" para Assets/_Project/Prefabs/Enemy.prefab
Delete Enemy da scene
```

### 8.2 BatteryPickup Prefab

#### 8.2.1 Create
```
Hierarchy > Right-click > 2D Object > Sprite > Circle
Name: BatteryPickup
Scale: X=0.4, Y=0.4, Z=1
```

#### 8.2.2 Setup
```
Sprite Renderer:
  Color: (255, 255, 0, 255) - Amarelo

Circle Collider 2D:
  Radius: 0.2
  Is Trigger: TRUE ✓

Add Component > Battery Pickup
  Recharge Amount: 50
  Attraction Distance: 3
```

#### 8.2.3 Save
```
Drag para Assets/_Project/Prefabs/BatteryPickup.prefab
Delete
```

### 8.3 CoinPickup Prefab

#### 8.3.1 Create
```
Hierarchy > Right-click > 2D Object > Sprite > Circle
Name: CoinPickup
Scale: X=0.25, Y=0.25, Z=1
```

#### 8.3.2 Setup
```
Sprite Renderer:
  Color: (255, 200, 0, 255) - Laranja

Circle Collider 2D:
  Radius: 0.15
  Is Trigger: TRUE ✓

Add Component > Coin Pickup
  Coin Amount: 1
  Attraction Distance: 3
```

#### 8.3.3 Save
```
Drag para Assets/_Project/Prefabs/CoinPickup.prefab
Delete
```

---

## PASSO 9: LINKAR SPAWN MANAGERS

### 9.1 SpawnManager
```
Select: Systems > SpawnManager

Inspector:
  Enemy Prefab: Drag Assets/_Project/Prefabs/Enemy.prefab
  Pool Size: 30
  Max Simultaneous: 6
  Spawn Radius: 15
```

### 9.2 BatteryPickupSpawner
```
Select: Systems > BatteryPickupSpawner

Inspector:
  Battery Pickup Prefab: Drag Assets/_Project/Prefabs/BatteryPickup.prefab
  Pool Size: 20
```

### 9.3 CoinPickupSpawner
```
Select: Systems > CoinPickupSpawner

Inspector:
  Coin Pickup Prefab: Drag Assets/_Project/Prefabs/CoinPickup.prefab
  Pool Size: 30
```

---

## PASSO 10: CRIAR CANVAS HUD

### 10.1 Create Canvas
```
Hierarchy > Right-click > UI (TextMeshPro) > Canvas
Name: CanvasHUD
```

### 10.2 Canvas Scaler
```
Select CanvasHUD
Inspector > Canvas Scaler:
  Ui Scale Mode: Scale With Screen Size
  Reference Resolution: X=1920, Y=1080
```

### 10.3 Create HUD Texts

Para cada um, create:
```
CanvasHUD > Right-click > UI (TextMeshPro) > Text - TextMeshPro
```

**Criar estes:**

#### TxtHealth
```
Name: TxtHealth
Position: (-900, 500)
Size: (300, 100)
Text: "❤️ 3"
Font Size: 36
Color: White
Alignment: Center
```

#### TxtBattery
```
Name: TxtBattery
Position: (-900, -500)
Text: "🔋 100%"
(mesmas configs acima)
```

#### TxtScore
```
Name: TxtScore
Position: (0, 500)
Text: "Score: 0"
```

#### TxtCoins
```
Name: TxtCoins
Position: (900, 500)
Text: "💰 0"
```

#### TxtHighScore
```
Name: TxtHighScore
Position: (0, 0)
Text: "Best: 0"
```

#### TxtTime
```
Name: TxtTime
Position: (0, -500)
Text: "00:00"
```

### 10.4 Create Pause Button
```
CanvasHUD > Right-click > UI (TextMeshPro) > Button - TextMeshPro
Name: BtnPause
Position: (850, -520) - Top Right
Size: (80, 80)

Image: Color = (50, 50, 50, 200) - Cinza
Text: "⏸" (Emoji pause)
```

---

## PASSO 11: LINKAR HUDController

### 11.1 Add HUDController
```
Select CanvasHUD
Add Component > HUD Controller
```

### 11.2 Assign Texts
```
Inspector > HUD Controller:
  Txt Health: Drag TxtHealth
  Txt Battery: Drag TxtBattery
  Txt Score: Drag TxtScore
  Txt Coins: Drag TxtCoins
  Txt HighScore: Drag TxtHighScore
  Txt Time: Drag TxtTime
  Pause Button: Drag BtnPause
```

---

## PASSO 12: CRIAR GAME OVER PANEL

### 12.1 Create Panel
```
CanvasHUD > Right-click > UI (TextMeshPro) > Panel
Name: PanelGameOver
```

### 12.2 Setup Panel
```
Anchor: Center
Position: (0, 0)
Size: (400, 300)
Image Color: (0, 0, 0, 200) - Preto transparente
```

### 12.3 Add Texts
```
PanelGameOver > Create 3 TextMeshPro texts:
  - TxtScoreFinal ("Score: 0")
  - TxtHighScoreFinal ("Best: 0")
  - TxtCoinsFinal ("Moedas: 0")
```

### 12.4 Add Buttons
```
PanelGameOver > Create 2 TextMeshPro buttons:
  - BtnRestart ("REINICIAR")
  - BtnMenu ("MENU")
```

### 12.5 Add GameOverPanelController
```
Select CanvasHUD
Add Component > Game Over Panel Controller

Assign:
  Game Over Canvas: CanvasHUD
  Txt Score Final: TxtScoreFinal
  Txt HighScore Final: TxtHighScoreFinal
  Txt Coins Final: TxtCoinsFinal
  Btn Restart: BtnRestart
  Btn Menu: BtnMenu
```

---

## PASSO 13: TESTAR! 🎮

### 13.1 Play Button
```
Press PLAY ▶️ 
```

### 13.2 Verificar
```
✓ Luna aparece no centro
✓ Flashlight ilumina (raio de luz)
✓ Teclado W/A/S/D move Luna
✓ Mouse rotaciona flashlight
✓ Inimigos spawnam (devem aparecer)
✓ HUD mostra vida/bateria/score
```

### 13.3 Debug Commands
```
W/A/S/D: Mover
Mouse: Mirar
ESC: Pausar
```

---

## ✅ PRONTO!

Se tudo funcionar, sua scene Gameplay está **100% completa**!

Próximo passo: Criar MainMenu scene (mais simples)

---

## 🆘 TROUBLESHOOTING

**Problema: "Luna não aparece"**
- Verify: Main Camera está na scene
- Check: Luna tem Sprite Renderer com color não transparente

**Problema: "Luz não funciona"**
- Check: Global Light 2D tem Intensity = 0
- Check: Flashlight (Light2D) está em Luna

**Problema: "Inimigos não spawnam"**
- Check: SpawnManager tem Enemy Prefab atribuído
- Check: Enemy tem EnemyController script

**Problema: "HUD não atualiza"**
- Check: HUDController tem todos os textos linkados
- Check: ScoreManager foi criado em Systems

---

**Parabéns! Seu projeto está montado! 🚀**
