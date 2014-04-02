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
		float verticalMove = 0f;
		float horizontalMove = 0f;
		float zRot = 0f;
		while(isLooking)
		{
			horizontalMove = Input.GetAxis("Mouse X");
			verticalMove = Input.GetAxis("Mouse Y");
			zRot = (transform.rotation.z != 0f) ? transform.rotation.eulerAngles.z : 0f;
			transform.Rotate(0f, horizontalMove * mouseSensivity, -zRot);
			zRot = (mouseLookCamera.transform.rotation.z != 0f) ? mouseLookCamera.transform.rotation.eulerAngles.z : 0f;
			if((mouseLookCamera.transform.rotation.eulerAngles.x-(verticalMove * mouseSensivity)) < 85f || (mouseLookCamera.transform.rotation.eulerAngles.x-(verticalMove * mouseSensivity)) > 280f)
			{
				mouseLookCamera.transform.Rotate(-verticalMove * mouseSensivity, 0f, -zRot);
			}
			yield return null;
		}
	}


}
