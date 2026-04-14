using UnityEngine;

public class PenadoAnimator : MonoBehaviour
{
    public Sprite[] animationFrames;
    
    private SpriteRenderer spriteRenderer;
    private int currentFrameIndex = 0;
    private float frameTimer = 0f;
    public float frameRate = 12f; // FPS da animação

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Se os frames não foram setados no Inspector, tentar carregar
        if (animationFrames == null || animationFrames.Length == 0)
        {
            LoadSpritesFromAssets();
        }
    }

    private void LoadSpritesFromAssets()
    {
        animationFrames = new Sprite[6];
        
        // Tentar carregar cada sprite
        for (int i = 1; i <= 6; i++)
        {
            string assetPath = $"Assets/PenadoWalk/frame_{i:D5}";
            Sprite sprite = Resources.Load<Sprite>(assetPath);
            animationFrames[i - 1] = sprite;
            
            if (sprite == null)
            {
                Debug.LogWarning($"Não conseguiu carregar: {assetPath}");
            }
        }
    }

    private void Update()
    {
        if (animationFrames == null || animationFrames.Length == 0 || spriteRenderer == null)
            return;

        frameTimer += Time.deltaTime;
        float frameDuration = 1f / frameRate;

        if (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;
            currentFrameIndex = (currentFrameIndex + 1) % animationFrames.Length;
            
            if (animationFrames[currentFrameIndex] != null)
            {
                spriteRenderer.sprite = animationFrames[currentFrameIndex];
            }
        }
    }
}
