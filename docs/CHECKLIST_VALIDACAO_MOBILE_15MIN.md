# Ghost Beam - Checklist de Validacao Mobile (15 min)

Objetivo: validar rapidamente se a build mobile esta pronta para seguir para balanceamento fino ou release de validacao.

Como usar:
- Execute na build real do dispositivo (nao apenas no editor).
- Marque cada item como OK ou FALHOU.
- Se qualquer item critico falhar, corrigir antes da proxima rodada.

## 1) Preparacao (2 min)

- [ ] Dispositivo em modo horizontal e brilho acima de 60%.
- [ ] Build correta instalada (anotar versao abaixo).
- [ ] Som ativado no dispositivo para validar SFX e trilha.
- [ ] FPS overlay ativado nas configuracoes do jogo.

Versao testada:
- Data:
- Plataforma: Android / iOS
- Device:
- Build:

## 2) Fluxo principal de telas (4 min)

### 2.1 Menu -> Gameplay
- [ ] Botao Jogar inicia run sem travar e sem painel residual.
- [ ] HUD aparece com vida, bateria, score, moedas, tempo e fase.

### 2.2 Gameplay -> Game Over
- [ ] Game over dispara ao perder condicao vital (vida/bateria conforme regra atual).
- [ ] Painel de game over mostra score final e recorde corretos.

### 2.3 Game Over -> Menu
- [ ] Botao de retorno/reinicio volta ao menu corretamente.
- [ ] Painel de game over nao fica preso ao voltar para menu.

## 3) Controle e UX mobile (3 min)

- [ ] Joystick esquerdo responde sem jitter e sem deadzone excessiva.
- [ ] Area direita de mira responde com arraste continuo.
- [ ] Pausa no topo direito nao conflita com gesto de mira.
- [ ] Safe area e HUD estao legiveis em toda tela (sem cortar textos).

## 4) Gameplay e feedback (3 min)

- [ ] Inimigos spawnam em progressao de dificuldade esperada.
- [ ] Drop de moeda/pilha ocorre com frequencia coerente.
- [ ] Coleta mostra popup (+moeda/+bateria) e toca SFX correto.
- [ ] Loja aplica upgrades e efeito e perceptivel na run seguinte.

## 5) Performance curta (3 min)

- [ ] Sessao de 3 minutos sem travamento.
- [ ] FPS medio >= 30 durante picos de inimigos.
- [ ] Sem stutter grave ao coletar drops em combate.
- [ ] LOAD do spawn adaptativo nao fica saturado por tempo prolongado.

## 6) Resultado da rodada (ate 1 min)

Status final:
- [ ] APROVADO para continuar balanceamento
- [ ] REPROVADO (corrigir antes da proxima build)

Falhas criticas encontradas:
1.
2.
3.

Ajustes recomendados para proxima rodada:
1.
2.
3.

## Criterio de aprovacao minima

Todos os itens abaixo precisam estar OK:
- Fluxo completo: menu -> gameplay -> game over -> menu.
- Controle mobile funcional (movimento e mira) sem conflito grave.
- HUD legivel (16:9 e 20:9) sem elementos cortados.
- Sem crash ou travamento durante a sessao curta.

Se algum item acima falhar, classificar a build como REPROVADA.