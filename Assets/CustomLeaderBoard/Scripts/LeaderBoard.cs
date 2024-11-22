using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomLeaderBoard
{
    public class LeaderBoard : MonoBehaviour
    {
        [SerializeField] private LeaderBoardData data;
        [SerializeField] private TMP_Text title;
        [SerializeField] private PositionCounter playerRankCounter;
        [SerializeField] private ItemData ActivePlayerItem;
        [SerializeField] private GameObject playerItemContainer;
        [SerializeField] private ItemData [] allLeaders;
        [SerializeField] private Transform TheLeaderBoard;
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private RectTransform contentRect;

        public List<Color> SpecialPlayerColors = new List<Color>();
        public List<Color> NormalColors = new List<Color>();
        private bool specialcolorspicked = false;
        private ItemData special_item;

        public void Reset ()
        {
            gameObject.SetActive(true);
            TheLeaderBoard.localScale = Vector3.zero;
            scrollView.horizontalNormalizedPosition = 0.5f;
            scrollView.enabled = false;
            contentRect.anchoredPosition = Vector2.zero;
        }

        private void OnTierChange ( Tier oldTier , Tier newTier , int rank )
        {
            Debug.Log($"Player moved from {oldTier} to {newTier} at rank {rank}");

            ShowTierChangePopup(oldTier , newTier , rank);
        }

        private void ShowTierChangePopup ( Tier oldTier , Tier newTier , int rank )
        {
            Debug.Log($"Tier Change Popup: {oldTier} -> {newTier} (Rank: {rank})");
        }

        // Updates rank display logic
        private void UpdateRankDisplay ( int oldRankPosition , int newRankPosition )
        {
            // Determine if the rank is moving up or down
            bool isRankUp = newRankPosition < oldRankPosition;
            var rankChangeSprite = isRankUp ? data.RankUpSprite : data.RankDownSprite;
            var rankChangeColor = isRankUp ? data.RankUpColor : data.RankDownColor;

            // Get the updated player info
            var playerInfo = new PlayerInfo()
            {
                Username = data.PlayerName ,
                Country = rankChangeSprite // Use rank change sprite as a placeholder for visual feedback
            };

            // Determine the tier for the new rank
            var newTier = data.GetTierByRank(newRankPosition);
            var oldTier = data.GetTierByRank(oldRankPosition);

            // Highlight tier change if applicable
            if (newTier != oldTier)
            {
                Debug.Log($"Player tier changed from {oldTier} to {newTier}.");
                // Add optional tier change animation or effect
                ShowTierChangePopup(oldTier , newTier,newRankPosition);
            }

            // Update the active player's display
            ActivePlayerItem.Initialize(playerInfo , newRankPosition , newTier);
            ActivePlayerItem.SetIconColor(rankChangeColor);

            // Update the special item (e.g., highlighted leader)
            int leaderIndex = Mathf.Clamp(newRankPosition - 1 , 0 , allLeaders.Length - 1);
            var specialItem = allLeaders [leaderIndex];
            specialItem.Initialize(playerInfo , newRankPosition , newTier);
            specialItem.SetIconColor(rankChangeColor);

            // Update the rank counter display
            playerRankCounter.SetDuration(data.RankCounterAnimationDuration);
            playerRankCounter.SetCount(newRankPosition);
        }


        // Displays the popup
        public void Show ( int oldRankPosition , int newRankPosition , Action onComplete = null )
        {
            Reset();
            title.text = data.PopupTitle;

            UpdateRankDisplay(oldRankPosition , newRankPosition);

            if (data.PopupShowAnimationDuration <= 0f)
            {
                TheLeaderBoard.localScale = Vector3.one;
                playerRankCounter.SetCount(newRankPosition);
                scrollView.enabled = true;

                ScrollRank(newRankPosition <= oldRankPosition , () =>
                {
                    playerItemContainer.SetActive(false);
                    special_item.ShowContent();
                    AssignSpecialColours(special_item);
                    onComplete?.Invoke();
                });
                return;
            }

            TheLeaderBoard.localScale = Vector3.zero;
            scrollView.enabled = false;

            ResizePopup(TheLeaderBoard.localScale , Vector3.one , data.PopupShowAnimationDuration , () =>
            {
                scrollView.enabled = true;
                playerRankCounter.SetCount(newRankPosition);

                ScrollRank(newRankPosition <= oldRankPosition , () =>
                {
                    playerItemContainer.SetActive(false);
                    special_item.ShowContent();
                    AssignSpecialColours(special_item);
                    onComplete?.Invoke();
                });
            });
        }

        private void pick_special_colors ( ItemData picked , ItemData item )
        {
            if (!specialcolorspicked)
            {
                specialcolorspicked = true;

                foreach (Image pic_img in picked.EditableColorsImages)
                {
                    SpecialPlayerColors.Add(pic_img.color);
                }

                foreach (Image pic_img in item.EditableColorsImages)
                {
                    NormalColors.Add(pic_img.color);
                }
            }
        }

        private void AssignSpecialColours ( ItemData Item_ )
        {
            for (int i = 0 ; i < allLeaders.Length ; i++)
            {
                for (int p = 0 ; p < allLeaders [i].EditableColorsImages.Length ; p++)
                {
                    allLeaders [i].EditableColorsImages [p].color = NormalColors [p];
                }
            }
            for (int p = 0 ; p < special_item.EditableColorsImages.Length ; p++)
            {
                Item_.EditableColorsImages [p].color = SpecialPlayerColors [p];
            }
        }

        public void Hide ( Action onComplete = null )
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (data.PopupHideAnimationDuration <= 0f)
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
                return;
            }

            ResizePopup(TheLeaderBoard.localScale , Vector3.zero , data.PopupHideAnimationDuration , () =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        private void ScrollRank ( bool isUp , Action onComplete )
        {
            var scrollTo = isUp ? 1f : 0f;

            Tweens.Value(this , 0.5f , scrollTo , v =>
            {
                scrollView.verticalNormalizedPosition = v;
            } , data.RankCounterAnimationDuration , 0f , onComplete);
        }

        private void ResizePopup ( Vector3 original , Vector3 target , float duration , Action onComplete )
        {
            Tweens.Value(this , 0f , 1f , v => { TheLeaderBoard.localScale = Vector3.Lerp(original , target , v); } ,
                duration , 0f , onComplete);
        }
    }
}
