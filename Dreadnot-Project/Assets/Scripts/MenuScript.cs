using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour
{
	public GUIStyle style;

	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width/2-128, Screen.height*0.3f, 256, 64), "TEST SHIPS", style))
		{
			Application.LoadLevel("SceneWorld");
		}
		if(GUI.Button(new Rect(Screen.width/2-128, Screen.height*0.6f-64, 256, 64), "BUILD SHIPS", style))
		{
			Application.LoadLevel("SceneBuild");
		}
	}
}