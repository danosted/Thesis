//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//using System.Collections.Generic;
//
//[InitializeOnLoad]
//[CustomEditor(typeof(GazePrefabTracker))]
//public class GazePrefabTrackerEditor : Editor
//{
//	static GazePrefabTrackerEditor()
//	{
//		GazePrefabTracker gazePrefabTracker = target as GazePrefabTracker;
//		gazePrefabTracker.SetAssetPath();
//	}
//
//	override public void OnInspectorGUI()
//	{
//		serializedObject.Update();
//		EditorGUILayout.PropertyField(serializedObject.FindProperty("assetPath"), false);
//		serializedObject.ApplyModifiedProperties();
//	}
//}