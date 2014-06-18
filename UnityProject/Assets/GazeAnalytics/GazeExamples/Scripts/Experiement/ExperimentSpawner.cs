using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TETCSharpClient;
using System.Security.Cryptography;

[ExecuteInEditMode]
public class ExperimentSpawner : MonoBehaviour
{
	public delegate void OnExperimentStepStartedDelegate();
    public event OnExperimentStepStartedDelegate OnExperimentStepStarted;

	public delegate void OnExperimentStepEndedDelegate();
    public event OnExperimentStepEndedDelegate OnExperimentStepEnded;

    public delegate void OnExperimentStartedDelegate();
    public event OnExperimentStartedDelegate OnExperimentStarted;

    public delegate void OnExperimentEndedDelegate();
    public event OnExperimentEndedDelegate OnExperimentEnded;

    public delegate void OnExperimentTargethitDelegate();
    public event OnExperimentTargethitDelegate OnExperimentTargethit;
	
	[SerializeField]
	private ExperimentType
		experimentType = ExperimentType.ContrastExperiment;
	[SerializeField]
	private float
		experimentStepDuration = 60f;
	[SerializeField]
	private int
		experimentSteps = 3;
	[SerializeField]
	private float
		endAlpha = 0.2f;
    [SerializeField]
    private float verticalClamp = 1f;
    [SerializeField]
    private float horizontalClamp = 1f;
	[SerializeField]
	private List<float> 
		targetStartSizes = new List<float>();
	[SerializeField]
	private List<float>
		targetEndSizes = new List<float>();
	[SerializeField]
	private Color
		backgroundColor = Color.grey;
	[SerializeField]
	private List<Transform>
		targets = new List<Transform>();
	[SerializeField]
	private Transform
		background;
	[SerializeField]
	private GazeCalculator
		gazeCalculator;
	[SerializeField]
	private Font
		textFont;

	private bool canRun;
	private bool isRunning;
	private bool showResults;
	private Vector3 upperBounds;
	private Vector3 lowerBounds;
	private float elapsedTime;
	private float currentTime;
	private List<float> finishingTimes = new List<float>();
	private float startTime;
	private int targetHitNum;
	private int currentExperimentStep;
	private TextMesh endText;

	public enum ExperimentType
	{
		ContrastExperiment,
		SizeExperiment,
		SpeedExperiment
	}

	// Use this for initialization
	void Awake()
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
				for(int i = 0; i < targets.Count; i++)
				{
					targets[i] = Instantiate(targets[i]) as Transform;
					targets[i].parent = transform;
					targets[i].gameObject.SetActive(false);
				}
				canRun = true;
			}
			upperBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height - 50, Camera.main.transform.position.z - background.transform.position.z));
			lowerBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.position.z - background.transform.position.z + 0.5f));
//			Debug.Log("upper: " + upperBounds.x + "," + upperBounds.y);
//			Debug.Log("Lower: " + lowerBounds.x + "," + lowerBounds.y);
			gazeCalculator.OnGazeObjectHit += OnTargetHit;

			GameObject textGo = new GameObject();
			textGo.AddComponent(typeof(TextMesh));
			textGo.renderer.material = textFont.material;
			endText = textGo.GetComponent<TextMesh>();
			endText.font = textFont;
			endText.characterSize = Camera.main.isOrthoGraphic ? 0.2f : 0.05f;
			endText.alignment = TextAlignment.Center;
			endText.anchor = TextAnchor.MiddleCenter;
			endText.fontSize = 50;
			endText.transform.position = Camera.main.transform.position + Vector3.forward * 2f;
			endText.gameObject.SetActive(false);
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
					StartCoroutine(RunExperiementFor(experimentStepDuration));
				}

			}
			else
			{
				if(GUI.Button(new Rect((Screen.width - width) * 0.5f, (Screen.height - height), width, height), "Stop Experiment"))
				{
					StopAllCoroutines();
					ResetTargets();
					endText.gameObject.SetActive(false);
					canRun = true;
				}
//				GUI.TextArea(new Rect((Screen.width - width) * 0.5f, height, width, height), elapsedTime.ToString());
			}
		}
	}

	void Update()
	{
		currentTime = Time.time - startTime;
	}

    //Source: http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
    private List<Transform> Shuffle(List<Transform> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Transform value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

	private IEnumerator RunExperiementFor(float length)
	{
		targetHitNum = 0;
		startTime = Time.time;
		currentExperimentStep = 0;
        if(OnExperimentStarted != null)
        {
            OnExperimentStarted();
        }
		if(experimentType == ExperimentType.ContrastExperiment)
		{
			for(int i = 0; i < experimentSteps; i++)
			{
                targets = Shuffle(targets);
				currentExperimentStep = i;
				if(OnExperimentStepStarted != null)
				{
					OnExperimentStepStarted();
				}
				yield return StartCoroutine(RunContrastExperimentFor(experimentStepDuration));
				ResetTargets();
				if(OnExperimentStepEnded != null)
				{	
					OnExperimentStepEnded();
				}
				yield return new WaitForSeconds(Random.Range(2f, 5f));
			}
		}
        //else if(experimentType == ExperimentType.SizeExperiment)
        //{
        //    for(int i = 0; i < experimentSteps; i++)
        //    {
        //        currentExperimentStep = i;
        //        if(OnExperimentStarted != null)
        //        {
        //            OnExperimentStarted();
        //        }
        //        yield return StartCoroutine(RunSizeExperimentFor(experimentStepDuration, 1));
        //        ResetTargets();
        //        if(OnExperimentEnded != null)
        //        {	
        //            OnExperimentEnded();
        //        }
        //        yield return new WaitForSeconds(Random.Range(2f, 5f));
        //    }
        //}
        //else if(experimentType == ExperimentType.SpeedExperiment)
        //{
        //    for(int i = 0; i < experimentSteps; i++)
        //    {
        //        currentExperimentStep = i;
        //        yield return StartCoroutine(RunSpeedExperimentFor(experimentStepDuration, 0, i + 1));
        //    }
        //}
		ResetTargets();
		StopAllCoroutines();
        if (OnExperimentEnded != null)
        {
            OnExperimentEnded();
        }
		StartCoroutine(ShowEndResults(targetHitNum));
	}

	private IEnumerator RunContrastExperimentFor(float runtime)
	{
		elapsedTime = 0f;
        int randCorner = Random.Range(0, 4);
        float width = Mathf.Abs(upperBounds.x - lowerBounds.x);
        float height = Mathf.Abs(upperBounds.y - lowerBounds.y);
        float widthStep = width / targets.Count;
        //Debug.Log("widthStep: " + widthStep);
		for(int i = 0; i < targets.Count; i++)
		{
            float scale = targetStartSizes[i];
            float horizontalClampClamped = Mathf.Clamp(horizontalClamp, 0f, (widthStep - scale) * 0.5f);
            float randOffsetX = Random.Range(-(widthStep - scale) * 0.5f + horizontalClampClamped, (widthStep - scale) * 0.5f - horizontalClampClamped);
            float vertClampClamped = Mathf.Clamp(verticalClamp, 0f, (height - scale) * 0.5f);
            float randOffsetY = Random.Range(-(height - scale) * 0.5f + vertClampClamped, (height - scale) * 0.5f - vertClampClamped);
            //Debug.Log("width/i: " + width / (i + 1));
            float mappedWidth = widthStep * (i + 1) - (width * 0.5f) - widthStep * 0.5f;
            //Debug.Log("mappedwidth: " + mappedWidth);
            targets[i].position = new Vector3(mappedWidth + randOffsetX, randOffsetY, background.position.z - 0.1f);
            //Debug.Log("t " + i + ": " + targets[i].position);
            Color c = targets[i].renderer.material.color;
            targets[i].renderer.material.color = new Color(c.r, c.g, c.b, 0f);
			targets[i].localScale = Vector3.one * targetStartSizes[i];
			targets[i].gameObject.SetActive(true);
		}
		while(elapsedTime < runtime)
		{
            float step = endAlpha / (runtime / Time.deltaTime);
			for(int i = 0; i < targets.Count; i++)
			{
                Color c = targets[i].renderer.material.color;
                float curAlpha = c.a;
                float nextAlpha = curAlpha + step;
                targets[i].renderer.material.color = new Color(c.r, c.g, c.b, nextAlpha);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator RunSizeExperimentFor(float runtime, int difficulty)
	{
		elapsedTime = 0f;
		bool isDone = false;
		for(int i = 0; i < targets.Count; i++)
		{
			targets[i].position = new Vector3(upperBounds.x * Random.Range(-1f, 1f), upperBounds.y * Random.Range(-1f, 1f), upperBounds.z);
			targets[i].localScale = Vector3.one * targetStartSizes[i];
			targets[i].gameObject.SetActive(true);
		}
		while(elapsedTime < runtime && !isDone)
		{
			for(int i = 0; i < targets.Count; i++)
			{
				Vector3 scale = targets[i].localScale;
				Vector3 endScale = Vector3.one * targetEndSizes[i];
				//				Vector3 endCol = targetColors.Count < targets.Length ? targetColors.ToArray()[0] : targetColors.ToArray()[i];
				scale = Vector3.MoveTowards(scale, endScale, Time.deltaTime * endAlpha / Mathf.Pow(2f, difficulty));
				targets[i].localScale = scale;
				if(Vector3.Distance(endScale, scale) < 0.1f)
				{
					for(int j = 0; j < targets.Count; j++)
					{
						targets[j].renderer.material.color = backgroundColor;
						targets[j].localScale = Vector3.one * targetStartSizes[j];
					}
					isDone = true;
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
			target.position = Vector3.MoveTowards(target.position, targetPoint, endAlpha * difficulty * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		target.gameObject.SetActive(false);
	}

	private IEnumerator ShowEndResults(int targetsHit)
	{
		float timeToShow = 8f;
        float endTime = 0f;
        if (finishingTimes.Count > 0)
        {
            foreach (float t in finishingTimes)
            {
                endTime += t;
            }
            endTime /= finishingTimes.Count;
        }
        else
        {
            endTime = Time.time - startTime;
        }
		string endTimeText = endTime.ToString();
		string shortText = "";
		if(endTime < 10f)
		{
			for(int i = 0; i < 3; i++)
			{
				shortText += endTimeText[i];
			}
		}
		else if(endTime < 100f)
		{
			for(int i = 0; i < 4; i++)
			{
				shortText += endTimeText[i];
			}
		}
		else
		{
			shortText = endTimeText;
		}

		endText.text = "Experiment Over!\nFinishing time average: " + shortText + " seconds.\nTarget Hits: " + targetsHit.ToString() + "/" + (targets.Count * experimentSteps) + "\nWell Done!";
		endText.gameObject.SetActive(true);
		yield return new WaitForSeconds(timeToShow);
		endText.gameObject.SetActive(false);
		canRun = true;
	}

    private Vector3 GetConstantDistancePositionFromIndex(int i)
    {
        Vector3 endPosition = Vector3.zero;
        float scale = targetStartSizes[i];
        if(i == 0)
        {
            Vector3 initPos = new Vector3((upperBounds.x - scale) * Random.Range(-1f, 1f), (upperBounds.y - scale) * Random.Range(-1f, 1f), -upperBounds.z);
            endPosition = initPos;
        }
        else if(i == 1)
        {
            Vector3 prevPos = targets[i - 1].position;
            Vector3 randomOnSphere = Random.onUnitSphere * verticalClamp;
            endPosition = prevPos + randomOnSphere;
        }
        else
        {
            Vector3 prevPos = targets[i - 1].position;
            Vector3 prevPrevPos = targets[i - 2].position;
            Vector3 dir = prevPos - prevPrevPos;
            float x = ((dir.x) * Mathf.Cos(90f)) - ((dir.y * Mathf.Sin(90f)));
            float y = ((dir.y) * Mathf.Cos(90f)) + ((dir.x * Mathf.Sin(90f)));
            endPosition = new Vector3(x, y, -upperBounds.z);
        }
        return endPosition;
    }

	private void ResetTargets()
	{
		for(int i = 0; i < targets.Count; i++)
		{
			targets[i].gameObject.SetActive(false);
		}
	}

	private void ResetTarget(Transform target)
	{
		targets.Find(x => x.Equals(target)).gameObject.SetActive(false);
	}

	private void OnTargetHit(Transform hit)
	{
        OnExperimentTargethit();
		targetHitNum++;
		ResetTarget(hit);
		int hits = 0;
		for(int i = 0; i < targets.Count; i++)
		{
			if(targets[i].gameObject.activeSelf)
			{
				break;
			}
			else
			{
				hits++;
			}
		}
		if(hits == targets.Count)
		{
            finishingTimes.Add(elapsedTime);
            Debug.Log("added finishtime");
		}
	}

	private Vector3 GetRandomVerticalPosition(Vector3 point)
	{
		return new Vector3(point.x, point.y * Random.value, point.z);
	}

	private Vector3 GetRandomPointOnRect(Vector3 point)
	{
		return new Vector3(point.x, point.y * Random.value, point.z);
	}

	public float ElapsedTime
	{
		get
		{
			return elapsedTime;
		}
	}

	public List<Transform> Targets
	{
		get
		{
			return targets;
		}
	}

	public float CurrentTime {
		get {
			return currentTime;
		}
	}
}