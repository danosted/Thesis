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
	private List<string>
		dataToCompare;
	[SerializeField]
	private Color
		pupilColor = Color.yellow;
	[SerializeField]
	private string
		staticfilename = "static.xml";
	[SerializeField]
	private List<string>
		savedFilenames = new List<string>();
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
	private Material
		meshMaterial;
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

	private float characterCubeSize = 0.5f;
	private float gazeRayHitSphereSize = 0.1f;
	private float maxHeatMapPointSize = 2f;

	private bool isSaving;

	public float maxPupilSize = 30f;
	public float minPupilSize = 20f;
	public float minGazeDataIndex = 0f;
	public float maxGazeDataIndex = 1f;

	private int startIndex = 0;
	private int endIndex = 0;

	private GUIStyle style = new GUIStyle();

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

	//Render 3D GazeMap
	void OnDrawGizmosSelected()
	{
		if(isShowingGazeEvents || isShowingPupilEvents || isShowingBlinkMap)
		{
			for(int fileindex = 0; fileindex < dataToCompare.Count; fileindex++)
			{
				foreach(KeyValuePair<string, List<GazeEvent>> entry in filenameToGazeEvent)
				{
					//If filename matches what is selected to be shown in the inspector
					if(entry.Key == dataToCompare.ToArray()[fileindex])
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
		//Hit point color
		Handles.color = eventHitPointColors.Count > fileindex ? eventHitPointColors.ToArray()[fileindex] : Color.yellow;
		Handles.DrawSolidDisc(e.eventHitPoint, (Camera.current.transform.position - e.eventHitPoint).normalized, gazeRayHitSphereSize + (0.001f * e.fixationLength));
		//Event origin color
		if(!isShowingPupilEvents && isShowingRayOrigin)
		{
			Gizmos.color = eventOriginColors.Count > fileindex ? eventOriginColors.ToArray()[fileindex] : Color.blue;
			Gizmos.DrawCube(e.eventOrigin, Vector3.one * characterCubeSize);
		}
		//index of event
		style.alignment = TextAnchor.MiddleCenter;
		Handles.color = Color.black;
		Handles.Label(e.eventHitPoint, (eventindex + 1).ToString(), style);
		//Name of object that was hit
		if(isShowingObjectName)
		{
			style.alignment = TextAnchor.MiddleCenter;
			Handles.Label(e.eventHitPoint + Vector3.up * 2f * gazeRayHitSphereSize, e.eventHitName, style);
		}
		/*
		 * Note to self:
		 * Asset prefab path selection is located in the editor class
		 */
	}

	public void ProcessGazeData()
	{
		foreach(string filename in dataToCompare)
		{
			List<GazeEvent> geIn = filenameToGazeEvent[filename];
			List<GazeEvent> geOut = ProcessGazeData(geIn, 0.3f);
			string newProcFilename = "processed_" + filename;
			savedFilenames.Add(newProcFilename);
			filenameToGazeEvent.Add(newProcFilename, geOut);
		}
	}

	private struct ClusterPoint
	{
		public int gazeIndex;
	}

	private List<GazeEvent> ProcessGazeData(List<GazeEvent> rawGazeEvents, float thresh)
	{

		//k-medoids algorithm adaptation

		/* preprocessing:
		 * Convert rawGazeEvents to clusterpoints for simplicity
		 * Find the different clusters by looking at the ditance between points.
		 * If two points are too far away it means that the next point belongs to
		 * another cluster
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
			if(Vector3.Distance(rawGazeEvents[i].eventHitPoint, rawGazeEvents[i + 1].eventHitPoint) > thresh || (i + 1) == rawGazeEvents.Count - 1)
			{
				clusterSteps.Add(i + 1);
//				Debug.Log("clusterstep[" + debugCount + "]: " + i);
//				Debug.Log("rawGazeEvents[" + (i + 1) + "]: " + ", end: " + rawGazeEvents.Count - 1);
//				debugCount++;
			}
		}
//		Debug.Log(clusterSteps.Count);
		ClusterPoint[] medoids = new ClusterPoint[clusterSteps.Count];
		int[] lastPoint2closestMedoid = new int[rawGazeEvents.Count];
		int indexCorrection = 0;
		for(int j = 0; j < clusterSteps.Count; j++)
		{
			List<ClusterPoint> nonMedoidPoints = new List<ClusterPoint>();
			ClusterPoint medoid;
			startIndex = j > 0 ? clusterSteps[j - 1] : 0;
//			Debug.Log("startInd: " + startIndex);
			for(int i = startIndex; i < clusterSteps[j]; i++)
			{
				nonMedoidPoints.Add(allClusterPoints[i]);
			}
			if(nonMedoidPoints.Count > 0)
			{
				if(nonMedoidPoints.Count == 1)
				{
					if(rawGazeEvents[nonMedoidPoints[0].gazeIndex].fixationLength > 0.5f)
					{
						medoids[j] = nonMedoidPoints[0];
					}
				}
				else
				{
					int rindex = Random.Range(0, nonMedoidPoints.Count - 1);
					medoid = nonMedoidPoints[rindex];
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
							ClusterPoint onmedoidtemp = nonMedoidPoints[x];
							nonMedoidPoints[x] = medoid;
							medoid = onmedoidtemp;
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
							medoids[j] = medoid;
						}
						else
						{
							ClusterPoint tempMedoid = nonMedoidPoints[x];
							nonMedoidPoints[x] = medoid;
							medoid = tempMedoid;
						}
					}
				}
			}
		}

		/*
		List<ClusterPoint> nonMedoidPoints = new List<ClusterPoint>();
		List<ClusterPoint> medoids = new List<ClusterPoint>(k);

		int debugSteps = 0;
		for(int i = 0; i < rawGazeEvents.Count; i++)
		{
			ClusterPoint point;
			point.gazeIndex = i;
			point.position = rawGazeEvents[i].eventHitPoint;
			nonMedoidPoints.Add(point);
		}
		//Step 1:
		//
		//random select a center 
		for(int i = 0; i < k; i++)
		{
			int rindex = Random.Range(0, nonMedoidPoints.Count - 1);
			medoids.Add(nonMedoidPoints[rindex]);
			nonMedoidPoints.RemoveAt(rindex);
		}
		//Step 2:
		//Calculate distance to associate points to nearest medoid
		int[] lastPoint2closestMedoid = new int[nonMedoidPoints.Count];
		float[] costs = new float[nonMedoidPoints.Count];
		//Assign to clusters based on the smallest distance
		for(int c = 0; c < nonMedoidPoints.Count; c++)
		{
			float minCost = float.MaxValue;
			for(int r = 0; r < medoids.Count; r++)
			{
				float curCost = Vector3.Distance(medoids[r].position, nonMedoidPoints[c].position);
				if(curCost <= minCost)
				{
					lastPoint2closestMedoid[c] = medoids[r].gazeIndex;
					minCost = curCost;
				}
			}
			costs[c] = minCost;
//			Debug.Log(c + ", " + minCost);
			Debug.Log("p: " + nonMedoidPoints[c].gazeIndex + ", m: " + lastPoint2closestMedoid[c]);
		}
		//Calculate initial total cost
		float lastCost = 0f;
		for(int c = 0; c < lastPoint2closestMedoid.Length; c++)
		{
			lastCost += costs[c];
		}
		//core loop
		for(int i = 0; i < medoids.Count; i++)
		{
			for(int j = 0; j < nonMedoidPoints.Count; j++)
			{
				ClusterPoint moid = medoids[i];
				ClusterPoint nonmoid = nonMedoidPoints[j];
				nonMedoidPoints[j] = moid;
				medoids[i] = nonmoid;
				//index = pointIndex, value = medoid gaze index
				int[] point2closestMedoid = new int[nonMedoidPoints.Count];
				//Assign to clusters based on the smallest distance
				for(int c = 0; c < nonMedoidPoints.Count; c++)
				{
					float minCost = float.MaxValue;
					for(int r = 0; r < medoids.Count; r++)
					{
						float curCost = Vector3.Distance(medoids[r].position, nonMedoidPoints[c].position);
						if(curCost <= minCost)
						{
							point2closestMedoid[c] = medoids[r].gazeIndex;
							minCost = curCost;
						}
					}


					costs[c] = minCost;
				}
				//Calculate total cost
				float totalCost = 0f;
				for(int c = 0; c < point2closestMedoid.Length; c++)
				{
					totalCost += costs[c];
				}
				if(totalCost < lastCost)
				{
					lastCost = totalCost;
					lastPoint2closestMedoid = point2closestMedoid;
				}
				else
				{
					nonMedoidPoints[j] = nonmoid;
					medoids[i] = moid;
				}
			}
		}
		*/


		//post processing
//		List<ClusterPoint> sortedMedoids = medoids.OrderBy(o => o.gazeIndex).ToList();
		List<GazeEvent> processedEvents = new List<GazeEvent>();
		for(int i = 0; i < medoids.Length; i++)
		{
			GazeEvent cluster2single = new GazeEvent();
			cluster2single.eventHitPoint = rawGazeEvents[medoids[i].gazeIndex].eventHitPoint;
			for(int j = 0; j < lastPoint2closestMedoid.Length; j++)
			{
				if(lastPoint2closestMedoid[j] == medoids[i].gazeIndex)
				{
					//Mouse debug:
//					cluster2single.fixationLength += 5f;
					cluster2single.fixationLength += rawGazeEvents[allClusterPoints[j].gazeIndex].fixationLength;
				}
			}
//			Debug.Log("medoid " + medoids[i].gazeIndex + " , newIndex: " + (i + 1) + ", pos: " + rawGazeEvents[medoids[i].gazeIndex].eventHitPoint);
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
			savedFilenames = Serializer.Instance.DeserializeFilenames(staticfilename);
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
		Serializer.Instance.SerializeFilenames(savedFilenames, staticfilename);
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

	public void ShowGazeData(string filename)
	{
		dataToCompare.Add(filename);
//		gazeDataList = Serializer.Instance.DeserializeHitmap(filename);
	}

	public void HideGazeData(string filename)
	{
		dataToCompare.Remove(filename);
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
			dataToCompare.Clear();
			savedFilenames = Serializer.Instance.DeserializeFilenames(staticfilename);
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
		dataToCompare.Clear();
		eventOriginColors.Clear();
		eventGazeRayColors.Clear();
		eventHitPointColors.Clear();
	}

	public void DeleteSaveFile(string filename)
	{
		if(FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/" + filename))
		{
			Debug.Log(filename + " deleted!");
			savedFilenames.Remove(filename);
			Serializer.Instance.SerializeFilenames(savedFilenames, staticfilename);
		}
		else
		{
			Debug.Log(filename + " not found.");
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
			Serializer.Instance.SerializeFilenames(savedFilenames, staticfilename);
		}
		else
		{
			Debug.Log("no files found");
		}
	}

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

	public List<string> DataToCompare
	{
		get
		{
			return dataToCompare;
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
