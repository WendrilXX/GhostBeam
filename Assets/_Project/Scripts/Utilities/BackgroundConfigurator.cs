using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GhostBeam.Utilities
{
    /// <summary>
    /// Configura automaticamente o background e iluminação da cena Gameplay
    /// para mostrar melhor o fundo da floresta.
    /// </summary>
    public class BackgroundConfigurator : MonoBehaviour
    {
        [SerializeField] private Sprite backgroundImage;
        [SerializeField] private float globalLightIntensity = 1.3f;  // Aumentado para 1.3 (muito mais claro)

        private void Start()
        {
            ConfigureBackground();
            ConfigureGlobalLight();
        }

        private void ConfigureBackground()
        {
            // Procura Background existente
            Transform backgroundTransform = transform.parent?.Find("Background");
            
            if (backgroundTransform == null)
            {
                // Se não existir, cria um novo
                GameObject backgroundObj = new GameObject("Background");
                backgroundObj.transform.SetParent(transform.parent);
                backgroundObj.transform.position = Vector3.zero;
                
                SpriteRenderer spriteRenderer = backgroundObj.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder = -10;
                
                if (backgroundImage != null)
                {
                    spriteRenderer.sprite = backgroundImage;
                    
                    // Remove pixelação: configura filter para Bilinear
                    if (backgroundImage.texture != null)
                    {
                        backgroundImage.texture.filterMode = FilterMode.Bilinear;
                    }
                    
                    // Calcular escala para preencher a tela mantendo proporção
                    Camera mainCamera = Camera.main;
                    if (mainCamera != null && mainCamera.orthographic)
                    {
                        // Dimensões da câmera em unidades de mundo
                        float cameraHeight = mainCamera.orthographicSize * 2f;
                        float cameraWidth = cameraHeight * mainCamera.aspect;
                        
                        // Dimensões do sprite em unidades de mundo
                        Sprite sprite = backgroundImage;
                        float spriteWidth = sprite.bounds.size.x;
                        float spriteHeight = sprite.bounds.size.y;
                        
                        // Escala necessária para cobrir a tela
                        float scaleX = cameraWidth / spriteWidth;
                        float scaleY = cameraHeight / spriteHeight;
                        float scale = Mathf.Max(scaleX, scaleY);  // Usa o maior para garantir cobertura
                        
                        backgroundObj.transform.localScale = new Vector3(scale, scale, 1f);
                        Debug.Log($"[BackgroundConfigurator] Background scale: {scale:F2}x (camera {cameraWidth:F2}x{cameraHeight:F2}, sprite {spriteWidth:F2}x{spriteHeight:F2})");
                    }
                    
                    Debug.Log($"[BackgroundConfigurator] Background sprite configurado: {backgroundImage.name}");
                }
                else
                {
                    // Fallback: cria um quad colorido como antes
                    spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Square");
                    spriteRenderer.color = new Color(0.12f, 0.12f, 0.2f, 1f); // Azul escuro
                    backgroundObj.transform.localScale = new Vector3(100, 100, 1);
                    Debug.LogWarning("[BackgroundConfigurator] Background image não atribuído! Usando fallback cor.");
                }
            }
            else
            {
                // Se existir, apenas atualiza o sprite
                SpriteRenderer spriteRenderer = backgroundTransform.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && backgroundImage != null)
                {
                    spriteRenderer.sprite = backgroundImage;
                    
                    // Remove pixelação: configura filter para Bilinear
                    if (backgroundImage.texture != null)
                    {
                        backgroundImage.texture.filterMode = FilterMode.Bilinear;
                    }
                    
                    // Recalcular escala
                    Camera mainCamera = Camera.main;
                    if (mainCamera != null && mainCamera.orthographic)
                    {
                        float cameraHeight = mainCamera.orthographicSize * 2f;
                        float cameraWidth = cameraHeight * mainCamera.aspect;
                        
                        Sprite sprite = backgroundImage;
                        float spriteWidth = sprite.bounds.size.x;
                        float spriteHeight = sprite.bounds.size.y;
                        
                        float scaleX = cameraWidth / spriteWidth;
                        float scaleY = cameraHeight / spriteHeight;
                        float scale = Mathf.Max(scaleX, scaleY);
                        
                        backgroundTransform.localScale = new Vector3(scale, scale, 1f);
                    }
                    
                    Debug.Log($"[BackgroundConfigurator] Background sprite atualizado: {backgroundImage.name}");
                }
            }
        }

        private void ConfigureGlobalLight()
        {
            // Procura Global Light 2D
            Light2D globalLight = FindFirstObjectByType<Light2D>();
            
            if (globalLight != null && globalLight.lightType == Light2D.LightType.Global)
            {
                globalLight.intensity = globalLightIntensity;
                Debug.Log($"[BackgroundConfigurator] Global Light 2D configurado com intensidade: {globalLightIntensity}");
            }
            else
            {
                Debug.LogError("[BackgroundConfigurator] Global Light 2D não encontrada! Crie manualmente na scene.");
            }
        }
    }
}
