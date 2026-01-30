using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LuckyCharm.Core;
using LuckyCharm.Utils;

namespace LuckyCharm.UI
{
    public class SettingsPanel : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;

        [Header("Language Selection")]
        [SerializeField] private Button englishButton;
        [SerializeField] private Button spanishButton;
        [SerializeField] private GameObject englishCheckmark;
        [SerializeField] private GameObject spanishCheckmark;

        private bool _initialized = false;

        private void Start()
        {
            // If references aren't set, try to build UI programmatically
            if (panel == null)
            {
                panel = gameObject;
                BuildUI();
            }

            SetupListeners();

            if (panel != null)
                panel.SetActive(false);

            if (LocalizationManager.Instance != null)
                LocalizationManager.Instance.OnLanguageChanged += UpdateCheckmarks;

            UpdateCheckmarks();
        }

        private void BuildUI()
        {
            if (_initialized) return;
            _initialized = true;

            // Create content container
            var content = CreateUIObject("Content", transform);
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(500, 400);

            var contentImage = content.AddComponent<Image>();
            contentImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);

            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(30, 30, 30, 30);
            layout.spacing = 25;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;

            // Title
            CreateText(content.transform, "Settings", 36, 60);

            // Language section
            var langSection = CreateUIObject("LanguageSection", content.transform);
            var langRect = langSection.GetComponent<RectTransform>();
            langRect.sizeDelta = new Vector2(0, 80);
            var langLayout = langSection.AddComponent<HorizontalLayoutGroup>();
            langLayout.spacing = 15;
            langLayout.childAlignment = TextAnchor.MiddleCenter;
            langLayout.childControlWidth = false;
            langLayout.childControlHeight = false;

            // Language buttons
            englishButton = CreateLanguageButton(langSection.transform, "English", out englishCheckmark);
            spanishButton = CreateLanguageButton(langSection.transform, "Español", out spanishCheckmark);

            // Close button
            closeButton = CreateButton(content.transform, "Close", 150, 50);
        }

        private void SetupListeners()
        {
            if (openButton != null)
                openButton.onClick.AddListener(OpenPanel);

            if (closeButton != null)
                closeButton.onClick.AddListener(ClosePanel);

            if (englishButton != null)
                englishButton.onClick.AddListener(() => SelectLanguage("en"));

            if (spanishButton != null)
                spanishButton.onClick.AddListener(() => SelectLanguage("es"));
        }

        private void OnDestroy()
        {
            if (LocalizationManager.Instance != null)
                LocalizationManager.Instance.OnLanguageChanged -= UpdateCheckmarks;
        }

        public void OpenPanel()
        {
            AudioManager.Instance?.PlayUITap();
            UpdateCheckmarks();

            if (panel != null)
                panel.SetActive(true);
        }

        public void ClosePanel()
        {
            AudioManager.Instance?.PlayUITap();

            if (panel != null)
                panel.SetActive(false);
        }

        private void SelectLanguage(string localeCode)
        {
            AudioManager.Instance?.PlayUITap();
            LocalizationManager.Instance?.SetLanguage(localeCode);
        }

        private void UpdateCheckmarks()
        {
            string currentCode = LocalizationManager.Instance?.CurrentLocale?.Identifier.Code ?? "en";

            if (englishCheckmark != null)
                englishCheckmark.SetActive(currentCode == "en");

            if (spanishCheckmark != null)
                spanishCheckmark.SetActive(currentCode == "es");
        }

        // UI Helper Methods
        private GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private void CreateText(Transform parent, string text, int fontSize, float height)
        {
            var go = CreateUIObject("Text", parent);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, height);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private Button CreateLanguageButton(Transform parent, string label, out GameObject checkmark)
        {
            var go = CreateUIObject(label + "Button", parent);
            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160, 70);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            var button = go.AddComponent<Button>();

            // Label
            var labelGO = CreateUIObject("Label", go.transform);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var labelText = labelGO.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 24;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            // Checkmark
            var checkGO = CreateUIObject("Checkmark", go.transform);
            var checkRect = checkGO.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(1, 0.5f);
            checkRect.anchorMax = new Vector2(1, 0.5f);
            checkRect.pivot = new Vector2(1, 0.5f);
            checkRect.anchoredPosition = new Vector2(-8, 0);
            checkRect.sizeDelta = new Vector2(25, 25);

            var checkText = checkGO.AddComponent<TextMeshProUGUI>();
            checkText.text = "✓";
            checkText.fontSize = 20;
            checkText.alignment = TextAlignmentOptions.Center;
            checkText.color = new Color(0.2f, 0.8f, 0.2f);

            checkmark = checkGO;
            return button;
        }

        private Button CreateButton(Transform parent, string label, float width, float height)
        {
            var go = CreateUIObject(label + "Button", parent);
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = width;
            layoutElement.preferredHeight = height;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.3f, 0.5f, 0.8f, 1f);

            var button = go.AddComponent<Button>();

            var labelGO = CreateUIObject("Label", go.transform);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var labelText = labelGO.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 22;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            return button;
        }
    }
}
