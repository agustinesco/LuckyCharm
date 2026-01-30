using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LuckyCharm.Core;

namespace LuckyCharm.Debug
{
    public class DebugPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button toggleButton;
        [SerializeField] private Button resetSaveButton;
        [SerializeField] private Button forceCookieButton;
        [SerializeField] private Button testNotificationButton;
        [SerializeField] private TextMeshProUGUI stateText;

        private void Start()
        {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            gameObject.SetActive(false);
            return;
#endif

            if (toggleButton != null)
                toggleButton.onClick.AddListener(TogglePanel);

            if (resetSaveButton != null)
                resetSaveButton.onClick.AddListener(ResetSave);

            if (forceCookieButton != null)
                forceCookieButton.onClick.AddListener(ForceCookie);

            if (testNotificationButton != null)
                testNotificationButton.onClick.AddListener(TestNotification);

            if (panel != null)
                panel.SetActive(false);

            GameManager.Instance.OnStateChanged += UpdateStateText;
        }

        private void TogglePanel()
        {
            if (panel != null)
                panel.SetActive(!panel.activeSelf);
        }

        private void ResetSave()
        {
            var saveData = SaveManager.Instance.Data;
            saveData.lastCookieDate = "";
            saveData.lastMessageId = "";
            saveData.history.Clear();
            saveData.seenMessageIds.Clear();
            saveData.hasSeenTutorial = false;
            SaveManager.Instance.Save();

            GameManager.Instance.CheckCookieAvailability();
        }

        private void ForceCookie()
        {
            var saveData = SaveManager.Instance.Data;
            saveData.lastCookieDate = "";
            SaveManager.Instance.Save();

            GameManager.Instance.CheckCookieAvailability();
        }

        private void TestNotification()
        {
            NotificationManager.Instance.ScheduleDailyNotification();
        }

        private void UpdateStateText(GameState state)
        {
            if (stateText != null)
                stateText.text = $"State: {state}";
        }
    }
}
