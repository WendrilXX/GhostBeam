using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GhostBeam.UI
{
    /// <summary>
    /// Menu central em coluna + tema azul sombrio nos painéis Loja, Configurações e Tutorial.
    /// </summary>
    public class MenuLayoutReorganizer : MonoBehaviour
    {
        private const string TitleText = "GHOST BEAM";
        private const string SubtitleText = "Sobreviva no Escuro";

        [SerializeField] private bool autoReorganizeOnStart;
        [SerializeField] private float edgePadding = 16f;
        [SerializeField] private float buttonGap = 14f;
        [SerializeField] [Range(0.28f, 0.52f)] private float columnWidthFraction = 0.42f;
        [SerializeField] [Range(0.48f, 0.72f)] private float columnHeightFraction = 0.62f;
        [SerializeField] [Range(52f, 92f)] private float buttonHeight = 76f;

        private RectTransform _mainMenuRect;

        private void Start()
        {
            if (autoReorganizeOnStart)
                ReorganizeLayout();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (_mainMenuRect != null && isActiveAndEnabled)
                ReorganizeLayout();
        }

        public void ReorganizeLayout()
        {
            RemoveTaglineRowIfPresent();

            Transform mainMenuContainer = transform.Find("MainMenuContainer");
            if (mainMenuContainer == null)
            {
                ApplyTitleAndSubtitle();
                AlignTitleSubtitleWithMainColumnX();
                ApplySubPanelsSombreStyle();
                return;
            }

            _mainMenuRect = mainMenuContainer.GetComponent<RectTransform>();
            if (_mainMenuRect == null)
            {
                ApplyTitleAndSubtitle();
                AlignTitleSubtitleWithMainColumnX();
                ApplySubPanelsSombreStyle();
                return;
            }

            var vlg = mainMenuContainer.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
                vlg.enabled = false;

            var hlg = mainMenuContainer.GetComponent<HorizontalLayoutGroup>();
            if (hlg != null)
                hlg.enabled = false;

            ApplyCenterColumnAnchors();
            ApplySafeAreaHorizontalInset();
            AlignTitleSubtitleWithMainColumnX();

            Button btnPlay = FindButtonByName(mainMenuContainer, "BtnPlay");
            Button btnShop = FindButtonByName(mainMenuContainer, "BtnShop");
            Button btnSettings = FindButtonByName(mainMenuContainer, "BtnSettings");
            Button btnTutorial = FindButtonByName(mainMenuContainer, "BtnTutorial");
            Button btnQuit = FindButtonByName(mainMenuContainer, "BtnQuit");

            if (btnPlay == null || btnShop == null || btnSettings == null || btnTutorial == null || btnQuit == null)
            {
                Debug.LogWarning("[MenuLayoutReorganizer] Botões do menu principal incompletos.");
                ApplyTitleAndSubtitle();
                AlignTitleSubtitleWithMainColumnX();
                ApplySubPanelsSombreStyle();
                return;
            }

            var stack = new[] { btnPlay, btnShop, btnSettings, btnTutorial, btnQuit };
            foreach (var b in stack)
                PrepareButtonForStack(b);

            LayoutVerticalStack(stack);
            StraightenUi(stack);

            foreach (var b in stack)
                StyleMetalMenuButton(b);

            ApplyTitleAndSubtitle();
            AlignTitleSubtitleWithMainColumnX();
            ApplySubPanelsSombreStyle();
        }

        private void AlignTitleSubtitleWithMainColumnX()
        {
            float x = _mainMenuRect != null ? _mainMenuRect.anchoredPosition.x : 0f;
            var titleTf = FindDeepChild(transform, "Title") as RectTransform;
            var subtitleTf = FindDeepChild(transform, "Subtitle") as RectTransform;
            if (titleTf != null)
            {
                Vector2 p = titleTf.anchoredPosition;
                titleTf.anchoredPosition = new Vector2(x, p.y);
            }

            if (subtitleTf != null)
            {
                Vector2 p = subtitleTf.anchoredPosition;
                subtitleTf.anchoredPosition = new Vector2(x, p.y);
            }
        }

        private void RemoveTaglineRowIfPresent()
        {
            Transform row = transform.Find("TaglineRow");
            if (row == null)
                return;

            Transform subtitle = row.Find("Subtitle");
            if (subtitle != null)
            {
                subtitle.SetParent(transform, false);
                Transform title = transform.Find("Title");
                int insert = title != null ? title.GetSiblingIndex() + 1 : 1;
                subtitle.SetSiblingIndex(Mathf.Min(insert, transform.childCount - 1));
            }

            Destroy(row.gameObject);
        }

        private void ApplySubPanelsSombreStyle()
        {
            foreach (string panelName in new[] { "ShopPanel", "SettingsPanel", "TutorialPanel" })
            {
                Transform panel = transform.Find(panelName);
                if (panel == null)
                    continue;

                StraightenHierarchy(panel);
                StylePanelRootBackdrop(panel);
                StylePanelImages(panel);
                StylePanelSliders(panel);
                foreach (var button in panel.GetComponentsInChildren<Button>(true))
                    StyleMetalMenuButton(button);

                foreach (var tmp in panel.GetComponentsInChildren<TextMeshProUGUI>(true))
                    StylePanelTextMesh(tmp);

                if (panelName == "SettingsPanel")
                    ApplySettingsPanelLayout(panel);
                else if (panelName == "ShopPanel")
                    ApplyShopPanelLayout(panel);
                else if (panelName == "TutorialPanel")
                    ApplyTutorialPanelLayout(panel);
            }
        }

        private void ApplySettingsPanelLayout(Transform settingsPanel)
        {
            Transform content = settingsPanel.Find("Content");
            if (content == null)
                return;

            var contentRt = content.GetComponent<RectTransform>();
            if (contentRt == null)
                return;

            Canvas canvas = GetComponent<Canvas>();
            RectTransform root = canvas != null ? canvas.transform as RectTransform : transform as RectTransform;
            if (root == null)
                return;

            float scale = root.rect.width / Mathf.Max(1f, (float)Screen.width);
            float safeH = Mathf.Max(1f, Screen.safeArea.height * scale - edgePadding * 2f);
            float panelH = Mathf.Clamp(safeH * 0.78f, 380f, 680f);
            float panelW = Mathf.Clamp(root.rect.width * 0.52f, 400f, 680f);

            contentRt.anchorMin = new Vector2(0.5f, 0.5f);
            contentRt.anchorMax = new Vector2(0.5f, 0.5f);
            contentRt.pivot = new Vector2(0.5f, 0.5f);
            contentRt.sizeDelta = new Vector2(panelW, panelH);
            contentRt.anchoredPosition = Vector2.zero;
            contentRt.localEulerAngles = Vector3.zero;
            contentRt.localScale = Vector3.one;

            var vlg = content.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                vlg.padding = new RectOffset(20, 20, 16, 16);
                vlg.spacing = 12;
                vlg.childAlignment = TextAnchor.MiddleCenter;
                vlg.childControlWidth = true;
                vlg.childControlHeight = true;
                vlg.childForceExpandWidth = false;
                vlg.childForceExpandHeight = false;
                vlg.childScaleWidth = false;
                vlg.childScaleHeight = false;
            }

            Transform backBtn = content.Find("BtnBack");
            if (backBtn != null)
            {
                FixBackButtonLayout(backBtn, panelW);
                backBtn.SetAsLastSibling();
            }

            ApplySettingsToggleRowWidths(content, panelW);

            foreach (Transform child in content)
            {
                if (child == null || child == backBtn)
                    continue;
                var rt = child.GetComponent<RectTransform>();
                if (rt == null)
                    continue;
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.localEulerAngles = Vector3.zero;
            }

            Transform settingsTitle = content.Find("Title");
            if (settingsTitle != null)
            {
                var tmp = settingsTitle.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
                    tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
                }

                var titleLe = settingsTitle.GetComponent<LayoutElement>();
                if (titleLe == null)
                    titleLe = settingsTitle.gameObject.AddComponent<LayoutElement>();
                titleLe.minHeight = 52f;
                titleLe.preferredHeight = 68f;
                titleLe.flexibleWidth = 1f;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRt);
        }

        private static void ApplySettingsToggleRowWidths(Transform content, float panelW)
        {
            float toggleW = Mathf.Clamp(panelW - 56f, 260f, 400f);
            foreach (Transform child in content)
            {
                if (child == null)
                    continue;
                string n = child.name;
                if (n != "Toggle_Vibracao" && n != "Toggle_FPSDisplay")
                    continue;

                var le = child.GetComponent<LayoutElement>();
                if (le == null)
                    le = child.gameObject.AddComponent<LayoutElement>();
                le.ignoreLayout = false;
                le.minWidth = 240f;
                le.preferredWidth = toggleW;
                le.flexibleWidth = 0f;
                le.preferredHeight = 68f;
                le.minHeight = 60f;

                var rt = child.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(0.5f, 0.5f);
                    rt.anchorMax = new Vector2(0.5f, 0.5f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = Vector2.zero;
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, toggleW);
                    rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 68f);
                }
            }
        }

        private void ApplyShopPanelLayout(Transform shopPanel)
        {
            Transform content = shopPanel.Find("Content");
            if (content == null)
                return;

            var contentRt = content.GetComponent<RectTransform>();
            if (contentRt == null)
                return;

            RectTransform root = GetComponent<Canvas>() != null ? GetComponent<Canvas>().transform as RectTransform : transform as RectTransform;
            if (root == null)
                return;

            float scale = root.rect.width / Mathf.Max(1f, (float)Screen.width);
            float safeH = Mathf.Max(1f, Screen.safeArea.height * scale - edgePadding * 2f);
            float panelW = Mathf.Clamp(root.rect.width * 0.56f, 420f, 720f);
            float panelH = Mathf.Clamp(safeH * 0.72f, 400f, 660f);

            contentRt.anchorMin = new Vector2(0.5f, 0.5f);
            contentRt.anchorMax = new Vector2(0.5f, 0.5f);
            contentRt.pivot = new Vector2(0.5f, 0.5f);
            contentRt.sizeDelta = new Vector2(panelW, panelH);
            contentRt.anchoredPosition = Vector2.zero;
            contentRt.localEulerAngles = Vector3.zero;
            contentRt.localScale = Vector3.one;

            var vlg = content.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                vlg.padding = new RectOffset(20, 20, 18, 18);
                vlg.spacing = 12;
                vlg.childAlignment = TextAnchor.MiddleCenter;
                vlg.childControlWidth = true;
                vlg.childControlHeight = true;
                vlg.childForceExpandWidth = false;
                vlg.childForceExpandHeight = false;
            }

            Transform shopBack = content.Find("BtnBack");
            if (shopBack != null)
            {
                FixBackButtonLayout(shopBack, panelW);
                shopBack.SetAsLastSibling();
            }

            foreach (Transform child in content)
            {
                if (child == null || child == shopBack)
                    continue;
                var rt = child.GetComponent<RectTransform>();
                if (rt == null)
                    continue;
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.localEulerAngles = Vector3.zero;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRt);
        }

        private void ApplyTutorialPanelLayout(Transform tutorialPanel)
        {
            Transform content = tutorialPanel.Find("Content");
            if (content == null)
                return;

            var contentRt = content.GetComponent<RectTransform>();
            if (contentRt == null)
                return;

            RectTransform root = GetComponent<Canvas>() != null ? GetComponent<Canvas>().transform as RectTransform : transform as RectTransform;
            if (root == null)
                return;

            float scale = root.rect.width / Mathf.Max(1f, (float)Screen.width);
            float safeH = Mathf.Max(1f, Screen.safeArea.height * scale - edgePadding * 2f);
            float panelW = Mathf.Clamp(root.rect.width * 0.54f, 400f, 700f);
            float panelH = Mathf.Clamp(safeH * 0.76f, 420f, 680f);

            contentRt.anchorMin = new Vector2(0.5f, 0.5f);
            contentRt.anchorMax = new Vector2(0.5f, 0.5f);
            contentRt.pivot = new Vector2(0.5f, 0.5f);
            contentRt.sizeDelta = new Vector2(panelW, panelH);
            contentRt.anchoredPosition = Vector2.zero;
            contentRt.localEulerAngles = Vector3.zero;
            contentRt.localScale = Vector3.one;

            var vlg = content.GetComponent<VerticalLayoutGroup>();
            if (vlg != null)
            {
                vlg.padding = new RectOffset(22, 22, 18, 18);
                vlg.spacing = 16;
                vlg.childAlignment = TextAnchor.MiddleCenter;
                vlg.childControlWidth = true;
                vlg.childControlHeight = true;
                vlg.childForceExpandWidth = false;
                vlg.childForceExpandHeight = false;
            }

            Transform columns = content.Find("Columns");
            if (columns != null)
            {
                var cRt = columns.GetComponent<RectTransform>();
                if (cRt != null)
                {
                    float colW = Mathf.Min(panelW - 28f, 640f);
                    float colH = Mathf.Min(panelH * 0.58f, 420f);
                    cRt.anchorMin = new Vector2(0.5f, 0.5f);
                    cRt.anchorMax = new Vector2(0.5f, 0.5f);
                    cRt.pivot = new Vector2(0.5f, 0.5f);
                    cRt.anchoredPosition = Vector2.zero;
                    cRt.sizeDelta = new Vector2(colW, colH);
                    cRt.localEulerAngles = Vector3.zero;
                }

                var colLe = columns.GetComponent<LayoutElement>();
                if (colLe == null)
                    colLe = columns.gameObject.AddComponent<LayoutElement>();
                colLe.flexibleWidth = 1f;
                colLe.preferredHeight = Mathf.Min(panelH * 0.58f, 420f);
                colLe.minHeight = 200f;
            }

            Transform tutBack = content.Find("BtnBack");
            if (tutBack != null)
            {
                FixBackButtonLayout(tutBack, panelW);
                tutBack.SetAsLastSibling();
            }

            foreach (Transform child in content)
            {
                if (child == null || child == tutBack)
                    continue;
                if (child == columns)
                    continue;
                var rt = child.GetComponent<RectTransform>();
                if (rt == null)
                    continue;
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.localEulerAngles = Vector3.zero;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRt);
        }

        private static void FixBackButtonLayout(Transform backBtn, float panelW)
        {
            var backRt = backBtn.GetComponent<RectTransform>();
            if (backRt == null)
                return;

            backRt.anchorMin = new Vector2(0.5f, 0.5f);
            backRt.anchorMax = new Vector2(0.5f, 0.5f);
            backRt.pivot = new Vector2(0.5f, 0.5f);
            backRt.anchoredPosition = Vector2.zero;
            backRt.localEulerAngles = Vector3.zero;
            backRt.localScale = Vector3.one;
            float bw = Mathf.Min(panelW - 40f, 520f);
            backRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bw);
            backRt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 54f);

            var le = backBtn.GetComponent<LayoutElement>();
            if (le == null)
                le = backBtn.gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = false;
            le.minHeight = 52f;
            le.preferredHeight = 54f;
            le.preferredWidth = bw;
            le.flexibleWidth = 0f;
        }

        private static void StraightenHierarchy(Transform root)
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
                t.localEulerAngles = Vector3.zero;
        }

        private static void StylePanelRootBackdrop(Transform panel)
        {
            var rootImage = panel.GetComponent<Image>();
            if (rootImage != null)
                rootImage.color = MenuVisualTheme.PanelBackdrop;
        }

        private static void StylePanelImages(Transform panel)
        {
            foreach (var img in panel.GetComponentsInChildren<Image>(true))
            {
                if (img.gameObject == panel.gameObject)
                    continue;

                if (img.GetComponentInParent<Slider>() != null)
                    continue;

                Color c = img.color;
                if (c.a < 0.03f)
                    continue;

                float lum = Luminance(c);
                if (lum > 0.78f)
                    img.color = new Color(0.06f, 0.08f, 0.12f, Mathf.Max(c.a, 0.88f));
                else if (lum < 0.14f && c.a > 0.45f)
                    img.color = new Color(0.04f + c.r * 0.35f, 0.05f + c.g * 0.35f, 0.09f + c.b * 0.55f, c.a);
            }
        }

        private static void StylePanelSliders(Transform panel)
        {
            foreach (var slider in panel.GetComponentsInChildren<Slider>(true))
            {
                if (slider.fillRect != null)
                {
                    var fill = slider.fillRect.GetComponent<Image>();
                    if (fill != null)
                        fill.color = MenuVisualTheme.SliderFill;
                }

                Transform bg = slider.transform.Find("Background");
                if (bg != null)
                {
                    var bgImg = bg.GetComponent<Image>();
                    if (bgImg != null)
                        bgImg.color = MenuVisualTheme.SliderTrack;
                }
            }
        }

        private static void StylePanelTextMesh(TextMeshProUGUI tmp)
        {
            if (tmp == null)
                return;

            if (ShouldKeepAccentPriceColor(tmp))
                return;

            tmp.characterHorizontalScale = 1f;
            tmp.fontStyle &= ~FontStyles.Italic;
            float lum = Luminance(tmp.color);
            if (lum > 0.55f)
                tmp.color = new Color(MenuVisualTheme.TextPrimary.r, MenuVisualTheme.TextPrimary.g, MenuVisualTheme.TextPrimary.b, tmp.color.a);
            else
                tmp.color = new Color(MenuVisualTheme.TextMuted.r * 0.95f, MenuVisualTheme.TextMuted.g * 0.95f, MenuVisualTheme.TextMuted.b, tmp.color.a);

            ResetTmpRectStraight(tmp);
        }

        private static bool ShouldKeepAccentPriceColor(TextMeshProUGUI tmp)
        {
            string n = tmp.gameObject.name.ToLowerInvariant();
            if (n.Contains("price"))
                return true;
            Color c = tmp.color;
            return c.g > 0.5f && c.r < 0.45f && c.b < 0.45f;
        }

        private static float Luminance(Color c) => 0.299f * c.r + 0.587f * c.g + 0.114f * c.b;

        private void ApplyCenterColumnAnchors()
        {
            Canvas canvas = GetComponent<Canvas>();
            RectTransform root = canvas != null ? canvas.transform as RectTransform : transform as RectTransform;
            if (root == null)
                return;

            float scale = root.rect.width / Mathf.Max(1f, (float)Screen.width);
            float safeW = Mathf.Max(1f, Screen.safeArea.width * scale - edgePadding * 2f);
            float safeH = Mathf.Max(1f, Screen.safeArea.height * scale - edgePadding * 2f);

            float colW = Mathf.Clamp(safeW * columnWidthFraction, 280f, 480f);
            float colH = Mathf.Clamp(safeH * columnHeightFraction, 300f, 560f);

            _mainMenuRect.anchorMin = new Vector2(0.5f, 0.5f);
            _mainMenuRect.anchorMax = new Vector2(0.5f, 0.5f);
            _mainMenuRect.pivot = new Vector2(0.5f, 0.5f);
            _mainMenuRect.localEulerAngles = Vector3.zero;
            _mainMenuRect.sizeDelta = new Vector2(colW, colH);
            _mainMenuRect.anchoredPosition = new Vector2(0f, -safeH * 0.04f);
        }

        private void ApplySafeAreaHorizontalInset()
        {
            Canvas canvas = GetComponent<Canvas>();
            RectTransform root = canvas != null ? canvas.transform as RectTransform : transform as RectTransform;
            if (root == null)
                return;

            float scale = root.rect.width / Mathf.Max(1f, (float)Screen.width);
            float left = Screen.safeArea.xMin * scale;
            float right = (Screen.width - Screen.safeArea.xMax) * scale;
            float shift = (left - right) * 0.5f;
            _mainMenuRect.anchoredPosition += new Vector2(shift, 0f);
        }

        private static void PrepareButtonForStack(Button button)
        {
            var rt = button.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            var le = button.GetComponent<LayoutElement>();
            if (le != null)
                le.ignoreLayout = true;
        }

        private void LayoutVerticalStack(Button[] stack)
        {
            Rect area = _mainMenuRect.rect;
            float w = Mathf.Max(1f, area.width - 8f);
            int n = stack.Length;
            float gap = buttonGap;
            float h = Mathf.Clamp(buttonHeight, 56f, 92f);
            float totalH = n * h + (n - 1) * gap;
            float topY = totalH * 0.5f - h * 0.5f;

            for (int i = 0; i < n; i++)
            {
                var rt = stack[i].GetComponent<RectTransform>();
                float y = topY - i * (h + gap);
                rt.anchoredPosition = new Vector2(0f, y);
                rt.sizeDelta = new Vector2(w, h);
            }
        }

        private static void StraightenUi(Button[] stack)
        {
            foreach (var b in stack)
            {
                if (b == null)
                    continue;
                b.transform.localEulerAngles = Vector3.zero;
                b.transform.localScale = Vector3.one;
            }
        }

        private static Button FindButtonByName(Transform parent, string name)
        {
            Transform child = parent.Find(name);
            return child != null ? child.GetComponent<Button>() : null;
        }

        private static void StyleMetalMenuButton(Button button)
        {
            if (button == null)
                return;

            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = MenuVisualTheme.ButtonFill;
                image.type = Image.Type.Simple;
            }

            var shadow = button.GetComponent<Shadow>();
            if (shadow == null)
                shadow = button.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
            shadow.effectDistance = new Vector2(4f, -4f);

            var outline = button.GetComponent<Outline>();
            if (outline == null)
                outline = button.gameObject.AddComponent<Outline>();
            outline.useGraphicAlpha = true;
            outline.effectColor = MenuVisualTheme.OutlineIdle;
            outline.effectDistance = new Vector2(1f, -1f);

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.05f, 1.06f, 1.08f, 1f);
            colors.pressedColor = new Color(0.82f, 0.84f, 0.88f, 1f);
            colors.selectedColor = Color.white;
            button.colors = colors;
            button.transition = Selectable.Transition.ColorTint;

            var label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
            {
                label.color = MenuVisualTheme.TextPrimary;
                label.fontStyle &= ~FontStyles.Italic;
                label.enableAutoSizing = true;
                label.fontSizeMin = 17;
                label.fontSizeMax = 32;
                label.characterSpacing = 0.5f;
                label.characterHorizontalScale = 1f;
                ResetTmpRectStraight(label);
            }
        }

        private void ApplyTitleAndSubtitle()
        {
            var titleTf = FindDeepChild(transform, "Title");
            if (titleTf != null)
            {
                StraightenHierarchy(titleTf);
                titleTf.localScale = Vector3.one;
                var title = titleTf.GetComponent<TextMeshProUGUI>();
                if (title != null)
                {
                    title.text = TitleText;
                    title.enableVertexGradient = true;
                    title.colorGradient = new VertexGradient(
                        new Color(0.92f, 0.95f, 1f, 1f),
                        new Color(0.55f, 0.72f, 0.92f, 1f),
                        new Color(0.12f, 0.22f, 0.38f, 1f),
                        new Color(0.28f, 0.42f, 0.62f, 1f));
                    title.color = Color.white;
                    title.characterSpacing = 3f;
                    title.fontStyle = FontStyles.Bold;
                    title.enableAutoSizing = true;
                    title.fontSizeMin = 34;
                    title.fontSizeMax = 92;
                    title.outlineWidth = 0.22f;
                    title.outlineColor = MenuVisualTheme.TitleOutline;
                    title.characterHorizontalScale = 1f;
                    title.horizontalAlignment = HorizontalAlignmentOptions.Center;
                    title.verticalAlignment = VerticalAlignmentOptions.Middle;
                    ResetTmpRectStraight(title);
                }
            }

            var subtitleTf = FindDeepChild(transform, "Subtitle");
            if (subtitleTf != null)
            {
                StraightenHierarchy(subtitleTf);
                subtitleTf.localScale = Vector3.one;
                var subtitle = subtitleTf.GetComponent<TextMeshProUGUI>();
                if (subtitle != null)
                {
                    subtitle.text = SubtitleText;
                    subtitle.color = MenuVisualTheme.TextMuted;
                    subtitle.fontStyle = FontStyles.Normal;
                    subtitle.enableAutoSizing = true;
                    subtitle.fontSizeMin = 15;
                    subtitle.fontSizeMax = 30;
                    subtitle.characterSpacing = 0.4f;
                    subtitle.characterHorizontalScale = 1f;
                    subtitle.horizontalAlignment = HorizontalAlignmentOptions.Center;
                    subtitle.verticalAlignment = VerticalAlignmentOptions.Middle;
                    ResetTmpRectStraight(subtitle);
                }
            }
        }

        private static void ResetTmpRectStraight(TextMeshProUGUI tmp)
        {
            var rt = tmp.rectTransform;
            rt.localEulerAngles = Vector3.zero;
            rt.localScale = Vector3.one;
        }

        private static Transform FindDeepChild(Transform parent, string name)
        {
            if (parent.name == name)
                return parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                var found = FindDeepChild(parent.GetChild(i), name);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}
