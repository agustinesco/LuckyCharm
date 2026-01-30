using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LuckyCharm.Core;
using LuckyCharm.Utils;

namespace LuckyCharm.UI
{
    public class MessageController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("UI References")]
        [SerializeField] private RectTransform rolledMessage;
        [SerializeField] private RectTransform unrolledMessage;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private CanvasGroup messageTextCanvasGroup;
        [SerializeField] private Button shareButton;

        [Header("Unroll Settings")]
        [SerializeField] private float unrollWidth = 800f;
        [SerializeField] private float minUnrollProgress = 0f;

        [Header("Tutorial")]
        [SerializeField] private GameObject tutorialHint;

        private float _unrollProgress = 0f;
        private bool _isFullyUnrolled = false;
        private bool _isDragging = false;
        private float _dragStartX;
        private float _progressAtDragStart;

        private void Start()
        {
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
            UpdateVisuals();

            if (shareButton != null)
            {
                shareButton.onClick.AddListener(OnShareClicked);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= OnGameStateChanged;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (GameManager.Instance.CurrentState != GameState.MessageRolled || _isFullyUnrolled)
                return;

            _isDragging = true;
            _dragStartX = eventData.position.x;
            _progressAtDragStart = _unrollProgress;

            AudioManager.Instance?.StartPaperRustle();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            float deltaX = eventData.position.x - _dragStartX;
            float progressDelta = deltaX / unrollWidth;
            _unrollProgress = Mathf.Clamp01(_progressAtDragStart + progressDelta);

            UpdateUnrollVisuals();

            HapticsManager.Instance?.VibrateLight();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            _isDragging = false;
            AudioManager.Instance?.StopPaperRustle();

            if (_unrollProgress >= 0.95f)
            {
                _unrollProgress = 1f;
                _isFullyUnrolled = true;
                UpdateUnrollVisuals();

                AudioManager.Instance?.PlayPaperSettle();
                HapticsManager.Instance?.VibrateSuccess();

                GameManager.Instance.OnMessageFullyUnrolled();
            }
        }

        private void UpdateUnrollVisuals()
        {
            if (rolledMessage != null)
            {
                float rolledScale = 1f - _unrollProgress;
                rolledMessage.localScale = new Vector3(rolledScale, 1f, 1f);
            }

            if (unrolledMessage != null)
            {
                float width = Mathf.Lerp(0, unrollWidth, _unrollProgress);
                unrolledMessage.sizeDelta = new Vector2(width, unrolledMessage.sizeDelta.y);
            }

            if (messageTextCanvasGroup != null)
            {
                messageTextCanvasGroup.alpha = Mathf.Clamp01((_unrollProgress - 0.3f) / 0.7f);
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var state = GameManager.Instance.CurrentState;

            bool showMessage = state == GameState.MessageRolled || state == GameState.MessageRevealed;
            gameObject.SetActive(showMessage);

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
                    messageText.text = !string.IsNullOrEmpty(localizedText) && localizedText != fortuneKey
                        ? localizedText
                        : message.text;
                }

                AudioManager.Instance?.PlayPaperWhoosh();

                if (tutorialHint != null)
                {
                    tutorialHint.SetActive(!SaveManager.Instance.Data.hasSeenTutorial);
                }
            }
            else if (state == GameState.MessageRevealed)
            {
                _unrollProgress = 1f;
                _isFullyUnrolled = true;
                UpdateUnrollVisuals();

                if (tutorialHint != null)
                {
                    tutorialHint.SetActive(false);
                    SaveManager.Instance.MarkTutorialSeen();
                }
            }

            if (shareButton != null)
            {
                shareButton.gameObject.SetActive(state == GameState.MessageRevealed);
            }
        }

        private void OnShareClicked()
        {
            AudioManager.Instance?.PlayUITap();
            ShareManager.Instance?.ShareCurrentMessage();
        }
    }
}
