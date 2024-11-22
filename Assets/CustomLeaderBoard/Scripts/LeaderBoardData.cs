using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomLeaderBoard
{
    public enum Tier
    {
        ROOKIE,
        NOVICE,
        APPRENTICE,
        AMATEUR,
        BRAWLER,
        PRO,
        VETERAN,
        LEGEND,
        HERO,
        CHAMPION,
        SUPERSTAR,
        IMMORTAL
    }

    [System.Serializable]
    public class TierThreshold
    {
        public Tier Tier;
        public int MinRank;
        public int MaxRank;
        public Color TierColor;
    }

    public struct PlayerInfo
    {
        public Sprite Country; // Country flag
        public string Username; // Username
        public int Rank; // Rank
    }


    [CreateAssetMenu(fileName = "LeaderBoardData" , menuName = "CustomLeaderboard/LeaderBoardData" , order = 0)]
    public class LeaderBoardData : ScriptableObject
    {
        [Header("Common settings")]
        // Default player name
        [SerializeField] public string playerName;

        // Text asset with usernames
        [SerializeField] private TextAsset usernamesText;

        // List of flags
        [SerializeField] private List<Sprite> countries;

        [Header("LeaderBoard")]

        // Popup title
        [SerializeField] private string LeaderBoardTitle;

        // Rank up sprite
        [SerializeField] private Sprite rankUpSprite;

        // Rank up color
        [SerializeField] private Color rankUpColor;

        // Rank down sprite
        [SerializeField] private Sprite rankDownSprite;

        // Rank down color
        [SerializeField] private Color rankDownColor;

        // Animation duration for rank counter
        [SerializeField] private float rankCounterAnimationDuration;

        // LeaderBoard appear animation duration
        [SerializeField] private float LeaderBoardAppearDuration;

        // LeaderBoard disappear animation duration
        [SerializeField] private float LeaderBoardDisappearDuration;

        [Header("Progress")]
        // Min initial rank position in leaderboard
        [SerializeField] private int minInitialRankPosition;

        // Max initial rank position in leaderboard
        [SerializeField] private int maxInitialRankPosition;

        // Minimum rank position decrement for one step
        [SerializeField] private int minRankDecrement;

        // Maximum rank position decrement for one step
        [SerializeField] private int maxRankDecrement;


        [Header("Tier Settings")]
        [SerializeField] private List<TierThreshold> tierThresholds;

        [Header("Return Data")]
        // Parsed usernames
        private string [] _usernames;

        // Returns default player name
        public string PlayerName => playerName;

        // Popup title
        public string PopupTitle => LeaderBoardTitle;

        // Returns rank up sprite
        public Sprite RankUpSprite => rankUpSprite;

        // Returns rank up color
        public Color RankUpColor => rankUpColor;

        // Returns rank down color
        public Color RankDownColor => rankDownColor;

        // Returns rank down sprite
        public Sprite RankDownSprite => rankDownSprite;

        // Returns min initial rank for auto progress
        public int MinInitialRank => minInitialRankPosition;

        // Returns max initial rank for auto progress
        public int MaxInitialRank => maxInitialRankPosition;

        // Returns min rank step in auto progress
        public int MinRankStep => minRankDecrement;

        // Returns max rank step in auto progress
        public int MaxRankStep => maxRankDecrement;

        // Returns counter animation duration
        public float RankCounterAnimationDuration => rankCounterAnimationDuration;

        // Returns show animation duration
        public float PopupShowAnimationDuration => LeaderBoardAppearDuration;

        // Returns hide animation duration
        public float PopupHideAnimationDuration => LeaderBoardDisappearDuration;


        public PlayerInfo CreatePlayerInfo ()
        {
            if (_usernames == null || _usernames.Length == 0)
            {
                InitializeUsernames();
            }

            return new PlayerInfo()
            {
                Username = _usernames [Random.Range(0 , _usernames.Length)] ,
                Country = countries [Random.Range(0 , countries.Count)]
            };
        }

        // Initializes usernames from text file
        private void InitializeUsernames () => _usernames = usernamesText.text.Split(new [] { '\n' , ',' });

        #region
        //// Get the tier for a given rank
        //public Tier GetTierByRank ( int rank )
        //{
        //    foreach (var threshold in tierThresholds)
        //    {
        //        if (rank >= threshold.MinRank && rank <= threshold.MaxRank)
        //            return threshold.Tier;
        //    }
        //    return Tier.ROOKIE; // Default to ROOKIE if no match
        //}

        //// Add a tier dynamically (for scalability)
        //public void AddTier ( Tier tier , int minRank , int maxRank,Color color)
        //{
        //    tierThresholds.Add(new TierThreshold
        //    {
        //        Tier = tier ,
        //        MinRank = minRank ,
        //        MaxRank = maxRank,
        //        TierColor = color
        //    });
        //}

        //public void InitializeTiers ()
        //{
        //    int positionsPerTier = 100;
        //    int currentMinRank = 1;

        //    foreach (Tier tier in System.Enum.GetValues(typeof(Tier)))
        //    {
        //        int currentMaxRank = currentMinRank + positionsPerTier - 1;
        //        Color tierColor = GetColorForTier(tier);
        //        AddTier(tier , currentMinRank , currentMaxRank,tierColor);

        //        currentMinRank = currentMaxRank + 1;
        //    }
        //}

        //public Color GetColorForTier ( Tier tier )
        //{
        //    // Search for the corresponding TierThreshold
        //    TierThreshold threshold = tierThresholds.FirstOrDefault(t => t.Tier == tier);

        //    // If found, return the color; otherwise, return a default color
        //    return threshold != null ? threshold.TierColor : Color.white;
        #endregion
    }
}