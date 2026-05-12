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

        /// <summary>
        /// Tempo desde o fim da intro; usado para estágios de spawn (evita "pular" dificuldade durante o fade).
        /// </summary>
        public static float StageElapsedSeconds =>
            !AllowGameplay || gameplayEpoch < 0f ? 0f : Time.timeSinceLevelLoad - gameplayEpoch;

        public static void BeginIntro()
        {
            AllowGameplay = false;
            gameplayEpoch = -1f;
        }

        public static void EndIntro()
        {
            gameplayEpoch = Time.timeSinceLevelLoad;
            AllowGameplay = true;
        }
    }
}
