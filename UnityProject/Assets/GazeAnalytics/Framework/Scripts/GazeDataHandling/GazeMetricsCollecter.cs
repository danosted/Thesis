using UnityEngine;
using System.Collections;

public class GazeMetricsCollecter : MonoBehaviour
{
	[SerializeField]
	private float
		timeBetweenDataCollects = 1f;
	private GazeCalculator gazeCalculator;

	void Start()
	{
		gazeCalculator = GetComponent<GazeCalculator>();
		StartCoroutine(CollectEyeMetrics());
	}

	private IEnumerator CollectEyeMetrics()
	{
		while(true)
		{
			Vector3 pos = transform.position;
			GazeMetricEvents.Instance.NewGazeEvent(Constants.GazeEvent, pos, gazeCalculator.GetCurrentHitPosition(), gazeCalculator.GetCurrentTargetName(), gazeCalculator.GetCurrentGazeRay());
//			yield return new WaitForSeconds(timeBetweenDataCollects);
//			TODO: Change to eye metrics data
			GazeMetricEvents.Instance.NewGazeEvent(Constants.PupilEvent, pos, gazeCalculator.PupilSize, 0f, 0f, 0f, 0f);
			yield return new WaitForSeconds(timeBetweenDataCollects);
		}
	}
}
