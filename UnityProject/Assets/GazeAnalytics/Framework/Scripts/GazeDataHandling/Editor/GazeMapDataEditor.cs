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

	private float scale;
	private float speed;
	private int index = 0;
	private bool deleteAllFiles;
	private bool showWarningWindow;
	private bool playAnimation;
	private bool isPressedPlay;

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
		string[] filenames = new string[gazeMapData.Filenames.Count];
		gazeMapData.Filenames.CopyTo(filenames);
		foreach(string filename in filenames)
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
					gazeMapData.LoadFile(filename);
				}
				else
				{
					gazeMapData.UnloadFile(filename);
				}
				SceneView.RepaintAll();
			}
			if(GUILayout.Button("Process", GUILayout.MaxWidth(100f)))
			{
				gazeMapData.CreateProcessedGazeDataFile(filename);
			}
			if(GUILayout.Button("Serialize", GUILayout.MaxWidth(100f)))
			{
				gazeMapData.CreateSaveFile(filename);
			}
			if(GUILayout.Button("Delete"))
			{
				gazeMapData.DeleteSaveFile(filename);
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
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingTargetPrefab"), new GUIContent("Show Target Object"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("isShowingPupilEvents"), new GUIContent("Show Pupil Dilation Events"), true);
		EditorGUILayout.MinMaxSlider(new GUIContent("Pupil Dilation Threshold"), ref gazeMapData.minPupilSize, ref gazeMapData.maxPupilSize, 0f, 40f);
		serializedObject.ApplyModifiedProperties();
		SceneView.RepaintAll();
		#endregion
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		#region animation
		EditorGUILayout.LabelField("Animate", largeHeaderStyle);
		float tempSpeed = speed;
		speed = EditorGUILayout.Slider("Animation speed", tempSpeed, 0f, 1f); 
		if(GUILayout.Button("Play", GUILayout.MaxWidth(100f)))
		{
			PlayAnimation();
		}
		if(GUILayout.Button("Stop", GUILayout.MaxWidth(100f)))
		{
			StopAnimation();
		}
		SceneView.RepaintAll();
		#endregion
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		#region pupilmaprender
		EditorGUILayout.LabelField("Color Coding", largeHeaderStyle);
		serializedObject.ApplyModifiedProperties();
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
							for(i = (int)(gazeMapData.minGazeDataIndex*(gazeArray.Length-1)); i < (int)(gazeMapData.maxGazeDataIndex*(gazeArray.Length)); i++)
							{
								GazeEvent e = gazeArray[i];
								if(e.filePath != "")
								{
									Camera cam = Camera.current;
									//			Quaternion.LookRotation(cam.transform.position - e.eventHitPoint)
									if(Handles.Button(e.eventHitPoint, Quaternion.LookRotation(cam.transform.position - e.eventHitPoint), 0.5f, 0.5f, Handles.RectangleCap))
									{
										GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(e.filePath, typeof(GameObject));
										if(go != null)
										{
											Selection.activeGameObject = go;
										}
									}
									
								}
							}
						}
					}
				}
			}
		}

		float realSpeed = speed * 0.0001f;
		if(isPressedPlay)
		{
			isPressedPlay = false;
			scale = gazeMapData.maxGazeDataIndex - gazeMapData.minGazeDataIndex;
			gazeMapData.minGazeDataIndex = 0f;
			gazeMapData.maxGazeDataIndex = 0f;
			playAnimation = true;
		}
		if(playAnimation && gazeMapData.maxGazeDataIndex + realSpeed <= 1f)
		{
			if(gazeMapData.maxGazeDataIndex - gazeMapData.minGazeDataIndex < scale)
			{
				gazeMapData.maxGazeDataIndex += realSpeed;
			}
			else
			{
				gazeMapData.minGazeDataIndex += realSpeed;
				gazeMapData.maxGazeDataIndex += realSpeed;
			}
			SceneView.RepaintAll();
		}
	}

	public void StopAnimation()
	{
		playAnimation = false;
	}
	
	public void PlayAnimation()
	{
		isPressedPlay = true;
	}


}
