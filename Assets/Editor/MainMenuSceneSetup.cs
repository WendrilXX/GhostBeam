using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;

public class MainMenuSceneSetup
{
    [MenuItem("GhostBeam/Setup/Create Main Menu Scene")]
    public static void CreateMainMenuScene()
    {
        // Criar nova cena vazia
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // Criar Canvas Menu
        GameObject canvasObj = new GameObject("Canvas - Menu");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;

        // Adicionar UIManager
        GameObject uiManagerObj = new GameObject("UIManager");
        uiManagerObj.AddComponent<MainMenuUIBuilder>();

        // Criar EventSystem
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        // Salvar cena
        EditorSceneManager.SaveScene(newScene, "Assets/Scenes/MainMenu.unity");
        
        Debug.Log("✅ MainMenu.unity criada com sucesso!");
    }
}
#endif
