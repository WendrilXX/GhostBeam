using UnityEditor;
using TMPro;
using UnityEngine;

/// <summary>
/// Utilitário para gerar emoji font assets
/// Nota: Geração automática via código não suporta todas as configurações necessárias
/// Use o método manual para melhor controle
/// </summary>
public class EmojiAssetGenerator
{
    [MenuItem("TextMeshPro/Emoji - Open Font Asset Creator")]
    public static void OpenFontAssetCreator()
    {
        EditorUtility.DisplayDialog(
            "Font Asset Creator",
            "A janela Font Asset Creator será aberta.\n\n" +
            "1. Source Font: Selecione NotoColorEmoji.ttf\n" +
            "2. Character Set: Unicode Range (Hex)\n" +
            "3. Range: 1F300-1F9FF\n" +
            "4. Sampling Point Size: 128\n" +
            "5. Atlas Resolution: 2048\n" +
            "6. Generate Font Atlas\n" +
            "7. Save em Assets/Resources/Fonts & Materials/",
            "OK"
        );
    }

    [MenuItem("TextMeshPro/Emoji - Verify Asset Generated")]
    public static void VerifyEmojiAsset()
    {
        TMP_FontAsset emojiAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/NotoColorEmoji-Regular SDF");
        
        if (emojiAsset != null)
        {
            Debug.Log("✅ Emoji font asset encontrado: " + emojiAsset.name);
            Selection.activeObject = emojiAsset;
        }
        else
        {
            Debug.LogError("❌ Emoji font asset não encontrado em Resources/Fonts & Materials/");
        }
    }
}
