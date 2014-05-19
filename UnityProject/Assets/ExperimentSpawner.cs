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
		if(GUI.Button(new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height), "Start Experiment") && canRun)
		{
			canRun = false;
			StartCoroutine(RunExperiementFor(experiementLength));
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
		Transform target = targets[index];
		target.position = lowerBounds;
		target.gameObject.SetActive(true);
		target.transform.localScale /= difficulty;
		while(elapsedTime < seconds)
		{
			if(Vector3.Distance(target.position, upperBounds) < 0.1f)
			{
				target.position = lowerBounds;
			}
			target.position = Vector3.MoveTowards(target.position, upperBounds, speed * difficulty * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		target.gameObject.SetActive(false);
	}
}