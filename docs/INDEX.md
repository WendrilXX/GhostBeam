# 📚 ÍNDICE DE DOCUMENTAÇÃO - GHOSTBEAM

## 🎯 Por Onde Começar?

**Sua primeira vez?** → [SETUP_COMPLETO.md](SETUP_COMPLETO.md) ⭐

**Quer entender o jogo?** → [GAME_DESIGN.md](GAME_DESIGN.md)

**Precisa de detalhes técnicos?** → [ARCHITECTURE.md](ARCHITECTURE.md)

**Quer saber o status?** → [PROJECT_STATUS.md](PROJECT_STATUS.md)

---

## 📖 Todos os Documentos

### 1. ⭐ [SETUP_COMPLETO.md](SETUP_COMPLETO.md)
**Tempo de leitura:** 5 min  
**Público:** Desenvolvedores querendo começar rápido

Cobre:
- Setup da cena em 5 minutos
- O que o script cria automaticamente
- Menu system completo
- HUD in-game
- Troubleshooting

**Use este se você:** Quer colocar o jogo em uma cena e começar

---

### 2. 🎮 [GAME_DESIGN.md](GAME_DESIGN.md)
**Tempo de leitura:** 15 min  
**Público:** Game Designers, Balanceadores

Cobre:
- 5 tipos de inimigos (Penadinho, Penado, Fantasmão, etc)
- Sistema de dificuldade (3 stages)
- Economia (moedas, upgrades, pickups)
- Balanceamento v1.1
- KPIs e métricas de sucesso
- Regras do jogo (hard rules)

**Use este se você:** Quer entender como o jogo funciona ou modificar balanceamento

---

### 3. 🛠️ [ARCHITECTURE.md](ARCHITECTURE.md)
**Tempo de leitura:** 20 min  
**Público:** Programadores, arquitetos

Cobre:
- Stack técnico (Unity 6, URP 2D)
- Padrões de design (Singleton, Object Pooling)
- GameManager, SpawnManager, etc
- Input System (Legacy)
- Physics e Lighting
- Mobile build process
- Performance optimization

**Use este se você:** Quer entender como o código está estruturado

---

### 4. 📊 [PROJECT_STATUS.md](PROJECT_STATUS.md)
**Tempo de leitura:** 10 min  
**Público:** PMs, stakeholders, time

Cobre:
- Status de desenvolvimento (88% complete)
- Histórico de versions (v0-v1.1)
- 8 sprints anteriores
- Próximas 4 fases (testing, content, social, stores)
- Timeline projetada (June 2026)
- Success criteria

**Use este se você:** Quer saber o andamento do projeto

---

### 5. 📚 [GHOSTBEAM_COMPLETE_GUIDE.md](GHOSTBEAM_COMPLETE_GUIDE.md)
**Tempo de leitura:** 60 min  
**Público:** Desenvolvedores, referência completa

Cobre:
- Setup zero-to-one (do zero ao jogo completo)
- 12 seções detalhadas
- Player, enemies, items, managers
- UI, audio, optimization
- Mobile builds (Android/iOS)
- 8-14 horas de desenvolvimento
- Priority order para MVP

**Use este se você:** Quer referência completa ou está reconstruindo o projeto

---

### 6. 📋 [CHECKLIST_VALIDACAO_MOBILE_15MIN.md](CHECKLIST_VALIDACAO_MOBILE_15MIN.md)
**Tempo de leitura:** 5 min  
**Público:** QA, testers

Checklist rápido para validar o jogo em 15 minutos

**Use este se você:** Está testando antes de release

---

### 7. 🇧🇷 [GUIA_IMPLEMENTACAO_UNITY.md](GUIA_IMPLEMENTACAO_UNITY.md)
**Público:** Desenvolvedores em português

Guia de implementação em português

---

## 🗺️ Mapa de Navegação

```
README.md (raiz)
    ↓
    ├─→ Quer começar rápido?
    │   └─→ docs/SETUP_COMPLETO.md ⭐
    │
    ├─→ Quer entender o design?
    │   └─→ docs/GAME_DESIGN.md
    │
    ├─→ Quer detalhes técnicos?
    │   ├─→ docs/ARCHITECTURE.md
    │   └─→ docs/GHOSTBEAM_COMPLETE_GUIDE.md
    │
    ├─→ Quer saber o status?
    │   └─→ docs/PROJECT_STATUS.md
    │
    └─→ Precisa testar?
        └─→ docs/CHECKLIST_VALIDACAO_MOBILE_15MIN.md
```

---

## 🎯 Por Objetivo

### "Como eu faço X?"

| Objetivo | Documento |
|----------|-----------|
| Colocar o menu em uma cena | SETUP_COMPLETO.md |
| Entender os inimigos | GAME_DESIGN.md |
| Adicionar um novo inimigo | ARCHITECTURE.md + GAME_DESIGN.md |
| Mudar balanceamento | GAME_DESIGN.md |
| Entender a estrutura de código | ARCHITECTURE.md |
| Fazer um novo script | GHOSTBEAM_COMPLETE_GUIDE.md |
| Build para Android/iOS | ARCHITECTURE.md |
| Testar o jogo | CHECKLIST_VALIDACAO_MOBILE_15MIN.md |
| Ver timeline do projeto | PROJECT_STATUS.md |

---

## 📱 Referência Rápida

### Principais Scripts
```
MainMenuUIBuilder.cs    - Menu, HUD, Pausa (MEGA SCRIPT)
GameManager.cs          - Gerenciador central
HealthSystem.cs         - Saúde e morte
ScoreManager.cs         - Pontos e moedas
BatterySystem.cs        - Bateria da lanterna
SpawnManager.cs         - Spawn de inimigos
```

### Hotkeys
```
ESC         - Pausa/Resume
Space       - (No menu) Navegar/Selecionar
```

### Arquivos Importantes
```
Assets/MainMenuUIBuilder.cs     ← Script principal
Assets/GameManager.cs           ← Centro do jogo
docs/SETUP_COMPLETO.md          ← Comece aqui!
README.md (raiz)                ← Overview
```

---

## ✅ Checklist de Documentação

- [x] SETUP_COMPLETO.md (novo, simplificado)
- [x] GAME_DESIGN.md (jogabilidade)
- [x] ARCHITECTURE.md (técnico)
- [x] PROJECT_STATUS.md (timeline)
- [x] GHOSTBEAM_COMPLETE_GUIDE.md (referência)
- [x] CHECKLIST_VALIDACAO_MOBILE_15MIN.md (testes)
- [x] GUIA_IMPLEMENTACAO_UNITY.md (português)
- [x] INDEX.md (este arquivo)

---

## 📞 Suporte Rápido

**Botões não clicam?**  
→ SETUP_COMPLETO.md → Troubleshooting → "Botões não clicam"

**HUD não mostra valores?**  
→ SETUP_COMPLETO.md → Troubleshooting → "HUD não mostra valores"

**Como modificar menus?**  
→ ARCHITECTURE.md → MainMenuUIBuilder

**Como adicionar novo inimigo?**  
→ GAME_DESIGN.md + GHOSTBEAM_COMPLETE_GUIDE.md

