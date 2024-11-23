using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace CustomLeaderBoard
{
    [CustomEditor(typeof(LeaderboardManager))]
    public class LeaderBoardManagerEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Add a button
            LeaderboardManager LeaderboardManager_ = (LeaderboardManager)target;
            if (GUILayout.Button("Reset"))
            {
                LeaderboardManager_.ResetLeaderBoard();
            }

            if (GUILayout.Button("Show"))
            {
                LeaderboardManager_.Show();
            }


            if (GUILayout.Button("Hide"))
            {
                LeaderboardManager_.Hide();
            }
        }
    }
}