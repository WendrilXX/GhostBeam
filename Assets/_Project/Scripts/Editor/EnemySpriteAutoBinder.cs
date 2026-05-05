using UnityEditor;
using UnityEngine;

namespace GhostBeam.Editor
{
    public static class EnemySpriteAutoBinder
    {
        [MenuItem("GhostBeam/Tools/Auto Bind Enemy Sprites")]
        public static void AutoBindEnemySprites()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_Project" });
            int updated = 0;

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                    continue;

                var controller = prefab.GetComponentInChildren<Enemy.EnemyController>(true);
                if (controller == null)
                    continue;

                controller.EditorAutoFillSprites();
                EditorUtility.SetDirty(prefab);
                updated++;
            }

            if (updated > 0)
                AssetDatabase.SaveAssets();

            Debug.Log($"[EnemySpriteAutoBinder] Sprites vinculados em {updated} prefab(s). Refaça a build.");
        }
    }
}
