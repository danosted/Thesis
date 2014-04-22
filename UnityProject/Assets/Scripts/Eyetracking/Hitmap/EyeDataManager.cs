using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class EyeDataManager
{

	private HashSet<HitmapEvent> hitmapDataSet = new HashSet<HitmapEvent>();
	private HashSet<EyeEvent> eyeDataSet = new HashSet<EyeEvent>();
	private static EyeDataManager instance;

	public static EyeDataManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new EyeDataManager();
			}
			return instance;
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

	public void NewEyeEvent(string eventName, Vector3 eventOrigin, float pupilMeanSize, float blinkFrequency, float blinkClosedToOpenedLength, float saccadeFrequency, float fixationMeanLength)
	{
		EyeEvent newEvent = new EyeEvent(eventName, eventOrigin, pupilMeanSize, blinkFrequency, blinkClosedToOpenedLength, saccadeFrequency, fixationMeanLength);
		eyeDataSet.Add(newEvent);
	}

	public HashSet<HitmapEvent> HitmapDataSet
	{
		get
		{
			return hitmapDataSet;
		}
	}

	public HashSet<EyeEvent> EyeDataSet
	{
		get
		{
			return eyeDataSet;
		}
	}
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

[System.Serializable]
public class EyeEvent
{
	public string eventName;
	public Vector3 eventOrigin;
	public float pupilMeanSize;
	public float blinkFrequency;
	public float blinkClosedToOpenedLength;
	public float saccadeFrequency;
	public float fixationMeanLength;
	
	public EyeEvent()
	{
		this.eventName = "";
		this.eventOrigin = Vector3.zero;
		this.pupilMeanSize = 0f;
		this.blinkFrequency = 0f;
		this.blinkClosedToOpenedLength = 0f;
		this.saccadeFrequency = 0f;
		this.fixationMeanLength = 0f;
	}

	public EyeEvent(string eventName, Vector3 eventOrigin, float pupilMeanSize, float blinkFrequency, float blinkClosedToOpenedLength, float saccadeFrequency, float fixationMeanLength)
	{
		this.eventName = eventName;
		this.eventOrigin = eventOrigin;
		this.pupilMeanSize = pupilMeanSize;
		this.blinkFrequency = blinkFrequency;
		this.blinkClosedToOpenedLength = blinkClosedToOpenedLength;
		this.saccadeFrequency = saccadeFrequency;
		this.fixationMeanLength = fixationMeanLength;
	}
	
}