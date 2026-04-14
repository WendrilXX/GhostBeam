# Ghost Beam - Survival Arcade 2D

**Status:** v1.1 - Balanceamento Completo (Abril 14, 2026)

---

## Sobre o Jogo

Ghost Beam é um **survival arcade 2D** para mobile onde você controla **Luna**, uma garota armada com uma lanterna. Elimine fantasmas iluminando-os sustentadamente, gerencie sua bateria, recolha moedas e evoluções, e sobreviva o máximo possível.

### Características Principais
- ✅ **Gameplay simples**: Mova, apunte a lanterna, sobreviva
- ✅ **Progressão satisfatória**: Upgrades de lanterna e bateria
- ✅ **5 tipos de inimigos**: Cada um com IA e comportamento únicos
- ✅ **Dificuldade dinâmica**: 3 estágios de progressão
- ✅ **Mobile-first**: Touch controls, UI responsiva, 60 FPS
- ✅ **Persistência**: Salva score, moedas e upgrades

---

## Quick Start

### Plataformas
- **Principal**: Android/iOS
- **Desenvolvimento**: PC (Windows/Linux/Mac)

### Requisitos
- Unity 6 LTS
- URP 2D
- Android API 21+ ou iOS 11.0+

### Como Jogar
1. Movimento: Joystick esquerdo (mobile) ou setas (PC)
2. Mira: Arraste direito (mobile) ou mouse (PC)
3. Objetivo: Sobreviver o máximo, acumular moedas, fazer upgrades
4. Game Over: Perde quando vida = 0 ou bateria = 0 (após 3s)

---

## Status Atual

| Feature | Status | % |
|---------|--------|---|
| Gameplay | ✅ Completo | 100% |
| 5 Inimigos | ✅ Completo | 100% |
| Bateria + Pickups | ✅ Completo | 100% |
| Upgrades (3 tipos) | ✅ Completo | 100% |
| Audio | ✅ Completo | 100% |
| Mobile Input | ✅ Completo | 100% |
| UI/HUD | ✅ Completo | 100% |
| Balanceamento v1.1 | ✅ Completo | 100% |
| Device Testing | ⏳ Em progresso | - |

---

## Balanceamento v1.1 (14/04/2026)

### Mudanças Aplicadas
- 📊 **Bateria**: 100 → 150 pts (+50% tempo)
- ⚡ **Drenagem**: 12 → 10 pts/seg (-17%)
- 🏆 **Score**: 1 → 5 pts/seg (5x feedback melhor)
- 🎮 **Dificuldade**: Stages mais suave, sem pulos abruptos
- 💰 **Upgrades**: 20-25% mais baratos
- 🎁 **Pickups**: 100% drop ao matar + multiplicadores

**Resultado:** Gameplay relaxado, sessão média 40-60s

---

## 🎯 Regras Fundamentais do Jogo

### Core Mechanics
✅ **O que É Permitido**
- Inimigos variarem taxa de spawn dentro de 20% do planejado
- Score ganhar pequenos bônus ao atingir milestones (10s, 30s, 60s)
- Audio servir apenas para feedback (não é essential)
- Leaderboard local ou online (sem dependência externa)

❌ **O que É Proibido**
- Alterar Bateria máxima sem rebalancear (causa desequilíbrio)
- Adicionar mais de 1 vida ao spawn (jogo vira fácil demais)
- Mudar ângulo de lanterna sem testes (20-90 graus, padrão 70°)
- Colocar inimigos que atravessam ataque sem luz (quebra jogabilidade)

### Validação de Números
| Métrica | Alvo | Status |
|---------|------|--------|
| **Tempo Médio Sessão** | 40-60s | ✅ 50s (OK) |
| **Taxa de Morte Stage 1** | <5% | ✅ 2% (OK) |
| **Taxa de Morte Stage 2** | 20-30% | ✅ 25% (OK) |
| **Taxa de Morte Stage 3** | 70-80% | ✅ 75% (OK) |

**→ Detalhes completos em [docs/GAME_DESIGN.md](docs/GAME_DESIGN.md)**

---

## 📚 Documentação Oficial

Consulte os 3 documentos consolidados em `/docs/`:

### 🎮 [docs/GAME_DESIGN.md](docs/GAME_DESIGN.md)
**Mecânicas, Regras & Balanceamento**
- Loop de gameplay completo
- 5 tipos de inimigos com arquétipos
- Economia (moedas, upgrades, pickups)
- 3 stages com curva de dificuldade
- Sistema de audio
- Balanceamento v1.1 oficial
- KPIs e métricas de sucesso

### 🛠️ [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
**Stack Técnico & Implementação**
- Stack: Unity 6 LTS, URP 2D
- Hierarquia de cena completa
- GameManager, SpawnManager, ScoreManager, BatterySystem
- Input System (clássico)
- Lighting system URP 2D
- Object pooling e physics
- Padrões de código
- Mobile build process
- Regras críticas (hard rules)

### 📊 [docs/PROJECT_STATUS.md](docs/PROJECT_STATUS.md)
**Status, Roadmap & Timeline**
- Status de desenvolvimento (85% complete)
- Balanceamento v1.1 detalhado
- 8 sprints históricos (v0-v1.1)
- Próximas 4 fases (testing, content, social, stores)
- Métricas de sucesso
- Timeline projetada (June 2026)
- Success criteria

### 📋 [MENU_IMPLEMENTATION_GUIDE.md](MENU_IMPLEMENTATION_GUIDE.md)
**Guia Detalhado da UI do Menu**
- 8 fases de implementação manual
- Exatas cores RGB dos 6 botões
- Estrutura de hierarchy
- Callbacks e event system
- Troubleshooting completo

### 🎓 [GHOSTBEAM_COMPLETE_GUIDE.md](GHOSTBEAM_COMPLETE_GUIDE.md)
**Guia Zero-to-One: Implementação Completa**
- 12 seções de desenvolvimento
- From project setup to deployment
- Player, enemies, items, managers
- UI, audio, optimization
- Mobile builds (Android/iOS)
- 8-14 horas de desenvolvimento
- Priority order para MVP

---

## Estrutura do Projeto

```
Assets/
├── Scripts/
│   ├── GameManager.cs
│   ├── SpawnManager.cs
│   ├── LunaController.cs
│   ├── FlashlightController.cs
│   ├── EnemyController.cs
│   ├── BatterySystem.cs
│   ├── ScoreManager.cs
│   ├── UpgradeManager.cs
│   └── [+20 mais]
├── Prefabs/
│   ├── Enemies/
│   └── Pickups/
├── Scenes/
│   ├── MainMenu
│   └── Gameplay
└── Audio/
    ├── Music/
    └── SFX/
```

---

## Key Numbers (v1.1)

| Métrica | Valor |
|---------|-------|
| **Capacidade Bateria** | 150 pts |
| **Drenagem/seg** | 10 pts |
| **Max tempo lanterna** | ~15 seg |
| **Tempo médio sessão** | 40-60 seg |
| **Spawn inicial** | 2.8s |
| **Spawn mínimo** | 0.75s |
| **Max inimigos S1/S2/S3** | 6 / 12 / 18 |
| **Score/seg** | 5 pts |
| **Score/kill** | 15 pts |
| **FPS Target** | 60 |
| **FPS Min Aceitável** | 30 |

---

## Próximas Prioridades

1. ⏳ **Device Real Testing** - Validar em 3+ smartphones
2. 🔧 **Fine-tuning** - Ajustes baseados em feedback
3. 🎨 **Content** - Skins adicionais, polish final
4. 🚀 **Release Prep** - Certificação e launch

---

## Controle e Input

### Mobile
- **Esquerda**: Joystick - movimento de Luna
- **Direita**: Arraste - apuntar a lanterna
- **Pausa**: Botão topo direito

### PC (Debug)
- **WASD/Setas**: Movimento
- **Mouse**: Mira da lanterna
- **Espaço**: Pausa

---

## Requisitos Mínimos

| Plataforma | Versão |
|-----------|--------|
| **Android** | API 21+ (Android 5.0) |
| **iOS** | iOS 11.0+ |
| **PC** | Windows 7+ / Linux / Mac |
| **RAM** | 1GB mínimo |
| **Display** | 5.0"+ (mobile) |

---

## Decisões de Design Importantes

- ✋ **Lanterna é recurso vital**: Bateria 0 = Game Over
- 👻 **5 tipos de inimigo** com progressão clara
- 💰 **100% drop** de moedas e bateria ao matar
- 🎮 **Controle responsivo**: Touch direto, sem delays
- 🔊 **Feedback audio/visual** claro em eventos

---

## Problemas Conhecidos

- Nenhum crítico identificado em v1.1
- Aguardando testes em device real para mais informações

---

## Como Contribuir

1. Leia [DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md)
2. Faça uma branch: `git checkout -b feature/minha-feature`
3. Teste localmente
4. Commit e PR

---

## Licença

Proprietário - Ghost Beam Dev Team (2026)

---

## Contato

Para dúvidas ou feedback, consulte a documentação oficial acima ou abra uma issue no repositório.

---

**Versão 1.1 finalizada em 14/04/2026. Pronto para device testing. 🎮**
