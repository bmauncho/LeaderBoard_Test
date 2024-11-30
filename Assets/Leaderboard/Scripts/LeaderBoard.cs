using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
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
        ItemData special_item;
        public GameObject LeaderBoardContainer;
        public RectTransform ContentContainer_;
        public ScrollRect scrollRect;
        public GameObject playerItem;
        public List<ItemData> LeaderboardEntries = new List<ItemData>();


        

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
                LeaderBoardTitle.text = "LEADERBOARD";
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
            StartCoroutine(InitializelocalLeaderBoard());
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

        public void ArrangeLeaderBoard ()
        {
            float entryTemplateHeight = 75f;
            for(int i = 0;i<LeaderboardEntries.Count;i++)
            {
                RectTransform entryTransform = LeaderboardEntries [i].GetComponent<RectTransform>();
                entryTransform.anchoredPosition = new Vector2(0 , -entryTemplateHeight * i);
            }
        }

        private IEnumerator InitializelocalLeaderBoard ()
        {
            yield return StartCoroutine(refreshLocalLeaderBoard());

            UpdateContentHeight(ContentContainer_ , 75f , 0 , 0);
            
            List<int> Scores = new List<int>();

            for(int i = 0; i < LeaderboardEntries.Count; i++)
            {
                Scores.Add(GetPlayerScore());
            }

            for(int i = 0;i< Scores.Count ; i++)
            {
                for(int j = i+1; j < Scores.Count ; j++)
                {
                    if(Scores [j] > Scores [i])
                    {
                        int score = Scores [i];
                        Scores [i] = Scores [j];
                        Scores [j] = score;
                    }
                }
            }

            for(int i = 0;i< LeaderboardEntries.Count ; i++)
            {
                LeaderboardEntries[i].PositionCounter_.SetScore(Scores [i]);
                int rank = i+1;
                LeaderboardEntries [i].InitializePlayer(CreatePlayerInfo() , rank , LeaderboardEntries [i].PositionCounter_.Score);
            }

            playerInfo playerInfo = new playerInfo
            {
                UserName = data.playerName ,
            };
            int PlayerScore = GetPlayerScore();
            int index = DetermineIndex(PlayerScore);
            var targetPlayerItem = special_item = LeaderboardEntries [index];
            targetPlayerItem.InitializePlayer(playerInfo , LeaderboardEntries [index].PositionCounter_.CurrentPosition, PlayerScore);
            yield return new WaitForSeconds(.25f);
            HighlightPlayer(targetPlayerItem);
            float TargetPos = GetNormalizedScrollPosition(targetPlayerItem.GetComponent<RectTransform>());
            yield return StartCoroutine ( ScrollToRank(TargetPos , 1f ));
        }
        bool IsUp ( int oldRankPosition , int newRankPosition )
        {
            return newRankPosition <= oldRankPosition;
        }

        public int GetOldRank ()
        {
            return 0;
        }

        private int DetermineIndex (int score_)
        {
            int index = -1; // Start with an invalid index
            float score = score_;
            float lowestDifference = int.MaxValue; // Initialize with the maximum possible difference

            for (int i = 0 ; i < LeaderboardEntries.Count ; i++)
            {
                if (LeaderboardEntries [i].gameObject.activeInHierarchy)
                {
                    float diff = Math.Abs(LeaderboardEntries [i].PositionCounter_.Score - score);

                    // Update index if this difference is smaller or if it's an exact match
                    if (diff < lowestDifference)
                    {
                        lowestDifference = diff;
                        index = i;
                    }
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
                    for (int j = 0 ;j< itemData.EditableImages.Length ; j++)
                    {
                        if (ColorUtility.TryParseHtmlString(Higlight_color , out Color color_))
                        {
                            itemData.EditableImages [j].color = color_ ;
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

        private IEnumerator refreshLocalLeaderBoard ()
        {
            foreach (ItemData itemData in LeaderboardEntries)
            {
                // Reactivate inactive entries
                if (!itemData.gameObject.activeSelf)
                {
                    itemData.gameObject.SetActive(true);
                }

                // Reset the colors using the method in ItemData
                itemData.ResetColors();
                itemData.ResetItemData();
            }

            int playerCount = GetPlayerCount();
            if (LeaderboardEntries.Count > playerCount)
            {
                // Disable children above the player count
                for (int i = playerCount ; i < LeaderBoardContainer.transform.childCount ; i++)
                {
                    Transform child = LeaderBoardContainer.transform.GetChild(i);
                    child.gameObject.SetActive(false);
                }
            }
            yield return null;
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
        }

        public IEnumerator ScrollToRank ( float targetPos , float Duration,Action OnComplete = null )
        {
            float startPos = scrollRect.verticalNormalizedPosition;

            // Exit the coroutine if startPos and targetPos are the same
            if (Mathf.Approximately(startPos , targetPos))
                yield break;

            float time = 0;

            while (time < Duration)
            {
                time += Time.deltaTime;
                scrollRect.horizontalNormalizedPosition = 0;
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos , targetPos , time / Duration);
                yield return null;
            }

            scrollRect.verticalNormalizedPosition = targetPos;
            scrollRect.normalizedPosition = new Vector2(0 , scrollRect.verticalNormalizedPosition);
            OnComplete?.Invoke();
        }


        public void ScrollUp ()
        {
           // StartCoroutine(ScrollToRank(1 , 1.5f));
        }

        public void ScrollDown ()
        {
            //StartCoroutine(ScrollToRank(0 , 1.5f));
        }

        bool IsUp ( float value )
        {
            // Return true if value is greater than 0.5, otherwise false
            return value > 0.5f && value <= 1f;
        }

        float GetNormalizedScrollPosition ( RectTransform target )
        {
            // Get the target position relative to the scroll content
            Vector2 targetPosition = (Vector2)scrollRect.content.InverseTransformPoint(target.position);

            // Get the content size
            RectTransform contentRect = scrollRect.content;

            // Calculate the normalized Y position
            float normalizedY = ( targetPosition.y - contentRect.rect.yMin ) / contentRect.rect.height;

            if( normalizedY <= 0.2)
            {
                normalizedY = 0f;
            }
            else if(normalizedY>=0.4f && normalizedY <= 0.6)
            {
                normalizedY = 0.5f;
            }
            else if( normalizedY >= 0.8f)
            {
                normalizedY = 1f;
            }
            // Return the clamped normalized Y value
            return Mathf.Clamp01(normalizedY);
        }

        private IEnumerator ScaleDown (Transform target,float Dur_)
        {
            Vector3 targetScale = new Vector3(1f , 1f , 1f);  // Target scale of (1, 1, 1)
            Vector3 currentScale = target.transform.localScale;

            float duration = Dur_;  // Time to complete the scale transition
            float timeElapsed = 0f;

            // Gradually reduce the scale over time
            while (timeElapsed < duration)
            {
                target.transform.localScale = Vector3.Lerp(currentScale , targetScale , timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;  // Wait until the next frame
            }

            target.transform.localScale = targetScale;  // Ensure we end at the exact target scale
        }
    }
}

