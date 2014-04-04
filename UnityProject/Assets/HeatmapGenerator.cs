using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class HeatmapGenerator : MonoBehaviour
{
	private string eventName;
	private float eventValue;
	private Vector3 eventPosition;
	
	private string[] heatmapNames;
	private float[] heatmapValues;
	private Vector3[] heatmapPositions;

	private Dictionary<string, Dictionary<float, Vector3>> nameToValueToPosition;
	private Dictionary<string, float> nameToValue;
	private Dictionary<string, Vector3> nameToPosition;

	private void GetYo()
	{

	}
	
}
