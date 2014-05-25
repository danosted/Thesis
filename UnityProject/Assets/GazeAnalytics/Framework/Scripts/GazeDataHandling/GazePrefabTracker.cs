using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GazePrefabTracker : MonoBehaviour
{

	[SerializeField]
	private string
		assetPath;

	public void SetAssetPath()
	{
		if(assetPath == "")
		{
			assetPath = AssetDatabase.GetAssetPath(this.gameObject);
		}
	}

	public string GetAssetPath()
	{
		return this.assetPath;
	}
}
