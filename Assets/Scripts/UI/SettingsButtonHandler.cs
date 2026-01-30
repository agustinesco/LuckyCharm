using UnityEngine;
using UnityEngine.UI;

namespace LuckyCharm.UI
{
    [RequireComponent(typeof(Button))]
    public class SettingsButtonHandler : MonoBehaviour
    {
        private Button _button;
        private SettingsPanel _settingsPanel;

        private void Start()
        {
            _button = GetComponent<Button>();
            _settingsPanel = FindObjectOfType<SettingsPanel>(true);

            if (_button != null && _settingsPanel != null)
            {
                _button.onClick.AddListener(_settingsPanel.OpenPanel);
            }
        }

        private void OnDestroy()
        {
            if (_button != null && _settingsPanel != null)
            {
                _button.onClick.RemoveListener(_settingsPanel.OpenPanel);
            }
        }
    }
}