using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExperimentSpawner : MonoBehaviour
{
	[SerializeField]
	private float
		experiementLength = 300f;
	[SerializeField]
	private float
		speed = 1f;
	[SerializeField]
	private float
		simpleSize = 1f;
	[SerializeField]
	private Color
		targetColor = Color.blue;
	[SerializeField]
	private Color
		backgroundColor = Color.grey;
	[SerializeField]
	private Transform[]
		targets;
	[SerializeField]
	private Transform
		background;

	private bool canRun;
	private bool isRunning;
	private Vector3 upperBounds;
	private Vector3 lowerBounds;

	// Use this for initialization
	void Start()
	{
		if(targets == null)
		{
			Debug.Log("Need targets to run experiements");
			canRun = false;
		}
		else
		{
			for(int i = 0; i < targets.Length; i++)
			{
				targets[i] = Instantiate(targets[i]) as Transform;
				targets[i].parent = transform;
				targets[i].gameObject.SetActive(false);
			}
			canRun = true;
		}
		background.renderer.material.color = backgroundColor;
		upperBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z - background.position.z));
		lowerBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z - background.position.z));
	}
	
	void OnGUI()
	{
		float width = 120f;
		float height = 20f;
		if(canRun)
		{
			if(GUI.Button(new Rect((Screen.width - width) * 0.5f, (Screen.height - height), width, height), "Start Experiment"))
			{
				canRun = false;
				StartCoroutine(RunExperiementFor(experiementLength));
			}

		}
		if(!canRun)
		{
			if(GUI.Button(new Rect((Screen.width - width) * 0.5f, (Screen.height - height), width, height), "Stop Experiment"))
			{
				StopAllCoroutines();
				for(int i = 0; i < targets.Length; i++)
				{
					targets[i].gameObject.SetActive(false);
				}
				canRun = true;
			}
		}
	}

	private IEnumerator RunExperiementFor(float length)
	{
		int index = 0;
		float difficulty = 1f;
		yield return StartCoroutine(RunExperimentFor(length * 0.33f, index, difficulty));
//		index++;
		difficulty++;
		yield return StartCoroutine(RunExperimentFor(length * 0.33f, index, difficulty));
//		index++;
		difficulty++;
		yield return StartCoroutine(RunExperimentFor(length * 0.33f, index, difficulty));
		canRun = true;
	}

	private IEnumerator RunExperimentFor(float seconds, int index, float difficulty)
	{
		float elapsedTime = 0f;
		float thresh = 1f;
		float step = -10f;
		Transform target = targets[index];
		Vector3 startPoint = new Vector3(lowerBounds.x, upperBounds.y * Random.value, -lowerBounds.z);
		Vector3 targetPoint = new Vector3(lowerBounds.x + step, upperBounds.y * Random.value, -lowerBounds.z);
		target.position = startPoint;
		target.gameObject.SetActive(true);
		target.transform.localScale /= difficulty;
		while(elapsedTime < seconds)
		{
			if(Mathf.Abs(upperBounds.x - target.position.x) < 0.1f)
			{
				target.position = new Vector3(lowerBounds.x, upperBounds.y * Random.value, -lowerBounds.z);
//				targetPoint = new Vector3(target.position.x + thresh, upperBounds.y * Random.value, lowerBounds.z);
				Debug.Log("resetting");
			}
			if(elapsedTime > thresh)
			{
				targetPoint = new Vector3(target.position.x + step, upperBounds.y * Random.value, -lowerBounds.z);
				thresh += elapsedTime;
//				Debug.Log("next step");
//				Debug.Log("elapsed: " + elapsedTime + " thresh: " + thresh);
			}
			target.position = Vector3.MoveTowards(target.position, targetPoint, speed * difficulty * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		target.gameObject.SetActive(false);
	}

	private Vector3 GetRandomVerticalPosition(Vector3 point)
	{
		return new Vector3(point.x, point.y * Random.value, point.z);
	}

	private Vector3 GetRandomPointOnRect(Vector3 point)
	{
		return new Vector3(point.x, point.y * Random.value, point.z);
	}

//	private Vector3 GetRandomDirectionFromTowards(Vector3 directionFrom, Vector3 directionTo)
//	{
//		Vector3 dir = directionTo;
//		float randX = Random.Range(0.5f, 1f);
//		float randY = Random.Range(0.5f, 1f);
//		dir = new Vector3()
//
//		return dir;
//	}
}