using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;


[InitializeOnLoad]
#endif
public class GazePrefabTracker : MonoBehaviour
{
	[SerializeField]
	private string
		assetPath = "";
	#if UNITY_EDITOR
	public void SetAssetPath()
	{
		if(assetPath == "")
		{
			assetPath = AssetDatabase.GetAssetPath(gameObject);
		}
	}
	#endif
	public string GetAssetPath()
	{
		return this.assetPath;
	}
}
