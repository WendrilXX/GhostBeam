using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace GhostBeam.UI
{
    /// <summary>
    /// Botões do menu central: escala no toque e contorno azul no item em foco (Jogar como padrão).
    /// </summary>
    public class MenuUIAnimator : MonoBehaviour
    {
        [SerializeField] private float buttonScaleOnHover = 1.03f;
        [SerializeField] private float buttonScaleOnPress = 0.97f;
        [SerializeField] private float animationDuration = 0.16f;
        [SerializeField] private CanvasGroup canvasGroup;

        private Button[] _menuButtons;
        private RectTransform[] _buttonRects;
        private Vector3[] _originalScales;
        private Outline[] _outlines;
        private Coroutine[] _pressCoroutines;
        private int _hoverIndex = -1;

        private void Start()
        {
            Transform mainMenu = transform.Find("MainMenuContainer");
            if (mainMenu == null)
                return;

            _menuButtons = mainMenu.GetComponentsInChildren<Button>(false);
            int n = _menuButtons.Length;
            _buttonRects = new RectTransform[n];
            _originalScales = new Vector3[n];
            _outlines = new Outline[n];
            _pressCoroutines = new Coroutine[n];

            for (int i = 0; i < n; i++)
            {
                int index = i;
                _buttonRects[i] = _menuButtons[i].GetComponent<RectTransform>();
                _originalScales[i] = _buttonRects[i].localScale;
                _outlines[i] = _menuButtons[i].GetComponent<Outline>();

                var trigger = _menuButtons[i].gameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                    trigger = _menuButtons[i].gameObject.AddComponent<EventTrigger>();
                else
                    trigger.triggers.Clear();

                AddEntry(trigger, EventTriggerType.PointerEnter, _ => OnHoverEnter(index));
                AddEntry(trigger, EventTriggerType.PointerExit, _ => OnPointerExitAny(index));
                AddEntry(trigger, EventTriggerType.PointerDown, _ => OnPressDown(index));
                AddEntry(trigger, EventTriggerType.PointerUp, _ => OnPressUp(index));
            }

            ApplyOutlineHighlight(0);

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                StartCoroutine(FadeInMenu());
            }
        }

        private static void AddEntry(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
        {
            var e = new EventTrigger.Entry { eventID = type };
            e.callback.AddListener(action);
            trigger.triggers.Add(e);
        }

        private void ApplyOutlineHighlight(int index)
        {
            if (_outlines == null)
                return;

            for (int i = 0; i < _outlines.Length; i++)
            {
                if (_outlines[i] == null)
                    continue;

                bool on = i == index;
                _outlines[i].effectColor = on ? MenuVisualTheme.OutlineGlow : MenuVisualTheme.OutlineIdle;
                _outlines[i].effectDistance = on ? new Vector2(3f, -3f) : new Vector2(1f, -1f);
            }
        }

        private void OnHoverEnter(int index)
        {
            if (!IsValidIndex(index)) return;
            _hoverIndex = index;
            ApplyOutlineHighlight(index);
            StopScaleCoroutine(index);
            _pressCoroutines[index] = StartCoroutine(ScaleTo(_buttonRects[index], _originalScales[index] * buttonScaleOnHover, animationDuration));
        }

        private void OnPointerExitAny(int index)
        {
            if (!IsValidIndex(index)) return;
            _hoverIndex = -1;
            ApplyOutlineHighlight(0);
            StopScaleCoroutine(index);
            _pressCoroutines[index] = StartCoroutine(ScaleTo(_buttonRects[index], _originalScales[index], animationDuration));
        }

        private void OnPressDown(int index)
        {
            if (!IsValidIndex(index)) return;
            StopScaleCoroutine(index);
            _pressCoroutines[index] = StartCoroutine(ScaleTo(_buttonRects[index], _originalScales[index] * buttonScaleOnPress, animationDuration * 0.55f));
        }

        private void OnPressUp(int index)
        {
            if (!IsValidIndex(index)) return;
            StopScaleCoroutine(index);
            float mul = _hoverIndex == index ? buttonScaleOnHover : 1f;
            _pressCoroutines[index] = StartCoroutine(ScaleTo(_buttonRects[index], _originalScales[index] * mul, animationDuration * 0.55f));
            ApplyOutlineHighlight(_hoverIndex >= 0 ? _hoverIndex : 0);
        }

        private bool IsValidIndex(int index)
        {
            return _buttonRects != null && index >= 0 && index < _buttonRects.Length && _buttonRects[index] != null;
        }

        private void StopScaleCoroutine(int index)
        {
            if (_pressCoroutines[index] != null)
            {
                StopCoroutine(_pressCoroutines[index]);
                _pressCoroutines[index] = null;
            }
        }

        private IEnumerator ScaleTo(RectTransform rect, Vector3 targetScale, float duration)
        {
            Vector3 startScale = rect.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                rect.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
                yield return null;
            }

            rect.localScale = targetScale;
        }

        private IEnumerator FadeInMenu()
        {
            float elapsed = 0f;
            float duration = 0.55f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = 1;
        }
    }
}
