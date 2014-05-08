using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GazeMapData))]
public class GazeMapDataEditor : Editor
{

	private int index = 0;

	override public void OnInspectorGUI()
	{
		GazeMapData gatherer = target as GazeMapData;

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
				Debug.Log(e);
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		index = EditorGUILayout.Popup(index, gatherer.Filenames.ToArray());
		if(gatherer.Filenames.Count > 0 && !gatherer.FilenamesToShow.Contains(gatherer.Filenames.ToArray()[index]))
		{
			gatherer.ShowGazeData(gatherer.Filenames.ToArray()[index]);
			SceneView.RepaintAll();
		}
		if(GUILayout.Button("Delete data file"))
		{
			try
			{
				gatherer.DeleteSaveFile(gatherer.Filenames.ToArray()[index]);
				index = index > 0 ? index - 1 : 0;
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

		EditorGUILayout.MinMaxSlider(new GUIContent("Time interval: "), ref gatherer.minGazeDataIndex, ref gatherer.maxGazeDataIndex, 0f, 1f);
		SceneView.RepaintAll();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("lower and upper values: " + gatherer.minGazeDataIndex.ToString(), EditorStyles.label);
		EditorGUILayout.LabelField(gatherer.maxGazeDataIndex.ToString(), EditorStyles.label);
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
		EditorGUILayout.PropertyField(serializedObject.FindProperty("filenamesToShow"), true);
		serializedObject.ApplyModifiedProperties();
	}

}
