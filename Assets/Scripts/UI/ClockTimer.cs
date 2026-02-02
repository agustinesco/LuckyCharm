using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LuckyCharm.Core;

namespace LuckyCharm.UI
{
    public class ClockTimer : MonoBehaviour
    {
        [Header("Clock Visuals")]
        [SerializeField] private Image clockFillImage;
        [SerializeField] private Image clockBackgroundImage;
        [SerializeField] private RectTransform hourHand;
        [SerializeField] private RectTransform minuteHand;
        
        [Header("Text Display")]
        [SerializeField] private TextMeshProUGUI timeText;
        
        [Header("Animation")]
        [SerializeField] private float pulseSpeed = 1f;
        [SerializeField] private float pulseAmount = 0.05f;
        
        private Vector3 _originalScale;
        private float _pulseTimer;

        private void Awake()
        {
            // Auto-find references if not assigned
            if (clockFillImage == null)
                clockFillImage = transform.Find("ClockFill")?.GetComponent<Image>();
            if (clockBackgroundImage == null)
                clockBackgroundImage = transform.Find("ClockBackground")?.GetComponent<Image>();
            if (hourHand == null)
                hourHand = transform.Find("HourHand")?.GetComponent<RectTransform>();
            if (minuteHand == null)
                minuteHand = transform.Find("MinuteHand")?.GetComponent<RectTransform>();
            if (timeText == null)
                timeText = transform.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            _originalScale = transform.localScale;
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
            UpdateVisibility();
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
                UpdateClock();
                AnimatePulse();
            }
        }
        
        private void UpdateClock()
        {
            var timeUntil = GameManager.Instance.GetTimeUntilNextCookie();
            
            // Update clock hands
            float totalMinutes = (float)timeUntil.TotalMinutes;
            float hours = (float)timeUntil.TotalHours;
            float minutes = (float)timeUntil.Minutes;
            float seconds = (float)timeUntil.Seconds;
            
            // Hour hand: full rotation = 12 hours
            if (hourHand != null)
            {
                float hourAngle = -(hours % 12f) / 12f * 360f;
                hourHand.localRotation = Quaternion.Euler(0, 0, hourAngle);
            }
            
            // Minute hand: full rotation = 60 minutes
            if (minuteHand != null)
            {
                float minuteAngle = -(minutes + seconds / 60f) / 60f * 360f;
                minuteHand.localRotation = Quaternion.Euler(0, 0, minuteAngle);
            }
            
            // Fill image: represents progress until cookie is ready (24 hour cycle)
            if (clockFillImage != null)
            {
                // Assuming max wait is 24 hours, fill based on remaining time
                float maxWaitHours = 24f;
                float fillAmount = 1f - Mathf.Clamp01((float)timeUntil.TotalHours / maxWaitHours);
                clockFillImage.fillAmount = fillAmount;
            }
            
            // Update text
            if (timeText != null)
            {
                string text;
                if (timeUntil.TotalHours >= 1)
                {
                    text = $"{(int)timeUntil.TotalHours:D2}:{timeUntil.Minutes:D2}:{timeUntil.Seconds:D2}";
                }
                else
                {
                    text = $"{timeUntil.Minutes:D2}:{timeUntil.Seconds:D2}";
                }
                timeText.text = text;
            }
        }
        
        private void AnimatePulse()
        {
            _pulseTimer += Time.deltaTime * pulseSpeed;
            float pulse = 1f + Mathf.Sin(_pulseTimer * Mathf.PI * 2f) * pulseAmount;
            transform.localScale = _originalScale * pulse;
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            UpdateVisibility();
        }
        
        private void UpdateVisibility()
        {
            bool showClock = GameManager.Instance.CurrentState == GameState.Waiting;
            gameObject.SetActive(showClock);
            
            if (showClock)
            {
                transform.localScale = _originalScale;
                _pulseTimer = 0f;
            }
        }
    }
}
