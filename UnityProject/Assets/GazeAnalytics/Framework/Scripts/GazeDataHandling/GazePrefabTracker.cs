using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[InitializeOnLoad]
public class GazePrefabTracker : MonoBehaviour
{
	[SerializeField]
	private string
		assetPath;

	public GazePrefabTracker()
	{
		SetAssetPath();
	}

	public void SetAssetPath()
	{
		if(assetPath == "")
		{
			assetPath = AssetDatabase.GetAssetPath(gameObject);
		}
	}

	public string GetAssetPath()
	{
		return this.assetPath;
	}
}
