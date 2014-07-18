using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(TETGazeTrackerData))]
public class GazeCalculator : MonoBehaviour
{

	public delegate void OnGazeObjectHitDelegate(Transform hit);
	public event OnGazeObjectHitDelegate OnGazeObjectHit;

	[SerializeField]
	private float
		hitRayMaxDistance = 100f;
	[SerializeField]
	private float
		hitRaySearchRadius = 1f;
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

	private Vector3 gazeHitPoint;

	private Transform currentTarget;

	private Ray gazeRay;

	private bool isHit;
	private bool mouseAsGaze;

	private float pupilSize;
	private float currentFixationLength;
	private float lastFixationLength;
	private float currentEyesClosedtime;
	private float currentTime;

	private int fixationIndex;
	private int blinkCount;

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
//			if(mouseAsGaze)
//			{
//				gazeRay = gazeCamera.ScreenPointToRay(Input.mousePosition);
//			}
//			else
//			{
//				gazeRay = HandleUtility.GUIPointToWorldRay(new Vector2(gazeData.GetGazeScreenPosition().x, gazeData.GetGazeScreenPosition().y));
//			}
			currentTime = Time.time;
			gazeRay = mouseAsGaze ? gazeCamera.ScreenPointToRay(Input.mousePosition) : gazeCamera.ScreenPointToRay(gazeData.GetGazeScreenPosition());
//			System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
//			Vector2 pos = new Vector2(EditorWindow.GetWindow(T).position.x,EditorWindow.GetWindow(T).position.y);
//			Debug.Log("windowPos: " + pos.x + "," + pos.y);
//			RaycastHit hit;
			RaycastHit[] hits;
			hits = Physics.SphereCastAll(gazeRay, hitRaySearchRadius);
			if(hits.Length > 0)
			{
				foreach(RaycastHit hit in hits)
				{
					if(hit.transform.GetComponent<GazePrefabTracker>() && (gazeData.isFixating() || mouseAsGaze))
					{
//						Debug.Log("hit: " + hit.transform.name);
						currentTarget = hit.transform;
						gazeHitPoint = hit.point;
						if(OnGazeObjectHit != null)
						{
							OnGazeObjectHit(hit.transform);
						}
						isHit = true;
						break;
					}
					else
					{
//						Debug.Log("hit: " + hit.transform.name);
						currentTarget = null;
						gazeHitPoint = hit.point;
						isHit = true;
					}
				}
			}
			else
			{
				currentTarget = null;
				gazeHitPoint = gazeRay.direction * hitRayMaxDistance;
				isHit = false;
			}
			//Fixation
            if(mouseAsGaze)
            {
                currentFixationLength = Random.RandomRange(0f, 4f);
                fixationIndex = fixationIndex + Random.Range(0, 1);
                lastFixationLength = Random.RandomRange(0f, 5f);
            }
            else
            {
                currentFixationLength = gazeData.GetCurrentFixationLength();
                fixationIndex = gazeData.GetFixationIndex();
                lastFixationLength = gazeData.GetLastFixationLength();
            }
			
			//Live Gaze Target
			if(showLiveGazeDebug)
			{
				target.position = gazeHitPoint;
                target.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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
			currentEyesClosedtime = gazeData.GetEyesClosedTime();
			blinkCount = gazeData.GetBlinkCount();
			yield return null;
		}
	}


	public string GetCurrentTargetObjectPath()
	{
		string path = "";
		if(currentTarget)
		{
#if UNITY_EDITOR
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
				if(path != "")
				{
					return path;
				}

			}
#else
			GazePrefabTracker gpt = currentTarget.GetComponent<GazePrefabTracker>();
			if(gpt)
			{
				path = gpt.GetAssetPath();
			}
			else
			{
				Debug.Log("Add the GazePrefabTracker script to Instantiated objects.");
			}
#endif
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
			return "";
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

	public Color GetCurrentTargetColor()
	{
		if(this.currentTarget)
		{
			if(this.currentTarget.GetComponent<Renderer>())
			{
				return this.currentTarget.renderer.material.color;
			}
		}
		return Color.white;
	}
	
	public float PupilSize
	{
		get
		{
			return pupilSize;
		}
	}

	public float CurrentFixationLength
	{
		get
		{
			return currentFixationLength;
		}
	}

	public float LastFixationLength
	{
		get
		{
			return lastFixationLength;
		}
	}

	public int FixationIndex {
		get {
			return fixationIndex;
		}
	}

	public float CurrentTime {
		get {
			return currentTime;
		}
	}

	public float CurrentEyesClosedTime {
		get {
			return currentEyesClosedtime;
		}
	}

	public int BlinkCount {
		get {
			return blinkCount;
		}
	}
}
