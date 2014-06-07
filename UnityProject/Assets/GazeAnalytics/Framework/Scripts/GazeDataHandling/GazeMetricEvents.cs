using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class GazeMetricEvents
{
	private List<GazeEvent> gazeDataList = new List<GazeEvent>();
	private HashSet<string> eventNames = new HashSet<string>();

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

	public void NewGazeEvent(string eventName, float eventTime, Vector3 eventHitPoint, Vector3 eventHitObjectPosition, Vector3 eventHitScale, Quaternion eventHitRotation, Color eventHitColor, string eventHitName, Ray eventGazeRay, float pupilMeanSize, float blinkFrequency, float blinkClosedToOpenedLength, float saccadeFrequency, float fixationLength, int fixationIndex, string filePath)
	{
		eventNames.Add(eventName);
		GazeEvent newEvent = new GazeEvent(eventName, eventTime, eventHitPoint, eventHitObjectPosition, eventHitScale, eventHitRotation, eventHitColor, eventHitName, eventGazeRay, pupilMeanSize, blinkFrequency, blinkClosedToOpenedLength, saccadeFrequency, fixationLength, fixationIndex, filePath);
		gazeDataList.Add(newEvent);
	}

	public List<GazeEvent> GazeDataList
	{
		get
		{
			return gazeDataList;
		}
	}

	public HashSet<string> EventNames
	{
		get
		{
			return eventNames;
		}
	}
}

[System.Serializable]
public class GazeEvent
{
	public string eventName;
	public float eventTime;
	public Vector3 eventHitPoint;
	public Vector3 eventHitObjectPosition;
	public Vector3 eventHitScale;
	public Quaternion eventHitRotation;
	public Color eventHitColor;
	public string eventHitName;
	public Ray eventGazeRay;
	public float pupilMeanSize;
	public float blinkFrequency;
	public float blinkClosedToOpenedLength;
	public float saccadeFrequency;
	public float fixationLength;
	public int fixationIndex;
	public string filePath;

	public GazeEvent()
	{
		this.eventName = "";
		this.eventTime = 0f;
		this.eventHitPoint = Vector3.zero;
		this.eventHitObjectPosition = Vector3.zero;
		this.eventHitScale = Vector3.zero;
		this.eventHitRotation = Quaternion.identity;
		this.eventHitColor = Color.white;
		this.eventHitName = "";
		this.eventGazeRay = new Ray();
		this.pupilMeanSize = 0f;
		this.blinkFrequency = 0f;
		this.blinkClosedToOpenedLength = 0f;
		this.saccadeFrequency = 0f;
		this.fixationLength = 0f;
		this.fixationIndex = 0;
		this.filePath = "";
	}

	public GazeEvent(string eventName, 
	                 float eventTime,
	                 Vector3 eventHitPoint,
	                 Vector3 eventHitObjectPosition,
	                 Vector3 eventHitScale,
	                 Quaternion eventHitRotation,
	                 Color eventHitColor,
	                 string eventHitName, 
	                 Ray eventGazeRay, 
	                 float pupilMeanSize, 
	                 float blinkFrequency, 
	                 float blinkClosedToOpenedLength, 
	                 float saccadeFrequency, 
	                 float fixationLength, 
	                 int fixationIndex,
	                 string filePath)
	{
		this.eventName = eventName;
		this.eventTime = eventTime;
		this.eventHitPoint = eventHitPoint;
		this.eventHitObjectPosition = eventHitObjectPosition;
		this.eventHitScale = eventHitScale;
		this.eventHitRotation = eventHitRotation;
		this.eventHitColor = eventHitColor;
		this.eventHitName = eventHitName;
		this.eventGazeRay = eventGazeRay;
		this.pupilMeanSize = pupilMeanSize;
		this.blinkFrequency = blinkFrequency;
		this.blinkClosedToOpenedLength = blinkClosedToOpenedLength;
		this.saccadeFrequency = saccadeFrequency;
		this.fixationLength = fixationLength;
		this.fixationIndex = fixationIndex;
		this.filePath = filePath;
	}

}
