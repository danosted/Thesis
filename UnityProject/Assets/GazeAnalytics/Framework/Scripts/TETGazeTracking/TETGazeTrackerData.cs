using UnityEngine;
using System.Collections;
using TETCSharpClient;
using TETCSharpClient.Data;
using Assets.Scripts;

public class TETGazeTrackerData : MonoBehaviour, IGazeListener
{
	private TETGazeTrackerDataValidator gazeUtils;

	private bool trackerIsActive;

	void Awake()
	{
		gazeUtils = new TETGazeTrackerDataValidator(15);

		//activate C# TET client
		GazeManager.Instance.Activate
            (
                GazeManager.ApiVersion.VERSION_1_0,
                GazeManager.ClientMode.Push
		);

		//register for gaze updates
		GazeManager.Instance.AddGazeListener(this);
		trackerIsActive = GazeManager.Instance.IsConnected;
	}

	public void OnGazeUpdate(GazeData gazeData)
	{
		//Add frame to GazeData cache handler
		gazeUtils.Update(gazeData);
	}

	void OnGUI()
	{
		int padding = 10;
		int btnWidth = 100;
		int btnHeight = 30;
		int y = padding;

		if(GUI.Button(new Rect(padding, y, btnWidth, btnHeight), "Press to Exit"))
		{
			Application.Quit();
		}

		if(!GazeManager.Instance.IsConnected)
		{
			y += btnHeight + padding;
			GUI.TextArea(new Rect(padding, y, 170, 20), "EyeTribe Server not running!");

		}
		else if(!GazeManager.Instance.IsCalibrated)
		{
			y += btnHeight + padding;
			GUI.TextArea(new Rect(padding, y, 190, 20), "EyeTribe Server not calibrated!");
		}
		y += btnHeight + padding;
		GUI.TextArea(new Rect(padding, y, 150, 20), gazeUtils.CloseTime.ToString());
		y += btnHeight + padding;
		GUI.TextArea(new Rect(padding, y, 150, 20), gazeUtils.BlinkCount.ToString());
		y += btnHeight + padding;
		GUI.TextArea(new Rect(padding, y, 150, 20), gazeUtils.HasClosed.ToString());
//		float pupil_left = (float)gazeUtils.GetLastValidLeftEye ().PupilSize;
//		float pupil_right = (float)gazeUtils.GetLastValidRightEye ().PupilSize;
//		Debug.Log ("left eye pupil: " + pupil_left);
//		Debug.Log ("right eye pupil: " + pupil_right);
//		float pupil = (pupil_left + pupil_right) * 0.5f;
//		Debug.Log("mean pupil: " + pupil);

//		if(gazeUtils.GetLeftEye() == null)
//		{
//			Debug.Log("left eye closed");
//		}
//		if(gazeUtils.GetRightEye() == null)
//		{
//			Debug.Log("right eye closed");
//		}
	}

	void OnApplicationQuit()
	{
		GazeManager.Instance.RemoveGazeListener(this);
		GazeManager.Instance.Deactivate();
	}

	public Vector3 GetGazeScreenPosition()
	{
		Point2D gp = gazeUtils.GetLastValidSmoothedGazeCoordinates();

		if(null != gp)
		{
			Point2D sp = UnityGazeUtils.getGazeCoordsToUnityWindowCoords(gp);
			return new Vector3((float)sp.X, (float)sp.Y, 0f);
		}
		else
		{
			return Vector3.zero;
		}

	}

	public float GetPupilDilationLeft()
	{
		float pupil_right = (float)gazeUtils.GetLastValidRightEye().PupilSize;
		return pupil_right;
	}

	public float GetPupilDilationRight()
	{
		float pupil_left = (float)gazeUtils.GetLastValidLeftEye().PupilSize;
		return pupil_left;
	}

	public float GetMeanPupilDilation()
	{
		float pupil_left = (float)gazeUtils.GetLastValidLeftEye().PupilSize;
		float pupil_right = (float)gazeUtils.GetLastValidRightEye().PupilSize;
		float pupil = (pupil_left + pupil_right) * 0.5f;
//		Debug.Log("pupil size: " + pupil);
		return pupil;
	}

	public long GetTimeSinceLastBlink()
	{
		return gazeUtils.CloseTime;
	}

	public int GetBlinkCount()
	{
		return gazeUtils.BlinkCount;
	}

	public bool isFixating()
	{
		return gazeUtils.isFixating();
		;
	}

	public bool TrackerIsActive
	{
		get
		{
			return trackerIsActive;
		}
	}
}
