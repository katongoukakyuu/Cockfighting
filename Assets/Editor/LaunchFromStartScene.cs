using UnityEngine;
using UnityEditor;
using System.Collections;

public class LaunchFromStartScene : MonoBehaviour {
	
	[MenuItem("Edit/Play-Stop, But From Prelaunch Scene %0")]
	public static void PlayFromPrelaunchScene()
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
}
