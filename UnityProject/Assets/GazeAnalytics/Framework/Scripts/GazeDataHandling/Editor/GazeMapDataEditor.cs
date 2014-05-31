using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GazeMapData))]
public class GazeMapDataEditor : Editor
{

	private int index = 0;
	private bool deleteAllFiles;
	private bool showWarningWindow;

	override public void OnInspectorGUI()
	{
		GazeMapData gazeMapData = target as GazeMapData;
		#region filehandling
		EditorGUILayout.LabelField("Gaze Data Editor", EditorStyles.label);

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Load all files"))
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
		if(GUILayout.Button("Delete all files"))
		{
			showWarningWindow = true;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		index = EditorGUILayout.Popup(index, gazeMapData.Filenames.ToArray());
		if(GUILayout.Button("Load"))
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
		if(GUILayout.Button("Delete"))
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
		if(GUILayout.Button("Process data", GUILayout.MaxWidth(100f)))
		{
			try
			{
				gazeMapData.ProcessGazeData();
			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
		if(GUILayout.Button("Clear data", GUILayout.MaxWidth(100f)))
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
		EditorGUILayout.EndHorizontal();
		#endregion

		#region gazemaprender
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingGazeEvents"), new GUIContent("Show Gaze Events"), true);
		SceneView.RepaintAll();
		serializedObject.ApplyModifiedProperties();
		//Prevent out of bounds exception due to slider inaccuracy:
		gazeMapData.minGazeDataIndex = (gazeMapData.minGazeDataIndex < 0f || gazeMapData.minGazeDataIndex > 1f) ? 0f : gazeMapData.minGazeDataIndex;
		gazeMapData.maxGazeDataIndex = (gazeMapData.maxGazeDataIndex < 0f || gazeMapData.maxGazeDataIndex > 1f) ? 1f : gazeMapData.maxGazeDataIndex;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(new GUIContent("Timeline: "), GUILayout.MaxWidth(80));
		EditorGUILayout.FloatField(gazeMapData.minGazeDataIndex, GUILayout.Width(30f));
		EditorGUILayout.MinMaxSlider(ref gazeMapData.minGazeDataIndex, ref gazeMapData.maxGazeDataIndex, 0f, 1f);
		EditorGUILayout.FloatField(gazeMapData.maxGazeDataIndex, GUILayout.Width(30f));
		EditorGUILayout.EndHorizontal();
		SceneView.RepaintAll();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingGazeRay"), new GUIContent("Show Gaze Rays"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingObjectName"), new GUIContent("Show Object Names"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingRayOrigin"), new GUIContent("Show Ray Origin"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingObjectSelectionBox"), new GUIContent("Show Object Selection Box"), true);
		serializedObject.ApplyModifiedProperties();
		#endregion

		#region pupilmaprender
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingPupilEvents"), new GUIContent("Show Pupil Dilation Events"), true);
		SceneView.RepaintAll();
		serializedObject.ApplyModifiedProperties();
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
		if(gazeMapData.IsShowingObjectSelectionBox)
		{
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
									}
									
								}
							}
						}
					}
				}
			}
		}
		int width = 200;
		int height = 50;
		Rect window = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);
		if(showWarningWindow)
		{
			GUI.Window(0, window, WarningWindow, "Are you sure?");
		}
		if(deleteAllFiles)
		{
			deleteAllFiles = false;
			gazeMapData.DeleteAllSaveFiles();
		}
	}

	void WarningWindow(int windowID)
	{
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Yes"))
		{
			deleteAllFiles = true;
			showWarningWindow = false;
		}
		if(GUILayout.Button("No"))
		{
			showWarningWindow = false;
		}
		GUILayout.EndHorizontal();
	}

}
