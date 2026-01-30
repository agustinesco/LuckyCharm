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
        public bool IsInitialized { get; private set; } = false;

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

            IsInitialized = true;
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