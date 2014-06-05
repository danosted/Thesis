using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GazeMetricsCollecter : MonoBehaviour
{
	[SerializeField]
	private float
		timeBetweenDataCollects = 0.5f;
	[SerializeField]
	private ExperimentSpawner
		experiment;

	private GazeCalculator gazeCalculator;

	private HashSet<Transform> hits = new HashSet<Transform>();

	void Start()
	{
		gazeCalculator = GetComponent<GazeCalculator>();
		gazeCalculator.OnGazeObjectHit += OnGazeObjectHit;
		experiment.OnExperimentStarted += OnExperimentStarted;
		StartCoroutine(CollectEyeMetrics());
	}

	private IEnumerator CollectEyeMetrics()
	{
		while(true)
		{
			GazeMetricEvents.Instance.NewGazeEvent("DataWithObject", 
			                                       transform.position, 
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
			                                       gazeCalculator.GetCurrentTargetObjectPath());
			yield return new WaitForSeconds(timeBetweenDataCollects);
		}
	}

	private void OnExperimentStarted()
	{
		hits.Clear();
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
			GazeMetricEvents.Instance.NewGazeEvent("DataWithObject", 
			                                       transform.position, 
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
			                                       gazeCalculator.GetCurrentTargetObjectPath());
		}
	}
}
