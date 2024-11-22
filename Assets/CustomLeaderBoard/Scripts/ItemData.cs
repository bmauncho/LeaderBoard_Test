using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomLeaderBoard
{
    public class ItemData : MonoBehaviour
    {
        [SerializeField] private Tier tier;
        // Item content container
        [SerializeField] private GameObject content;

        // Country icon
        [SerializeField] private Image icon;

        // Username text
        [SerializeField] private TMP_Text username;

        // Place counter
        [SerializeField] private PositionCounter placeCounter;

        // Sets color for country icon
        public void SetIconColor ( Color color ) => icon.color = color;

        // Hides content of item
        public void HideContent () => content.SetActive(false);

        // Shows content of item
        public void ShowContent () => content.SetActive(true);

        // Sets rank position
        private void SetRank ( int rank ) => placeCounter.SetCountQuiet(rank);

        public Image [] EditableColorsImages;

        // Sets player info
        private void SetPlayerInfo ( PlayerInfo playerInfo )
        {
            icon.sprite = playerInfo.Country;
            username.text = playerInfo.Username;
        }

        private void SetTier ( Tier _tier )
        {
            tier = _tier;
        }

        // Initializes item
        public void Initialize ( PlayerInfo playerInfo , int rank,Tier tier )
        {
            SetPlayerInfo(playerInfo);
            SetRank(rank);
            SetTier(tier);
        }
    }
}
