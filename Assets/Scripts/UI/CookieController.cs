using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LuckyCharm.Core;
using LuckyCharm.Utils;

namespace LuckyCharm.UI
{
    public class CookieController : MonoBehaviour, IPointerDownHandler
    {
        [Header("Cookie Sprites")]
        [SerializeField] private Sprite[] crackStageSprites;
        [SerializeField] private Image cookieImage;

        [Header("Effects")]
        [SerializeField] private ParticleSystem crackParticles;
        [SerializeField] private GameObject cookieLeftHalf;
        [SerializeField] private GameObject cookieRightHalf;

        [Header("Animation")]
        [SerializeField] private float shakeDuration = 0.1f;
        [SerializeField] private float shakeIntensity = 10f;

        private Vector3 _originalPosition;
        private bool _isAnimating = false;

        private void Start()
        {
            _originalPosition = transform.localPosition;
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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isAnimating) return;

            var state = GameManager.Instance.CurrentState;
            if (state != GameState.CookieAvailable && state != GameState.Cracking)
                return;

            GameManager.Instance.TapCookie();
            int tapCount = GameManager.Instance.GetTapCount();
            int tapsRequired = GameManager.Instance.GetTapsRequired();

            AudioManager.Instance?.PlayCrackLight(tapCount - 1);
            HapticsManager.Instance?.VibrateLight();

            UpdateCrackVisual(tapCount, tapsRequired);

            if (tapCount >= tapsRequired)
            {
                PlayBreakAnimation();
            }
            else
            {
                StartCoroutine(ShakeAnimation());
            }
        }

        private void UpdateCrackVisual(int tapCount, int tapsRequired)
        {
            if (crackStageSprites == null || crackStageSprites.Length == 0) return;

            int spriteIndex = Mathf.Clamp(
                (int)((float)tapCount / tapsRequired * crackStageSprites.Length),
                0,
                crackStageSprites.Length - 1
            );

            if (cookieImage != null)
            {
                cookieImage.sprite = crackStageSprites[spriteIndex];
            }
        }

        private System.Collections.IEnumerator ShakeAnimation()
        {
            _isAnimating = true;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                float x = Random.Range(-1f, 1f) * shakeIntensity;
                float y = Random.Range(-1f, 1f) * shakeIntensity;
                transform.localPosition = _originalPosition + new Vector3(x, y, 0);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = _originalPosition;
            _isAnimating = false;
        }

        private void PlayBreakAnimation()
        {
            _isAnimating = true;

            AudioManager.Instance?.PlayCrackBreak();
            HapticsManager.Instance?.VibrateHeavy();

            if (crackParticles != null)
            {
                crackParticles.Play();
            }

            if (cookieImage != null)
            {
                cookieImage.gameObject.SetActive(false);
            }

            // Animate halves flying apart
            if (cookieLeftHalf != null && cookieRightHalf != null)
            {
                cookieLeftHalf.SetActive(true);
                cookieRightHalf.SetActive(true);
                StartCoroutine(AnimateCookieHalves());
            }
            else
            {
                OnBreakAnimationComplete();
            }
        }

        private System.Collections.IEnumerator AnimateCookieHalves()
        {
            float duration = 0.5f;
            float elapsed = 0f;

            Vector3 leftStart = cookieLeftHalf.transform.localPosition;
            Vector3 rightStart = cookieRightHalf.transform.localPosition;
            Vector3 leftEnd = leftStart + new Vector3(-300, -200, 0);
            Vector3 rightEnd = rightStart + new Vector3(300, -200, 0);

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float easedT = 1 - Mathf.Pow(1 - t, 3); // Ease out cubic

                cookieLeftHalf.transform.localPosition = Vector3.Lerp(leftStart, leftEnd, easedT);
                cookieRightHalf.transform.localPosition = Vector3.Lerp(rightStart, rightEnd, easedT);

                cookieLeftHalf.transform.Rotate(0, 0, -360 * Time.deltaTime);
                cookieRightHalf.transform.Rotate(0, 0, 360 * Time.deltaTime);

                elapsed += Time.deltaTime;
                yield return null;
            }

            cookieLeftHalf.SetActive(false);
            cookieRightHalf.SetActive(false);

            OnBreakAnimationComplete();
        }

        private void OnBreakAnimationComplete()
        {
            _isAnimating = false;
            GameManager.Instance.OnCrackAnimationComplete();
        }

        private void OnGameStateChanged(GameState newState)
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var state = GameManager.Instance.CurrentState;

            bool showCookie = state == GameState.CookieAvailable || state == GameState.Cracking;
            gameObject.SetActive(showCookie);

            if (showCookie && cookieImage != null && crackStageSprites != null && crackStageSprites.Length > 0)
            {
                cookieImage.sprite = crackStageSprites[0];
                cookieImage.gameObject.SetActive(true);
            }

            if (cookieLeftHalf != null) cookieLeftHalf.SetActive(false);
            if (cookieRightHalf != null) cookieRightHalf.SetActive(false);
        }
    }
}
