using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;
using LuckyCharm.Core;

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

            // Subscribe only to LocalizationManager to avoid duplicate UpdateText calls
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged += UpdateText;
            }
        }

        private void OnDestroy()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
            }
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