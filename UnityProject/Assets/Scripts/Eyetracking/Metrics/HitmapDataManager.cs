using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class HitmapDataManager
{
	private static HitmapDataManager hitmapManager;

	private HashSet<HitmapData> hitmapDataSet = new HashSet<HitmapData>();

	public static HitmapDataManager HitmapManager
	{
		get
		{
			if(hitmapManager == null)
			{
				hitmapManager = new HitmapDataManager();
			}
			return hitmapManager;
		}
	}
	
	public void NewHitEvent(string eventName, Vector3 eventPosition, GameObject eventGazeTarget)
	{
		HitmapData newEvent = new HitmapData(eventName, eventPosition, eventGazeTarget);
		hitmapDataSet.Add(newEvent);
	}

	public void NewHitEvent(string eventName, float eventValue, Vector3 eventPosition, GameObject eventGazeTarget)
	{
		HitmapData newEvent = new HitmapData(eventName, eventValue, eventPosition, eventGazeTarget);
		hitmapDataSet.Add(newEvent);
	}

	public void NewHitEvent(string eventName, Vector3 eventPosition, Ray eventGazeRay)
	{
		HitmapData newEvent = new HitmapData(eventName, eventPosition, eventGazeRay);
		hitmapDataSet.Add(newEvent);
	}

	public void NewHitEvent(string eventName, float eventValue, Vector3 eventPosition, Ray eventGazeRay)
	{
		HitmapData newEvent = new HitmapData(eventName, eventValue, eventPosition, eventGazeRay);
		hitmapDataSet.Add(newEvent);
	}
	public void NewHitEvent(string eventName, float eventValue, Vector3 eventPosition, GameObject eventGazeTarget, Ray eventGazeRay)
	{
		HitmapData newEvent = new HitmapData(eventName, eventValue, eventPosition, eventGazeTarget, eventGazeRay);
		hitmapDataSet.Add(newEvent);
	}

	public HashSet<HitmapData> GetHitmapData()
	{
		return this.hitmapDataSet;
	}

	//TODO:
	//Create function to show gaze ray or store gaze ray parameters (on hit or also on no hit?). Show it shooting from a given position

}

public class HitmapData
{
	public string eventName;
	public float eventValue;
	public Vector3 eventPosition;
	public GameObject eventGazeTarget;
	public Ray eventGazeRay;
	
	public HitmapData(string eventName, Vector3 eventPosition, GameObject eventGazeTarget)
	{
		this.eventName = eventName;
		this.eventValue = 0f;
		this.eventPosition = eventPosition;
		this.eventGazeTarget = eventGazeTarget;
		this.eventGazeRay = new Ray();
	}
	
	public HitmapData(string eventName, float eventValue, Vector3 eventPosition, GameObject eventGazeTarget)
	{
		this.eventName = eventName;
		this.eventValue = eventValue;
		this.eventPosition = eventPosition;
		this.eventGazeTarget = eventGazeTarget;
		this.eventGazeRay = new Ray();
	}

	public HitmapData(string eventName, Vector3 eventPosition, Ray eventGazeRay)
	{
		this.eventName = eventName;
		this.eventValue = 0f;
		this.eventPosition = eventPosition;
		this.eventGazeTarget = null;
		this.eventGazeRay = eventGazeRay;
	}

	public HitmapData(string eventName, float eventValue, Vector3 eventPosition, Ray eventGazeRay)
	{
		this.eventName = eventName;
		this.eventValue = eventValue;
		this.eventPosition = eventPosition;
		this.eventGazeTarget = null;
		this.eventGazeRay = eventGazeRay;
	}

	public HitmapData(string eventName, float eventValue, Vector3 eventPosition, GameObject eventGazeTarget, Ray eventGazeRay)
	{
		this.eventName = eventName;
		this.eventValue = eventValue;
		this.eventPosition = eventPosition;
		this.eventGazeTarget = eventGazeTarget;
		this.eventGazeRay = eventGazeRay;
	}
}