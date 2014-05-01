using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode]
public class HitmapManager : MonoBehaviour
{
	private GazeMetricEvents data = GazeMetricEvents.Instance;

	[SerializeField]
	private List<HitmapEvent>
		gazeTargetData;
	[SerializeField]
	private List<EyeEvent>
		eyeData;
	[SerializeField]
	private Color
		pupilColor = Color.yellow;

	private float characterCubeSize = 0.5f;
	private float gazeRayHitSphereSize = 0.25f;
	private float maxHeatMapPointSize = 2f;

	private bool isRunning;
	private bool isShowingHitMap;
	private bool isShowingPupilMap;
	private bool isShowingBlinkMap;

	public float maxPupilSize = 30f;
	public float minPupilSize = 20f;

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
			CreateSaveFiles();
		}
		y += btnHeight + padding;
		bool showProfiler = false;
		showProfiler = GUI.Toggle(new Rect(x, y, 170, 20), showProfiler, "Toggle Data Collection Profiler");
		if(showProfiler)
		{
			y += btnHeight + padding;
			//TODO: Data point count
			GUI.TextArea(new Rect(x, y, 150, 20), "");
			y += btnHeight + padding;
			//TODO: 
			GUI.TextArea(new Rect(x, y, 150, 20), "");
			y += btnHeight + padding;
			//TODO:
			GUI.TextArea(new Rect(x, y, 150, 20), "");
		}
	}

	void OnDrawGizmos()
	{
		if(isShowingHitMap)
		{
			foreach(HitmapEvent e in gazeTargetData)
			{
//				Gizmos.color = Color.yellow;
//				Gizmos.DrawCube(e.eventOrigin, Vector3.one * characterCubeSize);
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(e.eventOrigin, e.eventHitPoint);
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(e.eventHitPoint, gazeRayHitSphereSize);
			}
		}
		if(isShowingPupilMap || isShowingBlinkMap)
		{
			foreach(EyeEvent e in eyeData)
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
		isShowingHitMap = !isShowingHitMap;
	}

	public void TogglePupilMap()
	{
		isShowingPupilMap = !isShowingPupilMap;
	}

	public void ToggleBlinkMap()
	{
		isShowingBlinkMap = !isShowingBlinkMap;
	}

	public void CreateSaveFiles()
	{
		if(!isRunning)
		{
			isRunning = true;
			StartCoroutine(CreateNewSaveFiles());
		}
		else
		{
			Debug.Log("Still creating hitmap", gameObject);
		}
	}

	private IEnumerator CreateNewSaveFiles()
	{
		if(gazeTargetData == null)
		{
			gazeTargetData = new List<HitmapEvent>();
		}
		else
		{
			gazeTargetData.Clear();
		}
		if(eyeData == null)
		{
			eyeData = new List<EyeEvent>();
		}
		else
		{
			eyeData.Clear();
		}
//		foreach(HitmapEvent e in data.HitmapDataSet)
//		{
//			gazeTargetData.Add(e);
//		}
//		foreach(EyeEvent e in data.EyeDataSet)
//		{
//			eyeData.Add(e);
//		}
		gazeTargetData = data.HitmapDataSet;
		eyeData = data.EyeDataSet;

		SaveData();
		isRunning = false;
		yield return null;
	}

	public void SaveData()
	{
		Serializer.Instance.SerializeHitmap(gazeTargetData);
		Serializer.Instance.SerializeEyedata(eyeData);
		Debug.Log("Data has been saved!", gameObject);
	}

	public void LoadData()
	{
		gazeTargetData = Serializer.Instance.DeserializeHitmap();
		eyeData = Serializer.Instance.DeserializeEyedata();
	}

}
