using UnityEngine;

namespace GhostBeam.UI
{
    /// <summary>
    /// Paleta sombria azul-escuro compartilhada pelo menu principal e pelos painéis (loja, opções, tutorial).
    /// </summary>
    public static class MenuVisualTheme
    {
        public static readonly Color PanelBackdrop = new Color(0.026f, 0.04f, 0.07f, 0.97f);
        public static readonly Color ButtonFill = new Color(0.07f, 0.1f, 0.15f, 0.96f);
        public static readonly Color OutlineIdle = new Color(0.34f, 0.44f, 0.58f, 0.92f);
        public static readonly Color OutlineGlow = new Color(0.38f, 0.68f, 0.98f, 1f);
        public static readonly Color TextPrimary = new Color(0.88f, 0.92f, 0.98f, 1f);
        public static readonly Color TextMuted = new Color(0.62f, 0.72f, 0.82f, 0.95f);
        public static readonly Color TitleOutline = new Color(0.04f, 0.12f, 0.22f, 0.9f);
        public static readonly Color LoadingBarFill = new Color(0.2f, 0.48f, 0.78f, 1f);
        public static readonly Color SliderFill = new Color(0.18f, 0.42f, 0.68f, 1f);
        public static readonly Color SliderTrack = new Color(0.06f, 0.08f, 0.12f, 1f);
    }
}
