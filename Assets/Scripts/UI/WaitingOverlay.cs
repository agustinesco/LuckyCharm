using UnityEngine;
using TMPro;
using LuckyCharm.Core;

namespace LuckyCharm.UI
{
    public class WaitingOverlay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private GameObject waitingContent;

        private void Start()
        {
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
            UpdateVisuals();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState == GameState.Waiting)
            {
                UpdateCountdown();
            }
        }

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

        private void OnGameStateChanged(GameState newState)
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            bool showWaiting = GameManager.Instance.CurrentState == GameState.Waiting;

            if (waitingContent != null)
            {
                waitingContent.SetActive(showWaiting);
            }

            gameObject.SetActive(showWaiting);
        }
    }
}
