using UnityEngine;
using System.Collections;

public class CharacterGAMetrics : MonoBehaviour
{

	[SerializeField]
	private GazeThesisTest
		gazeHandler;
	[SerializeField]
	private float
		hitCheckDistance = 100f;

	private Camera characterCamera;
	private Vector3 gazePos = Vector3.zero;
	private Transform currentTarget;

	void Start()
	{
		characterCamera = GetComponentInChildren<Camera>();
//				StartCoroutine(CollectEyetrackerMetrics());
//		StartCoroutine(CollectMetrics());
//				StartCoroutine(GazeRay());

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
			Debug.DrawRay(characterCamera.transform.position, (gazePos - characterCamera.transform.position) * 20f);
			RaycastHit hit;
			Ray ray = new Ray(characterCamera.transform.position, (gazePos - characterCamera.transform.position));
			if(Physics.Raycast(ray, out hit, hitCheckDistance))
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
//		gazeHandler.transform.position = world_coord;
		
//		float pos_x = world_coord.x;
//		pos_x =  Mathf.Clamp(pos_x, -MaxVelocityChange, MaxVelocityChange);
//		transform.position = new Vector3(pos_x, transform.position.y, transform.position.z);
		while(gazeHandler)
		{

//			GA.API.Design.NewEvent("PupilDilationLeft", gazeHandler.GetPupilDilationLeft(), transform.position);
//			GA.API.Design.NewEvent("PupilDilationRight", gazeHandler.GetPupilDilationRight(), transform.position);
//			GA.API.Design.NewEvent("GazeCoordinates", gazePos);
			GA.API.Design.NewEvent("PupilDilationMean", gazeHandler.GetMeanPupilDilation(), transform.position);

			yield return new WaitForSeconds(1f);

			GA.API.Design.NewEvent("GazeCoordinates", gazePos);
			
			yield return new WaitForSeconds(1f);
		}
	}
}
