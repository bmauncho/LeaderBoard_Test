using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LeaderBoard
{
    public class ItemData : MonoBehaviour
    {
        public GameObject Content;
        [SerializeField] private Image icon;
        public TMP_Text UserName;
        public PositionCounter PositionCounter_;
        public GameObject ranking;
        public Image [] EditableImages;
        public Color [] EditableImagesOriginalColors;
        public bool IsUsingIcon = false;

        private void Update ()
        {
            if (IsUsingIcon)
            {
                if (ranking)
                {
                    ranking.SetActive(true);
                }
            }
            else
            {
                if (ranking)
                {
                    ranking.SetActive(false);
                }
            }
        }

        public void SetIconColor (Color color)
        {
            icon.color = color;
        }

        public void SetIcon (Sprite sprite)
        {
            icon.sprite = sprite;
        }

        public void SetRank(int rank )
        {
            PositionCounter_.SetRank(rank);
        }

        public void SetScore(float score )
        {
            PositionCounter_.SetScore(score);
        }

        public void SetPlayerInfo(playerInfo playerInfo )
        {
            UserName.text = playerInfo.UserName;
            if (icon != null)
            {
                icon.sprite = playerInfo.Country;
            }
        }

        public void SetPlayerInfo_Text(string playerOption )
        {
            UserName.text=playerOption;
        }
        /// <summary>
        /// initialize fake players  with score
        /// </summary>
        /// <param name="info"></param>
        /// <param name="rank"></param>
        /// <param name="score"></param>
        public void InitializePlayer(playerInfo info,int rank,float score )
        {
            SetPlayerInfo( info );
            SetRank(rank);
            SetScore(score);
        }

        /// <summary>
        /// Intitialize fake players with out score
        /// </summary>
        /// <param name="info"></param>
        /// <param name="rank"></param>
        public void InitializePlayers ( playerInfo info , int rank )
        {
            SetPlayerInfo(info);
            SetRank(rank);

        }
        public void SetUpPlayers(string playerName,int rank,float score)
        {
            SetPlayerInfo_Text(playerName);
            SetRank(rank);
            SetScore(score);
        }

        public void ResetColors ()
        {
            // Ensure the lengths match to avoid out-of-bounds errors
            for (int i = 0 ; i < EditableImages.Length && i < EditableImagesOriginalColors.Length ; i++)
            {
                EditableImages [i].color = EditableImagesOriginalColors [i]; // Apply the default color
            }
        }

        public void ResetItemData ()
        {
            SetRank(0);
            SetScore (0);
        }

        public void HideContent ()
        {
            Content.SetActive (false);
        }

        public void ShowContent ()
        {
            Content.SetActive(true);
        }
    }
}
