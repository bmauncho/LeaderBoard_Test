using UnityEngine;
using UnityEditor;
using Leaderboard;

namespace LeaderBoard
{
    [CustomEditor(typeof(InGameLeaderBoard))]
    public class InGameLeaderBoardEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI(); // Draw default inspector elements

            InGameLeaderBoard inGameLeaderBoard = (InGameLeaderBoard)target;

            if(GUILayout.Button("Show"))
            {
                inGameLeaderBoard.ShowLeaderBoard();
            }

            if (GUILayout.Button("Hide"))
            {
                inGameLeaderBoard.HideLeaderBoard();
            }

            if (GUILayout.Button("Arrange Leaderboard"))
            {
                inGameLeaderBoard.ArrangeLeaderBoard();
            }

            if (GUILayout.Button("Update leaderboard realtime"))
            {
                inGameLeaderBoard.UpdateTheLeaderBoard();
            }
        }
    }
}
