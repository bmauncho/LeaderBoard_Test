using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LeaderBoard
{
    public class PositionCounter : MonoBehaviour
    {
        [Header("Progress")]
        public int CurrentPosition;
        public TMP_Text PositionLabel;

        [Header("Score")]
        public int Score;
        public TMP_Text ScoreLabel;

        void Update ()
        {
            SetUpRankVisual(CurrentPosition);
        }

        public void SetRank ( int rank )
        {
            CurrentPosition = rank;
        }

        public void SetScore ( int score )
        {
            Score = score;
            ScoreLabel.text = Score.ToString();
        }

        private void SetUpRankVisual ( int rank )
        {
            switch (rank)
            {
                default:
                    PositionLabel.text = rank.ToString() + " TH";
                    break;
                case 1:
                    PositionLabel.text = rank.ToString() + " ST";
                    break;
                case 2:
                    PositionLabel.text = rank.ToString() + " ND";
                    break;
                case 3:
                    PositionLabel.text = rank.ToString() + " RD";
                    break;
            }
        }
    }
}