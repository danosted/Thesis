using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GazeMapManager))]
public class GazeMapManagerEditor : Editor
{

	private int index = 0;

	override public void OnInspectorGUI()
	{
		GazeMapManager gatherer = target as GazeMapManager;

		#region filehandling
		EditorGUILayout.LabelField("Gaze Data Editor", EditorStyles.label);
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Load files on disk"))
		{
			try
			{
				gatherer.LoadFilesOnDisk();
			}
			catch(System.Exception e)
			{
				Debug.Log("No saved files found." + e);
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		index = EditorGUILayout.Popup(index, gatherer.Filenames.ToArray());
		if(GUILayout.Button("Load data file"))
		{
			try
			{
				gatherer.LoadData(gatherer.Filenames.ToArray()[index]);
				SceneView.RepaintAll();
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		if(GUILayout.Button("Delete data file"))
		{
			try
			{
				gatherer.DeleteSaveFile(gatherer.Filenames.ToArray()[index]);
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Clear current hitmap data"))
		{
			try
			{
				gatherer.ClearLoadedData();
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		if(GUILayout.Button("Delete all saved data"))
		{
			try
			{
				gatherer.DeleteAllSaveFiles();
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		EditorGUILayout.EndHorizontal();
		#endregion

		#region gazemaprender
		if(GUILayout.Button("Render Gaze Map"))
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
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.MinMaxSlider(new GUIContent("Time interval"), ref gatherer.minGazeDataIndex, ref gatherer.maxGazeDataIndex, 0f, gatherer.GazeDataList.Count - 1f);
		SceneView.RepaintAll();
		EditorGUILayout.EndHorizontal();
		#endregion

		#region pupilmaprender
		if(GUILayout.Button("Toggle Pupil Dilation Map"))
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
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.MinMaxSlider(new GUIContent("Pupil Dilation Threshold"), ref gatherer.minPupilSize, ref gatherer.maxPupilSize, 0f, 40f);
		EditorGUILayout.EndHorizontal();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("pupilColor"), true);
		serializedObject.ApplyModifiedProperties();
		#endregion

		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("gazeDataList"), true);
		serializedObject.ApplyModifiedProperties();
	}

}
