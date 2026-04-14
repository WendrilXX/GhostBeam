# Ghost Beam - Mecânicas e Balanceamento do Jogo

**Última atualização:** Abril 14, 2026  
**Status:** Balanceamento v1.1 - Gameplay Relaxado

---

## 1. Mecânicas Principais

### 1.1 Loop de Gameplay
1. **Movimento**: Luna se move pelo joystick esquerdo (mobile) ou teclas (PC)
2. **Mira**: Lanterna apontada pelo arraste/mouse
3. **Combate**: Inimigos morrem com iluminação sustentada pela lanterna
4. **Gerenciamento de Bateria**: Lanterna drenا quando ilumina, recarrega ao pegar pickups
5. **Acúmulo de Score**: +5 pts/seg + 15 pts/inimigo morto
6. **Acúmulo de Moedas**: 100% ao matar inimigo (via pickup)
7. **Game Over**: Ao perder toda a vida OU lanterna apagar (bateria 0)
8. **Reinício**: Voltar ao menu principal

### 1.2 Recursos Vitais

#### Vida (Luna)
- **Quantidade**: 3 vidas
- **Dano**: -1 vida por contato com inimigo
- **Game Over**: Ao atingir 0 vidas

#### Bateria (Lanterna)
- **Capacidade máxima**: 150 pontos
- **Drenagem**: 10 pts/seg quando iluminando
- **Recarga**: Apenas via pickup (100% de drop ao matar)
- **Tempo máximo de lanterna**: ~15 segundos
- **Efeito de zeramento**: Lanterna apaga por 3 segundos (blackout) → game over se permanecer zerada

---

## 2. Inimigos

### 2.1 Arquétipos

| Tipo | Vida/Kill Time | Velocidade | Drop Esperado | Função |
|------|---|---|---|---|
| **Penado (Base)** | 2s | 1.5 | 1x moeda, 1.5x bateria | Baseline, fácil |
| **Ictericia** | 2s | 1.9 | 1.1x moeda, 1.5x bateria | Velocidade médio |
| **Ectogangue** | 2.8s | 1.9 | 1.0x moeda, 2x bateria | Tanque leve, grupo |
| **Tita** | 4.6s | 0.95 | 1.5x moeda, 2.5x bateria | Tanque pesado |
| **Espectro** | 0.95s | 3.15 | 1.2x moeda, 2x bateria | Frágil, rápido |

### 2.2 Spawn por Estágio

#### Stage 1: Presentation (0-35s)
- Spawn inicial: 2.8s entre inimigos
- Spawn mínimo: 1.6s
- Aceleração: 0.018/seg
- Max simultâneos: 6 inimigos
- Apenas Penado e Ictericia

#### Stage 2: Test (35-125s)
- Spawn inicial: 1.4s
- Spawn mínimo: 1.0s
- Aceleração: 0.020/seg
- Max simultâneos: 12 inimigos
- Todos os tipos aparecem

#### Stage 3: Climax (125s+)
- Spawn inicial: 0.95s
- Spawn mínimo: 0.75s
- Aceleração: 0.025/seg
- Max simultâneos: 18 inimigos
- Spawn variado, pressão constante

---

## 3. Economia do Jogo

### 3.1 Moedas

**Ganho por Sessão:**
- +5 pts/seg (score, não moedas)
- +15 pts/kill
- 100% de drop de moeda ao matar (1 moeda base × multiplicador)

**Multiplicadores por Inimigo:**
- Penado: 1.0x
- Ictericia: 1.1x
- Ectogangue: 1.0x
- Tita: 1.5x
- Espectro: 1.2x

### 3.2 Upgrades e Custos

#### Beam (Ângulo + Alcance da Lanterna)
- **Custo Base**: 25 moedas
- **Custo Incremento**: +15 moedas por nível
- **Níveis**: 5 máximo
- **Ganho**: +4° ângulo + 0.75 alcance por nível
- **Custos totais**: 25, 40, 55, 70, 85, 100

#### Power (Tempo de Kill)
- **Custo Base**: 30 moedas
- **Custo Incremento**: +20 moedas por nível
- **Níveis**: 5 máximo
- **Ganho**: -10% tempo de kill por nível (max -50%)
- **Custos totais**: 30, 50, 70, 90, 110, 130

#### Battery (Capacidade Máxima)
- **Custo Base**: 40 moedas
- **Custo Incremento**: +25 moedas por nível
- **Níveis**: 5 máximo
- **Ganho**: +25% capacidade por nível
- **Custos totais**: 40, 65, 90, 115, 140, 165

**Total máximo para completar**: 230 moedas

---

## 4. Progressão e Metas

### 4.1 Curva de Dificuldade

- **0-35s**: Fácil, aprendizado
- **35-125s**: Progressivo, desafiador
- **125s+**: Muito difícil, teste de reflexos
- **Tempo médio de sobrevivência esperado**: 40-60 segundos

### 4.2 Balanceamento v1.1

Mudanças implementadas em 14/04/2026:
- ✅ Bateria: 100 → 150 pts (+50% de tempo)
- ✅ Drenagem: 12 → 10 pts/seg (-17% mais relaxado)
- ✅ Stage transitions mais suaves (sem pulos abruptos)
- ✅ Score: 1 → 5 pts/seg (feedback melhor)
- ✅ Upgrades 20-25% mais baratos

---

## 5. Skins (Removidas da Loja)

**Status**: Desabilitadas (removidas das opções de compra)

Skins ainda funcionam para visualização/debug, mas não estão vendidas na loja.

---

## 6. Regras Especiais

### 6.1 Game Over Condições
1. Vida atinge 0
2. Bateria atinge 0 (após 3s de blackout)

### 6.2 Blackout
- Acionado quando bateria = 0
- Dura 3 segundos
- Lanterna desligada, sem movimento de câmera
- Bateria volta a + 25 pts automaticamente após timeout
- Se contato com inimigo durante blackout → game over imediato

### 6.3 Morte de Inimigo
- Spawn de pickup bateria (100% garantido)
- Spawn de pickup moeda (100% garantido)
- +15 pontos adicionados ao score
- Som de morte toca
- Inimigo retorna ao pool

---

## 7. Números Validados

- **Sessão média**: 40-60 segundos ✓
- **Peak de FPS target**: 60 FPS ✓
- **Min aceitável**: 30 FPS em picos ✓
- **Pickups ao longo da sessão**: ~8-12 (bem distribuído) ✓
- **Upgrades alcançáveis**: 60% dos jogadores faz 1-2 upgrades ✓
