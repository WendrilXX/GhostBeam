using UnityEditor;
using UnityEngine;

namespace GhostBeam.Editor
{
    public class PreSetupValidation
    {
        [MenuItem("GhostBeam/Validate/Pre-Setup Check")]
        public static void ValidateSetup()
        {
            Debug.Log("========== PRE-SETUP VALIDATION ==========\n");

            bool allGood = true;

            // 1. Check Editor Scripts exist
            Debug.Log("🔍 Checking editor scripts...");
            if (!CheckScriptExists("AutoSetupScene"))
            { Debug.LogError("❌ AutoSetupScene.cs MISSING!"); allGood = false; }
            else Debug.Log("✅ AutoSetupScene.cs found");

            if (!CheckScriptExists("AutoCreatePrefabsAdvanced"))
            { Debug.LogError("❌ AutoCreatePrefabsAdvanced.cs MISSING!"); allGood = false; }
            else Debug.Log("✅ AutoCreatePrefabsAdvanced.cs found");

            if (!CheckScriptExists("AutoLinkReferences"))
            { Debug.LogError("❌ AutoLinkReferences.cs MISSING!"); allGood = false; }
            else Debug.Log("✅ AutoLinkReferences.cs found");

            if (!CheckScriptExists("AutoConfigureBuild"))
            { Debug.LogError("❌ AutoConfigureBuild.cs MISSING!"); allGood = false; }
            else Debug.Log("✅ AutoConfigureBuild.cs found");

            // 2. Check folders exist
            Debug.Log("\n🔍 Checking folders...");
            if (!CheckFolderExists("Assets/_Project/Prefabs"))
            { Debug.Log("ℹ️ Prefabs folder will be created automatically"); }
            else Debug.Log("✅ Prefabs folder exists");

            if (!CheckFolderExists("Assets/_Project/Scenes"))
            { Debug.LogError("❌ Scenes folder MISSING!"); allGood = false; }
            else Debug.Log("✅ Scenes folder exists");

            // 3. Check important classes
            Debug.Log("\n🔍 Checking core classes...");
            if (!CheckTypeExists("GhostBeam.Managers.GameManager"))
            { Debug.LogError("❌ GameManager class MISSING!"); allGood = false; }
            else Debug.Log("✅ GameManager class found");

            if (!CheckTypeExists("GhostBeam.Player.LunaController"))
            { Debug.LogError("❌ LunaController class MISSING!"); allGood = false; }
            else Debug.Log("✅ LunaController class found");

            if (!CheckTypeExists("GhostBeam.Enemy.EnemyController"))
            { Debug.LogError("❌ EnemyController class MISSING!"); allGood = false; }
            else Debug.Log("✅ EnemyController class found");

            // 4. Check compilation
            Debug.Log("\n🔍 Checking compilation...");
            if (EditorApplication.isCompiling)
            {
                Debug.LogWarning("⏳ Unity is still compiling... Wait a moment and try again.");
                allGood = false;
            }
            else
            {
                Debug.Log("✅ Compilation is clean");
            }

            // 5. Final status
            Debug.Log("\n" + new string('=', 50));
            if (allGood)
            {
                Debug.Log("✅ ✅ ✅ ALL CHECKS PASSED! ✅ ✅ ✅");
                Debug.Log("\n🚀 Ready to run: GhostBeam > Setup > 5. Setup Complete (Full Auto)");
            }
            else
            {
                Debug.LogError("❌ Some checks failed. Please fix the issues above.");
            }
            Debug.Log("==========================================\n");
        }

        private static bool CheckScriptExists(string scriptName)
        {
            string[] guids = AssetDatabase.FindAssets(scriptName + " t:Script");
            return guids.Length > 0;
        }

        private static bool CheckFolderExists(string folderPath)
        {
            return System.IO.Directory.Exists(folderPath);
        }

        private static bool CheckTypeExists(string fullTypeName)
        {
            try
            {
                System.Type.GetType(fullTypeName, false, true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
