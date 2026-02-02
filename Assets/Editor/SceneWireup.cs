using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using LuckyCharm.UI;

namespace LuckyCharm.Editor
{
    public static class SceneWireup
    {
        [MenuItem("LuckyCharm/Wire Up Scene References")]
        public static void WireUpSceneReferences()
        {
            // Find all the GameObjects
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                UnityEngine.Debug.LogError("Canvas not found!");
                return;
            }
            
            // Wire up CookieController
            WireUpCookieController(canvas);
            
            // Wire up MessageController
            WireUpMessageController(canvas);
            
            // Wire up WaitingOverlay
            WireUpWaitingOverlay(canvas);
            
            // Wire up HistoryPanel
            WireUpHistoryPanel(canvas);
            
            // Wire up Background
            WireUpBackground(canvas);

            // Assign sprites to UI Image components
            AssignUISprites(canvas);

            // Mark scene dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            
            UnityEngine.Debug.Log("Scene references wired up successfully!");
        }
        
        private static void WireUpCookieController(GameObject canvas)
        {
            var cookieContainer = canvas.transform.Find("CookieContainer");
            if (cookieContainer == null) return;
            
            var controller = cookieContainer.GetComponent<CookieController>();
            if (controller == null) return;
            
            var so = new SerializedObject(controller);
            
            // Cookie Image
            var cookie = cookieContainer.Find("Cookie");
            if (cookie != null)
            {
                var cookieImage = cookie.GetComponent<Image>();
                so.FindProperty("cookieImage").objectReferenceValue = cookieImage;
            }
            
            // Cookie halves
            var leftHalf = cookieContainer.Find("CookieLeftHalf");
            if (leftHalf != null)
            {
                so.FindProperty("cookieLeftHalf").objectReferenceValue = leftHalf.gameObject;
            }
            
            var rightHalf = cookieContainer.Find("CookieRightHalf");
            if (rightHalf != null)
            {
                so.FindProperty("cookieRightHalf").objectReferenceValue = rightHalf.gameObject;
            }
            
            // Crack particles
            var crackParticles = cookieContainer.Find("CrackParticles");
            if (crackParticles != null)
            {
                var ps = crackParticles.GetComponent<ParticleSystem>();
                so.FindProperty("crackParticles").objectReferenceValue = ps;
            }
            
            // Load crack stage sprites
            var crackSpritesProperty = so.FindProperty("crackStageSprites");
            crackSpritesProperty.arraySize = 5;
            
            string[] spriteNames = { "cookie_intact", "cookie_crack_1", "cookie_crack_2", "cookie_crack_3", "cookie_crack_4" };
            for (int i = 0; i < spriteNames.Length; i++)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{spriteNames[i]}.png");
                crackSpritesProperty.GetArrayElementAtIndex(i).objectReferenceValue = sprite;
            }
            
            so.ApplyModifiedProperties();
            UnityEngine.Debug.Log("CookieController wired up.");
        }
        
        private static void WireUpMessageController(GameObject canvas)
        {
            var messageContainer = canvas.transform.Find("MessageContainer");
            if (messageContainer == null) return;
            
            var controller = messageContainer.GetComponent<MessageController>();
            if (controller == null) return;
            
            var so = new SerializedObject(controller);
            
            // Rolled message
            var rolledMessage = messageContainer.Find("RolledMessage");
            if (rolledMessage != null)
            {
                so.FindProperty("rolledMessage").objectReferenceValue = rolledMessage.GetComponent<RectTransform>();
            }
            
            // Unrolled message
            var unrolledMessage = messageContainer.Find("UnrolledMessage");
            if (unrolledMessage != null)
            {
                so.FindProperty("unrolledMessage").objectReferenceValue = unrolledMessage.GetComponent<RectTransform>();
                
                // Message text (child of unrolled message)
                var messageText = unrolledMessage.Find("MessageText");
                if (messageText != null)
                {
                    so.FindProperty("messageText").objectReferenceValue = messageText.GetComponent<TextMeshProUGUI>();
                    so.FindProperty("messageTextCanvasGroup").objectReferenceValue = messageText.GetComponent<CanvasGroup>();
                }
            }
            
            // Share button
            var shareButton = messageContainer.Find("ShareButton");
            if (shareButton != null)
            {
                so.FindProperty("shareButton").objectReferenceValue = shareButton.GetComponent<Button>();
            }
            
            // Tutorial hint
            var tutorialHint = canvas.transform.Find("TutorialHint");
            if (tutorialHint != null)
            {
                so.FindProperty("tutorialHint").objectReferenceValue = tutorialHint.gameObject;
            }
            
            so.ApplyModifiedProperties();
            UnityEngine.Debug.Log("MessageController wired up.");
        }
        
        private static void WireUpWaitingOverlay(GameObject canvas)
        {
            var waitingOverlay = canvas.transform.Find("WaitingOverlay");
            if (waitingOverlay == null) return;
            
            var controller = waitingOverlay.GetComponent<WaitingOverlay>();
            if (controller == null) return;
            
            var so = new SerializedObject(controller);
            
            // Countdown text
            var countdownText = waitingOverlay.Find("CountdownText");
            if (countdownText != null)
            {
                so.FindProperty("countdownText").objectReferenceValue = countdownText.GetComponent<TextMeshProUGUI>();
            }
            
            // Waiting content
            var waitingContent = waitingOverlay.Find("WaitingContent");
            if (waitingContent != null)
            {
                so.FindProperty("waitingContent").objectReferenceValue = waitingContent.gameObject;
            }
            
            so.ApplyModifiedProperties();
            UnityEngine.Debug.Log("WaitingOverlay wired up.");
        }
        
        private static void WireUpHistoryPanel(GameObject canvas)
        {
            var historyPanelObj = canvas.transform.Find("HistoryPanel");
            if (historyPanelObj == null) return;
            
            var controller = historyPanelObj.GetComponent<HistoryPanel>();
            if (controller == null) return;
            
            var so = new SerializedObject(controller);
            
            // Panel
            var panel = historyPanelObj.Find("Panel");
            if (panel != null)
            {
                so.FindProperty("panel").objectReferenceValue = panel.gameObject;
                
                // Content parent (inside panel)
                var content = panel.Find("Content");
                if (content != null)
                {
                    so.FindProperty("contentParent").objectReferenceValue = content;
                }
            }
            
            // Open button (HistoryButton)
            var openButton = canvas.transform.Find("HistoryButton");
            if (openButton != null)
            {
                so.FindProperty("openButton").objectReferenceValue = openButton.GetComponent<Button>();
            }
            
            // Close button
            var closeButton = historyPanelObj.Find("CloseButton");
            if (closeButton != null)
            {
                so.FindProperty("closeButton").objectReferenceValue = closeButton.GetComponent<Button>();
            }
            
            // History entry prefab
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/HistoryEntry.prefab");
            if (prefab != null)
            {
                so.FindProperty("historyEntryPrefab").objectReferenceValue = prefab;
            }
            
            so.ApplyModifiedProperties();
            UnityEngine.Debug.Log("HistoryPanel wired up.");
        }
        
        private static void WireUpBackground(GameObject canvas)
        {
            var background = canvas.transform.Find("Background");
            if (background == null) return;

            var image = background.GetComponent<Image>();
            if (image == null) return;

            // Add or get BackgroundController component
            var controller = background.GetComponent<BackgroundController>();
            if (controller == null)
            {
                controller = background.gameObject.AddComponent<BackgroundController>();
            }

            var so = new SerializedObject(controller);

            // Load and set background sprites for timer states
            var timerActiveSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Backgrounds/kepokepo_on.PNG");
            var cookieReadySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Backgrounds/kepokepo_off.PNG");

            so.FindProperty("timerActiveSprite").objectReferenceValue = timerActiveSprite;
            so.FindProperty("cookieReadySprite").objectReferenceValue = cookieReadySprite;
            so.FindProperty("backgroundImage").objectReferenceValue = image;

            so.ApplyModifiedProperties();

            // Set initial sprite (kepokepo_off as default)
            if (cookieReadySprite != null)
            {
                image.sprite = cookieReadySprite;
                EditorUtility.SetDirty(image);
            }

            UnityEngine.Debug.Log("Background wired up with BackgroundController.");
        }

        private static void AssignUISprites(GameObject canvas)
        {
            // Cookie Container sprites
            var cookieContainer = canvas.transform.Find("CookieContainer");
            if (cookieContainer != null)
            {
                AssignSprite(cookieContainer.Find("Cookie"), "cookie_intact");
                AssignSprite(cookieContainer.Find("CookieLeftHalf"), "cookie_left_half");
                AssignSprite(cookieContainer.Find("CookieRightHalf"), "cookie_right_half");
            }

            // Message Container sprites
            var messageContainer = canvas.transform.Find("MessageContainer");
            if (messageContainer != null)
            {
                AssignSprite(messageContainer.Find("RolledMessage"), "paper_rolled");
                AssignSprite(messageContainer.Find("UnrolledMessage"), "paper_unrolled");
            }

            UnityEngine.Debug.Log("UI sprites assigned.");
        }

        private static void AssignSprite(Transform target, string spriteName)
        {
            if (target == null) return;

            var image = target.GetComponent<Image>();
            if (image == null) return;

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{spriteName}.png");
            if (sprite != null)
            {
                image.sprite = sprite;
                EditorUtility.SetDirty(image);
            }
        }
    }
}
