using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode]
public class GazeMapData : MonoBehaviour
{
	private GazeMetricEvents data = GazeMetricEvents.Instance;

	[SerializeField]
	private List<Color>
		eventOriginColors = new List<Color>();
	[SerializeField]
	private List<Color>
		eventGazeRayColors = new List<Color>();
	[SerializeField]
	private List<Color>
		eventHitPointColors = new List<Color>();
	[SerializeField]
	private Color
		pupilColor = Color.yellow;

	[SerializeField]
	private bool
		isShowingGazeEvents;
	[SerializeField]
	private bool
		isShowingPupilEvents;
	[SerializeField]
	private bool
		isShowingBlinkMap;
	[SerializeField]
	private bool
		isShowingGazeRay;
	[SerializeField]
	private bool
		isShowingObjectName;
	[SerializeField]
	private bool
		isShowingRayOrigin;
	[SerializeField]
	private bool
		isShowingObjectSelectionBox;

	private Dictionary<string, List<GazeEvent>> filenameToGazeEvent = new Dictionary<string, List<GazeEvent>>();
	private List<GazeEvent> gazeDataList = new List<GazeEvent>();
	private List<string> savedFilenames = new List<string>();
	private List<string> loadedFiles = new List<string>();

	private float characterCubeSize = 0.5f;
	private float gazeRayHitSphereRadius = 0.5f;
	private float maxHeatMapPointSize = 2f;

	private bool isSaving;

	private int startIndex = 0;
	private int endIndex = 0;
	
	private GUIStyle style = new GUIStyle();

	#region publicEditorFields
	public float maxSaccadeJumpDistance = 0.1f;
	public float minFixationDuration = 0.5f;
	public float maxFixationSize = 0.1f;
	public float minFixationSize = 1f;
	public float maxPupilSize = 30f;
	public float minPupilSize = 20f;
	public float minGazeDataIndex = 0f;
	public float maxGazeDataIndex = 1f;
	#endregion

	void OnGUI()
	{
		int padding = 10;
		int width = 180;
		int btnHeight = 30;
		int y = padding;
		int x = Screen.width - (width + padding);

		if(GUI.Button(new Rect(x, y, 170, 20), "Save Session Data"))
		{
			CreateSaveFile();
		}
	}

#if UNITY_EDITOR
	//Render 3D GazeMap
	void OnDrawGizmosSelected()
	{
		if(isShowingGazeEvents || isShowingPupilEvents || isShowingBlinkMap)
		{
			for(int fileindex = 0; fileindex < loadedFiles.Count; fileindex++)
			{
				foreach(KeyValuePair<string, List<GazeEvent>> entry in filenameToGazeEvent)
				{
					//If filename matches what is selected to be shown in the inspector
					if(entry.Key == loadedFiles[fileindex])
					{
						GazeEvent[] gazeArray = entry.Value.ToArray();
						int i = 0;

						startIndex = (int)(minGazeDataIndex * (gazeArray.Length - 1));
						endIndex = (int)(maxGazeDataIndex * gazeArray.Length);
						for(i = startIndex; i < endIndex; i++)
						{
							GazeEvent e = gazeArray[i];
							if(isShowingGazeEvents)
							{
								Vector3 nextSaccadePoint = i < endIndex - 1 ? gazeArray[i + 1].eventHitPoint : e.eventHitPoint;
//								Vector3 nextSaccadePoint = gazeArray[i + 1].eventHitPoint;
								DrawGazeEvent(e, nextSaccadePoint, fileindex, i);
							}
							if(isShowingPupilEvents)
							{
								float pupilSize = gazeArray[i].pupilMeanSize;
								//TODO: Find the right min and max for pupil size
								float map = Mathf.InverseLerp(minPupilSize, maxPupilSize, pupilSize);
								Gizmos.color = new Color(pupilColor.r * map, pupilColor.g * map, pupilColor.b * map, 0.8f);
								Gizmos.DrawSphere(gazeArray[i].eventOrigin, maxHeatMapPointSize * (map + 0.1f));
							}
							if(isShowingBlinkMap)
							{
								
							}
						}
					}
				}
			}
		}
	}

	private void DrawGazeEvent(GazeEvent e, Vector3 nextSaccadePoint, int fileindex, int eventindex)
	{
		Vector3 camDir = (Camera.current.transform.position - e.eventHitPoint).normalized;
		//Gaze Ray
		if(isShowingGazeRay)
		{
			Gizmos.color = eventGazeRayColors.Count > fileindex ? eventGazeRayColors.ToArray()[fileindex] : Color.cyan;
			Gizmos.DrawLine(e.eventOrigin, e.eventHitPoint);
		}
		//Saccade Ray
		if(nextSaccadePoint != e.eventHitPoint)
		{
			Gizmos.color = eventHitPointColors.Count > fileindex ? eventHitPointColors.ToArray()[fileindex] : Color.yellow;
			Gizmos.DrawLine(e.eventHitPoint, nextSaccadePoint);
		}
		//HitPoint
		Handles.color = eventHitPointColors.Count > fileindex ? eventHitPointColors.ToArray()[fileindex] : Color.yellow;
		Handles.DrawSolidDisc(e.eventHitPoint, camDir, gazeRayHitSphereRadius + Mathf.Log(e.fixationLength, 10) * 0.4f);
		//Event origin color
		if(!isShowingPupilEvents && isShowingRayOrigin)
		{
			Gizmos.color = eventOriginColors.Count > fileindex ? eventOriginColors.ToArray()[fileindex] : Color.blue;
			Gizmos.DrawCube(e.eventOrigin, Vector3.one * characterCubeSize);
		}
		//index of event
		style.alignment = TextAnchor.MiddleCenter;
		style.fontStyle = FontStyle.Bold;
		style.fontSize = 10 + (int)(Mathf.Log(e.fixationLength, 10) * 0.1f);
		string eventIndexString = (eventindex + 1).ToString();
		Handles.color = Color.black;
		Handles.Label(e.eventHitPoint, eventIndexString, style);
		//Name of object that was hit
		if(isShowingObjectName)
		{
			Handles.Label(e.eventHitPoint + Vector3.up * 2f * gazeRayHitSphereRadius, e.eventHitName, style);
		}
		/*
		 * Note to self:
		 * Asset prefab path selection is located in the editor class
		 */
	}
#endif

	public void CreateProcessedGazeDataFile(string filename)
	{
		string newProcFilename = "processed_" + filename;
		if(!filenameToGazeEvent.ContainsKey(newProcFilename))
		{
			List<GazeEvent> geIn = filenameToGazeEvent[filename];
			List<GazeEvent> geOut = ProcessGazeData(geIn);
			savedFilenames.Add(newProcFilename);
			filenameToGazeEvent.Add(newProcFilename, geOut);
		}
	}

	private struct ClusterPoint
	{
		public int gazeIndex;
	}

	private List<GazeEvent> ProcessGazeData(List<GazeEvent> rawGazeEvents)
	{

		/*
		 * K-medoids algorithm adaptation
		 * preprocessing:
		 * Convert rawGazeEvents to clusterpoints for simplicity
		 * Find the different clusters by looking at the ditance between points.
		 * If two points are too far away it means that the next point belongs to
		 * another cluster.
		 */
		List<ClusterPoint> allClusterPoints = new List<ClusterPoint>();
		for(int i = 0; i < rawGazeEvents.Count; i++)
		{
			ClusterPoint point;
			point.gazeIndex = i;
			allClusterPoints.Add(point);
		}
		List<int> clusterSteps = new List<int>();
//		int debugCount = 0;
		for(int i = 0; i < rawGazeEvents.Count-1; i++)
		{
			if((i + 1) == (rawGazeEvents.Count - 1))
			{
				clusterSteps.Add(i + 1);
			}
			else if(Vector3.Distance(rawGazeEvents[i].eventHitPoint, rawGazeEvents[i + 1].eventHitPoint) > maxSaccadeJumpDistance)
			{
				clusterSteps.Add(i + 1);
//				Debug.Log("clusterstep[" + debugCount + "]: " + i);
//				Debug.Log("rawGazeEvents[" + (i + 1) + "]: " + ", end: " + rawGazeEvents.Count - 1);
//				debugCount++;
			}

		}
//		Debug.Log(clusterSteps.Count);
		List<ClusterPoint> medoids = new List<ClusterPoint>();
		int[] lastPoint2closestMedoid = new int[rawGazeEvents.Count];
		for(int j = 0; j < clusterSteps.Count; j++)
		{
			List<ClusterPoint> nonMedoidPoints = new List<ClusterPoint>();
			ClusterPoint medoid;
			startIndex = j > 0 ? clusterSteps[j - 1] : 0;
			int medoidIndex = 0;
//			Debug.Log("startInd: " + startIndex);
			for(int i = startIndex; i < clusterSteps[j]; i++)
			{
				if(rawGazeEvents[allClusterPoints[i].gazeIndex].fixationLength > 0f)
				{
					nonMedoidPoints.Add(allClusterPoints[i]);
				}
			}
			if(nonMedoidPoints.Count > 0)
			{
				if(nonMedoidPoints.Count == 1)
				{
					if(rawGazeEvents[nonMedoidPoints[0].gazeIndex].fixationLength > minFixationDuration)
					{
						medoids.Add(nonMedoidPoints[0]);
					}
				}
				else
				{
					int rindex = Random.Range(0, nonMedoidPoints.Count - 1);
					medoid = nonMedoidPoints[rindex];
					medoids.Add(medoid);
					nonMedoidPoints.RemoveAt(rindex);
					float minCost = float.MaxValue;
					float lastCost = float.MaxValue;
					bool first = true;
					for(int x = 0; x < nonMedoidPoints.Count; x++)
					{
						if(first)
						{
							first = false;
						}
						else
						{
							ClusterPoint newMedoid = nonMedoidPoints[x];
							nonMedoidPoints[x] = medoid;
							medoid = newMedoid;
						}
						//index = pointIndex, value = medoid gaze index
						int[] point2closestMedoid = new int[nonMedoidPoints.Count];
						float[] costs = new float[nonMedoidPoints.Count];
						//Assign points to current medoid and store the distances
						for(int c = 0; c < nonMedoidPoints.Count; c++)
						{
							costs[c] = Vector3.Distance(rawGazeEvents[medoid.gazeIndex].eventHitPoint, rawGazeEvents[nonMedoidPoints[c].gazeIndex].eventHitPoint);
							point2closestMedoid[c] = medoid.gazeIndex;
						}
						//Calculate total cost for current medoid
						float totalCost = 0f;
						for(int c = 0; c < point2closestMedoid.Length; c++)
						{
							totalCost += costs[c];
						}
//						Debug.Log("medoid " + medoid.gazeIndex + " , totalCost: " + totalCost + ", pos: " + rawGazeEvents[medoid.gazeIndex].eventHitPoint);
						if(totalCost < lastCost)
						{
							lastCost = totalCost;
							for(int y = 0; y < point2closestMedoid.Length; y++)
							{
//								Debug.Log("startIndex+y: " + (startIndex + y) + " lastindex: " + (rawGazeEvents.Count - 1));
//								lastPoint2closestMedoid[startIndex + y - indexCorrection] = point2closestMedoid[y];
								lastPoint2closestMedoid[startIndex + y] = point2closestMedoid[y];
							}
//					Debug.Log("medoids: " + medoids.Length + ", j; " + j);
							medoids[medoidIndex] = medoid;
//							Debug.Log("medoid gazeindex" + medoid.gazeIndex);
						}
						else
						{
							ClusterPoint previousMedoid = nonMedoidPoints[x];
							nonMedoidPoints[x] = medoid;
							medoid = previousMedoid;
						}
					}
				}
			}
			medoidIndex++;
		}

		//post processing
		List<ClusterPoint> sortedMedoids = medoids.OrderBy(o=>o.gazeIndex).ToList();
		List<GazeEvent> processedEvents = new List<GazeEvent>();
		for(int i = 0; i < sortedMedoids.Count; i++)
		{
//			Debug.Log("sortedmedoid gazeindex" + sortedMedoids[i].gazeIndex);
			GazeEvent cluster2single = new GazeEvent();
			cluster2single.eventHitPoint = rawGazeEvents[sortedMedoids[i].gazeIndex].eventHitPoint;
			cluster2single.fixationLength = rawGazeEvents[sortedMedoids[i].gazeIndex].fixationLength;
			for(int j = 0; j < lastPoint2closestMedoid.Length; j++)
			{
				if(lastPoint2closestMedoid[j] == sortedMedoids[i].gazeIndex)
				{
					//Mouse debug:
//					cluster2single.fixationLength += 5f;
					cluster2single.fixationLength += rawGazeEvents[allClusterPoints[j].gazeIndex].fixationLength;
				}
			}
//			Debug.Log("medoid " + sortedMedoids[i].gazeIndex + " , newIndex: " + (i + 1) + ", pos: " + rawGazeEvents[medoids[i].gazeIndex].eventHitPoint);
			processedEvents.Add(cluster2single);
		}
		return processedEvents;
	}

	public void CreateSaveFile()
	{
		if(!isSaving)
		{
			isSaving = true;
			StartCoroutine(CreateNewSaveFile());
		}
		else
		{
			Debug.Log("Still creating hitmap", gameObject);
		}
	}

	private IEnumerator CreateNewSaveFile()
	{
		gazeDataList = data.GazeDataList;
		HashSet<string> eventNames = data.EventNames;
		try
		{
			savedFilenames = Serializer.Instance.DeserializeFilenames();
		}
		catch(System.Exception e)
		{
			Debug.Log("filename logfile not found, creating new." + e);
		}
		foreach(string eventName in eventNames)
		{
			string filename = CreateFilename(eventName);
			List<GazeEvent> gazeEventList = new List<GazeEvent>();
			savedFilenames.Add(filename);
			foreach(GazeEvent e in gazeDataList)
			{
				if(e.eventName.Equals(eventName))
				{
					gazeEventList.Add(e);
				}
			}
			SaveGazeEventDataToFile(gazeEventList, filename);
		}
		Serializer.Instance.SerializeFilenames(savedFilenames);
		Debug.Log("saved " + savedFilenames.Count.ToString() + " filenames.");
		isSaving = false;
		yield return null;
	}

	public void SaveGazeEventDataToFile(List<GazeEvent> gazeDataList, string filename)
	{
//		Debug.Log(filename);
		Serializer.Instance.SerializeHitmap(gazeDataList, filename);
		Debug.Log("Data has been saved to file: " + filename, gameObject);
	}

	public void LoadFilesOnDisk()
	{
		isShowingGazeEvents = false;
		isShowingPupilEvents = false;
		isShowingBlinkMap = false;
		isShowingGazeRay = false;
		isShowingObjectName = false;
		isShowingRayOrigin = false;
		isShowingObjectSelectionBox = false;
		if(savedFilenames.Count == 0 || filenameToGazeEvent.Count == 0)
		{
			eventOriginColors.Clear();
			eventGazeRayColors.Clear();
			eventHitPointColors.Clear();
			loadedFiles.Clear();
			savedFilenames = Serializer.Instance.DeserializeFilenames();
			int i = 0;
			foreach(string filename in savedFilenames)
			{
				List<GazeEvent> eventToAdd = new List<GazeEvent>();
				try
				{
					eventToAdd = Serializer.Instance.DeserializeHitmap(filename);
				}
				catch(System.IO.FileNotFoundException e)
				{
					Debug.Log("!GazeDebug! Filename: " + filename + " not found on disk. Removing from filelog.");
					savedFilenames.Remove(filename);
					return;
				}
				filenameToGazeEvent.Add(filename, eventToAdd);
				eventOriginColors.Add(Color.blue);
				eventGazeRayColors.Add(Color.cyan);
				eventHitPointColors.Add(Color.yellow);
			}
		}
	}

	public void ClearLoadedData()
	{
		isShowingGazeEvents = false;
		isShowingPupilEvents = false;
		isShowingBlinkMap = false;
		isShowingGazeRay = false;
		isShowingObjectName = false;
		isShowingRayOrigin = false;
		isShowingObjectSelectionBox = false;
		gazeDataList.Clear();
		filenameToGazeEvent.Clear();
		loadedFiles.Clear();
		eventOriginColors.Clear();
		eventGazeRayColors.Clear();
		eventHitPointColors.Clear();
	}

#if UNITY_EDITOR
	public void DeleteSaveFile(string filename)
	{
		if(FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/" + filename))
		{
			Debug.Log(filename + " deleted!");
			savedFilenames.Remove(filename);
			Serializer.Instance.SerializeFilenames(savedFilenames);
		}
		else
		{
			savedFilenames.Remove(filename);
			Debug.Log(filename + " not found on disk. Removing from list.");
		}
	}


	public void DeleteAllSaveFiles()
	{
		if(savedFilenames.Count > 0)
		{
			foreach(string filename in savedFilenames)
			{
				if(FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/" + filename))
				{
					Debug.Log(filename + " deleted!");
				}
				else
				{
					Debug.Log(filename + " not found.");
				}
			}
			savedFilenames.Clear();
			Serializer.Instance.SerializeFilenames(savedFilenames);
		}
		else
		{
			Debug.Log("no files found");
		}
	}
#endif

	private string CreateFilename(string eventName)
	{
		System.DateTime date = System.DateTime.Now;
		string filename = Application.loadedLevelName + "_" + 
			eventName + "_" + 
			date.Day.ToString() + "_" + 
			date.Month.ToString() + "_" +
			date.Year.ToString() + "_" + 
			date.TimeOfDay.Hours + "_" +
			date.TimeOfDay.Minutes + "_" +
			date.TimeOfDay.Seconds + ".dat";
		return filename;
	}

	public Dictionary<string, List<GazeEvent>> FilenameToGazeEvent
	{
		get
		{
			return filenameToGazeEvent;
		}
	}

	public List<string> Filenames
	{
		get
		{
			return savedFilenames;
		}
	}

	public List<GazeEvent> GazeDataList
	{
		get
		{
			return gazeDataList;
		}
	}

	public List<string> LoadedFiles
	{
		get
		{
			return loadedFiles;
		}
	}

	public bool IsShowingGazeEvents
	{
		get
		{
			return isShowingGazeEvents;
		}
	}

	public bool IsShowingPupilEvents
	{
		get
		{
			return isShowingPupilEvents;
		}
	}

	public bool IsShowingBlinkMap
	{
		get
		{
			return isShowingBlinkMap;
		}
	}

	public bool IsShowingObjectSelectionBox
	{
		get
		{
			return isShowingObjectSelectionBox;
		}
	}
}
