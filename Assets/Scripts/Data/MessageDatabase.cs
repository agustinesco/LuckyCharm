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
                UnityEngine.Debug.LogError("messages.json not found in Resources!");
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
