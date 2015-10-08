using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using System.IO;
using Facebook.Unity;

public class LoginManager : MonoBehaviour {

	public RectTransform loginPanel;
	public RectTransform registerPanel;
	public InputField loginPanelUsernameInput;
	public InputField loginPanelPasswordInput;
	public InputField registerPanelUsernameInput;
	public InputField registerPanelEmailInput;
	public Text errorText;

	public float animDuration = 0.1f;
	public int animSteps = 10;
	public float xDist = 1280f;

	private bool isAnimating;

	private static LoginManager instance;
	private LoginManager() {}

	public static LoginManager Instance {
		get {
			if(instance == null) {
				instance = (LoginManager)GameObject.FindObjectOfType(typeof(LoginManager));
			}
			return instance;
		}
	}

	public void LoginScreenButtonLogin() {
		if(loginPanelUsernameInput.text.Length < Constants.USERNAME_MIN_LENGTH ||
		   loginPanelUsernameInput.text.Length > Constants.USERNAME_MAX_LENGTH) {
			DisplayError(Constants.LOGIN_ERROR_USERNAME_LENGTH);
			return;
		}
		if(loginPanelPasswordInput.text.Length < Constants.PASSWORD_MIN_LENGTH ||
		   loginPanelPasswordInput.text.Length > Constants.PASSWORD_MAX_LENGTH) {
			DisplayError(Constants.LOGIN_ERROR_PASSWORD_LENGTH);
			return;
		}
		/*if(DatabaseManager.Instance.LoginAccount(loginPanelUsernameInput.text,loginPanelPasswordInput.text)) {
			Application.LoadLevel(Constants.SCENE_FARM);
		}
		else
			DisplayError(Constants.LOGIN_FAIL);*/
	}

	public void LoginScreenButtonFacebookLogin() {
		var perms = new List<string>(){"public_profile", "email", "user_friends"};
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}
	
	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			print(aToken.UserId);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				print(perm);
			}

			//FB.API ("/me",HttpMethod.GET,PrintName);

			if(!DatabaseManager.Instance.LoginAccount(aToken.UserId)) {
				Dictionary<string, object> d = GameManager.Instance.RegisterAccount(aToken.UserId,"");
				DatabaseManager.Instance.RegisterAccount(d);
				DatabaseManager.Instance.LoginAccount(aToken.UserId);
			}

			Application.LoadLevel(Constants.SCENE_FARM);
		} else {
			print("User cancelled login");
		}
	}

	private void PrintName(IResult result) {
		print ("result: " + result.RawResult);
		IDictionary id = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
		print ("name: " + id["name"]);
	}

	public void RegisterScreenButtonRegister() {
		if(registerPanelUsernameInput.text.Length < Constants.USERNAME_MIN_LENGTH ||
		   registerPanelUsernameInput.text.Length > Constants.USERNAME_MAX_LENGTH) {
			DisplayError(Constants.LOGIN_ERROR_USERNAME_LENGTH);
			return;
		}
		//Dictionary<string, object> d = GameManager.Instance.RegisterAccount(registerPanelUsernameInput.text,registerPanelEmailInput.text,"");
		//DatabaseManager.Instance.RegisterAccount(d);
	}

	private void DisplayError(string msg) {
		errorText.text = msg;
	}

	public void SwitchScreen(int screen) {
		if (!isAnimating)
			StartCoroutine (SwitchScreen(animDuration, animSteps, screen));
	}

	public void ReinitializeDatabase() {
		DatabaseManager.Instance.ReinitializeDatabase();
	}

	private IEnumerator SwitchScreen(float animDuration, int animSteps, int screen) {
		/*
		 * screen:
		 * 0 - login screen
		 * 1 - register screen
		 */

		float posIncX = xDist / animSteps;
		float animWait = animDuration / animSteps;
		
		isAnimating = true;
		errorText.text = "";
		if(screen == 1)
			posIncX *= -1;

		for(int x = 0; x < animSteps; x++) {
			loginPanel.anchoredPosition = new Vector2(loginPanel.anchoredPosition.x + posIncX,
			                                          loginPanel.anchoredPosition.y);

			registerPanel.anchoredPosition = new Vector2(registerPanel.anchoredPosition.x + posIncX,
			                                             registerPanel.anchoredPosition.y);
			
			yield return new WaitForSeconds(animWait);
		}

		isAnimating = false;
	}

}
