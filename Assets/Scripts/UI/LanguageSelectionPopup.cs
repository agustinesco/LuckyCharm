using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LuckyCharm.Core;
using LuckyCharm.Utils;

namespace LuckyCharm.UI
{
    public class LanguageSelectionPopup : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject panel;

        [Header("Language Buttons")]
        [SerializeField] private Button englishButton;
        [SerializeField] private Button spanishButton;
        [SerializeField] private GameObject englishSelectedIndicator;
        [SerializeField] private GameObject spanishSelectedIndicator;

        [Header("Confirm")]
        [SerializeField] private Button confirmButton;

        private string _selectedLocale = "en";
        private bool _initialized = false;

        private void Start()
        {
            // If references aren't set, build UI programmatically
            if (panel == null)
            {
                panel = gameObject;
                BuildUI();
            }

            SetupListeners();
            StartCoroutine(CheckFirstLaunch());
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
            contentRect.sizeDelta = new Vector2(500, 500);

            var contentImage = content.AddComponent<Image>();
            contentImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);

            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(30, 30, 40, 30);
            layout.spacing = 30;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;

            // Title
            CreateText(content.transform, "Select Language\nSeleccionar Idioma", 32, 90);

            // Language section
            var langSection = CreateUIObject("LanguageSection", content.transform);
            var langRect = langSection.GetComponent<RectTransform>();
            langRect.sizeDelta = new Vector2(0, 200);
            var langLayout = langSection.AddComponent<VerticalLayoutGroup>();
            langLayout.spacing = 15;
            langLayout.childAlignment = TextAnchor.MiddleCenter;
            langLayout.childControlWidth = true;
            langLayout.childControlHeight = false;

            // Language buttons
            englishButton = CreateLargeLanguageButton(langSection.transform, "English", out englishSelectedIndicator);
            spanishButton = CreateLargeLanguageButton(langSection.transform, "Español", out spanishSelectedIndicator);

            // Confirm button
            confirmButton = CreateButton(content.transform, "Confirm / Confirmar", 350, 60);
        }

        private void SetupListeners()
        {
            if (englishButton != null)
                englishButton.onClick.AddListener(() => SelectLanguage("en"));

            if (spanishButton != null)
                spanishButton.onClick.AddListener(() => SelectLanguage("es"));

            if (confirmButton != null)
                confirmButton.onClick.AddListener(ConfirmSelection);
        }

        private IEnumerator CheckFirstLaunch()
        {
            yield return new WaitUntil(() => LocalizationManager.Instance != null);
            yield return new WaitUntil(() => LocalizationManager.Instance.IsInitialized);

            if (LocalizationManager.Instance.IsFirstLaunch)
            {
                _selectedLocale = LocalizationManager.Instance.GetDetectedLanguageCode();
                UpdateSelectionVisuals();
                ShowPopup();
            }
            else
            {
                HidePopup();
            }
        }

        private void SelectLanguage(string localeCode)
        {
            AudioManager.Instance?.PlayUITap();
            _selectedLocale = localeCode;
            LocalizationManager.Instance?.SetLanguage(localeCode);
            UpdateSelectionVisuals();
        }

        private void UpdateSelectionVisuals()
        {
            if (englishSelectedIndicator != null)
                englishSelectedIndicator.SetActive(_selectedLocale == "en");

            if (spanishSelectedIndicator != null)
                spanishSelectedIndicator.SetActive(_selectedLocale == "es");
        }

        private void ConfirmSelection()
        {
            AudioManager.Instance?.PlayUITap();
            LocalizationManager.Instance?.SetLanguage(_selectedLocale);
            LocalizationManager.Instance?.MarkLanguageSelected();
            HidePopup();
        }

        private void ShowPopup()
        {
            if (panel != null)
                panel.SetActive(true);
        }

        private void HidePopup()
        {
            if (panel != null)
                panel.SetActive(false);
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

        private Button CreateLargeLanguageButton(Transform parent, string label, out GameObject indicator)
        {
            var go = CreateUIObject(label + "Button", parent);
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 80;
            layoutElement.flexibleWidth = 1;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.25f, 0.25f, 0.3f, 1f);

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
            labelText.fontSize = 32;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            // Selected indicator
            var indicatorGO = CreateUIObject("SelectedIndicator", go.transform);
            var indicatorRect = indicatorGO.GetComponent<RectTransform>();
            indicatorRect.anchorMin = Vector2.zero;
            indicatorRect.anchorMax = Vector2.one;
            indicatorRect.sizeDelta = Vector2.zero;
            indicatorRect.offsetMin = Vector2.zero;
            indicatorRect.offsetMax = Vector2.zero;

            var indicatorImage = indicatorGO.AddComponent<Image>();
            indicatorImage.color = new Color(0.3f, 0.6f, 1f, 0.3f);

            indicator = indicatorGO;
            indicator.SetActive(false);

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
            labelText.fontSize = 24;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            return button;
        }
    }
}
