using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [HideInInspector]public string TierName = "";
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
         public bool UseTierLeaderboared;
         public bool ManuallySetTierColor;
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


        // Get the tier for a given rank
        public Tier GetTierByRank ( int rank )
        {
            foreach (var threshold in tierThresholds)
            {
                if (rank >= threshold.MinRank && rank <= threshold.MaxRank)
                    return threshold.Tier;
            }
            return Tier.ROOKIE; // Default to ROOKIE if no match
        }

        public TierThreshold? GetNextTier ( Tier currentTier )
        {
            var currentIndex = tierThresholds.FindIndex(t => t.Tier == currentTier);
            if (currentIndex < 0 || currentIndex >= tierThresholds.Count - 1)
                return null; // No next tier
            return tierThresholds [currentIndex + 1];
        }


        // Add a tier dynamically (for scalability)
        public void AddTier ( Tier tier , int minRank , int maxRank , Color color )
        {
            tierThresholds.Add(new TierThreshold
            {
                TierName = tier.ToString(),
                Tier = tier ,
                MinRank = minRank ,
                MaxRank = maxRank ,
                TierColor = color
            });
        }

        public void InitializeTiers ()
        {
            Array Tiers = System.Enum.GetValues(typeof(Tier));
            int enumLength = Tiers.Length;

            // Ensure there are enough positions for each tier
            if (tierThresholds.Count >= enumLength)
                return;

            // Positions per tier is strictly 100 for each tier
            int currentMinRank = 1;
            int currentMaxRank = 100;

            foreach (Tier tier in Tiers)
            {
                // Ensure that the rank range stays within 1-100 for each tier
                if (currentMinRank > 100)
                {
                    currentMinRank = 1;  // Reset the minimum rank to 1 for the next tier
                    currentMaxRank = 100; // Set max rank to 100 again
                }

                Color tierColor = Color.white;
                if (ManuallySetTierColor)
                {
                    tierColor = GetColorForTier(tier);
                }
                else
                {
                    tierColor = SetColorforTier(tier);
                }

                // Add the tier with the correct rank range
                AddTier(tier , currentMinRank , currentMaxRank , tierColor);

                // Increment the rank range for the next tier
                currentMinRank = currentMaxRank + 1;

                // If the rank exceeds 100, reset the range for the next tier
                if (currentMinRank > 100)
                {
                    currentMinRank = 1;
                    currentMaxRank = 100;
                }
            }
        }



        public Color GetColorForTier ( Tier tier )
        {
            // Search for the corresponding TierThreshold
            TierThreshold threshold = tierThresholds.FirstOrDefault(t => t.Tier == tier);

            // If found, return the color; otherwise, return a default color
            return threshold != null ? threshold.TierColor : Color.white;

        }

        public List<TierThreshold> GetTierThresholds ()
        {
            return tierThresholds;
        }

        public Color SetColorforTier ( Tier tier )
        {
            // Define hex codes for each tier
            const string ROOKIE_COLOR = "#45D9FF";     
            const string NOVICE_COLOR = "#BB45FF";      
            const string APPRENTICE_COLOR = "#4590FF";  
            const string AMATEUR_COLOR = "#45FF99";     
            const string BRAWLER_COLOR = "#FF6B45";     
            const string PRO_COLOR = "#FF45C6";         
            const string VETERAN_COLOR = "#D245FF";     
            const string LEGEND_COLOR = "#FFA645";      
            const string HERO_COLOR = "#4575FF";        
            const string CHAMPION_COLOR = "#4B4B4B";    
            const string SUPERSTAR_COLOR = "#FF0000";   
            const string IMMORTAL_COLOR = "#6A2BD6";   

            // Default color if no match
            const string DEFAULT_COLOR = "#FFFFFF";     // White

            // Switch case to assign the color
            string hexCode = tier switch
            {
                Tier.ROOKIE => ROOKIE_COLOR,
                Tier.NOVICE => NOVICE_COLOR,
                Tier.APPRENTICE => APPRENTICE_COLOR,
                Tier.AMATEUR => AMATEUR_COLOR,
                Tier.BRAWLER => BRAWLER_COLOR,
                Tier.PRO => PRO_COLOR,
                Tier.VETERAN => VETERAN_COLOR,
                Tier.LEGEND => LEGEND_COLOR,
                Tier.HERO => HERO_COLOR,
                Tier.CHAMPION => CHAMPION_COLOR,
                Tier.SUPERSTAR => SUPERSTAR_COLOR,
                Tier.IMMORTAL => IMMORTAL_COLOR,
                _ => DEFAULT_COLOR,
            };

            // Parse the hex code to a Unity Color
            if (ColorUtility.TryParseHtmlString(hexCode , out Color color))
            {
                return color;
            }

            // Fallback to white if parsing fails
            return Color.white;
        }

    }
}