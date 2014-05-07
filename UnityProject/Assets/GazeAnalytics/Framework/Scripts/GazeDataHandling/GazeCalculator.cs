using UnityEngine;
using System.Collections;

public class GazeCalculator : MonoBehaviour
{

	[SerializeField]
	private TETGazeTrackerData
		gazeData;
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
		mouseAsGaze;
	[SerializeField]
	private bool
		showLiveGazeDebug;
	#endregion

	private Vector3 gazePosWorld = Vector3.zero;
	private Vector3 gazePosScreen = Vector3.zero;
	private Transform currentTarget;
	private Ray gazeRay;
	private bool isHit;
	private Vector3 poi;
	private float pupilSize;

	void Start()
	{
		gazeCamera = GetComponentInChildren<Camera>();
		if(mouseAsGaze)
		{
			StartCoroutine(CalculateGazeRayWithMouse());
		}
		else
		{
			StartCoroutine(CalculateGazeRay());
			StartCoroutine(CalculateEyeData());
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
				Gizmos.DrawSphere(poi, 0.5f);
			}
			Gizmos.color = Color.cyan;
			Gizmos.DrawRay(gazeRay.origin, gazeRay.direction * hitRayMaxDistance);
		}
	}

	private IEnumerator CalculateGazeRayWithMouse()
	{
		while(true)
		{
			gazePosWorld = transform.position;
//			Vector3 gazePosOrigin = characterCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
			Vector3 gazePosOrigin = gazeCamera.transform.position;
			gazePosWorld = gazeCamera.ScreenToWorldPoint(new Vector3(Mathf.Clamp((int)Input.mousePosition.x, 0, Screen.width), Mathf.Clamp((int)Input.mousePosition.y, 0, Screen.height), gazeCamera.farClipPlane));

			//Ray showing gaze direction
			RaycastHit hit;
			gazeRay = new Ray(gazePosOrigin, gazePosWorld);
//			Debug.DrawRay(gazeRay.origin, gazeRay.direction * hitRayMaxDistance, rayColor);
			if(Physics.Raycast(gazeRay, out hit, hitRayMaxDistance))
			{
				currentTarget = hit.transform;
				Debug.Log("hit something: " + currentTarget.name, currentTarget.gameObject);
				poi = hit.point;
				isHit = true;

			}
			else
			{
				currentTarget = null;
				poi = gazeRay.direction * hitRayMaxDistance;
				isHit = false;
//				Debug.Log("staring into infinity and beyond");
			}
			
			yield return null;
		}
	}

	private IEnumerator CalculateGazeRay()
	{
		while(true)
		{
			gazePosWorld = transform.position;
			Vector3 eyescreenpos = gazeData.GetGazeScreenPosition();
//			gazePosScreen = new Vector3(Mathf.Clamp((int)eyescreenpos.x, 0, Screen.width), Mathf.Clamp((int)eyescreenpos.y, 0, Screen.height), characterCamera.farClipPlane);
			gazePosScreen = eyescreenpos;
			Vector3 gazePosOrigin = gazeCamera.transform.position;
			gazePosWorld = gazeCamera.ScreenToWorldPoint(new Vector3(gazePosScreen.x, gazePosScreen.y, gazeCamera.farClipPlane));

			//Ray showing gaze direction
			RaycastHit hit;
			gazeRay = new Ray(gazePosOrigin, gazePosWorld);
			Debug.DrawRay(gazeRay.origin, gazeRay.direction * hitRayMaxDistance, rayColor);

			//Ray Cast Gaze Ray
			if(Physics.Raycast(gazeRay, out hit, hitRayMaxDistance))
			{
				currentTarget = hit.transform;
				Debug.Log("hit something: " + currentTarget.name, currentTarget.gameObject);
				poi = hit.point;
				isHit = true;
			}
			else
			{
				currentTarget = null;
				poi = gazeRay.direction * hitRayMaxDistance;
				isHit = false;
			}
			//Live Gaze Target
			target.position = poi;
			target.gameObject.SetActive(showLiveGazeDebug);
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

	public Ray GetCurrentGazeRay()
	{
		return this.gazeRay;
	}

	public string GetCurrentTargetName()
	{
		if(!currentTarget)
		{
			return "NAN";
		}
		else
		{
			return currentTarget.name;
		}
	}

	public Vector3 GetCurrentHitPosition()
	{
		return this.poi;
	}

	public float PupilSize
	{
		get
		{
			return pupilSize;
		}
	}
}
