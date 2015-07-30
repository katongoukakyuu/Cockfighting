using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Constants : MonoBehaviour {

	public const string DB_NAME = "cockfighting";

	public const string SCENE_LOGIN = "Login";
	public const string SCENE_FARM = "Farm";
	public const string SCENE_CONTROL_PANEL = "Control Panel";
	public const string SCENE_CONTROL_PANEL_BREEDS = "Control Panel - Breeds";
	public const string SCENE_CONTROL_PANEL_BUILDINGS = "Control Panel - Buildings";
	public const string SCENE_CONTROL_PANEL_FEEDS = "Control Panel - Feeds";

	public const string PATH_SPRITES = "Sprites/";

	public const string FARM_MANAGER_STATE_FREE_SELECT = "Free Select";
	public const string FARM_MANAGER_STATE_BUILD_STRUCTURE = "Build Structure";

	public const string FEEDS_MANAGER_STATE_FREE_SELECT = "Free Select";
	public const string FEEDS_MANAGER_STATE_ASSIGN_ITEM = "Assign Item";

	public const string ORIENTATION_NORTH = "north";
	public const string ORIENTATION_SOUTH = "south";
	public const string ORIENTATION_EAST = "east";
	public const string ORIENTATION_WEST = "west";

	public const int USERNAME_MIN_LENGTH = 3;
	public const int USERNAME_MAX_LENGTH = 16;
	public const int PASSWORD_MIN_LENGTH = 4;
	public const int PASSWORD_MAX_LENGTH = 16;

	public const string TIMER_DEFAULT = "0:00:00:00";

	public const string LOGIN_ERROR_USERNAME_LENGTH = "Username must be 3-16 characters long.";
	public const string LOGIN_ERROR_PASSWORD_LENGTH = "Password must be 4-16 characters long.";
	public const string LOGIN_SUCCESS = "Login success!";
	public const string LOGIN_FAIL = "Login fail.";

	public const string GENERIC_ERROR_FIELDS = "Please fill up all the fields.";

	// schedule panel messages
	public const string MESSAGE_SCHEDULE_PANEL_1 = "Click the icon to assign an item!";
	// end schedule panel messages

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

	public const string DB_COUCHBASE_ID = "_id";

	public const string DB_KEYWORD_TYPE = "type";
	public const string DB_TYPE_ACCOUNT = "account";
	public const string DB_TYPE_CHICKEN = "chicken";
	public const string DB_TYPE_BREED = "breed";
	public const string DB_TYPE_BUILDING = "building";
	public const string DB_TYPE_BUILDING_OWNED = "building owned";
	public const string DB_TYPE_FEEDS = "feeds";
	public const string DB_TYPE_FEEDS_SCHEDULE = "feeds schedule";
	public const string DB_TYPE_FIGHTING_MOVE = "fighting move";
	public const string DB_TYPE_FIGHTING_MOVE_OWNED = "fighting move owned";
	public const string DB_TYPE_REPLAY = "replay";
	public const string DB_TYPE_REPLAY_TURN = "replay turn";

	public const string DB_KEYWORD_ID = "_id";
	public const string DB_KEYWORD_NAME = "name";
	public const string DB_KEYWORD_DESCRIPTION = "description";
	public const string DB_KEYWORD_CREATED_AT = "created at";
	public const string DB_KEYWORD_CHICKEN_ID = "chicken id";
	public const string DB_KEYWORD_FEEDS_ID = "feeds id";
	public const string DB_KEYWORD_FIGHTING_MOVE_ID = "fighting move id";

	// player document keywords
	public const string DB_KEYWORD_USERNAME = "username";
	public const string DB_KEYWORD_PASSWORD = "password";
	public const string DB_KEYWORD_EMAIL = "email";
	public const string DB_KEYWORD_MATCHES_WON = "matches won";
	public const string DB_KEYWORD_MATCHES_LOST = "matches lost";
	public const string DB_KEYWORD_MATCHES_TIED = "matches tied";
	public const string DB_KEYWORD_COIN = "coin";
	public const string DB_KEYWORD_CASH = "cash";
	// end player document keywords

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

	// building-owned-by-player keywords
	public const string DB_KEYWORD_MAP_NAME = "map name";
	public const string DB_KEYWORD_X_POSITION = "x position";
	public const string DB_KEYWORD_Y_POSITION = "y position";
	public const string DB_KEYWORD_ORIENTATION = "orientation";
	// end building-owned-by-player keywords

	// feeds document keywords
	public const string DB_KEYWORD_DURATION_DAYS = "days";
	public const string DB_KEYWORD_DURATION_HOURS = "hours";
	public const string DB_KEYWORD_DURATION_MINUTES = "minutes";
	public const string DB_KEYWORD_DURATION_SECONDS = "seconds";
	// end feeds document keywords

	// feeds schedule document keywords
	public const string DB_KEYWORD_ORDER = "order";
	// end feeds schedule document keywords

	// replay document keywords
	public const string DB_KEYWORD_CHICKEN_ID_1 = "chicken id 1";
	public const string DB_KEYWORD_CHICKEN_ID_2 = "chicken id 2";
	public const string DB_KEYWORD_REPLAY = "replay";
	// end replay document keywords

	public const string GENDER_MALE = "male";
	public const string GENDER_FEMALE = "female";

	public const string LIFE_STAGE_EGG = "egg";
	public const string LIFE_STAGE_CHICK = "chick";
	public const string LIFE_STAGE_STAG = "stag";
	public const string LIFE_STAGE_COCK = "cock";
	public const string LIFE_STAGE_HEN = "hen";

	// schedule panel children
	public const string SCHEDULE_PANEL_ICON = "Inventory Icon";
	public const string SCHEDULE_PANEL_ICON_COUNT = "Inventory Icon/Panel/Inventory Count";
	public const string SCHEDULE_PANEL_NAME = "Info/Name/Name Text";
	public const string SCHEDULE_PANEL_STATS = "Info/Stats/Stats Text";
	public const string SCHEDULE_PANEL_TIMER = "Timer/Timer/Timer Text";
	public const string SCHEDULE_PANEL_HURRY_BUTTON = "Timer/Buttons/Hurry";
	public const string SCHEDULE_PANEL_CHANGE_BUTTON = "Timer/Buttons/Change";
	public const string SCHEDULE_PANEL_CANCEL_BUTTON = "Timer/Buttons/Cancel";
	// end schedule panel children

	// fight move list
	public const string FIGHT_MOVE_NONE = "None";
	public const string FIGHT_MOVE_DASH = "Dash";
	public const string FIGHT_MOVE_FLYING_TALON = "Flying Talon";
	public const string FIGHT_MOVE_SIDESTEP = "Sidestep";
	public const string FIGHT_MOVE_PECK = "Peck";

	public static List<string> FIGHT_MOVES = new List<string>() {
		FIGHT_MOVE_DASH,
		FIGHT_MOVE_FLYING_TALON,
		FIGHT_MOVE_SIDESTEP,
		FIGHT_MOVE_PECK
	};
	// end fight move list

	public const string REPLAY_ATK1 = "atk1";
	public const string REPLAY_DEF1 = "def1";
	public const string REPLAY_HP1 = "hp1";
	public const string REPLAY_AGI1 = "agi1";
	public const string REPLAY_GAM1 = "gam1";
	public const string REPLAY_AGG1 = "agg1";
	public const string REPLAY_X1 = "x1";
	public const string REPLAY_Y1 = "y1";
	public const string REPLAY_MOVE1 = "move1";
	public const string REPLAY_ATK2 = "atk2";
	public const string REPLAY_DEF2 = "def2";
	public const string REPLAY_HP2 = "hp2";
	public const string REPLAY_AGI2 = "agi2";
	public const string REPLAY_GAM2 = "gam2";
	public const string REPLAY_AGG2 = "agg2";
	public const string REPLAY_X2 = "x2";
	public const string REPLAY_Y2 = "y2";
	public const string REPLAY_MOVE2 = "move2";
}
