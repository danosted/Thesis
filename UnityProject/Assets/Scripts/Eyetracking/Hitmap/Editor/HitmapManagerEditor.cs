using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(HitmapManager))]
public class HitmapManagerEditor : Editor
{
	override public void OnInspectorGUI()
	{
		HitmapManager gatherer = target as HitmapManager;
//		Handles.DrawCamera();
		EditorGUILayout.LabelField("Hitmap of Gaze and target Information", EditorStyles.largeLabel);
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Load saved data"))
		{
			try
			{
				gatherer.LoadData();

			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		if(GUILayout.Button("Show Hitmap"))
		{
			try
			{
				gatherer.ToggleHitmap();
				SceneView.RepaintAll();
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		if(GUILayout.Button("Show Pupil Dilation Heatmap"))
		{
			try
			{
				gatherer.TogglePupilMap();
				SceneView.RepaintAll();
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.MinMaxSlider(ref gatherer.minPupilSize,ref gatherer.maxPupilSize,0f,40f);
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("pupilColor"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("gazeTargetData"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("eyeData"), true);
		serializedObject.ApplyModifiedProperties();
	}

}
