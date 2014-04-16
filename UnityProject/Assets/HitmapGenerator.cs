using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitmapGenerator : MonoBehaviour
{
	private HitmapDataManager data = HitmapDataManager.HitmapManager;

	[SerializeField]
	private List<HitmapEvent>
		dataInstanceList;

	private IEnumerator CreateHitmap()
	{
		if(dataInstanceList != null)
		{
			dataInstanceList = new List<HitmapEvent>();
		}
		foreach(HitmapEvent e in data.GetHitmapData())
		{
			dataInstanceList.Add(e);
		}
		yield return null;
	}
}
