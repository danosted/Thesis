using UnityEngine;
using System.Collections;

public class MouseCameraLook : MonoBehaviour {

	[SerializeField]
	private Camera mouseLookCamera;
	[SerializeField]
	private float mouseSensivity;
	[SerializeField]
	private float thresh;

	private bool isLooking;

	void Start()
	{
		if(!mouseLookCamera)
		{
			try
			{
				mouseLookCamera = transform.GetComponentInChildren<Camera>();
			}
			catch(System.Exception ex)
			{
				Debug.Log("no camera: " + ex);
			}
		}
		Initialize();
	}

	private void Initialize()
	{
		isLooking = true;
		StartCoroutine(MouseOrbit());
	}

	private IEnumerator MouseOrbit()
	{
//		Input.
		float verticalAxis = 0f;
		float horizontalAxis = 0f;
		while(isLooking)
		{
			verticalAxis = Input.GetAxis("Mouse X");
			horizontalAxis = Input.GetAxis("Mouse Y");
			transform.Rotate(-horizontalAxis * mouseSensivity, verticalAxis * mouseSensivity, 0f);
			yield return null;
		}
	}


}
