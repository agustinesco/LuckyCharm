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
