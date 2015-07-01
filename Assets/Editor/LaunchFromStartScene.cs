using UnityEngine;
using UnityEditor;
using System.Collections;

public class LaunchFromStartScene : MonoBehaviour {
	
	[MenuItem("Edit/Start from Login %1")]
	public static void PlayFromLogin()
	{
		if ( EditorApplication.isPlaying == true )
		{
			EditorApplication.isPlaying = false;
			return;
		}
		
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene("Assets/Scenes/Login.unity");
		EditorApplication.isPlaying = true;
	}

	[MenuItem("Edit/Start from Control Panel %2")]
	public static void PlayFromControlPanel()
	{
		if ( EditorApplication.isPlaying == true )
		{
			EditorApplication.isPlaying = false;
			return;
		}
		
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene("Assets/Scenes/Control Panel.unity");
		EditorApplication.isPlaying = true;
	}
}
