# GHOSTBEAM - Arcade de Sobrevivência 2D

**Versão:** 1.2.3  
**Status:** Pronto para Produção  
**Última Atualização:** 13 de Maio de 2026

## Participantes do Projeto

- Ingrid Ribeiro Silva
- João Antônio Gomes Gonçalves Leal Neto
- João Cruz de Farias Filho
- Kévilla da Silva Aguiar
- Luis Gabriel Salvador Barros
- Wendril Gabriel Medeiros Holanda

---

## 📱 Downloads & Releases

### ✅ Versão Pronta para Download

| Plataforma | Versão | Status | Download |
|-----------|--------|--------|----------|
| **Android (APK)** | 1.2.3 | ✅ Pronto | [Baixar APK](https://github.com/WendrilXX/GhostBeam/releases/download/v1.2.3/ghostbeam-v1.2.3.apk) |
| **Android (AAB)** | 1.2.3 | ✅ Disponível | Google Play Store (em breve) |
| **iOS** | 1.2.3 | ⏳ Em Preparação | App Store (em breve) |
| **Windows** | 1.2.3 | ✅ Pronto | [Executável](https://github.com/WendrilXX/GhostBeam/releases/download/v1.2.3/ghostbeam-v1.2.3.zip) |

### 🚀 Como Instalar o APK

1. **Baixar o arquivo APK**
2. **Transferir para seu dispositivo Android**
3. **Habilitar instalação de fontes desconhecidas** (Configurações → Segurança)
4. **Tocar no arquivo APK para instalar**
5. **Pronto! Começar a jogar**

---

## Visão Geral

GHOSTBEAM é um jogo arcade de sobrevivência 2D para celular onde o jogador controla Luna, uma personagem equipada com uma lanterna. Sobreviva a ondas de fantasmas, colete moedas, gerencie recursos de bateria e avance em fases cada vez mais difíceis. O jogo possui um sistema de UI completo com menus, HUD, pausa e suporte a localização.

### Funcionalidades Principais

| Funcionalidade | Status | Detalhes |
|--------|--------|---------|
| Sistema de Menu Principal | ✓ Completo | Jogar, Loja, Configurações, Sair |
| Menu de Configurações | ✓ Completo | Controle de volume, alternância de vibração |
| Tela de Game Over | ✓ Completo | Detecção automática, reiniciar, retornar ao menu |
| HUD do Jogo | ✓ Completo | Barra de saúde, pontuação, timer, onda, moedas, bateria |
| Botão de Pausa | ✓ Completo | Centro superior da tela, ícone ⏸ |
| Localização | ✓ Completo | Português (PT-BR) |
| Background da Floresta | ✓ Completo | Fundo visível com iluminação configurável |
| Desempenho | ✓ Completo | 60 FPS estável, otimizado para celular |
| Suporte de Plataforma | ✓ Completo | Android, iOS, Windows Desktop |
| Compilação | ✓ Completo | 0 erros, 0 avisos |

---

## Índice

1. [GDD - Game Design Document](#gdd---game-design-document)
2. [Início Rápido](#início-rápido)
3. [Requisitos do Sistema](#requisitos-do-sistema)
4. [Instalação](#instalação)
5. [Estrutura do Projeto](#estrutura-do-projeto)
6. [Arquitetura](#arquitetura)
7. [Funcionalidades](#funcionalidades)
8. [Documentação](#documentação)
9. [Status de Desenvolvimento](#status-de-desenvolvimento)
10. [Build & Implantação](#build--implantação)
11. [Suporte & Resolução de Problemas](#suporte--resolução-de-problemas)

---

## Início Rápido

### Setup em 5 Minutos

1. **Abra a cena de Gameplay**
2. **Crie um GameObject vazio** e nomeie-o "UIRoot"
3. **Anexe o script MainMenuUIBuilder.cs** ao UIRoot
4. **Pressione Play** no editor

O script cria automaticamente:
- Canvas do Menu Principal
- Canvas do Menu de Pausa
- Canvas de Configurações
- Canvas de Game Over
- Canvas HUD do Jogo
- EventSystem (para interações de botão)

### ⚠️ Setup #5 - Verificação da Hierarquia do Projeto

**IMPORTANTE:** Após executar o setup acima, **certifique-se que a hierarquia do projeto contém as duas cenas principais:**

```
Hierarchy
├── All
├── MainMenu
└── Gameplay
```

Ambas as cenas devem estar presentes no projeto para o fluxo de menu funcionasse corretamente.

### 🎨 Setup #6 - Configurar Background da Floresta

**Como adicionar o fundo visível durante o jogo:**

#### Passo 1: Importe a Imagem do Background
1. Copie sua imagem de fundo (PNG/JPG) para: `Assets/_Project/Sprites/`
2. Selecione a imagem no Project
3. No Inspector, configure:
   - **Texture Type:** Sprite (2D and UI)
   - **Sprite Mode:** Single
   - **Pixels Per Unit:** 100
   - **Filter Mode:** Bilinear
4. Clique **Apply**

#### Passo 2: Adicione BackgroundConfigurator na Cena
1. Na cena **Gameplay**, selecione o GameManager ou qualquer GameObject no root
2. **Add Component** → `BackgroundConfigurator`
3. No Inspector, arraste sua imagem para o campo **Background Image**
4. Defina **Global Light Intensity** (padrão: 0.4)

#### Passo 3: Execute
- Play no editor
- O script automaticamente:
  - Cria GameObject "Background" com sua imagem
  - Configura sorting order (-10) para ficar atrás
  - Aumenta Global Light 2D Intensity para 0.4 (ilumina o cenário)

**Resultado:** Seu fundo aparecerá visível com iluminação ambiente adequada! 🌲

**Para instruções de setup detalhadas, veja [docs/SETUP_COMPLETO.md](docs/SETUP_COMPLETO.md)**

**IMPORTANTE - Novas Regras:**

- **Bateria drena 2-7% por segundo** (aleatório) enquanto ilumina
- **Bateria APENAS recarrega ao matar inimigos:** +10% a +45% (aleatório)
- ⚠️ **Sem recarga automática** - A bateria não recarrega passivamente
- **Game Over acontece se:** Vida = 0 OU Bateria = 0 (imediato)

Essas mudanças tornaram o jogo **mais estratégico**: você precisa gerenciar melhor a bateria matando inimigos!

**Para instruções de setup detalhadas, veja [docs/SETUP_COMPLETO.md](docs/SETUP_COMPLETO.md)**

---

## Requisitos do Sistema

### Requisitos de Execução

| Requisito | Mínimo | Recomendado |
|-------------|--------|-------------|
| **Engine** | Unity 2023 LTS | Unity 6 LTS+ |
| **Gráficos** | URP 2D | URP 2D |
| **RAM (Android)** | 1 GB | 2 GB+ |
| **RAM (iOS)** | 1 GB | 2 GB+ |
| **Display** | 4.5" (paisagem) | 5.5"+ (paisagem) |
| **API Android** | 21+ (Lollipop) | 28+ (Pie) |
| **iOS** | 11.0+ | 14.0+ |
| **Desempenho** | 50 FPS | 60 FPS |

### Requisitos de Desenvolvimento

- **Versão Unity:** 2023.2 LTS ou superior
- **TextMesh Pro:** Última versão (incluído no Unity)
- **Sistema de Entrada:** Legacy Input Manager ativado
- **OS de Desenvolvimento:** Windows, macOS ou Linux
- **Plataformas Alvo:** Android 5.0+, iOS 11.0+, Windows Desktop

---

---

## Estrutura do Projeto

### Organização de Diretórios

```
Assets/
├── _Project/
│   ├── Managers/          # GameManager, AudioManager, SettingsManager, UpgradeManager, SkinManager
│   ├── Player/            # LunaController, FlashlightController, PlayerInput
│   ├── Enemy/             # EnemyController, SpawnManager, EnemyArchetype
│   ├── Items/             # BatterySystem, CoinPickup, BatteryPickup, Pickup
│   ├── Gameplay/          # PauseSystem, GameOverPanelController, ScoreManager
│   ├── UI/                # MainMenuController, UIBootstrapper, HUDController, SettingsManager
│   └── Utilities/         # ObjectPool, PooledObject, SafeAreaFitter, PerformanceOverlay
├── Resources/             # Assets localizáveis e prefabs
├── Sav1.unity             # Cena principal de Gameplay
└── [Configurações padrão do Unity]
```

### Namespaces e Estrutura de Scripts

**24 scripts reconstruídos** com namespaces claros (Abr/2026):

| Namespace | Scripts | Função |
|-----------|---------|--------|
| `GhostBeam.Managers` | GameManager, AudioManager, SettingsManager, UpgradeManager, SkinManager | Gerência de estado e configurações |
| `GhostBeam.Player` | LunaController, FlashlightController | Controles do jogador |
| `GhostBeam.Enemy` | EnemyController, SpawnManager, EnemyArchetype | Sistema de inimigos |
| `GhostBeam.Items` | BatterySystem, CoinPickup, BatteryPickup, PickupFloatingText | Itens e pickups |
| `GhostBeam.Gameplay` | PauseSystem, GameOverPanelController, ScoreManager, HealthSystem, GameplayIntroState | Lógica de gameplay |
| `GhostBeam.UI` | MainMenuController, UIBootstrapper, HUDController, MenuLayoutReorganizer, MenuUIAnimator, MenuBackgroundAnimator, GameplayIntroFade, MobileControlsOverlay | Sistemas de UI |
| `GhostBeam.Utilities` | ObjectPool, PooledObject, SafeAreaFitter, PerformanceOverlay, ProjectValidator | Ferramentas auxiliares |

### Ferramentas de Desenvolvimento

**ProjectValidator.cs** (Utilities)
- Valida integridade do projeto ao iniciar
- Verifica se Managers estão configurados
- Detecta referências faltantes
- Compatível com SafeAreaFitter para layout mobile

**PerformanceOverlay.cs** (Utilities)
- Exibe FPS, memória e carga adaptativa em tempo real
- Configurável via SettingsManager (toggle persistente)
- Facilita tuning de performance em device mobile

---

## Arquitetura

### Sistemas Principais

**GameManager (Padrão Singleton)**
- Coordenador central do jogo
- Manipula transições de estado (MainMenu → Gameplay → Pause → GameOver)
- Sistema de eventos para mudanças de estado
- Gerenciamento de escala de tempo para pausa

**MainMenuUIBuilder**
- Cria e gerencia todos os canvas de UI
- Manipula navegação de menu
- Atualiza HUD em tempo real
- Cria automaticamente EventSystem se estiver faltando

**HealthSystem**
- Rastreamento de saúde do jogador
- Detecção de morte
- Eventos de mudança de saúde
- Coloração dinâmica da barra de saúde (verde → amarelo → vermelho)

**ScoreManager**
- Rastreamento de pontuação e moedas
- Persistência de recorde
- Callbacks de coleta de moeda

**BatterySystem**
- Gerenciamento de bateria da lanterna
- Mecânica de drenagem de bateria
- Manipulação de pickup e recarga

**SpawnManager**
- Spawning de onda de inimigos
- Progressão de dificuldade (3 estágios)
- Configuração de distância de spawn
- Contador de onda

### Padrões de Design

- **Singleton:** GameManager, AudioManager, gerenciadores
- **Observer:** Comunicação baseada em eventos entre sistemas
- **Object Pooling:** Reutilização de inimigos e projéteis
- **State Machine:** Gerenciamento de estado do jogo

### Fluxo de Dados

```
Entrada do Jogador
    ↓
InputSystem → GameManager
    ↓
Atualizar: HealthSystem, ScoreManager, BatterySystem
    ↓
Evento: onHealthChanged, onScoreChanged, etc.
    ↓
Atualizar HUD: MainMenuUIBuilder
---

## Funcionalidades

### Sistema de Menu

| Funcionalidade | Implementação |
|--------|-----------------|
| **Menu Principal** | Jogar, Loja (placeholder), Configurações, Sair |
| **Menu de Pausa** | Retomar, Configurações, Retornar ao Menu (tecla ESC) |
| **Configurações** | Controle deslizante de volume (0-1), alternância de vibração |
| **Game Over** | Detecção automática, exibição de Pontuação/Recorde |
| **Localização** | Português (PT-BR) com texto dinâmico |

### HUD do Jogo

- **Barra de Saúde:** Coloração dinâmica (verde 75%+ → amarelo 25-75% → vermelho <25%)
- **Exibição de Pontuação:** Contador de pontuação em tempo real
- **Timer:** Tempo de sobrevivência em formato MM:SS
- **Contador de Onda:** Estágio de dificuldade atual
- **Moedas:** Exibição de moedas coletadas
- **Bateria:** Nível atual de bateria da lanterna

### Mecânicas de Gameplay

- **5 Tipos de Inimigo:** Dificuldade progressiva com comportamentos únicos
- **Escalonamento de Dificuldade:** 3 estágios com progressão baseada em ondas
- **Sistema de Bateria:** Drenagem 2-7% por segundo enquanto ilumina, recarrega 10-45% AO MATAR INIMIGOS
- **Regra de Game Over:** Vida = 0 OU Bateria = 0 (imediato, sem delay)
- **Coleta de Moeda:** 100% de queda na derrota de inimigo
- **Sistema de Pickup:** Pickups de bateria e moeda com feedback de texto flutuante
- **Sistema de Onda:** Spawning progressivo de inimigos com curva de dificuldade

### Suporte de Plataforma

- **Android:** 5.0+ (API 21+) com orientação paisagem
- **iOS:** 11.0+ com tratamento de área segura
- **Windows:** Suporte a build desktop
- **Orientação:** Apenas paisagem (1920x1080)

### Redesign do Menu Principal (v1.2)

**Layout Grid 2x2 Moderno:**
- Botões reorganizados em grid responsivo
- Cores vibrantes por função: Jogar (vermelho), Loja (amarelo), Config (roxo), Tutorial (cyan)
- Tamanho otimizado para touch (200×200px)
- Animações suaves: fade-in, scale on hover, transições

**Scripts de UI:**
- `MenuLayoutReorganizer.cs` - Reorganização automática do grid
- `MenuUIAnimator.cs` - Animações elegantes
- `MenuBackgroundAnimator.cs` - Fundo dinâmico
- `GameplayIntroFade.cs` - Fade de intro do gameplay
- `MobileControlsOverlay.cs` - Overlay de controles mobile

**Veja:** [docs/MENU_REDESIGN.md](docs/MENU_REDESIGN.md)

### Garantia de Qualidade

- **Desempenho:** 60 FPS estável (50 FPS mínimo aceitável)
- **Compilação:** 0 erros, 0 avisos
- **Memória:** Otimizado para dispositivos 1 GB+
---

## Documentação

Documentação completa está disponível na pasta `/docs/`. Comece com o índice:

### Documentos Principais

| Documento | Propósito | Público |
|----------|---------|----------|
| [GDD (1).pdf](GDD%20(1).pdf) | Game Design Document | Todos |
| [docs/INDEX.md](docs/INDEX.md) | Mapa de documentação | Todos |
| [docs/SETUP_COMPLETO.md](docs/SETUP_COMPLETO.md) | Guia de setup rápido (5 min) | Desenvolvedores |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Arquitetura técnica | Programadores |
| [docs/GAME_DESIGN.md](docs/GAME_DESIGN.md) | Mecânicas & balanceamento do jogo | Designers, Balanceadores |
| [docs/MENU_REDESIGN.md](docs/MENU_REDESIGN.md) | Redesign do menu v1.2 | Designers, Desenvolvedores |
| [docs/PROJECT_STATUS.md](docs/PROJECT_STATUS.md) | Cronograma & roadmap | PMs, Stakeholders |
| [docs/GHOSTBEAM_COMPLETE_GUIDE.md](docs/GHOSTBEAM_COMPLETE_GUIDE.md) | Referência completa | Todos (abrangente) |

### Links Rápidos por Função

- **Quero configurar o jogo:** → [SETUP_COMPLETO.md](docs/SETUP_COMPLETO.md)
- **Quero entender o código:** → [ARCHITECTURE.md](docs/ARCHITECTURE.md)
- **Quero modificar o balanceamento do jogo:** → [GAME_DESIGN.md](docs/GAME_DESIGN.md)
---

## Status de Desenvolvimento

### Versão Atual: v1.2.2 (Otimizações Finais)

**Status de Conclusão:** 90%

| Componente | Status | Última Atualização |
|-----------|--------|-------------|
| Gameplay Principal | ✓ Completo | 13 de Maio de 2026 |
| Sistema de Menu | ✓ Completo | 11 de Maio de 2026 |
| Sistema HUD | ✓ Completo | 13 de Maio de 2026 |
| Sistema de Pausa | ✓ Completo | 30 de Abril de 2026 |
| Sistema de Áudio | ✓ Completo | 13 de Maio de 2026 |
| Sistema de Bateria | ✓ Completo | 13 de Maio de 2026 |
| Background Visual | ✓ Completo | 13 de Maio de 2026 |
| Localização | ✓ Completo | 30 de Abril de 2026 |
| Otimização de Desempenho | ✓ Completo | 30 de Abril de 2026 |
| Teste em Dispositivo | Em Progresso | - |
| Polish de Conteúdo | Pendente | - |

### Histórico de Versão

- **v1.2.1** (13 de Maio de 2026) - Balanceamento de bateria, game over unificado, background visual
- **v1.2** (11 de Maio de 2026) - Redesign do menu principal com novo layout grid
- **v1.1** (14 de Abril de 2026) - Consolidação de menu, melhorias de balanceamento
- **v1.0** (1º de Abril de 2026) - Lançamento inicial
- **v0.x** (Sprints anteriores) - Iterações de desenvolvimento

### Próximas Prioridades

1. **Teste em Dispositivo Real** - Validar em 5+ dispositivos Android/iOS
2. **Perfil de Desempenho** - Garantir 60 FPS estável em todos os dispositivos
3. **Expansão de Conteúdo** - Skins adicionais, cosméticos

---

## Padrões de Código

- **Linguagem:** C# (.NET)
- **Versão Unity:** 2023 LTS+
- **Formatação:** Use .editorconfig se disponível
- **Comentários:** Documentação em inglês com intenção clara
- **Testes:** Testes unitários para sistemas críticos
- **Convenções:** PascalCase para classes, camelCase para variáveis
- **Sem avisos de compilador**

## Links Importantes

- **Documentação Geral:** [docs/INDEX.md](docs/INDEX.md)
- **Arquitetura Técnica:** [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
- **Design do Jogo:** [docs/GAME_DESIGN.md](docs/GAME_DESIGN.md)
- **Game Design Document:** [GDD (1).pdf](GDD%20(1).pdf)
- **Cronograma & Status:** [docs/PROJECT_STATUS.md](docs/PROJECT_STATUS.md)

---

## Changelog

### v1.2.2 - 13 de Maio de 2026 (Otimizações Finais)

**Sistema de Intro - Fallback Automático:**
- ✅ GameplayIntroState com `EnsureGameplayStarted()` - auto-inicia gameplay se não houver GameplayIntroFade
- ✅ Fallback automático após 2 segundos se intro ficar travada
- ✅ Resolve issue de fases de spawn presas em 0 segundos
- ✅ Compatível com scenes sem animação de intro

**Visual do Background - Brilho Aumentado:**
- ✅ Global Light 2D Intensity aumentado de 0.7 → **1.3** (86% mais claro)
- ✅ Floresta de fundo agora **completamente visível** durante gameplay
- ✅ BackgroundConfigurator.cs atualizado com novo valor de intensidade
- ✅ Aplicação automática de Filter Mode **Bilinear** para remover pixelação

**Progressão de Spawn - Fases Aceleradas:**
- ✅ Fases reduzidas de **60s → 40s** por fase (33% mais rápido)
- ✅ Spawn rates 50% mais rápidos: 2.8s → **1.5s** inicial (reduz até 0.25s máximo)
- ✅ Nova progressão:
  - 0-40s: Penado 100% (1.8→1.5s spawn)
  - 40-80s: Penado 38% + Ictericia 62% (1.5→1.2s spawn)
  - 80-120s: 3 tipos (1.2→0.9s spawn)
  - 120-160s: 4 tipos com Titã (0.9→0.6s spawn)
  - 160s+: Espectro ondas (0.6→0.25s spawn)
- ✅ Gameplay **mais dinâmico e arcade**

**Limpeza do Setup:**
- ✅ Removido Background duplicado do MainMenu (apenas MenuCanvas UI background)
- ✅ AutoSetupCompleteGame.cs atualizado para não criar sprite Background redundante
- ✅ Hierarquia mais limpa e otimizada

**Verificação Completa:**
- ✅ Todos 5 mobs (Penado, Ictericia, Ectogangue, Titã, Espectro) spawning corretamente
- ✅ Compilação: 0 erros, 0 avisos
- ✅ Documentação atualizada (README, ARCHITECTURE, GAME_DESIGN)

### v1.2.1 - 13 de Maio de 2026

**Balanceamento de Gameplay - Regra de Bateria & Game Over:**
- ✅ **Bateria agora drena 2-7% por segundo** (variável aleatória) enquanto ilumina
- ✅ **Bateria APENAS recarrega ao matar inimigos**: 10-45% aleatório por kill
- ✅ Removida a recarga automática de bateria (antes recarregava ~1% por segundo)
- ✅ **Regra de Game Over unificada:** Vida = 0 OU Bateria = 0 (imediato, sem delay)
- ✅ **Corrigido bug:** Bateria agora dispara game over mesmo que jogador solte o botão de iluminar
- ✅ Melhorias em AudioManager: Verificação de volume zero em celular
- ✅ Debug logs para bateria recarregada ao matar inimigo

**Background da Floresta - Sistema Visual:**
- ✅ BackgroundConfigurator.cs - Script automático para configurar background e iluminação
- ✅ Global Light 2D Intensity aumentado automaticamente (0 → 0.4)
- ✅ Setup simplificado: Basta arrastar imagem no Inspector
- ✅ Instruções de setup adicionadas: README (Setup #6)

**Documentação Atualizada:**
- ✅ GAME_DESIGN.md, ARCHITECTURE.md, README.md atualizados
- ✅ Changelog v1.2.1 com todas as mudanças

### v1.2 - 11 de Maio de 2026

**Redesign do Menu Principal:**
- ✅ Layout grid 2x2 moderno com cores vibrantes (vermelho, amarelo, roxo, cyan)
- ✅ MenuLayoutReorganizer.cs - Reorganização automática de botões
- ✅ MenuUIAnimator.cs - Animações elegantes (fade-in, scale, transições)
- ✅ MenuBackgroundAnimator.cs - Fundo dinâmico com suporte para Light2D
- ✅ GameplayIntroFade.cs - Fade de intro do gameplay
- ✅ GameplayIntroState.cs - Novo estado de intro
- ✅ MobileControlsOverlay.cs - Overlay de controles mobile
- ✅ Otimização de spawner: BatteryPickupSpawner, CoinPickupSpawner refatorados
- ✅ Documentação: MENU_REDESIGN.md adicionado
- ✅ Responsivo para mobile landscape (1920×1080)



## Reconhecimentos

### Créditos Técnicos

- Unity Technologies pelo game engine
- TextMesh Pro para renderização de texto
- Feedback e testes da comunidade
