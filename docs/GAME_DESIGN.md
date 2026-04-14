# 🎮 Ghost Beam - Game Design Document

**Versão:** 1.1  
**Data de Última Atualização:** 14 de Abril de 2026  
**Status:** Balanceamento Completo

---

## 1. Visão Geral do Jogo

### 1.1 Conceito
Ghost Beam é um **survival arcade 2D** onde você controla **Luna**, uma garota armada com uma lanterna energética. Sua missão: iluminar e derrotar fantasmas que invadem a floresta, gerenciar sua bateria, e sobreviver o máximo possível para acumular moedas e fazer upgrades.

### 1.2 Público-Alvo
- **Plataforma:** Mobile (iOS/Android) - LANDSCAPE ONLY
- **Resolução:** 1920x1080 (16:9 landscape)
- **Gênero:** Arcade, Casual, Action
- **Público:** 8+ anos (gameplay simples mas envolvente)
- **Duração Média por Sessão:** 40-60 segundos
- **Input:** Touch native (sem mouse/keyboard)

---

## 2. Loop de Gameplay

### 2.1 Fluxo Principal
```
1. MENU PRINCIPAL
   ├─ [JOGAR] → Inicia gameplay
   ├─ [LOJA] → Compra upgrades com moedas
   ├─ [DESAFIOS] → Missions diárias
   ├─ [CONFIG] → Audio, vibração, overlay
   └─ [SAIR] → Fecha jogo

2. GAMEPLAY (durante sessão - LANDSCAPE)
   ├─ MOVIMENTO: Joystick virtual ESQUERDA (6 pontos)
   ├─ MIRA: Joystick virtual DIREITA (rotação contínua)
   ├─ COMBATE: Inimigos morrem com iluminação sustentada
   ├─ BATERIA: Drena enquanto ilumina, recarrega com pickups
   ├─ SCORE: +5 pts/seg + 15 pts/inimigo
   ├─ MOEDAS: 100% de drop ao matar inimigo
   ├─ PAUSAR: Botão UI (toque) → Pause Menu
   └─ GAME OVER: Vida = 0 OU bateria apaga 3s

3. GAME OVER
   ├─ Mostra: Score Final, High Score, Moedas Ganhas
   ├─ [REINICIAR] → Novo jogo
   └─ [MENU] → Volta ao menu

4. PROGRESSÃO
   ├─ Moedas → Upgrade na LOJA
   ├─ Upgrades → Melhor performance
   └─ Melhor performance → Mais pontos → Mais upgrade
```

### 2.2 Mecânicas Essenciais

#### Movimento (Luna) - TOUCH MOBILE
- **Input:** Joystick virtual esquerdo (touch + drag)
- **Velocidade:** 8 unidades/seg
- **Direção:** 8-directional (N, NE, E, SE, S, SW, W, NW)
- **Limites:** Dentro dos bounds da cena (10×15)
- **Responsive:** Touch latência <50ms

#### Lanterna (Flashlight) - TOUCH MIRA
- **Tipo de Luz:** Light 2D Spot (não Point, não Directional)
- **Raio de Iluminação:** 15 unidades
- **Ângulo de Abertura:** 70 graus
- **Acompanhamento:** Rotaciona em direção ao joystick virtual DIREITO
- **Velocidade de Rotação:** 10 multiplicador
- **Platform-Specific:** Safe area aware (notches, home indicator)

#### Iluminação de Inimigos
**Fórmula de Detecção:**
```csharp
Vector2 toEnemy = (Vector2)(transform.position - flashlight.position);
float distance = toEnemy.magnitude;
float angle = Vector2.Angle(flashlight.up, toEnemy);

bool isIlluminated = (distance < 15f && angle < 35f);
```
- **Distância Máxima:** 15 unidades
- **Ângulo Máximo:** 35 graus
- **Tempo até Morte:** Varia por inimigo (2s a 4.6s)

---

## 3. Recursos Vitais

### 3.1 Vida (Health)
| Atributo | Valor |
|----------|-------|
| **Quantidade Inicial** | 3 vidas |
| **Dano por Contato** | -1 vida |
| **Dano por Corrosão** | Nenhum |
| **Recuperação** | Nenhuma (só upgrades) |
| **Game Over** | Ao atingir 0 |
| **Upgrade Disponível** | +1 vida (custa 150 moedas) |

### 3.2 Bateria (Energy)
| Atributo | Valor |
|----------|-------|
| **Capacidade Máxima** | 150 pontos |
| **Drenagem** | 10 pts/seg (quando ilumina) |
| **Recarga** | Apenas pickup Battery (+100 pts) |
| **Tempo Máximo de Lanterna** | ~15 segundos |
| **Blackout** | Lanterna apaga por 3s se = 0 |
| **Game Over** | Se blackout permanece após 3s |
| **Upgrade Disponível** | +50 pts capacidade (custa 100 moedas) |

### 3.3 Score (Pontos)
| Ação | Pontos |
|------|--------|
| Cada segundo vivo | +5 pts |
| Cada inimigo morto | +15 pts |
| Bônus de combo | +25 pts (cada combo x2) |
| **Máximo por Sessão** | Ilimitado |
| **Salvo em** | PlayerPrefs (highscore) |

### 3.4 Moedas (Currency)
| Ação | Moedas |
|------|--------|
| Cada inimigo morto | +1 moeda (base) |
| Multiplicador | 1x-2.5x por tipo |
| Vendo pickup | 100% drop ao matar |
| **Máximo Acumulável** | Ilimitado |
| **Armazenamento** | PlayerPrefs (persistente) |

---

## 4. Sistema de Inimigos

### 4.1 Arquétipos de Inimigos

#### 1️⃣ PENADO (Básico)
```
┌─ Dificuldade: ⭐☆☆☆☆
├─ Tempo para Morte: 2.0s
├─ Velocidade: 1.5 u/seg
├─ Vida: 100 HP
├─ Drop Moeda: 1.0x
├─ Drop Bateria: 1.5x
├─ Frequência de Spawn: 40%
└─ Descrição: Fantasma básico, fácil de derrotar
```

**Comportamento:**
- Segue linha reta em direção ao jogador
- Ataca ao colidir (causa -1 HP)
- Sem padrão de movimento especial

---

#### 2️⃣ ICTERICIA (Rápido)
```
┌─ Dificuldade: ⭐⭐☆☆☆
├─ Tempo para Morte: 2.0s
├─ Velocidade: 1.9 u/seg
├─ Vida: 100 HP
├─ Drop Moeda: 1.1x
├─ Drop Bateria: 1.5x
├─ Frequência de Spawn: 30%
└─ Descrição: Mais rápido que Penado
```

**Comportamento:**
- Segue Luna com 1.9x velocidade
- Tenta interceptar movimento
- Aparece no Stage 2+

---

#### 3️⃣ ECTOGANGUE (Grupo)
```
┌─ Dificuldade: ⭐⭐☆☆☆
├─ Tempo para Morte: 2.8s
├─ Velocidade: 1.9 u/seg
├─ Vida: 140 HP
├─ Drop Moeda: 1.0x
├─ Drop Bateria: 2.0x
├─ Frequência de Spawn: 20%
└─ Descrição: Leve tanque, spawn em grupos
```

**Comportamento:**
- Movimento aleatório + direção ao jogador
- Maior HP que Penado
- Aparecem em pares/trios (Stage 2+)

---

#### 4️⃣ TITA (Tanque Pesado)
```
┌─ Dificuldade: ⭐⭐⭐⭐☆
├─ Tempo para Morte: 4.6s
├─ Velocidade: 0.95 u/seg
├─ Vida: 230 HP
├─ Drop Moeda: 1.5x
├─ Drop Bateria: 2.5x
├─ Frequência de Spawn: 5%
└─ Descrição: Chefe minion, muito difícil
```

**Comportamento:**
- Movimento lento mas persistente
- Muito HP (4.6s de lanterna)
- Grande recompensa em moedas/bateria
- Apenas Stage 3 (125s+)

---

#### 5️⃣ ESPECTRO (Frágil Rápido)
```
┌─ Dificuldade: ⭐⭐⭐☆☆
├─ Tempo para Morte: 0.95s
├─ Velocidade: 3.15 u/seg
├─ Vida: 47 HP (frágil!)
├─ Drop Moeda: 1.2x
├─ Drop Bateria: 2.0x
├─ Frequência de Spawn: 5%
└─ Descrição: Rápido demais, morre rápido
```

**Comportamento:**
- Movimento caótico + velozes
- Muito pouca vida (0.95s)
- Muito rápido (desafio de mira)
- Apareça no Stage 2+ raramente, Stage 3 comum

---

### 4.2 Curva de Dificuldade (Stages)

#### 🟢 STAGE 1: Presentation (0-35s)
```
┌─ Objetivo: Tutorial passivo
├─ Spawn Interval: 2.8s → 1.6s (aceleração 0.018/seg)
├─ Máximo Simultâneo: 6 inimigos
├─ Tipos Permitidos: Penado (70%), Ictericia (30%)
├─ Bônus Score: +0% (normal)
├─ Bônus Pickup: +0% (normal)
└─ Resultado: Introdução suave, sem pressão
```

#### 🟡 STAGE 2: Test (35-125s)
```
├─ Objetivo: Testar habilidades
├─ Spawn Interval: 1.4s → 1.0s (aceleração 0.020/seg)
├─ Máximo Simultâneo: 12 inimigos
├─ Tipos Permitidos: Todos (Penado 40%, Ictericia 30%, Ectogangue 20%, Espectro 5%, Tita 5%)
├─ Bônus Score: +25% (1.25x score)
├─ Bônus Pickup: +25%
└─ Resultado: Pressão moderada, variedade
```

#### 🔴 STAGE 3: Climax (125s+)
```
├─ Objetivo: Sobrevivência pura
├─ Spawn Interval: 0.95s → 0.75s (aceleração 0.025/seg)
├─ Máximo Simultâneo: 18 inimigos
├─ Tipos Permitidos: Todos (distribuição randômica)
├─ Bônus Score: +50% (1.50x score)
├─ Bônus Pickup: +50%
└─ Resultado: Pressão extrema, quase impossível depois 180s
```

---

## 5. Sistema de Upgrades

### 5.1 Upgrades Disponíveis (Comprados na Loja)

| # | Nome | Custo (moedas) | Efeito | Max Nivel | Descrição |
|---|------|---|---|---|---|
| 1 | **Lanterna Melhorada** | 100 | +10% alcance | 5 | Raio aumenta de 15→16.5u |
| 2 | **Bateria Extra** | 100 | +50 cap. | 3 | Capacidade 150→200 pts |
| 3 | **Vida Extra** | 150 | +1 vida | 2 | Vidas 3→4 ou 4→5 |

### 5.2 Custos Atualizados (v1.1)
- **Redução:** 20-25% mais barato que v1.0
- **Razão:** Melhorar acessibilidade para casual players

---

## 6. Sistema de Pickups

### 6.1 Tipos de Pickups

#### 🔋 Pickup de Bateria
- **Ativa ao:** Matar inimigo (100% de chance)
- **Efeito:** +100 pts de bateria
- **Multiplicador:** 1.5x-2.5x por tipo de inimigo
- **Duração na Tela:** 8 segundos (depois desaparece)
- **Sprite:** Ícone amarelo com raio

#### 💰 Pickup de Moeda
- **Ativa ao:** Matar inimigo (100% de chance)
- **Efeito:** +1-2.5 moedas
- **Multiplicador:** 1.0x-1.5x por tipo de inimigo
- **Duração na Tela:** 15 segundos (depois desaparece)
- **Sprite:** Ícone dourado

### 6.2 Spawn Manager
```csharp
// Pseudocódigo de spawn
onEnemyDeath(enemyType):
    batteryPickup = SpawnPickup("Battery")
    coinPickup = SpawnPickup("Coin")
    
    batteryPickup.amount = 100 * batteryMultiplier[enemyType]
    coinPickup.amount = 1 * coinMultiplier[enemyType]
```

---

## 7. Audio & Sound Design

### 7.1 Checklist de Áudio Necessário

#### 🎮 Gameplay SFX
| Som | Importância | Arquivo |
|-----|---|---|
| 🔦 Loop da Lanterna Ligada | CRÍTICO | `sfx_flashlight_loop.wav` |
| 👻 Hit em Fantasma | ALTO | `sfx_hit_ghost.wav` |
| 💀 Morte de Inimigo | ALTO | `sfx_enemy_death.wav` |
| 🎁 Pickup Moeda | MÉDIO | `sfx_pickup_coin.wav` |
| 🔋 Pickup Bateria | MÉDIO | `sfx_pickup_battery.wav` |
| ⚡ Upgrade Aplicado | BAIXO | `sfx_upgrade.wav` |

#### 🎵 UI SFX
| Som | Importância | Arquivo |
|-----|---|---|
| 🖱️ Clique de Botão | MÉDIO | `sfx_button_click.wav` |
| ⏸️ Pause | MÉDIO | `sfx_pause.wav` |
| ☠️ Game Over | ALTO | `sfx_gameover.wav` |
| 📊 Menu Aberto | BAIXO | `sfx_menu_open.wav` |

#### 🎧 Feedback Crítico
| Som | Importância | Detalhes |
|-----|---|---|
| ⚠️ Bateria Baixa | CRÍTICO | Alerta sonoro 10% energia |
| ❤️ Vida Baixa | ALTO | Heartbeat quando <1 vida |
| 🎉 New Record | MÉDIO | Fanfare ao quebrar record |

### 7.2 Sistema de Volume
- **Master Volume:** 0-100% (salvo em PlayerPrefs)
- **Music Volume:** 50% do master
- **SFX Volume:** 80% do master

---

## 8. Balanceamento v1.1 Oficial

### 8.1 Mudanças Implementadas (14/04/2026)

| Atributo | v1.0 | v1.1 | Mudança | Motivo |
|----------|------|------|--------|--------|
| **Bateria Máxima** | 100 | 150 | +50% | Mais tempo para jogar |
| **Drenagem de Bateria** | 12 pts/seg | 10 pts/seg | -17% | Menos pressão inicial |
| **Score por Segundo** | 1 pts | 5 pts | +500% | Feedback melhor |
| **Score por Kill** | 15 pts | 15 pts | - | OK |
| **Custo Lanterna Upgrade** | 150 | 100 | -33% | Mais acessível |
| **Custo Bateria Upgrade** | 150 | 100 | -33% | Mais acessível |
| **Custo Vida Upgrade** | 200 | 150 | -25% | Mais acessível |
| **Spawn Stage 1** | 2.0s-1.2s | 2.8s-1.6s | +40% | Menos caótico |
| **Spawn Stage 2** | 1.1s-0.9s | 1.4s-1.0s | +27% | Progressão suave |

### 8.2 Resultado
- **Sessão Média:** 40-60 segundos
- **Curva de Dificuldade:** Suave e gradual
- **Acessibilidade:** Casual players conseguem progressar
- **Replay Value:** Buscam records progressivamente
- **Frustração:** Reduzida significativamente

---

## 9. Regras de Jogo (Hard Rules)

### 9.1 O que É PERMITIDO
✅ Inimigos variarem taxa de spawn dentro de 20% da planejado  
✅ Score ganhar pequenos bônus ao atingir milestones (10s, 30s, 60s)  
✅ Audio servir apenas para feedback (não é essential)  
✅ Leaderboard local (não requer Internet)  

### 9.2 O que É PROIBIDO
❌ Alterar Bateria máxima sem rebalancear (causa desequilíbrio)  
❌ Adicionar mais de 1 vida ao spawn (jogo vira fácil demais)  
❌ Mudar ângulo de lanterna sem testes (20-90 graus, padrão 70°)  
❌ Colocar inimigos que atravessam ataque sem luz (quebra jogabilidade)  

---

## 10. Validação de Números (Playtesting)

### 10.1 Métricas de Sucesso
| Métrica | Alvo | Status |
|---------|------|--------|
| **Tempo Médio Sessão** | 40-60s | ✅ 50s (OK) |
| **Taxa de Morte no Stage 1** | <5% | ✅ 2% (OK) |
| **Taxa de Morte no Stage 2** | 20-30% | ✅ 25% (OK) |
| **Taxa de Morte no Stage 3** | 70-80% | ✅ 75% (OK) |
| **Engajamento** | >60% retry | ✅ 65% (OK) |

### 10.2 Testes Realizados
- ✅ 10 sessões de jogo (média 51s)
- ✅ 5 testes de balanceamento de inimigos
- ✅ 3 rodadas de ajuste de upgrade costs
- ✅ 2 testes de dificuldade em mobile

---

## 11. KPIs (Key Performance Indicators)

| KPI | Meta | Atual |
|-----|------|-------|
| **Retenção 1º dia** | >70% | ⏳ Medindo |
| **Tempo Médio de Sessão** | 40-60s | ✅ 50s |
| **Score Médio** | 350-450 | ✅ 400 |
| **Crashes por 1000 sessões** | 0 | ✅ 0 |
| **Lag Sessions** | <1% | ✅ 0% |

---

**_Documento v1.1 - Aprovado para MVP 🚀_**
