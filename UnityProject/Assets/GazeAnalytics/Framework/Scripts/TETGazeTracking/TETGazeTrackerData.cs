using UnityEngine;
using System.Collections;
using TETCSharpClient;
using TETCSharpClient.Data;
using Assets.Scripts;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TETGazeTrackerData : MonoBehaviour, IGazeListener
{
	[SerializeField]
	private bool showEyeData;
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
		trackerIsActive = GazeManager.Instance.IsActivated;
	}

	public void OnGazeUpdate(GazeData gazeData)
	{
		//Add frame to GazeData cache handler
		gazeUtils.Update(gazeData);
	}

	void OnGUI()
	{
        if(showEyeData)
        {
		    int padding = 10;
		    int btnWidth = 180;
		    int btnHeight = 20;
		    int y = padding;

		    if(GUI.Button(new Rect(padding, y, btnWidth, btnHeight), "Press to Exit"))
		    {
			    Application.Quit();
		    }

		    if(!GazeManager.Instance.IsActivated)
		    {
			    y += btnHeight + padding;
			    GUI.TextArea(new Rect(padding, y, btnWidth, btnHeight), "EyeTribe Server not running!");

		    }
		    else if(!GazeManager.Instance.IsCalibrated)
		    {
			    y += btnHeight + padding;
			    GUI.TextArea(new Rect(padding, y, btnWidth, btnHeight), "EyeTribe Server not calibrated!");
		    }

			y += btnHeight + padding;
			GUI.TextArea(new Rect(padding, y, btnWidth, btnHeight), gazeUtils.EyesCloseTime.ToString());
			y += btnHeight + padding;
			GUI.TextArea(new Rect(padding, y, btnWidth, btnHeight), "curfix: " + gazeUtils.CurrentFixationTime.ToString());
			y += btnHeight + padding;
			GUI.TextArea(new Rect(padding, y, btnWidth, btnHeight), "lastfix: " + gazeUtils.LastFixationTime.ToString());
			y += btnHeight + padding;
			GUI.TextArea(new Rect(padding, y, btnWidth, btnHeight), "closetime: " + gazeUtils.EyesCloseTime.ToString());
			y += btnHeight + padding;
			GUI.TextArea(new Rect(padding, y, btnWidth, btnHeight), "blinks: " + gazeUtils.BlinkCount.ToString());
//			y += btnHeight + padding;
//			GUI.TextArea(new Rect(padding, y, btnWidth, btnHeight), "blinks: " + gazeUtils.Dist.ToString());
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
	}

	void OnApplicationQuit()
	{
		GazeManager.Instance.RemoveGazeListener(this);
		GazeManager.Instance.Deactivate();
	}

	public Vector3 GetGazeScreenPosition()
	{
		Point2D gp = gazeUtils.GetLastValidSmoothedGazeCoordinates();
//		Point2D gp = gazeUtils.GetLastValidRawGazeCoordinates();

		if(null != gp)
		{
			Point2D sp = UnityGazeUtils.getGazeCoordsToUnityWindowCoords(gp);
//			Debug.Log("gaze: " + ((float)sp.X).ToString() + "," +((float)sp.Y).ToString());
//			Debug.Log("mouse: " + Input.mousePosition.x.ToString() + "," + Input.mousePosition.y.ToString());
			return new Vector3((float)sp.X, (float)sp.Y, 0f);
//			return new Vector3((float)gp.X, (float)gp.Y, 0f);
		}
		else
		{
			return Vector3.zero;
		}

	}

	public float GetMeanPupilDilation()
	{
		if(gazeUtils.GetLastValidLeftEye() != null && gazeUtils.GetLastValidRightEye() != null)
		{
			float pupil_left = (float)gazeUtils.GetLastValidLeftEye().PupilSize;
			float pupil_right = (float)gazeUtils.GetLastValidRightEye().PupilSize;
			float pupil = (pupil_left + pupil_right) * 0.5f;
			return pupil;
		}
		return 0f;
	}

	public float GetEyesClosedTime()
	{
		return gazeUtils.EyesCloseTime;
	}

	public int GetBlinkCount()
	{
		return gazeUtils.BlinkCount;
	}

	public float GetCurrentFixationLength()
	{
		return gazeUtils.CurrentFixationTime;
	}

	public float GetLastFixationLength()
	{
		return gazeUtils.LastFixationTime;
	}

	public int GetFixationIndex()
	{
		return gazeUtils.FixationIndex;
	}

	public bool isFixating()
	{
		return gazeUtils.isFixating();
	}

	public bool TrackerIsActive
	{
		get
		{
			return trackerIsActive;
		}
	}
}
