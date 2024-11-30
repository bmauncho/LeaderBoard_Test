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

        [Header("LeaderBoard")]
        public TMP_Text LeaderBoardTitle;
        public GameObject TheLeaderBoard;
        public GameObject Banner;

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
            LeaderBoardTitle.text = "LEADERBOARD";
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
    }
}

