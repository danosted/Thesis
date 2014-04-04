using UnityEngine;
using System.Collections;

public class GA_ExampleManager : MonoBehaviour
{
	private string _exampleGameKey = "9d8d64b2bde76d5cf289f5fa7ce70216";
	private string _exampleSecretKey = "008cca260245c4f7302ef4c7c1c18c4558b4cca4";

	void Start()
	{
		if(!GA.SettingsGA.SendExampleGameDataToMyGame)
		{
			GA.API.Submit.SetupKeys(_exampleGameKey, _exampleSecretKey);
			GA.Log("Changed GameAnalytics Game Key and Secret Key for this example game. To send example game data to your own game enable Get Example Game Data under GA_Settings > Debug");
		}
		else
		{
			GA.Log("Sending example game data to your game. To stop sending example game data to your own game disable Get Example Game Data under GA_Settings > Debug");
		}
	}
	
	void OnGUI()
	{
		if(Time.time < 5)
		{
			if(!GA.SettingsGA.SendExampleGameDataToMyGame)
			{
				GUI.Box(new Rect(Screen.width / 2 - 220, Screen.height / 2 - 12, 440, 24), "Example Game Data will NOT be sent to your game (see log for details).");
			}
			else
			{
				GUI.Box(new Rect(Screen.width / 2 - 220, Screen.height / 2 - 12, 440, 24), "Example Game Data WILL be sent to your game (see log for details).");
			}
		}
	}
}
