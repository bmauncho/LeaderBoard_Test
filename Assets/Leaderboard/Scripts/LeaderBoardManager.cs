using CustomLeaderBoard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LeaderBoard
{
    [System.Serializable]
    public class playerInfo
    {
        public int Rank;
        public string UserName;
        public Sprite Country;
        public int Score;
    }

    public enum Tiers
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
    public class TierInfo
    {
        [HideInInspector] public string TierName = "";
        public Tiers tier;
        public Color TierColor;
    }

    public class LeaderBoardManager : MonoBehaviour
    {
        public string playerName;
        public TextAsset UserNamesText;
        private string [] _UserNames;
        [SerializeField] private List<Sprite> countries;
        public bool IsUsingTierLeaderBoard = false;
        public bool ManuallySetTierColor = false;
        [SerializeField]private List<TierInfo> TierInfos = new List<TierInfo>();
        

        public playerInfo CreatePlayerInfo ()
        {
            if (_UserNames == null || _UserNames.Length <= 0)
            {
                InitializePlayerNames();
            }
            return new playerInfo()
            {
                UserName = _UserNames [Random.Range(0 , _UserNames.Length)] ,
                Country = countries [Random.Range(0 , countries.Count)]
            };
        }

        void InitializePlayerNames ()
        {
            _UserNames = UserNamesText.text.Split(new [] { '\n' , ',' });
        }

        public void AddTier ( Tiers tier_ , Color color )
        {
            TierInfos.Add(new TierInfo()
            {
               TierName = tier_.ToString(),
               tier = tier_ ,
               TierColor =color,
            });
        }

        public void InitializeTiers ()
        {
            Array Tiers = System.Enum.GetValues(typeof(Tiers));
            int enumLength = Tiers.Length;

            // Ensure there are enough positions for each tier
            if (TierInfos.Count >= enumLength)
                return;

            foreach (Tiers tier in Tiers)
            {

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
                AddTier(tier , tierColor);
            }
        }

        public Color GetColorForTier ( Tiers tier_ )
        {
            // Log the provided tier for debugging
            //Debug.Log($"Requested tier: {tier_}");

            TierInfo threshold = TierInfos.FirstOrDefault(t => t.tier == tier_);

            if (threshold != null)
            {
                //Debug.Log($"Found tier: {threshold.tier}, Color: {threshold.TierColor}");
                return threshold.TierColor;
            }

            Debug.LogWarning($"Tier not found: {tier_}. Returning default color.");
            return Color.white; // Default color
        }

        public Color SetColorforTier ( Tiers tiers )
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
            const string DEFAULT_COLOR = "#45D9FF";

            // Switch case to assign the color
            string hexCode = tiers switch
            {
                Tiers.ROOKIE => ROOKIE_COLOR,
                Tiers.NOVICE => NOVICE_COLOR,
                Tiers.APPRENTICE => APPRENTICE_COLOR,
                Tiers.AMATEUR => AMATEUR_COLOR,
                Tiers.BRAWLER => BRAWLER_COLOR,
                Tiers.PRO => PRO_COLOR,
                Tiers.VETERAN => VETERAN_COLOR,
                Tiers.LEGEND => LEGEND_COLOR,
                Tiers.HERO => HERO_COLOR,
                Tiers.CHAMPION => CHAMPION_COLOR,
                Tiers.SUPERSTAR => SUPERSTAR_COLOR,
                Tiers.IMMORTAL => IMMORTAL_COLOR,
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

        public float MaximumScoreByTier (Tiers tier)
        {
            float theScore = 200;
            switch(tier)
            {
                case Tiers.ROOKIE:
                    return theScore;
                case Tiers.NOVICE:
                    return theScore + 200;
                case Tiers.APPRENTICE:
                    return theScore + 200;
                case Tiers.AMATEUR:
                    return theScore + 200;
                case Tiers.BRAWLER:
                    return theScore + 200;
                case Tiers.PRO:
                    return theScore + 200;
                case Tiers.VETERAN:
                    return theScore + 200;
                case Tiers.LEGEND:
                    return theScore + 200;
                case Tiers.HERO:
                    return theScore + 200;
                case Tiers.CHAMPION:
                    return theScore + 200;
                case Tiers.SUPERSTAR:
                    return theScore + 200;
                case Tiers.IMMORTAL:
                    return theScore + 200;
                default:
                    return theScore;
            }
        }

    }

    [System.Serializable]
    public static class ColorUtils
    {
        public static bool IsColorBright ( Color color )
        {
            // Calculate luminance based on the Rec. 709 formula
            float luminance = 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;

            // Define a threshold for brightness
            float threshold = 0.5f;

            return luminance > threshold;
        }
    }

}

