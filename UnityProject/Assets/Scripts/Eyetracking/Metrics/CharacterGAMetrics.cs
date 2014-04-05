using UnityEngine;
using System.Collections;

public class CharacterGAMetrics : MonoBehaviour
{

	[SerializeField]
	private GazeThesisTest
		gazeHandler;
	[SerializeField]
	private float
		hitRayMaxDistance = 100f;
	[SerializeField]
	private float
		minPupilDilation = 15f;
	[SerializeField]
	private Color
		rayColor;

	private Camera characterCamera;
	private Vector3 gazePos = Vector3.zero;
	private Transform currentTarget;
	private Ray gazeRay;

	void Start()
	{
		characterCamera = GetComponentInChildren<Camera>();
//		StartCoroutine(CollectEyetrackerMetrics());
//		StartCoroutine(CollectMetrics());
//		StartCoroutine(GazeRay());
		StartCoroutine(DebugGazeRay());

	}

	private IEnumerator DebugGazeRay()
	{
		while(true)
		{
			gazePos = transform.position;
			try
			{
				gazePos = gazeHandler.GetGazeScreenPosition();
			}
			catch(System.Exception ex)
			{
				Debug.Log(ex);
			}
			gazePos = characterCamera.ScreenToWorldPoint(new Vector3(gazePos.x, gazePos.y, characterCamera.nearClipPlane));
			
			//Ray showing gaze direction
			RaycastHit hit;
			gazeRay = new Ray(characterCamera.transform.position, characterCamera.transform.forward);
			Debug.DrawRay(gazeRay.origin, gazeRay.direction * hitRayMaxDistance, rayColor);
			if(Physics.Raycast(gazeRay, out hit, hitRayMaxDistance))
			{
				currentTarget = hit.transform;
				Debug.Log("hit something: " + currentTarget.name, currentTarget.gameObject);
			}
			else
			{
				currentTarget = null;
//				Debug.Log("staring into infinity and beyond");
			}
			
			yield return null;
		}
	}

	private IEnumerator GazeRay()
	{
		while(true)
		{
			gazePos = transform.position;
			try
			{
				gazePos = gazeHandler.GetGazeScreenPosition();
			}
			catch(System.Exception ex)
			{
				Debug.Log(ex);
			}
			gazePos = characterCamera.ScreenToWorldPoint(new Vector3(gazePos.x, gazePos.y, characterCamera.nearClipPlane));

			//Ray showing gaze direction
			RaycastHit hit;
			gazeRay = new Ray(characterCamera.transform.position, (gazePos - characterCamera.transform.position));
			Debug.DrawRay(gazeRay.origin, gazeRay.direction * hitRayMaxDistance, rayColor);
			if(Physics.Raycast(gazeRay, out hit, hitRayMaxDistance))
			{
				currentTarget = hit.transform;
				Debug.Log("hit something: " + currentTarget.name, currentTarget.gameObject);
			}
			else
			{
				currentTarget = null;
				Debug.Log("staring into infinity and beyond");
			}

			yield return null;
		}
	}

	private IEnumerator CollectMetrics()
	{
		while(true)
		{
			GA.API.Design.NewEvent("RandomTestPosition", Random.Range(20, 24), transform.position);
			
			yield return new WaitForSeconds(1f);
		}
	}

	private IEnumerator CollectEyetrackerMetrics()
	{
//		Vector3 screen_coord = Input.mousePosition;
//		Vector3 world_coord = Camera.main.ScreenToWorldPoint(new Vector3(screen_coord.x, screen_coord.y, transform.position.z - Camera.main.transform.position.z));
//		gazeHandler.transform.position = world_coord
//		float pos_x = world_coord.x;
//		pos_x =  Mathf.Clamp(pos_x, -MaxVelocityChange, MaxVelocityChange);
//		transform.position = new Vector3(pos_x, transform.position.y, transform.position.z);
		while(gazeHandler)
		{
			float pupilDilation = gazeHandler.GetMeanPupilDilation();
			if(pupilDilation > minPupilDilation)
			{
				GA.API.Design.NewEvent("PupilDilationMean", pupilDilation, transform.position);
				Debug.Log("Pupil Dilation Current: " + pupilDilation);
			}

			yield return new WaitForSeconds(1f);

			GA.API.Design.NewEvent("GazeCoordinates", gazePos);
			
			yield return new WaitForSeconds(1f);
		}
	}

	public Ray GetCurrentGazeRay()
	{
		return this.gazeRay;
	}

	public GameObject GetCurrentTarget()
	{
		if(!currentTarget)
		{
			return null;
		}
		return this.currentTarget.gameObject;
	}
}
