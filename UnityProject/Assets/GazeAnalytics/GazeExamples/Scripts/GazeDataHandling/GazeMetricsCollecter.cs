using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using TETCSharpClient;
//using TETCSharpClient.Data;

public class GazeMetricsCollecter : MonoBehaviour
{
	[SerializeField]
	private float
		timeBetweenDataCollects = 0.033f;
	[SerializeField]
	private ExperimentSpawner
		experiment;

	private GazeCalculator gazeCalculator;
	private TETGazeTrackerData gazeDataTracker;
	private GazeMetricEvents gazeEvent;

	private HashSet<Transform> hits = new HashSet<Transform>();

	void Start()
	{
		gazeCalculator = GetComponent<GazeCalculator>();
		gazeDataTracker = GetComponent<TETGazeTrackerData>();
		gazeCalculator.OnGazeObjectHit += OnGazeObjectHit;
		experiment.OnExperimentStarted += OnExperimentStarted;
		experiment.OnExperimentEnded += OnExperimentEnded;
//		GazeManager.Instance.AddGazeListener(this);
		gazeEvent = GazeMetricEvents.Instance;

	}

//	public void OnGazeUpdate(GazeData gazeData)
//	{
//		gazeEvent.NewGazeEvent("DataWithObject", 
//		                                       gazeCalculator.CurrentTime,
//		                                       gazeCalculator.GetCurrentHitPosition(),
//		                                       gazeCalculator.GetCurrentTargetPosition(),
//		                                       gazeCalculator.GetCurrentTargetScale(),
//		                                       gazeCalculator.GetCurrentTargetRotation(),
//		                                       gazeCalculator.GetCurrentTargetColor(),
//		                                       gazeCalculator.GetCurrentTargetName(), 
//		                                       gazeCalculator.GetCurrentGazeRay(), 
//		                                       gazeCalculator.PupilSize, 
//		                                       0f, 
//		                                       0f, 
//		                                       0f, 
//		                                       gazeCalculator.CurrentFixationLength,
//		                                       gazeCalculator.FixationIndex,
//		                                       gazeCalculator.GetCurrentTargetObjectPath());
//	}

	private IEnumerator CollectEyeMetrics()
	{
		while(true)
		{
			gazeEvent.NewGazeEvent("DataWithObject", 
	                               experiment.CurrentTime,
	                               gazeCalculator.GetCurrentHitPosition(),
	                               gazeCalculator.GetCurrentTargetPosition(),
	                               gazeCalculator.GetCurrentTargetScale(),
	                               gazeCalculator.GetCurrentTargetRotation(),
	                               gazeCalculator.GetCurrentTargetColor(),
	                               gazeCalculator.GetCurrentTargetName(), 
	                               gazeCalculator.GetCurrentGazeRay(), 
	                               gazeCalculator.PupilSize, 
	                               0f, 
	                               0f, 
	                               0f, 
	                               gazeCalculator.CurrentFixationLength,
	                               gazeCalculator.FixationIndex,
	                               gazeCalculator.GetCurrentTargetObjectPath());
			yield return new WaitForSeconds(timeBetweenDataCollects);
		}
	}

	private void OnExperimentStarted()
	{
		StartCoroutine(CollectEyeMetrics());
		hits.Clear();
	}

	private void OnExperimentEnded()
	{
		StopCoroutine("CollectEyeMetrics");
	}

	private void OnGazeObjectHit(Transform hit)
	{
		if(hit.tag == "ExperimentTarget" && !hits.Contains(hit))
		{
			int index = experiment.Targets.IndexOf(hit);
			string material = experiment.TargetMaterials[index].name;
			hits.Add(hit);
			GA.API.Design.NewEvent("Hit: " + hit.name + ", material: " + material, experiment.ElapsedTime, gazeCalculator.GetCurrentTargetPosition());
		}
		if(hit.GetComponent<GazePrefabTracker>())
		{
			gazeEvent.NewGazeEvent("DataWithObject", 
			                       experiment.CurrentTime,
	                               gazeCalculator.GetCurrentHitPosition(),
	                               gazeCalculator.GetCurrentTargetPosition(),
	                               gazeCalculator.GetCurrentTargetScale(),
	                               gazeCalculator.GetCurrentTargetRotation(),
	                               gazeCalculator.GetCurrentTargetColor(),
	                               gazeCalculator.GetCurrentTargetName(), 
	                               gazeCalculator.GetCurrentGazeRay(), 
	                               gazeCalculator.PupilSize, 
	                               0f, 
	                               0f, 
	                               0f, 
	                               gazeCalculator.CurrentFixationLength,
	                               gazeCalculator.FixationIndex,
	                               gazeCalculator.GetCurrentTargetObjectPath());
		}
	}
}
