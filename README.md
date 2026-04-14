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

## Documentação Oficial

Consulte os 3 documentos principais:

###📘 [GAME_MECHANICS.md](GAME_MECHANICS.md)
Mecânicas do jogo, números, balanceamento
- Regras de gameplay
- Inimigos e arquétipos
- Economia de moedas
- Upgrades e custos
- Validação de numbers

### 🛠️ [DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md)
Guia técnico de desenvolvimento
- Arquitetura de cena
- Principais sistemas (GameManager, SpawnManager, etc)
- Mobile integration
- Build process
- Debugging tips

### 🗺️ [ROADMAP.md](ROADMAP.md)
Roadmap e histórico de desenvolvimento
- Sprint history
- Status do projeto
- Próximas fases
- KPIs e métricas

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
