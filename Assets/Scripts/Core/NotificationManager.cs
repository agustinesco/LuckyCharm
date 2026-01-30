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

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // GameManager handles DontDestroyOnLoad for the root parent
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

            string title = LocalizationManager.Instance?.GetLocalizedString("Messages", "notification_title")
                ?? "Your Lucky Cookie is Ready!";
            string body = LocalizationManager.Instance?.GetLocalizedString("Messages", "notification_body")
                ?? "Come crack open today's fortune and discover your message.";

#if UNITY_ANDROID
            var notification = new AndroidNotification
            {
                Title = title,
                Text = body,
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
                Title = title,
                Body = body,
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                Trigger = timeTrigger
            };
            iOSNotificationCenter.ScheduleNotification(notification);
#endif

            UnityEngine.Debug.Log($"Notification scheduled for {scheduledTime}");
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
