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
            // GameManager handles DontDestroyOnLoad for the root parent
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
                    UnityEngine.Debug.LogError($"Failed to load save data: {e.Message}");
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
                UnityEngine.Debug.LogError($"Failed to save data: {e.Message}");
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
