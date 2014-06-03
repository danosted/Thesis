using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GazePrefabTracker))]
public class GazePrefabTrackerEditor : Editor
{
	override public void OnInspectorGUI()
	{
		GazePrefabTracker gazePrefabTracker = target as GazePrefabTracker;
		gazePrefabTracker.SetAssetPath();

//		serializedObject.Update();
//		EditorGUILayout.PropertyField(serializedObject.FindProperty("assetPath"), false);
//		serializedObject.ApplyModifiedProperties();
	}
}