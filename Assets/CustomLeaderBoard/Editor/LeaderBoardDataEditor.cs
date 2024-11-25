using UnityEditor;
using UnityEngine;

namespace CustomLeaderBoard
{
    [CustomEditor(typeof(LeaderBoardData))]
    public class LeaderBoardDataEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            // Get the target object
            LeaderBoardData leaderboardData = (LeaderBoardData)target;

            // Draw default fields in the Inspector
            serializedObject.Update();

            // Draw properties except tierThresholds and ManuallySetTierColor
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.name != "tierThresholds" && iterator.name != "ManuallySetTierColor")
                {
                    EditorGUILayout.PropertyField(iterator , true);
                }
                enterChildren = false;
            }

            // Conditionally show the tierThresholds list and "ManuallySetTierColor"
            if (leaderboardData.UseTierLeaderboared)
            {
                // Show ManuallySetTierColor when UseTierLeaderboared is true
                SerializedProperty manuallySetTierColorProp = serializedObject.FindProperty("ManuallySetTierColor");
                EditorGUILayout.PropertyField(manuallySetTierColorProp);
                // Show tierThresholds and ManuallySetTierColor if UseTierLeaderboared is true
                SerializedProperty tierThresholdsProp = serializedObject.FindProperty("tierThresholds");
                EditorGUILayout.PropertyField(tierThresholdsProp , true);
                if (GUILayout.Button("Add Tier"))
                {
                    leaderboardData.InitializeTiers();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
