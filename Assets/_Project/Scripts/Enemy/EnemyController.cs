using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GhostBeam.Enemy
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemyController : MonoBehaviour
    {
        public enum EnemyArchetype
        {
            Penado = 0,
            Ictericia = 1,
            Ectogangue = 2,
            Tita = 3,
            Espectro = 4
        }

        [Serializable]
        private class DirectionalSpriteSet
        {
            public Sprite direita;
            public Sprite esquerda;
            public Sprite cimaDireita;
            public Sprite cimaEsquerda;
            public Sprite baixoDireita;
            public Sprite baixoEsquerda;

            public Sprite Resolve(Vector2 direction, float horizontalThreshold = 0.22f)
            {
                if (Mathf.Abs(direction.y) <= horizontalThreshold)
                    return direction.x >= 0f ? direita : esquerda;

                if (direction.y > 0f)
                    return direction.x >= 0f ? cimaDireita : cimaEsquerda;

                return direction.x >= 0f ? baixoDireita : baixoEsquerda;
            }
        }

        [SerializeField] private float speed = 3f;
        [SerializeField] private float timeToKillWhileIlluminated = 2f;  // Increased damage - faster kill
        [SerializeField] private int damageOnContact = 2;  // Increased from 1
        [SerializeField] private float batteryRechargeOnKill = 12f;
        [SerializeField] private EnemyArchetype archetype = EnemyArchetype.Penado;
        [SerializeField] private DirectionalSpriteSet penadoSprites;
        [SerializeField] private DirectionalSpriteSet ictericiaSprites;
        [SerializeField] private DirectionalSpriteSet ectogangueSprites;
        [SerializeField] private DirectionalSpriteSet titaSprites;
        [SerializeField] private DirectionalSpriteSet espectroSprites;

        private Transform playerTransform;
        private Light2D flashlight;
        private float illuminationTimer = 0f;
        private bool isDead = false;
        private Vector2 lastMoveDirection = Vector2.right;
        private Sprite currentDirectionalSprite;

        private SpriteRenderer spriteRenderer;
        public static event Action<Vector3, int> onEnemyKilled;
        public static event Action onEnemyRemoved;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.white;  // Always white - no tint
        }

        private void Start()
        {
            var lunaController = FindAnyObjectByType<Player.LunaController>();
            if (lunaController != null)
            {
                playerTransform = lunaController.transform;
                flashlight = lunaController.GetComponentInChildren<Light2D>();
            }

            ApplyDirectionalSprite(force: true);
        }

        private void Update()
        {
            if (isDead || playerTransform == null)
                return;

            MoveTowardPlayer();
            UpdateIllumination();
            ApplyDirectionalSprite(force: false);
        }

        private void MoveTowardPlayer()
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            if (direction.sqrMagnitude > 0.0001f)
                lastMoveDirection = direction;

            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTransform.position,
                speed * Time.deltaTime
            );
        }

        private void ApplyDirectionalSprite(bool force)
        {
            var set = GetSpriteSet(archetype);
            if (set == null)
                return;

            Sprite resolved = set.Resolve(lastMoveDirection);
            if (resolved == null)
                return;

            if (!force && resolved == currentDirectionalSprite)
                return;

            currentDirectionalSprite = resolved;
            spriteRenderer.sprite = resolved;
        }

        private DirectionalSpriteSet GetSpriteSet(EnemyArchetype target)
        {
            switch (target)
            {
                case EnemyArchetype.Penado:
                    return penadoSprites;
                case EnemyArchetype.Ictericia:
                    return ictericiaSprites;
                case EnemyArchetype.Ectogangue:
                    return ectogangueSprites;
                case EnemyArchetype.Tita:
                    return titaSprites;
                case EnemyArchetype.Espectro:
                    return espectroSprites;
                default:
                    return penadoSprites;
            }
        }

        private void UpdateIllumination()
        {
            bool isIlluminated = IsIlluminated();

            if (isIlluminated)
            {
                illuminationTimer += Time.deltaTime * GetIlluminationPowerMultiplier();
                
                // Feedback visual: fade from white to reddish (burning effect)
                float lerpValue = Mathf.Clamp01(illuminationTimer / timeToKillWhileIlluminated);
                Color burnColor = new Color(1f, 0.3f, 0.3f, 1f);  // Reddish color
                spriteRenderer.color = Color.Lerp(Color.white, burnColor, lerpValue);

                if (illuminationTimer >= timeToKillWhileIlluminated)
                {
                    Die();
                }
            }
            else
            {
                illuminationTimer = 0f;
                spriteRenderer.color = Color.white;  // Back to white - no damage
            }
        }

        private float GetIlluminationPowerMultiplier()
        {
            if (flashlight == null)
                return 1f;

            // Intensity upgrades increase how fast enemies are eliminated.
            return Mathf.Clamp(flashlight.intensity / 1.9f, 0.75f, 2.5f);
        }

        private bool IsIlluminated()
        {
            if (flashlight == null || !flashlight.enabled)
                return false;

            Vector2 toEnemy = (Vector2)(transform.position - flashlight.transform.position);
            float distance = toEnemy.magnitude;
            float angle = Vector2.Angle(flashlight.transform.up, toEnemy);

            float lightRange = flashlight.pointLightOuterRadius > 0f ? flashlight.pointLightOuterRadius : 15f;
            float outerAngle = flashlight.pointLightOuterAngle > 0f ? flashlight.pointLightOuterAngle : 70f;
            float halfConeAngle = outerAngle * 0.5f;

            return distance < lightRange && angle < halfConeAngle;
        }

        private void Die()
        {
            if (isDead)
                return;

            isDead = true;

            // Score
            if (Managers.ScoreManager.Instance != null)
            {
                Managers.ScoreManager.Instance.AddScore(15); // +15 pontos por kill
            }

            // Recarrega uma parte da bateria a cada inimigo eliminado
            var batterySystem = FindAnyObjectByType<Gameplay.BatterySystem>();
            if (batterySystem != null && batteryRechargeOnKill > 0f)
            {
                batterySystem.Recharge(batteryRechargeOnKill);
            }

            // Evento para spawners
            onEnemyKilled?.Invoke(transform.position, 1);

            // Vibração
            if (Managers.SettingsManager.Instance != null)
            {
                Managers.SettingsManager.Instance.Vibrate();
            }

            // Disable collider and movement
            GetComponent<CircleCollider2D>().enabled = false;

            // Fall and fade out animation
            StartCoroutine(FallAndDisappear());
        }

        private IEnumerator FallAndDisappear()
        {
            float fallDuration = 0.6f;
            float elapsedTime = 0f;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + Vector3.down * 1.5f;

            while (elapsedTime < fallDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / fallDuration;

                // Fall down
                transform.position = Vector3.Lerp(startPosition, endPosition, progress);

                // Fade out (reduce alpha)
                Color currentColor = spriteRenderer.color;
                currentColor.a = Mathf.Lerp(1f, 0f, progress);
                spriteRenderer.color = currentColor;

                yield return null;
            }

            // Remove from game
            var pooled = GetComponent<Utilities.PooledObject>();
            if (pooled != null)
            {
                onEnemyRemoved?.Invoke();
                pooled.ReleaseToPool();
                yield break;
            }

            onEnemyRemoved?.Invoke();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            onEnemyRemoved?.Invoke();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name == "Luna" || collision.gameObject.CompareTag("Player"))
            {
                var healthSystem = collision.gameObject.GetComponent<Gameplay.HealthSystem>();
                if (healthSystem != null && !isDead)
                {
                    Debug.Log($"[Enemy] HIT! Luna took {damageOnContact} damage");
                    healthSystem.TakeDamage(damageOnContact);
                    
                    if (Managers.SettingsManager.Instance != null)
                    {
                        Managers.SettingsManager.Instance.Vibrate();
                    }
                }
                else if (healthSystem == null)
                {
                    Debug.LogWarning($"[Enemy] {collision.gameObject.name} has no HealthSystem component!");
                }
            }
        }

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            // Draw detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 15f);
            #endif
        }

        public void Reset()
        {
            isDead = false;
            illuminationTimer = 0f;
            
            // Reset color with full alpha
            Color resetColor = Color.white;
            resetColor.a = 1f;
            spriteRenderer.color = resetColor;
            
            // Re-enable collider
            var collider = GetComponent<CircleCollider2D>();
            if (collider != null)
                collider.enabled = true;
            
            currentDirectionalSprite = null;
            ApplyDirectionalSprite(force: true);
        }

        public void InitializeArchetype(EnemyArchetype targetArchetype)
        {
            archetype = targetArchetype;
            currentDirectionalSprite = null;
            ApplyDirectionalSprite(force: true);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EnsureSpriteReferences();
        }

        public void EditorAutoFillSprites()
        {
            EnsureSpriteReferences();
            EditorUtility.SetDirty(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }

        private void EnsureSpriteReferences()
        {
            EnsureSet(ref penadoSprites);
            EnsureSet(ref ictericiaSprites);
            EnsureSet(ref ectogangueSprites);
            EnsureSet(ref titaSprites);
            EnsureSet(ref espectroSprites);

            AutoFillSet(penadoSprites,
                "Assets/_Project/Art/Sprites/Penado",
                "penado",
                "cima_esquerda",
                "baixo_esquerda");

            AutoFillSet(ictericiaSprites,
                "Assets/_Project/Art/Sprites/Ictericia",
                "Ictericia",
                "esquerda_cima",
                "esquerda_baixo");

            AutoFillSet(ectogangueSprites,
                "Assets/_Project/Art/Sprites/Ectogangue",
                "Ectogangue",
                "cima_esquerda",
                "baixo_esquerda");

            AutoFillSet(titaSprites,
                "Assets/_Project/Art/Sprites/Tita",
                "Tita",
                "cima_esquerda",
                "baixo_esquerda");

            AutoFillSet(espectroSprites,
                "Assets/_Project/Art/Sprites/Espectro",
                "Espectro",
                "cima_esquerda",
                "baixo_esquerda");
        }

        private void EnsureSet(ref DirectionalSpriteSet set)
        {
            if (set == null)
                set = new DirectionalSpriteSet();
        }

        private void AutoFillSet(DirectionalSpriteSet set, string baseFolder, string prefix, string cimaEsquerdaSuffix, string baixoEsquerdaSuffix)
        {
            if (set == null)
                return;

            set.direita = LoadIfMissing(set.direita, $"{baseFolder}/{prefix}_direita.png");
            set.esquerda = LoadIfMissing(set.esquerda, $"{baseFolder}/{prefix}_esquerda.png");
            set.cimaDireita = LoadIfMissing(set.cimaDireita, $"{baseFolder}/{prefix}_cima_direita.png");
            set.cimaEsquerda = LoadIfMissing(set.cimaEsquerda, $"{baseFolder}/{prefix}_{cimaEsquerdaSuffix}.png");
            set.baixoDireita = LoadIfMissing(set.baixoDireita, $"{baseFolder}/{prefix}_baixo_direita.png");
            set.baixoEsquerda = LoadIfMissing(set.baixoEsquerda, $"{baseFolder}/{prefix}_{baixoEsquerdaSuffix}.png");
        }

        private Sprite LoadIfMissing(Sprite current, string path)
        {
            if (current != null)
                return current;

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
#endif
    }
}
