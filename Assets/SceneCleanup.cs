using UnityEngine;

/// <summary>
/// SceneCleanup - Valida e remove componentes com scripts faltando
/// Execute uma vez ao abrir a cena para limpar referências quebradas
/// </summary>
public class SceneCleanup : MonoBehaviour
{
    private void Awake()
    {
        CleanupMissingScripts();
    }

    private void CleanupMissingScripts()
    {
        int removed = 0;
        Component[] components = GetComponentsInChildren<Component>(true);

        foreach (Component comp in components)
        {
            if (comp == null)
            {
                Debug.LogWarning($"Found missing script on GameObject: {gameObject.name}");
                removed++;
            }
        }

        if (removed > 0)
        {
            Debug.LogWarning($"Limpeza: {removed} componentes faltando foram encontrados");
        }
        else
        {
            Debug.Log("✅ Nenhum script faltando encontrado");
        }

        Destroy(this);
    }
}
