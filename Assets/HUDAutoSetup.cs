using UnityEngine;
using TMPro;

public class HUDAutoSetup : MonoBehaviour
{
    private void Start()
    {
        HUDController hudController = GetComponent<HUDController>();
        if (hudController == null) return;

        // Encontra todos os TMP_Text filhos
        TMP_Text[] allTexts = GetComponentsInChildren<TMP_Text>(true);

        // Atribui cada um ao campo correto baseado no nome
        foreach (TMP_Text text in allTexts)
        {
            switch (text.name)
            {
                case "TxtVida":
                    hudController.healthText = text;
                    break;
                case "TxtScore":
                    hudController.scoreText = text;
                    break;
                case "TxtRecorde":
                    hudController.highscoreText = text;
                    break;
                case "TxtBateria":
                    hudController.batteryText = text;
                    break;
                case "TxtMoedas":
                    hudController.coinsText = text;
                    break;
                case "TxtTempo":
                    hudController.survivalTimeText = text;
                    break;
                case "TxtFase":
                    hudController.stageText = text;
                    break;
                case "TxtPerf":
                    hudController.performanceText = text;
                    break;
            }
        }

        // HUD conectado automaticamente
    }
}
