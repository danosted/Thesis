using UnityEngine;
using System.Collections;

public class GA_ExampleController : MonoBehaviour
{
	public float Speed = 1.0f;
	public float MaxVelocityChange = 5.3f;

	[SerializeField]
	private GazeAngryBotsWrap gazeHandler;

	private int i = 0;

	void Start()
	{

	}
	
	void Update ()
	{
//		Vector3 screen_coord = gazeHandler.GetGazeScreenPosition();
		Vector3 screen_coord = Input.mousePosition;
		Vector3 world_coord = Camera.main.ScreenToWorldPoint(new Vector3(screen_coord.x, screen_coord.y, transform.position.z - Camera.main.transform.position.z));
		gazeHandler.transform.position = world_coord;
//		Debug.Log("screen: " + screen_coord + ", world: " + world_coord);
		float pos_x = world_coord.x;
		pos_x =  Mathf.Clamp(pos_x, -MaxVelocityChange, MaxVelocityChange);
		transform.position = new Vector3(pos_x, transform.position.y, transform.position.z);
//		Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
//		targetVelocity *= Speed;
		
		// Apply a force that attempts to reach our target velocity
//		Vector3 velocity = rigidbody.velocity;
//		Vector3 velocityChange = (targetVelocity - velocity);
//		velocityChange.x = Mathf.Clamp(velocityChange.x, -MaxVelocityChange, MaxVelocityChange);
//		velocityChange.y = 0;
//		velocityChange.z = 0;
//		rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
		if(i < 5)
		{
			GA.API.Design.NewEvent("Test", i, new Vector3(0f,0f,0f));
			i++;
		}
	}
}
