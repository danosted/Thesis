using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GazeMapData))]
public class GazeMapDataEditor : Editor
{

	private static GUIStyle largeHeaderStyle = new GUIStyle();
	private static GUIStyle mediumHeaderStyle = new GUIStyle();
	private static GUIStyle smallFontStyle = new GUIStyle();
	private static GUIStyle boldFontStyle = new GUIStyle();

	private int index = 0;
	private bool deleteAllFiles;
	private bool showWarningWindow;

	static GazeMapDataEditor()
	{
		largeHeaderStyle.fontSize = 14;
		largeHeaderStyle.fontStyle = FontStyle.Bold;
		mediumHeaderStyle.fontSize = 11;
		mediumHeaderStyle.fontStyle = FontStyle.Bold;
//		smallFontStyle.fontSize = 8;
//		boldFontStyle.fontSize = 8;
		boldFontStyle.fontStyle = FontStyle.Bold;
	}

	override public void OnInspectorGUI()
	{
		GazeMapData gazeMapData = target as GazeMapData;
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		#region filehandling
		EditorGUILayout.LabelField("Data File Handling", largeHeaderStyle);
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Find save files", GUILayout.MaxWidth(120f)))
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
		if(!showWarningWindow)
		{
			if(GUILayout.Button("Delete save files", GUILayout.MaxWidth(120f)))
			{
				showWarningWindow = true;
			}
		}
		else
		{
			if(GUILayout.Button("Yes", GUILayout.MaxWidth(60f)))
			{
				gazeMapData.DeleteAllSaveFiles();
				showWarningWindow = false;
			}
			if(GUILayout.Button("No", GUILayout.MaxWidth(60f)))
			{
				showWarningWindow = false;
			}
		}

		if(GUILayout.Button("Reset data", GUILayout.MaxWidth(120f)))
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

		#region fileVisualization
		if(gazeMapData.Filenames.Count > 0)
		{
			EditorGUILayout.LabelField("Data files on disk", largeHeaderStyle);
		}
		foreach(string filename in gazeMapData.Filenames.ToArray())
		{
			bool isLoaded = gazeMapData.LoadedFiles.Contains(filename);
			EditorGUILayout.BeginHorizontal();
			if(!isLoaded)
			{
				EditorGUILayout.LabelField(new GUIContent(filename), smallFontStyle);
			}
			else
			{
				EditorGUILayout.LabelField(new GUIContent(filename), boldFontStyle);
			}
			string loadText = !isLoaded ? "Load" : "Unload";
			if(GUILayout.Button(loadText))
			{
				if(!isLoaded)
				{
					gazeMapData.LoadedFiles.Add(filename);
				}
				else
				{
					gazeMapData.LoadedFiles.Remove(filename);
				}
				SceneView.RepaintAll();
//				}

			}

			if(GUILayout.Button("Delete"))
			{
				gazeMapData.DeleteSaveFile(filename);
			}

			if(GUILayout.Button("Process", GUILayout.MaxWidth(100f)))
			{
				gazeMapData.CreateProcessedGazeDataFile(filename);
			}
			EditorGUILayout.EndHorizontal();
		}

		#endregion
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		#region gazeDataProcessing
		EditorGUILayout.LabelField("Gaze Data Processing Parameters", largeHeaderStyle);
		float sacJump = gazeMapData.maxSaccadeJumpDistance;
		gazeMapData.maxSaccadeJumpDistance = EditorGUILayout.Slider("Max Saccade Jump Dist (units)", sacJump, 0f, 2f); 
		float fix = gazeMapData.minFixationDuration;
		gazeMapData.minFixationDuration = EditorGUILayout.Slider("Min fixation duration (seconds)", fix, 0f, 2f); 

//		serializedObject.Update();
//		EditorGUILayout.PropertyField(serializedObject.FindProperty("fixationDistanceThreshold"), new GUIContent("Max Saccade Jump Length (units)"), true);
//		EditorGUILayout.PropertyField(serializedObject.FindProperty("fixationLengthThreshold"), new GUIContent("Min Fixation Duration Threshold (seconds)"), true);
//		serializedObject.ApplyModifiedProperties();
//		EditorGUILayout.BeginHorizontal();
//		if(GUILayout.Button("Process data", GUILayout.MaxWidth(100f)))
//		{
//			try
//			{
//				gazeMapData.ProcessGazeData();
//			}
//			catch(System.Exception e)
//			{
//				Debug.Log(e);
//			}
//		}
//		EditorGUILayout.EndHorizontal();
		#endregion
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		#region gazemaprender
		EditorGUILayout.LabelField("Gaze Visualization", largeHeaderStyle);
		//Prevent out of bounds exception due to slider inaccuracy:
		gazeMapData.minGazeDataIndex = (gazeMapData.minGazeDataIndex < 0f || gazeMapData.minGazeDataIndex > 1f) ? 0f : gazeMapData.minGazeDataIndex;
		gazeMapData.maxGazeDataIndex = (gazeMapData.maxGazeDataIndex < 0f || gazeMapData.maxGazeDataIndex > 1f) ? 1f : gazeMapData.maxGazeDataIndex;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(new GUIContent("Timeline (%)"), GUILayout.MaxWidth(80));
		EditorGUILayout.FloatField(gazeMapData.minGazeDataIndex, GUILayout.Width(30f));
		EditorGUILayout.MinMaxSlider(ref gazeMapData.minGazeDataIndex, ref gazeMapData.maxGazeDataIndex, 0f, 1f);
		EditorGUILayout.FloatField(gazeMapData.maxGazeDataIndex, GUILayout.Width(30f));
		EditorGUILayout.EndHorizontal();
		SceneView.RepaintAll();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingGazeEvents"), new GUIContent("Show Gaze Events"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingGazeRay"), new GUIContent("Show Gaze Rays"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingObjectName"), new GUIContent("Show Object Names"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingRayOrigin"), new GUIContent("Show Ray Origin"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingObjectSelectionBox"), new GUIContent("Show Object Selection Box"), true);
		serializedObject.ApplyModifiedProperties();
		SceneView.RepaintAll();
		#endregion
		EditorGUILayout.Space();
		EditorGUILayout.Space();
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
		serializedObject.ApplyModifiedProperties();
	}

	void OnSceneGUI()
	{
		
		GazeMapData gazeMapData = target as GazeMapData;
		if(gazeMapData.IsShowingObjectSelectionBox)
		{
			if(gazeMapData.IsShowingGazeEvents || gazeMapData.IsShowingPupilEvents || gazeMapData.IsShowingBlinkMap)
			{
				List<string> dataToCompare = gazeMapData.LoadedFiles;
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
//		if(showWarningWindow)
//		{
//			GUI.Window(0, window, WarningWindow, "Are you sure?");
//		}
//		if(deleteAllFiles)
//		{
//			deleteAllFiles = false;
//			gazeMapData.DeleteAllSaveFiles();
//		}
	}

//	void WarningWindow(int windowID)
//	{
//		GUILayout.BeginHorizontal();
//		if(GUILayout.Button("Yes"))
//		{
//			deleteAllFiles = true;
//			showWarningWindow = false;
//		}
//		if(GUILayout.Button("No"))
//		{
//			showWarningWindow = false;
//		}
//		GUILayout.EndHorizontal();
//	}

}
