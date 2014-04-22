using UnityEngine;
using System.Collections;

public class CharacterEyeMetricsCollecter : MonoBehaviour
{
	[SerializeField]
	private float
		timeBetweenDataCollects = 1f;
	private CharacterEyeMetrics eyeMetrics;

	void Start()
	{
		eyeMetrics = GetComponent<CharacterEyeMetrics>();
		StartCoroutine(CollectCharacterMetrics());
	}

	private IEnumerator CollectCharacterMetrics()
	{
		while(true)
		{
			Vector3 pos = transform.position;
			EyeDataManager.Instance.NewHitEvent("GazeRayWithTargetObject", pos, eyeMetrics.GetCurrentTargetName(), eyeMetrics.GetCurrentHitPosition(), eyeMetrics.GetCurrentGazeRay());
//			yield return new WaitForSeconds(timeBetweenDataCollects);
//			TODO: Change to eye metrics data
			EyeDataManager.Instance.NewEyeEvent("PupilSizeChange", pos, Random.Range(20f, 25f), 0f, 0f, 0f, 0f);
			yield return new WaitForSeconds(timeBetweenDataCollects);
		}
	}
}
