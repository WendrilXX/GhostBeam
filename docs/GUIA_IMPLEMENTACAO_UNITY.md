# Ghost Beam - Guia de Desenvolvimento

Ultima atualizacao: 26/03/2026

Este documento esta organizado para acompanhamento continuo em 3 blocos:
1) Escopo do produto
2) Processos feitos
3) Processos a fazer

## 1. Escopo do Produto

### 1.1 Visao do jogo
- Jogo 2D top-down de sobrevivencia.
- Personagem principal: Luna.
- Fantasmas perseguem Luna e so podem ser eliminados com iluminacao direta da lanterna.

### 1.2 Plataformas alvo
- Mobile: Android e iOS (plataforma principal).
### 1.3 Loop principal
- Mover Luna.
- Mirar lanterna.
- Sobreviver ao spawn progressivo de inimigos.
- Acumular score por tempo e eliminacoes.
- Gerenciar bateria da lanterna.
- Morrer ao perder toda a vida.
- Com bateria zerada, a partida termina (game over por recurso vital).

### 1.4 Pilares tecnicos
- Unity 6 com URP 2D.
- Cena escura com Global Light 2D e lanterna Spot 2D como fonte principal.
- Input Manager classico (sem migracao para novo Input System).
- UI adaptavel para multiplas resolucoes (PC e mobile).

### 1.5 Estrutura de cena esperada
- Main Camera
- Global Light 2D
- Background
- Luna
  - Flashlight
- Systems
  - GameManager
  - ScoreManager
  - SpawnManager
  - BatteryPickupSpawner
- CanvasHUD
  - TxtVida
  - TxtBateria
  - TxtScore
  - TxtRecorde
- CanvasGameOver
  - PanelGameOver
    - TxtScoreFinal
    - TxtRecordeFinal
    - BtnReiniciar

## 2. Processos Feitos

### 2.1 Core de gameplay
- [x] Movimento da Luna com teclado no PC.
- [x] Rotacao da lanterna por mouse no PC.
- [x] Inimigo base segue Luna.
- [x] Inimigo morre ao ficar tempo suficiente no cone de luz.
- [x] Spawn progressivo de inimigos com limite de quantidade.

### 2.2 Loop de partida
- [x] Sistema de vida da Luna com dano por contato.
- [x] Game over ao zerar vida.
- [x] Pausa de jogo no game over.
- [x] Reinicio da cena por botao.

### 2.3 Pontuacao
- [x] Score por tempo sobrevivido.
- [x] Score por inimigo eliminado.
- [x] Highscore salvo com PlayerPrefs.
- [x] Moedas por coleta fisica no mapa via drop de inimigo.

### 2.4 Sistema de bateria
- [x] Consumo de bateria ao iluminar inimigos.
- [x] Recarga gradual quando nao esta drenando.
- [x] Apagao temporario ao zerar bateria.
- [x] Retorno da lanterna apos cooldown.
- [x] Spawn de pilhas e coleta para recarga.
- [x] Drop de pilhas por inimigo derrotado (coleta no mapa).
- [x] Atracao magnetica de pilhas/coins em curta distancia para coleta fluida no mobile.

### 2.5 HUD e Game Over
- [x] HUD com vida, bateria, score e recorde.
- [x] Tela de game over com score final e recorde final.
- [x] Botao de reiniciar ligado ao fluxo de partida.
- [x] Migracao de textos do game over para TMP.

### 2.6 Suporte PC + Mobile (implementado em codigo)
- [x] Fallback de movimento por toque no mobile.
- [x] Fallback de mira por toque no mobile.
- [x] Overlay visual mobile para controles (zona esquerda de movimento + zona direita de mira).
- [x] Safe Area para telas com notch e barras.
- [x] Canvas Scaler configurado para responsividade.

### 2.7 Automacao de configuracao de UI
- [x] Script de bootstrap para ligar referencias do HUD e Game Over por nome.
- [x] Aplicacao automatica de layout base (ancoras, tamanhos e alinhamento).
- [x] Remocao visual de placeholders New Text e Button no fluxo principal.
- [x] Build C# validado sem erros apos as mudancas.

### 2.8 Scripts existentes no projeto
- LunaController.cs
- FlashlightController.cs
- EnemyController.cs
- SpawnManager.cs
- HealthSystem.cs
- GameManager.cs
- ScoreManager.cs
- HUDController.cs
- GameOverPanelController.cs
- BatterySystem.cs
- BatteryPickup.cs
- BatteryPickupSpawner.cs
- SafeAreaFitter.cs
- UIBootstrapper.cs

## 3. Processos a Fazer

### 3.1 Alta prioridade (proxima sprint)
- [ ] Fechar validacao completa de gameplay em dispositivo mobile real.
- [x] Implementar input mobile final (joystick virtual na esquerda + area de mira dedicada na direita).
- [ ] Validar input mobile final em dispositivo real (Android/iOS).
- [ ] Revisar colliders e triggers em todos os prefabs de inimigo e Luna na cena final.
- [ ] Garantir que CanvasHUD e CanvasGameOver estao 100% consistentes em todas as resolucoes alvo.

### 3.2 Experiencia e feedback
- [x] Timer de sobrevivencia no HUD.
- [x] Feedback de inimigo iluminado (piscar/material/efeito visual).
- [x] Efeito de morte mais claro (dissolve, particulas ou flash).
- [ ] Polimento visual da UI (tipografia final, contraste, hierarquia, espacos).

### 3.3 Conteudo e progressao
- [ ] Aplicar sprite final da Luna.
- [ ] Corrigir e finalizar animacao do Penado (13 frames).
- [x] Introduzir novos tipos de inimigo conforme planejamento de progressao do roadmap.
- [x] Escalonar dificuldade por tempo (spawn, velocidade, mistura de inimigos).

### 3.4 Audio
- [x] Som de lanterna.
- [x] Som de inimigo eliminado.
- [x] Som de dano na Luna.
- [x] Trilha ambiente e tensao.

### 3.5 Performance e arquitetura
- [x] Implementar object pooling para inimigos e pickups.
- [x] Reduzir custo de buscas frequentes em runtime quando necessario.
- [x] Aplicar ajuste adaptativo de spawn com base em FPS medio (janela curta) para estabilidade mobile.
- [ ] Perfil de performance para alvo mobile intermediario.

### 3.6 Entrega
- [x] Tela de menu inicial.
- [ ] Fluxo completo de jogo (menu -> gameplay -> game over -> menu).
- [ ] Build Android de teste.
- [ ] Build iOS de teste.
- [ ] Checklist final de release.

### 3.7 Rito de acompanhamento (recomendado)
- Atualizar este documento ao final de cada sessao de trabalho.
- Mover itens concluidos de Processos a Fazer para Processos Feitos.
- Registrar brevemente bloqueios tecnicos e decisoes tomadas.

## 4. Analise Minuciosa: GDD x Implementacao

Data da analise: 26/03/2026
Fonte: GDD oficial da equipe (GDD.pdf)

Legenda de status:
- Alinhado: implementado e testavel no estado atual.
- Parcial: existe base implementada, mas falta parte funcional.
- Pendente: nao implementado.

### 4.1 Conceito geral e USP
- Alinhado: combate por cone de luz como mecanica central.
- Alinhado: objetivo de sobrevivencia e quebra de recorde.

### 4.2 Mecanicas de jogo
- Controles touch para mirar:
  - Alinhado em codigo: joystick virtual na esquerda + area de mira dedicada na direita.
  - Alinhado em UX visual: zonas de controle e indicadores em tela durante a partida.
  - Pendente de validacao: testes em dispositivo real Android/iOS.
- Botao de pausa:
  - Alinhado: pausa implementada no HUD.
- Derrota por dano/recursos vitais:
  - Alinhado: derrota por vida zero.
  - Alinhado em design: com bateria zerada, a partida encerra imediatamente.

### 4.3 Sistemas principais do GDD
- Combate por tempo de iluminacao:
  - Alinhado.
- Recursos (bateria + pilhas):
  - Alinhado.
- Pontuacao (tempo + inimigos):
  - Alinhado.
- Progressao por moedas, upgrades e skins:
  - Alinhado.

### 4.4 Game loop
- Loop micro (mirar -> iluminar -> eliminar -> coletar recompensa):
  - Alinhado.
- Loop macro (ganhar moedas -> investir -> enfrentar ondas mais dificeis):
  - Alinhado.

### 4.5 Narrativa, personagens e conteudo
- Luna + ambientacao de floresta noturna:
  - Alinhado em conceito e base visual.
- Inimigos nv2 a nv5 (Ictericia, Ectogangue, Tita, Espectro):
  - Alinhado.

### 4.6 Level design e fluxo de telas
- Fluxo de telas (Menu -> Gameplay -> Game Over -> Menu):
  - Parcial: menu existe, mas falta validacao final do fluxo em build mobile.
- Estrutura de dificuldade (apresentacao/teste/climax):
  - Alinhado.
- HUD com bateria, vida, pontos/moedas, pausa e itens:
  - Alinhado para bateria/vida/score/moedas/pausa; itens extras seguem opcionais de escopo.

### 4.7 Estetica e audio
- Direcao visual (alto contraste luz/escuridao):
  - Alinhado em base.
- SFX e trilha:
  - Alinhado em pacote minimo.

### 4.8 Planejamento tecnico do GDD
- Semana 1 e 2:
  - Alinhado.
- Semana 3 e 4:
  - Parcial: menus e audio base concluidos; polimento final e validacao mobile pendentes.

### 4.9 Checklist executavel (prioridade por impacto)

#### Bloco A - Fechar aderencia do loop principal
- [x] Implementar botao de pausa funcional no HUD.
- [x] Adicionar moedas de partida (drop ou recompensa por kill/tempo).
- [x] Exibir moedas no HUD (separado de score).
- [x] Definir regra de derrota por recurso vital (com bateria zerada, contato com mob encerra a partida).

Atualizacao Mar/2026:
- Regra vital aplicada em runtime no HealthSystem: com bateria em blackout, contato/proximidade de inimigo encerra a partida imediatamente.
- Contagens de inimigos/pickups e checagens de iluminacao migradas para registros ativos, reduzindo chamadas caras de busca global por frame.
- Drops de moeda/pilha agora aceitam ponderacao por tipo de inimigo (recompensa maior para inimigos mais perigosos).

#### Bloco B - Progressao e motivacao
- [x] Criar sistema de upgrades da lanterna (feixe, dano/tempo para eliminar).
- [x] Adicionar upgrade de bateria (duracao maxima) com persistencia.
- [x] Criar sistema de desbloqueio de skins (Luna e lanterna).
- [x] Persistir progresso (moedas e desbloqueios) em PlayerPrefs/local save.

#### Bloco C - Conteudo e dificuldade
- [x] Implementar Ictericia (nv2).
- [x] Implementar Ectogangue (nv3 em grupo).
- [x] Implementar Tita (nv4 tanque lento).
- [x] Implementar Espectro (nv5 veloz).
- [x] Ajustar curva de dificuldade por estagios: apresentacao, teste, climax.

Atualizacao Mar/2026 (GDD revisado):
- Escada macro por tempo sincronizada no SpawnManager: 0-60 (Penado), 60-120 (Ictericia), 120-180 (Ectogangue), 180-240 (Tita), 240+ (Espectro + aumento de pressao).
- Ritmo de pressao reforcado por degraus de 30s no spawn.

#### Bloco D - UX, audio e entrega
- [ ] Implementar e validar fluxo completo menu -> gameplay -> game over -> menu em build mobile.
- [x] Definir e aplicar UX mobile final de controle (joystick esquerdo + mira por arraste na direita).
- [ ] Validar UX mobile final de controle em dispositivo real.
- [x] Implementar pacote minimo de audio (lanterna, dano, kill, trilha).
- [ ] Validar em dispositivo real Android/iOS com checklist de performance.

## 5. Decisoes Consolidadas (GDD v1.1)

### 5.1 Derrota e recurso vital
- Recurso vital principal: vida.
- Regra adicional oficial: com bateria zerada, a partida termina imediatamente (game over por recurso vital).

### 5.2 Fluxo de tela oficial
- Menu Principal -> Gameplay -> Game Over -> Menu Principal.
- Nao existe tela de vitoria; o destaque e quebra de recorde durante a run.

### 5.3 Controle mobile oficial
- Metade esquerda: joystick virtual para movimento.
- Metade direita: toque e arraste para mirar o facho de luz.
- Botao de pausa no topo direito, fora das areas de gesto.

### 5.4 Balanceamento base de inimigos
- Penado (nv1): baseline de vida e velocidade.
- Ictericia (nv2): mesma vida do Penado e velocidade maior.
- Ectogangue (nv3): vida media, velocidade normal e spawn em grupo.
- Tita (nv4): vida alta e velocidade baixa.
- Espectro (nv5): vida baixa e velocidade muito alta.

### 5.5 Metas de performance mobile
- Alvo principal: 60 FPS em dispositivo intermediario.
- Minimo aceitavel em pico: 30 FPS.
- Estabilidade: 10+ minutos de sessao sem travamento e sem degradacao severa.

## 6. Distribuicao de Tarefas por Semana (Equipe)

Referencia consolidada a partir do quadro da equipe para planejamento de execucao.

### 6.1 Semana 1 - PROTOTIPO

Entregas:
- Lanterna funcionando.
- Inimigo simples.
- Mecanica de luz.

Pacotes de trabalho:
- Sistema de lanterna (luz e colisao).
- Controles touch (arrastar para mirar).
- Arte da Luna (sprite base).
- Conceito dos cenarios (floresta noturna).
- Inimigo basico Penado (IA de movimento).
- Spawn e colisao com inimigo.
- Sprite Penado (fantasma nv1).
- Exposicao ao feixe (dano ao inimigo).
- Timer de exposicao e destruicao.
- Efeito visual do feixe (iluminacao 2D).

### 6.2 Semana 2 - LOOP COMPLETO

Entregas:
- Spawn continuo.
- Morte / Game Over.
- Pontuacao.

Pacotes de trabalho:
- Sistema de waves (spawn por timer).
- Aumento progressivo de dificuldade.
- Sprites nv2 Ictericia e nv3 Ectogangue.
- Animacao de spawn dos inimigos.
- Dano ao jogador e sistema de vida.
- Tela de Game Over e reinicio.
- Tela de Game Over (layout e arte).
- Sistema de score (tempo + inimigos).
- Gerenciamento de moedas.
- UI de moedas e score.

### 6.3 Semana 3 - EXPERIENCIA

Entregas:
- HUD completo.
- Sons.
- Feedback visual.

Pacotes de trabalho:
- Integracao HUD <-> sistemas de jogo.
- Indicadores de bateria, vida e pausa.
- Layout e icones do HUD.
- Barra de bateria e vida animadas.
- Integracao de audio (Unity AudioSource).
- Efeitos sonoros (inimigos, lanterna).
- Selecao/curadoria de assets de audio.
- Particulas de destruicao de inimigo.
- Flash de dano no personagem.
- Efeito de morte (particulas/sprite).

### 6.4 Semana 4 - POLIMENTO

Entregas:
- Ajuste de dificuldade.
- Performance.
- Build final.

Pacotes de trabalho:
- Balanceamento de waves e stats.
- Testes de equilibrio e ajuste de valores.
- Sprites nv4 Tita e nv5 Espectro.
- Revisao geral de arte/UI.
- Object pooling e otimizacao.
- Profiling e correcao de bugs.
- Compressao de sprites.
- Build Android (APK/AAB).
- Testes finais em dispositivo.
- Integracao final e entrega.
- Icone do app e splash screen.
- Screenshots para entrega.

## 7. Pacote Pronto para Implementacao (Proxima Rodada)

Objetivo: manter avancando com foco em entrega mobile sem depender de arte final.

### 7.1 Fluxo de entrega mobile
- [ ] Validar fluxo completo em dispositivo real: Menu -> Gameplay -> Game Over -> Menu.
- [ ] Rodar checklist de orientacao horizontal, safe area e escala de HUD em 16:9 e 20:9.
- [ ] Fechar build Android de validacao e registrar versao testada.
- Referencia rapida de execucao: CHECKLIST_VALIDACAO_MOBILE_15MIN.md.

### 7.2 Balanceamento e performance
- [ ] Rodar sessao de 10+ minutos com overlay de performance ligado.
- [ ] Ajustar thresholds de adaptacao de spawn (low/high FPS, slowdown, cap reduction).
- [ ] Revisar chances de drop de moeda/pilha por arquétipo para manter progressao consistente.

### 7.3 UX e feedback de coleta
- [x] Adicionar feedback visual de coleta (+moeda/+bateria) proximo da Luna.
- [x] Adicionar SFX dedicado para coleta de moeda e pilha.
- [ ] Validar legibilidade dos controles visuais (joystick esquerdo e direito) em telas pequenas.

Atualizacao Mar/2026:
- Fluxo de game over/menu reforcado em runtime: painel de game over responde melhor as transicoes para menu.
