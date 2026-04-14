# Enemy Prefabs Setup

Use esta pasta para manter prefabs de inimigos separados por arquétipo.

Sugestao de nomes:
- Enemy_Base.prefab
- Enemy_Ictericia.prefab
- Enemy_Ectogangue.prefab
- Enemy_Tita.prefab
- Enemy_Espectro.prefab

Configuracao no SpawnManager:
- Base Enemy Prefab -> Enemy_Base
- Ictericia Prefab -> Enemy_Ictericia
- Ectogangue Prefab -> Enemy_Ectogangue
- Tita Prefab -> Enemy_Tita
- Espectro Prefab -> Enemy_Espectro

Atalho no Inspector (menu de contexto do componente SpawnManager):
- Enemy Setup/Populate Empty Archetypes With Base
- Enemy Setup/Validate Archetype Prefabs

Observacao:
- Se um prefab especifico nao estiver definido, o SpawnManager usa o Base Enemy Prefab como fallback.
