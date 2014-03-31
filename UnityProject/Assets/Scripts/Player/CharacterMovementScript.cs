using UnityEngine;
using System.Collections;

public class CharacterMovementScript : MonoBehaviour {

	[SerializeField]
	private float movementSpeed = 1f;
	[SerializeField]
	private float jumpForce = 1f;
	[SerializeField]
	private float resetBounds = -10f;
	
	private KeyInput2Object keyInput;	
	private float checkFrequency = 1f;
	private bool goingLeft;
	private bool goingRight;
	private bool goingForward;
	private bool goingBackward;

	void Awake () 
	{
		keyInput = GetComponent<KeyInput2Object>();
		keyInput.OnLeftPressed += OnLeftPressed;
		keyInput.OnLeftReleased += OnLeftReleased;
		keyInput.OnRightPressed += OnRightPressed;
		keyInput.OnRightReleased += OnRightReleased;
		keyInput.OnUpPressed += OnUpPressed;
		keyInput.OnUpReleased += OnUpReleased;
		keyInput.OnDownPressed += OnDownPressed;
		keyInput.OnDownReleased += OnDownReleased;
		keyInput.OnSpacePressed += OnJumpPressed;
		StartCoroutine(ResetPosition());
	}

	private void OnLeftPressed()
	{
		keyInput.OnLeftPressed -= OnLeftPressed;
		goingLeft = true;
		StartCoroutine(StartMoveLeft());
	}

	private void OnLeftReleased()
	{
		goingLeft = false;
		keyInput.OnLeftPressed += OnLeftPressed;
	}

	private void OnRightPressed()
	{
		keyInput.OnRightPressed -= OnRightPressed;
		goingRight = true;
		StartCoroutine(StartMoveRight());
	}
	
	private void OnRightReleased()
	{
		keyInput.OnRightPressed += OnRightPressed;
		goingRight = false;
	}

	private void OnUpPressed()
	{
		keyInput.OnUpPressed -= OnUpPressed;
		goingForward = true;
		StartCoroutine(StartMoveForward());
	}
	
	private void OnUpReleased()
	{
		goingForward = false;
		keyInput.OnUpPressed += OnUpPressed;
	}
	
	private void OnDownPressed()
	{
		keyInput.OnDownPressed -= OnDownPressed;
		goingBackward = true;
		StartCoroutine(StartMoveBackward());
	}
	
	private void OnDownReleased()
	{
		keyInput.OnDownPressed += OnDownPressed;
		goingBackward = false;
	}

	private void OnJumpPressed()
	{
		keyInput.OnSpacePressed -= OnJumpPressed;
		StartCoroutine(Jump());
	}

	private void OnLanded()
	{
		keyInput.OnSpacePressed += OnJumpPressed;
	}

	private IEnumerator StartMoveLeft()
	{
		while(goingLeft)
		{
			transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.left * movementSpeed, Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator StartMoveRight()
	{
		while(goingRight)
		{
			transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right * movementSpeed, Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator StartMoveForward()
	{
		while(goingForward)
		{
			transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.forward * movementSpeed, Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator StartMoveBackward()
	{
		while(goingBackward)
		{
			transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.back * movementSpeed, Time.deltaTime);
			yield return null;
		}
	}

	private IEnumerator Jump()
	{
		int duration = 100;
		float jumpSign = 1f;

		while(duration > -100)
		{
			jumpSign = Mathf.Clamp(duration, -1, 1);
			transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * jumpSign, Time.deltaTime);
			duration--;
			yield return null;
		}
		OnLanded();
	}

	private IEnumerator ResetPosition()
	{
		while(true)
		{
			if(transform.position.y < resetBounds)
			{
				rigidbody2D.velocity = Vector2.zero;
				rigidbody2D.Sleep();
				transform.position = Constants.CharacterStartPosition;
				OnLanded();
			}
			yield return new WaitForSeconds(checkFrequency);
		}
	}
	
//	Fake physics jump
//	private IEnumerator Jump()
//	{
//		Vector2 origin = new Vector2(transform.position.x, transform.position.y);
//		float speed = airSpeedMax;
//		while(speed > airSpeedMin)
//		{
//			speed = (speed - 1f/gravityMagnitude);
//			transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0f, speed, 0f), Time.deltaTime);
//			yield return null;
//		}
//		while(transform.position.y > origin.y)
//		{
//			speed = (speed + 1f/gravityMagnitude);
//			transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0f, -speed, 0f), Time.deltaTime);
//			yield return null;
//		}
//		transform.position = new Vector3(transform.position.x,origin.y, transform.position.z);
//		OnLanded();
//	}

//	private IEnumerator Fall()
//	{
//		Vector3 origin = transform.position;
//		float height = transform.position.y + jumpHeight;
//		while(transform.position.y != height)
//		{
//			Vector3.MoveTowards(transform.position, transform.position + new Vector3(0f, height, 0f), jumpSpeed);
//			yield return WaitForSeconds(0.02f);
//		}
//	}

	private void Crouch()
	{
		
	}
}
