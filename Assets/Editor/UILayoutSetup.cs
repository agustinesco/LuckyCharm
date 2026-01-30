using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace LuckyCharm.Editor
{
    public static class UILayoutSetup
    {
        [MenuItem("LuckyCharm/Setup UI Layout")]
        public static void SetupUILayout()
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                UnityEngine.Debug.LogError("Canvas not found!");
                return;
            }

            // Setup Background - stretch to fill
            SetupBackground(canvas.transform);

            // Setup CookieContainer - centered
            SetupCookieContainer(canvas.transform);

            // Setup MessageContainer - centered below cookie
            SetupMessageContainer(canvas.transform);

            // Setup WaitingOverlay - stretch to fill
            SetupWaitingOverlay(canvas.transform);

            // Setup HistoryButton - top right corner
            SetupHistoryButton(canvas.transform);

            // Setup HistoryPanel - stretch to fill
            SetupHistoryPanel(canvas.transform);

            // Setup TutorialHint - below center
            SetupTutorialHint(canvas.transform);

            // Setup DebugPanel - bottom left
            SetupDebugPanel(canvas.transform);

            // Mark scene dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            UnityEngine.Debug.Log("UI Layout setup complete!");
        }

        private static void SetupBackground(Transform canvas)
        {
            var bg = canvas.Find("Background");
            if (bg == null) return;

            var rect = bg.GetComponent<RectTransform>();
            if (rect == null) return;

            // Stretch to fill entire canvas
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            var image = bg.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(0.55f, 0.35f, 0.2f); // Brown fallback color
            }
        }

        private static void SetupCookieContainer(Transform canvas)
        {
            var container = canvas.Find("CookieContainer");
            if (container == null) return;

            var rect = EnsureRectTransform(container);

            // Center in upper portion of screen
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0, 200);
            rect.sizeDelta = new Vector2(300, 300);
            rect.localScale = Vector3.one;

            // Setup Cookie child
            var cookie = container.Find("Cookie");
            if (cookie != null)
            {
                var cookieRect = EnsureRectTransform(cookie);
                cookieRect.anchorMin = new Vector2(0.5f, 0.5f);
                cookieRect.anchorMax = new Vector2(0.5f, 0.5f);
                cookieRect.anchoredPosition = Vector2.zero;
                cookieRect.sizeDelta = new Vector2(256, 256);
                cookieRect.localScale = Vector3.one;
            }

            // Setup Cookie halves (hidden initially)
            SetupCookieHalf(container.Find("CookieLeftHalf"), new Vector2(-80, 0));
            SetupCookieHalf(container.Find("CookieRightHalf"), new Vector2(80, 0));

            // Setup CrackParticles
            var particles = container.Find("CrackParticles");
            if (particles != null)
            {
                particles.localPosition = Vector3.zero;
                particles.localScale = Vector3.one;
            }
        }

        private static void SetupCookieHalf(Transform half, Vector2 offset)
        {
            if (half == null) return;

            var rect = EnsureRectTransform(half);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = new Vector2(144, 256);
            rect.localScale = Vector3.one;

            half.gameObject.SetActive(false);
        }

        private static void SetupMessageContainer(Transform canvas)
        {
            var container = canvas.Find("MessageContainer");
            if (container == null) return;

            var rect = EnsureRectTransform(container);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0, -100);
            rect.sizeDelta = new Vector2(800, 200);
            rect.localScale = Vector3.one;

            container.gameObject.SetActive(false); // Hidden initially

            // Setup RolledMessage
            var rolled = container.Find("RolledMessage");
            if (rolled != null)
            {
                var rolledRect = EnsureRectTransform(rolled);
                rolledRect.anchorMin = new Vector2(0.5f, 0.5f);
                rolledRect.anchorMax = new Vector2(0.5f, 0.5f);
                rolledRect.anchoredPosition = Vector2.zero;
                rolledRect.sizeDelta = new Vector2(128, 64);
                rolledRect.localScale = Vector3.one;
            }

            // Setup UnrolledMessage
            var unrolled = container.Find("UnrolledMessage");
            if (unrolled != null)
            {
                var unrolledRect = EnsureRectTransform(unrolled);
                unrolledRect.anchorMin = new Vector2(0.5f, 0.5f);
                unrolledRect.anchorMax = new Vector2(0.5f, 0.5f);
                unrolledRect.anchoredPosition = Vector2.zero;
                unrolledRect.sizeDelta = new Vector2(600, 150);
                unrolledRect.localScale = Vector3.one;

                // Setup MessageText
                var messageText = unrolled.Find("MessageText");
                if (messageText != null)
                {
                    var textRect = EnsureRectTransform(messageText);
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    textRect.offsetMin = new Vector2(20, 20);
                    textRect.offsetMax = new Vector2(-20, -20);
                    textRect.localScale = Vector3.one;

                    var tmp = messageText.GetComponent<TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        tmp.text = "Your fortune will appear here...";
                        tmp.fontSize = 24;
                        tmp.alignment = TextAlignmentOptions.Center;
                        tmp.color = Color.black;
                    }
                }
            }

            // Setup ShareButton
            var shareBtn = container.Find("ShareButton");
            if (shareBtn != null)
            {
                var btnRect = EnsureRectTransform(shareBtn);
                btnRect.anchorMin = new Vector2(0.5f, 0f);
                btnRect.anchorMax = new Vector2(0.5f, 0f);
                btnRect.anchoredPosition = new Vector2(0, -60);
                btnRect.sizeDelta = new Vector2(150, 50);
                btnRect.localScale = Vector3.one;
            }
        }

        private static void SetupWaitingOverlay(Transform canvas)
        {
            var overlay = canvas.Find("WaitingOverlay");
            if (overlay == null) return;

            var rect = EnsureRectTransform(overlay);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            overlay.gameObject.SetActive(false); // Hidden initially

            // Setup CountdownText
            var countdown = overlay.Find("CountdownText");
            if (countdown != null)
            {
                var textRect = EnsureRectTransform(countdown);
                textRect.anchorMin = new Vector2(0.5f, 0.5f);
                textRect.anchorMax = new Vector2(0.5f, 0.5f);
                textRect.anchoredPosition = Vector2.zero;
                textRect.sizeDelta = new Vector2(600, 100);
                textRect.localScale = Vector3.one;

                var tmp = countdown.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = "Next cookie in: 00:00:00";
                    tmp.fontSize = 36;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.color = Color.white;
                }
            }
        }

        private static void SetupHistoryButton(Transform canvas)
        {
            var btn = canvas.Find("HistoryButton");
            if (btn == null) return;

            var rect = EnsureRectTransform(btn);
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-60, -60);
            rect.sizeDelta = new Vector2(80, 80);
            rect.localScale = Vector3.one;

            var image = btn.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(0.9f, 0.85f, 0.75f);
            }
        }

        private static void SetupHistoryPanel(Transform canvas)
        {
            var historyPanel = canvas.Find("HistoryPanel");
            if (historyPanel == null) return;

            var rect = EnsureRectTransform(historyPanel);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            historyPanel.gameObject.SetActive(false); // Hidden initially

            // Setup Panel (the actual content panel)
            var panel = historyPanel.Find("Panel");
            if (panel != null)
            {
                var panelRect = EnsureRectTransform(panel);
                panelRect.anchorMin = new Vector2(0.1f, 0.1f);
                panelRect.anchorMax = new Vector2(0.9f, 0.9f);
                panelRect.offsetMin = Vector2.zero;
                panelRect.offsetMax = Vector2.zero;
                panelRect.localScale = Vector3.one;

                var image = panel.GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(0.95f, 0.92f, 0.85f);
                }

                // Setup ScrollView
                var scrollView = panel.Find("ScrollView");
                if (scrollView != null)
                {
                    var svRect = EnsureRectTransform(scrollView);
                    svRect.anchorMin = new Vector2(0, 0);
                    svRect.anchorMax = new Vector2(1, 0.9f);
                    svRect.offsetMin = new Vector2(20, 20);
                    svRect.offsetMax = new Vector2(-20, -20);
                    svRect.localScale = Vector3.one;
                }

                // Setup Content
                var content = panel.Find("Content");
                if (content != null)
                {
                    var contentRect = EnsureRectTransform(content);
                    contentRect.anchorMin = new Vector2(0, 1);
                    contentRect.anchorMax = new Vector2(1, 1);
                    contentRect.pivot = new Vector2(0.5f, 1);
                    contentRect.anchoredPosition = Vector2.zero;
                    contentRect.sizeDelta = new Vector2(0, 0);
                    contentRect.localScale = Vector3.one;
                }
            }

            // Setup CloseButton
            var closeBtn = historyPanel.Find("CloseButton");
            if (closeBtn != null)
            {
                var closeBtnRect = EnsureRectTransform(closeBtn);
                closeBtnRect.anchorMin = new Vector2(1, 1);
                closeBtnRect.anchorMax = new Vector2(1, 1);
                closeBtnRect.anchoredPosition = new Vector2(-30, -30);
                closeBtnRect.sizeDelta = new Vector2(60, 60);
                closeBtnRect.localScale = Vector3.one;

                var image = closeBtn.GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(0.8f, 0.3f, 0.3f);
                }
            }
        }

        private static void SetupTutorialHint(Transform canvas)
        {
            var hint = canvas.Find("TutorialHint");
            if (hint == null) return;

            var rect = EnsureRectTransform(hint);
            rect.anchorMin = new Vector2(0.5f, 0.3f);
            rect.anchorMax = new Vector2(0.5f, 0.3f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(400, 50);
            rect.localScale = Vector3.one;

            hint.gameObject.SetActive(false);

            // Add TextMeshProUGUI if not present
            var tmp = hint.GetComponent<TextMeshProUGUI>();
            if (tmp == null)
            {
                tmp = hint.gameObject.AddComponent<TextMeshProUGUI>();
            }
            tmp.text = "Swipe to unroll your fortune";
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
        }

        private static void SetupDebugPanel(Transform canvas)
        {
            var debug = canvas.Find("DebugPanel");
            if (debug == null) return;

            var rect = EnsureRectTransform(debug);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(100, 100);
            rect.sizeDelta = new Vector2(200, 150);
            rect.localScale = Vector3.one;

#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            debug.gameObject.SetActive(false);
#endif
        }

        private static RectTransform EnsureRectTransform(Transform t)
        {
            var rect = t.GetComponent<RectTransform>();
            if (rect == null)
            {
                // This shouldn't happen for UI elements, but just in case
                UnityEngine.Debug.LogWarning($"Missing RectTransform on {t.name}");
            }
            return rect;
        }
    }
}
