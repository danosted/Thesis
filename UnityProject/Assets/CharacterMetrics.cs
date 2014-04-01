using UnityEngine;
using System.Collections;

public class CharacterMetrics : MonoBehaviour {

	[SerializeField]
	private GazeThesisTest gazeHandler;

	private Camera characterCamera;
	
	void Start () 
	{
		characterCamera = GetComponentInChildren<Camera> ();
		StartCoroutine (CollectMetrics ());
	}
	
	private IEnumerator CollectMetrics()
	{
//		Vector3 screen_coord = Input.mousePosition;
//		Vector3 world_coord = Camera.main.ScreenToWorldPoint(new Vector3(screen_coord.x, screen_coord.y, transform.position.z - Camera.main.transform.position.z));
//		gazeHandler.transform.position = world_coord;
		
//		float pos_x = world_coord.x;
//		pos_x =  Mathf.Clamp(pos_x, -MaxVelocityChange, MaxVelocityChange);
//		transform.position = new Vector3(pos_x, transform.position.y, transform.position.z);
		while (true) 
		{
			Vector3 gazePos = transform.position;
			try
			{
				gazePos = gazeHandler.GetGazeScreenPosition ();
			}
			catch(System.Exception ex)
			{
				Debug.Log(ex);
			}
			gazePos = characterCamera.ScreenToWorldPoint (new Vector3 (gazePos.x, gazePos.y, characterCamera.nearClipPlane));

			//Ray showing gaze direction
			Debug.DrawRay (characterCamera.transform.position, (gazePos - characterCamera.transform.position) * 20f);

			GA.API.Design.NewEvent("PupilDilationLeft", gazeHandler.GetPupilDilationLeft(), transform.position);
			GA.API.Design.NewEvent("PupilDilationRight", gazeHandler.GetPupilDilationRight(), transform.position);
			GA.API.Design.NewEvent("GazeCoordinates", gazePos);


			yield return null;
		}
	}
}
