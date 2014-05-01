using UnityEngine;
using System.Collections;

public class GazeMetricsCollecter : MonoBehaviour
{
	[SerializeField]
	private float
		timeBetweenDataCollects = 1f;
	private FirstPersonGazeCalculator eyeMetrics;

	void Start()
	{
		eyeMetrics = GetComponent<FirstPersonGazeCalculator>();
		StartCoroutine(CollectEyeMetrics());
	}

	private IEnumerator CollectEyeMetrics()
	{
		while(true)
		{
			Vector3 pos = transform.position;
			GazeMetricEvents.Instance.NewHitEvent("GazeRayWithTargetObject", pos, eyeMetrics.GetCurrentTargetName(), eyeMetrics.GetCurrentHitPosition(), eyeMetrics.GetCurrentGazeRay());
//			yield return new WaitForSeconds(timeBetweenDataCollects);
//			TODO: Change to eye metrics data
			GazeMetricEvents.Instance.NewEyeEvent("PupilSizeChange", pos, eyeMetrics.PupilSize, 0f, 0f, 0f, 0f);
			yield return new WaitForSeconds(timeBetweenDataCollects);
		}
	}
}
