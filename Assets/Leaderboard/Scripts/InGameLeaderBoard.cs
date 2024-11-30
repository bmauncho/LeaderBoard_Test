using LeaderBoard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;
using Random = UnityEngine.Random;

namespace Leaderboard
{
    public class InGameLeaderBoard : MonoBehaviour
    {
        LeaderBoardManager data;
        [Header("Leaderboard Content")]
        ItemData special_item;
        public GameObject LeaderBoard;
        public GameObject playerItem;
        public List<ItemData> LeaderboardEntries = new List<ItemData>();
        Dictionary<string , Dictionary<int , float>> playerDetails = new Dictionary<string ,Dictionary<int , float>>();

        // Start is called before the first frame update
        void Start ()
        {
            data = FindObjectOfType<LeaderBoardManager>(true);
            FetchPlayersInfo(() =>
            {
                UpdateThePlayersInfo();
            });
        }

        public void ArrangeLeaderBoard ()
        {
            if (LeaderboardEntries.Count <= 0)
            {
                Debug.LogError("LeaderBoard Entries List is EMPTY!");
                return;
            }

            float entryTemplateHeight = 38f;
            for (int i = 0 ; i < LeaderboardEntries.Count ; i++)
            {
                RectTransform entryTransform = LeaderboardEntries [i].GetComponent<RectTransform>();
                entryTransform.anchoredPosition = new Vector2(0 , -entryTemplateHeight * i);
            }
        }

        public playerInfo CreatePlayerInfo ()
        {
            return data.CreatePlayerInfo();
        }

        public void ShowLeaderBoard ()
        {
            LeaderBoard.SetActive(true);
            LeaderBoard.transform.localScale = Vector3.one;
        }

        public void HideLeaderBoard ()
        {
            LeaderBoard.SetActive(false);
            LeaderBoard.transform.localScale = Vector3.zero;
        }

        public IEnumerator InitializeLeaderBoard ()
        {
           
            yield return null;
        }

        private float GetPlayerCount ()
        {
            return 20;
        }

        private float GetPlayerScore ()
        {
            return Random.Range(5 , 100);
        }

        private void FetchPlayersInfo (Action Oncomplete = null)
        {
            // Clear playerDetails to avoid duplicates
            playerDetails.Clear();
            bool hasActivePlayer = false;
            for (int i = 0 ; i < GetPlayerCount() ; i++)
            {
                // Create a new rank-score dictionary for each player
                Dictionary<int , float> rankScore = new Dictionary<int , float>
                {
                    { 0, 0 } // Initialize with Rank 0 and Score 0
                };

                // Create a unique player info
                playerInfo newplayer = CreatePlayerInfo();


                if (!playerDetails.ContainsKey(newplayer.UserName))
                {
                    playerDetails.Add(newplayer.UserName , rankScore);
                    if (!hasActivePlayer)
                    {
                        hasActivePlayer = true;
                        playerInfo ActivePlayer = new playerInfo
                        {
                            UserName = data.playerName ,
                        };

                        if (!playerDetails.ContainsKey(ActivePlayer.UserName))
                        {
                            playerDetails.Add(ActivePlayer.UserName , rankScore);
                        }
                    }
                }
            }
            playerDetails = playerDetails.OrderBy(entry => entry.Key).ToDictionary(entry => entry.Key , entry => entry.Value);

            // Adding the rank
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

            //Optional:Debugging
            foreach (var entry in playerDetails)
            {
                int playerRank = entry.Value.Keys.First();
                float playerScore = entry.Value.Values.First();
                Debug.Log($"Name: {entry.Key}, Rank: {playerRank}, Score: {playerScore}");
            }

            Oncomplete?.Invoke();
        }

        public void UpdateThePlayersInfo ()
        {
            var playerKeys = playerDetails.Keys.ToList();
            var playerValues = playerDetails.Values.ToList();

            for (int i = 0 ; i < LeaderboardEntries.Count ; i++)
            {
                var innerDictionary = playerValues [i]; // Inner dictionary for this player

                // Assuming the first entry in the inner dictionary is rank and score
                int rank = innerDictionary.Keys.First();   // First key is rank
                float score = innerDictionary.Values.First(); // Corresponding value is score

                LeaderboardEntries [i].SetUpPlayers(playerKeys [i] , rank , score);

                var itemData = playerItem.GetComponentInChildren<ItemData>();
                if (itemData != null)
                {
                    if (playerDetails.TryGetValue(data.playerName , out var playerData))
                    {
                        int playerRank = playerData.Keys.First();
                        float playerScore = playerData.Values.First();
                        itemData.SetUpPlayers(data.playerName , playerRank , playerScore);
                    }
                }
            }
        }

        public void UpdateTheLeaderBoard ()
        {
            var playerKeys = playerDetails.Keys.ToList();

            for(int i = 0 ;i< playerKeys.Count ;i++)
            {
                float randscore = GetPlayerScore();
                UpdatePlayersScore (playerKeys [i], randscore);
            }

        }

        public void UpdatePlayersScore ( string whichPlayer , float newScore)
        {
            if (playerDetails.ContainsKey(whichPlayer))
            {
                var innerDictionary = playerDetails [whichPlayer];
                int rank=innerDictionary.Keys.First();
                playerDetails [whichPlayer] = new Dictionary<int , float>
                {
                    {rank,newScore }
                };

                UpdatePlayerRanks();
            }
        }

        public void UpdatePlayerRanks ()
        {
            // Sort players by score in descending order
            playerDetails = playerDetails
                .OrderByDescending(entry => entry.Value.Values.First()) // Sort by score
                .ThenBy(entry => entry.Key) // Secondary sort by name for ties
                .ToDictionary(entry => entry.Key , entry => entry.Value);

            // Reassign ranks after sorting
            int rank = 1;
            foreach (var key in playerDetails.Keys.ToList())
            {
                var score = playerDetails [key].Values.First();
                playerDetails [key] = new Dictionary<int , float> { { rank , score } };
                rank++;
            }

            // Update the leaderboard UI
            for (int i = 0 ; i < LeaderboardEntries.Count ; i++)
            {
                var playerName = playerDetails.ElementAt(i).Key;
                var rankScore = playerDetails [playerName];
                int updatedRank = rankScore.Keys.First();
                float updatedScore = rankScore.Values.First();

                LeaderboardEntries [i].SetUpPlayers(playerName , updatedRank , updatedScore);
            }

            // Update the player item for the active player
            var itemData = playerItem.GetComponentInChildren<ItemData>();
            if (itemData != null && playerDetails.TryGetValue(data.playerName , out var activePlayerData))
            {
                int activePlayerRank = activePlayerData.Keys.First();
                float activePlayerScore = activePlayerData.Values.First();

                if (activePlayerRank <= 3)
                {
                    // Hide the player item if rank is 1, 2, or 3
                    playerItem.SetActive(false);
                }
                else
                {
                    // Show the player item and update its data
                    playerItem.SetActive(true);
                    itemData.SetUpPlayers(data.playerName , activePlayerRank , activePlayerScore);
                }
            }
            // Highlight the active player in the leaderboard
            HighlightPlayer(itemData);
        }

        private void HighlightPlayer ( ItemData activePlayerItem )
        {
            const string HighlightColor = "#FFAF00"; // Highlight color
            if (ColorUtility.TryParseHtmlString(HighlightColor , out Color highlightColor))
            {
                // Highlight the active player in the leaderboard

                for( int i = 0; i < LeaderboardEntries.Count; i++)
                {
                    if (i < 3)
                    {
                        if (LeaderboardEntries [i].UserName.text == data.playerName)
                        {
                            for( int j = 0 ; j < LeaderboardEntries [i].EditableImages.Length; j++)
                            {
                                Image highlightImage = LeaderboardEntries [i].EditableImages [j];
                                if (highlightImage != null) // Ensure no null references
                                {
                                    highlightImage.color = highlightColor;
                                }
                            }
                        }
                        else
                        {
                            LeaderboardEntries [i].ResetColors();
                        }
                    }
                }

                // Highlight the active player's item (if visible)
                if (activePlayerItem != null)
                {
                    foreach (var image in activePlayerItem.EditableImages)
                    {
                        if (image != null) // Ensure no null references
                        {
                            image.color = highlightColor;
                        }
                    }
                }
            }
        }



    }
}
