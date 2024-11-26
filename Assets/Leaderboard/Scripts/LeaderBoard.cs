using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LeaderBoard
{
    public class LeaderBoard : MonoBehaviour
    {
        LeaderBoardManager data;
        [Header("Tier")]
        [SerializeField]public Tiers ActiveTier;

        [Header("LeaderBoard")]
        public TMP_Text LeaderBoardTitle;
        public GameObject TheLeaderBoard;
        public GameObject Banner;
        bool IsUseTiers = false;

        [Header("Leaderboard Content")]

        public GameObject LeaderBoardContainer;
        public RectTransform ContentContainer_;
        public GameObject entrytemplate;

        public List<ItemData> LeaderboardEntries = new List<ItemData>();

        ItemData special_item;
        // Start is called before the first frame update
        void Start ()
        {
            data = FindObjectOfType<LeaderBoardManager>(true);
        }

        // Update is called once per frame
        void Update ()
        {
            if(data != null)
            {
                IsUseTiers = data.CanUseTiers;
            }
        }
        #region
        /// <summary>
        /// leaderboard controller
        /// </summary>
        public void ResetLeaderBoard ()
        {
            HideLeaderBoard();
        }

        public void refreshLeaderBoard ()
        {
            SetLeaderBoardTitle();
        }

        public playerInfo CreatePlayerInfo ()
        {
            return data.CreatePlayerInfo();
        }
        
        public void SetLeaderBoardTitle ()
        {
            
            if (!IsUseTiers)
            {
                LeaderBoardTitle.text = "WORLD RANK";
                SetLeaderBoardColor(false,LeaderBoardTitle);
            }
            else
            { 
                LeaderBoardTitle.text = "WORLD RANK" + " : " + ActiveTier.ToString();
                SetLeaderBoardColor(true, LeaderBoardTitle);
            }
        }

        public void SetLeaderBoardColor (bool IsUseTiers,TMP_Text text)
        {
            const string DefaultBanner_color = "#F3F3F3";
            const string DefaultText_color = "#282828";
       
            if (IsUseTiers)
            {
                Color newColor = Banner.GetComponent<Image>().color = data.GetColorForTier(ActiveTier);
                if (ColorUtils.IsColorBright(newColor))
                {
                    if (ColorUtility.TryParseHtmlString(DefaultText_color , out Color color_))
                    {
                        text.color = color_;
                    }
                }
                else
                {
                    text.color = Color.white;
                }

            }
            else
            {
                if (ColorUtility.TryParseHtmlString(DefaultBanner_color , out Color color))
                {
                    Banner.GetComponent<Image>().color = color;
                }

                if(ColorUtility.TryParseHtmlString(DefaultText_color , out Color color_))
                {
                    text.color = color_;
                }
            }
        }

        public void ShowLeaderBoard ()
        {
            TheLeaderBoard.SetActive(true);
            TheLeaderBoard.transform.localScale = Vector3.one;
            SetLeaderBoardTitle();
        }

        public void HideLeaderBoard ()
        {
            TheLeaderBoard.SetActive(false);
            TheLeaderBoard.transform.localScale = Vector3.zero;
        }
        #endregion

        #region
        /// <summary>
        /// Tier Leaderboard
        /// </summary>
        public void IncreaseTier ()
        {
            ActiveTier++;

            Array tiersArray = System.Enum.GetValues(typeof(Tiers));
            int enumLength = tiersArray.Length;

            // Check if ActiveTier exceeds the highest tier
            if ((int)ActiveTier >= enumLength) 
            {
                ActiveTier = (Tiers)( enumLength - 1 ); // Set to the last enum value
            }
            SetLeaderBoardTitle();
        }

        public void LowerTier ()
        {
            ActiveTier--;

            if ((int)ActiveTier < 0) 
            {
                ActiveTier = (Tiers)0; 
            }

            SetLeaderBoardTitle();
        }
        #endregion

        /// <summary>
        /// normal leaderboard / highscorelist
        /// </summary>
        /// 

        private int DetermineIndex (int score_)
        {
            int index = -1; // Start with an invalid index
            int score = score_;
            int lowestDifference = int.MaxValue; // Initialize with the maximum possible difference

            for (int i = 0 ; i < LeaderboardEntries.Count ; i++)
            {
                int diff = Math.Abs(LeaderboardEntries [i].PositionCounter_.Score - score);

                // Update index if this difference is smaller or if it's an exact match
                if (diff < lowestDifference)
                {
                    lowestDifference = diff;
                    index = i;
                }
            }
            return Mathf.Max(index , 0);
        }

        private void HighlightPlayer(ItemData itemData )
        {
            const string Higlight_color = "#FFAF00";
            for (int i = 0 ;i< LeaderboardEntries.Count ;i++)
            {
                if (LeaderboardEntries [i]== itemData)
                {
                    for (int j = 0 ;j< itemData.EditableColorsImages.Length ; j++)
                    {
                        if (ColorUtility.TryParseHtmlString(Higlight_color , out Color color_))
                        {
                            itemData.EditableColorsImages [j].color = color_ ;
                        }
                    }
                }
            }
        }

        private int GetPlayerCount ()
        {
            return Random.Range(10 , 20);
        }

        int GetPlayerScore ()
        {
            return Random.Range(5 , 100);
        }

       
       
        public void UpdateContentHeight ( RectTransform contentContainer , float itemHeight , float spacing , float padding )
        {
            int activeChildCount = 0;

            // Count active children only
            foreach (Transform child in contentContainer)
            {
                if (child.gameObject.activeSelf)
                {
                    activeChildCount++;
                }
            }

            // Calculate the total height based on active children
            float totalHeight = ( activeChildCount * itemHeight ) + ( ( activeChildCount - 1 ) * spacing ) + ( 2 * padding );
            contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x , totalHeight);

            Debug.Log($"Updated content height to: {totalHeight}, Active children: {activeChildCount}");
        }

    }
}

