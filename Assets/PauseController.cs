using TMPro;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public TMP_Text pauseButtonLabel;
    public string pausedText = "RETOMAR";
    public string unpausedText = "PAUSAR";

    private void OnEnable()
    {
        GameManager.onPauseChanged += UpdatePauseLabel;
        UpdatePauseLabel(GameManager.Instance != null && GameManager.Instance.IsPaused);
    }

    private void OnDisable()
    {
        GameManager.onPauseChanged -= UpdatePauseLabel;
    }

    public void TogglePause()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.TogglePause();
    }

    private void UpdatePauseLabel(bool isPaused)
    {
        if (pauseButtonLabel == null)
        {
            return;
        }

        pauseButtonLabel.text = isPaused ? pausedText : unpausedText;
    }
}
