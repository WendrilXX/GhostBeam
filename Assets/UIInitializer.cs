using UnityEngine;

/// <summary>
/// UIInitializer - Força inicialização correta de toda a UI em tempo de execução
/// Executa com ExecuteInEditMode = false para rodar apenas em gameplay
/// Prioridade = -100 para executar ANTES de outros Awake()
/// </summary>
[DefaultExecutionOrder(-100)]
public class UIInitializer : MonoBehaviour
{
    private static bool isInitialized = false;

    private void Awake()
    {
        if (isInitialized)
            return;

        isInitialized = true;

        Debug.Log("[UIInitializer] === INICIALIZANDO UI SISTEMA ===");

        // Dar tempo para GameManager e UIBuilder completarem Awake()
        // Depois conectar tudo
        Invoke(nameof(InitializeUI), 0.01f);
    }

    private void InitializeUI()
    {
        Debug.Log("[UIInitializer] Executando inicialização atrasada...");

        // 1. Encontrar GameManager
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm == null)
        {
            Debug.LogError("[UIInitializer] ❌ GameManager não encontrado!");
            return;
        }
        Debug.Log($"[UIInitializer] ✅ GameManager: {gm.gameObject.name}, IsInMainMenu={gm.IsInMainMenu}");

        // 2. Encontrar MainMenuController
        MainMenuController menu = FindAnyObjectByType<MainMenuController>();
        if (menu == null)
        {
            Debug.LogError("[UIInitializer] ❌ MainMenuController não encontrado!");
            return;
        }
        Debug.Log($"[UIInitializer] ✅ MainMenuController: {menu.gameObject.name}");

        // 3. Verificar e reportar estado
        ReportMenuState(menu);

        // 4. Se ainda não está tudo conectado, forçar conexão
        if (menu.panelRoot == null || menu.shopPanelRoot == null)
        {
            Debug.Log("[UIInitializer] Detectado estado incompleto - Forçando reconexão...");
            ForceReconnectControllers(menu);
        }

        // 5. Forçar que menu fique visível
        menu.SetMenuVisible(true);

        Debug.Log("[UIInitializer] === INICIALIZAÇÃO COMPLETA ===");
    }

    private void ReportMenuState(MainMenuController menu)
    {
        Debug.Log("[UIInitializer] === Status Atual ===");
        Debug.Log($"  panelRoot: {(menu.panelRoot != null ? menu.panelRoot.name : "NULL")}");
        Debug.Log($"  shopPanelRoot: {(menu.shopPanelRoot != null ? menu.shopPanelRoot.name : "NULL")}");
        Debug.Log($"  settingsPanelRoot: {(menu.settingsPanelRoot != null ? menu.settingsPanelRoot.name : "NULL")}");
        Debug.Log($"  shopController: {(menu.shopController != null ? menu.shopController.gameObject.name : "NULL")}");
        Debug.Log($"  leaderboardController: {(menu.leaderboardController != null ? menu.leaderboardController.gameObject.name : "NULL")}");
        Debug.Log($"  dailyQuestsController: {(menu.dailyQuestsController != null ? menu.dailyQuestsController.gameObject.name : "NULL")}");
    }

    private void ForceReconnectControllers(MainMenuController menu)
    {
        // Procurar Canvas do menu
        Canvas[] allCanvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include);
        Canvas mainMenuCanvas = System.Array.Find(allCanvases, c => c.gameObject.name == "CanvasMainMenu");

        if (mainMenuCanvas == null)
        {
            Debug.LogError("[UIInitializer] CanvasMainMenu não encontrado!");
            return;
        }

        // Reconectar panelRoot
        if (menu.panelRoot == null)
        {
            Transform panelRoot = mainMenuCanvas.transform.Find("PanelMenu");
            if (panelRoot != null)
            {
                menu.panelRoot = panelRoot.gameObject;
                Debug.Log($"[UIInitializer] ✅ Reconectado panelRoot: {panelRoot.name}");
            }
        }

        // Reconectar shop
        ShopScreenController shopCtrl = FindAnyObjectByType<ShopScreenController>();
        if (shopCtrl != null && menu.shopPanelRoot == null)
        {
            menu.shopController = shopCtrl;
            menu.shopPanelRoot = shopCtrl.shopPanelRoot;
            Debug.Log($"[UIInitializer] ✅ Reconectado shopController e shopPanelRoot");
        }

        // Reconectar leaderboard
        LeaderboardScreenController leaderboardCtrl = FindAnyObjectByType<LeaderboardScreenController>();
        if (leaderboardCtrl != null && menu.leaderboardController == null)
        {
            menu.leaderboardController = leaderboardCtrl;
            Debug.Log($"[UIInitializer] ✅ Reconectado leaderboardController");
        }

        // Reconectar quests
        DailyQuestsScreenController questsCtrl = FindAnyObjectByType<DailyQuestsScreenController>();
        if (questsCtrl != null && menu.dailyQuestsController == null)
        {
            menu.dailyQuestsController = questsCtrl;
            Debug.Log($"[UIInitializer] ✅ Reconectado dailyQuestsController");
        }
    }
}
