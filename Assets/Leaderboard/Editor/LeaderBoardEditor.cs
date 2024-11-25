using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LeaderBoard
{
    [CustomEditor(typeof(LeaderBoard))]
    public class LeaderBoardEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            LeaderBoard leaderBoard = (LeaderBoard)target;
            LeaderBoardManager leaderBoardManager = FindObjectOfType<LeaderBoardManager>(true); // Get LeaderBoardManager if it exists

            serializedObject.Update(); // Update the serialized object

            // Conditionally display ActiveTier at the top if CanUseTiers is true
            if (leaderBoardManager != null && leaderBoardManager.CanUseTiers)
            {
                SerializedProperty activeTier = serializedObject.FindProperty("ActiveTier");
                EditorGUILayout.PropertyField(activeTier);
            }

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            // Iterate over all other properties except "ActiveTier"
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.name != "ActiveTier")
                {
                    EditorGUILayout.PropertyField(iterator , true);
                }
                enterChildren = false;
            }

            // Add custom buttons
            if (GUILayout.Button("Show LeaderBoard"))
            {
                leaderBoard.ShowLeaderBoard();
            }

            if (GUILayout.Button("Hide LeaderBoard"))
            {
                leaderBoard.HideLeaderBoard();
            }

            if (leaderBoardManager != null && leaderBoardManager.CanUseTiers)
            {
                if (GUILayout.Button("Increase Tier"))
                {
                    leaderBoard.IncreaseTier();
                }

                if (GUILayout.Button("Lower Tier"))
                {
                    leaderBoard.LowerTier();
                }
            }
           

            serializedObject.ApplyModifiedProperties(); // Apply changes to the serialized object
        }
    }
}
