using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace LeaderBoard
{
    [CustomEditor(typeof(LeaderBoardManager))]
    public class LeaderBoardManagerEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            LeaderBoardManager leaderboardMan = (LeaderBoardManager)target;

            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();

            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.name != "TierInfos" && iterator.name != "ManuallySetTierColor")
                {
                    EditorGUILayout.PropertyField(iterator , true);
                }
                enterChildren = false;
            }

            if (leaderboardMan.CanUseTiers)
            {
                SerializedProperty manuallySetTierColorProp = serializedObject.FindProperty("ManuallySetTierColor");

                EditorGUILayout.PropertyField(manuallySetTierColorProp);

                SerializedProperty tierThresholdsProp = serializedObject.FindProperty("TierInfos");

                EditorGUILayout.PropertyField(tierThresholdsProp , true);

                if (GUILayout.Button("Add Tier"))
                {
                    leaderboardMan.InitializeTiers();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
