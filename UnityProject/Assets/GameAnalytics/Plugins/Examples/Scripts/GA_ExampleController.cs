using UnityEngine;
using System.Collections;

public class GA_ExampleController : MonoBehaviour
{
	public float Speed = 1.0f;
	public float MaxVelocityChange = 5.3f;

	[SerializeField]
	private TETGazeData gazeHandler;

	private int i = 0;

	void Start()
	{

	}
	
	void Update ()
	{
		Vector3 screen_coord = Input.mousePosition;
		Vector3 world_coord = Camera.main.ScreenToWorldPoint(new Vector3(screen_coord.x, screen_coord.y, transform.position.z - Camera.main.transform.position.z));
		gazeHandler.transform.position = world_coord;

		float pos_x = world_coord.x;
		pos_x =  Mathf.Clamp(pos_x, -MaxVelocityChange, MaxVelocityChange);
		transform.position = new Vector3(pos_x, transform.position.y, transform.position.z);

		if(i < 500)
		{
			GA.API.Design.NewEvent("Test", i, new Vector3(i,i,i));
			i++;
		}
	}
}
