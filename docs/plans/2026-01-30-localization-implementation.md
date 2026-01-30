# Localization System Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add English/Spanish localization with first-launch language selection and settings panel.

**Architecture:** Unity Localization package with String Tables for UI, Messages, and Fortunes. LocalizationManager singleton handles language detection and persistence. LocalizedText component for reactive UI updates.

**Tech Stack:** Unity 2022.3, com.unity.localization, TextMeshPro, PlayerPrefs

---

### Task 1: Install Unity Localization Package

**Files:**
- Modify: `Packages/manifest.json`

**Step 1: Add localization package to manifest**

Add `"com.unity.localization": "1.5.5"` to the dependencies in manifest.json.

**Step 2: Refresh Unity and verify installation**

Run: `refresh_unity` via MCP
Check: `read_console` for any errors

**Step 3: Verify package installed**

Check that no compilation errors appear in the console.

---

### Task 2: Create Localization Settings and Locales

**Files:**
- Create: `Assets/Localization/Locales/en.asset` (via Unity)
- Create: `Assets/Localization/Locales/es.asset` (via Unity)
- Create: `Assets/Localization/LocalizationSettings.asset` (via Unity)

**Step 1: Create folder structure**

Create `Assets/Localization/Locales` and `Assets/Localization/Tables` folders via `manage_asset`.

**Step 2: Create LocalizationSettings via menu**

Execute menu item: `Edit/Project Settings...` then manually configure, OR create a setup script.

**Note:** Unity Localization assets must be created through Unity's API or Editor. We'll create an Editor script to automate this setup.

**Step 3: Create Editor setup script**

Create `Assets/Editor/LocalizationSetup.cs` that:
- Creates LocalizationSettings if missing
- Creates English (en) and Spanish (es) locales
- Creates the three String Tables (UI, Messages, Fortunes)
- Populates initial entries

**Step 4: Run setup via menu**

Execute: `LuckyCharm/Setup Localization` menu item

**Step 5: Verify setup**

Check console for success message and verify assets created.

---

### Task 3: Create LocalizationSetup Editor Script

**Files:**
- Create: `Assets/Editor/LocalizationSetup.cs`

**Step 1: Write the setup script**

```csharp
using UnityEngine;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.IO;
using System.Collections.Generic;

namespace LuckyCharm.Editor
{
    public static class LocalizationSetup
    {
        [MenuItem("LuckyCharm/Setup Localization")]
        public static void Setup()
        {
            CreateFolders();
            var settings = CreateLocalizationSettings();
            var enLocale = CreateLocale("en", "English");
            var esLocale = CreateLocale("es", "Spanish");

            settings.GetAvailableLocales().AddLocale(enLocale);
            settings.GetAvailableLocales().AddLocale(esLocale);

            CreateStringTables(enLocale, esLocale);

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Localization setup complete!");
        }

        private static void CreateFolders()
        {
            string[] folders = {
                "Assets/Localization",
                "Assets/Localization/Locales",
                "Assets/Localization/Tables"
            };

            foreach (var folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    var parent = Path.GetDirectoryName(folder).Replace("\\", "/");
                    var name = Path.GetFileName(folder);
                    AssetDatabase.CreateFolder(parent, name);
                }
            }
        }

        private static LocalizationSettings CreateLocalizationSettings()
        {
            const string path = "Assets/Localization/LocalizationSettings.asset";

            var settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(path);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LocalizationSettings>();
                AssetDatabase.CreateAsset(settings, path);

                // Set as active localization settings
                LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            }
            return settings;
        }

        private static Locale CreateLocale(string code, string name)
        {
            string path = $"Assets/Localization/Locales/{code}.asset";

            var locale = AssetDatabase.LoadAssetAtPath<Locale>(path);
            if (locale == null)
            {
                locale = Locale.CreateLocale(new UnityEngine.Localization.LocaleIdentifier(code));
                locale.name = name;
                AssetDatabase.CreateAsset(locale, path);
            }
            return locale;
        }

        private static void CreateStringTables(Locale enLocale, Locale esLocale)
        {
            CreateUITable(enLocale, esLocale);
            CreateMessagesTable(enLocale, esLocale);
            CreateFortunesTable(enLocale, esLocale);
        }

        private static void CreateUITable(Locale enLocale, Locale esLocale)
        {
            var entries = new Dictionary<string, (string en, string es)>
            {
                { "settings_title", ("Settings", "Configuración") },
                { "language_label", ("Language", "Idioma") },
                { "history_title", ("History", "Historial") },
                { "close_button", ("Close", "Cerrar") },
                { "share_button", ("Share", "Compartir") },
                { "select_language_title", ("Select Language", "Seleccionar Idioma") },
                { "confirm_button", ("Confirm", "Confirmar") },
                { "english", ("English", "Inglés") },
                { "spanish", ("Spanish", "Español") },
                { "tap_cookie", ("Tap the cookie!", "¡Toca la galleta!") },
                { "swipe_to_unroll", ("Swipe to unroll", "Desliza para desenrollar") }
            };

            CreateTable("UI", entries, enLocale, esLocale);
        }

        private static void CreateMessagesTable(Locale enLocale, Locale esLocale)
        {
            var entries = new Dictionary<string, (string en, string es)>
            {
                { "countdown_hours", ("Next cookie in\n{0}h {1}m", "Siguiente galleta en\n{0}h {1}m") },
                { "countdown_minutes", ("Next cookie in\n{0}m {1}s", "Siguiente galleta en\n{0}m {1}s") },
                { "countdown_seconds", ("Next cookie in\n{0}s", "Siguiente galleta en\n{0}s") },
                { "notification_title", ("Your Lucky Cookie is Ready!", "¡Tu Galleta de la Suerte está Lista!") },
                { "notification_body", ("Come crack open today's fortune and discover your message.", "Ven a abrir la fortuna de hoy y descubre tu mensaje.") },
                { "share_subject", ("My Lucky Cookie Fortune", "Mi Fortuna de la Galleta") },
                { "share_text", ("\"{0}\"\n\nShared from LuckyCharm", "\"{0}\"\n\nCompartido desde LuckyCharm") }
            };

            CreateTable("Messages", entries, enLocale, esLocale);
        }

        private static void CreateFortunesTable(Locale enLocale, Locale esLocale)
        {
            var entries = new Dictionary<string, (string en, string es)>
            {
                { "fortune_1", ("The best time to plant a tree was 20 years ago. The second best time is now.", "El mejor momento para plantar un árbol fue hace 20 años. El segundo mejor momento es ahora.") },
                { "fortune_2", ("Your kindness is a gift that keeps giving.", "Tu amabilidad es un regalo que sigue dando.") },
                { "fortune_3", ("Courage is not the absence of fear, but action in spite of it.", "El coraje no es la ausencia de miedo, sino actuar a pesar de él.") },
                { "fortune_4", ("Today's struggle is tomorrow's strength.", "La lucha de hoy es la fuerza de mañana.") },
                { "fortune_5", ("A smile is a curve that sets everything straight.", "Una sonrisa es una curva que lo endereza todo.") },
                { "fortune_6", ("The journey of a thousand miles begins with a single step.", "El viaje de mil millas comienza con un solo paso.") },
                { "fortune_7", ("Be the reason someone believes in the goodness of people.", "Sé la razón por la que alguien cree en la bondad de las personas.") },
                { "fortune_8", ("What you seek is seeking you.", "Lo que buscas te está buscando.") },
                { "fortune_9", ("Stars can't shine without darkness.", "Las estrellas no pueden brillar sin oscuridad.") },
                { "fortune_10", ("Your potential is endless. Go do what you were created to do.", "Tu potencial es infinito. Ve y haz aquello para lo que fuiste creado.") },
                { "fortune_11", ("The only way to do great work is to love what you do.", "La única forma de hacer un gran trabajo es amar lo que haces.") },
                { "fortune_12", ("Small acts of kindness create ripples of change.", "Pequeños actos de bondad crean ondas de cambio.") },
                { "fortune_13", ("You are braver than you believe, stronger than you seem.", "Eres más valiente de lo que crees, más fuerte de lo que pareces.") },
                { "fortune_14", ("Every expert was once a beginner.", "Todo experto fue una vez un principiante.") },
                { "fortune_15", ("Life is short. Smile while you still have teeth.", "La vida es corta. Sonríe mientras aún tengas dientes.") },
                { "fortune_16", ("Believe you can and you're halfway there.", "Cree que puedes y ya estás a mitad de camino.") },
                { "fortune_17", ("In a world where you can be anything, be kind.", "En un mundo donde puedes ser cualquier cosa, sé amable.") },
                { "fortune_18", ("Fortune favors the bold.", "La fortuna favorece a los audaces.") },
                { "fortune_19", ("Growth is painful. Change is painful. But nothing is as painful as staying stuck.", "Crecer es doloroso. Cambiar es doloroso. Pero nada es tan doloroso como quedarse estancado.") },
                { "fortune_20", ("Life is too important to be taken seriously.", "La vida es demasiado importante para tomársela en serio.") },
                { "fortune_21", ("Your only limit is your mind.", "Tu único límite es tu mente.") },
                { "fortune_22", ("Happiness is found when you stop comparing yourself to others.", "La felicidad se encuentra cuando dejas de compararte con los demás.") },
                { "fortune_23", ("The universe is conspiring in your favor.", "El universo está conspirando a tu favor.") },
                { "fortune_24", ("Be fearless in the pursuit of what sets your soul on fire.", "Sé intrépido en la búsqueda de lo que enciende tu alma.") },
                { "fortune_25", ("You are allowed to be both a masterpiece and a work in progress.", "Tienes permiso de ser tanto una obra maestra como un trabajo en progreso.") },
                { "fortune_26", ("The sun will rise and we will try again.", "El sol saldrá y lo intentaremos de nuevo.") },
                { "fortune_27", ("Your energy introduces you before you even speak.", "Tu energía te presenta antes de que hables.") },
                { "fortune_28", ("Do something today that your future self will thank you for.", "Haz algo hoy por lo que tu yo del futuro te agradecerá.") },
                { "fortune_29", ("It's okay to not be okay, as long as you don't stay that way.", "Está bien no estar bien, mientras no te quedes así.") },
                { "fortune_30", ("Throw kindness around like confetti.", "Esparce amabilidad como confeti.") },
                { "fortune_31", ("The best view comes after the hardest climb.", "La mejor vista viene después de la subida más difícil.") },
                { "fortune_32", ("You don't have to be perfect to be amazing.", "No tienes que ser perfecto para ser increíble.") },
                { "fortune_33", ("Today is a good day to have a good day.", "Hoy es un buen día para tener un buen día.") },
                { "fortune_34", ("Your vibe attracts your tribe.", "Tu vibra atrae a tu tribu.") },
                { "fortune_35", ("Be a voice, not an echo.", "Sé una voz, no un eco.") },
                { "fortune_36", ("Difficult roads often lead to beautiful destinations.", "Los caminos difíciles a menudo llevan a destinos hermosos.") },
                { "fortune_37", ("You are capable of more than you know.", "Eres capaz de más de lo que sabes.") },
                { "fortune_38", ("Life begins at the end of your comfort zone.", "La vida comienza al final de tu zona de confort.") },
                { "fortune_39", ("The comeback is always stronger than the setback.", "El regreso siempre es más fuerte que el retroceso.") },
                { "fortune_40", ("Make today so awesome that yesterday gets jealous.", "Haz que hoy sea tan increíble que el ayer tenga envidia.") },
                { "fortune_41", ("You are never too old to set another goal or dream a new dream.", "Nunca eres demasiado viejo para fijarte otra meta o soñar un nuevo sueño.") },
                { "fortune_42", ("Kindness is free. Sprinkle it everywhere.", "La amabilidad es gratis. Espárcela por todas partes.") },
                { "fortune_43", ("What lies behind us and what lies before us are tiny matters compared to what lies within us.", "Lo que está detrás y lo que está delante de nosotros son pequeñeces comparado con lo que está dentro de nosotros.") },
                { "fortune_44", ("Take the risk or lose the chance.", "Toma el riesgo o pierde la oportunidad.") },
                { "fortune_45", ("Bloom where you are planted.", "Florece donde estés plantado.") },
                { "fortune_46", ("Good things come to those who hustle.", "Las cosas buenas llegan a quienes se esfuerzan.") },
                { "fortune_47", ("Let your faith be bigger than your fear.", "Deja que tu fe sea más grande que tu miedo.") },
                { "fortune_48", ("The only impossible journey is the one you never begin.", "El único viaje imposible es el que nunca comienzas.") },
                { "fortune_49", ("You are the author of your own story.", "Eres el autor de tu propia historia.") },
                { "fortune_50", ("Dream big. Work hard. Stay humble.", "Sueña en grande. Trabaja duro. Mantente humilde.") }
            };

            CreateTable("Fortunes", entries, enLocale, esLocale);
        }

        private static void CreateTable(string tableName, Dictionary<string, (string en, string es)> entries, Locale enLocale, Locale esLocale)
        {
            var collection = LocalizationEditorSettings.GetStringTableCollection(tableName);

            if (collection == null)
            {
                collection = LocalizationEditorSettings.CreateStringTableCollection(tableName, $"Assets/Localization/Tables");
                collection.AddNewTable(enLocale.Identifier);
                collection.AddNewTable(esLocale.Identifier);
            }

            var enTable = collection.GetTable(enLocale.Identifier) as StringTable;
            var esTable = collection.GetTable(esLocale.Identifier) as StringTable;

            foreach (var entry in entries)
            {
                collection.SharedData.AddKey(entry.Key);

                if (enTable != null)
                    enTable.AddEntry(entry.Key, entry.Value.en);

                if (esTable != null)
                    esTable.AddEntry(entry.Key, entry.Value.es);
            }

            EditorUtility.SetDirty(collection);
            if (enTable != null) EditorUtility.SetDirty(enTable);
            if (esTable != null) EditorUtility.SetDirty(esTable);
        }
    }
}
```

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

**Step 3: Execute setup menu item**

Run: `execute_menu_item` with path `LuckyCharm/Setup Localization`

**Step 4: Verify localization assets created**

Check that `Assets/Localization/` contains LocalizationSettings.asset, Locales/, and Tables/.

---

### Task 4: Create LocalizationManager Script

**Files:**
- Create: `Assets/Scripts/Core/LocalizationManager.cs`

**Step 1: Write the LocalizationManager**

```csharp
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace LuckyCharm.Core
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        public event Action OnLanguageChanged;

        private const string LocalePrefsKey = "selected_locale";
        private const string FirstLaunchKey = "has_selected_language";

        public bool IsFirstLaunch => PlayerPrefs.GetInt(FirstLaunchKey, 0) == 0;
        public Locale CurrentLocale => LocalizationSettings.SelectedLocale;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            StartCoroutine(InitializeLocalization());
        }

        private IEnumerator InitializeLocalization()
        {
            yield return LocalizationSettings.InitializationOperation;

            if (!IsFirstLaunch)
            {
                string savedLocale = PlayerPrefs.GetString(LocalePrefsKey, "en");
                yield return SetLanguageAsync(savedLocale);
            }
            else
            {
                string systemLanguage = GetSystemLanguageCode();
                yield return SetLanguageAsync(systemLanguage);
            }
        }

        private string GetSystemLanguageCode()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.Spanish => "es",
                _ => "en"
            };
        }

        public string GetDetectedLanguageCode()
        {
            return GetSystemLanguageCode();
        }

        public void SetLanguage(string localeCode)
        {
            StartCoroutine(SetLanguageAsync(localeCode));
        }

        private IEnumerator SetLanguageAsync(string localeCode)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            Locale targetLocale = null;

            foreach (var locale in locales)
            {
                if (locale.Identifier.Code == localeCode)
                {
                    targetLocale = locale;
                    break;
                }
            }

            if (targetLocale != null)
            {
                LocalizationSettings.SelectedLocale = targetLocale;
                PlayerPrefs.SetString(LocalePrefsKey, localeCode);
                PlayerPrefs.Save();

                yield return null;

                OnLanguageChanged?.Invoke();
            }
        }

        public void MarkLanguageSelected()
        {
            PlayerPrefs.SetInt(FirstLaunchKey, 1);
            PlayerPrefs.Save();
        }

        public string GetLocalizedString(string tableName, string key)
        {
            var table = LocalizationSettings.StringDatabase.GetTable(tableName);
            if (table != null)
            {
                var entry = table.GetEntry(key);
                if (entry != null)
                {
                    return entry.GetLocalizedString();
                }
            }
            return key;
        }

        public string GetLocalizedString(string tableName, string key, params object[] args)
        {
            string format = GetLocalizedString(tableName, key);
            return string.Format(format, args);
        }
    }
}
```

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

---

### Task 5: Create LocalizedText Component

**Files:**
- Create: `Assets/Scripts/UI/LocalizedText.cs`

**Step 1: Write the LocalizedText component**

```csharp
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using TMPro;

namespace LuckyCharm.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string tableName = "UI";
        [SerializeField] private string entryKey;

        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            UpdateText();

            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged += UpdateText;
            }

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void OnDestroy()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
            }

            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged(Locale locale)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (_text == null || string.IsNullOrEmpty(entryKey)) return;

            if (LocalizationManager.Instance != null)
            {
                _text.text = LocalizationManager.Instance.GetLocalizedString(tableName, entryKey);
            }
        }

        public void SetKey(string table, string key)
        {
            tableName = table;
            entryKey = key;
            UpdateText();
        }
    }
}
```

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

---

### Task 6: Create SettingsPanel Script

**Files:**
- Create: `Assets/Scripts/UI/SettingsPanel.cs`

**Step 1: Write the SettingsPanel script**

```csharp
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

        private void Start()
        {
            if (openButton != null)
                openButton.onClick.AddListener(OpenPanel);

            if (closeButton != null)
                closeButton.onClick.AddListener(ClosePanel);

            if (englishButton != null)
                englishButton.onClick.AddListener(() => SelectLanguage("en"));

            if (spanishButton != null)
                spanishButton.onClick.AddListener(() => SelectLanguage("es"));

            if (panel != null)
                panel.SetActive(false);

            if (LocalizationManager.Instance != null)
                LocalizationManager.Instance.OnLanguageChanged += UpdateCheckmarks;

            UpdateCheckmarks();
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
    }
}
```

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

---

### Task 7: Create LanguageSelectionPopup Script

**Files:**
- Create: `Assets/Scripts/UI/LanguageSelectionPopup.cs`

**Step 1: Write the LanguageSelectionPopup script**

```csharp
using UnityEngine;
using UnityEngine.UI;
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

        private void Start()
        {
            if (englishButton != null)
                englishButton.onClick.AddListener(() => SelectLanguage("en"));

            if (spanishButton != null)
                spanishButton.onClick.AddListener(() => SelectLanguage("es"));

            if (confirmButton != null)
                confirmButton.onClick.AddListener(ConfirmSelection);

            // Check if this is first launch
            StartCoroutine(CheckFirstLaunch());
        }

        private System.Collections.IEnumerator CheckFirstLaunch()
        {
            // Wait for LocalizationManager to initialize
            yield return new WaitUntil(() => LocalizationManager.Instance != null);
            yield return new WaitForSeconds(0.5f);

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
    }
}
```

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

---

### Task 8: Update WaitingOverlay for Localization

**Files:**
- Modify: `Assets/Scripts/UI/WaitingOverlay.cs`

**Step 1: Update WaitingOverlay to use localized strings**

Replace the `UpdateCountdown` method to use LocalizationManager:

```csharp
private void UpdateCountdown()
{
    var timeUntil = GameManager.Instance.GetTimeUntilNextCookie();

    if (countdownText != null)
    {
        string text;
        if (timeUntil.TotalHours >= 1)
        {
            text = LocalizationManager.Instance?.GetLocalizedString("Messages", "countdown_hours", (int)timeUntil.TotalHours, timeUntil.Minutes)
                ?? $"Next cookie in\n{(int)timeUntil.TotalHours}h {timeUntil.Minutes}m";
        }
        else if (timeUntil.TotalMinutes >= 1)
        {
            text = LocalizationManager.Instance?.GetLocalizedString("Messages", "countdown_minutes", timeUntil.Minutes, timeUntil.Seconds)
                ?? $"Next cookie in\n{timeUntil.Minutes}m {timeUntil.Seconds}s";
        }
        else
        {
            text = LocalizationManager.Instance?.GetLocalizedString("Messages", "countdown_seconds", timeUntil.Seconds)
                ?? $"Next cookie in\n{timeUntil.Seconds}s";
        }
        countdownText.text = text;
    }
}
```

Also add namespace: `using LuckyCharm.Core;` at the top.

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

---

### Task 9: Update NotificationManager for Localization

**Files:**
- Modify: `Assets/Scripts/Core/NotificationManager.cs`

**Step 1: Update notification text to use localized strings**

Remove the const strings and update `ScheduleDailyNotification`:

```csharp
public void ScheduleDailyNotification()
{
    CancelAllNotifications();

    DateTime now = DateTime.Now;
    DateTime scheduledTime;

    if (now.Hour >= 10)
    {
        scheduledTime = now.Date.AddDays(1).AddHours(10);
    }
    else
    {
        scheduledTime = now.Date.AddHours(10);
    }

    TimeSpan delay = scheduledTime - now;

    string title = LocalizationManager.Instance?.GetLocalizedString("Messages", "notification_title")
        ?? "Your Lucky Cookie is Ready!";
    string body = LocalizationManager.Instance?.GetLocalizedString("Messages", "notification_body")
        ?? "Come crack open today's fortune and discover your message.";

#if UNITY_ANDROID
    var notification = new AndroidNotification
    {
        Title = title,
        Text = body,
        FireTime = scheduledTime,
        SmallIcon = "icon_small",
        LargeIcon = "icon_large"
    };
    AndroidNotificationCenter.SendNotification(notification, ChannelId);
#elif UNITY_IOS
    var timeTrigger = new iOSNotificationTimeIntervalTrigger()
    {
        TimeInterval = delay,
        Repeats = false
    };

    var notification = new iOSNotification()
    {
        Identifier = "daily_cookie",
        Title = title,
        Body = body,
        ShowInForeground = true,
        ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
        Trigger = timeTrigger
    };
    iOSNotificationCenter.ScheduleNotification(notification);
#endif

    UnityEngine.Debug.Log($"Notification scheduled for {scheduledTime}");
}
```

Remove the const fields `NotificationTitle` and `NotificationText`.

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

---

### Task 10: Update ShareManager for Localization

**Files:**
- Modify: `Assets/Scripts/Core/ShareManager.cs`

**Step 1: Update share text to use localized strings**

Update the `ShareImage` method:

```csharp
private void ShareImage(string imagePath, string messageText)
{
#if UNITY_EDITOR
    UnityEngine.Debug.Log($"Share image: {imagePath}");
    UnityEngine.Debug.Log($"Message: {messageText}");
#else
    string subject = LocalizationManager.Instance?.GetLocalizedString("Messages", "share_subject")
        ?? "My Lucky Cookie Fortune";
    string shareText = LocalizationManager.Instance?.GetLocalizedString("Messages", "share_text", messageText)
        ?? $"\"{messageText}\"\n\nShared from LuckyCharm";

    new NativeShare()
        .AddFile(imagePath)
        .SetSubject(subject)
        .SetText(shareText)
        .Share();
#endif
}
```

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

---

### Task 11: Update MessageController for Localized Fortunes

**Files:**
- Modify: `Assets/Scripts/UI/MessageController.cs`

**Step 1: Update to get fortune from Fortunes table**

In the `UpdateVisuals` method, update the message text section:

```csharp
if (state == GameState.MessageRolled)
{
    _unrollProgress = 0f;
    _isFullyUnrolled = false;
    UpdateUnrollVisuals();

    var message = GameManager.Instance.CurrentMessage;
    if (message != null && messageText != null)
    {
        // Get localized fortune text
        string fortuneKey = $"fortune_{message.id}";
        string localizedText = LocalizationManager.Instance?.GetLocalizedString("Fortunes", fortuneKey);
        messageText.text = !string.IsNullOrEmpty(localizedText) ? localizedText : message.text;
    }

    AudioManager.Instance?.PlayPaperWhoosh();

    if (tutorialHint != null)
    {
        tutorialHint.SetActive(!SaveManager.Instance.Data.hasSeenTutorial);
    }
}
```

Add the using statement at the top: `using LuckyCharm.Core;`

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

---

### Task 12: Create UI Setup Editor Script

**Files:**
- Create: `Assets/Editor/LocalizationUISetup.cs`

**Step 1: Write UI setup script that creates all UI elements**

```csharp
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
                Debug.LogError("Canvas not found in scene!");
                return;
            }

            var managers = GameObject.Find("Managers");
            if (managers == null)
            {
                Debug.LogError("Managers not found in scene!");
                return;
            }

            // Add LocalizationManager to Managers
            if (managers.GetComponentInChildren<LuckyCharm.Core.LocalizationManager>() == null)
            {
                var locManagerGO = new GameObject("LocalizationManager");
                locManagerGO.transform.SetParent(managers.transform);
                locManagerGO.AddComponent<LuckyCharm.Core.LocalizationManager>();
                Debug.Log("Added LocalizationManager to Managers");
            }

            // Create Settings Button
            CreateSettingsButton(canvas.transform);

            // Create Settings Panel
            CreateSettingsPanel(canvas.transform);

            // Create Language Selection Popup
            CreateLanguageSelectionPopup(canvas.transform);

            EditorUtility.SetDirty(canvas);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(canvas.scene);

            Debug.Log("Localization UI setup complete!");
        }

        private static void CreateSettingsButton(Transform canvasTransform)
        {
            // Check if already exists
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

            // Add gear icon text (placeholder)
            var iconGO = new GameObject("Icon", typeof(RectTransform), typeof(TextMeshProUGUI));
            iconGO.transform.SetParent(buttonGO.transform, false);
            var iconRect = iconGO.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.sizeDelta = Vector2.zero;
            var iconText = iconGO.GetComponent<TextMeshProUGUI>();
            iconText.text = "⚙";
            iconText.fontSize = 48;
            iconText.alignment = TextAlignmentOptions.Center;
            iconText.color = Color.white;

            Debug.Log("Created SettingsButton");
        }

        private static void CreateSettingsPanel(Transform canvasTransform)
        {
            if (canvasTransform.Find("SettingsPanel") != null) return;

            // Create panel container
            var panelGO = new GameObject("SettingsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelGO.transform.SetParent(canvasTransform, false);

            var panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;

            var panelImage = panelGO.GetComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);

            // Create content container
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

            // Title
            CreateTextElement(contentGO.transform, "Title", "Settings", 48, 80);

            // Language section
            var langSection = new GameObject("LanguageSection", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            langSection.transform.SetParent(contentGO.transform, false);
            var langRect = langSection.GetComponent<RectTransform>();
            langRect.sizeDelta = new Vector2(0, 100);
            var langLayout = langSection.GetComponent<HorizontalLayoutGroup>();
            langLayout.spacing = 20;
            langLayout.childAlignment = TextAnchor.MiddleCenter;
            langLayout.childControlWidth = false;
            langLayout.childControlHeight = false;

            // English button
            CreateLanguageButton(langSection.transform, "EnglishButton", "English", true);

            // Spanish button
            CreateLanguageButton(langSection.transform, "SpanishButton", "Español", false);

            // Close button
            var closeGO = CreateButton(contentGO.transform, "CloseButton", "Close", 200, 60);

            // Add SettingsPanel component
            var settingsPanel = panelGO.AddComponent<LuckyCharm.UI.SettingsPanel>();

            // Wire up references via SerializedObject
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
            Debug.Log("Created SettingsPanel");
        }

        private static void CreateLanguageSelectionPopup(Transform canvasTransform)
        {
            if (canvasTransform.Find("LanguageSelectionPopup") != null) return;

            // Create popup container
            var popupGO = new GameObject("LanguageSelectionPopup", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            popupGO.transform.SetParent(canvasTransform, false);

            var popupRect = popupGO.GetComponent<RectTransform>();
            popupRect.anchorMin = Vector2.zero;
            popupRect.anchorMax = Vector2.one;
            popupRect.sizeDelta = Vector2.zero;

            var popupImage = popupGO.GetComponent<Image>();
            popupImage.color = new Color(0, 0, 0, 0.9f);

            // Create content container
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

            // Title
            CreateTextElement(contentGO.transform, "Title", "Select Language\nSeleccionar Idioma", 42, 120);

            // Language buttons section
            var langSection = new GameObject("LanguageSection", typeof(RectTransform), typeof(VerticalLayoutGroup));
            langSection.transform.SetParent(contentGO.transform, false);
            var langRect = langSection.GetComponent<RectTransform>();
            langRect.sizeDelta = new Vector2(0, 220);
            var langLayout = langSection.GetComponent<VerticalLayoutGroup>();
            langLayout.spacing = 20;
            langLayout.childAlignment = TextAnchor.MiddleCenter;
            langLayout.childControlWidth = true;
            langLayout.childControlHeight = false;

            // English button
            CreateLargeLanguageButton(langSection.transform, "EnglishButton", "🇺🇸  English", true);

            // Spanish button
            CreateLargeLanguageButton(langSection.transform, "SpanishButton", "🇪🇸  Español", false);

            // Confirm button
            var confirmGO = CreateButton(contentGO.transform, "ConfirmButton", "Confirm / Confirmar", 400, 70);

            // Add LanguageSelectionPopup component
            var popup = popupGO.AddComponent<LuckyCharm.UI.LanguageSelectionPopup>();

            // Wire up references
            var so = new SerializedObject(popup);
            so.FindProperty("panel").objectReferenceValue = popupGO;
            so.FindProperty("englishButton").objectReferenceValue = langSection.transform.Find("EnglishButton")?.GetComponent<Button>();
            so.FindProperty("spanishButton").objectReferenceValue = langSection.transform.Find("SpanishButton")?.GetComponent<Button>();
            so.FindProperty("englishSelectedIndicator").objectReferenceValue = langSection.transform.Find("EnglishButton/SelectedIndicator")?.gameObject;
            so.FindProperty("spanishSelectedIndicator").objectReferenceValue = langSection.transform.Find("SpanishButton/SelectedIndicator")?.gameObject;
            so.FindProperty("confirmButton").objectReferenceValue = confirmGO.GetComponent<Button>();
            so.ApplyModifiedProperties();

            popupGO.SetActive(false);
            Debug.Log("Created LanguageSelectionPopup");
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

            // Label
            var labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGO.transform.SetParent(buttonGO.transform, false);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            var labelText = labelGO.GetComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 28;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            // Checkmark
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

            // Label
            var labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGO.transform.SetParent(buttonGO.transform, false);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            var labelText = labelGO.GetComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 36;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            // Selected indicator (border effect)
            var indicatorGO = new GameObject("SelectedIndicator", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline));
            indicatorGO.transform.SetParent(buttonGO.transform, false);
            var indicatorRect = indicatorGO.GetComponent<RectTransform>();
            indicatorRect.anchorMin = Vector2.zero;
            indicatorRect.anchorMax = Vector2.one;
            indicatorRect.sizeDelta = Vector2.zero;
            var indicatorImage = indicatorGO.GetComponent<Image>();
            indicatorImage.color = new Color(0.3f, 0.6f, 1f, 0.3f);
            var outline = indicatorGO.GetComponent<Outline>();
            outline.effectColor = new Color(0.3f, 0.6f, 1f, 1f);
            outline.effectDistance = new Vector2(3, 3);
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

            // Label
            var labelGO = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGO.transform.SetParent(buttonGO.transform, false);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            var labelText = labelGO.GetComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 28;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            return buttonGO;
        }
    }
}
```

**Step 2: Verify script compiles**

Run: `refresh_unity` and check console for errors.

---

### Task 13: Run Setup and Verify

**Step 1: Run Localization Setup**

Execute menu: `LuckyCharm/Setup Localization`

**Step 2: Run UI Setup**

Execute menu: `LuckyCharm/Setup Localization UI`

**Step 3: Save scene**

Save the scene via `manage_scene` with action `save`.

**Step 4: Verify everything works**

- Check console for any errors
- Enter play mode and verify language popup appears
- Select a language and confirm
- Check settings panel opens with gear button
- Verify language switching works

---

### Task 14: Test and Polish

**Step 1: Enter play mode**

Use `manage_editor` to enter play mode.

**Step 2: Verify first-launch popup**

Clear PlayerPrefs if needed to test first launch.

**Step 3: Test language switching**

Switch between English and Spanish, verify all text updates.

**Step 4: Stop play mode and save**

Exit play mode and save the scene.

---

## Summary

This plan implements a complete localization system:
1. Unity Localization package installation
2. String Tables for UI, Messages, and Fortunes (50 translations)
3. LocalizationManager for language detection and persistence
4. LocalizedText component for reactive UI
5. SettingsPanel with language selection
6. First-launch LanguageSelectionPopup
7. Updates to existing scripts (WaitingOverlay, NotificationManager, ShareManager, MessageController)
8. Editor scripts for automated setup
