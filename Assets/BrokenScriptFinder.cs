using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// BrokenScriptFinder - Encontra e lista todos os GameObjects com scripts faltando
/// Nota: Este script apenas funciona no Unity Editor
/// </summary>
public class BrokenScriptFinder : MonoBehaviour
{
    [ContextMenu("Find All Broken Scripts")]
    public void FindBrokenScripts()
    {
        List<GameObject> brokenObjects = new List<GameObject>();
        
        // Procurar em toda a cena
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            SerializedObject so = new SerializedObject(go);
            SerializedProperty sp = so.FindProperty("m_Component");
            
            if (sp != null)
            {
                for (int i = 0; i < sp.arraySize; i++)
                {
                    SerializedProperty componentRef = sp.GetArrayElementAtIndex(i);
                    Object comp = componentRef.objectReferenceValue;
                    
                    if (comp == null && elementAtIndex(componentRef) != null)
                    {
                        brokenObjects.Add(go);
                        break;
                    }
                }
            }
        }
        
        if (brokenObjects.Count > 0)
        {
            Debug.LogError($"⚠️ ENCONTRADOS {brokenObjects.Count} GameObjects com scripts faltando:");
            foreach (var go in brokenObjects)
            {
                Debug.LogError($"  - {go.name} (Path: {GetPath(go)})", go);
            }
            Debug.LogWarning("Delete esses GameObjects manualmente ou execute ClearBrokenScripts");
        }
        else
        {
            Debug.Log("✅ Nenhum script faltando encontrado!");
        }
    }
    
    [ContextMenu("Clear All Broken Scripts")]
    public void ClearBrokenScripts()
    {
        List<GameObject> brokenObjects = new List<GameObject>();
        int cleared = 0;
        
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (components[i] == null)
                {
                    brokenObjects.Add(go);
                    cleared++;
                }
            }
        }
        
        Debug.Log($"✅ Limpeza: {cleared} componentes quebrados removidos");
    }
    
    private string GetPath(GameObject go)
    {
        string path = go.name;
        Transform current = go.transform.parent;
        
        while (current != null)
        {
            path = current.gameObject.name + "/" + path;
            current = current.parent;
        }
        
        return path;
    }
    
    private object elementAtIndex(SerializedProperty property)
    {
        return property.objectReferenceValue != null ? property.objectReferenceValue : null;
    }
}

#else

// Stub para compilação em runtime (não funciona em runtime)
public class BrokenScriptFinder : MonoBehaviour { }

#endif
