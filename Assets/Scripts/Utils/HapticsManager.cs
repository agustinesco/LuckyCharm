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
            // GameManager handles DontDestroyOnLoad for the root parent
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
