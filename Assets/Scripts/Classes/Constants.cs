using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Constants : MonoBehaviour {

	public const bool DEBUG = false;

	public const string DB_NAME = "cockfighting";

	public const string SCENE_LOGIN = "Login";
	public const string SCENE_FARM = "Farm";
	public const string SCENE_FIGHT_RING = "Fight Ring 2";
	public const string SCENE_CONTROL_PANEL = "Control Panel";
	public const string SCENE_CONTROL_PANEL_BREEDS = "Control Panel - Breeds";
	public const string SCENE_CONTROL_PANEL_BUILDINGS = "Control Panel - Buildings";
	public const string SCENE_CONTROL_PANEL_FEEDS = "Control Panel - Feeds";

	public const string PATH_SPRITES = "Sprites/";
	public const string PATH_SPRITES_STORE = "Store Icons/";
	public const string PATH_SPRITES_FEEDS = "Feeds Icons/";
	public const string PATH_SPRITES_TREATS = "Treats Icons/";
	public const string PATH_SPRITES_VITAMINS = "Vitamins Icons/";
	public const string PATH_SPRITES_SHOTS = "Shots Icons/";
	public const string PATH_PREFABS_BUILDINGS = "Prefabs/Buildings/";

	public const string FARM_MANAGER_STATE_FREE_SELECT = "Free Select";
	public const string FARM_MANAGER_STATE_BUILD_STRUCTURE = "Build Structure";

	public const string FEEDS_MANAGER_STATE_FREE_SELECT = "Free Select";
	public const string FEEDS_MANAGER_STATE_ASSIGN_ITEM = "Assign Item";

	public const string BREEDS_MANAGER_STATE_FREE_SELECT = "Free Select";
	public const string BREEDS_MANAGER_STATE_ASSIGN_MATE = "Assign Mate";

	public const string FIGHT_MANAGER_STATE_CATEGORY_SELECT = "Category Select";
	public const string FIGHT_MANAGER_STATE_MATCH_SELECT = "Match Select";

	public const string MAILBOX_MANAGER_STATE_VIEW_LIST = "Viewing List";
	public const string MAILBOX_MANAGER_STATE_VIEW_MAIL = "Viewing Mail";

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

	public const string GENERIC_TRUE = "true";
	public const string GENERIC_FALSE = "false";
	public const string GENERIC_CANCELED = "canceled";
	public const string GENERIC_HURRIED = "hurried";
	public const string GENERIC_CANCEL = "Cancel";
	public const string GENERIC_BACK = "Back";
	public const string GENERIC_ERROR_FIELDS = "Please fill up all the fields.";

	// schedule panel messages
	public const string MESSAGE_SCHEDULE_PANEL_1 = "Press the icon to assign an item!";
	public const string MESSAGE_SCHEDULE_PANEL_2 = "Press here to assign a mate!";
	public const string MESSAGE_SCHEDULE_PANEL_3 = "Choose a mate from the list to the left!";
	public const string MESSAGE_SCHEDULE_CANCEL_TITLE = "Cancel Schedule";
	public const string MESSAGE_SCHEDULE_CANCEL = "Are you sure you want to cancel the schedule?";
	public const string MESSAGE_SCHEDULE_HURRY_TITLE = "Hurry Schedule";
	public const string MESSAGE_SCHEDULE_HURRY = "Are you sure you want to hurry the schedule?";
	public const string MESSAGE_SCHEDULE_FEED = "Do you want to feed your chicken with this?";
	public const string MESSAGE_SCHEDULE_BREED_TITLE = "Confirm Breeding";
	// end schedule panel messages

	// match create messages
	public const string MESSAGE_MATCH_CREATE_NO_COCKS_AVAILABLE_TITLE = "No Cocks Available";
	public const string MESSAGE_MATCH_CREATE_NO_COCKS_AVAILABLE = "You have no cocks to fight with.";
	public const string MESSAGE_MATCH_CREATE_NOT_ENOUGH_COINS_TITLE = "Not Enough Coins";
	public const string MESSAGE_MATCH_CREATE_NOT_ENOUGH_COINS = "You do not have enough Coins to make the minimum bet.";
	public const string MESSAGE_MATCH_CREATE_CONFIRM_TITLE = "Confirm Fight";
	public const string MESSAGE_MATCH_CREATE_CONFIRM = "Are you sure you want to fight with this cock?";
	public const string MESSAGE_MATCH_CREATE_INCORRECT_PASSWORD_TITLE = "Incorrect Password";
	public const string MESSAGE_MATCH_CREATE_INCORRECT_PASSWORD = "The password is incorrect.";
	// end match create messages

	// match view fight messages
	public const string MESSAGE_MATCH_VIEW_CONFIRM_FIGHT_TITLE = "Confirm Fight";
	public const string MESSAGE_MATCH_VIEW_CONFIRM_FIGHT = "Are you sure you want to fight this cock?";
	public const string MESSAGE_MATCH_VIEW_CANCEL_FIGHT_TITLE = "Cancel Fight";
	public const string MESSAGE_MATCH_VIEW_CANCEL_FIGHT = "Are you sure you want to cancel this fight? All bets will be refunded.";
	public const string MESSAGE_MATCH_VIEW_CONFIRM_SINGLE_BET_1 = "Are you sure you want to bet ";
	public const string MESSAGE_MATCH_VIEW_CONFIRM_SINGLE_BET_2 = " Coins to fight this cock?";
	public const string MESSAGE_MATCH_VIEW_CONFIRM_SPECTATOR_BET_1 = "Are you sure you want to bet at least ";
	public const string MESSAGE_MATCH_VIEW_CONFIRM_SPECTATOR_BET_2 = " Coins to fight this cock?";
	// end match view fight messages

	// match bet fight messages
	public const string MESSAGE_MATCH_BET_CONFIRM_TITLE = "Confirm Bet";
	public const string MESSAGE_MATCH_BET_CONFIRM = "Are you sure you want to bet on the selected cock?";
	public const string MESSAGE_MATCH_BET_MESSAGE_1 = "With the odds at ";
	public const string MESSAGE_MATCH_BET_MESSAGE_2A = " for you, your winning payout will be ";
	public const string MESSAGE_MATCH_BET_MESSAGE_2B = " against you, your winning payout will be ";
	public const string MESSAGE_MATCH_BET_MESSAGE_3 = " Coins.";
	// end match bet fight messages

	// store messages
	public const string STORE_PURCHASE_CONFIRM_TITLE = "Confirm Purchase";
	public const string STORE_PURCHASE_CONFIRM = "Are you sure you want to buy the items?";
	// end store messages

	// automated mail messages
	public const string MAIL_FIGHT_CONCLUDED_TITLE = "Fight Concluded";
	public const string MAIL_FIGHT_CONCLUDED_MESSAGE_1 = "The fight between ";
	public const string MAIL_FIGHT_CONCLUDED_MESSAGE_2 = " of your farm and ";
	public const string MAIL_FIGHT_CONCLUDED_MESSAGE_3 = " of the ";
	public const string MAIL_FIGHT_CONCLUDED_MESSAGE_4 = " farm has concluded! Fight rewards has been distributed. Press \"View Replay\" to view the replay.";
	// end automated mail messages

	public const int COIN_START = 10000;
	public const int CASH_START = 200;

	public const int CHICKEN_ATTACK_DEFAULT_START = 1000;
	public const int CHICKEN_DEFENSE_DEFAULT_START = 1000;
	public const int CHICKEN_HP_DEFAULT_START = 1000;
	public const int CHICKEN_AGILITY_DEFAULT_START = 1000;
	public const int CHICKEN_GAMENESS_DEFAULT_START = 1000;
	public const int CHICKEN_AGGRESSION_DEFAULT_START = 1000;
	public const int CHICKEN_CONDITIONING_DEFAULT_START = 100;

	public const int CHICKEN_ATTACK_DEFAULT_MAX = 2000;
	public const int CHICKEN_DEFENSE_DEFAULT_MAX = 2000;
	public const int CHICKEN_HP_DEFAULT_MAX = 2000;
	public const int CHICKEN_AGILITY_DEFAULT_MAX = 2000;
	public const int CHICKEN_GAMENESS_DEFAULT_MAX = 2000;
	public const int CHICKEN_AGGRESSION_DEFAULT_MAX = 2000;
	public const int CHICKEN_CONDITIONING_DEFAULT_MAX = 100;
	public const int CHICKEN_CONDITIONING_DEFAULT_MIN = 50;

	public const double CHICKEN_CONDITIONING_DEFAULT_DECAY_TIMER = 1;
	public const int CHICKEN_CONDITIONING_DEFAULT_DECAY_AMOUNT = 10;

	public const double BREED_DURATION_DEFAULT_DAYS = 0d;
	public const double BREED_DURATION_DEFAULT_HOURS = 0d;
	public const double BREED_DURATION_DEFAULT_MINUTES = 0d;
	public const double BREED_DURATION_DEFAULT_SECONDS = 10d;

	public const string DB_COUCHBASE_ID = "_id";

	public const string DB_KEYWORD_TYPE = "type";
	public const string DB_KEYWORD_SUBTYPE = "subtype";
	public const string DB_TYPE_ACCOUNT = "account";
	public const string DB_TYPE_BET = "bet";
	public const string DB_TYPE_BETTING_ODDS = "betting odds";
	public const string DB_TYPE_BREED = "breed";
	public const string DB_TYPE_BUILDING = "building";
	public const string DB_TYPE_BUILDING_OWNED = "building owned";
	public const string DB_TYPE_CHICKEN = "chicken";
	public const string DB_TYPE_FEEDS = "feeds";
	public const string DB_TYPE_TREATS = "treats";
	public const string DB_TYPE_VITAMINS = "vitamins";
	public const string DB_TYPE_SHOTS = "shots";
	public const string DB_TYPE_FEEDS_SCHEDULE = "feeds schedule";
	public const string DB_TYPE_BREED_SCHEDULE = "breed schedule";
	public const string DB_TYPE_CONDITIONING_DECAY_SCHEDULE = "conditioning decay schedule";
	public const string DB_TYPE_FIGHTING_MOVE = "fighting move";
	public const string DB_TYPE_FIGHTING_MOVE_OWNED = "fighting move owned";
	public const string DB_TYPE_MAIL = "mail";
	public const string DB_TYPE_MATCH = "match";
	public const string DB_TYPE_MATCHMAKING_CATEGORY = "matchmaking category";
	public const string DB_TYPE_ITEM = "item";
	public const string DB_TYPE_ITEM_OWNED = "item owned";
	public const string DB_TYPE_REPLAY = "replay";
	public const string DB_TYPE_REPLAY_TURN = "replay turn";

	public const string DB_KEYWORD_NAME = "name";
	public const string DB_KEYWORD_DESCRIPTION = "description";
	public const string DB_KEYWORD_CREATED_AT = "created at";
	public const string DB_KEYWORD_CHICKEN_ID = "chicken id";
	public const string DB_KEYWORD_FEEDS_ID = "feeds id";
	public const string DB_KEYWORD_BREED_ID = "breed id";
	public const string DB_KEYWORD_FIGHTING_MOVE_ID = "fighting move id";

	// player document keywords
	public const string DB_KEYWORD_USER_ID = "user id";
	public const string DB_KEYWORD_USERNAME = "username";
	public const string DB_KEYWORD_PASSWORD = "password";
	public const string DB_KEYWORD_EMAIL = "email";
	public const string DB_KEYWORD_FARM_NAME = "farm name";
	public const string DB_KEYWORD_MATCHES_WON = "matches won";
	public const string DB_KEYWORD_MATCHES_LOST = "matches lost";
	public const string DB_KEYWORD_MATCHES_TIED = "matches tied";
	public const string DB_KEYWORD_COIN = "coin";
	public const string DB_KEYWORD_CASH = "cash";
	// end player document keywords

	// breed document keywords
	public const string DB_KEYWORD_ATTACK_GROWTH = "attack growth";
	public const string DB_KEYWORD_DEFENSE_GROWTH = "defense growth";
	public const string DB_KEYWORD_HP_GROWTH = "hp growth";
	public const string DB_KEYWORD_AGILITY_GROWTH = "agility growth";
	public const string DB_KEYWORD_GAMENESS_GROWTH = "gameness growth";
	public const string DB_KEYWORD_AGGRESSION_GROWTH = "aggression growth";
	// end breed document keywords

	// breed document VERSION 1 keywords
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
	// end breed document VERSION 1 keywords

	// chicken document keywords
	public const string DB_KEYWORD_OWNER = "owner";
	public const string DB_KEYWORD_GENDER = "gender";
	public const string DB_KEYWORD_NOTES = "notes";
	public const string DB_KEYWORD_LIFE_STAGE = "life stage";

	public const string DB_KEYWORD_GENERATION_ID = "generation id";
	public const string DB_KEYWORD_GENERATION_COUNT = "generation count";

	public const string DB_KEYWORD_ATTACK = "attack";
	public const string DB_KEYWORD_DEFENSE = "defense";
	public const string DB_KEYWORD_HP = "hp";
	public const string DB_KEYWORD_AGILITY = "agility";
	public const string DB_KEYWORD_GAMENESS = "gameness";
	public const string DB_KEYWORD_AGGRESSION = "aggression";
	public const string DB_KEYWORD_CONDITIONING = "conditioning";

	public const string DB_KEYWORD_ATTACK_MAX = "attack max";
	public const string DB_KEYWORD_DEFENSE_MAX = "defense max";
	public const string DB_KEYWORD_HP_MAX = "hp max";
	public const string DB_KEYWORD_AGILITY_MAX = "agility max";
	public const string DB_KEYWORD_GAMENESS_MAX = "gameness max";
	public const string DB_KEYWORD_AGGRESSION_MAX = "aggression max";
	public const string DB_KEYWORD_CONDITIONING_MAX = "conditioning max";

	public const string DB_KEYWORD_IS_QUEUED_FOR_MATCH = "is queued for match";
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

	// item document keywords
	public const string DB_KEYWORD_AVAILABLE_AT_STORE = "store";
	public const string DB_KEYWORD_DURATION_DAYS = "days";
	public const string DB_KEYWORD_DURATION_HOURS = "hours";
	public const string DB_KEYWORD_DURATION_MINUTES = "minutes";
	public const string DB_KEYWORD_DURATION_SECONDS = "seconds";
	// end item document keywords

	// feeds schedule document keywords
	public const string DB_KEYWORD_END_TIME = "end time";
	public const string DB_KEYWORD_IS_COMPLETED = "is completed";
	// end feeds schedule document keywords

	// replay document keywords
	public const string DB_KEYWORD_CHICKEN_ID_1 = "chicken id 1";
	public const string DB_KEYWORD_CHICKEN_ID_2 = "chicken id 2";
	public const string DB_KEYWORD_REPLAY = "replay";
	// end replay document keywords

	// item owned document keywords
	public const string DB_KEYWORD_PLAYER_ID = "player id";
	public const string DB_KEYWORD_ITEM_ID = "item id";
	public const string DB_KEYWORD_QUANTITY = "quantity";
	// end item owned document keywords

	// matchmaking category document keywords
	public const string DB_KEYWORD_IS_PVP = "is pvp";
	public const string DB_KEYWORD_CUSTOM_MATCHES_ALLOWED = "custom matches allowed";
	// end matchmaking category document keywords

	// match document keywords
	public const string DB_KEYWORD_PLAYER_ID_1 = "player id 1";
	public const string DB_KEYWORD_PLAYER_ID_2 = "player id 2";
	public const string DB_KEYWORD_CATEGORY_ID = "category id";
	public const string DB_KEYWORD_LLAMADO = "llamado";
	public const string DB_KEYWORD_DEHADO = "dehado";
	public const string DB_KEYWORD_BETTING_OPTION = "betting option";
	public const string DB_KEYWORD_STATUS = "status";
	public const string DB_KEYWORD_INTERVAL = "interval";
	public const string DB_KEYWORD_INTERVALS_LEFT = "intervals left";
	public const string DB_KEYWORD_INTERVAL_TIME = "interval time";
	// end match document keywords

	// betting options
	public const string BETTING_OPTION_NONE = "No Betting";
	public const string BETTING_OPTION_SINGLE_BETTING = "Single Betting";
	public const string BETTING_OPTION_SPECTATOR_BETTING = "Spectator Betting";
	public const float MINIMUM_BET_RATIO = 0.5f;
	public const float MAXIMUM_BET_RATIO = 1.5f;
	// end betting options

	// match status
	public const string MATCH_STATUS_WAITING_FOR_OPPONENT = "waiting for opponent";
	public const string MATCH_STATUS_OPPONENT_FOUND = "opponent found";
	public const string MATCH_STATUS_CANCELED = "canceled";
	public const string MATCH_STATUS_FINISHED = "finished";
	public const string MATCH_STATUS_BETTING_PERIOD = "betting period";
	// end match status

	// betting odds document keywords
	public const string DB_KEYWORD_LLAMADO_ODDS = "llamado odds";
	public const string DB_KEYWORD_DEHADO_ODDS = "dehado odds";
	public const string DB_KEYWORD_ORDER = "order";
	// end betting odds document keywords

	// bet document keywords
	public const string DB_KEYWORD_MATCH_ID = "match id";
	public const string DB_KEYWORD_BETTING_ODDS_ID = "betting odds id";
	public const string DB_KEYWORD_BETTED_CHICKEN_ID = "betted chicken id";
	public const string DB_KEYWORD_BETTED_CHICKEN_STATUS = "betted chicken status";
	public const string DB_KEYWORD_BET_AMOUNT = "bet amount";

	public const string BETTED_CHICKEN_STATUS_LLAMADO = "llamado";
	public const string BETTED_CHICKEN_STATUS_DEHADO = "dehado";
	// end bet document keywords

	// mail document keywords
	public const string DB_KEYWORD_TITLE = "title";
	public const string DB_KEYWORD_FROM = "from";
	public const string DB_KEYWORD_TO = "to";
	public const string DB_KEYWORD_MESSAGE = "message";
	public const string DB_KEYWORD_MAIL_TYPE = "mail type";
	public const string DB_KEYWORD_REPLAY_ID = "replay id";
	// end mail document keywords

	// mail status
	public const string MAIL_STATUS_UNREAD = "unread";
	public const string MAIL_STATUS_READ = "read";
	public const string MAIL_STATUS_DELETED = "deleted";
	// end mail status

	// mail types
	public const string MAIL_TYPE_NOTIFICATION = "notification";
	public const string MAIL_TYPE_MESSAGE = "message";
	public const string MAIL_TYPE_REPLAY = "replay";
	// end mail types

	public const int BETTING_ODDS_COUNT = 7;

	public const string GENDER_MALE = "male";
	public const string GENDER_FEMALE = "female";

	public const string LIFE_STAGE_EGG = "egg";
	public const string LIFE_STAGE_CHICK = "chick";
	public const string LIFE_STAGE_STAG = "stag";
	public const string LIFE_STAGE_COCK = "cock";
	public const string LIFE_STAGE_HEN = "hen";

	// message panel children
	public const string MESSAGE_PANEL_FINAL_TITLE = "Title/Text";
	public const string MESSAGE_PANEL_TITLE = "Title/Text";
	public const string MESSAGE_PANEL_MESSAGE = "Main Panel/Message Panel/Text";
	public const string MESSAGE_PANEL_OK_BUTTON = "Main Panel/Button Panel/OK Button";
	public const string MESSAGE_PANEL_CANCEL_BUTTON = "Main Panel/Button Panel/Cancel Button";
	// end message panel children

	// schedule panel children
	public const string SCHEDULE_PANEL_ICON = "Inventory Icon";
	public const string SCHEDULE_PANEL_ICON_COUNT = "Inventory Icon/Panel/Inventory Count";
	public const string SCHEDULE_PANEL_NAME = "Info/Name/Name Text";
	public const string SCHEDULE_PANEL_STATS = "Info/Stats/Stats Text";
	public const string SCHEDULE_PANEL_TIMER = "Timer/Timer/Timer Text";
	public const string SCHEDULE_PANEL_HURRY_BUTTON = "Timer/Buttons/Hurry";
	public const string SCHEDULE_PANEL_CANCEL_BUTTON = "Timer/Buttons/Cancel";
	// end schedule panel children

	// match panel children
	public const string MATCH_PANEL_IDLE_1 = "Idle 1";
	public const string MATCH_PANEL_IDLE_2 = "Idle 2";
	public const string MATCH_PANEL_INFO_1 = "Info 1";
	public const string MATCH_PANEL_INFO_2 = "Info 2";
	public const string MATCH_PANEL_WLD_1 = "WLD 1";
	public const string MATCH_PANEL_WLD_2 = "WLD 2";

	public const string MATCH_PANEL_CHICKEN_1 = "Info 1/Chicken Name/Chicken Name Text";
	public const string MATCH_PANEL_CHICKEN_2 = "Info 2/Chicken Name/Chicken Name Text";
	public const string MATCH_PANEL_FARM_1 = "Info 1/Farm Name/Farm Name Text";
	public const string MATCH_PANEL_FARM_2 = "Info 2/Farm Name/Farm Name Text";
	public const string MATCH_PANEL_WIN_1 = "WLD 1/Win/Win";
	public const string MATCH_PANEL_WIN_2 = "WLD 2/Win/Win";
	public const string MATCH_PANEL_LOSE_1 = "WLD 1/Lose/Lose";
	public const string MATCH_PANEL_LOSE_2 = "WLD 2/Lose/Lose";
	public const string MATCH_PANEL_DRAW_1 = "WLD 1/Draw/Draw";
	public const string MATCH_PANEL_DRAW_2 = "WLD 2/Draw/Draw";
	public const string MATCH_PANEL_ODDS_1 = "WLD 1/Odds/Odds";
	public const string MATCH_PANEL_ODDS_2 = "WLD 2/Odds/Odds";

	public const string MATCH_PANEL_TIMER = "Timer/Timer/Timer Text";
	public const string MATCH_PANEL_VIEW_BUTTON = "Timer/Buttons/View";
	public const string MATCH_PANEL_CANCEL_BUTTON = "Timer/Buttons/Cancel";
	// end match panel children

	// create match children
	public const string CREATE_MATCH_STAT_ATK_SLIDER = "ATK/Slider";
	public const string CREATE_MATCH_STAT_DEF_SLIDER = "DEF/Slider";
	public const string CREATE_MATCH_STAT_HP_SLIDER = "HP/Slider";
	public const string CREATE_MATCH_STAT_AGI_SLIDER = "AGI/Slider";
	public const string CREATE_MATCH_STAT_GAM_SLIDER = "GAM/Slider";
	public const string CREATE_MATCH_STAT_AGG_SLIDER = "AGG/Slider";
	public const string CREATE_MATCH_STAT_CON_SLIDER = "CON/Slider";

	public const string CREATE_MATCH_BETTING_BUTTON = "Betting Button";
	public const string CREATE_MATCH_BETTING_PANEL = "Betting Panel";
	public const string CREATE_MATCH_BETTING_AMOUNT_SLIDER = "Betting Panel/Amount/Slider";
	public const string CREATE_MATCH_BETTING_2_BUTTON = "Betting 2 Button";
	public const string CREATE_MATCH_BETTING_2_PANEL = "Betting 2 Panel";
	public const string CREATE_MATCH_BETTING_2_AMOUNT_SLIDER = "Betting 2 Panel/Amount/Slider";
	public const string CREATE_MATCH_BETTING_2_WAIT_DURATION_PANEL = "Betting 2 Panel/Wait Duration";
	public const string CREATE_MATCH_BETTING_2_WAIT_DURATION_SLIDER = "Betting 2 Panel/Wait Duration/Slider";
	public const string CREATE_MATCH_PRIVATE_BUTTON = "Private Button";
	public const string CREATE_MATCH_PRIVATE_PANEL = "Private Panel";
	public const string CREATE_MATCH_PRIVATE_PASSWORD = "Private Panel/Password/InputField";
	// end create match children

	// view match children
	public const string VIEW_MATCH_CHICKEN_VIEW_NAME = "Text/Chicken Name";
	public const string VIEW_MATCH_CHICKEN_VIEW_LEFT_ARROW = "Text/Left Button";
	public const string VIEW_MATCH_CHICKEN_VIEW_RIGHT_ARROW = "Text/Right Button";
	public const string VIEW_MATCH_CHICKEN_VIEW_IMAGE = "Text/Chicken View";

	public const string VIEW_MATCH_STAT_VIEW_NAME = "Text/Text";
	public const string VIEW_MATCH_STAT_VIEW_LEFT_ARROW = "Text/Left Button";
	public const string VIEW_MATCH_STAT_VIEW_RIGHT_ARROW = "Text/Right Button";

	public const string VIEW_MATCH_STAT_PANEL_1 = "Stats Panel 1";
	public const string VIEW_MATCH_STAT_PANEL_2 = "Stats Panel 2";
	public const string VIEW_MATCH_STAT_PANEL_1_NAME = "Cock Stats";
	public const string VIEW_MATCH_STAT_PANEL_2_NAME = "Farm Info";

	public const string VIEW_MATCH_STAT_ATK_SLIDER = "Stats Panel 1/ATK/Slider";
	public const string VIEW_MATCH_STAT_DEF_SLIDER = "Stats Panel 1/DEF/Slider";
	public const string VIEW_MATCH_STAT_HP_SLIDER = "Stats Panel 1/HP/Slider";
	public const string VIEW_MATCH_STAT_AGI_SLIDER = "Stats Panel 1/AGI/Slider";
	public const string VIEW_MATCH_STAT_GAM_SLIDER = "Stats Panel 1/GAM/Slider";
	public const string VIEW_MATCH_STAT_AGG_SLIDER = "Stats Panel 1/AGG/Slider";
	public const string VIEW_MATCH_STAT_CON_SLIDER = "Stats Panel 1/CON/Slider";

	public const string VIEW_MATCH_STAT_FARM_NAME = "Stats Panel 2/Main Panel/Farm Name/Text 2";
	public const string VIEW_MATCH_STAT_OWNER = "Stats Panel 2/Main Panel/Owner/Text 2";
	public const string VIEW_MATCH_STAT_RECORD = "Stats Panel 2/Main Panel/Record/Text 2";

	public const string VIEW_MATCH_INFO_BETTING_STYLE = "Panel/Main Panel/Betting Style/Text 2";
	public const string VIEW_MATCH_INFO_FAVORITE = "Panel/Main Panel/Favorite/Text 2";
	public const string VIEW_MATCH_INFO_ODDS = "Panel/Main Panel/Odds/Text 2";
	// end view match children

	// fight ring ui
	public const string FIGHT_RING_UI_HP_1_SLIDER = "HP Panel/HP 1";
	public const string FIGHT_RING_UI_HP_2_SLIDER = "HP Panel/HP 2";
	public const string FIGHT_RING_UI_HP_1_FILL_SLIDER = "HP Panel/HP 1/Fill Area/Fill";
	public const string FIGHT_RING_UI_HP_2_FILL_SLIDER = "HP Panel/HP 2/Fill Area/Fill";
	public const string FIGHT_RING_UI_CHICKEN_1 = "Name Panel/Image/Name 1";
	public const string FIGHT_RING_UI_CHICKEN_2 = "Name Panel/Image/Name 2";
	public const string FIGHT_RING_UI_FARM_1 = "Farm Panel/Name 1";
	public const string FIGHT_RING_UI_FARM_2 = "Farm Panel/Name 2";
	// end fight ring ui

	// match bet children	
	public const string BET_MATCH_BET_SLIDER = "Bet Panel/Bet/Slider";
	public const string BET_MATCH_MESSAGE = "Message Panel/Message";
	// match bet children

	// mail panel children
	public const string MAIL_PANEL_LIST_TOGGLE = "Toggle/Toggle";
	public const string MAIL_PANEL_LIST_TITLE = "Content/Title/Text/Text";
	public const string MAIL_PANEL_LIST_MESSAGE = "Content/Message/Text/Text";

	public const string MAIL_PANEL_MAIL_TITLE = "Title Panel/Title";
	public const string MAIL_PANEL_MAIL_FROM = "Title Panel/From";
	public const string MAIL_PANEL_MAIL_TO = "Title Panel/To";
	public const string MAIL_PANEL_MAIL_MESSAGE = "Message Panel/Scroll Panel/Panel/Text";
	// end mail panel children

	// store details panel children
	public const string STORE_DETAILS_PANEL_ICON = "Store Icon";
	public const string STORE_DETAILS_PANEL_ICON_IMAGE = "Store Icon";
	public const string STORE_DETAILS_PANEL_NAME = "Name";
	public const string STORE_DETAILS_PANEL_CATEGORY = "Category";
	public const string STORE_DETAILS_PANEL_PRICE = "Price";
	public const string STORE_DETAILS_PANEL_COIN_PRICE = "Price/Coin Price";
	public const string STORE_DETAILS_PANEL_CASH_PRICE = "Price/Cash Price";
	public const string STORE_DETAILS_PANEL_DESCRIPTION = "Description";
	// end store details panel children

	// store quantity panel children
	public const string STORE_QUANTITY_PANEL = "Main Panel - Store";
	public const string STORE_QUANTITY_PANEL_PLAYER_COIN = "Panel/Price/Coin Price";
	public const string STORE_QUANTITY_PANEL_PLAYER_CASH = "Panel/Price/Cash Price";
	public const string STORE_QUANTITY_PANEL_ICON = "Details Panel/Panel 1/Store Icon";
	public const string STORE_QUANTITY_PANEL_NAME = "Details Panel/Panel 2/Name";
	public const string STORE_QUANTITY_PANEL_CATEGORY = "Details Panel/Panel 2/Category";
	public const string STORE_QUANTITY_PANEL_COIN_PRICE = "Details Panel/Panel 3/Price/Coin Price";
	public const string STORE_QUANTITY_PANEL_COIN_IMAGE = "Details Panel/Panel 3/Price/Coin Image";
	public const string STORE_QUANTITY_PANEL_CASH_PRICE = "Details Panel/Panel 3/Price/Cash Price";
	public const string STORE_QUANTITY_PANEL_CASH_IMAGE = "Details Panel/Panel 3/Price/Cash Image";
	public const string STORE_QUANTITY_PANEL_DESCRIPTION = "Details Panel/Panel 3/Description";
	public const string STORE_QUANTITY_PANEL_SLIDER = "Slider/Slider";
	public const string STORE_QUANTITY_PANEL_AMOUNT = "Slider/Amount";
	// end store quantity panel children

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

	// replay document keywords
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
	// end replay document keywords
}
