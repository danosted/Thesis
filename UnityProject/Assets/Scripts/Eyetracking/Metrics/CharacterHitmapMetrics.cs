using UnityEngine;
using System.Collections;

public class CharacterHitmapMetrics : MonoBehaviour
{
	private CharacterGAMetrics characterMetrics;

	void Start()
	{
		characterMetrics = GetComponent<CharacterGAMetrics>();
		StartCoroutine(CollectCharacterMetrics());

	}

	private IEnumerator CollectCharacterMetrics()
	{
		while(true)
		{
			HitmapDataManager.HitmapManager.NewHitEvent("GazeRayWithTargetObject", 0f, transform.position, characterMetrics.GetCurrentTarget(), characterMetrics.GetCurrentGazeRay());
			yield return new WaitForSeconds(1f);
		}
	}
}
