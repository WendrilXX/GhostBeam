# Ghost Beam - Guia Técnico de Desenvolvimento

**Última atualização:** Abril 14, 2026  
**Status:** Framework Consolidado

---

## 1. Tecnologia e Ambiente

### 1.1 Stack Técnico
- **Engine**: Unity 6 (LTS)
- **Render Pipeline**: Universal Render Pipeline (URP) 2D
- **Input**: Input Manager (clássico)
- **UI**: TextMesh Pro (TMP)
- **Target**: Android/iOS (primary), PC (debug)

### 1.2 Requisitos Mínimos por Plataforma
- **Android**: API 21+ (Android 5.0), RAM 1GB
- **iOS**: iOS 11.0+, RAM 1GB
- **PC**: Windows 7+ ou Linux

---

## 2. Arquitetura de Cena

### 2.1 Hierarquia Padrão

```
Scene Root
├── Main Camera
│   └── Canvas Scaler (responsivo)
├── Global Light 2D (intensity 0)
├── Background (sprite)
└── Gameplay
    ├── Luna (personagem)
    │   └── Flashlight (Light 2D Spot)
    ├── Enemies (container para pool)
    └── Systems
        ├── GameManager
        ├── SpawnManager
        ├── ScoreManager
        ├── BatterySystem
        ├── BatteryPickupSpawner
        ├── CoinPickupSpawner
        ├── AudioManager
        ├── UpgradeManager
        ├── SkinManager
        ├── SettingsManager
        └── UIBootstrapper
├── CanvasHUD
│   ├── TxtVida
│   ├── TxtBateria
│   ├── TxtScore
│   ├── TxtRecorde
│   ├── TxtTempo
│   └── TxtFase
└── CanvasGameOver
    ├── PanelGameOver
    │   ├── TxtScoreFinal
    │   ├── TxtRecordeFinal
    └── BtnReiniciar
```

### 2.2 Convenções de Naming

| Tipo | Formato | Exemplo |
|------|---------|---------|
| GameObjects | PascalCase | `Luna`, `Flashlight`, `SpawnManager` |
| Prefabs | PascalCase + suffix | `Enemy.prefab`, `CoinPickup.prefab` |
| Scripts | PascalCase.cs | `LunaController.cs` |
| Variables | camelCase | `currentBattery`, `spawnTimer` |
| Constants | SCREAMING_SNAKE_CASE | `MAX_ENEMIES`, `SPAWN_INTERVAL` |

---

## 3. Principais Sistemas

### 3.1 GameManager (Ciclo de Jogo)

**Responsabilidades:**
- Estado do jogo (Menu, Gameplay, GameOver)
- Pausa/unpause (Time.timeScale)
- Triggeração de game over
- Reset de fase
- Transição entre telas

**Scripts:**
- `GameManager.cs`
- `GameManager.SceneFlow.cs` (abstração de cenas)
- `GameManager.State.cs` (state machine)

### 3.2 SpawnManager (Aumento de Dificuldade)

**Features:**
- Spawn progressivo de inimigos por estágio
- Adaptive performance (FPS monitoramento)
- Object pooling de inimigos
- Difficulty stages (Presentation, Test, Climax)

**Configs atuais:**
- Stage 1 (0-35s): 6 max inimigos, 1.6-2.8s spawn
- Stage 2 (35-125s): 12 max inimigos, 1.0-1.4s spawn
- Stage 3 (125s+): 18 max inimigos, 0.75-0.95s spawn

### 3.3 BatterySystem (Recurso Vital)

**Lógica:**
- Drena 10 pts/seg quando iluminando
- Sem recarga automática (apenas pickups)
- Blackout após atingir 0 (3s de espera)
- Game over se permanecer zerado

**Listeners:**
- `onBatteryChanged`: Notifica HUD e UI
- `RestoreBattery(amount)`: Chamado por pickups

### 3.4 ScoreManager (Progressão)

**Contadores:**
- Score: +5 pts/seg + 15 pts/kill
- Coins: 100% ao matar (com multiplicador)
- Highscore: Persisted em PlayerPrefs
- Survival time: Segundos em sessão

**Salvamento:**
- PlayerPrefs keys: `Highscore`, `Coins`, `Upgrade_*`, `Skin_*`

### 3.5 UpgradeManager (Progressão Econômica)

**Upgrades:**
- Beam (ângulo + alcance)
- Power (reduz tempo de kill)
- Battery (aumenta capacidade)

**Persistência:**
- PlayerPrefs: `Upgrade_BeamLevel`, `Upgrade_PowerLevel`, etc
- Max 5 níveis cada

### 3.6 Object Pooling

**Pools implementados:**
- `EnemyController`: Pool de inimigos por archetype
- `BatteryPickup`: Pool reutilizável
- `CoinPickup`: Pool reutilizável

**Padrão:**
```csharp
PooledObject component com `ReturnToPool()` 
ObjectPool gerencia Spawn() e despawn automático
```

---

## 4. Integração com Mobile

### 4.1 Controles Touch

**Sistema:**
- Joystick esquerdo: Movimento (Luna)
- Arraste direito: Mira (Flashlight)
- Fallback: Tap para input único

**Scripts:**
- `FlashlightController.cs`: Aiming logic
- `LunaController.cs`: Movement + touch handling

### 4.2 UI Responsividade

**Implementação:**
- Canvas Scaler com DPI matching
- Anchor presets padronizados
- TMP texto escalável

**Safe Area:**
- `SafeAreaFitter.cs` aplica notch handling
- Funciona em iOS (notch) e Android (navigation bar)

---

## 5. Processo de Build

### 5.1 Validação Pré-Build
- [ ] Cena principal aberta e testada
- [ ] PlayerPrefs foi resetado (ou não)
- [ ] All scenes incluídas em Build Settings
- [ ] Sprites empacotados corretamente
- [ ] Console sem erros visíveis

### 5.2 Build Android
1. File > Build Settings
2. Select Platform: Android
3. Target API Level: 31+
4. Build (ou Build and Run se device conectado)
5. Arquivo: `.apk` ou `.aab` (bundle)

### 5.3 Build iOS
1. File > Build Settings
2. Select Platform: iOS
3. Open generated Xcode project
4. Configurar provisioning profiles
5. Build na Xcode

### 5.4 Build PC (Debug)
1. File > Build Settings
2. Select Platform: Windows/Linux/Mac
3. Development Build (checkbox)
4. Scripts Debugging (checkbox)
5. Build

---

## 6. Debugging e Profiling

### 6.1 Performance Overlay
- Toggle em SettingsManager
- Mostra FPS, mem, bateria (simulada)
- Acessível no menu de settings

### 6.2 Profiler
- Unity > Window > Analysis > Profiler
- Memory: Acompanhar pool vs heap
- CPU: Identificar bottlenecks
- GPU: Validar draw calls

### 6.3 Common Issues

| Problema | Causa Comum | Solução |
|----------|---|---|
| Inimigos não morrem | Kill time muito alto | Verificar UpgradeManager |
| Lag ao aparecer inimigo | Pool não pré-aquecido | Aumentar `enemyPoolPrewarm` |
| Som distorcido | Master volume 1.0 | Reduzir em SettingsManager |
| Tela em branco | Canvas order errado | Verificar sorting order |

---

## 7. Checklist de Implementação Recente

- [x] PNG Penado 6 frames (512x512)
- [x] Animator automático para Penado
- [x] Remoção de skins da loja
- [x] Bateria 100% drop ao matar
- [x] Rebalanceamento de dificuldade
- [x] Score aumentado (5 pts/seg)
- [x] Upgrade costs reduzidos
- [x] Pickups bateria aumentados (1.5-2.5x)

---

## 8. Próximos Passos Sugeridos

1. **Testes em Device Real**: Validar touch + performance
2. **Balanceamento Fine-Tuning**: Sessões de gameplay feedback
3. **Conteúdo Adicional**: Mais skins/inimigos/power-ups
4. **Analytics**: Integrar tracker de sessão/playtime
5. **Polimento**: SFX/VFX adicionais, juice geral
6. **Otimização**: Memory profiling, draw calls reduction

---

## 9. Referências Rápidas

### Arquivos críticos:
- `GameManager.cs` - Estado global
- `SpawnManager.cs` - Dificuldade dinâmica
- `LunaController.cs` - Input e movimento
- `FlashlightController.cs` - Mira
- `EnemyController.cs` - IA inimigo
- `BatterySystem.cs` - Recurso vital

### PlayerPrefs Keys:
- `Highscore` → int
- `Coins` → int
- `Upgrade_*Level` → int (0-5)
- `Skin_*Unlocked` → bool
- `Skin_*Equipped` → bool
- `Settings_*` → vários tipos

---

**Para dúvidas técnicas, consulte este documento + comentários nos scripts respectivos.**
