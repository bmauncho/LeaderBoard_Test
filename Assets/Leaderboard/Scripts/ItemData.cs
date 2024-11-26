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
        public Image [] EditableColorsImages;
        public Color [] EditableImages;

        public void SetRank(int rank )
        {
            PositionCounter_.SetRank(rank);
        }

        public void SetScore(int score )
        {
            PositionCounter_.SetScore(score);
        }

        public void SetPlayerInfo(playerInfo playerInfo )
        {
            UserName.text = playerInfo.UserName;
        }

        public void InitializePlayer(playerInfo info,int rank,int score )
        {
            SetPlayerInfo( info );
            SetRank(rank);
            SetScore(score);
        }

        public void ResetColors ()
        {
            // Ensure the lengths match to avoid out-of-bounds errors
            for (int i = 0 ; i < EditableColorsImages.Length && i < EditableImages.Length ; i++)
            {
                EditableColorsImages [i].color = EditableImages [i]; // Apply the default color
            }
        }

        public void ResetItemData ()
        {
            SetRank(0);
            SetScore (0);
        }
    }
}
