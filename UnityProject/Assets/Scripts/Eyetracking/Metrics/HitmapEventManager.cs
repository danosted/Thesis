using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class HitmapDataManager
{

	private HashSet<HitmapEvent> hitmapDataSet = new HashSet<HitmapEvent>();

	private static HitmapDataManager hitmapManager;

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
	
	public void NewHitEvent(string eventName, Vector3 eventPosition, string eventGazeTarget)
	{
		HitmapEvent newEvent = new HitmapEvent(eventName, eventPosition, eventGazeTarget);
		hitmapDataSet.Add(newEvent);
	}

	public void NewHitEvent(string eventName, Vector3 eventPosition, string eventGazeTarget, Vector3 eventHitPosition)
	{
		HitmapEvent newEvent = new HitmapEvent(eventName, eventPosition, eventGazeTarget, eventHitPosition);
		hitmapDataSet.Add(newEvent);
	}

	public void NewHitEvent(string eventName, Vector3 eventPosition, string eventGazeTarget, Vector3 eventHitPosition, Ray eventGazeRay)
	{
		HitmapEvent newEvent = new HitmapEvent(eventName, eventPosition, eventGazeTarget, eventHitPosition, eventGazeRay);
		hitmapDataSet.Add(newEvent);
	}

	public HashSet<HitmapEvent> GetHitmapData()
	{
		return this.hitmapDataSet;
	}

	//TODO:
	//Create function to show gaze ray or store gaze ray parameters (on hit or also on no hit?). Show it shooting from a given position

}

[System.Serializable]
public class HitmapEvent
{
	public string eventName;
	[SerializeField]
	public Vector3
		eventOrigin;
	[SerializeField]
	public string
		eventGazeTarget;
	[SerializeField]
	public Vector3
		eventHitPosition;
	[SerializeField]
	public Ray
		eventGazeRay;

	public HitmapEvent()
	{
		this.eventName = "";
		this.eventHitPosition = Vector3.zero;
		this.eventOrigin = Vector3.zero;
		this.eventGazeTarget = "";
		this.eventGazeRay = new Ray();
	}
	
	public HitmapEvent(string eventName, Vector3 eventOrigin, string eventGazeTarget)
	{
		this.eventName = eventName;
		this.eventHitPosition = Vector3.zero;
		this.eventOrigin = eventOrigin;
		this.eventGazeTarget = eventGazeTarget;
		this.eventGazeRay = new Ray();
	}
	
	public HitmapEvent(string eventName, Vector3 eventOrigin, string eventGazeTarget, Vector3 eventHitPosition)
	{
		this.eventName = eventName;
		this.eventHitPosition = eventHitPosition;
		this.eventOrigin = eventOrigin;
		this.eventGazeTarget = eventGazeTarget;
		this.eventGazeRay = new Ray();
	}

	public HitmapEvent(string eventName, Vector3 eventOrigin, string eventGazeTarget, Vector3 eventHitPosition, Ray eventGazeRay)
	{
		this.eventName = eventName;
		this.eventHitPosition = eventHitPosition;
		this.eventOrigin = eventOrigin;
		this.eventGazeTarget = eventGazeTarget;
		this.eventGazeRay = eventGazeRay;
	}

}