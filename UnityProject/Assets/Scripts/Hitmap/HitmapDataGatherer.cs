using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode]
public class HitmapDataGatherer : MonoBehaviour
{
	private HitmapDataManager data = HitmapDataManager.HitmapManager;

	[SerializeField]
	private List<HitmapEvent>
		dataInstanceList;

	private bool isRunning;

	void Start()
	{
		try
		{
			LoadData();
		}
		catch(System.Exception e)
		{
			Debug.Log(e);
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			CreateHitmapFromData();
		}
	}

	public void CreateHitmapFromData()
	{
		if(!isRunning)
		{
			isRunning = true;
			StartCoroutine(CreateNewHitmap());
		}
		else
		{
			Debug.Log("Still creating hitmap");
		}
	}

	private IEnumerator CreateNewHitmap()
	{
		if(dataInstanceList != null)
		{
			dataInstanceList = new List<HitmapEvent>();
		}
		else
		{
			dataInstanceList.Clear();
		}
		foreach(HitmapEvent e in data.GetHitmapData())
		{
			dataInstanceList.Add(e);
		}
		isRunning = false;
		SaveData();
		yield return null;
	}

	public void SaveData()
	{
//		object o = (object)dataInstanceList;
//		Serializer.Instance.Serialize(o);
		Serializer.Instance.Serialize(dataInstanceList);
	}

	public void LoadData()
	{
//		object o = Serializer.Instance.Deserialize();
//		dataInstanceList = (List<HitmapEvent>)o;
		dataInstanceList = Serializer.Instance.Deserialize();
			
	}

	public List<HitmapEvent> GetDataInstanceList()
	{
		return this.dataInstanceList;
	}

	public void SetDataInstanceList(List<HitmapEvent> dataInstanceList)
	{
		this.dataInstanceList = dataInstanceList;
	}
}
