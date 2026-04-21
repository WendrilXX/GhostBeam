# 🎮 GhostBeam - Guia de Setup Rápido

**Versão:** v1.0  
**Data:** 20 de Abril de 2026  
**Tempo Estimado:** 30-45 minutos

---

## 📋 Pré-Requisitos
- Unity 6 LTS com URP 2D
- TextMeshPro configurado (Window > TextMeshPro > Import TMP Essential Resources)
- Projeto aberto e compilado sem erros

---

## 1️⃣ CRIAR SCENE "GAMEPLAY"

### 1.1 Nova Scene
- `Ctrl+N` → Create > Scene
- File > Save Scene → `Assets/_Project/Scenes/Gameplay.unity`

### 1.2 Hierarquia Base
```
Gameplay (Scene Root)
├── Main Camera
├── Global Light 2D
├── Background
├── Systems
│   ├── GameManager
│   ├── ScoreManager
│   ├── SpawnManager
│   ├── BatteryPickupSpawner
│   ├── CoinPickupSpawner
│   ├── AudioManager
│   └── SettingsManager
├── Gameplay
│   ├── Luna (Player)
│   │   ├── Sprite Renderer
│   │   ├── Circle Collider 2D
│   │   ├── Rigidbody 2D
│   │   ├── LunaController.cs
│   │   └── Flashlight (Light 2D)
│   ├── Enemies
│   └── Items
└── UI
    └── Canvas HUD
```

### 1.3 Configurar Main Camera
1. **Selecionar** Main Camera
2. **Propriedades:**
   - Position: (0, 0, -10)
   - Orthographic: TRUE
   - Orthographic Size: 7.5
   - Background: (0, 0, 0, 1) - Preto

### 1.4 Global Light 2D
1. **Create > 2D Object > Light > Global Light 2D**
2. **Propriedades:**
   - Intensity: 0 (cena escura)
   - Color: White
   - Mode: Additive
   - Normals: Disabled

### 1.5 Background
1. **Create > 2D Object > Sprite > Square**
2. **Propriedades:**
   - Scale: (100, 100, 1)
   - Color: (30, 30, 50, 255) - Azul escuro
   - Sorting Order: -10

### 1.6 Systems (Empty GameObject)
1. **Create Empty** → Rename to "Systems"
2. **Add Scripts:**
   - GameManager.cs
   - ScoreManager.cs
   - SpawnManager.cs
   - BatteryPickupSpawner.cs
   - CoinPickupSpawner.cs
   - AudioManager.cs
   - SettingsManager.cs

---

## 2️⃣ CRIAR LUNA (PLAYER)

### 2.1 GameObject
1. **Create > 2D Object > Sprite > Circle**
2. **Rename:** Luna
3. **Position:** (0, 0, 0)
4. **Scale:** (1, 1, 1)
5. **Sprite Renderer:**
   - Color: (200, 100, 255, 255) - Roxo

### 2.2 Physics (Rigidbody 2D)
1. **Add Component > Rigidbody 2D**
2. **Propriedades:**
   - Body Type: Dynamic
   - Mass: 1
   - Gravity Scale: 0
   - Drag: 0
   - Angular Drag: 0
   - Constraints: Freeze Rotation Z
   - Collision Detection: Continuous

### 2.3 Collider (Circle)
1. **Add Component > Circle Collider 2D**
2. **Propriedades:**
   - Radius: 0.5
   - Offset: (0, 0)
   - Is Trigger: FALSE

### 2.4 Health System
1. **Add Component > Health System**
2. **Tag:** Add tag "Player"

### 2.5 LunaController
1. **Add Component > Luna Controller**
2. **Propriedades:**
   - Move Speed: 8

### 2.6 Flashlight (Light 2D)
1. **Luna > Create > Light > Spot Light 2D**
2. **Propriedades:**
   - Position: (0, 0, 0)
   - Light Type: Spot
   - Intensity: 1.0
   - Outer Radius: 15
   - Inner Radius: 0
   - Outer Angle: 70
   - Color: White
   - Mode: Additive
   - Shadows: Enabled
   - Sorting Order: 0

### 2.7 FlashlightController
1. **Luna > Add Component > Flashlight Controller**
2. **Assign Flashlight** (Light2D que criou acima)
3. **Rotation Speed:** 10

---

## 3️⃣ CRIAR ENEMY (PREFAB)

### 3.1 GameObject
1. **Create > 2D Object > Sprite > Circle**
2. **Rename:** Enemy
3. **Position:** (5, 5, 0)
4. **Scale:** (0.8, 0.8, 1)
5. **Color:** (100, 255, 100, 255) - Verde

### 3.2 Components
1. **Add > Circle Collider 2D**
   - Radius: 0.4
   - Is Trigger: TRUE
2. **Add > Enemy Controller**
   - Speed: 3
   - Time To Kill: 3
   - Damage On Contact: 1

### 3.3 Save as Prefab
1. **Drag Enemy para** `Assets/_Project/Prefabs/Enemy.prefab`
2. **Delete Enemy** da scene

---

## 4️⃣ CRIAR BATTERY PICKUP (PREFAB)

### 4.1 GameObject
1. **Create > 2D Object > Sprite > Circle**
2. **Rename:** BatteryPickup
3. **Scale:** (0.4, 0.4, 1)
4. **Color:** (255, 255, 0, 255) - Amarelo

### 4.2 Components
1. **Add > Circle Collider 2D**
   - Radius: 0.2
   - Is Trigger: TRUE
2. **Add > Battery Pickup**
   - Recharge Amount: 50
   - Attraction Distance: 3

### 4.3 Save as Prefab
1. **Drag para** `Assets/_Project/Prefabs/BatteryPickup.prefab`
2. **Delete** da scene

---

## 5️⃣ CRIAR COIN PICKUP (PREFAB)

### 5.1 GameObject
1. **Create > 2D Object > Sprite > Circle**
2. **Rename:** CoinPickup
3. **Scale:** (0.25, 0.25, 1)
4. **Color:** (255, 200, 0, 255) - Laranja

### 5.2 Components
1. **Add > Circle Collider 2D**
   - Radius: 0.15
   - Is Trigger: TRUE
2. **Add > Coin Pickup**
   - Coin Amount: 1
   - Attraction Distance: 3

### 5.3 Save as Prefab
1. **Drag para** `Assets/_Project/Prefabs/CoinPickup.prefab`
2. **Delete** da scene

---

## 6️⃣ CONFIGURAR GAMEPLAY SCENE

### 6.1 Containers
1. **Create Empty > Rename:** Enemies (filho de Gameplay)
2. **Create Empty > Rename:** Items (filho de Gameplay)

### 6.2 SpawnManager (Setup)
1. **Select Systems > SpawnManager component**
2. **Assign:**
   - Enemy Prefab: `Assets/_Project/Prefabs/Enemy.prefab`
   - Pool Size: 30
   - Max Simultaneous: 6

### 6.3 BatteryPickupSpawner (Setup)
1. **Select Systems > BatteryPickupSpawner**
2. **Assign:**
   - Battery Pickup Prefab: `Assets/_Project/Prefabs/BatteryPickup.prefab`

### 6.4 CoinPickupSpawner (Setup)
1. **Select Systems > CoinPickupSpawner**
2. **Assign:**
   - Coin Pickup Prefab: `Assets/_Project/Prefabs/CoinPickup.prefab`

---

## 7️⃣ CRIAR UI (HUD)

### 7.1 Canvas
1. **Create > Canvas**
2. **Rename:** CanvasHUD
3. **Canvas Scaler:**
   - Ui Scale Mode: Scale With Screen Size
   - Reference Resolution: (1920, 1080)

### 7.2 HUD Texts (TextMeshPro)
```
CanvasHUD
├── TxtHealth (top-left, "❤️ 3")
├── TxtBattery (bottom-left, "🔋 100%")
├── TxtScore (top-center, "Score: 0")
├── TxtCoins (top-right, "💰 0")
├── TxtHighScore (center, "Best: 0")
├── TxtTime (bottom-center, "00:00")
├── BatterySlider (battery bar)
└── BtnPause (button, pause)
```

### 7.3 Add HUDController
1. **Select CanvasHUD**
2. **Add Component > HUD Controller**
3. **Assign:**
   - Txt Health: Arrastar TxtHealth aqui
   - Txt Battery: Arrastar TxtBattery
   - Txt Score: Arrastar TxtScore
   - Txt Coins: Arrastar TxtCoins
   - Txt HighScore: Arrastar TxtHighScore
   - Txt Time: Arrastar TxtTime
   - Battery Slider: Arrastar Battery Slider
   - Pause Button: Arrastar BtnPause

---

## 8️⃣ CRIAR GAME OVER PANEL

### 8.1 Painel
1. **CanvasHUD > Create > Panel > Rename:** PanelGameOver
2. **Propriedades:**
   - Anchor: Center
   - Size: (400, 300)
   - Color: (0, 0, 0, 200) - Preto semi-transparente

### 8.2 Conteúdo
```
PanelGameOver
├── TxtScoreFinal ("Score: 0")
├── TxtHighScoreFinal ("Best: 0")
├── TxtCoinsFinal ("Moedas: 0")
├── BtnRestart ("REINICIAR")
└── BtnMenu ("MENU")
```

### 8.3 Add GameOverPanelController
1. **Select CanvasHUD**
2. **Add Component > Game Over Panel Controller**
3. **Assign:**
   - Game Over Canvas: CanvasHUD (ou PanelGameOver)
   - Txt Score Final: TxtScoreFinal
   - Txt HighScore Final: TxtHighScoreFinal
   - Txt Coins Final: TxtCoinsFinal
   - Btn Restart: BtnRestart
   - Btn Menu: BtnMenu

---

## 9️⃣ TESTAR GAMEPLAY

### 9.1 Play Button
1. **Press Play** ▶️
2. **Verificar:**
   - ✅ Luna aparece no centro
   - ✅ Flashlight ilumina (raio de luz)
   - ✅ Teclado/mouse controla (debug)
   - ✅ Inimigos spawnam
   - ✅ HUD atualiza

### 9.2 Debug (Keyboard)
- **W/A/S/D:** Mover
- **Mouse:** Mirar
- **Esc:** Pausar
- **Space:** (se implementado)

---

## 🔟 CRIAR SCENE "MAINMENU"

### 10.1 Nova Scene
- `Ctrl+N` → Save como `Assets/_Project/Scenes/MainMenu.unity`

### 10.2 Hierarquia
```
MainMenu
├── Systems (igual Gameplay)
├── Canvas Main Menu
│   ├── Background
│   ├── TxtTitulo ("GHOST BEAM")
│   ├── BtnPlay ("JOGAR")
│   ├── BtnSettings ("CONFIG")
│   └── BtnQuit ("SAIR")
```

### 10.3 Scripts
1. **Add UIBootstrapper** ao Canvas
2. **Add SceneBootstrapper** (vazio, sem loadGameplay)

---

## 📱 CONFIGURAR BUILD

### 11.1 Build Settings
- **File > Build Settings**
- **Scenes in Build:**
  1. Assets/_Project/Scenes/MainMenu.unity (Scene 0)
  2. Assets/_Project/Scenes/Gameplay.unity (Scene 1)

### 11.2 Player Settings
- **Edit > Project Settings > Player**
- **Android:**
  - Package Name: `com.yourcompany.ghostbeam`
  - Minimum API: 21
  - Target API: 31+
  - Orientation: Landscape
- **Resolution:** 1920 x 1080

---

## ✅ CHECKLIST FINAL

- [ ] Gameplay scene funciona
- [ ] Luna se move com teclado/mouse
- [ ] Inimigos spawnam e morrem
- [ ] HUD atualiza score/bateria
- [ ] Game over funciona
- [ ] MainMenu scene existe
- [ ] Build Settings com 2 scenes
- [ ] Tag "Player" criada
- [ ] Play funciona do Menu para Gameplay

---

**Pronto! Seu GhostBeam está funcional e pronto para produção! 🚀**

Se tiver problemas, verifique:
1. Namespaces estão corretos
2. Componentes estão linkados no Inspector
3. Tags estão criadas
4. Scenes foram salvas
