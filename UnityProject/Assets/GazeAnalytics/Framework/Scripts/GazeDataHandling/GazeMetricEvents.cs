using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class GazeMetricEvents
{
	private List<GazeEvent> gazeDataList = new List<GazeEvent>();

	private static GazeMetricEvents instance;

	public static GazeMetricEvents Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GazeMetricEvents();
			}
			return instance;
		}
	}

	public void NewGazeEvent(string eventName, Vector3 eventOrigin, Vector3 eventHitPosition, string eventHitName, Ray eventGazeRay)
	{
		GazeEvent newEvent = new GazeEvent(eventName, eventOrigin, eventHitPosition, eventHitName, eventGazeRay);
		gazeDataList.Add(newEvent);
	}

	public void NewGazeEvent(string eventName, Vector3 eventOrigin, float pupilMeanSize, float blinkFrequency, float blinkClosedToOpenedLength, float saccadeFrequency, float fixationMeanLength)
	{
		GazeEvent newEvent = new GazeEvent(eventName, eventOrigin, pupilMeanSize, blinkFrequency, blinkClosedToOpenedLength, saccadeFrequency, fixationMeanLength);
		gazeDataList.Add(newEvent);
	}

	public void NewGazeEvent(string eventName, Vector3 eventOrigin, Vector3 eventHitPosition, string eventHitName, Ray eventGazeRay, float pupilMeanSize, float blinkFrequency, float blinkClosedToOpenedLength, float saccadeFrequency, float fixationMeanLength)
	{
		GazeEvent newEvent = new GazeEvent(eventName, eventOrigin, eventHitPosition, eventHitName, eventGazeRay, pupilMeanSize, blinkFrequency, blinkClosedToOpenedLength, saccadeFrequency, fixationMeanLength);
		gazeDataList.Add(newEvent);
	}

	public List<GazeEvent> GazeDataList
	{
		get
		{
			return gazeDataList;
		}
	}

}

[System.Serializable]
public class GazeEvent
{
	public string eventName;
	public Vector3 eventOrigin;
	public Vector3 eventHitPoint;
	public string eventHitName;
	public Ray eventGazeRay;
	public float pupilMeanSize;
	public float blinkFrequency;
	public float blinkClosedToOpenedLength;
	public float saccadeFrequency;
	public float fixationMeanLength;

	public GazeEvent()
	{
		this.eventName = "";
		this.eventOrigin = Vector3.zero;
		this.eventHitPoint = Vector3.zero;
		this.eventHitName = "";
		this.eventGazeRay = new Ray();
		this.pupilMeanSize = 0f;
		this.blinkFrequency = 0f;
		this.blinkClosedToOpenedLength = 0f;
		this.saccadeFrequency = 0f;
		this.fixationMeanLength = 0f;
	}

	public GazeEvent(string eventName, Vector3 eventOrigin, Vector3 eventHitPosition, string eventHitName, Ray eventGazeRay)
	{
		this.eventName = eventName;
		this.eventOrigin = eventOrigin;
		this.eventHitPoint = eventHitPosition;
		this.eventHitName = eventHitName;
		this.eventGazeRay = new Ray();
		this.pupilMeanSize = 0f;
		this.blinkFrequency = 0f;
		this.blinkClosedToOpenedLength = 0f;
		this.saccadeFrequency = 0f;
		this.fixationMeanLength = 0f;
	}

	public GazeEvent(string eventName, Vector3 eventOrigin, float pupilMeanSize, float blinkFrequency, float blinkClosedToOpenedLength, float saccadeFrequency, float fixationMeanLength)
	{
		this.eventName = eventName;
		this.eventOrigin = eventOrigin;
		this.eventHitPoint = Vector3.zero;
		this.eventHitName = "";
		this.eventGazeRay = new Ray();
		this.pupilMeanSize = pupilMeanSize;
		this.blinkFrequency = blinkFrequency;
		this.blinkClosedToOpenedLength = blinkClosedToOpenedLength;
		this.saccadeFrequency = saccadeFrequency;
		this.fixationMeanLength = fixationMeanLength;
	}

	public GazeEvent(string eventName, Vector3 eventOrigin, Vector3 eventHitPosition, string eventHitName, Ray eventGazeRay, float pupilMeanSize, float blinkFrequency, float blinkClosedToOpenedLength, float saccadeFrequency, float fixationMeanLength)
	{
		this.eventName = eventName;
		this.eventOrigin = eventOrigin;
		this.eventHitPoint = eventHitPosition;
		this.eventHitName = eventHitName;
		this.eventGazeRay = eventGazeRay;
		this.pupilMeanSize = pupilMeanSize;
		this.blinkFrequency = blinkFrequency;
		this.blinkClosedToOpenedLength = blinkClosedToOpenedLength;
		this.saccadeFrequency = saccadeFrequency;
		this.fixationMeanLength = fixationMeanLength;
	}

}

//[System.Serializable]
//public class EyeEvent
//{
//	public string eventName;
//	public Vector3 eventOrigin;
//	public float pupilMeanSize;
//	public float blinkFrequency;
//	public float blinkClosedToOpenedLength;
//	public float saccadeFrequency;
//	public float fixationMeanLength;
//	
//	public EyeEvent()
//	{
//		this.eventName = "";
//		this.eventOrigin = Vector3.zero;
//		this.pupilMeanSize = 0f;
//		this.blinkFrequency = 0f;
//		this.blinkClosedToOpenedLength = 0f;
//		this.saccadeFrequency = 0f;
//		this.fixationMeanLength = 0f;
//	}
//
//	public EyeEvent(string eventName, Vector3 eventOrigin, float pupilMeanSize, float blinkFrequency, float blinkClosedToOpenedLength, float saccadeFrequency, float fixationMeanLength)
//	{
//		this.eventName = eventName;
//		this.eventOrigin = eventOrigin;
//		this.pupilMeanSize = pupilMeanSize;
//		this.blinkFrequency = blinkFrequency;
//		this.blinkClosedToOpenedLength = blinkClosedToOpenedLength;
//		this.saccadeFrequency = saccadeFrequency;
//		this.fixationMeanLength = fixationMeanLength;
//	}
//	
//}