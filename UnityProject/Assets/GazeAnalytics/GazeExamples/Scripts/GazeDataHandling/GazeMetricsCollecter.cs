using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GazeMetricsCollecter : MonoBehaviour
{
	[SerializeField]
	private float
		timeBetweenDataCollects = 0.033f;
	[SerializeField]
	private ExperimentSpawner
		experiment;
	[SerializeField]
	private string eventName = "experiment";

	private GazeCalculator gazeCalculator;
	private GazeMetricEvents gazeEvent;

	private HashSet<Transform> hits = new HashSet<Transform>();

	void Start()
	{
		gazeCalculator = GetComponent<GazeCalculator>();
		gazeCalculator.OnGazeObjectHit += OnGazeObjectHit;
		experiment.OnExperimentStepStarted += OnExperimentStarted;
		experiment.OnExperimentStepEnded += OnExperimentEnded;
        experiment.OnExperimentGoodTargetDisappear += OnGazeObjectHit;
		gazeEvent = GazeMetricEvents.Instance;
		StartCoroutine(CollectEyeMetrics());
	}

	private IEnumerator CollectEyeMetrics()
	{
		while(true)
		{
			gazeEvent.NewGazeEvent(eventName, 
			                       Time.time,
	                               gazeCalculator.GetCurrentHitPosition(),
	                               gazeCalculator.GetCurrentTargetPosition(),
	                               gazeCalculator.GetCurrentTargetScale(),
	                               gazeCalculator.GetCurrentTargetRotation(),
	                               gazeCalculator.GetCurrentTargetColor(),
	                               gazeCalculator.GetCurrentTargetName(), 
	                               gazeCalculator.GetCurrentGazeRay(), 
	                               gazeCalculator.PupilSize,
			                       0,
	                               gazeCalculator.CurrentEyesClosedTime, 
			                       0,
			                       0f,
			                       gazeCalculator.CurrentFixationLength,
	                               gazeCalculator.FixationIndex,
	                               gazeCalculator.GetCurrentTargetObjectPath());
			yield return new WaitForSeconds(timeBetweenDataCollects);
		}
	}

	private void OnExperimentStarted()
	{
		gazeEvent.NewGazeEvent(eventName, 
		                       Time.time,
		                       gazeCalculator.GetCurrentHitPosition(),
		                       gazeCalculator.GetCurrentTargetPosition(),
		                       gazeCalculator.GetCurrentTargetScale(),
		                       gazeCalculator.GetCurrentTargetRotation(),
		                       gazeCalculator.GetCurrentTargetColor(),
		                       "ExperimentStarted", 
		                       gazeCalculator.GetCurrentGazeRay(), 
		                       gazeCalculator.PupilSize,
		                       0,
		                       gazeCalculator.CurrentEyesClosedTime, 
		                       0,
		                       0f,
		                       gazeCalculator.CurrentFixationLength,
		                       gazeCalculator.FixationIndex,
                               "ExperimentStarted");
		hits.Clear();
	}

	private void OnExperimentEnded()
	{
		gazeEvent.NewGazeEvent(eventName, 
		                       Time.time,
		                       gazeCalculator.GetCurrentHitPosition(),
		                       gazeCalculator.GetCurrentTargetPosition(),
		                       gazeCalculator.GetCurrentTargetScale(),
		                       gazeCalculator.GetCurrentTargetRotation(),
		                       gazeCalculator.GetCurrentTargetColor(),
                               "ExperimentEnded", 
		                       gazeCalculator.GetCurrentGazeRay(), 
		                       gazeCalculator.PupilSize,
		                       0,
		                       gazeCalculator.CurrentEyesClosedTime, 
		                       0,
		                       0f,
		                       gazeCalculator.CurrentFixationLength,
		                       gazeCalculator.FixationIndex,
                               "ExperimentEnded");
	}

	private void OnGazeObjectHit(Transform hit)
	{
        if(hit == null)
        {
            return;
        }
		if(hit.GetComponent<GazePrefabTracker>())
		{
			gazeEvent.NewGazeEvent(eventName, 
			                       Time.time,
			                       gazeCalculator.GetCurrentHitPosition(),
			                       gazeCalculator.GetCurrentTargetPosition(),
			                       gazeCalculator.GetCurrentTargetScale(),
			                       gazeCalculator.GetCurrentTargetRotation(),
			                       gazeCalculator.GetCurrentTargetColor(),
			                       "hit: " + hit.name, 
			                       gazeCalculator.GetCurrentGazeRay(), 
			                       gazeCalculator.PupilSize,
			                       0,
			                       gazeCalculator.CurrentEyesClosedTime, 
			                       0,
			                       0f,
			                       gazeCalculator.CurrentFixationLength,
			                       gazeCalculator.FixationIndex,
			                       gazeCalculator.GetCurrentTargetObjectPath());
		}
	}
}
