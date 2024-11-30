using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LeaderBoard
{
    public class TierLeaderBoard : MonoBehaviour
    {
        LeaderBoardManager data;
        [Header("Tier")]
        [SerializeField] public Tiers ActiveTier;

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
        public Sprite RankUpSprite;
        public Color RankUpColor;
        public Sprite RankDownSprite;
        public Color RankDownColor;
        public GameObject playerItemContainer;
        public List<ItemData> LeaderboardEntries = new List<ItemData>();
        Dictionary<string,Dictionary<int,float>> playerDetails = new Dictionary<string,Dictionary<int,float>>();
        int oldRank_ = 0;
       // Start is called before the first frame update
        void Start ()
        {
            data = FindObjectOfType<LeaderBoardManager>(true);
            ResetLeaderBoard();
        }

        // Update is called once per frame
        void Update ()
        {
            if (data != null)
            {
                IsUseTiers = data.IsUsingTierLeaderBoard;
            }
        }

        void ResetLeaderBoard ()
        {
            scrollRect.verticalNormalizedPosition = 0.5f;
        }

        public void ArrangeLeaderBoard ()
        {
            float entryTemplateHeight = 75f;
            for (int i = 0 ; i < LeaderboardEntries.Count ; i++)
            {
                RectTransform entryTransform = LeaderboardEntries [i].GetComponent<RectTransform>();
                entryTransform.anchoredPosition = new Vector2(0 , -entryTemplateHeight * i);
            }
        }

        public void ShowLeaderBoard ()
        {
            ResetLeaderBoard();
            TheLeaderBoard.SetActive(true);
            TheLeaderBoard.transform.localScale = Vector3.one;
            SetLeaderBoardTitle();
        }

        public void HideLeaderBoard ()
        {
            ResetLeaderBoard ();
            TheLeaderBoard.SetActive(false);
            TheLeaderBoard.transform.localScale = Vector3.zero;
        }

        public void SetLeaderBoardTitle ()
        {
            LeaderBoardTitle.text = "WORLD RANK" + " : " + ActiveTier.ToString();
            SetLeaderBoardColor(LeaderBoardTitle);
        }

        public void SetLeaderBoardColor ( TMP_Text text )
        {
            const string DefaultText_color = "#282828";

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

        public void UpdateTierLeaderBoard ()
        {
            ShowLeaderBoard();
            UpdateContentHeight(ContentContainer_ , 75f , 0 , 0);
            Show(GetRandomOldRank(),(100 - GetRandomOldRank()),GetPlayerScore());
        }

        public void rankUp ()
        {
            int newRank = oldRank_ - 20;
            Debug.Log("Rank up position: " + newRank);
            if (newRank <= 1)
            {
                newRank = 1;
            }
            UpdateContentHeight(ContentContainer_ , 75f , 0 , 0);
            Show(oldRank_ , newRank , GetPlayerScore());
            
        }

        public void RankDown ()
        {
            int newRank = oldRank_ + 20;
            Debug.Log("Rank down position: "+newRank);
            if (newRank >= 100)
            {
                newRank = 100;
            }
            UpdateContentHeight(ContentContainer_ , 75f , 0 , 0);
            Show(oldRank_ , newRank , GetPlayerScore());
        }


        public void Show(int oldRank, int newRank, float newScore )
        {
            bool isUp = IsUp(oldRank, newRank);
            var rankChangeSprite = GetRankChangeSprite(isUp);
            var rankChangeColor = GetRankChangeColor(isUp);

            var playerInfo = data.CreatePlayerInfo();

            var index = DetermineIndex(newRank);
            if (!isUp)
            {
                index = LeaderboardEntries.Count - 3;
            }

            var place = newRank - index;
            for( int i = 0;i<LeaderboardEntries.Count;i++ )
            {
                LeaderboardEntries [i].SetUpPlayers(playerInfo.UserName , place , newScore);
                place++;
            }



            playerInfo ActivePlayerInfo = new playerInfo
            {
                UserName = data.playerName ,
            };

            var targetPlayerItem = special_item = LeaderboardEntries [index];
            oldRank_ = LeaderboardEntries [index].PositionCounter_.CurrentPosition;
            targetPlayerItem.SetUpPlayers(ActivePlayerInfo.UserName , LeaderboardEntries [index].PositionCounter_.CurrentPosition , newScore);
            HighlightPlayer(targetPlayerItem);
            float TargetPos = GetNormalizedScrollPosition(targetPlayerItem.GetComponent<RectTransform>());
            StartCoroutine(ScrollToRank(TargetPos , 1f));
        }

        bool IsUp ( int oldRankPosition , int newRankPosition )
        {
            return newRankPosition <= oldRankPosition;
        }

        private Sprite GetRankChangeSprite ( bool isUp )
        {
            return isUp ? RankUpSprite : RankDownSprite;
        }

        private Color GetRankChangeColor ( bool isUp )
        {
            return isUp ? RankUpColor : RankDownColor;
        }

        private int DetermineIndex ( int newRankPosition )
        {
            return newRankPosition < 3 ? newRankPosition - 1 : 2;
        }

        private void HighlightPlayer ( ItemData itemData )
        {
            const string Higlight_color = "#FFAF00";
            for (int i = 0 ; i < LeaderboardEntries.Count ; i++)
            {
                if (LeaderboardEntries [i] == itemData)
                {
                    for (int j = 0 ; j < itemData.EditableImages.Length ; j++)
                    {
                        if (ColorUtility.TryParseHtmlString(Higlight_color , out Color color_))
                        {
                            itemData.EditableImages [j].color = color_;
                        }
                    }
                }
                else
                {
                    LeaderboardEntries [i].ResetColors();
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

        int GetRandomOldRank ()
        {
            return Random.Range(1 , 100);
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

        public IEnumerator ScrollToRank ( float targetPos , float Duration , Action OnComplete = null )
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

        float GetNormalizedScrollPosition ( RectTransform target )
        {
            // Get the target position relative to the scroll content
            Vector2 targetPosition = (Vector2)scrollRect.content.InverseTransformPoint(target.position);

            // Get the content size
            RectTransform contentRect = scrollRect.content;

            // Calculate the normalized Y position
            float normalizedY = ( targetPosition.y - contentRect.rect.yMin ) / contentRect.rect.height;

            if (normalizedY <= 0.2)
            {
                normalizedY = 0f;
            }
            else if (normalizedY >= 0.4f && normalizedY <= 0.6)
            {
                normalizedY = 0.5f;
            }
            else if (normalizedY >= 0.8f)
            {
                normalizedY = 1f;
            }
            // Return the clamped normalized Y value
            return Mathf.Clamp01(normalizedY);
        }

        public void UpdateTierAndRank ()
        {
            if (oldRank_ <= 1)
            {
                IncreaseTier();

            }
            else if(oldRank_ >=100) 
            {
                LowerTier();
            }

        }
    }
}
