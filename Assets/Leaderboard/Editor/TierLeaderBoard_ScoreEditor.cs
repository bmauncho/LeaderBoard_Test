using Leaderboard;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace LeaderBoard
{
    [CustomEditor(typeof(TierLeaderBoard_Score))]
    public class TierLeaderBoard_ScoreEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI();

            TierLeaderBoard_Score tierLeaderBoard_Score = (TierLeaderBoard_Score)target;

            if (GUILayout.Button("Show"))
            {
                tierLeaderBoard_Score.ShowLeaderBoard();
            }

            if (GUILayout.Button("Hide"))
            {
                tierLeaderBoard_Score.HideLeaderBoard();
            }

            if (GUILayout.Button("Rank Up"))
            {
                tierLeaderBoard_Score.RankUp();
            }

            if (GUILayout.Button("Rank Down"))
            {
                tierLeaderBoard_Score.RankDown();
            }

            if (GUILayout.Button("Increase Tier"))
            {
                tierLeaderBoard_Score.IncreaseTier();
            }

            if (GUILayout.Button("Lower Tier"))
            {
                tierLeaderBoard_Score.LowerTier();
            }

            if (GUILayout.Button("Arrange Leaderboard"))
            {
                tierLeaderBoard_Score.ArrangeLeaderBoard();
            }
        }
    }
}
