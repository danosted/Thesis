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

	public void NewGazeEvent(string eventName, float eventTime)
	{
		eventNames.Add(eventName);
		GazeEvent newEvent = new GazeEvent(eventName, eventTime);
		gazeDataList.Add(newEvent);
	}

	public void NewGazeEvent(string eventName, 
	                         float eventTime,
	                         Vector3 eventHitPoint,
	                         Vector3 eventHitObjectPosition,
	                         Vector3 eventHitScale,
	                         Quaternion eventHitRotation,
	                         Color eventHitColor,
	                         string eventHitName, 
	                         Ray eventGazeRay, 
	                         float pupilMeanSize, 
	                         int blinkCount, 
	                         float eyeClosedTime, 
	                         int saccadeCount, 
	                         float saccadeJumpLength,
	                         float fixationLength, 
	                         int fixationIndex,
	                         string filePath)
	{
		eventNames.Add(eventName);
		GazeEvent newEvent = new GazeEvent(eventName, eventTime, eventHitPoint, eventHitObjectPosition, eventHitScale, eventHitRotation, eventHitColor, eventHitName, eventGazeRay, pupilMeanSize, blinkCount, eyeClosedTime, saccadeCount, saccadeJumpLength, fixationLength, fixationIndex, filePath);
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
	public int blinkCount;
	public float eyeClosedTime;
	public int saccadeCount;
	public float saccadeJumpLength;
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
		this.blinkCount = 0;
		this.eyeClosedTime = 0f;
		this.saccadeCount = 0;
		this.saccadeJumpLength = 0f;
		this.fixationLength = 0f;
		this.fixationIndex = 0;
		this.filePath = "";
	}

	public GazeEvent(string eventName, float eventTime)
	{
		this.eventName = eventName;
		this.eventTime = eventTime;
		this.eventHitPoint = Vector3.zero;
		this.eventHitObjectPosition = Vector3.zero;
		this.eventHitScale = Vector3.zero;
		this.eventHitRotation = Quaternion.identity;
		this.eventHitColor = Color.white;
		this.eventHitName = "";
		this.eventGazeRay = new Ray();
		this.pupilMeanSize = 0f;
		this.blinkCount = 0;
		this.eyeClosedTime = 0f;
		this.saccadeCount = 0;
		this.saccadeJumpLength = 0f;
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
	                 int blinkCount, 
	                 float eyeClosedTime, 
	                 int saccadeCount, 
	                 float saccadeJumpLength,
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
		this.blinkCount = blinkCount;
		this.eyeClosedTime = eyeClosedTime;
		this.saccadeCount = saccadeCount;
		this.saccadeJumpLength = saccadeJumpLength;
		this.fixationLength = fixationLength;
		this.fixationIndex = fixationIndex;
		this.filePath = filePath;
	}

}
