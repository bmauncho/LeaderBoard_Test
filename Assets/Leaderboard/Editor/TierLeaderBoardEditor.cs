using LeaderBoard;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace leaderboard
{
    [CustomEditor(typeof(TierLeaderBoard))]
    public class TierLeaderBoardEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI ();

            TierLeaderBoard tierLeaderBoard = (TierLeaderBoard)target;

            if (GUILayout.Button("Show"))
            {
                tierLeaderBoard.ShowLeaderBoard();
            }

            if (GUILayout.Button("Hide"))
            {
                tierLeaderBoard.HideLeaderBoard();
            }

            if (GUILayout.Button("RankUp"))
            {
                tierLeaderBoard.RankUp();
            }

            if (GUILayout.Button("RankDown"))
            {
                tierLeaderBoard.RankDown();
            }

            if (GUILayout.Button("Increase Tier"))
            {
                tierLeaderBoard.IncreaseTier();
            }

            if (GUILayout.Button("Decrease Tier"))
            {
                tierLeaderBoard.LowerTier();
            }

            if (GUILayout.Button("Arrange Leaderboard"))
            {
                tierLeaderBoard.ArrangeLeaderBoard();
            }
        }
    }
}
