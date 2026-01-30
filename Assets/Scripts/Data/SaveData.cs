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
