using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LuckyCharm.Data;

namespace LuckyCharm.Core
{
    public class ShareManager : MonoBehaviour
    {
        public static ShareManager Instance { get; private set; }

        [Header("Share Image Settings")]
        [SerializeField] private Camera captureCamera;
        [SerializeField] private Canvas shareCanvas;
        [SerializeField] private RectTransform shareTemplate;
        [SerializeField] private TMPro.TextMeshProUGUI shareMessageText;

        [Header("Watermark")]
        [SerializeField] private GameObject watermark;

        private const int ShareImageWidth = 1920;
        private const int ShareImageHeight = 1080;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void ShareCurrentMessage()
        {
            var message = GameManager.Instance.CurrentMessage;
            if (message == null)
            {
                UnityEngine.Debug.LogError("No message to share");
                return;
            }

            StartCoroutine(CaptureAndShare(message));
        }

        private IEnumerator CaptureAndShare(FortuneMessage message)
        {
            // Setup share template
            if (shareMessageText != null)
            {
                shareMessageText.text = message.text;
            }

            if (watermark != null)
            {
                watermark.SetActive(true);
            }

            if (shareCanvas != null)
            {
                shareCanvas.gameObject.SetActive(true);
            }

            // Wait for the frame to fully render
            yield return new WaitForEndOfFrame();

            // Capture the screen using ReadPixels (more reliable on mobile)
            string imagePath = CaptureScreenshot();

            // Hide share canvas
            if (shareCanvas != null)
            {
                shareCanvas.gameObject.SetActive(false);
            }

            if (watermark != null)
            {
                watermark.SetActive(false);
            }

            if (string.IsNullOrEmpty(imagePath))
            {
                UnityEngine.Debug.LogError("Failed to capture share image");
                yield break;
            }

            // Share using native share
            ShareImage(imagePath, message.text);
        }

        private string CaptureScreenshot()
        {
            // Get screen dimensions
            int width = Screen.width;
            int height = Screen.height;

            // Create texture to hold the screenshot
            Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

            // Read pixels from the screen buffer (must be called after WaitForEndOfFrame)
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshot.Apply();

            // Save to file
            string fileName = $"LuckyCharm_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
            string filePath = Path.Combine(Application.temporaryCachePath, fileName);

            byte[] bytes = screenshot.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);

            Destroy(screenshot);

            return filePath;
        }

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
    }
}
