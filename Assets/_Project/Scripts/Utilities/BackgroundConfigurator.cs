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
        [SerializeField] private float globalLightIntensity = 0.1f;  // Muito baixo - iluminação vem da lanterna
        [SerializeField] private Color globalLightColor = new Color(0.8f, 0.8f, 1f);  // Azul suave
        [SerializeField] private float maxBackgroundScale = 2.5f;  // Permite mais escala para usar mais da tela

        private void Start()
        {
            ConfigureBackground();
            ConfigureShadowOverlay();
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
                    spriteRenderer.color = Color.white;  // Sem brilho extra, mantém cor original
                    
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
                        scale = Mathf.Clamp(scale, 1f, maxBackgroundScale);  // Limita entre 1x e máximo
                        
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
                    spriteRenderer.color = Color.white;  // Sem brilho extra, mantém cor original
                    
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
                        float scale = Mathf.Max(scaleX, scaleY);  // Usa o maior para garantir cobertura
                        scale = Mathf.Clamp(scale, 1f, maxBackgroundScale);  // Limita entre 1x e máximo
                        
                        backgroundTransform.localScale = new Vector3(scale, scale, 1f);
                    }
                    
                    Debug.Log($"[BackgroundConfigurator] Background sprite atualizado: {backgroundImage.name}");
                }
            }
        }

        private void ConfigureShadowOverlay()
        {
            // Procura Shadow Overlay existente
            Transform shadowTransform = transform.parent?.Find("ShadowOverlay");
            
            if (shadowTransform == null)
            {
                // Cria novo shadow overlay
                GameObject shadowObj = new GameObject("ShadowOverlay");
                shadowObj.transform.SetParent(transform.parent);
                shadowObj.transform.position = Vector3.zero;
                
                SpriteRenderer shadowRenderer = shadowObj.AddComponent<SpriteRenderer>();
                shadowRenderer.sortingOrder = -5;  // Entre fundo (-10) e gameplay (0)
                
                // Cria um quad branco simples como sprite
                Sprite whiteSquare = Resources.Load<Sprite>("Sprites/Square");
                if (whiteSquare != null)
                {
                    shadowRenderer.sprite = whiteSquare;
                }
                
                // Cor preta bem escura (75% de opacidade) para manter escuridão dinâmica
                shadowRenderer.color = new Color(0f, 0f, 0f, 0.75f);
                
                // Escala para cobrir toda a tela
                Camera mainCamera = Camera.main;
                if (mainCamera != null && mainCamera.orthographic)
                {
                    float cameraHeight = mainCamera.orthographicSize * 2f;
                    float cameraWidth = cameraHeight * mainCamera.aspect;
                    
                    shadowObj.transform.localScale = new Vector3(cameraWidth, cameraHeight, 1f);
                    Debug.Log($"[BackgroundConfigurator] Shadow overlay criado com escala: {cameraWidth:F2}x{cameraHeight:F2}");
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
                globalLight.color = globalLightColor;
                Debug.Log($"[BackgroundConfigurator] Global Light 2D configurado com intensidade: {globalLightIntensity} e cor branca");
            }
            else
            {
                Debug.LogError("[BackgroundConfigurator] Global Light 2D não encontrada! Crie manualmente na scene.");
            }
        }
    }
}
