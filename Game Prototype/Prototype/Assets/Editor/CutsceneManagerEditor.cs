using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(CutsceneManager))]
public class CutsceneManagerEditor : Editor
{
    private string[] actionTypeNames;
    private Type[] actionTypes;
    private int[] selectedActionTypes;

    void OnEnable()
    {
        // Get all types that inherit from CutsceneAction
        actionTypes = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(CutsceneAction)) && !type.IsAbstract)
            .ToArray();

        // Create display names
        actionTypeNames = actionTypes.Select(type => type.Name.Replace("Action", "")).ToArray();

        CutsceneManager manager = (CutsceneManager)target;
        selectedActionTypes = new int[manager.cutscenes.Count];
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CutsceneManager cutsceneManager = (CutsceneManager)target;

        // Draw References section
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        SerializedProperty dialogueManagerProp = serializedObject.FindProperty("dialogueManager");
        SerializedProperty dialogueControllerProp = serializedObject.FindProperty("dialogueController");
        SerializedProperty cameraControllerProp = serializedObject.FindProperty("cameraController");
        SerializedProperty playerControllerProp = serializedObject.FindProperty("playerController");
        SerializedProperty mainCameraProp = serializedObject.FindProperty("mainCamera");

        EditorGUILayout.PropertyField(dialogueManagerProp);
        EditorGUILayout.PropertyField(dialogueControllerProp);
        EditorGUILayout.PropertyField(cameraControllerProp);
        EditorGUILayout.PropertyField(playerControllerProp);
        EditorGUILayout.PropertyField(mainCameraProp);

        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        // Draw Debug section
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        SerializedProperty debugModeProp = serializedObject.FindProperty("debugMode");
        SerializedProperty testCutsceneIndexProp = serializedObject.FindProperty("testCutsceneIndex");

        EditorGUILayout.PropertyField(debugModeProp);
        if (debugModeProp.boolValue)
        {
            EditorGUILayout.PropertyField(testCutsceneIndexProp);
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        // Draw Cutscenes section
        EditorGUILayout.LabelField("Cutscenes", EditorStyles.boldLabel);

        SerializedProperty cutscenesProp = serializedObject.FindProperty("cutscenes");

        // Ensure selectedActionTypes array matches cutscenes count
        if (selectedActionTypes.Length != cutscenesProp.arraySize)
        {
            System.Array.Resize(ref selectedActionTypes, cutscenesProp.arraySize);
        }

        // Add/Remove cutscenes
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Size: {cutscenesProp.arraySize}");
        if (GUILayout.Button("+", GUILayout.Width(25)))
        {
            cutscenesProp.arraySize++;
            System.Array.Resize(ref selectedActionTypes, cutscenesProp.arraySize);
        }
        if (GUILayout.Button("-", GUILayout.Width(25)) && cutscenesProp.arraySize > 0)
        {
            cutscenesProp.arraySize--;
            System.Array.Resize(ref selectedActionTypes, cutscenesProp.arraySize);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Draw each cutscene
        for (int i = 0; i < cutscenesProp.arraySize; i++)
        {
            SerializedProperty cutsceneProp = cutscenesProp.GetArrayElementAtIndex(i);
            SerializedProperty actionsProp = cutsceneProp.FindPropertyRelative("actions");
            SerializedProperty cutsceneNameProp = cutsceneProp.FindPropertyRelative("cutsceneName");
            SerializedProperty disablePlayerMovementProp = cutsceneProp.FindPropertyRelative("disablePlayerMovement");
            SerializedProperty returnCameraToPlayerProp = cutsceneProp.FindPropertyRelative("returnCameraToPlayer");
            SerializedProperty onCutsceneStartProp = cutsceneProp.FindPropertyRelative("onCutsceneStart");
            SerializedProperty onCutsceneEndProp = cutsceneProp.FindPropertyRelative("onCutsceneEnd");

            EditorGUILayout.BeginVertical("box");

            // Cutscene header
            EditorGUILayout.BeginHorizontal();
            cutsceneProp.isExpanded = EditorGUILayout.Foldout(cutsceneProp.isExpanded, $"Cutscene {i}", true);
            if (GUILayout.Button("Remove Cutscene", GUILayout.Width(120)))
            {
                cutscenesProp.DeleteArrayElementAtIndex(i);
                System.Array.Resize(ref selectedActionTypes, cutscenesProp.arraySize);
                break;
            }
            EditorGUILayout.EndHorizontal();

            if (cutsceneProp.isExpanded)
            {
                EditorGUI.indentLevel++;

                // Cutscene properties
                EditorGUILayout.PropertyField(cutsceneNameProp);
                EditorGUILayout.PropertyField(disablePlayerMovementProp);
                EditorGUILayout.PropertyField(returnCameraToPlayerProp);
                EditorGUILayout.PropertyField(onCutsceneStartProp);
                EditorGUILayout.PropertyField(onCutsceneEndProp);

                EditorGUILayout.Space();

                // Actions section
                EditorGUILayout.LabelField($"Actions ({actionsProp.arraySize})", EditorStyles.boldLabel);

                // Display existing actions
                for (int j = 0; j < actionsProp.arraySize; j++)
                {
                    SerializedProperty actionProp = actionsProp.GetArrayElementAtIndex(j);

                    EditorGUILayout.BeginHorizontal();

                    if (actionProp.managedReferenceValue != null)
                    {
                        string actionName = actionProp.managedReferenceValue.GetType().Name;
                        actionProp.isExpanded = EditorGUILayout.Foldout(actionProp.isExpanded, $"{j}: {actionName}", true);
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"{j}: <null>");
                    }

                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        actionsProp.DeleteArrayElementAtIndex(j);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();

                    // Show action properties if expanded
                    if (actionProp.isExpanded && actionProp.managedReferenceValue != null)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(actionProp, GUIContent.none, true);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();
                    }
                }

                EditorGUILayout.Space();

                // Add new action controls
                EditorGUILayout.BeginHorizontal();
                selectedActionTypes[i] = EditorGUILayout.Popup("Add Action:", selectedActionTypes[i], actionTypeNames);

                if (GUILayout.Button("Add", GUILayout.Width(60)))
                {
                    // Create new action instance
                    Type selectedType = actionTypes[selectedActionTypes[i]];
                    CutsceneAction newAction = (CutsceneAction)Activator.CreateInstance(selectedType);

                    // Add to the actions list
                    actionsProp.arraySize++;
                    SerializedProperty newActionProp = actionsProp.GetArrayElementAtIndex(actionsProp.arraySize - 1);
                    newActionProp.managedReferenceValue = newAction;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
    }
}