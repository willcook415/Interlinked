using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Interlinked.UI
{
    /// <summary>
    /// A self-contained, programmatic rename modal.
    /// No prefab, no inspector wiring, no scene dependencies.
    /// </summary>
    public sealed class RuntimeRenameUI : MonoBehaviour
    {
        public static RuntimeRenameUI Instance { get; private set; }

        Canvas _canvas;
        CanvasGroup _cg;
        RectTransform _panel;
        TMP_InputField _input;
        Button _ok, _cancel;
        Action<string> _onConfirm;
        public bool IsOpen { get; private set; }

        // ====== Public API ======
        public static RuntimeRenameUI GetOrCreate()
        {
            if (Instance != null) return Instance;

            var go = new GameObject("Interlinked_RenameCanvas");
            Instance = go.AddComponent<RuntimeRenameUI>();
            Instance.BuildUI();
            return Instance;
        }

        public void Open(string currentName, Vector2? screenPos, Action<string> onConfirm)
        {
            EnsureEventSystem();

            _onConfirm = onConfirm ?? (_ => { });
            _input.text = currentName ?? "";
            _input.caretPosition = _input.text.Length;

            // Position panel
            var sp = screenPos ?? new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var clamped = new Vector2(
                Mathf.Clamp(sp.x, 220f, Screen.width - 220f),
                Mathf.Clamp(sp.y, 90f, Screen.height - 90f)
            );
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_canvas.transform, clamped, null, out var local);
            _panel.anchoredPosition = local;

            Show();
        }

        public void Close() => Hide();

        // ====== Internals ======
        void BuildUI()
        {
            // Canvas
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 5000;
            gameObject.AddComponent<GraphicRaycaster>();

            // Scaler
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            _cg = gameObject.AddComponent<CanvasGroup>();
            _cg.alpha = 0; _cg.interactable = false; _cg.blocksRaycasts = false;

            // Panel
            var panelGO = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelGO.transform.SetParent(_canvas.transform, false);
            _panel = panelGO.GetComponent<RectTransform>();
            _panel.anchorMin = _panel.anchorMax = new Vector2(0.5f, 0.5f);
            _panel.pivot = new Vector2(0.5f, 0.5f);
            _panel.sizeDelta = new Vector2(420, 140);
            var img = panelGO.GetComponent<Image>();
            img.color = new Color(0, 0, 0, 0.75f);

            // Label
            var labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGO.transform.SetParent(panelGO.transform, false);
            var labelRT = labelGO.GetComponent<RectTransform>();
            labelRT.anchorMin = new Vector2(0, 1); labelRT.anchorMax = new Vector2(1, 1);
            labelRT.pivot = new Vector2(0.5f, 1);
            labelRT.anchoredPosition = new Vector2(0, -12);
            labelRT.sizeDelta = new Vector2(-20, 28);
            var label = labelGO.GetComponent<TextMeshProUGUI>();
            label.text = "Rename station";
            label.fontSize = 22;
            label.alignment = TextAlignmentOptions.Center;

            // Input
            var inputGO = new GameObject("Input", typeof(RectTransform));
            inputGO.transform.SetParent(panelGO.transform, false);
            var inputRT = inputGO.GetComponent<RectTransform>();
            inputRT.anchorMin = new Vector2(0, 0.5f); inputRT.anchorMax = new Vector2(1, 0.5f);
            inputRT.pivot = new Vector2(0.5f, 0.5f);
            inputRT.anchoredPosition = new Vector2(0, 10);
            inputRT.sizeDelta = new Vector2(-30, 32);

            var inputBG = inputGO.AddComponent<Image>();
            inputBG.color = new Color(1, 1, 1, 1);

            _input = inputGO.AddComponent<TMP_InputField>();
            var textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(inputGO.transform, false);
            var text = textGO.GetComponent<TextMeshProUGUI>();
            text.enableAutoSizing = true;
            text.fontSizeMin = 12;
            text.fontSizeMax = 28;
            text.color = Color.black;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            var textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = new Vector2(0, 0); textRT.anchorMax = new Vector2(1, 1);
            textRT.offsetMin = new Vector2(8, 4); textRT.offsetMax = new Vector2(-8, -4);

            _input.textComponent = text;
            _input.characterLimit = 40;
            _input.lineType = TMP_InputField.LineType.SingleLine;

            // OK
            _ok = MakeButton(panelGO.transform, "OK", new Vector2(-90, -40), OnOk);
            // Cancel
            _cancel = MakeButton(panelGO.transform, "Cancel", new Vector2(90, -40), OnCancel);

            Hide(); // start hidden
        }

        Button MakeButton(Transform parent, string label, Vector2 anchoredPos, Action onClick)
        {
            var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(140, 34);
            rt.anchoredPosition = anchoredPos;

            var img = go.GetComponent<Image>();
            img.color = new Color(1, 1, 1, 1);

            var txtGO = new GameObject("Text", typeof(TextMeshProUGUI));
            txtGO.transform.SetParent(go.transform, false);
            var txt = txtGO.GetComponent<TextMeshProUGUI>();
            txt.text = label;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = Color.black;

            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => onClick?.Invoke());
            return btn;
        }

        void OnOk()
        {
            var name = (_input.text ?? "").Trim();
            if (name.Length == 0) { Hide(); return; }
            _onConfirm?.Invoke(name);
            Hide();
        }

        void OnCancel() => Hide();

        void Show()
        {
            IsOpen = true;
            _cg.alpha = 1f; _cg.interactable = true; _cg.blocksRaycasts = true;
            EnsureEventSystem(); // once more, just in case
            EventSystem.current?.SetSelectedGameObject(_input.gameObject);
            _input.Select(); _input.ActivateInputField();
        }

        void Hide()
        {
            IsOpen = false;
            _cg.alpha = 0f; _cg.interactable = false;
            // IMPORTANT: do NOT block raycasts when hidden to avoid “freeze”
            _cg.blocksRaycasts = false;
        }

        static void EnsureEventSystem()
        {
            var systems = UnityEngine.Object.FindObjectsOfType<EventSystem>(true);
            if (systems.Length > 0)
            {
                var es = systems[0];
                var old = es.GetComponent<StandaloneInputModule>();
#if UNITY_EDITOR
                if (old) UnityEngine.Object.DestroyImmediate(old);
#else
                if (old) UnityEngine.Object.Destroy(old);
#endif
                if (!es.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>())
                    es.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

                for (int i = 1; i < systems.Length; i++)
                    systems[i].gameObject.SetActive(false);
                return;
            }

            var go = new GameObject("EventSystem",
                typeof(EventSystem),
                typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
        }
    }
}
