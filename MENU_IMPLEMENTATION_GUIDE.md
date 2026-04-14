# 🎮 GUIA PASSO A PASSO: IMPLEMENTAR MENU NA HIERARQUIA

**Data:** 15 de Abril de 2026  
**Tempo Estimado:** 60-80 minutos  
**Dificuldade:** ⭐⭐ Intermediária

---

## 📋 SUMÁRIO

- [FASE 1: Setup Inicial](#fase-1-setup-inicial)
- [FASE 2: Criar Canvas Main Menu](#fase-2-criar-canvas-main-menu)
- [FASE 3: Criar Canvas - Shop](#fase-3-criar-canvas---shop)
- [FASE 4: Criar Canvas - Leaderboard](#fase-4-criar-canvas---leaderboard)
- [FASE 5: Criar Canvas - Daily Quests](#fase-5-criar-canvas---daily-quests)
- [FASE 6: Criar Canvas - Settings](#fase-6-criar-canvas---settings)
- [FASE 7: Conectar Scripts e Callbacks](#fase-7-conectar-scripts-e-callbacks)
- [FASE 8: Testes Finais](#fase-8-testes-finais)

---

# FASE 1: SETUP INICIAL
**Tempo: 5 minutos**

## Passo 1.1: Abrir Cena
- Arquivo > Open Scene
- Selecione `Assets/Scenes/SampleScene.unity`
- Carregue a cena

## Passo 1.2: Verificar GameManager
Na Hierarquia, procure por `GameManager`:
- ✅ Se existe: Passe para Passo 1.3
- ❌ Se não existe: 
  - Right-click na Hierarquia > Create Empty
  - Renomeie para `GameManager`
  - No Inspector > Add Component > GameManager.cs

## Passo 1.3: Limpar GameObjects Quebrados
- Abra `Window > Analysis > BrokenScriptFinder` (ou procure na Hierarquia)
- Procure por ícones de erro (triangulo amarelo)
- **Delete** qualquer GameObject com erro
- Salve a cena (Ctrl+S)

---

# FASE 2: CRIAR CANVAS MAIN MENU
**Tempo: 20-25 minutos**

## Passo 2.1: Criar Canvas
- Right-click na Hierarquia > UI > Canvas
- Na Hierarquia, renomeie para `Canvas - Main Menu`

## Passo 2.2: Configurar Canvas
Selecione `Canvas - Main Menu` e no Inspector configure:

```
Canvas (Component)
  ├─ Render Mode: Screen Space - Overlay
  ├─ Sort Order: 100

Canvas Scaler (Add Component)
  ├─ UI Scale Mode: Scale With Screen Size
  ├─ Reference Resolution: 1920 x 1080

Graphic Raycaster: (já está)
```

## Passo 2.3: Adicionar Background
- Right-click em `Canvas - Main Menu` > Image
- Renomeie para `Background`
- No Inspector:
  ```
  Image Component
    ├─ Color: (20, 20, 30, 255) = #141E1E
  
  RectTransform
    ├─ Anchors: Preset "Stretch All"
    ├─ Left: 0, Right: 0, Top: 0, Bottom: 0
  ```

## Passo 2.4: Criar PanelMenu
- Right-click em `Canvas - Main Menu` > UI > Panel
- Renomeie para `PanelMenu`
- **IMPORTANTE**: Delete o Image do panel (deixe só o RectTransform)
- No Inspector:
  ```
  RectTransform
    ├─ Width: 1000
    ├─ Height: 700
    ├─ Center X: 0, Center Y: 0
  
  Add Component: Vertical Layout Group
    ├─ Child Force Expand Width: true
    ├─ Child Force Expand Height: false
    ├─ Spacing: 30
    ├─ Padding: Left 40, Right 40, Top 40, Bottom 40
  ```

## Passo 2.5: Adicionar Título
- Right-click em `PanelMenu` > TextMeshPro - Text
- Renomeie para `TxtTitulo`
- No Inspector:
  ```
  Text Component
    ├─ Text: "GHOST BEAM"
    ├─ Font Size: 100
    ├─ Alignment: Center
    ├─ Color: White (255, 255, 255)
  
  RectTransform
    ├─ Height: 150
  ```

## Passo 2.6: Criar Botão JOGAR
- Right-click em `PanelMenu` > Button - TextMeshPro
- Renomeie para `BtnJogar`
- Configure:
  ```
  Button Component
    ├─ Colors:
    │  ├─ Normal Color: (51, 255, 51, 255) = Verde
    │  ├─ Highlighted Color: (102, 255, 102, 255)
    │  ├─ Pressed Color: (0, 200, 0, 255)
    │  └─ Disabled Color: (100, 100, 100, 255)
  
  RectTransform
    ├─ Height: 130
  
  TextMeshPro - Text (dentro)
    ├─ Text: "JOGAR"
    ├─ Font Size: 110
  ```

## Passo 2.7: Criar Row 1 (LOJA, RANKING, DESAFIOS)
- Right-click em `PanelMenu` > Create Empty - UI
- Renomeie para `Row1`
- Configure:
  ```
  RectTransform
    ├─ Height: 100
  
  Add Component: Horizontal Layout Group
    ├─ Child Force Expand Width: true
    ├─ Child Force Expand Height: true
    ├─ Spacing: 30
  ```

### Botão LOJA em Row1
- Right-click em `Row1` > Button - TextMeshPro
- Renomeie para `BtnLoja`
- Colors Normal: (255, 166, 51, 255) = Laranja
- TextMeshPro: "LOJA", Font Size: 75

### Botão RANKING em Row1
- Right-click em `Row1` > Button - TextMeshPro
- Renomeie para `BtnRanking`
- Colors Normal: (51, 255, 255, 255) = Ciano
- TextMeshPro: "RANKING", Font Size: 75

### Botão DESAFIOS em Row1
- Right-click em `Row1` > Button - TextMeshPro
- Renomeie para `BtnDesafios`
- Colors Normal: (255, 51, 255, 255) = Magenta
- TextMeshPro: "DESAFIOS", Font Size: 75

## Passo 2.8: Criar Row 2 (CONFIG, SAIR)
- Right-click em `PanelMenu` > Create Empty - UI
- Renomeie para `Row2`
- Mesmo setup que Row1

### Botão CONFIG em Row2
- Right-click em `Row2` > Button - TextMeshPro
- Renomeie para `BtnConfiguracoes`
- Colors Normal: (255, 255, 51, 255) = Amarelo
- TextMeshPro: "CONFIG", Font Size: 75

### Botão SAIR em Row2
- Right-click em `Row2` > Button - TextMeshPro
- Renomeie para `BtnSair`
- Colors Normal: (255, 77, 77, 255) = Vermelho
- TextMeshPro: "SAIR", Font Size: 75

## Passo 2.9: Adicionar Best Score
- Right-click em `PanelMenu` > TextMeshPro - Text
- Renomeie para `TxtBestScore`
- Configure:
  ```
  Text: "BEST SCORE: 5420"
  Font Size: 32
  Alignment: Center
  Color: Yellow (255, 255, 0)
  
  RectTransform Height: 80
  ```

✅ **Canvas - Main Menu PRONTO!**

---

# FASE 3: CRIAR CANVAS - SHOP
**Tempo: 10-12 minutos**

## Passo 3.1: Criar Canvas
- Right-click na Hierarquia > UI > Canvas
- Renomeie para `Canvas - Shop`
- Configure similarmente ao Main Menu:
  ```
  Canvas > Sort Order: 160
  CanvasScaler > Reference Resolution: 1920 x 1080
  ```

## Passo 3.2: Criar PanelLoja
- Right-click em `Canvas - Shop` > UI > Panel
- Renomeie para `PanelLoja`
- Delete o Image
- Add Component: Vertical Layout Group (mesmo config que Panel Menu)

## Passo 3.3: Adicionar Elementos à Loja
Dentro de `PanelLoja`, crie:

### 3.3.1 Título
- TextMeshPro - Text: `TxtTituloLoja`
- Text: "LOJA"
- Font Size: 50
- Color: Orange (255, 166, 51)

### 3.3.2 Display de Moedas
- TextMeshPro - Text: `TxtMoedasLoja`
- Text: "MOEDAS: 0"
- Font Size: 36
- Color: Yellow (255, 255, 0)

### 3.3.3 Criar 3 Buttons de Item
Crie 3 botões dentro de `PanelLoja`:
- `BtnItem1`: "Item 1\n100 coins" | Color: Cyan (51, 204, 255)
- `BtnItem2`: "Item 2\n150 coins" | Color: Orange (255, 166, 51)
- `BtnItem3`: "Item 3\n200 coins" | Color: Magenta (200, 100, 255)

Cada com Height: 80

### 3.3.4 Botão Voltar
- Button - TextMeshPro: `BtnVoltarLoja`
- Text: "VOLTAR"
- Color: Red (255, 77, 77)
- Height: 80

✅ **Canvas - Shop PRONTO!**

---

# FASE 4: CRIAR CANVAS - LEADERBOARD
**Tempo: 10-12 minutos**

## Passo 4.1: Criar Canvas
- Right-click na Hierarquia > UI > Canvas
- Renomeie para `Canvas - Leaderboard`
- Sort Order: 155

## Passo 4.2: Criar PanelLeaderboard
- Similar ao Shop, com:
  - `TxtRanking`: "RANKING" (Font Size: 50, Cyan)
  - `TxtPlayerRank`: "SEU RANK: #1" (Font Size: 32, Yellow)

## Passo 4.3: Adicionar Scroll View para Ranking
- Right-click em `PanelLeaderboard` > UI > Scroll View (Vertical)
- Renomeie para `ScrollLeaderboard`
- Configure:
  ```
  RectTransform Height: 400
  Scroll Rect:
    ├─ Vertical: checked
    ├─ Horizontal: unchecked
  ```

### 4.3.1 Configurar Content
- Selecione o `Content` dentro do Scroll View
- RectTransform:
  ```
  Width: 700
  Height: 600 (ou suficiente)
  Layout Group: Vertical Layout Group
    ├─ Child Force Expand Width: true
    ├─ Child Force Expand Height: false
    ├─ Spacing: 10
  ```

### 4.3.2 Adicionar Items Mock
- Dentro de `Content`, crie 10 TextMeshPro - Text:
  - `RankItem1`: "#1 NinjaGhost    5420"
  - `RankItem2`: "#2 ShadowBlade   4890"
  - ... e assim por diante

Cada com:
- Font Size: 24
- Height: 50

## Passo 4.4: Botão Voltar
- Button - TextMeshPro: `BtnVoltarRanking`
- Text: "VOLTAR"
- Color: Red (255, 77, 77)

✅ **Canvas - Leaderboard PRONTO!**

---

# FASE 5: CRIAR CANVAS - DAILY QUESTS
**Tempo: 10-12 minutos**

## Passo 5.1: Criar Canvas
- Right-click na Hierarquia > UI > Canvas
- Renomeie para `Canvas - Daily Quests`
- Sort Order: 150

## Passo 5.2: Criar PanelDailyQuests
- Similar ao Leaderboard

### Elementos:
- `TxtDesafios`: "DESAFIOS" (Font Size: 50, Magenta)
- `TxtResetTime`: "RESET: 23:45" (Font Size: 28, Gray)

## Passo 5.3: Adicionar Scroll View para Quests
- Similar ao Leaderboard
- `ScrollQuests` com `Content`

### 5.3.1 Adicionar 3 Items de Quest
Dentro de `Content`, crie estruturas para cada quest:

**Quest 1:**
```
GameObject: Quest1
├─ TextMeshPro: "DESAFIO 1: Mate 10 Inimigos"
├─ Slider: Progress (value 5, max 10)
└─ TextMeshPro: "Recompensa: 100 moedas"
```

**Quest 2:** Similar
**Quest 3:** Similar

Cada item com Height: 120

## Passo 5.4: Botão Voltar
- Button - TextMeshPro: `BtnVoltarQuests`
- Text: "VOLTAR"
- Color: Red

✅ **Canvas - Daily Quests PRONTO!**

---

# FASE 6: CRIAR CANVAS - SETTINGS
**Tempo: 8-10 minutos**

## Passo 6.1: Criar Canvas
- Right-click na Hierarquia > UI > Canvas
- Renomeie para `Canvas - Settings`
- Sort Order: 165

## Passo 6.2: Criar PanelConfiguracoes
- Similar ao Shop

### Elementos:

**6.2.1 Título**
- TextMeshPro: `TxtConfiguracoes`
- Text: "CONFIGURAÇÕES"
- Font Size: 50

**6.2.2 Volume**
```
Create Empty: VolumeSetting
├─ TextMeshPro: "VOLUME"
└─ Slider: SliderVolume
   ├─ Min Value: 0
   ├─ Max Value: 1
   ├─ Value: 0.8
```

**6.2.3 Vibrações**
```
Button: BtnVibracao
├─ Text: "VIBRAÇÕES: ON"
├─ Color: Green or Red
```

**6.2.4 Timer HUD**
```
Button: BtnTimerHUD
├─ Text: "TIMER HUD: ON"
```

**6.2.5 Performance Overlay**
```
Button: BtnPerfHUD
├─ Text: "PERFORMANCE: OFF"
```

**6.2.6 Botão Voltar**
- Button: `BtnVoltarConfig`
- Text: "VOLTAR"
- Color: Red

✅ **Canvas - Settings PRONTO!**

---

# FASE 7: CONECTAR SCRIPTS E CALLBACKS
**Tempo: 15-20 minutos**

## Passo 7.1: Adicionar MainMenuController
- Selecione `Canvas - Main Menu` na Hierarquia
- Inspector > Add Component > MainMenuController

### Configurar Referências no Inspector
Arraste e solte (Drag & Drop) no Inspector:
```
Main Menu Controller (Script)
├─ Panel Root: Canvas - Main Menu > PanelMenu
├─ Shop Panel Root: Canvas - Shop > PanelLoja
├─ Settings Panel Root: Canvas - Settings > PanelConfiguracoes
```

## Passo 7.2: Adicionar ShopScreenController
- Selecione `Canvas - Shop`
- Add Component > ShopScreenController
- Drag & Drop:
  ```
  ├─ Shop Panel Root: PanelLoja
  ├─ Shop Coins Text: TxtMoedasLoja
  └─ Close Button: BtnVoltarLoja
  ```

## Passo 7.3: Adicionar LeaderboardScreenController
- Selecione `Canvas - Leaderboard`
- Add Component > LeaderboardScreenController
- Drag & Drop:
  ```
  ├─ Leaderboard Panel Root: PanelLeaderboard
  ├─ Player Rank Text: TxtPlayerRank
  ├─ Leaderboard Content: Content (do Scroll View)
  └─ Close Button: BtnVoltarRanking
  ```

## Passo 7.4: Adicionar DailyQuestsScreenController
- Selecione `Canvas - Daily Quests`
- Add Component > DailyQuestsScreenController
- Drag & Drop:
  ```
  ├─ Quests Panel Root: PanelDailyQuests
  ├─ Reset Time Text: TxtResetTime
  ├─ Quests Content: Content (do Scroll View)
  └─ Close Button: BtnVoltarQuests
  ```

---

# FASE 8: CONECTAR BUTTON CALLBACKS
**Tempo: 10-15 minutos**

## Passo 8.1: Botão JOGAR
- Selecione `BtnJogar`
- Inspector > Button > On Click (não tem listener)
- Clique no **+** para adicionar listener
- Drag & Drop: `Canvas - Main Menu` (que tem MainMenuController) para o campo
- Dropdown > MainMenuController > StartGame()

## Passo 8.2: Botão LOJA
- Selecione `BtnLoja`
- Button > On Click > +
- Drag `Canvas - Main Menu`
- MainMenuController > OpenShop()

## Passo 8.3: Botão RANKING
- Selecione `BtnRanking`
- Button > On Click > +
- Drag `Canvas - Main Menu`
- MainMenuController > OpenLeaderboard()

## Passo 8.4: Botão DESAFIOS
- Selecione `BtnDesafios`
- Button > On Click > +
- Drag `Canvas - Main Menu`
- MainMenuController > OpenDailyQuests()

## Passo 8.5: Botão CONFIGURAÇÕES
- Selecione `BtnConfiguracoes`
- Button > On Click > +
- Drag `Canvas - Main Menu`
- MainMenuController > OpenSettings()

## Passo 8.6: Botão SAIR
- Selecione `BtnSair`
- Button > On Click > +
- Drag `Canvas - Main Menu`
- MainMenuController > QuitGame()

## Passo 8.7: Botões VOLTAR das Sub-telas

### Voltar Loja
- Selecione `BtnVoltarLoja`
- Button > On Click > +
- Drag `Canvas - Main Menu`
- MainMenuController > CloseShop()

### Voltar Ranking
- Selecione `BtnVoltarRanking`
- Button > On Click > +
- Drag `Canvas - Main Menu`
- MainMenuController > CloseLeaderboard()

### Voltar Desafios
- Selecione `BtnVoltarQuests`
- Button > On Click > +
- Drag `Canvas - Main Menu`
- MainMenuController > CloseDailyQuests()

### Voltar Configurações
- Selecione `BtnVoltarConfig`
- Button > On Click > +
- Drag `Canvas - Main Menu`
- MainMenuController > CloseSettings()

---

# FASE 8: TESTES FINAIS
**Tempo: 5-10 minutos**

## Passo 8.1: Salvar Cena
- Ctrl+S para salvar

## Passo 8.2: Play Mode
- Pressione **Play** no Editor
- Verifique:

```
Checklist de Testes:

VISUAL:
□ Menu Principal aparece com todos os 6 botões
□ Cores estão corretas (Verde, Laranja, Ciano, Magenta, Amarelo, Vermelho)
□ Textos legíveis ("GHOST BEAM" em destaque)
□ Layout responsivo (sem elementos saindo da tela)

FUNCIONALIDADE:
□ Clica LOJA → mostra Shop com 3 items
□ Clica VOLTAR em Shop → volta para Menu
□ Clica RANKING → mostra Top 10 com rank do player
□ Clica VOLTAR em Ranking → volta para Menu
□ Clica DESAFIOS → mostra 3 desafios diários
□ Clica VOLTAR em Desafios → volta para Menu
□ Clica CONFIG → mostra Volume, Vibrações, etc
□ Clica VOLTAR em Config → volta para Menu
□ Clica JOGAR → inicia jogo (carrega próxima cena ou gameplay)
□ Clica SAIR → fecha jogo

CONSOLE:
□ Sem erros vermelhos
□ Logs do MainMenuController aparecem ao clicar botões
```

## Passo 8.3: Debug Mobile
- Se for testar Mobile:
  - Game View > Dimensão: "Simulate"
  - Escolha um device (iPhone, iPad, Android)
  - Verifique se layout se adapta

---

# 🎉 PRONTO!

Se tudo passou no Checklist, seu Menu está **100% funcional**!

---

## 📞 TROUBLESHOOTING

### Erro: "Não consigo arrastar o Canvas para o script"
**Solução:** No Dropdown de seleção de componente, escolha "Canvas (Main Menu)" ou o GameObject pai que tem MainMenuController.

### Erro: "Botão não dispara função"
**Solução:** Verifique se:
1. O Object arrastado tem o script (MainMenuController, etc)
2. A função está selecionada corretamente no dropdown
3. O Button tem o componente "Button" (não só Image)

### Erro: "Menu não fica visível ao iniciar"
**Solução:**
1. Verifique se Canvas tem Sort Order + 100 (maior que outros)
2. Verifique se RenderMode está "Screen Space - Overlay"
3. Abra `Canvas - Main Menu` na Hierarquia (clique triangle)

### Erro: "Texto muito pequeno ou grande"
**Solução:** Ajuste Font Size no TextMeshPro:
- Texto principal: 80-100
- Botões: 75
- Subtítulos: 32
- Items: 24

---

## 📝 NOTAS IMPORTANTES

1. **Order of Canvas**: Sempre deixar Main Menu com Sort Order MENOR que os outros (100 < 155/160/165)
2. **LayerMask**: Se tiver problemas com clicks, verifique se Canvas está no Canvas Layer padrão
3. **RespStatus**: Canvas precisa de CanvasScaler para funcionar bem em diferentes resoluções
4. **Performance**: Se ficar lento, reduza o número de items no Scroll View

---

**BOA SORTE! 🚀 Qualquer dúvida, avise!**
