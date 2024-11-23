using System;
using System.Collections;
using System.Collections.Generic;
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
            var rankProgress = GetRandomProgress();
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

        

        public int GetRandomProgress ()
        {
            int randomProgress = Random.Range(1 , 100);

            // Ensure the random value is not in the recent values list.
            while (recentRandomProgressValues.Contains(randomProgress))
            {
                randomProgress = Random.Range(1 , 100);
            }

            // Add the new value to the list of recent values.
            recentRandomProgressValues.Add(randomProgress);

            // Limit the size of the recent values list to avoid memory issues.
            if (recentRandomProgressValues.Count > 4) // Arbitrary size limit; adjust as needed.
            {
                recentRandomProgressValues.RemoveAt(0);
            }

            return randomProgress;
        }
    }
}
