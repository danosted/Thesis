using UnityEngine;
using System.Collections;

public class GazeMetricsCollecter : MonoBehaviour
{
	[SerializeField]
	private float
		timeBetweenDataCollects = 0.5f;
	[SerializeField]
	private ExperimentSpawner experiment;

	private GazeCalculator gazeCalculator;

	void Start()
	{
		gazeCalculator = GetComponent<GazeCalculator>();
		gazeCalculator.OnGazeObjectHit += OnGazeObjectHit;
		StartCoroutine(CollectEyeMetrics());
	}

	private IEnumerator CollectEyeMetrics()
	{
		while(true)
		{
			Vector3 pos = transform.position;
			GazeMetricEvents.Instance.NewGazeEvent("DataWithObject", 
			                                       pos, 
			                                       gazeCalculator.GetCurrentHitPosition(), 
			                                       gazeCalculator.GetCurrentTargetName(), 
			                                       gazeCalculator.GetCurrentGazeRay(), 
			                                       gazeCalculator.PupilSize, 
			                                       0f, 
			                                       0f, 
			                                       0f, 
			                                       gazeCalculator.CurrentFixationLength,
			                                       gazeCalculator.GetCurrentTargetObjectPath());
//			yield return new WaitForSeconds(timeBetweenDataCollects);
//			GazeMetricEvents.Instance.NewGazeEvent("WhenShitHitsTheFan2", 
//			                                       pos, 
//			                                       gazeCalculator.GetCurrentHitPosition(), 
//			                                       gazeCalculator.GetCurrentTargetName(), 
//			                                       gazeCalculator.GetCurrentGazeRay(), 
//			                                       10f, 
//			                                       0f, 
//			                                       0f, 
//			                                       0f, 
//			                                       0f,
//			                                       "");
			yield return new WaitForSeconds(timeBetweenDataCollects);
		}
	}

	private void OnGazeObjectHit(Object obj)
	{
		GA.API.Design.NewEvent("GazeHitObjectTime", experiment.ElapsedTime, gazeCalculator.GetCurrentTargetPosition());
	}
}
