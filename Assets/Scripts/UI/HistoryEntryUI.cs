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
