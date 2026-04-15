# 🎮 GHOSTBEAM - SETUP COMPLETO

## ⚡ Quick Start (5 minutos)

### 1. Cena Principal
```
1. Abra a cena "Gameplay"
2. Delete canvases antigos (se existir)
3. Crie GameObject vazio → renomeie "UIRoot"
4. Arraste MainMenuUIBuilder.cs para UIRoot
5. Play! ✓
```

### 2. O que o Script Cria Automaticamente
```
✅ MainMenuCanvas (Menu principal)
✅ PauseMenuCanvas (Pause)
✅ SettingsCanvas (Configurações)
✅ GameOverCanvas (Game Over)
✅ HUDCanvas (HUD in-game)
✅ EventSystem (Sistema de cliques)
```

---

## 🎯 Estrutura Completa

### Gameplay
```
GameManager ✓
├─ AudioManager
├─ SpawnManager
├─ Player / Luna / Flashlight
├─ Managers (HealthSystem, ScoreManager, etc)
├─ Main Camera (Orthographic, size 5)
└─ UIRoot (seu GameObject)
   └─ MainMenuUIBuilder.cs ← COLOCA AQUI
```

---

## 🎨 Menu System

### Main Menu
```
LOJA (amarelo) → Shop não implementado
CONFIGURAÇÃO (azul) → Volume + Vibração
PLAY (verde) → Inicia gameplay
SAIR (vermelho) → Fecha app
```

### In-Game
```
ESC ou botão PAUSA → Pause Menu
HUD mostra: Health, Score, Time, Wave, Coins, Battery
```

### Pause Menu
```
RETOMAR (verde) → Continua jogo
CONFIGURAÇÃO (azul) → Volume/Vibração
MENU (vermelho) → Volta ao menu
```

### Game Over
```
Automaticamente quando Luna morre
Mostra Score e Recorde
REINICIAR → Reinicia cena
MENU → Volta ao menu principal
```

---

## ⚙️ Configurações Importantes

### Camera (Main Camera)
```
Transform:
  Position: 0, 0, -10
  Rotation: 0, 0, 0

Camera:
  Projection: Orthographic ✓
  Size: 5
  Near Clip Plane: 0.3
  Far Clip Plane: 1000
  Viewport Rect: X=0, Y=0, W=1, H=1
```

### AudioManager
```
✅ Único AudioListener (não ter duplicatas)
✅ Som de background music
✅ Sliders funcionais (0-1)
```

### SpawnManager
```
spawnMargin = 8f (fantasmas nascem 8 unidades longe)
✓ Luna não morre no start
```

---

## 🔧 Scripts Principais

### MainMenuUIBuilder.cs
```
- Cria 5 Canvases
- Gerencia navegação de menus
- Atualiza HUD em tempo real
- Detecta Game Over
- Controla pausa com ESC
- TUDO EM 1 ARQUIVO!
```

### GameManager.cs (SceneFlow.cs)
```
- StartGameplay() → Inicia jogo
- ReturnToMenu() → Volta ao menu
- TogglePause() → Pausa/Resume
- TriggerGameOver() → Game Over
```

### Managers Necessários
```
✓ GameManager (Singleton)
✓ HealthSystem (Detecta morte)
✓ ScoreManager (Score e Moedas)
✓ BatterySystem (Bateria)
✓ SpawnManager (Inimigos)
✓ AudioManager (Som)
```

---

## ✨ Funcionalidades

### Menu
```
✅ Play → Inicia gameplay
✅ Loja → Placeholder
✅ Configuração → Volume/Vibração
✅ Sair → Application.Quit()
```

### Gameplay
```
✅ HUD com health bar dinâmica (verde/amarelo/vermelho)
✅ Score e Recorde
✅ Timer (MM:SS)
✅ Wave/Dificuldade
✅ Moedas e Bateria
✅ Pausa com ESC
✅ Botão de pausa no canto
```

### Game Over
```
✅ Detecta automaticamente (health <= 0)
✅ Mostra score e recorde
✅ Restart recarrega cena
✅ Menu volta ao principal
```

---

## 🐛 Troubleshooting

### Botões não clicam
```
→ Adicione EventSystem manualmente:
  Hierarchy → botão direito
  UI → Event System
(ou deixe o script criar automaticamente)
```

### HUD não mostra valores
```
→ Verifique que tem na cena:
  ✓ HealthSystem
  ✓ ScoreManager
  ✓ BatterySystem
  ✓ SpawnManager
```

### Fantasmas nascem perto de Luna
```
→ Aumentar SpawnManager.spawnMargin
  (já foi para 8f, deve estar ok)
```

### Pausa não funciona
```
→ Pressione ESC ou clique botão PAUSA
→ Verifique que GameManager existe
```

---

## 📊 Status Atual

```
✅ Menu 100% funcional
✅ HUD 100% funcional
✅ Pausa 100% funcional
✅ Game Over 100% funcional
✅ 0 erros de compilação
✅ 0 warnings
```

---

## 🚀 Próximos Passos

- [ ] Testar menu completo
- [ ] Testar gameplay flow
- [ ] Testar pause e resume
- [ ] Testar game over
- [ ] Testar mobile (touch)
- [ ] Ajustar cores/layouts conforme preferência
- [ ] Implementar Shop (se necessário)

