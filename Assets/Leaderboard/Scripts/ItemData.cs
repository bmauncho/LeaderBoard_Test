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
        public Image [] EditableImages;
        public Color [] EditableImagesOriginalColors;

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
        }

        public void SetPlayerInfo_Text(string playerOption )
        {
            UserName.text=playerOption;
        }

        public void InitializePlayer(playerInfo info,int rank,float score )
        {
            SetPlayerInfo( info );
            SetRank(rank);
            SetScore(score);
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
