using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using LuckyCharm.UI;
using LuckyCharm.Core;
using LuckyCharm.Utils;

namespace LuckyCharm.Editor
{
    public static class RebuildUI
    {
        [MenuItem("LuckyCharm/Rebuild UI (Fix RectTransforms)")]
        public static void RebuildUIHierarchy()
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                UnityEngine.Debug.LogError("Canvas not found! Creating new canvas...");
                CreateCanvas();
                canvas = GameObject.Find("Canvas");
            }

            // Delete old UI children and rebuild
            RebuildCookieContainer(canvas.transform);
            RebuildMessageContainer(canvas.transform);
            RebuildWaitingOverlay(canvas.transform);
            RebuildHistoryButton(canvas.transform);
            RebuildHistoryPanel(canvas.transform);
            RebuildTutorialHint(canvas.transform);
            RebuildDebugPanel(canvas.transform);
            RebuildBackground(canvas.transform);

            // Wire up references
            SceneWireup.WireUpSceneReferences();

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            UnityEngine.Debug.Log("UI Rebuilt successfully!");
        }

        private static void CreateCanvas()
        {
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // Create EventSystem if needed
            if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        private static void RebuildBackground(Transform canvas)
        {
            DestroyChild(canvas, "Background");

            var bg = CreateUIElement("Background", canvas);
            var rect = bg.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = bg.AddComponent<Image>();
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/background.png");
            if (sprite != null)
                image.sprite = sprite;
            else
                image.color = new Color(0.55f, 0.35f, 0.2f);

            // Move to back
            bg.transform.SetAsFirstSibling();
        }

        private static void RebuildCookieContainer(Transform canvas)
        {
            DestroyChild(canvas, "CookieContainer");

            var container = CreateUIElement("CookieContainer", canvas);
            var containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.6f);
            containerRect.anchorMax = new Vector2(0.5f, 0.6f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(300, 300);

            // Add CookieController
            container.AddComponent<CookieController>();

            // Cookie (main image)
            var cookie = CreateUIElement("Cookie", container.transform);
            var cookieRect = cookie.GetComponent<RectTransform>();
            cookieRect.anchorMin = new Vector2(0.5f, 0.5f);
            cookieRect.anchorMax = new Vector2(0.5f, 0.5f);
            cookieRect.anchoredPosition = Vector2.zero;
            cookieRect.sizeDelta = new Vector2(256, 256);

            var cookieImage = cookie.AddComponent<Image>();
            var cookieSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/cookie_intact.png");
            if (cookieSprite != null)
                cookieImage.sprite = cookieSprite;

            // Cookie Left Half
            var leftHalf = CreateUIElement("CookieLeftHalf", container.transform);
            var leftRect = leftHalf.GetComponent<RectTransform>();
            leftRect.anchorMin = new Vector2(0.5f, 0.5f);
            leftRect.anchorMax = new Vector2(0.5f, 0.5f);
            leftRect.anchoredPosition = new Vector2(-50, 0);
            leftRect.sizeDelta = new Vector2(144, 256);

            var leftImage = leftHalf.AddComponent<Image>();
            var leftSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/cookie_left_half.png");
            if (leftSprite != null)
                leftImage.sprite = leftSprite;
            leftHalf.SetActive(false);

            // Cookie Right Half
            var rightHalf = CreateUIElement("CookieRightHalf", container.transform);
            var rightRect = rightHalf.GetComponent<RectTransform>();
            rightRect.anchorMin = new Vector2(0.5f, 0.5f);
            rightRect.anchorMax = new Vector2(0.5f, 0.5f);
            rightRect.anchoredPosition = new Vector2(50, 0);
            rightRect.sizeDelta = new Vector2(144, 256);

            var rightImage = rightHalf.AddComponent<Image>();
            var rightSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/cookie_right_half.png");
            if (rightSprite != null)
                rightImage.sprite = rightSprite;
            rightHalf.SetActive(false);

            // Crack Particles
            var particles = new GameObject("CrackParticles");
            particles.transform.SetParent(container.transform, false);
            particles.transform.localPosition = Vector3.zero;
            var ps = particles.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.playOnAwake = false;
            main.startLifetime = 0.5f;
            main.startSpeed = 2f;
            main.startSize = 0.1f;
            ps.Stop();
        }

        private static void RebuildMessageContainer(Transform canvas)
        {
            DestroyChild(canvas, "MessageContainer");

            var container = CreateUIElement("MessageContainer", canvas);
            var containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.35f);
            containerRect.anchorMax = new Vector2(0.5f, 0.35f);
            containerRect.anchoredPosition = Vector2.zero;
            containerRect.sizeDelta = new Vector2(900, 300);

            container.AddComponent<MessageController>();
            container.SetActive(false);

            // Rolled Message
            var rolled = CreateUIElement("RolledMessage", container.transform);
            var rolledRect = rolled.GetComponent<RectTransform>();
            rolledRect.anchorMin = new Vector2(0.5f, 0.5f);
            rolledRect.anchorMax = new Vector2(0.5f, 0.5f);
            rolledRect.anchoredPosition = Vector2.zero;
            rolledRect.sizeDelta = new Vector2(128, 64);

            var rolledImage = rolled.AddComponent<Image>();
            var rolledSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/paper_rolled.png");
            if (rolledSprite != null)
                rolledImage.sprite = rolledSprite;

            // Unrolled Message
            var unrolled = CreateUIElement("UnrolledMessage", container.transform);
            var unrolledRect = unrolled.GetComponent<RectTransform>();
            unrolledRect.anchorMin = new Vector2(0.5f, 0.5f);
            unrolledRect.anchorMax = new Vector2(0.5f, 0.5f);
            unrolledRect.anchoredPosition = Vector2.zero;
            unrolledRect.sizeDelta = new Vector2(700, 150);

            var unrolledImage = unrolled.AddComponent<Image>();
            var unrolledSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/paper_unrolled.png");
            if (unrolledSprite != null)
                unrolledImage.sprite = unrolledSprite;
            else
                unrolledImage.color = new Color(0.98f, 0.95f, 0.87f);

            // Message Text
            var messageText = CreateUIElement("MessageText", unrolled.transform);
            var textRect = messageText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(30, 20);
            textRect.offsetMax = new Vector2(-30, -20);

            var tmp = messageText.AddComponent<TextMeshProUGUI>();
            tmp.text = "Your fortune appears here...";
            tmp.fontSize = 28;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.2f, 0.15f, 0.1f);

            messageText.AddComponent<CanvasGroup>();

            // Share Button
            var shareBtn = CreateUIElement("ShareButton", container.transform);
            var shareBtnRect = shareBtn.GetComponent<RectTransform>();
            shareBtnRect.anchorMin = new Vector2(0.5f, 0f);
            shareBtnRect.anchorMax = new Vector2(0.5f, 0f);
            shareBtnRect.anchoredPosition = new Vector2(0, -50);
            shareBtnRect.sizeDelta = new Vector2(200, 60);

            var btnImage = shareBtn.AddComponent<Image>();
            btnImage.color = new Color(0.3f, 0.6f, 0.4f);
            var btn = shareBtn.AddComponent<Button>();
            btn.targetGraphic = btnImage;

            // Share button text
            var shareTxt = CreateUIElement("Text", shareBtn.transform);
            var shareTxtRect = shareTxt.GetComponent<RectTransform>();
            shareTxtRect.anchorMin = Vector2.zero;
            shareTxtRect.anchorMax = Vector2.one;
            shareTxtRect.offsetMin = Vector2.zero;
            shareTxtRect.offsetMax = Vector2.zero;

            var shareTmp = shareTxt.AddComponent<TextMeshProUGUI>();
            shareTmp.text = "Share";
            shareTmp.fontSize = 24;
            shareTmp.alignment = TextAlignmentOptions.Center;
            shareTmp.color = Color.white;
        }

        private static void RebuildWaitingOverlay(Transform canvas)
        {
            DestroyChild(canvas, "WaitingOverlay");

            var overlay = CreateUIElement("WaitingOverlay", canvas);
            var overlayRect = overlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            var overlayImage = overlay.AddComponent<Image>();
            overlayImage.color = new Color(0, 0, 0, 0.7f);

            overlay.AddComponent<WaitingOverlay>();
            overlay.SetActive(false);

            // Waiting Content
            var content = CreateUIElement("WaitingContent", overlay.transform);
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(600, 200);

            // Countdown Text
            var countdown = CreateUIElement("CountdownText", overlay.transform);
            var countdownRect = countdown.GetComponent<RectTransform>();
            countdownRect.anchorMin = new Vector2(0.5f, 0.5f);
            countdownRect.anchorMax = new Vector2(0.5f, 0.5f);
            countdownRect.anchoredPosition = Vector2.zero;
            countdownRect.sizeDelta = new Vector2(600, 100);

            var countdownTmp = countdown.AddComponent<TextMeshProUGUI>();
            countdownTmp.text = "Next cookie in: 00:00:00";
            countdownTmp.fontSize = 42;
            countdownTmp.alignment = TextAlignmentOptions.Center;
            countdownTmp.color = Color.white;
        }

        private static void RebuildHistoryButton(Transform canvas)
        {
            DestroyChild(canvas, "HistoryButton");

            var btn = CreateUIElement("HistoryButton", canvas);
            var btnRect = btn.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1, 1);
            btnRect.anchorMax = new Vector2(1, 1);
            btnRect.anchoredPosition = new Vector2(-60, -80);
            btnRect.sizeDelta = new Vector2(80, 80);

            var image = btn.AddComponent<Image>();
            image.color = new Color(0.85f, 0.75f, 0.6f);

            var button = btn.AddComponent<Button>();
            button.targetGraphic = image;

            // Icon text
            var icon = CreateUIElement("Icon", btn.transform);
            var iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            var iconTmp = icon.AddComponent<TextMeshProUGUI>();
            iconTmp.text = "\u2630"; // hamburger menu icon
            iconTmp.fontSize = 36;
            iconTmp.alignment = TextAlignmentOptions.Center;
            iconTmp.color = new Color(0.3f, 0.2f, 0.1f);
        }

        private static void RebuildHistoryPanel(Transform canvas)
        {
            DestroyChild(canvas, "HistoryPanel");

            var historyPanel = CreateUIElement("HistoryPanel", canvas);
            var hpRect = historyPanel.GetComponent<RectTransform>();
            hpRect.anchorMin = Vector2.zero;
            hpRect.anchorMax = Vector2.one;
            hpRect.offsetMin = Vector2.zero;
            hpRect.offsetMax = Vector2.zero;

            historyPanel.AddComponent<HistoryPanel>();
            historyPanel.SetActive(false);

            // Panel background
            var panel = CreateUIElement("Panel", historyPanel.transform);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.05f, 0.05f);
            panelRect.anchorMax = new Vector2(0.95f, 0.95f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.95f, 0.92f, 0.85f);

            // Title
            var title = CreateUIElement("Title", panel.transform);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.anchoredPosition = new Vector2(0, -40);
            titleRect.sizeDelta = new Vector2(0, 60);

            var titleTmp = title.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "Fortune History";
            titleTmp.fontSize = 36;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = new Color(0.3f, 0.2f, 0.1f);

            // ScrollView
            var scrollView = CreateUIElement("ScrollView", panel.transform);
            var svRect = scrollView.GetComponent<RectTransform>();
            svRect.anchorMin = new Vector2(0, 0);
            svRect.anchorMax = new Vector2(1, 1);
            svRect.offsetMin = new Vector2(20, 20);
            svRect.offsetMax = new Vector2(-20, -80);

            var svImage = scrollView.AddComponent<Image>();
            svImage.color = new Color(0.9f, 0.87f, 0.8f);
            scrollView.AddComponent<Mask>().showMaskGraphic = true;
            var scrollRect = scrollView.AddComponent<ScrollRect>();

            // Content
            var content = CreateUIElement("Content", panel.transform);
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(-40, 0);

            var vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 10;
            vlg.padding = new RectOffset(10, 10, 10, 10);
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var csf = content.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = contentRect;
            scrollRect.viewport = svRect;

            // Close Button
            var closeBtn = CreateUIElement("CloseButton", historyPanel.transform);
            var closeBtnRect = closeBtn.GetComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(1, 1);
            closeBtnRect.anchorMax = new Vector2(1, 1);
            closeBtnRect.anchoredPosition = new Vector2(-40, -40);
            closeBtnRect.sizeDelta = new Vector2(60, 60);

            var closeImage = closeBtn.AddComponent<Image>();
            closeImage.color = new Color(0.8f, 0.3f, 0.3f);

            var closeButton = closeBtn.AddComponent<Button>();
            closeButton.targetGraphic = closeImage;

            var closeTxt = CreateUIElement("X", closeBtn.transform);
            var closeTxtRect = closeTxt.GetComponent<RectTransform>();
            closeTxtRect.anchorMin = Vector2.zero;
            closeTxtRect.anchorMax = Vector2.one;
            closeTxtRect.offsetMin = Vector2.zero;
            closeTxtRect.offsetMax = Vector2.zero;

            var closeTmp = closeTxt.AddComponent<TextMeshProUGUI>();
            closeTmp.text = "X";
            closeTmp.fontSize = 32;
            closeTmp.alignment = TextAlignmentOptions.Center;
            closeTmp.color = Color.white;
        }

        private static void RebuildTutorialHint(Transform canvas)
        {
            DestroyChild(canvas, "TutorialHint");

            var hint = CreateUIElement("TutorialHint", canvas);
            var hintRect = hint.GetComponent<RectTransform>();
            hintRect.anchorMin = new Vector2(0.5f, 0.2f);
            hintRect.anchorMax = new Vector2(0.5f, 0.2f);
            hintRect.anchoredPosition = Vector2.zero;
            hintRect.sizeDelta = new Vector2(500, 50);

            var hintTmp = hint.AddComponent<TextMeshProUGUI>();
            hintTmp.text = "< Swipe to unroll >";
            hintTmp.fontSize = 28;
            hintTmp.alignment = TextAlignmentOptions.Center;
            hintTmp.color = new Color(1, 1, 1, 0.8f);

            hint.SetActive(false);
        }

        private static void RebuildDebugPanel(Transform canvas)
        {
            DestroyChild(canvas, "DebugPanel");

            var debug = CreateUIElement("DebugPanel", canvas);
            var debugRect = debug.GetComponent<RectTransform>();
            debugRect.anchorMin = new Vector2(0, 0);
            debugRect.anchorMax = new Vector2(0, 0);
            debugRect.anchoredPosition = new Vector2(120, 120);
            debugRect.sizeDelta = new Vector2(220, 180);

            var debugImage = debug.AddComponent<Image>();
            debugImage.color = new Color(0, 0, 0, 0.8f);

            debug.AddComponent<LuckyCharm.Debug.DebugPanel>();

            var vlg = debug.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 5;
            vlg.padding = new RectOffset(10, 10, 10, 10);
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            // Debug title
            var title = CreateUIElement("Title", debug.transform);
            var titleTmp = title.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "Debug";
            titleTmp.fontSize = 18;
            titleTmp.alignment = TextAlignmentOptions.Center;
            titleTmp.color = Color.white;
            var titleLE = title.AddComponent<LayoutElement>();
            titleLE.preferredHeight = 25;

            // Buttons will be added by DebugPanel script at runtime
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            debug.SetActive(false);
#endif
        }

        private static GameObject CreateUIElement(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void DestroyChild(Transform parent, string childName)
        {
            var child = parent.Find(childName);
            if (child != null)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }
}
