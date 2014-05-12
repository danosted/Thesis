using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

	private Dictionary<string, List<GazeEvent>> filenameToGazeEvent = new Dictionary<string, List<GazeEvent>>();
	private List<GazeEvent> gazeDataList = new List<GazeEvent>();

	private float characterCubeSize = 0.5f;
	private float gazeRayHitSphereSize = 0.25f;
	private float maxHeatMapPointSize = 2f;

	private bool isSaving;
	private bool isShowingGazeEvents;
	private bool isShowingPupilEvents;
	private bool isShowingBlinkMap;

	public float maxPupilSize = 30f;
	public float minPupilSize = 20f;
	public float minGazeDataIndex = 0f;
	public float maxGazeDataIndex = 1f;

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
		y += btnHeight + padding;
		bool showProfiler = false;
		showProfiler = GUI.Toggle(new Rect(x, y, 170, 20), showProfiler, "Show Data Collection");
		if(showProfiler)
		{
			y += btnHeight + padding;
			//TODO: Data point count
			GUI.TextArea(new Rect(x, y, 150, 20), "");
			y += btnHeight + padding;
			//TODO: add profiling
			GUI.TextArea(new Rect(x, y, 150, 20), "");
			y += btnHeight + padding;
			//TODO: add profiling
			GUI.TextArea(new Rect(x, y, 150, 20), "");
		}
	}

	//Render 3D GazeMap
	void OnDrawGizmos()
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
						for(i = (int)(minGazeDataIndex*(gazeArray.Length-1)); i < (int)(maxGazeDataIndex*(gazeArray.Length-1)); i++)
						{
							GazeEvent e = gazeArray[i];
							if(isShowingGazeEvents)
							{
								DrawGazeEvent(e, fileindex, i);

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

	private void DrawGazeEvent(GazeEvent e, int fileindex, int eventindex)
	{
		//Ray color
		Gizmos.color = eventGazeRayColors.Count > 0 ? eventGazeRayColors.ToArray()[fileindex] : Color.cyan;
		Gizmos.DrawLine(e.eventOrigin, e.eventHitPoint);
		//Hit point color
		Gizmos.color = eventHitPointColors.Count > 0 ? eventHitPointColors.ToArray()[fileindex] : Color.yellow;
		Gizmos.DrawSphere(e.eventHitPoint, gazeRayHitSphereSize);
		//Event origin color
		Gizmos.color = eventOriginColors.Count > 0 ? eventOriginColors.ToArray()[fileindex] : Color.blue;
		Gizmos.DrawCube(e.eventOrigin, Vector3.one * characterCubeSize);
		//index of event
		Handles.Label((e.eventOrigin + e.eventHitPoint) * 0.5f, (eventindex + 1).ToString() + ".");
		//Name of object that was hit
		Handles.Label(e.eventHitPoint, e.eventHitName);
	}

	public void ToggleHitmap()
	{
		isShowingGazeEvents = !isShowingGazeEvents;
	}

	public void TogglePupilMap()
	{
		isShowingPupilEvents = !isShowingPupilEvents;
	}

	public void ToggleBlinkMap()
	{
		isShowingBlinkMap = !isShowingBlinkMap;
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
			Debug.Log("filename file not found, creating new." + e);
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
				filenameToGazeEvent.Add(filename, Serializer.Instance.DeserializeHitmap(filename));
				eventOriginColors.Add(Color.blue);
				eventGazeRayColors.Add(Color.cyan);
				eventHitPointColors.Add(Color.yellow);
			}
		}
	}
	
	public void ClearLoadedData()
	{
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
			Serializer.Instance.SerializeFilenames(savedFilenames, staticfilename);
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

	public List<string> FilenamesToShow
	{
		get
		{
			return dataToCompare;
		}
	}
}
