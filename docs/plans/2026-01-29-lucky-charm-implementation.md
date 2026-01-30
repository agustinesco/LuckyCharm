# LuckyCharm Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build a 2D mobile fortune cookie app with daily notifications, tap-to-crack interaction, horizontal swipe to unroll message, history tracking, and social sharing.

**Architecture:** Single-scene Unity 2D app with UI-based state machine. Managers handle notifications, persistence, audio, and haptics. No backend required - all data stored locally.

**Tech Stack:** Unity 2022.3 LTS, C#, Mobile Notifications package, DOTween (animations), NativeShare (sharing), TextMeshPro (text rendering)

---

## Phase 1: Project Setup & Core Infrastructure

### Task 1: Add Required Unity Packages

**Files:**
- Modify: `Packages/manifest.json`

**Step 1: Add mobile notifications package**

Add these lines to the dependencies in `Packages/manifest.json`:
```json
"com.unity.mobile.notifications": "2.3.2"
```

**Step 2: Verify Unity recompiles without errors**

Check Unity console for errors after package installation.

---

### Task 2: Create Folder Structure

**Files:**
- Create folders in Assets/

**Step 1: Create the directory structure**

Create these folders:
```
Assets/Scripts/Core/
Assets/Scripts/UI/
Assets/Scripts/Data/
Assets/Scripts/Utils/
Assets/Prefabs/
Assets/Sprites/
Assets/Audio/
Assets/Resources/
Assets/Fonts/
```

**Step 2: Verify folders appear in Unity Project window**

---

### Task 3: Create Data Models

**Files:**
- Create: `Assets/Scripts/Data/FortuneMessage.cs`
- Create: `Assets/Scripts/Data/SaveData.cs`
- Create: `Assets/Scripts/Data/HistoryEntry.cs`

**Step 1: Create FortuneMessage.cs**

```csharp
using System;

namespace LuckyCharm.Data
{
    [Serializable]
    public class FortuneMessage
    {
        public string id;
        public string text;
        public string category;
    }
}
```

**Step 2: Create HistoryEntry.cs**

```csharp
using System;

namespace LuckyCharm.Data
{
    [Serializable]
    public class HistoryEntry
    {
        public string date;
        public string messageId;
        public string messageText;
    }
}
```

**Step 3: Create SaveData.cs**

```csharp
using System;
using System.Collections.Generic;

namespace LuckyCharm.Data
{
    [Serializable]
    public class SaveData
    {
        public string lastCookieDate;
        public string lastMessageId;
        public List<HistoryEntry> history = new List<HistoryEntry>();
        public List<string> seenMessageIds = new List<string>();
        public bool hasSeenTutorial;
        public bool soundEnabled = true;
        public bool hapticsEnabled = true;
    }
}
```

**Step 4: Verify no compilation errors in Unity console**

---

### Task 4: Create Message Database

**Files:**
- Create: `Assets/Scripts/Data/MessageDatabase.cs`
- Create: `Assets/Resources/messages.json`

**Step 1: Create MessageDatabase.cs**

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LuckyCharm.Data
{
    [Serializable]
    public class MessageList
    {
        public List<FortuneMessage> messages;
    }

    public static class MessageDatabase
    {
        private static List<FortuneMessage> _messages;

        public static void Load()
        {
            var jsonFile = Resources.Load<TextAsset>("messages");
            if (jsonFile != null)
            {
                var data = JsonUtility.FromJson<MessageList>(jsonFile.text);
                _messages = data.messages;
            }
            else
            {
                Debug.LogError("messages.json not found in Resources!");
                _messages = new List<FortuneMessage>();
            }
        }

        public static FortuneMessage GetRandomMessage(List<string> excludeIds = null)
        {
            if (_messages == null || _messages.Count == 0)
            {
                Load();
            }

            var available = _messages;
            if (excludeIds != null && excludeIds.Count > 0 && excludeIds.Count < _messages.Count)
            {
                available = _messages.Where(m => !excludeIds.Contains(m.id)).ToList();
            }

            if (available.Count == 0)
            {
                available = _messages;
            }

            int index = UnityEngine.Random.Range(0, available.Count);
            return available[index];
        }

        public static int TotalCount => _messages?.Count ?? 0;
    }
}
```

**Step 2: Create messages.json with 50 starter messages**

Create `Assets/Resources/messages.json`:
```json
{
    "messages": [
        {"id": "1", "text": "The best time to plant a tree was 20 years ago. The second best time is now.", "category": "motivation"},
        {"id": "2", "text": "Your kindness is a gift that keeps giving.", "category": "kindness"},
        {"id": "3", "text": "Courage is not the absence of fear, but action in spite of it.", "category": "courage"},
        {"id": "4", "text": "Today's struggle is tomorrow's strength.", "category": "growth"},
        {"id": "5", "text": "A smile is a curve that sets everything straight.", "category": "humor"},
        {"id": "6", "text": "The journey of a thousand miles begins with a single step.", "category": "motivation"},
        {"id": "7", "text": "Be the reason someone believes in the goodness of people.", "category": "kindness"},
        {"id": "8", "text": "What you seek is seeking you.", "category": "wisdom"},
        {"id": "9", "text": "Stars can't shine without darkness.", "category": "growth"},
        {"id": "10", "text": "Your potential is endless. Go do what you were created to do.", "category": "motivation"},
        {"id": "11", "text": "The only way to do great work is to love what you do.", "category": "wisdom"},
        {"id": "12", "text": "Small acts of kindness create ripples of change.", "category": "kindness"},
        {"id": "13", "text": "You are braver than you believe, stronger than you seem.", "category": "courage"},
        {"id": "14", "text": "Every expert was once a beginner.", "category": "growth"},
        {"id": "15", "text": "Life is short. Smile while you still have teeth.", "category": "humor"},
        {"id": "16", "text": "Believe you can and you're halfway there.", "category": "motivation"},
        {"id": "17", "text": "In a world where you can be anything, be kind.", "category": "kindness"},
        {"id": "18", "text": "Fortune favors the bold.", "category": "courage"},
        {"id": "19", "text": "Growth is painful. Change is painful. But nothing is as painful as staying stuck.", "category": "growth"},
        {"id": "20", "text": "Life is too important to be taken seriously.", "category": "humor"},
        {"id": "21", "text": "Your only limit is your mind.", "category": "motivation"},
        {"id": "22", "text": "Happiness is found when you stop comparing yourself to others.", "category": "wisdom"},
        {"id": "23", "text": "The universe is conspiring in your favor.", "category": "motivation"},
        {"id": "24", "text": "Be fearless in the pursuit of what sets your soul on fire.", "category": "courage"},
        {"id": "25", "text": "You are allowed to be both a masterpiece and a work in progress.", "category": "growth"},
        {"id": "26", "text": "The sun will rise and we will try again.", "category": "motivation"},
        {"id": "27", "text": "Your energy introduces you before you even speak.", "category": "wisdom"},
        {"id": "28", "text": "Do something today that your future self will thank you for.", "category": "motivation"},
        {"id": "29", "text": "It's okay to not be okay, as long as you don't stay that way.", "category": "growth"},
        {"id": "30", "text": "Throw kindness around like confetti.", "category": "kindness"},
        {"id": "31", "text": "The best view comes after the hardest climb.", "category": "courage"},
        {"id": "32", "text": "You don't have to be perfect to be amazing.", "category": "wisdom"},
        {"id": "33", "text": "Today is a good day to have a good day.", "category": "motivation"},
        {"id": "34", "text": "Your vibe attracts your tribe.", "category": "wisdom"},
        {"id": "35", "text": "Be a voice, not an echo.", "category": "courage"},
        {"id": "36", "text": "Difficult roads often lead to beautiful destinations.", "category": "growth"},
        {"id": "37", "text": "You are capable of more than you know.", "category": "motivation"},
        {"id": "38", "text": "Life begins at the end of your comfort zone.", "category": "courage"},
        {"id": "39", "text": "The comeback is always stronger than the setback.", "category": "growth"},
        {"id": "40", "text": "Make today so awesome that yesterday gets jealous.", "category": "humor"},
        {"id": "41", "text": "You are never too old to set another goal or dream a new dream.", "category": "motivation"},
        {"id": "42", "text": "Kindness is free. Sprinkle it everywhere.", "category": "kindness"},
        {"id": "43", "text": "What lies behind us and what lies before us are tiny matters compared to what lies within us.", "category": "wisdom"},
        {"id": "44", "text": "Take the risk or lose the chance.", "category": "courage"},
        {"id": "45", "text": "Bloom where you are planted.", "category": "growth"},
        {"id": "46", "text": "Good things come to those who hustle.", "category": "motivation"},
        {"id": "47", "text": "Let your faith be bigger than your fear.", "category": "courage"},
        {"id": "48", "text": "The only impossible journey is the one you never begin.", "category": "motivation"},
        {"id": "49", "text": "You are the author of your own story.", "category": "wisdom"},
        {"id": "50", "text": "Dream big. Work hard. Stay humble.", "category": "motivation"}
    ]
}
```

**Step 3: Verify no compilation errors**

---

### Task 5: Create SaveManager

**Files:**
- Create: `Assets/Scripts/Core/SaveManager.cs`

**Step 1: Create SaveManager.cs**

```csharp
using System;
using System.IO;
using UnityEngine;
using LuckyCharm.Data;

namespace LuckyCharm.Core
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        public SaveData Data { get; private set; }

        private string SavePath => Path.Combine(Application.persistentDataPath, "savedata.json");

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        public void Load()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    string json = File.ReadAllText(SavePath);
                    Data = JsonUtility.FromJson<SaveData>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load save data: {e.Message}");
                    Data = new SaveData();
                }
            }
            else
            {
                Data = new SaveData();
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(Data, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save data: {e.Message}");
            }
        }

        public bool HasOpenedCookieToday()
        {
            return Data.lastCookieDate == DateTime.Now.ToString("yyyy-MM-dd");
        }

        public bool IsCookieAvailable()
        {
            if (HasOpenedCookieToday()) return false;
            return DateTime.Now.Hour >= 10;
        }

        public void RecordCookieOpened(FortuneMessage message)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            Data.lastCookieDate = today;
            Data.lastMessageId = message.id;

            Data.history.Insert(0, new HistoryEntry
            {
                date = today,
                messageId = message.id,
                messageText = message.text
            });

            if (!Data.seenMessageIds.Contains(message.id))
            {
                Data.seenMessageIds.Add(message.id);
            }

            Save();
        }

        public void MarkTutorialSeen()
        {
            Data.hasSeenTutorial = true;
            Save();
        }

        public void SetSoundEnabled(bool enabled)
        {
            Data.soundEnabled = enabled;
            Save();
        }

        public void SetHapticsEnabled(bool enabled)
        {
            Data.hapticsEnabled = enabled;
            Save();
        }
    }
}
```

**Step 2: Verify no compilation errors**

---

### Task 6: Create GameState Enum and GameManager

**Files:**
- Create: `Assets/Scripts/Core/GameState.cs`
- Create: `Assets/Scripts/Core/GameManager.cs`

**Step 1: Create GameState.cs**

```csharp
namespace LuckyCharm.Core
{
    public enum GameState
    {
        Waiting,
        CookieAvailable,
        Cracking,
        MessageRolled,
        MessageRevealed
    }
}
```

**Step 2: Create GameManager.cs**

```csharp
using System;
using UnityEngine;
using LuckyCharm.Data;

namespace LuckyCharm.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action<GameState> OnStateChanged;
        public event Action<FortuneMessage> OnMessageRevealed;

        public GameState CurrentState { get; private set; } = GameState.Waiting;
        public FortuneMessage CurrentMessage { get; private set; }

        private int _tapCount = 0;
        private const int TapsRequired = 5;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            MessageDatabase.Load();
            CheckCookieAvailability();
        }

        public void CheckCookieAvailability()
        {
            if (SaveManager.Instance.IsCookieAvailable())
            {
                CurrentMessage = MessageDatabase.GetRandomMessage(SaveManager.Instance.Data.seenMessageIds);
                SetState(GameState.CookieAvailable);
            }
            else
            {
                SetState(GameState.Waiting);
            }
        }

        public void TapCookie()
        {
            if (CurrentState != GameState.CookieAvailable && CurrentState != GameState.Cracking)
                return;

            _tapCount++;

            if (_tapCount >= TapsRequired)
            {
                SetState(GameState.Cracking);
            }
        }

        public void OnCrackAnimationComplete()
        {
            _tapCount = 0;
            SetState(GameState.MessageRolled);
        }

        public void OnMessageFullyUnrolled()
        {
            SetState(GameState.MessageRevealed);
            SaveManager.Instance.RecordCookieOpened(CurrentMessage);
            OnMessageRevealed?.Invoke(CurrentMessage);
        }

        public void ResetForNextDay()
        {
            CurrentMessage = null;
            _tapCount = 0;
            CheckCookieAvailability();
        }

        public int GetTapCount() => _tapCount;
        public int GetTapsRequired() => TapsRequired;

        public TimeSpan GetTimeUntilNextCookie()
        {
            DateTime now = DateTime.Now;
            DateTime nextCookie;

            if (now.Hour >= 10)
            {
                nextCookie = now.Date.AddDays(1).AddHours(10);
            }
            else
            {
                nextCookie = now.Date.AddHours(10);
            }

            return nextCookie - now;
        }

        private void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
```

**Step 3: Verify no compilation errors**

---

### Task 7: Create NotificationManager

**Files:**
- Create: `Assets/Scripts/Core/NotificationManager.cs`

**Step 1: Create NotificationManager.cs**

```csharp
using System;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace LuckyCharm.Core
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        private const string ChannelId = "lucky_charm_channel";
        private const string NotificationTitle = "Your Lucky Cookie is Ready!";
        private const string NotificationText = "Come crack open today's fortune and discover your message.";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeNotifications();
        }

        private void InitializeNotifications()
        {
#if UNITY_ANDROID
            var channel = new AndroidNotificationChannel()
            {
                Id = ChannelId,
                Name = "Lucky Charm",
                Importance = Importance.High,
                Description = "Daily fortune cookie notifications"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);
#elif UNITY_IOS
            StartCoroutine(RequestIOSAuthorization());
#endif
        }

#if UNITY_IOS
        private System.Collections.IEnumerator RequestIOSAuthorization()
        {
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound;
            using (var request = new AuthorizationRequest(authorizationOption, true))
            {
                while (!request.IsFinished)
                {
                    yield return null;
                }
            }
        }
#endif

        public void ScheduleDailyNotification()
        {
            CancelAllNotifications();

            DateTime now = DateTime.Now;
            DateTime scheduledTime;

            if (now.Hour >= 10)
            {
                scheduledTime = now.Date.AddDays(1).AddHours(10);
            }
            else
            {
                scheduledTime = now.Date.AddHours(10);
            }

            TimeSpan delay = scheduledTime - now;

#if UNITY_ANDROID
            var notification = new AndroidNotification
            {
                Title = NotificationTitle,
                Text = NotificationText,
                FireTime = scheduledTime,
                SmallIcon = "icon_small",
                LargeIcon = "icon_large"
            };
            AndroidNotificationCenter.SendNotification(notification, ChannelId);
#elif UNITY_IOS
            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = delay,
                Repeats = false
            };

            var notification = new iOSNotification()
            {
                Identifier = "daily_cookie",
                Title = NotificationTitle,
                Body = NotificationText,
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                Trigger = timeTrigger
            };
            iOSNotificationCenter.ScheduleNotification(notification);
#endif

            Debug.Log($"Notification scheduled for {scheduledTime}");
        }

        public void CancelAllNotifications()
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
            iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (!SaveManager.Instance.HasOpenedCookieToday())
                {
                    ScheduleDailyNotification();
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (!SaveManager.Instance.HasOpenedCookieToday())
            {
                ScheduleDailyNotification();
            }
        }
    }
}
```

**Step 2: Verify no compilation errors**

---

### Task 8: Create AudioManager

**Files:**
- Create: `Assets/Scripts/Utils/AudioManager.cs`

**Step 1: Create AudioManager.cs**

```csharp
using UnityEngine;

namespace LuckyCharm.Utils
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] crackLightClips;
        [SerializeField] private AudioClip crackBreakClip;
        [SerializeField] private AudioClip paperWhooshClip;
        [SerializeField] private AudioClip paperRustleClip;
        [SerializeField] private AudioClip paperSettleClip;
        [SerializeField] private AudioClip uiTapClip;

        private AudioSource _sfxSource;
        private AudioSource _loopSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;

            _loopSource = gameObject.AddComponent<AudioSource>();
            _loopSource.playOnAwake = false;
            _loopSource.loop = true;
        }

        private bool IsSoundEnabled()
        {
            return Core.SaveManager.Instance?.Data?.soundEnabled ?? true;
        }

        public void PlayCrackLight(int tapIndex)
        {
            if (!IsSoundEnabled()) return;
            if (crackLightClips != null && crackLightClips.Length > 0)
            {
                int index = Mathf.Clamp(tapIndex, 0, crackLightClips.Length - 1);
                _sfxSource.PlayOneShot(crackLightClips[index]);
            }
        }

        public void PlayCrackBreak()
        {
            if (!IsSoundEnabled()) return;
            if (crackBreakClip != null)
            {
                _sfxSource.PlayOneShot(crackBreakClip);
            }
        }

        public void PlayPaperWhoosh()
        {
            if (!IsSoundEnabled()) return;
            if (paperWhooshClip != null)
            {
                _sfxSource.PlayOneShot(paperWhooshClip);
            }
        }

        public void StartPaperRustle()
        {
            if (!IsSoundEnabled()) return;
            if (paperRustleClip != null && !_loopSource.isPlaying)
            {
                _loopSource.clip = paperRustleClip;
                _loopSource.Play();
            }
        }

        public void StopPaperRustle()
        {
            _loopSource.Stop();
        }

        public void PlayPaperSettle()
        {
            if (!IsSoundEnabled()) return;
            if (paperSettleClip != null)
            {
                _sfxSource.PlayOneShot(paperSettleClip);
            }
        }

        public void PlayUITap()
        {
            if (!IsSoundEnabled()) return;
            if (uiTapClip != null)
            {
                _sfxSource.PlayOneShot(uiTapClip);
            }
        }
    }
}
```

**Step 2: Verify no compilation errors**

---

### Task 9: Create HapticsManager

**Files:**
- Create: `Assets/Scripts/Utils/HapticsManager.cs`

**Step 1: Create HapticsManager.cs**

```csharp
using UnityEngine;

namespace LuckyCharm.Utils
{
    public class HapticsManager : MonoBehaviour
    {
        public static HapticsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private bool IsHapticsEnabled()
        {
            return Core.SaveManager.Instance?.Data?.hapticsEnabled ?? true;
        }

        public void VibrateLight()
        {
            if (!IsHapticsEnabled()) return;
#if UNITY_IOS && !UNITY_EDITOR
            // iOS uses taptic engine - light impact
            iOSHapticFeedback.Trigger(iOSHapticFeedback.Type.ImpactLight);
#elif UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }

        public void VibrateMedium()
        {
            if (!IsHapticsEnabled()) return;
#if UNITY_IOS && !UNITY_EDITOR
            iOSHapticFeedback.Trigger(iOSHapticFeedback.Type.ImpactMedium);
#elif UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }

        public void VibrateHeavy()
        {
            if (!IsHapticsEnabled()) return;
#if UNITY_IOS && !UNITY_EDITOR
            iOSHapticFeedback.Trigger(iOSHapticFeedback.Type.ImpactHeavy);
#elif UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }

        public void VibrateSuccess()
        {
            if (!IsHapticsEnabled()) return;
#if UNITY_IOS && !UNITY_EDITOR
            iOSHapticFeedback.Trigger(iOSHapticFeedback.Type.Success);
#elif UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }
    }

#if UNITY_IOS
    public static class iOSHapticFeedback
    {
        public enum Type
        {
            ImpactLight,
            ImpactMedium,
            ImpactHeavy,
            Success,
            Warning,
            Error
        }

        public static void Trigger(Type type)
        {
            // This would use native iOS calls - for now using Unity's basic vibration
            // In production, use a native plugin for fine-grained haptic control
            Handheld.Vibrate();
        }
    }
#endif
}
```

**Step 2: Verify no compilation errors**

---

## Phase 2: UI Setup

### Task 10: Create Main Scene Structure

**Files:**
- Modify: `Assets/Scenes/SampleScene.unity` (rename to MainScene)

**Step 1: Set up the scene hierarchy in Unity**

Create this hierarchy in the scene:
```
- Main Camera (configure for 2D)
- Managers (empty GameObject)
  - GameManager
  - SaveManager
  - NotificationManager
  - AudioManager
  - HapticsManager
- Canvas (Screen Space - Overlay)
  - Background
  - CookieContainer
    - Cookie
    - CrackParticles
  - MessageContainer
    - RolledMessage
    - UnrolledMessage
      - MessageText (TextMeshPro)
    - ShareButton
  - WaitingOverlay
    - CountdownText (TextMeshPro)
  - HistoryButton
  - HistoryPanel
    - ScrollView
      - Content
    - CloseButton
  - TutorialHint
```

**Step 2: Configure Canvas Scaler**

- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1080 x 1920
- Match: 0.5

**Step 3: Attach manager scripts to Managers children**

---

### Task 11: Create CookieController

**Files:**
- Create: `Assets/Scripts/UI/CookieController.cs`

**Step 1: Create CookieController.cs**

```csharp
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
```

**Step 2: Verify no compilation errors**

---

### Task 12: Create MessageController

**Files:**
- Create: `Assets/Scripts/UI/MessageController.cs`

**Step 1: Create MessageController.cs**

```csharp
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
                    messageText.text = message.text;
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
            // ShareManager will be implemented in Phase 3
            Debug.Log("Share clicked - to be implemented");
        }
    }
}
```

**Step 2: Verify no compilation errors**

---

### Task 13: Create WaitingOverlay

**Files:**
- Create: `Assets/Scripts/UI/WaitingOverlay.cs`

**Step 1: Create WaitingOverlay.cs**

```csharp
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
                if (timeUntil.TotalHours >= 1)
                {
                    countdownText.text = $"Next cookie in\n{(int)timeUntil.TotalHours}h {timeUntil.Minutes}m";
                }
                else if (timeUntil.TotalMinutes >= 1)
                {
                    countdownText.text = $"Next cookie in\n{timeUntil.Minutes}m {timeUntil.Seconds}s";
                }
                else
                {
                    countdownText.text = $"Next cookie in\n{timeUntil.Seconds}s";
                }
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
```

**Step 2: Verify no compilation errors**

---

### Task 14: Create HistoryPanel

**Files:**
- Create: `Assets/Scripts/UI/HistoryPanel.cs`
- Create: `Assets/Scripts/UI/HistoryEntryUI.cs`

**Step 1: Create HistoryEntryUI.cs**

```csharp
using UnityEngine;
using TMPro;
using LuckyCharm.Data;

namespace LuckyCharm.UI
{
    public class HistoryEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI messageText;

        public void Setup(HistoryEntry entry)
        {
            if (dateText != null)
            {
                dateText.text = entry.date;
            }

            if (messageText != null)
            {
                messageText.text = entry.messageText;
            }
        }
    }
}
```

**Step 2: Create HistoryPanel.cs**

```csharp
using UnityEngine;
using UnityEngine.UI;
using LuckyCharm.Core;
using LuckyCharm.Utils;

namespace LuckyCharm.UI
{
    public class HistoryPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject historyEntryPrefab;

        private void Start()
        {
            if (openButton != null)
            {
                openButton.onClick.AddListener(OpenPanel);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePanel);
            }

            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        public void OpenPanel()
        {
            AudioManager.Instance?.PlayUITap();
            PopulateHistory();

            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        public void ClosePanel()
        {
            AudioManager.Instance?.PlayUITap();

            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        private void PopulateHistory()
        {
            // Clear existing entries
            if (contentParent != null)
            {
                foreach (Transform child in contentParent)
                {
                    Destroy(child.gameObject);
                }
            }

            // Add entries from save data
            var history = SaveManager.Instance.Data.history;
            if (history == null || historyEntryPrefab == null || contentParent == null)
                return;

            foreach (var entry in history)
            {
                var entryGO = Instantiate(historyEntryPrefab, contentParent);
                var entryUI = entryGO.GetComponent<HistoryEntryUI>();
                if (entryUI != null)
                {
                    entryUI.Setup(entry);
                }
            }
        }
    }
}
```

**Step 3: Verify no compilation errors**

---

## Phase 3: Sharing System

### Task 15: Create ShareManager

**Files:**
- Create: `Assets/Scripts/Core/ShareManager.cs`

**Step 1: Create ShareManager.cs**

```csharp
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

        private const int ShareImageWidth = 1080;
        private const int ShareImageHeight = 1920;

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
                Debug.LogError("No message to share");
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

            yield return new WaitForEndOfFrame();

            // Capture the image
            string imagePath = CaptureShareImage();

            // Hide share canvas
            if (shareCanvas != null)
            {
                shareCanvas.gameObject.SetActive(false);
            }

            if (string.IsNullOrEmpty(imagePath))
            {
                Debug.LogError("Failed to capture share image");
                yield break;
            }

            // Share using native share
            ShareImage(imagePath, message.text);
        }

        private string CaptureShareImage()
        {
            RenderTexture rt = new RenderTexture(ShareImageWidth, ShareImageHeight, 24);

            if (captureCamera != null)
            {
                captureCamera.targetTexture = rt;
                captureCamera.Render();
            }

            RenderTexture.active = rt;
            Texture2D screenshot = new Texture2D(ShareImageWidth, ShareImageHeight, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, ShareImageWidth, ShareImageHeight), 0, 0);
            screenshot.Apply();

            if (captureCamera != null)
            {
                captureCamera.targetTexture = null;
            }
            RenderTexture.active = null;
            Destroy(rt);

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
            Debug.Log($"Share image: {imagePath}");
            Debug.Log($"Message: {messageText}");
#else
            // Use NativeShare plugin
            // Note: NativeShare must be imported separately
            // new NativeShare()
            //     .AddFile(imagePath)
            //     .SetSubject("My Lucky Cookie Fortune")
            //     .SetText($"My fortune today: \"{messageText}\" - Shared from LuckyCharm")
            //     .Share();

            // Fallback for testing without NativeShare
            Debug.Log($"Would share: {imagePath}");
#endif
        }
    }
}
```

**Step 2: Update MessageController to use ShareManager**

In `MessageController.cs`, update the `OnShareClicked` method:
```csharp
private void OnShareClicked()
{
    AudioManager.Instance?.PlayUITap();
    ShareManager.Instance?.ShareCurrentMessage();
}
```

**Step 3: Verify no compilation errors**

---

### Task 16: Import NativeShare Package

**Files:**
- Modify: Unity Package Manager

**Step 1: Import NativeShare**

Option A - Via OpenUPM (recommended):
```bash
# In terminal, navigate to project root
cd /Users/agustinesco/mobileProtos/LuckyCharm
# Add OpenUPM registry and package
# Or manually add to manifest.json:
```

Add to `Packages/manifest.json` in scopedRegistries:
```json
{
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.yasirkula.nativeshare"
      ]
    }
  ],
  "dependencies": {
    "com.yasirkula.nativeshare": "1.5.1",
    ...
  }
}
```

Option B - Manual import from GitHub releases.

**Step 2: Update ShareManager to use NativeShare**

Uncomment the NativeShare code in `ShareManager.cs`:
```csharp
private void ShareImage(string imagePath, string messageText)
{
    new NativeShare()
        .AddFile(imagePath)
        .SetSubject("My Lucky Cookie Fortune")
        .SetText($"\"{messageText}\"\n\nShared from LuckyCharm")
        .Share();
}
```

**Step 3: Verify no compilation errors**

---

## Phase 4: Visual Polish & Assets

### Task 17: Create Placeholder Sprites

**Files:**
- Create: `Assets/Sprites/` placeholder images

**Step 1: Create placeholder sprites in Unity**

For initial testing, create simple colored sprites:
- `cookie_intact.png` - 256x256 brown circle
- `cookie_crack_1.png` - Cookie with 1 crack line
- `cookie_crack_2.png` - Cookie with 2 crack lines
- `cookie_crack_3.png` - Cookie with 3 crack lines
- `cookie_crack_4.png` - Cookie with many cracks
- `cookie_left_half.png` - Left half of cookie
- `cookie_right_half.png` - Right half of cookie
- `paper_rolled.png` - Small rolled paper cylinder
- `paper_unrolled.png` - Horizontal paper rectangle
- `background.png` - Warm wood texture or gradient

**Step 2: Import sprites and set to Sprite (2D and UI)**

---

### Task 18: Create Prefabs

**Files:**
- Create: `Assets/Prefabs/HistoryEntry.prefab`

**Step 1: Create HistoryEntry prefab**

Create a UI prefab with:
- Parent (LayoutElement, preferred height 120)
  - DateText (TextMeshPro, left aligned, small font)
  - MessageText (TextMeshPro, left aligned, wrapping enabled)
  - Divider line (Image, 2px height)

Attach `HistoryEntryUI` component and wire up references.

**Step 2: Save as prefab to Assets/Prefabs/**

---

### Task 19: Wire Up Scene References

**Files:**
- Modify: `Assets/Scenes/MainScene.unity`

**Step 1: Attach scripts to GameObjects**

- Cookie GameObject: Add `CookieController`, wire sprite references
- MessageContainer: Add `MessageController`, wire UI references
- WaitingOverlay: Add `WaitingOverlay`, wire text reference
- HistoryPanel parent: Add `HistoryPanel`, wire button and content references
- Managers/ShareManager: Add `ShareManager` (if using share canvas approach)

**Step 2: Configure AudioManager with placeholder/null clips**

**Step 3: Test play mode - verify state machine transitions**

---

## Phase 5: Platform Configuration

### Task 20: Configure iOS Build Settings

**Files:**
- Modify: `ProjectSettings/ProjectSettings.asset`

**Step 1: In Unity Editor > Build Settings > iOS**

- Set Bundle Identifier: `com.yourcompany.luckycharm`
- Set minimum iOS version: 13.0
- Set Camera Usage Description (if needed): empty or remove
- Set Notification Usage Description: "LuckyCharm sends a daily reminder when your fortune cookie is ready."

**Step 2: In Player Settings > iOS**

- Default Orientation: Portrait
- Allowed Orientations: Portrait only
- Status Bar: Hidden

---

### Task 21: Configure Android Build Settings

**Files:**
- Modify: `ProjectSettings/ProjectSettings.asset`

**Step 1: In Unity Editor > Build Settings > Android**

- Set Package Name: `com.yourcompany.luckycharm`
- Set Minimum API Level: 24 (Android 7.0)
- Set Target API Level: 34 (latest)

**Step 2: In Player Settings > Android**

- Default Orientation: Portrait
- Allowed Orientations: Portrait only
- Install Location: Automatic

**Step 3: Add notification icons**

Create and add:
- `Assets/Plugins/Android/res/drawable/icon_small.png` (24x24, white silhouette)
- `Assets/Plugins/Android/res/drawable/icon_large.png` (256x256)

---

## Phase 6: Testing & Polish

### Task 22: Create Test Scene for Development

**Files:**
- Create: `Assets/Scripts/Debug/DebugPanel.cs`

**Step 1: Create DebugPanel.cs**

```csharp
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
```

**Step 2: Add debug panel to scene (only shows in Editor/Development builds)**

---

### Task 23: Final Integration Test

**Step 1: Test complete flow in Editor**

1. Launch app → Should show cookie (if after 10 AM) or waiting overlay
2. Tap cookie 5 times → Cookie cracks and breaks
3. Rolled message appears → Drag horizontally to unroll
4. Message revealed → Share button appears
5. Tap share → Native share sheet (or log in editor)
6. Open history → See today's message
7. Restart app → Should show waiting state (already opened today)

**Step 2: Test on iOS Simulator/Device**

1. Build to device
2. Grant notification permission
3. Open cookie, then close app
4. Wait for notification (or test with debug button)
5. Verify notification appears

**Step 3: Test on Android Emulator/Device**

1. Build APK
2. Install and run
3. Same flow as iOS
4. Verify notification channel appears in system settings

---

## Summary

**Total Tasks:** 23

**Key Files Created:**
- Data models: `FortuneMessage.cs`, `SaveData.cs`, `HistoryEntry.cs`
- Managers: `GameManager.cs`, `SaveManager.cs`, `NotificationManager.cs`, `AudioManager.cs`, `HapticsManager.cs`, `ShareManager.cs`
- UI Controllers: `CookieController.cs`, `MessageController.cs`, `WaitingOverlay.cs`, `HistoryPanel.cs`
- Resources: `messages.json` (50 motivational messages)

**Dependencies Added:**
- `com.unity.mobile.notifications` - Local push notifications
- `com.yasirkula.nativeshare` - Native share sheet

**Build Targets:**
- iOS 13.0+
- Android API 24+ (Android 7.0)
