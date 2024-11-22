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

        private void Awake ()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
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

        public void Reset ()
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
            var newRank = oldRank - Random.Range(data.MinRankStep , data.MaxRankStep);
            newRank = Mathf.Max(1 , newRank);
            Show(oldRank , newRank , onComplete);
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
            leaderboard_.Reset();
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
    }
}
