using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(TETGazeTrackerData))]
public class GazeCalculator : MonoBehaviour
{

	public delegate void OnGazeObjectHitDelegate(Object obj);
	public event OnGazeObjectHitDelegate OnGazeObjectHit;

	[SerializeField]
	private float
		hitRayMaxDistance = 100f;
	[SerializeField]
	private Color
		rayColor = Color.cyan;
	[SerializeField]
	private Transform
		target;
	[SerializeField]
	private Camera
		gazeCamera;

	#region debug
	[SerializeField]
	private bool
		showLiveGazeDebug;
	#endregion

	private TETGazeTrackerData gazeData;

	private Vector3 gazePosWorld = Vector3.zero;
	private Vector3 gazePosScreen = Vector3.zero;
	private Vector3 gazeHitPoint;
	private Vector3 upperBounds;
	private Vector3 lowerBounds;

	private Transform currentTarget;

	private Ray gazeRay;

	private bool isHit;
	private bool mouseAsGaze;

	private float pupilSize;
	private float currentFixationLength;
	private float lastFixationLength;

	void Start()
	{
		//Don't find another camera if it is already assigned
		gazeCamera = gazeCamera ? gazeCamera : GetComponentInChildren<Camera>();
		gazeData = GetComponent<TETGazeTrackerData>();
		//Report if no camera assigned in inspector and none found in prefab
		if(!gazeCamera)
		{
			Debug.LogError("No camera found for gaze tracking", gameObject);
		}
		else
		{

			if(gazeData.TrackerIsActive)
			{
				StartCoroutine(CalculateEyeData());
			}
			else
			{
				Debug.Log("Tracker not connected, switching to mouse");
				mouseAsGaze = true;
			}
			StartCoroutine(CalculateGazeRay());
		}
		upperBounds = gazeCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, gazeCamera.farClipPlane));
		lowerBounds = gazeCamera.ScreenToWorldPoint(new Vector3(0, 0, gazeCamera.farClipPlane));
	}

	/*
	 *	Live Editor Debugging
	 */
	void OnDrawGizmos()
	{
		if(showLiveGazeDebug)
		{
			if(isHit)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(gazeHitPoint, 0.25f);
			}
			Gizmos.color = Color.cyan;
			Gizmos.DrawRay(gazeRay.origin, gazeRay.direction * hitRayMaxDistance);
		}
	}

	private IEnumerator CalculateGazeRay()
	{
		while(true)
		{
			//Gaze Ray
			gazeRay = mouseAsGaze ? gazeCamera.ScreenPointToRay(Input.mousePosition) : gazeCamera.ScreenPointToRay(gazeData.GetGazeScreenPosition());
			RaycastHit hit;
			if(Physics.Raycast(gazeRay, out hit, hitRayMaxDistance))
			{
				currentTarget = hit.transform;
				gazeHitPoint = hit.point;
				if(OnGazeObjectHit != null)
				{
					OnGazeObjectHit(hit.transform);
				}
				isHit = true;
			}
			else
			{
				currentTarget = null;
				gazeHitPoint = gazeRay.direction * hitRayMaxDistance;
				isHit = false;
			}
			//Fixation
			currentFixationLength = gazeData.GetCurrentFixationLength();
			lastFixationLength = gazeData.GetLastFixationLength();
			//Live Gaze Target
			if(showLiveGazeDebug)
			{
				target.position = gazeHitPoint;
				float width, height;
				width = Mathf.Abs(upperBounds.x - lowerBounds.x);
				height = Mathf.Abs(upperBounds.y - lowerBounds.y);
				if(width > height)
				{
					float size = height * 0.01f;
					target.localScale = new Vector3(size, size, size);
				}
				else
				{
					float size = width * 0.01f;
					target.localScale = new Vector3(size, size, size);
				}
				target.gameObject.SetActive(true);
			}
			else
			{
				target.gameObject.SetActive(false);
			}

			yield return null;
		}
	}

	private IEnumerator CalculateEyeData()
	{
		while(true)
		{
			pupilSize = gazeData.GetMeanPupilDilation();

			try
			{
//				Debug.Log("TimeSinceLastBlink: ");
//				Debug.Log(gazeHandler.GetTimeSinceLastBlink());
			}
			catch(System.Exception e)
			{
				Debug.Log(e, gameObject);
			}
			yield return null;
		}
	}

	public string GetCurrentTargetObjectPath()
	{
		string path = "";
		if(currentTarget)
		{
			Transform parentObject = (Transform)PrefabUtility.GetPrefabParent(currentTarget);
			if(!parentObject)
			{
				GazePrefabTracker gpt = currentTarget.GetComponent<GazePrefabTracker>();
				if(gpt)
				{
					path = gpt.GetAssetPath();
				}
				else
				{
					Debug.Log("Add the GazePrefabTracker script to Instantiated objects.");
				}
//				return AssetDatabase.GetAssetPath(currentTarget);
			}
			else
			{
				path = AssetDatabase.GetAssetPath(parentObject);
				Debug.Log(path);
				if(path != "")
				{
					return path;
				}
			}

		}
		return path;
	}

	public Ray GetCurrentGazeRay()
	{
		return this.gazeRay;
	}

	public string GetCurrentTargetName()
	{
		if(!currentTarget)
		{
			return "N/A";
		}
		else
		{
			return currentTarget.name;
		}
	}

	public Vector3 GetCurrentTargetPosition()
	{
		if(!currentTarget)
		{
			return Vector3.zero;
		}
		else
		{
			return currentTarget.position;
		}
	}

	public Quaternion GetCurrentTargetRotation()
	{
		if(!currentTarget)
		{
			return Quaternion.identity;
		}
		else
		{
			return currentTarget.rotation;
		}
	}

	public Vector3 GetCurrentTargetScale()
	{
		if(!currentTarget)
		{
			return Vector3.zero;
		}
		else
		{
			return currentTarget.localScale;
		}
	}

	public Vector3 GetCurrentHitPosition()
	{
		return this.gazeHitPoint;
	}

	public float PupilSize
	{
		get
		{
			return pupilSize;
		}
	}

	public float CurrentFixationLength {
		get {
			return currentFixationLength;
		}
	}

	public float LastFixationLength {
		get {
			return lastFixationLength;
		}
	}
}
