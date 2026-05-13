using UnityEngine;

namespace GhostBeam.Managers
{
    /// <summary>
    /// Enquanto a intro estiver ativa, sistemas de gameplay devem permanecer "em espera"
    /// (sem spawn, sem movimento da Luna, sem contagem de tempo/pontos, etc.).
    /// </summary>
    public static class GameplayIntroState
    {
        public static bool AllowGameplay { get; private set; } = true;

        private static float gameplayEpoch = -1f;
        private static bool hasInitialized = false;

        /// <summary>
        /// Tempo desde o fim da intro; usado para estágios de spawn (evita "pular" dificuldade durante o fade).
        /// </summary>
        public static float StageElapsedSeconds =>
            !AllowGameplay || gameplayEpoch < 0f ? 0f : Time.timeSinceLevelLoad - gameplayEpoch;

        public static void BeginIntro()
        {
            AllowGameplay = false;
            gameplayEpoch = -1f;
            Debug.Log($"[GameplayIntroState] Intro STARTED | AllowGameplay=false | StageTime will be 0");
        }

        public static void EndIntro()
        {
            gameplayEpoch = Time.timeSinceLevelLoad;
            AllowGameplay = true;
            Debug.Log($"[GameplayIntroState] Intro ENDED at {gameplayEpoch:F2}s | AllowGameplay=true | StageTime now counting!");
        }

        /// <summary>
        /// Auto-end intro if it's stuck (for scenes without GameplayIntroFade)
        /// </summary>
        public static void AutoEndIntroIfStuck(float maxWaitSeconds = 2f)
        {
            if (!AllowGameplay && gameplayEpoch < 0f && Time.timeSinceLevelLoad > maxWaitSeconds)
            {
                Debug.LogWarning($"[GameplayIntroState] Intro was stuck for {maxWaitSeconds}s, auto-ending it!");
                EndIntro();
            }
        }

        /// <summary>
        /// Ensure gameplay has started - call this if there's no intro fade
        /// </summary>
        public static void EnsureGameplayStarted()
        {
            if (!hasInitialized)
            {
                hasInitialized = true;
                if (gameplayEpoch < 0f)
                {
                    gameplayEpoch = Time.timeSinceLevelLoad;
                    AllowGameplay = true;
                    Debug.Log($"[GameplayIntroState] Gameplay auto-started (no intro fade detected) at {gameplayEpoch:F2}s");
                }
            }
        }
    }
}
