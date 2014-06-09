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
	
	private string eventText = "experiment";

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
		gazeEvent = GazeMetricEvents.Instance;
		StartCoroutine(CollectEyeMetrics());
	}

	private IEnumerator CollectEyeMetrics()
	{
		while(true)
		{
			gazeEvent.NewGazeEvent(eventText, 
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
		gazeEvent.NewGazeEvent(eventText, 
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
		hits.Clear();
	}

	private void OnExperimentEnded()
	{
		gazeEvent.NewGazeEvent(eventText, 
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
			gazeEvent.NewGazeEvent(eventText, 
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
		}
	}
}
