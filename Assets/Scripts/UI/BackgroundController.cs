using UnityEngine;
using UnityEngine.UI;
using LuckyCharm.Core;

namespace LuckyCharm.UI
{
    public class BackgroundController : MonoBehaviour
    {
        [Header("Background Sprites")]
        [SerializeField] private Sprite timerActiveSprite;   // kepokepo_on - when timer is running
        [SerializeField] private Sprite cookieReadySprite;   // kepokepo_off - when cookie is ready

        [Header("References")]
        [SerializeField] private Image backgroundImage;

        private void Awake()
        {
            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
            }
        }

        private void Start()
        {
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
            UpdateBackground();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            UpdateBackground();
        }

        private void UpdateBackground()
        {
            if (backgroundImage == null) return;

            bool isWaiting = GameManager.Instance.CurrentState == GameState.Waiting;
            backgroundImage.sprite = isWaiting ? timerActiveSprite : cookieReadySprite;
        }
    }
}
