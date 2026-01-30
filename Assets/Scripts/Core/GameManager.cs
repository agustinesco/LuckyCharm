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

            // Apply DontDestroyOnLoad to root parent (Managers)
            Transform root = transform.root;
            if (root != null)
            {
                DontDestroyOnLoad(root.gameObject);
            }
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
