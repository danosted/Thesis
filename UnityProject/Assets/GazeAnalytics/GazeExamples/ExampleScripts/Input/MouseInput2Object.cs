using UnityEngine;
using System.Collections;

public class MouseInput2Object : MonoBehaviour {
	
	public delegate void OnClickDelegate();
	public event OnClickDelegate OnClick;

	public delegate void OnPressDelegate();
	public event OnPressDelegate OnPress;

	public delegate void OnExitDelegate();
	public event OnExitDelegate OnExit;
	
	void OnMouseUp()
	{
		if(OnClick != null)
		{
			OnClick();
		}
		else
		{
			Debug.Log ("no listener to event", gameObject);
		}
	}

	void OnMouseExit()
	{
		if(OnExit != null)
		{
			OnExit();
		}
		else
		{
			Debug.Log ("no listener to event", gameObject);
		}
	}

	void OnMouseDown()
	{
		if(OnPress != null)
		{
			OnPress();
		}
		else
		{
			Debug.Log ("no listener to event", gameObject);
		}
	}

	void OnMouseEnter()
	{
		if(Input.GetMouseButton(0))
		{
			if(OnPress != null)
			{
				OnPress();
			}
			else
			{
				Debug.Log ("no listener to event", gameObject);
			}
		}
	}

	void OnMouseOver()
	{
		if(Input.GetMouseButtonUp(0))
		{
			if(OnClick != null)
			{
				OnClick();
			}
			else
			{
				Debug.Log ("no listener to event", gameObject);
			}
		}
	}
}
