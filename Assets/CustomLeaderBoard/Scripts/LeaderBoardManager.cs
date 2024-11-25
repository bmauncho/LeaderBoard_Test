using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace CustomLeaderBoard
{
    public class LeaderboardManager : MonoBehaviour
    {
        public Action CloseAfterDelay;
        private static LeaderboardManager _instance;
        public static LeaderboardManager Instance => _instance;
        private const string PlayerRankKey = "PlayerRank";
        [SerializeField] private LeaderBoardData data;
        private LeaderBoard leaderBoardPopup;
        private EventSystem eventSystem;
        private LeaderBoard leaderboard_;
        [SerializeField]private List<int> recentRandomProgressValues = new List<int>(); // To store recent random values.

        private void Awake ()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            InitializeComponents();
        }

        public PlayerInfo CreatePlayerInfo ()
        {
            return data.CreatePlayerInfo();
        }

        public int GetRank ()
        {
            return PlayerPrefs.GetInt
                (PlayerRankKey , Random.Range(
                data.MinInitialRank ,
                data.MaxInitialRank
                ));
        }

        public void ResetLeaderBoard ()
        {
            PlayerPrefs.SetInt(PlayerRankKey , Random.Range(
                data.MinInitialRank ,
                data.MaxInitialRank
            ));
            PlayerPrefs.Save();
        }
        // Shows rank popup with auto progress
        public void Show ( Action onComplete = null )
        {
            var oldRank = GetRank();
            var rankProgress = GetRandomProgress(oldRank);
            //var rankProgress = oldRank - Random.Range(data.MinRankStep , data.MaxRankStep);
            rankProgress = Mathf.Max(1 , rankProgress);
           
            //rankProgress = ( 100 - rankProgress );
            Show(oldRank , rankProgress , onComplete);
        }
        // Shows rank popup without auto progress
        public void Show ( int oldRankPosition , int newRankPosition , Action onComplete = null )
        {
            PlayerPrefs.SetInt(PlayerRankKey , newRankPosition);
            PlayerPrefs.Save();
            StartCoroutine(ShowCoroutine(oldRankPosition , newRankPosition , onComplete));
        }
        // Hides rank popup
        public void Hide ( Action onComplete = null )
        {
            if (leaderboard_ == null)
            {
                return;
            }
            leaderboard_.Hide(onComplete);
        }
        // Shows popup
        private IEnumerator ShowCoroutine ( int oldRankPosition , int newRankPosition , Action onComplete = null )
        {
            InitializeComponents();
            leaderboard_.Refreshleaderboard();
            yield return null;
            leaderboard_.Show(oldRankPosition , newRankPosition , onComplete);
        }
        // Initializes canvas and input event system
        private void InitializeComponents ()
        {
            var es = FindObjectOfType<EventSystem>(true);
            leaderboard_ = FindObjectOfType<LeaderBoard>(true);
            if (leaderboard_ == null)
            {
                leaderboard_ = Instantiate(leaderBoardPopup);
            }
            if (es == null)
            {
                Instantiate(eventSystem);
            }
        }

        public int GetRandomProgress ( int currentRank )
        {
            var currentTier = data.GetTierByRank(currentRank);

            // Ensure there's a valid match for the current tier
            var tierThreshold = data.GetTierThresholds().FirstOrDefault(t => t.Tier == currentTier);
            if (tierThreshold == null)
                throw new InvalidOperationException("Invalid tier structure or current rank.");

            // Check if at the top of the current tier
            if (currentRank == tierThreshold.MinRank)
            {
                var nextTier = data.GetNextTier(currentTier);
                if (nextTier != null)
                {
                    return nextTier.MaxRank; // Move to position 100 of the next tier
                }
            }

            // Default progression logic
            int randomProgress = Random.Range(1 , 100);
            while (recentRandomProgressValues.Contains(randomProgress))
            {
                randomProgress = Random.Range(1 , 100);
            }

            // Update recent progress values
            recentRandomProgressValues.Add(randomProgress);
            if (recentRandomProgressValues.Count > 4)
            {
                recentRandomProgressValues.RemoveAt(0);
            }

            return Mathf.Max(1 , currentRank - randomProgress);
        }

        public void UpdatePlayerRank ( int newRank )
        {
            var currentRank = GetRank();
            var currentTier = data.GetTierByRank(currentRank);

            if (newRank == 1)
            {
                // Rank up to the next tier
                var nextTier = data.GetNextTier(currentTier);
                if (nextTier != null)
                {
                    newRank = nextTier.MaxRank; // Move to position 100 of the next tier 
                }
            }
            else if (newRank == 100)
            {
                // Rank down to the previous tier
                var previousTier = GetPreviousTier(currentTier);
                if (previousTier != null)
                {
                    newRank = previousTier.MinRank; // Move to position 1 of the previous tier
                }
            }

            // Update the rank in PlayerPrefs
            PlayerPrefs.SetInt(PlayerRankKey , newRank);
            PlayerPrefs.Save();
        }

        private TierThreshold? GetPreviousTier ( Tier currentTier )
        {
            var thresholds = data.GetTierThresholds();
            var currentIndex = thresholds.FindIndex(t => t.Tier == currentTier);
            if (currentIndex > 0)
            {
                return thresholds [currentIndex - 1];
            }
            return null; // No previous tier
        }
    }
}
