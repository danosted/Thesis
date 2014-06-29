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
	[SerializeField]
	private bool
		isShowingTargetPrefab;

	private Dictionary<string, GameObject> filepathToGameObject = new Dictionary<string, GameObject>();
	private Dictionary<string, List<GazeEvent>> filenameToGazeEvent = new Dictionary<string, List<GazeEvent>>();
	private List<GazeEvent> gazeDataList = new List<GazeEvent>();
	private HashSet<string> savedFilenames = new HashSet<string>();
	private List<string> loadedFiles = new List<string>();

	private float characterCubeSize = 0.5f;
	private float gazeRayHitSphereRadius = 0.2f;
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

	void Awake()
	{
		//Cleanup
		if(Application.isPlaying)
		{
			Transform trashcan = transform.GetChild(0);
			for(int i = 0; i < trashcan.childCount; i++)
			{
				Destroy(trashcan.GetChild(i).gameObject);
			}
			filepathToGameObject.Clear();
		}
	}

#if UNITY_EDITOR
    void OnGUI()
	{
		int padding = 10;
		int width = 180;
		int y = padding;
		int x = Screen.width - (width + padding);

		if(GUI.Button(new Rect(x, y, 170, 20), "Save Session Data"))
		{
			SaveCurrentSession();
		}
	}

	//Render 3D GazeMap
	void OnDrawGizmos()
	{

		if((isShowingGazeEvents || isShowingPupilEvents || isShowingBlinkMap || isShowingTargetPrefab) && Selection.activeGameObject == this.gameObject)
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
                        float start = minGazeDataIndex * (gazeArray.Length - 1);
                        float end = maxGazeDataIndex * gazeArray.Length;
                        
						startIndex = (int)start;
						endIndex = (int)Mathf.Ceil(end);



						for(i = startIndex; i < endIndex; i++)
						{
							GazeEvent e = gazeArray[i];
							if(isShowingGazeEvents || isShowingTargetPrefab)
							{
                                float percentage = 1f;
                                if(i == endIndex - 1)
                                {
                                    percentage = end - (float)(i);
                                }
								Vector3 nextSaccadePoint = i < endIndex - 1 ? gazeArray[i + 1].eventHitPoint : e.eventHitPoint;
								DrawGazeEvent(e, nextSaccadePoint, fileindex, i, percentage);
							}
							else
							{
								string filepath2Color2position = CreateUniqueHiteventFilepath(e);
								if(filepathToGameObject.ContainsKey(filepath2Color2position))
								{
									filepathToGameObject[filepath2Color2position].SetActive(false);
								}
							}
							if(isShowingPupilEvents)
							{
								float pupilSize = gazeArray[i].pupilMeanSize;
								//TODO: Find the right min and max for pupil size
								float map = Mathf.InverseLerp(minPupilSize, maxPupilSize, pupilSize);
								Gizmos.color = new Color(pupilColor.r * map, pupilColor.g * map, pupilColor.b * map, 0.8f);
								Gizmos.DrawSphere(gazeArray[i].eventGazeRay.origin, maxHeatMapPointSize * (map + 0.1f));
							}
//							if(isShowingBlinkMap)
//							{
//								
//							}
						}
						//We need to hide the instantiated prefabs for those events not shown
						for(i = 0; i < startIndex; i++)
						{
							GazeEvent e = gazeArray[i];
							string filepath2Color2position = CreateUniqueHiteventFilepath(e);
							if(filepathToGameObject.ContainsKey(filepath2Color2position))
							{
								filepathToGameObject[filepath2Color2position].SetActive(false);
							}
						}
						for(i = endIndex; i < gazeArray.Length; i++)
						{
							GazeEvent e = gazeArray[i];
							string filepath2Color2position = CreateUniqueHiteventFilepath(e);
							if(filepathToGameObject.ContainsKey(filepath2Color2position))
							{
								filepathToGameObject[filepath2Color2position].SetActive(false);
							}
						}
					}
				}
			}
		}
		//We need to hide the instantiated prefabs when current selection is not the gazemapmanager
		if(Selection.activeGameObject != this.gameObject)
		{
			foreach(KeyValuePair<string, GameObject> entry in filepathToGameObject)
			{
				entry.Value.SetActive(false);
			}
		}
	}

	private void DrawGazeEvent(GazeEvent e, Vector3 nextSaccadePoint, int fileindex, int eventindex, float percentage)
	{
		Vector3 camDir = (Camera.current.transform.position - e.eventHitPoint).normalized;
		//Gaze Ray
		if(isShowingGazeRay)
		{
			Gizmos.color = eventGazeRayColors.Count > fileindex ? eventGazeRayColors.ToArray()[fileindex] : Color.cyan;
			Gizmos.DrawLine(e.eventGazeRay.origin, e.eventHitPoint);
		}
		//Saccade Ray
		if(isShowingGazeEvents)
		{
			if(nextSaccadePoint != e.eventHitPoint)
			{
				Gizmos.color = eventHitPointColors.Count > fileindex ? eventHitPointColors.ToArray()[fileindex] : Color.yellow;
				Gizmos.DrawLine(e.eventHitPoint, nextSaccadePoint);
			}
			//HitPoint
			Handles.color = eventHitPointColors.Count > fileindex ? eventHitPointColors.ToArray()[fileindex] : Color.yellow;
			float fixSize = Mathf.Clamp(e.fixationLength, 0.1f, 0.5f) * percentage;
			Handles.DrawSolidDisc(e.eventHitPoint, camDir, /*gazeRayHitSphereRadius*/fixSize);
			//index of event
			style.alignment = TextAnchor.MiddleCenter;
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 10;
			string eventIndexString = (eventindex + 1).ToString();
			Handles.color = Color.black;
			Handles.Label(e.eventHitPoint, eventIndexString, style);
		}
		//Event origin color
		if(!isShowingPupilEvents && isShowingRayOrigin)
		{
			Gizmos.color = eventOriginColors.Count > fileindex ? eventOriginColors.ToArray()[fileindex] : Color.blue;
			Gizmos.DrawCube(e.eventGazeRay.origin, Vector3.one * characterCubeSize);
		}
		//Name of object that was hit
		if(isShowingObjectName)
		{
			Handles.Label(e.eventHitPoint + Vector3.up * 2f * gazeRayHitSphereRadius, e.eventHitName, style);
		}
        //Target prefab
		if(isShowingTargetPrefab)
		{
			if(e.filePath != "")
			{
//				Handles.Label(e.eventHitPoint + Vector3.up * 2f * gazeRayHitSphereRadius, e.filePath, style);
				string filepath2Color2position = CreateUniqueHiteventFilepath(e);
				if(filepathToGameObject.ContainsKey(filepath2Color2position))
				{
					filepathToGameObject[filepath2Color2position].SetActive(true);
				}
				else
				{
					Object prefab = AssetDatabase.LoadAssetAtPath(e.filePath, typeof(GameObject));
					if(prefab != null)
					{
						GameObject go = Instantiate(prefab, e.eventHitObjectPosition, e.eventHitRotation) as GameObject;
						go.transform.localScale = e.eventHitScale;
						go.transform.renderer.sharedMaterial.color = e.eventHitColor;
						go.transform.parent = transform.GetChild(0);
						filepathToGameObject.Add(filepath2Color2position, go);	
					}
				}
			}
		}
		else
		{
			string filepath2Color2position = CreateUniqueHiteventFilepath(e);
			if(filepathToGameObject.ContainsKey(filepath2Color2position))
			{
				filepathToGameObject[filepath2Color2position].SetActive(false);
			}
		}
		/*
		 * Note to self:
		 * Asset prefab path selection is located in the editor class
		 */

		//Debug
//		Handles.Label(e.eventHitPoint, e.fixationIndex.ToString(), style);

	}
#endif

	public void CreateProcessedGazeDataFile(string filename)
	{
        LoadFile(filename);
        UnloadFile(filename);
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
		public int medoidGazeIndex;
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
        #region init
        List<ClusterPoint> allClusterPoints = new List<ClusterPoint>();
		for(int i = 0; i < rawGazeEvents.Count; i++)
		{
			ClusterPoint point;
			point.gazeIndex = i;
			point.medoidGazeIndex = -1;
			allClusterPoints.Add(point);
        }
        #endregion
        #region clustersteps
        List<int> clusterSteps = new List<int>();
		bool foundHit = false;
		string lastFP = "";
        for (int i = 0; i < rawGazeEvents.Count-1; i++)
		{
            if ((i + 1) == (rawGazeEvents.Count - 1))
            {
                clusterSteps.Add(i + 2);
//				Debug.Log("clusterstep: " + (i + 1));
//				Debug.Log("added eventHitname: " + rawGazeEvents[i+1].eventHitName + " at: " + i);
            }
            else
            {
                if (rawGazeEvents[i].filePath != "")
                {
                    //Debug.Log("foundhit: " + (i));
                    //Debug.Log("fp: " + rawGazeEvents[i].filePath + " at: " + i);
                    if (foundHit)
                    {
                        if (Vector3.Distance(rawGazeEvents[i].eventHitPoint, rawGazeEvents[i - 1].eventHitPoint) > maxSaccadeJumpDistance)
                        {
                            clusterSteps.Add(i);
                            foundHit = false;
//                            Debug.Log("added fp: " + rawGazeEvents[i].filePath + " at: " + i);
                            //Debug.Log("clusterstep: " + i);
                        }
                        else
                        {
                            string thisFP = CreateUniqueHiteventFilepath(rawGazeEvents[i]);

                            if (!thisFP.Equals(lastFP))
                            {
                                clusterSteps.Add(i);
                                lastFP = thisFP;
//								Debug.Log("added fp: " + thisFP + " at: " + i);
                                //Debug.Log("clusterstep: " + i);
                            }
                        }
                    }
                    else
                    {
                        lastFP = CreateUniqueHiteventFilepath(rawGazeEvents[i]);
                        foundHit = true;
                    }
                }
                else if (Vector3.Distance(rawGazeEvents[i].eventHitPoint, rawGazeEvents[i + 1].eventHitPoint) > maxSaccadeJumpDistance)
                {
                    clusterSteps.Add(i + 1);
                    foundHit = false;
                    //Debug.Log("clusterstep: " + i);
                }
            }
        }
        #endregion
        #region k-medoid
        List<ClusterPoint> medoids = new List<ClusterPoint>();
		Dictionary<ClusterPoint, List<ClusterPoint>> medoid2cluster = new Dictionary<ClusterPoint, List<ClusterPoint>>();
		int medoidIndex = 0;
		for(int j = 0; j < clusterSteps.Count; j++)
		{
//            HashSet<ClusterPoint> processedClusterPoints = new HashSet<ClusterPoint>();
			List<ClusterPoint> nonMedoidPoints = new List<ClusterPoint>();
			ClusterPoint medoid;
			startIndex = j > 0 ? clusterSteps[j - 1] : 0;
//			Debug.Log("startInd: " + startIndex + ", clusterstep[j]: " + clusterSteps[j]);
			for(int i = startIndex; i < clusterSteps[j]; i++)
			{
				nonMedoidPoints.Add(allClusterPoints[i]);
//				Debug.Log("added: " + i);
			}
			if(nonMedoidPoints.Count > 0)
			{
				if(nonMedoidPoints.Count == 1)
				{
                    if (rawGazeEvents[nonMedoidPoints[0].gazeIndex].fixationLength > minFixationDuration || rawGazeEvents[nonMedoidPoints[0].gazeIndex].filePath != "" || rawGazeEvents[nonMedoidPoints[0].gazeIndex].eventHitName != "")
					{
//						Debug.Log("medoid: " + nonMedoidPoints[0].gazeIndex);
						medoids.Add(nonMedoidPoints[0]);

                        //testing
                        medoid2cluster.Add(medoids[medoidIndex], new List<ClusterPoint>());
					}
					else
					{
						medoidIndex--;
					}
				}
				else if(nonMedoidPoints.Count == 2)
				{
					int rindex = Random.Range(0, nonMedoidPoints.Count - 1);
					medoids.Add(nonMedoidPoints[rindex]);
					nonMedoidPoints.RemoveAt(rindex);
					ClusterPoint cp = allClusterPoints.Find(ge => ge.gazeIndex.Equals(nonMedoidPoints[0].gazeIndex));
					cp.medoidGazeIndex = medoids[medoidIndex].gazeIndex;
//                    processedClusterPoints.Add(cp);
                    
                    //testing
					medoid2cluster.Add(medoids[medoidIndex], nonMedoidPoints);
				}
				else
				{
					int rindex = Random.Range(0, nonMedoidPoints.Count - 1);
					medoid = nonMedoidPoints[rindex];
					medoids.Add(medoid);
					nonMedoidPoints.RemoveAt(rindex);
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
						//Assign points to current medoid and calculate the cost
						float totalCost = 0f;
						for(int c = 0; c < nonMedoidPoints.Count; c++)
						{
							totalCost += Vector3.Distance(rawGazeEvents[medoid.gazeIndex].eventHitPoint, rawGazeEvents[nonMedoidPoints[c].gazeIndex].eventHitPoint);
                            ClusterPoint cp = allClusterPoints.Find(ge => ge.gazeIndex.Equals(nonMedoidPoints[c].gazeIndex));
                            cp.medoidGazeIndex = medoid.gazeIndex;
//                            processedClusterPoints.Add(cp);
						}
//						Debug.Log("medoid " + medoid.gazeIndex + " , totalCost: " + totalCost + ", pos: " + rawGazeEvents[medoid.gazeIndex].eventHitPoint);
						if(totalCost < lastCost)
						{
							lastCost = totalCost;
//							Debug.Log("medoidIndex " + medoidIndex + ", medoidsSize: " + medoids.Count);
							medoids[medoidIndex] = medoid;
//                            if(processedClusterPoints.Contains(medoid))
//                            {
//                                processedClusterPoints.Remove(medoid);
//                            }
						}
						else
						{
//							processedClusterPoints.Clear();
							ClusterPoint previousMedoid = nonMedoidPoints[x];
							nonMedoidPoints[x] = medoid;
							medoid = previousMedoid;
						}
					}

                    //testing
//					Debug.Log("procpoints: " + nonMedoidPoints.Count);
					medoid2cluster.Add(medoids[medoidIndex], nonMedoidPoints);
				}
			}
			else
			{
				Debug.Log("empty encountered");
			}
			medoidIndex++;
        }
        #endregion
//		Debug.Log("allpoints " + allClusterPoints.Count);
        #region post-processing
		List<ClusterPoint> sortedMedoids = medoids.OrderBy(o => o.gazeIndex).ToList();
        List<GazeEvent> processedEvents = new List<GazeEvent>();
		for (int i = 0; i < sortedMedoids.Count; i++)
        {
//			Debug.Log("sortedmedoid: " + sortedMedoids[i].gazeIndex);
            List<ClusterPoint> values = medoid2cluster[sortedMedoids[i]];
			HashSet<string> eventNames = new HashSet<string>();
			Dictionary<int, float> fixIndex2duration = new Dictionary<int, float>();
            GazeEvent cluster2single = rawGazeEvents[sortedMedoids[i].gazeIndex];
			int curFixIndex = cluster2single.fixationIndex;
			int max_fixindex = 0;
			float max_timestep = 0f;
			bool hasFilepath = cluster2single.filePath != "" ? true : false;
			bool haseventHitName = true;
			if(cluster2single.eventHitName != "")
			{
				if((cluster2single.eventHitName.Contains(":") || cluster2single.eventHitName.Contains("Experiment")))
			   	{
					eventNames.Add(cluster2single.eventHitName);
					haseventHitName = true;
				}
				else
				{
					haseventHitName = false;
				}
			}
//			Debug.Log(hasFilepath);
//			Debug.Log(allClusterPoints.Count);
//			Debug.Log("procpoints " + values.Count);
            foreach (ClusterPoint cp in values)
            {
//				Debug.Log("geindex: " + cp.gazeIndex);
                GazeEvent ge = rawGazeEvents[cp.gazeIndex];
                string filepath = ge.filePath;
                #region set_filepath
                if (filepath != "")
                {
                    if (!hasFilepath)
                    {
                        cluster2single.filePath = filepath;
                        cluster2single.eventHitObjectPosition = ge.eventHitObjectPosition;
                        cluster2single.eventHitScale = ge.eventHitScale;
                        cluster2single.eventHitRotation = ge.eventHitRotation;
                        cluster2single.eventHitColor = ge.eventHitColor;
                        cluster2single.filePath = filepath;
                        hasFilepath = true;
                    }
                }
                #endregion
                #region set_eventhitname
                string eventHitName = ge.eventHitName;
				if (eventHitName != "" && (eventHitName.Contains(":") || eventHitName.Contains("Experiment")))
                {
//					Debug.Log("event: " + eventHitName + ", i: " + cp.gazeIndex); 
                    //Debug.Log("eventHitName: " + eventHitName);
                    if (!haseventHitName)
                    {
						eventNames.Add(eventHitName);
                        //Debug.Log("double eventname: " + allClusterPoints[j].gazeIndex);
                        cluster2single.eventHitName = eventHitName;
                        haseventHitName = true;
                    }
                    else
                    {
						if(!eventNames.Contains(eventHitName) || eventHitName.Contains("Experiment"))
						{
							eventNames.Add(eventHitName);
							cluster2single.eventHitName += "\n" + eventHitName;
//							Debug.Log("double eventHitName: " + eventHitName + " at " + cp.gazeIndex);
						}
                        //Debug.Log("double eventName: " + allClusterPoints[j].gazeIndex);
                    }
                }
                #endregion
                #region set_fixationindex
				max_fixindex = max_fixindex > ge.fixationIndex ? max_fixindex : ge.fixationIndex;
                if (curFixIndex == ge.fixationIndex)
                {
                    if (cluster2single.fixationLength < ge.fixationLength)
                    {
                        cluster2single.fixationLength = ge.fixationLength;
                    }
                }
                else
                {
                    if (fixIndex2duration.ContainsKey(ge.fixationIndex))
                    {
                        if (fixIndex2duration[ge.fixationIndex] < ge.fixationLength)
                        {
                            fixIndex2duration[ge.fixationIndex] = ge.fixationLength;
                        }
                    }
                    else
                    {
                        fixIndex2duration.Add(ge.fixationIndex, ge.fixationLength);
                    }
                }
                #endregion
				#region set_timestep
				max_timestep = max_timestep > ge.eventTime ? max_timestep : ge.eventTime;
				#endregion
            }
            foreach (float duration in fixIndex2duration.Values)
            {
                cluster2single.fixationLength += duration;
            }
			cluster2single.fixationIndex = max_fixindex;
//			Debug.Log("medoid " + sortedMedoids[i].gazeIndex + " , newIndex: " + (i + 1) + ", pos: " + rawGazeEvents[medoids[i].gazeIndex].eventHitPoint);
            if(cluster2single.fixationLength > 0f || hasFilepath)
            {
                //saccadeCount++;
                //cluster2single.saccadeCount = saccadeCount;
                if (processedEvents.Count > 0)
                {
                    float saccadeJumpLength = Vector3.Distance(cluster2single.eventHitPoint, rawGazeEvents[sortedMedoids[i - 1].gazeIndex].eventHitPoint);
                    cluster2single.saccadeJumpLength = saccadeJumpLength;
                }
                processedEvents.Add(cluster2single);
            }
        }
        #endregion
        return processedEvents;

    }

	public void SaveCurrentSession()
	{
		gazeDataList = data.GazeDataList;
		HashSet<string> eventNames = data.EventNames;
		try
		{
			savedFilenames = Serializer.Instance.DeserializeFilenames();
		}
		catch(System.Exception e)
		{
            Debug.Log("!GazeDebug!: " + "filename logfile not found, creating new." + e);
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
        Debug.Log("!GazeDebug!: " + "saved " + eventNames.Count.ToString() + " filenames.");
	}

	private void SaveGazeEventDataToFile(List<GazeEvent> gazeDataList, string filename)
	{
//		Debug.Log(filename);
		Serializer.Instance.SerializeHitmap(gazeDataList, filename);
        Debug.Log("!GazeDebug!: " + "Data has been saved to file: " + filename, gameObject);
	}

	public void CreateSaveFile(string filename)
	{
		//		Debug.Log(filename);
		if(filenameToGazeEvent.ContainsKey(filename))
		{
			List<GazeEvent> gazeData = filenameToGazeEvent[filename];
			Serializer.Instance.SerializeHitmap(gazeData, filename);
			savedFilenames
				= Serializer.Instance.DeserializeFilenames();
			if(!savedFilenames.Contains(filename))
			{
				savedFilenames.Add(filename);
			}
			Serializer.Instance.SerializeFilenames(savedFilenames);
            Debug.Log("!GazeDebug!: " + "Data has been saved to file: " + filename, gameObject);
		}
		else
		{
			if(savedFilenames.Contains(filename))
			{
				savedFilenames.Remove(filename);
			}
			if(loadedFiles.Contains(filename))
			{
				loadedFiles.Remove(filename);
			}
            Debug.Log("!GazeDebug!: " + "Filename data not found. Removing filename.");
		}
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
		isShowingTargetPrefab = false;
		if(savedFilenames.Count == 0 || filenameToGazeEvent.Count == 0)
		{
			eventOriginColors.Clear();
			eventGazeRayColors.Clear();
			eventHitPointColors.Clear();
			loadedFiles.Clear();
            savedFilenames = Serializer.Instance.DeserializeFilenames();
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
		isShowingTargetPrefab = false;
		Transform trashcan = transform.GetChild(0);
		while(trashcan.childCount > 0)
		{
			DestroyImmediate(trashcan.GetChild(0).gameObject);
		}
		filepathToGameObject.Clear();
		gazeDataList.Clear();
		filenameToGazeEvent.Clear();
		loadedFiles.Clear();
		eventOriginColors.Clear();
		eventGazeRayColors.Clear();
		eventHitPointColors.Clear();
	}

	public void UnloadFile(string filename)
	{
		int index = loadedFiles.IndexOf(filename);
		loadedFiles.Remove(filename);
		eventOriginColors.RemoveAt(index);
		eventGazeRayColors.RemoveAt(index);
		eventHitPointColors.RemoveAt(index);
		foreach(KeyValuePair<string, GameObject> entry in filepathToGameObject)
		{
			entry.Value.SetActive(false);
		}
	}

	public void LoadFile(string filename)
	{
        if (!filenameToGazeEvent.ContainsKey(filename))
        {
            List<GazeEvent> eventToAdd = new List<GazeEvent>();
            try
            {
                eventToAdd = Serializer.Instance.DeserializeHitmap(filename);
            }
            catch (System.IO.FileNotFoundException e)
            {
                Debug.Log("!GazeDebug! Filename: " + filename + " not found on disk. Removing from filelog." + "\n" + e);
                savedFilenames.Remove(filename);
                Serializer.Instance.SerializeFilenames(savedFilenames);
            }
            if (savedFilenames.Contains(filename))
            {
                filenameToGazeEvent.Add(filename, eventToAdd);
            }
        }
        loadedFiles.Add(filename);
        eventOriginColors.Add(Color.blue);
        eventGazeRayColors.Add(Color.cyan);
        eventHitPointColors.Add(Color.yellow);
	}

#if UNITY_EDITOR
	public void DeleteSaveFile(string filename)
	{
		if(FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/" + filename))
		{
			Debug.Log(filename + " deleted!");
			savedFilenames.Remove(filename);
		}
		else
		{
			savedFilenames.Remove(filename);
            Debug.Log("!GazeDebug! Filename: " + filename + " not found on disk. Removing from list.");
		}
        Serializer.Instance.SerializeFilenames(savedFilenames);
	}


	public void DeleteAllSaveFiles()
	{
		if(savedFilenames.Count > 0)
		{
			foreach(string filename in savedFilenames)
			{
				if(FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/" + filename))
				{
					Debug.Log("!GazeDebug! Filename: " + filename + " deleted!");
				}
				else
				{
                    Debug.Log("!GazeDebug! Filename: " + filename + " not found.");
				}
			}
			savedFilenames.Clear();
			Serializer.Instance.SerializeFilenames(savedFilenames);
		}
		else
		{
            Debug.Log("!GazeDebug!: " + "no files found");
		}
	}
#endif

	private string CreateFilename(string eventName)
	{
		System.DateTime date = System.DateTime.Now;
		string filename =  
			eventName + "_" +
            Application.loadedLevelName + "_" +
			date.Day.ToString() + "_" + 
			date.Month.ToString() + "_" +
			date.Year.ToString() + "_" + 
			date.TimeOfDay.Hours + "_" +
			date.TimeOfDay.Minutes + "_" +
			date.TimeOfDay.Seconds + ".xml";
		return filename;
	}

	private string CreateUniqueHiteventFilepath(GazeEvent e)
	{
		string fp = e.filePath + e.eventHitObjectPosition;
		return fp;
	}

	public Dictionary<string, List<GazeEvent>> FilenameToGazeEvent
	{
		get
		{
			return filenameToGazeEvent;
		}
	}

	public HashSet<string> Filenames
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
