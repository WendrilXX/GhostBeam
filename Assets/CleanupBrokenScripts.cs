using UnityEngine;

/// <summary>
/// CleanupBrokenScripts - Remove GameObjects com componentes quebrados ao iniciar
/// Executa primeiro para limpar a cena de lixo antes de qualquer coisa
/// </summary>
[DefaultExecutionOrder(-1000)]
public class CleanupBrokenScripts : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("[CleanupBrokenScripts] Procurando GameObjects com scripts quebrados...");

        // Encontrar todos os GameObjects na cena
        GameObject[] allGOs = FindObjectsByType<GameObject>(FindObjectsInactive.Include);
        int brokenCount = 0;

        foreach (GameObject go in allGOs)
        {
            // Pular objetos que sabemos que devem existir
            if (go.name == "GameManager" || go.name == "UIBuilder" || go.name == "UIInitializer")
                continue;

            Component[] components = go.GetComponents<Component>();
            
            foreach (Component comp in components)
            {
                // Se component é null, significa que é um script quebrado
                if (comp == null)
                {
                    Debug.LogWarning($"[CleanupBrokenScripts] Encontrado GameObject com script quebrado: {go.name} - REMOVENDO");
                    Destroy(go);
                    brokenCount++;
                    break;
                }
            }
        }

        if (brokenCount > 0)
        {
            Debug.Log($"[CleanupBrokenScripts] Removidos {brokenCount} GameObjects com scripts quebrados");
        }
        else
        {
            Debug.Log("[CleanupBrokenScripts] Nenhum script quebrado encontrado");
        }
    }
}
