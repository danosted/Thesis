using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ExperimentSpawner : MonoBehaviour
{
	[SerializeField]
	private float
		experiementLength = 60f;
	[SerializeField]
	private bool contrastExperiment = true;
	[SerializeField]
	private bool sizeExperiment = false;
	[SerializeField]
	private bool speedExperiment = false;
	[SerializeField]
	private float
		speed = 1f;
	[SerializeField]
	private int
		experimentSteps = 3;
	[SerializeField]
	private List<Color>
		targetColors = new List<Color>();
	[SerializeField]
	private Color
		backgroundColor = Color.grey;
	[SerializeField]
	private Transform[]
		targets;
	[SerializeField]
	private Transform
		background;
	[SerializeField]
	private GazeCalculator gazeCalculator;

	private bool canRun;
	private bool isRunning;
	private Vector3 upperBounds;
	private Vector3 lowerBounds;
	private float elapsedTime;

	// Use this for initialization
	void Start()
	{
		if(Application.isPlaying)
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
			upperBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z - background.position.z));
			lowerBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z - background.position.z + 0.5f));
		}
	}
	
	void OnGUI()
	{
		if(!Application.isPlaying)
		{
			background.renderer.sharedMaterial.color = backgroundColor;
		}
		else
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
						if(!targets[i])
						{
							targets[i] = Instantiate(targets[i]) as Transform;
							targets[i].parent = transform;
						}
						targets[i].gameObject.SetActive(false);
					}
					canRun = true;
				}
				GUI.TextArea(new Rect((Screen.width - width) * 0.5f, height, width, height), elapsedTime.ToString());
			}
		}
	}

	private IEnumerator RunExperiementFor(float length)
	{
		if(contrastExperiment)
		{
			for(int i = 0; i < experimentSteps; i++)
			{
				yield return StartCoroutine(RunContrastExperimentFor(length / experimentSteps, i + 1));
			}
			canRun = true;
		}
		else if(sizeExperiment)
		{
		}
		else if(speedExperiment)
		{
			for(int i = 0; i < experimentSteps; i++)
			{
				yield return StartCoroutine(RunSpeedExperimentFor(length / experimentSteps, 0, i + 1));
			}
			canRun = true;
		}
	}

	private IEnumerator RunContrastExperimentFor(float runtime, int difficulty)
	{
		elapsedTime = 0f;
		bool isLerping = true;
		for(int i = 0; i < targets.Length; i++)
		{
			targets[i].GetChild(0).renderer.material.color = backgroundColor;
			targets[i].position = new Vector3(Random.Range(lowerBounds.x, upperBounds.x), Random.Range(lowerBounds.y, upperBounds.y), -upperBounds.z);
			targets[i].localScale = Vector3.one / difficulty;
			targets[i].gameObject.SetActive(true);
		}
		while(elapsedTime < runtime)
		{
			for(int i = 0; i < targets.Length; i++)
			{
				Color c = targets[i].GetChild(0).renderer.material.color;
				Vector3 col = new Vector3(c.r, c.g, c.b);
				Vector3 endCol = new Vector3(targetColors.ToArray()[i].r, targetColors.ToArray()[i].g, targetColors.ToArray()[i].b);
//				Vector3 endCol = targetColors.Count < targets.Length ? targetColors.ToArray()[0] : targetColors.ToArray()[i];
				col = Vector3.MoveTowards(col, endCol, Time.deltaTime * speed * 0.01f / Mathf.Pow(2f,difficulty));
				targets[i].GetChild(0).renderer.material.color = new Color(col.x, col.y, col.z);
				if(endCol == col)
				{
					for(int j = 0; j < targets.Length; j++)
					{
						targets[j].GetChild(0).renderer.material.color = backgroundColor;
						targets[j].position = new Vector3(Random.Range(lowerBounds.x, upperBounds.x), Random.Range(lowerBounds.y, upperBounds.y), -upperBounds.z);
						targets[j].localScale = Vector3.one / difficulty;
						targets[j].gameObject.SetActive(true);
					}
				}
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator RunSpeedExperimentFor(float runtime, int targetIndex, int difficulty)
	{
		elapsedTime = 0f;
		float thresh = 1f;
		float step = -1f;
		Transform target = targets[targetIndex];
		Vector3 startPoint = new Vector3(lowerBounds.x, upperBounds.y * Random.Range(-1f, 1f), -lowerBounds.z);
		Vector3 targetPoint = new Vector3(lowerBounds.x + step, upperBounds.y * Random.Range(-1f, 1f), -lowerBounds.z);
		target.position = startPoint;
		target.gameObject.SetActive(true);
		target.localScale = Vector3.one / difficulty;
		while(elapsedTime < runtime)
		{
			//Check if the target is outside camera border
			if(Mathf.Abs(upperBounds.x - target.position.x) < 0.1f)
			{
				target.position = new Vector3(lowerBounds.x, upperBounds.y * Random.Range(-1f, 1f), -lowerBounds.z);
				targetPoint = new Vector3(target.position.x + step, upperBounds.y * Random.Range(-1f, 1f), -lowerBounds.z);
				Debug.Log("resetting");
			}
			if(elapsedTime > thresh || Mathf.Abs(targetPoint.x - target.position.x) < 0.1f)
			{
				targetPoint = new Vector3(target.position.x + step, upperBounds.y * Random.Range(-1f, 1f), -lowerBounds.z);
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

	private void OnTargetHit()
	{

	}

	private Vector3 GetRandomVerticalPosition(Vector3 point)
	{
		return new Vector3(point.x, point.y * Random.value, point.z);
	}

	private Vector3 GetRandomPointOnRect(Vector3 point)
	{
		return new Vector3(point.x, point.y * Random.value, point.z);
	}

	public float ElapsedTime {
		get {
			return elapsedTime;
		}
	}
}