using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace CustomLeaderBoard
{
    public class LeaderBoard : MonoBehaviour
    {
        [SerializeField] private LeaderBoardData data;
        // Title of LeaderBoard
        [SerializeField] private TMP_Text title;
        // Player counter animator
        [SerializeField] private PositionCounter playerRankCounter;
        // Player item in front of leaderboard
        [SerializeField] private ItemData playerItem;
        // Player item container
        [SerializeField] private GameObject playerItemContainer;
        // All leaders
        [SerializeField] private ItemData [] allLeaders;
        // LeaderBord transform
        [SerializeField] private Transform leaderboard_;
        // Scroll view
        [SerializeField] private ScrollRect scrollView;
        // Content rect transform
        [SerializeField] private RectTransform contentRect;

        public List<Color> SpecialPlayerColors = new List<Color>();
        public List<Color> NormalColors = new List<Color>();
        bool specialcolorspicked = false;
        ItemData special_item;

        private void OnEnable ()
        {
            ResetLeaderboardstate();
        }
        // Resets Leaderboard state
        public void ResetLeaderboardstate ()
        {
            gameObject.SetActive(true);
            leaderboard_.localScale = Vector3.zero;
            scrollView.horizontalNormalizedPosition = 0.5f;
            scrollView.enabled = false;
            contentRect.anchoredPosition = Vector2.zero;
        }

        public void Refreshleaderboard ()
        {
            if(!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            scrollView.horizontalNormalizedPosition = 0.5f;
            scrollView.enabled = false;
            contentRect.anchoredPosition = Vector2.zero;
        }
        
        public void Show ( int oldRankPosition , int newRankPosition , Action onComplete = null )
        {
            var isUp = IsUp(oldRankPosition, newRankPosition);
            var rankChangeSprite = GetRankChangeSprite(isUp);
            var rankChangeColor = GetRankChangeColor(isUp);

            UpdateLeaderBoardTitle ();

            var playerInfo = CreatePlayerInfo(rankChangeSprite);

            var index = DetermineIndex(newRankPosition);
            if (!isUp)
            {
                index = allLeaders.Length - 3;
            }

            var place = newRankPosition - index;

            foreach (var leader in allLeaders)
            {
                leader.Initialize(LeaderboardManager.Instance.CreatePlayerInfo() , place);
                place++;
            }

            var targetPlayerItem = special_item = allLeaders [index];
            pick_special_colors(playerItem , allLeaders [0]);

            playerItemContainer.SetActive(true);
            playerItem.Initialize(playerInfo , oldRankPosition);
            playerItem.SetIconColor(rankChangeColor);

            targetPlayerItem.Initialize(playerInfo , newRankPosition);
            targetPlayerItem.SetIconColor(rankChangeColor);
            targetPlayerItem.HideContent();

            playerRankCounter.SetDuration(data.RankCounterAnimationDuration);

            if (data.PopupShowAnimationDuration <= 0f)
            {
                PopUpAnimation(newRankPosition,isUp,targetPlayerItem,onComplete);
                return;
            }

            if(gameObject.activeSelf && leaderboard_.localScale == Vector3.one)
            {
                PopUpAnimation(newRankPosition , isUp , targetPlayerItem , onComplete);
            }
            else
            {
                LeaderBoardAnimation(newRankPosition , isUp , targetPlayerItem , onComplete);
            }
            
        }

        private void UpdateLeaderBoardTitle ()
        {
            title.text = data.PopupTitle;
        }

        bool IsUp ( int oldRankPosition , int newRankPosition )
        {
            return newRankPosition <= oldRankPosition;
        }

        private Sprite GetRankChangeSprite( bool isUp )
        {
            return isUp ? data.RankUpSprite : data.RankDownSprite;
        }
        
        private Color GetRankChangeColor ( bool isUp )
        {
            return isUp ? data.RankUpColor : data.RankDownColor;
        }
       
        private PlayerInfo CreatePlayerInfo ( Sprite countryflag )
        {
            return new PlayerInfo
            {
                Username = data.PlayerName ,
                Country = countryflag
            };
        }

        private int DetermineIndex (int newRankPosition)
        {
           return newRankPosition < 3 ? newRankPosition - 1 : 2;
        }

        private void PopUpAnimation ( int newRankPosition,bool isUp,ItemData targetPlayerItem, Action onComplete = null )
        {
            leaderboard_.localScale = Vector3.one;
            playerRankCounter.SetCount(newRankPosition);
            scrollView.enabled = true;

            ScrollRank(isUp , () => {
                playerItemContainer.SetActive(false);
                targetPlayerItem.ShowContent();
                AssignSpecialColours(targetPlayerItem);
                onComplete?.Invoke();
            });
        }

        private void LeaderBoardAnimation (int newRankPosition,bool isUp,ItemData targetPlayerItem,Action onComplete=null)
        {
            leaderboard_.localScale = Vector3.zero;
            scrollView.enabled = false;

            ResizeLeaderBoard(leaderboard_.localScale , Vector3.one , data.PopupShowAnimationDuration , () =>
            {
                scrollView.enabled = true;
                playerRankCounter.SetCount(newRankPosition);

                ScrollRank(isUp , () =>
                {
                    playerItemContainer.SetActive(false);
                    targetPlayerItem.ShowContent();
                    AssignSpecialColours(targetPlayerItem);
                    onComplete?.Invoke();
                });
            });
        }

        bool checkIfShouldAnimateLeaderBoard ()
        {
            if (gameObject.activeSelf)
            {
                if (leaderboard_.localScale == Vector3.one)
                {
                    return true;
                }
            }
            return false;
        }

        void pick_special_colors ( ItemData picked , ItemData item )
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


        void AssignSpecialColours ( ItemData Item_ )
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

        // Hides popup
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

            ResizeLeaderBoard(leaderboard_.localScale , Vector3.zero , data.PopupHideAnimationDuration , () => {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        // Animates rank scrolling
        private void ScrollRank ( bool isUp , Action onComplete )
        {
            float scrollTo = isUp ? 1f : 0f;

            Tweens.Value(this , 0.5f , scrollTo , v => {
                scrollView.verticalNormalizedPosition = v;
            } , data.RankCounterAnimationDuration , 0f , onComplete);
        }

        // Animates popup scale
        private void ResizeLeaderBoard ( Vector3 original , Vector3 target , float duration , Action onComplete )
        {
            Tweens.Value(this , 0f , 1f , v => { leaderboard_.localScale = Vector3.Lerp(original , target , v); } ,
                duration , 0f , onComplete);
        }
    }
}
