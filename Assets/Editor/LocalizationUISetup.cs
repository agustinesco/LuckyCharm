using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace LuckyCharm.Editor
{
    public static class LocalizationUISetup
    {
        [MenuItem("LuckyCharm/Setup Localization UI")]
        public static void SetupUI()
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                UnityEngine.Debug.LogError("Canvas not found in scene!");
                return;
            }

            var managers = GameObject.Find("Managers");
            if (managers == null)
            {
                UnityEngine.Debug.LogError("Managers not found in scene!");
                return;
            }

            // Add LocalizationManager to Managers
            if (managers.GetComponentInChildren<LuckyCharm.Core.LocalizationManager>() == null)
            {
                var locManagerGO = new GameObject("LocalizationManager");
                locManagerGO.transform.SetParent(managers.transform);
                locManagerGO.AddComponent<LuckyCharm.Core.LocalizationManager>();
                UnityEngine.Debug.Log("Added LocalizationManager to Managers");
            }

            // Create Settings Button
            CreateSettingsButton(canvas.transform);

            // Create Settings Panel
            CreateSettingsPanel(canvas.transform);

            // Create Language Selection Popup
            CreateLanguageSelectionPopup(canvas.transform);

            EditorUtility.SetDirty(canvas);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(canvas.scene);

            UnityEngine.Debug.Log("Localization UI setup complete!");
        }

        private static void CreateSettingsButton(Transform canvasTransform)
        {
            if (canvasTransform.Find("SettingsButton") != null) return;

            var buttonGO = new GameObject("SettingsButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonGO.transform.SetParent(canvasTransform, false);

            var rect = buttonGO.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -50);
            rect.sizeDelta = new Vector2(80, 80);

            var image = buttonGO.GetComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var iconGO = new GameObject("Icon", typeof(RectTransform), typeof(TextMeshProUGUI));
            iconGO.transform.SetParent(buttonGO.transform, false);
            var iconRect = iconGO.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.sizeDelta = Vector2.zero;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            var iconText = iconGO.GetComponent<TextMeshProUGUI>();
            iconText.text = "⚙";
            iconText.fontSize = 48;
            iconText.alignment = TextAlignmentOptions.Center;
            iconText.color = Color.white;

            UnityEngine.Debug.Log("Created SettingsButton");
        }

        private static void CreateSettingsPanel(Transform canvasTransform)
        {
            if (canvasTransform.Find("SettingsPanel") != null) return;

            var panelGO = new GameObject("SettingsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelGO.transform.SetParent(canvasTransform, false);

            var panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var panelImage = panelGO.GetComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);

            var contentGO = new GameObject("Content", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup));
            contentGO.transform.SetParent(panelGO.transform, false);

            var contentRect = contentGO.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(600, 500);

            var contentImage = contentGO.GetComponent<Image>();
            contentImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);

            var layout = contentGO.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(40, 40, 40, 40);
            layout.spacing = 30;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;

            CreateTextElement(contentGO.transform, "Title", "Settings", 48, 80);

            var langSection = new GameObject("LanguageSection", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            langSection.transform.SetParent(contentGO.transform, false);
            var langRect = langSection.GetComponent<RectTransform>();
            langRect.sizeDelta = new Vector2(0, 100);
            var langLayout = langSection.GetComponent<HorizontalLayoutGroup>();
            langLayout.spacing = 20;
            langLayout.childAlignment = TextAnchor.MiddleCenter;
            langLayout.childControlWidth = false;
            langLayout.childControlHeight = false;

            CreateLanguageButton(langSection.transform, "EnglishButton", "English", true);
            CreateLanguageButton(langSection.transform, "SpanishButton", "Español", false);

            var closeGO = CreateButton(contentGO.transform, "CloseButton", "Close", 200, 60);

            var settingsPanel = panelGO.AddComponent<LuckyCharm.UI.SettingsPanel>();

            var so = new SerializedObject(settingsPanel);
            so.FindProperty("panel").objectReferenceValue = panelGO;
            so.FindProperty("openButton").objectReferenceValue = canvasTransform.Find("SettingsButton")?.GetComponent<Button>();
            so.FindProperty("closeButton").objectReferenceValue = closeGO.GetComponent<Button>();
            so.FindProperty("englishButton").objectReferenceValue = langSection.transform.Find("EnglishButton")?.GetComponent<Button>();
            so.FindProperty("spanishButton").objectReferenceValue = langSection.transform.Find("SpanishButton")?.GetComponent<Button>();
            so.FindProperty("englishCheckmark").objectReferenceValue = langSection.transform.Find("EnglishButton/Checkmark")?.gameObject;
            so.FindProperty("spanishCheckmark").objectReferenceValue = langSection.transform.Find("SpanishButton/Checkmark")?.gameObject;
            so.ApplyModifiedProperties();

            panelGO.SetActive(false);
            UnityEngine.Debug.Log("Created SettingsPanel");
        }

        private static void CreateLanguageSelectionPopup(Transform canvasTransform)
        {
            if (canvasTransform.Find("LanguageSelectionPopup") != null) return;

            var popupGO = new GameObject("LanguageSelectionPopup", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            popupGO.transform.SetParent(canvasTransform, false);

            var popupRect = popupGO.GetComponent<RectTransform>();
            popupRect.anchorMin = Vector2.zero;
            popupRect.anchorMax = Vector2.one;
            popupRect.sizeDelta = Vector2.zero;
            popupRect.offsetMin = Vector2.zero;
            popupRect.offsetMax = Vector2.zero;

            var popupImage = popupGO.GetComponent<Image>();
            popupImage.color = new Color(0, 0, 0, 0.9f);

            var contentGO = new GameObject("Content", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(VerticalLayoutGroup));
            contentGO.transform.SetParent(popupGO.transform, false);

            var contentRect = contentGO.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(600, 600);

            var contentImage = contentGO.GetComponent<Image>();
            contentImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);

            var layout = contentGO.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(40, 40, 60, 40);
            layout.spacing = 40;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;

            CreateTextElement(contentGO.transform, "Title", "Select Language\nSeleccionar Idioma", 42, 120);

            var langSection = new GameObject("LanguageSection", typeof(RectTransform), typeof(VerticalLayoutGroup));
            langSection.transform.SetParent(contentGO.transform, false);
            var langRect = langSection.GetComponent<RectTransform>();
            langRect.sizeDelta = new Vector2(0, 220);
            var langLayout = langSection.GetComponent<VerticalLayoutGroup>();
            langLayout.spacing = 20;
            langLayout.childAlignment = TextAnchor.MiddleCenter;
            langLayout.childControlWidth = true;
            langLayout.childControlHeight = false;

            CreateLargeLanguageButton(langSection.transform, "EnglishButton", "English", true);
            CreateLargeLanguageButton(langSection.transform, "SpanishButton", "Español", false);

            var confirmGO = CreateButton(contentGO.transform, "ConfirmButton", "Confirm / Confirmar", 400, 70);

            var popup = popupGO.AddComponent<LuckyCharm.UI.LanguageSelectionPopup>();

            var so = new SerializedObject(popup);
            so.FindProperty("panel").objectReferenceValue = popupGO;
            so.FindProperty("englishButton").objectReferenceValue = langSection.transform.Find("EnglishButton")?.GetComponent<Button>();
            so.FindProperty("spanishButton").objectReferenceValue = langSection.transform.Find("SpanishButton")?.GetComponent<Button>();
            so.FindProperty("englishSelectedIndicator").objectReferenceValue = langSection.transform.Find("EnglishButton/SelectedIndicator")?.gameObject;
            so.FindProperty("spanishSelectedIndicator").objectReferenceValue = langSection.transform.Find("SpanishButton/SelectedIndicator")?.gameObject;
            so.FindProperty("confirmButton").objectReferenceValue = confirmGO.GetComponent<Button>();
            so.ApplyModifiedProperties();

            popupGO.SetActive(false);
            UnityEngine.Debug.Log("Created LanguageSelectionPopup");
        }

        private static void CreateTextElement(Transform parent, string name, string text, int fontSize, float height)
        {
            var textGO = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            textGO.transform.SetParent(parent, false);

            var rect = textGO.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, height);

            var tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private static void CreateLanguageButton(Transform parent, string name, string label, bool hasCheckmark)
        {
            var buttonGO = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonGO.transform.SetParent(parent, false);

            var rect = buttonGO.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 80);

            var image = buttonGO.GetComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            var labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGO.transform.SetParent(buttonGO.transform, false);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var labelText = labelGO.GetComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 28;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            var checkGO = new GameObject("Checkmark", typeof(RectTransform), typeof(TextMeshProUGUI));
            checkGO.transform.SetParent(buttonGO.transform, false);
            var checkRect = checkGO.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(1, 0.5f);
            checkRect.anchorMax = new Vector2(1, 0.5f);
            checkRect.pivot = new Vector2(1, 0.5f);
            checkRect.anchoredPosition = new Vector2(-10, 0);
            checkRect.sizeDelta = new Vector2(30, 30);
            var checkText = checkGO.GetComponent<TextMeshProUGUI>();
            checkText.text = "✓";
            checkText.fontSize = 24;
            checkText.alignment = TextAlignmentOptions.Center;
            checkText.color = new Color(0.2f, 0.8f, 0.2f);
            checkGO.SetActive(hasCheckmark);
        }

        private static void CreateLargeLanguageButton(Transform parent, string name, string label, bool isSelected)
        {
            var buttonGO = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonGO.transform.SetParent(parent, false);

            var layoutElement = buttonGO.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 90;
            layoutElement.flexibleWidth = 1;

            var image = buttonGO.GetComponent<Image>();
            image.color = new Color(0.25f, 0.25f, 0.3f, 1f);

            var labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGO.transform.SetParent(buttonGO.transform, false);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var labelText = labelGO.GetComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 36;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            var indicatorGO = new GameObject("SelectedIndicator", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            indicatorGO.transform.SetParent(buttonGO.transform, false);
            var indicatorRect = indicatorGO.GetComponent<RectTransform>();
            indicatorRect.anchorMin = Vector2.zero;
            indicatorRect.anchorMax = Vector2.one;
            indicatorRect.sizeDelta = Vector2.zero;
            indicatorRect.offsetMin = Vector2.zero;
            indicatorRect.offsetMax = Vector2.zero;
            var indicatorImage = indicatorGO.GetComponent<Image>();
            indicatorImage.color = new Color(0.3f, 0.6f, 1f, 0.3f);
            indicatorGO.SetActive(isSelected);
        }

        private static GameObject CreateButton(Transform parent, string name, string label, float width, float height)
        {
            var buttonGO = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonGO.transform.SetParent(parent, false);

            var layoutElement = buttonGO.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = width;
            layoutElement.preferredHeight = height;

            var image = buttonGO.GetComponent<Image>();
            image.color = new Color(0.3f, 0.5f, 0.8f, 1f);

            var labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGO.transform.SetParent(buttonGO.transform, false);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var labelText = labelGO.GetComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 28;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            return buttonGO;
        }
    }
}