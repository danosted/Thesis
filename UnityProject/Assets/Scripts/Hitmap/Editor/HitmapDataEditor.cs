using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(HitmapDataGatherer))]
public class HitmapDataEditor : Editor
{
	override public void OnInspectorGUI()
	{
		HitmapDataGatherer gatherer = target as HitmapDataGatherer;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Hitmap of Gaze and target Information", EditorStyles.largeLabel);
		EditorGUILayout.EndHorizontal();
		if(GUILayout.Button("Load saved data"))
		{
			try
			{
				gatherer.LoadData();

			}
			catch(System.Exception e)
			{
				Debug.Log(e);
			}
		}
	}

}
