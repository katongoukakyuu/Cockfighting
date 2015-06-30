using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {

	public const string DB_NAME = "cockfighting";

	public const string SCENE_LOGIN = "Login";
	public const string SCENE_FARM = "Farm";
	public const string SCENE_CONTROL_PANEL = "Control Panel";
	public const string SCENE_CONTROL_PANEL_BREEDS = "Control Panel - Breeds";
	public const string SCENE_CONTROL_PANEL_BUILDINGS = "Control Panel - Buildings";

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

	public const string GENERIC_ERROR_FIELDS = "Please fill up all the fields.";

	public const int COIN_START = 10000;
	public const int CASH_START = 200;

	public const int CHICKEN_ATTACK_DEFAULT_START = 1000;
	public const int CHICKEN_DEFENSE_DEFAULT_START = 1000;
	public const int CHICKEN_HP_DEFAULT_START = 1000;
	public const int CHICKEN_AGILITY_DEFAULT_START = 1000;
	public const int CHICKEN_GAMENESS_DEFAULT_START = 1000;
	public const int CHICKEN_AGGRESSION_DEFAULT_START = 1000;

	public const int CHICKEN_ATTACK_DEFAULT_MAX = 2500;
	public const int CHICKEN_DEFENSE_DEFAULT_MAX = 2500;
	public const int CHICKEN_HP_DEFAULT_MAX = 2500;
	public const int CHICKEN_AGILITY_DEFAULT_MAX = 2500;
	public const int CHICKEN_GAMENESS_DEFAULT_MAX = 2500;
	public const int CHICKEN_AGGRESSION_DEFAULT_MAX = 2500;

	public const string DB_KEYWORD_TYPE = "type";
	public const string DB_TYPE_ACCOUNT = "account";
	public const string DB_TYPE_CHICKEN = "chicken";
	public const string DB_TYPE_BREED = "breed";
	public const string DB_TYPE_BUILDING = "building";
	public const string DB_KEYWORD_NAME = "name";
	public const string DB_KEYWORD_DESCRIPTION = "description";
	public const string DB_KEYWORD_CREATED_AT = "created at";

	// breed document keywords
	public const string DB_KEYWORD_HEAD_COLOR_1 = "head color 1";
	public const string DB_KEYWORD_HEAD_COLOR_1_CHANCE = "head color 1 chance";
	public const string DB_KEYWORD_HEAD_COLOR_2 = "head color 2";
	public const string DB_KEYWORD_HEAD_COLOR_2_CHANCE = "head color 2 chance";

	public const string DB_KEYWORD_BODY_COLOR_1 = "body color 1";
	public const string DB_KEYWORD_BODY_COLOR_1_CHANCE = "body color 1 chance";
	public const string DB_KEYWORD_BODY_COLOR_2 = "body color 2";
	public const string DB_KEYWORD_BODY_COLOR_2_CHANCE = "body color 2 chance";

	public const string DB_KEYWORD_WING_COLOR_1 = "wing color 1";
	public const string DB_KEYWORD_WING_COLOR_1_CHANCE = "wing color 1 chance";
	public const string DB_KEYWORD_WING_COLOR_2 = "wing color 2";
	public const string DB_KEYWORD_WING_COLOR_2_CHANCE = "wing color 2 chance";

	public const string DB_KEYWORD_TAIL_COLOR_1 = "tail color 1";
	public const string DB_KEYWORD_TAIL_COLOR_1_CHANCE = "tail color 1 chance";
	public const string DB_KEYWORD_TAIL_COLOR_2 = "tail color 2";
	public const string DB_KEYWORD_TAIL_COLOR_2_CHANCE = "tail color 2 chance";
	// end breed document keywords

	// chicken document keywords
	public const string DB_KEYWORD_OWNER = "owner";
	public const string DB_KEYWORD_GENDER = "gender";
	public const string DB_KEYWORD_NOTES = "notes";
	public const string DB_KEYWORD_LIFE_STAGE = "life stage";

	public const string DB_KEYWORD_ATTACK = "attack";
	public const string DB_KEYWORD_DEFENSE = "defense";
	public const string DB_KEYWORD_HP = "hp";
	public const string DB_KEYWORD_AGILITY = "agility";
	public const string DB_KEYWORD_GAMENESS = "gameness";
	public const string DB_KEYWORD_AGGRESSION = "aggression";

	public const string DB_KEYWORD_ATTACK_MAX = "attack max";
	public const string DB_KEYWORD_DEFENSE_MAX = "defense max";
	public const string DB_KEYWORD_HP_MAX = "hp max";
	public const string DB_KEYWORD_AGILITY_MAX = "agility max";
	public const string DB_KEYWORD_GAMENESS_MAX = "gameness max";
	public const string DB_KEYWORD_AGGRESSION_MAX = "aggression max";
	// end chicken document keywords

	// building document keywords
	public const string DB_KEYWORD_COIN_COST = "coin cost";
	public const string DB_KEYWORD_CASH_COST = "cash cost";
	public const string DB_KEYWORD_X_SIZE = "x size";
	public const string DB_KEYWORD_Y_SIZE = "y size";
	public const string DB_KEYWORD_X_CENTER = "x center";
	public const string DB_KEYWORD_Y_CENTER = "y center";
	public const string DB_KEYWORD_PREFAB_NAME = "prefab name";
	public const string DB_KEYWORD_IMAGE_NAME = "image name";
	// end building document keywords

	public const string GENDER_MALE = "male";
	public const string GENDER_FEMALE = "female";

	public const string LIFE_STAGE_EGG = "egg";
	public const string LIFE_STAGE_CHICK = "chick";
	public const string LIFE_STAGE_STAG = "stag";
	public const string LIFE_STAGE_COCK = "cock";
	public const string LIFE_STAGE_HEN = "hen";
}
