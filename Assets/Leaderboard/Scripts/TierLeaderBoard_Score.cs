using LeaderBoard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Leaderboard
{
    public class TierLeaderBoard_Score : MonoBehaviour
    {
        LeaderBoardManager data;
        [Header("Tier")]
        [SerializeField] public Tiers ActiveTier;
        public const int MinimumTierPlayersCount = 100;

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
        public List<ItemData> LeaderboardEntries = new List<ItemData>();
        Dictionary<string,Dictionary<int,float>> playerDetails = new Dictionary<string,Dictionary<int,float>>();
        int oldRank_ = 0;
        bool IsRankUp = false;
        public float prevScore = 0;
        bool initialized = false;

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

        public playerInfo CreatePlayerInfo ()
        {
            return data.CreatePlayerInfo();
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
        void SetUp ()
        {
            for (int i = 0 ; i < LeaderboardEntries.Count ; i++)
            {
                LeaderboardEntries [i].IsUsingIcon = true;
            }
            UpdateContentHeight(ContentContainer_,75f,0,0);
        }

        public void ShowLeaderBoard ()
        {
            TheLeaderBoard.SetActive(true);
            TheLeaderBoard.transform.localScale = Vector3.one;
            SetLeaderBoardTitle();
            SetUp();
            RefreshLeaderBoard();
            if (!initialized)
            {
                initialized = true;
                float storedScore = 0;
                if (ActiveTier > 0)
                {
                    storedScore = data.MaximumScoreByTier(ActiveTier - 1);
                }

                prevScore = storedScore;
            }
            UpdateTierUI(prevScore);
        }

        public void HideLeaderBoard ()
        {
            ResetLeaderBoard();
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

        public void RankUp ()
        {
            IsRankUp = true;
            ResetLeaderBoard();

            float newScore = prevScore + 40;
           // Debug.Log($"The previous score was : {prevScore}");
            CheckAndUpdateTier(newScore , () =>
            {
                UpdateTierUI(newScore);
            });
            
        }

        public void RankDown ()
        {
            IsRankUp = false;
            ResetLeaderBoard();
           // Debug.Log($"The previous score was : {prevScore}");
            float newScore = prevScore - 20;
            if(newScore<=0)
            {
                newScore = 0;
            }
            CheckAndUpdateTier(newScore , () =>
            {
                UpdateTierUI(newScore);
            });
        }

        public void RefreshLeaderBoard ()
        {
            SetUpTier(() =>
            {
                DisplayTierData();
            });
        }

        public playerInfo CreateUniquePlayerInfo ( HashSet<string> existingUsernames )
        {
            playerInfo newPlayer;
            do
            {
                newPlayer = CreatePlayerInfo(); // Generate a new player
            } while (existingUsernames.Contains(newPlayer.UserName)); // Retry if duplicate

            existingUsernames.Add(newPlayer.UserName); // Add to the set of existing usernames
            return newPlayer;
        }

        public void SetUpTier ( Action Oncomplete = null )
        {
            playerDetails.Clear();
            int PlayerCount = MinimumTierPlayersCount;

            // Use a HashSet to track existing usernames
            HashSet<string> existingUsernames = new HashSet<string>();

            for (int i = 0 ; i < PlayerCount ; i++)
            {
                Dictionary<int , float> rankScore = new Dictionary<int , float>
                {
                    { 0, 0 } // Initialize with Rank 0 and Score 0
                };
                // Create a unique player info
                playerInfo newPlayer = CreateUniquePlayerInfo(existingUsernames);
                playerDetails.Add(newPlayer.UserName , rankScore);
            }

            playerDetails = playerDetails.OrderBy(entry => entry.Key).ToDictionary(entry => entry.Key , entry => entry.Value);

            // Adding the rank
            int rank = 1;

            foreach (var key in playerDetails.Keys.ToList())
            {
                playerDetails [key] = new Dictionary<int , float>
                {
                    { rank, playerDetails[key].Values.First() }
                };
                rank++;
            }

            foreach (var entry in playerDetails)
            {
                int playerRank = entry.Value.Keys.First();
                float Score = entry.Value.Values.First();

               // Debug.Log($"Name: {entry.Key}, Rank: {playerRank}, Score: {Score}");
            }

            Oncomplete?.Invoke();
        }


        public void DisplayTierData ()
        {
            //update the player details
            var playerKeys = playerDetails.Keys.ToList();
            var playerValues = playerDetails.Values.ToList();

            //Give each random score with the player having the least score if it is the first time
            for (int i = 0; i < playerKeys.Count; i++)
            {
                var innerDictionary = playerValues [i]; // Inner dictionary for this player

                int currentRank = innerDictionary.Keys.First();   // First key is rank
                float score = innerDictionary.Values.First(); // Corresponding value is score

                float newScore = GetNewScore();

                playerDetails [playerKeys [i]] = new Dictionary<int , float>
                {
                    { currentRank, newScore },
                };
            }
            // Sorting players by their highest score in descending order
            playerDetails = playerDetails
                .OrderByDescending(player => player.Value.Values.Max())
                .ToDictionary(pair => pair.Key , pair => pair.Value);

            //Assign new ranks
            int rank = 1;

            // Create a temporary list of keys to avoid modifying the collection during iteration
            var keys = playerDetails.Keys.ToList();

            foreach (var key in keys)
            {
                playerDetails [key] = new Dictionary<int , float>
                {
                    { rank, playerDetails[key].Values.First() }
                };
                rank++;
            }

            // Output the sorted list: Debugging
            //foreach (var entry in playerDetails)
            //{
            //    int playerRank = entry.Value.Keys.First();
            //    float Score = entry.Value.Values.First();

            //    Debug.Log($"Name: {entry.Key}, Rank: {playerRank}, Score: {Score}");
            //}
        }

        public float GetNewScore ()
        {
            float newScore = 0;

            // Get all possible tiers as an array
            Array Tiers = System.Enum.GetValues(typeof(Tiers));
            int enumLength = Tiers.Length;

            // Check if the current tier has increased compared to the previous tier
            if (ActiveTier > 0)
            {
                newScore = Random.Range(data.MaximumScoreByTier(ActiveTier) , data.MaximumScoreByTier(ActiveTier - 1));
            }
            else
            {
                // If no tier lower than the current tier, assign a score within the first tier
                newScore = Random.Range(5 , data.MaximumScoreByTier(ActiveTier));
            }

            return newScore;
        }


        public void UpdateTierUI ( float playerScore )
        {
            //Update Ui
            bool isUp = IsRankUp;
            var rankChangeSprite = GetRankChangeSprite(isUp);
            var rankChangeColor = GetRankChangeColor(isUp);

            int index = 0;
            if (isUp)
            {
                index = 5;
            }
            else
            {
                index = 15;
            }

            if (index < 0 || index >= LeaderboardEntries.Count)
            {
                Debug.LogWarning($"Index out of range: {index}. Adjusting to valid range.");
                index = 10;
            }

            var newRankIndex = DetermineNewRank(playerScore);
            int placeIndex =0;

            if (newRankIndex >= playerDetails.Count-1 && ActiveTier == Tiers.ROOKIE)
            {
                Debug.Log(1);
                index = LeaderboardEntries.Count - 1;
                placeIndex = newRankIndex - ( LeaderboardEntries.Count - 1 );
                Debug.Log(1.1);
            }
            else if (newRankIndex <= 1 && ActiveTier == Tiers.IMMORTAL)
            {
                Debug.Log(2);
                index = 0;
                placeIndex = newRankIndex;
                Debug.Log(2.1);
            }
            else
            {
                Debug.Log(3);
                if (isUp)
                {
                    placeIndex = newRankIndex - 5;
                }
                else
                {
                    placeIndex = newRankIndex -15;
                }
                
                if( placeIndex <= 0 )
                {
                    if (!isUp)
                    {
                        placeIndex = 0;

                        index = newRankIndex;

                        //optional: precaution
                        if (index > LeaderboardEntries.Count - 1)
                        {
                            index = LeaderboardEntries.Count - 1;
                        }
                    }
                    else
                    {
                        placeIndex = 0;
                        index = 0;
                    }

                }
                Debug.Log(3.1);
            }


            Debug.Log($"The new rank Index is: {newRankIndex} + {1} = {newRankIndex+1}");
            Debug.Log($"The place index is: {placeIndex}");

            int DisplayIndex = 0;
            for( int i = 0; i < playerDetails.Count; i++ )
            {
                if (i == placeIndex && DisplayIndex < LeaderboardEntries.Count)
                {
                    if (placeIndex <= i)
                    {
                        var playerName = playerDetails.ElementAt(i).Key;
                        var rankScore = playerDetails [playerName];
                        float updatedScore = (float)Mathf.Floor(rankScore.Values.First());
                        LeaderboardEntries [DisplayIndex].SetUpPlayers(playerName , CreatePlayerInfo().Country , placeIndex + 1 , updatedScore);
                    }
                   
                    placeIndex++;
                    DisplayIndex++;
                    if (placeIndex > 100)
                    {
                        if (ActiveTier != Tiers.ROOKIE)
                        {
                            placeIndex = 0;
                        }
                    }
                }
            }


            if (DisplayIndex < LeaderboardEntries.Count)
            {
                int nextPosition = placeIndex + 1; // Start from the next position
                float previousTierScore = LeaderboardEntries [DisplayIndex].PositionCounter_.Score;

                for (int i = DisplayIndex ; i < LeaderboardEntries.Count ; i++)
                {
                    // Decrement score for each subsequent player
                    previousTierScore -= 1;
                    if (previousTierScore <= 0)
                    {
                        previousTierScore = 0; // Prevent negative scores
                    }

                    // Initialize the remaining leaderboard entries
                    LeaderboardEntries [i].InitializePlayer(CreatePlayerInfo() , nextPosition , previousTierScore);

                    nextPosition++; // Increment position for the next entry
                }
            }



            playerInfo ActivePlayerInfo = new playerInfo
            {
                UserName = data.playerName ,
            };
            var targetPlayerItem = LeaderboardEntries [index];
            if (targetPlayerItem != null)
            {
                oldRank_ = targetPlayerItem.PositionCounter_.CurrentPosition;
                targetPlayerItem.InitializePlayer(ActivePlayerInfo , oldRank_,playerScore);
                prevScore = playerScore;
                HighlightPlayer(targetPlayerItem);
                targetPlayerItem.SetIcon(rankChangeSprite);
                targetPlayerItem.SetIconColor(rankChangeColor);

                float targetPos = GetNormalizedScrollPosition(targetPlayerItem.GetComponent<RectTransform>());
                StartCoroutine(ScrollToRank(targetPos , 1f));
            }
        }

        public int DetermineNewRank (float playerscore)
        {
            int index = -1; // Start with an invalid index
            var playerKeys = playerDetails.Keys.ToList();
            var playerValues = playerDetails.Values.ToList();

            float lowestDifference = int.MaxValue;

            for (int i = 0;i< playerKeys.Count ; i++)
            {
                var innerDictionary = playerValues [i]; // Inner dictionary for this player

                int currentRank = innerDictionary.Keys.First();   // First key is rank
                float score = innerDictionary.Values.First();

                float diff = Math.Abs(score-playerscore);

                // Update index if this difference is smaller or if it's an exact match
                if (Mathf.Approximately(diff , lowestDifference) || diff < lowestDifference)
                {
                    lowestDifference = diff;
                    index = i;
                }

            }
            return Mathf.Max(index , 0);
        }

        private float GetPlayerScore ()
        {
            return Random.Range(5 , 100);
        }

        private Sprite GetRankChangeSprite ( bool isUp )
        {
            return isUp ? RankUpSprite : RankDownSprite;
        }

        private Color GetRankChangeColor ( bool isUp )
        {
            return isUp ? RankUpColor : RankDownColor;
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

        private void CheckAndUpdateTier ( float newScore , Action OnComplete = null )
        {
            if (newScore > data.MaximumScoreByTier(ActiveTier))
            {
                IncreaseTier();
                SetUpTier(() =>
                {
                    DisplayTierData(); // Ensures the new tier data is shown
                    OnComplete?.Invoke();
                });
            }
            else if (ActiveTier > Tiers.ROOKIE && newScore <= data.MaximumScoreByTier(ActiveTier - 1))
            {
                LowerTier();
                SetUpTier(() =>
                {
                    DisplayTierData(); // Ensures the new tier data is shown
                    OnComplete?.Invoke();
                });
            }
            else
            {
                OnComplete?.Invoke();
            }
        }
    }
}
