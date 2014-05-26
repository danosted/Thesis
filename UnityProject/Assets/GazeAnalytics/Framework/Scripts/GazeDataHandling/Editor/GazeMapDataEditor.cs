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
		GazeMapData gazeMapData = target as GazeMapData;
		#region filehandling
		EditorGUILayout.LabelField("Gaze Data Editor", EditorStyles.label);

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Load files on disk"))
		{
			try
			{
				gazeMapData.LoadFilesOnDisk();
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		index = EditorGUILayout.Popup(index, gazeMapData.Filenames.ToArray());
		if(GUILayout.Button("Toggle Gaze Data File"))
		{
			if(gazeMapData.Filenames.Count > 0)
			{
				if(!gazeMapData.DataToCompare.Contains(gazeMapData.Filenames.ToArray()[index]))
				{
					gazeMapData.ShowGazeData(gazeMapData.Filenames.ToArray()[index]);
				}
				else
				{
					gazeMapData.HideGazeData(gazeMapData.Filenames.ToArray()[index]);
				}
				SceneView.RepaintAll();
			}
		}
		if(GUILayout.Button("Delete data file"))
		{
			try
			{
				if(gazeMapData.Filenames.Count > 0)
				{
					gazeMapData.DeleteSaveFile(gazeMapData.Filenames.ToArray()[index]);
					index = index > 0 ? index - 1 : 0;
				}
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
				gazeMapData.ClearLoadedData();
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
				gazeMapData.ClearLoadedData();
				gazeMapData.DeleteAllSaveFiles();
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
				gazeMapData.ToggleHitmap();
				SceneView.RepaintAll();
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		//Prevent out of bounds exception due to slider inaccuracy:
		gazeMapData.minGazeDataIndex = (gazeMapData.minGazeDataIndex < 0f || gazeMapData.minGazeDataIndex > 1f) ? 0f : gazeMapData.minGazeDataIndex;
		gazeMapData.maxGazeDataIndex = (gazeMapData.maxGazeDataIndex < 0f || gazeMapData.maxGazeDataIndex > 1f) ? 1f : gazeMapData.maxGazeDataIndex;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(new GUIContent("Time interval: "));
		EditorGUILayout.FloatField(gazeMapData.minGazeDataIndex, GUILayout.Width(30f));
		EditorGUILayout.MinMaxSlider(ref gazeMapData.minGazeDataIndex, ref gazeMapData.maxGazeDataIndex, 0f, 1f);
		EditorGUILayout.FloatField(gazeMapData.maxGazeDataIndex, GUILayout.Width(30f));
		EditorGUILayout.EndHorizontal();
		SceneView.RepaintAll();
//		EditorGUILayout.BeginHorizontal();
//		EditorGUILayout.LabelField("lower and upper values: ", EditorStyles.label);
//		EditorGUILayout.LabelField(gazeMapData.minGazeDataIndex.ToString() + "         " + gazeMapData.maxGazeDataIndex.ToString(), EditorStyles.label);
//		EditorGUILayout.EndHorizontal();
		#endregion

		#region pupilmaprender
		if(GUILayout.Button("Toggle Pupil Dilation Map"))
		{
			try
			{
				gazeMapData.TogglePupilMap();
				SceneView.RepaintAll();
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.MinMaxSlider(new GUIContent("Pupil Dilation Threshold"), ref gazeMapData.minPupilSize, ref gazeMapData.maxPupilSize, 0f, 40f);
		EditorGUILayout.EndHorizontal();

		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("pupilColor"), true);
		serializedObject.ApplyModifiedProperties();
		#endregion

		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("eventOriginColors"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("eventGazeRayColors"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("eventHitPointColors"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("dataToCompare"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("savedFilenames"), true);
		serializedObject.ApplyModifiedProperties();
	}

	void OnSceneGUI()
	{
		GazeMapData gazeMapData = target as GazeMapData;
		if(gazeMapData.IsShowingGazeEvents || gazeMapData.IsShowingPupilEvents || gazeMapData.IsShowingBlinkMap)
		{
			List<string> dataToCompare = gazeMapData.DataToCompare;
			for(int fileindex = 0; fileindex < dataToCompare.Count; fileindex++)
			{
				foreach(KeyValuePair<string, List<GazeEvent>> entry in gazeMapData.FilenameToGazeEvent)
				{
					//If filename matches what is selected to be shown in the inspector
					if(entry.Key == dataToCompare.ToArray()[fileindex])
					{
						GazeEvent[] gazeArray = entry.Value.ToArray();
						int i = 0;
						for(i = (int)(gazeMapData.minGazeDataIndex*(gazeArray.Length-1)); i < (int)(gazeMapData.maxGazeDataIndex*(gazeArray.Length-1)); i++)
						{
							GazeEvent e = gazeArray[i];
							if(e.filePath != "")
							{
								Camera cam = Camera.current;
								//			Quaternion.LookRotation(cam.transform.position - e.eventHitPoint)
								if(Handles.Button(e.eventHitPoint, Quaternion.LookRotation(cam.transform.position - e.eventHitPoint), 0.5f, 0.5f, Handles.RectangleCap))
								{
									GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(e.filePath, typeof(GameObject));
									Selection.activeGameObject = go;
//									Debug.Log("Object selected: " + e.eventHitName, go);

//									Debug.Log("Object selected: " + e.eventHitName);
//									GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(e.filePath, typeof(GameObject));
//									Debug.Log("Object selected: " + e.eventHitName, go);

								}
								
							}
						}
					}
				}
			}
		}
	}

}
