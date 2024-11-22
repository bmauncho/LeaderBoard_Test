using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomLeaderBoard
{
    public class LeaderboardManager : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private string defaultPlayerName = "Player";
        [SerializeField] private float popupShowDuration = 0.5f;
        [SerializeField] private float popupHideDuration = 0.5f;
        [SerializeField] private float hideDelay = 5f;

        [Header("Leaderboard Data")]
        [SerializeField] private LeaderBoardData leaderboardData;
        [SerializeField] private Transform leaderboardContainer;
        [SerializeField] private GameObject leaderboardEntryPrefab;

        [Header("Color Customization")]
        public Color defaultColor = Color.white;
        public List<Color> tierColors = new List<Color>();

        private int currentRank;
        private int targetRank;
        private bool isPopupVisible = false;

        private static LeaderboardManager _instance;
        public static LeaderboardManager Instance => _instance;

        private void Awake ()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        private void Start ()
        {
            LoadPlayerRank();
        }

        private void LoadPlayerRank ()
        {
            currentRank = PlayerPrefs.GetInt("PlayerRank" , 100); // Default rank is 100.
            targetRank = currentRank;
        }

        public void SavePlayerRank ( int rank )
        {
            PlayerPrefs.SetInt("PlayerRank" , rank);
            PlayerPrefs.Save();
        }

        public void ShowLeaderboard ( int newRank , Action onComplete = null )
        {
            targetRank = newRank;

            if (!isPopupVisible)
            {
                StartCoroutine(ShowLeaderboardPopup(onComplete));
            }
        }

        private IEnumerator ShowLeaderboardPopup ( Action onComplete )
        {
            isPopupVisible = true;
            leaderboardContainer.localScale = Vector3.zero;
            leaderboardContainer.gameObject.SetActive(true);

            // Animate the popup showing
            yield return AnimatePopup(Vector3.zero , Vector3.one , popupShowDuration);

            UpdateLeaderboardUI();

            if (onComplete != null)
                onComplete.Invoke();
        }

        public void HideLeaderboard ( Action onComplete = null )
        {
            if (!isPopupVisible) return;

            StartCoroutine(HideLeaderboardPopup(onComplete));
        }

        private IEnumerator HideLeaderboardPopup ( Action onComplete )
        {
            // Animate the popup hiding
            yield return AnimatePopup(Vector3.one , Vector3.zero , popupHideDuration);

            leaderboardContainer.gameObject.SetActive(false);
            isPopupVisible = false;

            if (onComplete != null)
                onComplete.Invoke();
        }

        private IEnumerator AnimatePopup ( Vector3 startScale , Vector3 endScale , float duration )
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                leaderboardContainer.localScale = Vector3.Lerp(startScale , endScale , elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            leaderboardContainer.localScale = endScale;
        }

        private void UpdateLeaderboardUI ()
        {
            ClearLeaderboardEntries();

            // Simulate leaderboard data
            for (int i = 1 ; i <= 10 ; i++)
            {
                var entry = Instantiate(leaderboardEntryPrefab , leaderboardContainer);
                var entryText = entry.GetComponentInChildren<TMP_Text>();
                entryText.text = $"Player {i} - Rank {i}";
                entry.GetComponent<Image>().color = i == targetRank ? tierColors [0] : defaultColor;
            }
            SavePlayerRank(targetRank);
        }

        private void ClearLeaderboardEntries ()
        {
            foreach (Transform child in leaderboardContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
