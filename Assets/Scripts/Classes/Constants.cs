using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {

	public const string SCENE_LOGIN = "Login";
	public const string SCENE_FARM = "Farm";

	public const int ORIENTATION_NORTH = 0;
	public const int ORIENTATION_SOUTH = 1;
	public const int ORIENTATION_EAST = 2;
	public const int ORIENTATION_WEST = 3;

	public const int USERNAME_MIN_LENGTH = 3;
	public const int USERNAME_MAX_LENGTH = 16;
	public const int PASSWORD_MIN_LENGTH = 4;
	public const int PASSWORD_MAX_LENGTH = 16;

	public const string LOGIN_ERROR_USERNAME_LENGTH = "Username must be 3-16 characters long.";
	public const string LOGIN_ERROR_PASSWORD_LENGTH = "Password must be 4-16 characters long.";
	public const string LOGIN_SUCCESS = "Login success!";
	public const string LOGIN_FAIL = "Login fail.";

	public const int COIN_START = 10000;
	public const int CASH_START = 200;

	public const string DB_TYPE_ACCOUNT = "accounts";
}
