﻿using UnityEngine;
using System.Collections;

public class CharacterMovementScript : MonoBehaviour
{

	[SerializeField]
	private float
		movementSpeed = 1f;
	[SerializeField]
	private float
		resetBounds = -10f;
	[SerializeField]
	private Transform
		rhs;
	[SerializeField]
	private Transform
		lhs;
	[SerializeField]
	private float
		jumpHeight = 5f;
	
	private KeyInput2Object keyInput;	
	private float checkFrequency = 1f;
	private bool goingLeft;
	private bool goingRight;
	private bool goingForward;
	private bool goingBackward;

	void Awake()
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
			transform.position = Vector3.MoveTowards(transform.position, transform.position + (lhs.position - transform.position), Time.deltaTime * movementSpeed);
			yield return null;
		}
	}

	private IEnumerator StartMoveRight()
	{
		while(goingRight)
		{
			transform.position = Vector3.MoveTowards(transform.position, transform.position + (rhs.position - transform.position), Time.deltaTime * movementSpeed);
			yield return null;
		}
	}

	private IEnumerator StartMoveForward()
	{
		while(goingForward)
		{
			transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * movementSpeed);
			yield return null;
		}
	}

	private IEnumerator StartMoveBackward()
	{
		while(goingBackward)
		{
			transform.position = Vector3.MoveTowards(transform.position, transform.position - transform.forward, Time.deltaTime * movementSpeed);
			yield return null;
		}
	}

	private IEnumerator Jump()
	{
		int duration = 100;
		float jumpSign = 1f;

		while(duration > -100)
		{
			jumpSign = Mathf.Clamp(duration, -jumpHeight, jumpHeight);
			transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * jumpSign, Time.deltaTime);
			duration--;
			yield return null;
		}
		OnLanded();
	}

}
