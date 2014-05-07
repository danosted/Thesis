using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode]
public class GazeMapManager : MonoBehaviour
{
	private GazeMetricEvents data = GazeMetricEvents.Instance;

	[SerializeField]
	private List<GazeEvent>
		gazeDataList;
	[SerializeField]
	private Color
		pupilColor = Color.yellow;
	[SerializeField]
	private string
		staticfilename = "static.xml";
	[SerializeField]
	private List<string>
		filenames = new List<string>();

	private float characterCubeSize = 0.5f;
	private float gazeRayHitSphereSize = 0.25f;
	private float maxHeatMapPointSize = 2f;

	private bool isSaving;
	private bool isShowingGazeMap;
	private bool isShowingPupilMap;
	private bool isShowingBlinkMap;

	public float maxPupilSize = 30f;
	public float minPupilSize = 20f;

	public float minGazeDataIndex = 0;
	public float maxGazeDataIndex = 0;

//	void Update()
//	{
//		if(Input.GetKeyDown(KeyCode.Space) && Application.isPlaying)
//		{
//			CreateSaveFiles();
//		}
//	}

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
		if(isShowingGazeMap || isShowingPupilMap || isShowingBlinkMap)
		{
			GazeEvent[] gazeArray = gazeDataList.ToArray();
			for(int i = (int)minGazeDataIndex; i < (int)maxGazeDataIndex; i++)
			{
				if(isShowingGazeMap && gazeArray[i].eventName.Equals(Constants.GazeEvent))
				{
					Gizmos.color = Color.cyan;
					Gizmos.DrawLine(gazeArray[i].eventOrigin, gazeArray[i].eventHitPoint);
					Gizmos.color = Color.red;
					Gizmos.DrawSphere(gazeArray[i].eventHitPoint, gazeRayHitSphereSize);
					Handles.Label(gazeArray[i].eventHitPoint, (i + 1).ToString() + ". " + gazeArray[i].eventHitName);
					Gizmos.color = Color.yellow;
					Gizmos.DrawCube(gazeArray[i].eventOrigin, Vector3.one * characterCubeSize);
				}
				if(isShowingPupilMap && gazeArray[i].eventName.Equals(Constants.PupilEvent))
				{
					float pupilSize = gazeArray[i].pupilMeanSize;
					//TODO: Find the right min and max for pupil size
					float map = Mathf.InverseLerp(minPupilSize, maxPupilSize, pupilSize);
					Gizmos.color = new Color(pupilColor.r * map, pupilColor.g * map, pupilColor.b * map, 0.8f);
					Gizmos.DrawSphere(gazeArray[i].eventOrigin, maxHeatMapPointSize * (map + 0.1f));
				}
				if(isShowingBlinkMap && gazeArray[i].eventName.Equals(Constants.BlinkEvent))
				{

				}
			}
		}
		if(isShowingPupilMap || isShowingBlinkMap)
		{
			foreach(GazeEvent e in gazeDataList)
			{
				if(isShowingPupilMap)
				{
					float pupilSize = e.pupilMeanSize;
					//TODO: Find the right min and max for pupil size
					float map = Mathf.InverseLerp(minPupilSize, maxPupilSize, pupilSize);
					Gizmos.color = new Color(pupilColor.r * map, pupilColor.g * map, pupilColor.b * map, 0.8f);
					Gizmos.DrawSphere(e.eventOrigin, maxHeatMapPointSize * (map + 0.1f));
				}
//				if(isShowingBlinkMap)
//				{
//					Gizmos.color = Color.red;
//					Gizmos.DrawSphere(e.eventOrigin, gazeRayHitSphereSize);
//				}
//				Gizmos.color = Color.yellow;
//				Gizmos.DrawCube(e.eventOrigin, Vector3.one * characterCubeSize);
//				Gizmos.color = Color.cyan;
//				Gizmos.DrawLine(e.eventOrigin, e.eventHitPosition);
//				Gizmos.color = Color.red;
//				Gizmos.DrawSphere(e.eventHitPosition, gazeRayHitSphereSize);
			}
		}
		//		Gizmos.color = Color.yellow;
		//		Gizmos.DrawSphere(gazePosOrigin, 0.5f);
	}

	public void ToggleHitmap()
	{
		if(gazeDataList.Count > 0)
		{
			isShowingGazeMap = !isShowingGazeMap;
		}
		else
		{
			isShowingGazeMap = false;
		}
	}

	public void TogglePupilMap()
	{
		isShowingPupilMap = !isShowingPupilMap;
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
		if(gazeDataList == null)
		{
			gazeDataList = new List<GazeEvent>();
		}

		gazeDataList = data.GazeDataList;
		SaveData(createFilename());
		isSaving = false;
		yield return null;
	}

	public void LoadFilesOnDisk()
	{
		filenames = Serializer.Instance.DeserializeFilenames(staticfilename);
	}

	public void SaveData(string filename)
	{
		Debug.Log(filename);
		Serializer.Instance.SerializeHitmap(gazeDataList, filename);
		try
		{
			filenames = Serializer.Instance.DeserializeFilenames(staticfilename);
		}
		catch(System.Exception e)
		{
			Debug.Log("filename file not found, creating new." + e);
		}
		filenames.Add(filename);
		Serializer.Instance.SerializeFilenames(filenames, staticfilename);
		Debug.Log("Data has been saved to file: " + filename, gameObject);
	}

	public void LoadData(string filename)
	{
		gazeDataList = Serializer.Instance.DeserializeHitmap(filename);
	}

	public void ClearLoadedData()
	{
		gazeDataList.Clear();
	}

	public void DeleteSaveFile(string filename)
	{
		if(FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/" + filename))
		{
			Debug.Log(filename + " deleted!");
			filenames.Remove(filename);
			Serializer.Instance.SerializeFilenames(filenames, staticfilename);
		}
		else
		{
			Debug.Log(filename + " not found.");
			Serializer.Instance.SerializeFilenames(filenames, staticfilename);
		}
	}

	public void DeleteAllSaveFiles()
	{
		if(filenames.Count > 0)
		{
			foreach(string filename in filenames)
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
			filenames.Clear();
			Serializer.Instance.SerializeFilenames(filenames, staticfilename);
		}
		else
		{
			Debug.Log("no files found");
		}
	}

	private string createFilename()
	{
		System.DateTime date = System.DateTime.Now;
		string filename = Application.loadedLevelName + "_" + 
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
			return filenames;
		}
	}

	public List<GazeEvent> GazeDataList
	{
		get
		{
			return gazeDataList;
		}
	}
}
