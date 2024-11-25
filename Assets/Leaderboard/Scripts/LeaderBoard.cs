using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LeaderBoard
{
    public class LeaderBoard : MonoBehaviour
    {
        LeaderBoardManager data;
        [SerializeField]public Tiers ActiveTier;
        public TMP_Text LeaderBoardTitle;
        public GameObject TheLeaderBoard;

        // Start is called before the first frame update
        void Start ()
        {
            data = FindObjectOfType<LeaderBoardManager>(true);
        }

        // Update is called once per frame
        void Update ()
        {

        }

        public void ResetLeaderBoard ()
        {
            HideLeaderBoard();
        }

        public void SetLeaderBoardTitle ()
        {
            LeaderBoardTitle.text = "WORLD RANK";
        }

        public void ShowLeaderBoard ()
        {
            TheLeaderBoard.SetActive(true);
            TheLeaderBoard.transform.localScale = Vector3.one;
        }

        public void HideLeaderBoard ()
        {
            TheLeaderBoard.SetActive(false);
            TheLeaderBoard.transform.localScale = Vector3.zero;
        }

    }
}

