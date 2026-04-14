# Ghost Beam - Instrucoes para o Agente de IA

## Sobre o Projeto
Ghost Beam e um jogo 2D top-down de sobrevivencia feito em Unity 6 com URP 2D.
O jogador usa uma lanterna para iluminar e derrotar inimigos fantasmas em uma floresta noturna.

## Regras Criticas (nao violar)

### Input System
- O projeto usa o INPUT MANAGER CLASSICO (nao o novo Input System)
- Sempre usar: `Input.GetAxis("Horizontal")`, `Input.GetAxis("Vertical")`, `Input.mousePosition`
- NUNCA usar: `InputSystem`, `InputAction`, `PlayerInput`

### Iluminacao
- Sistema: Unity URP 2D Lighting
- A lanterna e um `Light 2D` do tipo `Spot` (nao Point, nao direcional)
- Global Light 2D tem intensidade 0 (cena escura)
- A direcao correta da lanterna e `flashlight.up` no world space

### Deteccao de Luz nos Inimigos
- NAO usar Physics colliders para detectar a luz
- Usar calculo matematico: distancia + angulo
- Codigo correto:
```csharp
Vector2 toEnemy = (Vector2)(transform.position - flashlight.position);
float distance = toEnemy.magnitude;
float angle = Vector2.Angle(flashlight.up, toEnemy);
if (distance < 15f && angle < 35f) { /* iluminado */ }
```

### Hierarquia de Objetos
```
Luna
  └── Flashlight   <-- filho da Luna, posicao local 0,0,0
```
- A Flashlight SEMPRE deve ser filha da Luna
- Nunca mover a Flashlight para fora da Luna

### Fisica
- Usar apenas `Vector2.MoveTowards` para movimento de inimigos
- NAO usar Rigidbody2D nos inimigos (causa comportamento imprevisivel com a luz)
- Colisoes de dano: usar Collider2D com `Is Trigger` e `OnTriggerEnter2D`

## Padroes de Codigo

### Nomenclatura
- Scripts: PascalCase — `EnemyController`, `SpawnManager`
- Variaveis publicas: camelCase — `public float speed`
- Variaveis privadas: camelCase com prefixo — `private float lightTimer`

### Estrutura de Scripts
```csharp
using UnityEngine;

public class NomeDoScript : MonoBehaviour
{
    // Variaveis publicas (aparecem no Inspector)
    public Transform target;
    public float speed = 1.5f;

    // Variaveis privadas
    private float timer = 0f;

    void Update()
    {
        // logica aqui
    }
}
```

## Arquivos de Referencia
- `README.md` — visao geral e estrutura da cena
- `GUIA_IMPLEMENTACAO_UNITY.md` — documento consolidado (escopo, processos feitos e processos para fazer)
- `ROADMAP.md` — priorizacao do que ja foi feito e proximos passos

## Padrao de Documentacao
- Manter apenas a trilha oficial: `README.md`, `GUIA_IMPLEMENTACAO_UNITY.md`, `ROADMAP.md`.
- Preferir termos padronizados: mobile, PC, HUD, Game Over, score, highscore.
- Evitar criar documentos paralelos sem necessidade.

## O que NAO fazer
- Nao usar NavMesh (o projeto e simples, usar MoveTowards)
- Nao usar Animator Controller por enquanto (animacoes simples por script)
- Nao adicionar fisicas complexas (Rigidbody com gravidade, joints)
- Nao mudar a hierarquia Luna/Flashlight
- Nao usar co-rotinas sem necessidade (preferir Update com timer)
