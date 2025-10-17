using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
// NEW: no Keyboard polling here anymore

namespace Interlinked.UI
{
    [RequireComponent(typeof(CanvasGroup))] // <— ensure a CanvasGroup exists
    public sealed class StationRenameOverlay : MonoBehaviour
    {
        [Header("Wiring (auto if left empty)")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform panel;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Button okButton;
        [SerializeField] private Button cancelButton;

        private Action<string> _onConfirm;
        public bool IsOpen { get; private set; }

        private void Awake()
        {
            // Auto-wire & ensure CanvasGroup is present
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (!panel) panel = transform.Find("Panel") as RectTransform;
            if (!input) input = transform.Find("Panel/Input")?.GetComponent<TMP_InputField>();
            if (!label) label = transform.Find("Panel/Label")?.GetComponent<TMP_Text>();
            if (!okButton) okButton = transform.Find("Panel/Confirm")?.GetComponent<Button>();
            if (!cancelButton) cancelButton = transform.Find("Panel/Cancel")?.GetComponent<Button>();

            if (okButton) { okButton.onClick.RemoveAllListeners(); okButton.onClick.AddListener(OnClickOk); }
            if (cancelButton) { cancelButton.onClick.RemoveAllListeners(); cancelButton.onClick.AddListener(OnClickCancel); }

            EnsureEventSystem();
            HideImmediate(); // start hidden
        }

        public void Open(string currentName, Vector2 screenPos, Action<string> onConfirm)
        {
            _onConfirm = onConfirm;
            if (label) label.text = "Rename station";
            if (input)
            {
                input.text = currentName;
                input.caretPosition = input.text.Length;
            }

            // place near cursor (clamped)
            var canvasRt = (RectTransform)transform;
            Vector2 clamped = new Vector2(
                Mathf.Clamp(screenPos.x, 220f, Screen.width - 220f),
                Mathf.Clamp(screenPos.y, 80f, Screen.height - 80f)
            );
            if (panel) panel.anchoredPosition = ScreenToCanvas(canvasRt, clamped);

            Show();
            if (input && EventSystem.current)
            {
                EventSystem.current.SetSelectedGameObject(input.gameObject);
                input.Select();
                input.ActivateInputField();
            }
        }

        public void OnClickOk() => Confirm(input ? input.text : null);
        public void OnClickCancel() => HideImmediate();

        // Helpers the controller can call:
        public void ForceOpenAtCenter(string currentName, Action<string> onConfirm = null)
        {
            var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Open(currentName, center, onConfirm ?? (_ => { }));
        }
        public void ForceClose() => HideImmediate();
        public void ConfirmCurrent() => Confirm(input ? input.text : null);

        private void Confirm(string text)
        {
            var name = (text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(name)) { HideImmediate(); return; }
            _onConfirm?.Invoke(name);
            HideImmediate();
        }

        private void Show()
        {
            IsOpen = true;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            gameObject.SetActive(true);
        }

        private void HideImmediate()
        {
            IsOpen = false;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(true); // keep active for references
        }

        private static Vector2 ScreenToCanvas(RectTransform canvas, Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screen, null, out var local);
            return local;
        }

        private static void EnsureEventSystem()
        {
            // Find ANY event systems (active or inactive)
            var systems = UnityEngine.Object.FindObjectsOfType<EventSystem>(true);
            if (systems.Length > 0)
            {
                // Upgrade the first one to use the new Input System UI module
                var es = systems[0];

                var old = es.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#if UNITY_EDITOR
        if (old) UnityEngine.Object.DestroyImmediate(old);
#else
                if (old) UnityEngine.Object.Destroy(old);
#endif
                if (!es.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>())
                    es.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

                // Disable any extras to avoid "There can be only one active Event System."
                for (int i = 1; i < systems.Length; i++)
                    systems[i].gameObject.SetActive(false);

                return;
            }

            // None found ? create ONE correctly configured ES
            var go = new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule)
            );
        }


    }
}

