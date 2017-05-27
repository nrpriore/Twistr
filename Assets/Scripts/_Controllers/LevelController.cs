using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Object that controls the level variables and functions.
/// Essentially this is the script that runs when a player clicks the "Start" button.
/// In short it spawns walls, sets timers, sets the game type, etc.
/// </summary>
public class LevelController:MonoBehaviour {

    // Declare constants
    private const float GAME_SPEED 	    = 0.3f; 	// Speed of game items coming at you
    public const float 	WALL_TIME 		= 2.8f;		// Time between each wall
    private const float SPIN_SPEED 		= 15f;		// Speed the player spins
    private const float GAME_MULT_STRT 	= 1f;		// Starting speed mult for game
    private const float MAX_MULT 		= 1.5f;		// Max game mult
    private const float TIME_MAX_MULT 	= 180f;		// Seconds until max mult

    public const float 	TOKEN_START_Z 	= 100f;		// Starting Z value of tokens
    public const float 	PLR_START_Y	    = -0.6f;	// Y offset of everything
    private const float PLR_START_Z 	= 2f;		// Starting Z of player

    // Declare private vars
    private bool        _playing;   	// Are walls coming at you?
    private bool 		_crashed;		// Did the player crash?
    private float 		_gameMult;		// Game speed multiplier
    private float       _levelTimer;    // Time since level started
    private int         _wallID;        // ID of wall to spawn
    private Wall       	_currWall;		// Last wall that's been spawned
    private Player		_currPlayer;    // The player
    private GameObject	_nonRot;		// Empty GameObject for pivot point to keep some objects from rotating

    // Declare stats for this level
    private int        	_wallsCleared;  // The number of walls the player dodged
    private int 		_coinsCollected;// The number of coins the player collected

    // Declare controllers for reference
    private PowerupController   _pc;
//  private SpawnController     _sc;

    // Declare UI interactables
    private Button      _left;
    private Button      _right;
    private InGamePowerup[]     _selectedPowerups;



#region privatevars
    // Returns the initial speed of the game
    public float GameSpeed {
        get {return GAME_SPEED;}
    }
    // Returns the game speed multiplier
    public float GameMult {
    	get {return _gameMult;}
    	set {_gameMult = value;}
    }
    // Returns the modified game speed
    public float ModGameSpeed {
    	get {return GameSpeed * GameMult;}
    }
    // Returns the modified frame time for timers
    public float ModDeltaTime {
    	get {return Time.deltaTime * GameMult;}
    }
    // Returns if the level is currently playing
    public bool Playing {
        get {return _playing;}
        set {_playing = value;}
    }
    public float LevelTimer {
    	get {return _levelTimer;}
    	set {_levelTimer = value;}
    }
    public bool Crashed {
    	get {return _crashed;}
    	set {_crashed = value;}
    }
    // Returns the current wallID to be spawned
    public int WallID {
        get {return _wallID;}
        set {_wallID = value;}
    }
    // Returns the last wall that was spawned
    public Wall CurrWall {
        get {return _currWall;}
        set {_currWall = value;}
    }
    // Returns the player
    public Player CurrPlayer {
    	get {return _currPlayer;}
    	set {_currPlayer = value;}
    }
    // Returns the current rotation of the camera
    public Quaternion CurrentRot {
    	get {return Camera.main.transform.rotation;}
    	set {Camera.main.transform.rotation = value;}
    }
    // Returns the rotation of the player SideIndex
    // (aka the rotation the camera needs to get to)
    public float PlayerIndexRot {
    	get {return (CurrPlayer != null && CurrWall != null)? CurrPlayer.SideIndex * 360 / CurrWall.NumSides : 0;}
    }
    // Returns the walls cleared in this level
    public int WallsCleared {
    	get {return _wallsCleared;}
    	set {_wallsCleared = value;}
    }
    // Returns the coins collected in this level
    public int CoinsCollected {
    	get {return _coinsCollected;}
    	set {_coinsCollected = value;}
    }
    // Returns the Powerup Controller
    public PowerupController PC {
        get {return _pc;}
    }
    // Returns the selected powerups to spawn
    public InGamePowerup[] SelectedPowerups {
        get {return _selectedPowerups;}
        set {_selectedPowerups = value;}
    }
#endregion



    // Initialize variables and spawn first wall
    // wallID is the type of wall
    // 1 - Square
    // 2 - ...
    public void InitLevel(int wallID, string[] selectedPowerups) {
    	// Initialize level variables
    	InitVars(wallID);

        // Initialize stats to keep
        InitStats();

        // Initialize UI components
        // Level tokens (player, etc)
        // Various controllers
        InitUI(selectedPowerups);

        // Begin the level
        Playing = true;
    }

    // Initialize level variables
    private void InitVars(int wallID) {
    	_wallID 		= wallID;			// Sets wallID to spawn
    	 _currWall 		= null;				// Sets the current wall (none since first)
        _gameMult 		= GAME_MULT_STRT;	// Sets Initial GameMultiplier
        Crashed 		= false;			// Set crashed to non-null;
        LevelTimer		= 0f;				// Init LevelTimer

        _left = GameObject.Find("Left").GetComponent<Button>();
        _right = GameObject.Find("Right").GetComponent<Button>();
        _left.onClick.AddListener(() => {MoveLeft();});
        _right.onClick.AddListener(() => {MoveRight();});
    }

    // Initialize stats to keep
    private void InitStats() {
    	WallsCleared 	= 0;
        CoinsCollected = 0;
    }

    // Initialize all aspects of the UI
    private void InitUI(string[] selectedPowerups) {
        // Initialize level tokens (player, spawncontroller)
        InitTokens();
        // Initialize InGamePowerups
        SelectedPowerups = new InGamePowerup[selectedPowerups.Length];
        for(int x = 0; x < selectedPowerups.Length; x++) {
            float xwidth = 1.3f;
            float xpos = -xwidth + (float)(x + 1) * 2f * xwidth / (float)(selectedPowerups.Length + 1);
            InGamePowerup igpu = (Instantiate(Resources.Load("Prefabs/UI/Powerup"), new Vector3(xpos,-0.9f,2f), Quaternion.identity) as GameObject).GetComponent<InGamePowerup>();
            SelectedPowerups[x] = igpu;
            igpu.AssignPowerup(selectedPowerups[x], new int[] {1,1});
            igpu.gameObject.transform.parent = _nonRot.transform;
        }
    }

    // Initialize level tokens (player, spawncontroller)
    private void InitTokens() {
    	// Instantiate Player and empty GameObject to control pivot point around origin
        // When rotating, we rotate the pivot point object "PlayerRot", not the Player
        _nonRot = new GameObject("NonRot");
        // Instantiate Player GameObject at start position with 0 rotation
        // Set CurrPlayer to Player script of the Player GameObject
        CurrPlayer = (Instantiate(Resources.Load("Prefabs/Player"), new Vector3(0,PLR_START_Y,PLR_START_Z), Quaternion.identity) as GameObject).GetComponent<Player>();
        // Set Player parent to "PlayerRot" for Unity hierarchy
        CurrPlayer.gameObject.transform.parent = _nonRot.transform;

        // Add controllers
        gameObject.AddComponent<SpawnController>();
        _pc = gameObject.AddComponent<PowerupController>();
    }

    // Runs every frame
    void Update() {
        // Run level functions if Playing
        if(Playing) {
        // Timing Mechanics //
        	// Adjust LevelTimer
            LevelTimer += ModDeltaTime;
        // Restart Mechanics //
        	if(Crashed) {
            	EndLevel();
            	return;
	        }
	    // Rotation Mechanics //
	        // Check if user clicks left or right to rotate game
	        if(Input.GetKeyDown("right")) {
	        	// Move player right
	        	MoveRight();
	        }
	        if(Input.GetKeyDown("left")) {
	        	// Move player left
	        	MoveLeft();
	        }
	        // Lerp current camera rotation to desired rotation (index of player)
	        CurrentRot = Quaternion.Lerp(
	        	CurrentRot,
	        	Quaternion.Euler(new Vector3(0,0,PlayerIndexRot)),
	        	SPIN_SPEED * Time.deltaTime
	        );
	        // Equally lerp gameObject holding non-rotational objects to be in sync with camera
	        _nonRot.transform.rotation = Quaternion.Lerp(
	        	_nonRot.transform.rotation,
	        	Quaternion.Euler(new Vector3(0,0,PlayerIndexRot)),
	        	SPIN_SPEED * Time.deltaTime
	        );

	    // Game Modifiers //
	    	// Increase game speed multiplier over time
	    	// Increases linearly from GAME_MULT_START to MAX_MULT over duration of TIME_MAX_MULT
	    	// During dev, if holding space, increase mult
	        float currMult = GAME_MULT_STRT + LevelTimer * (MAX_MULT - GAME_MULT_STRT) / TIME_MAX_MULT;
            currMult *= (_pc.TimeWarpActive)? _pc.TimeWarpSlow : 1;
	        GameMult = (currMult < MAX_MULT)? currMult : MAX_MULT;
	    }

	// Dev - If crashed, press space to reload scene
	    if(Crashed && Input.GetKeyDown("space")) {
	    	UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
	    }
    }

    // Move player left
    private void MoveLeft() {
        if(!Crashed) {
            // Adjust the index of the Player
            // If full loop, reset to max index (NumSides-1)
            CurrPlayer.SideIndex -= (CurrPlayer.SideIndex == 0)? -(CurrWall.NumSides-1) : 1;
        }else{
            // Dev - If crashed, press to reload scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
    // Move player right
    private void MoveRight() {
        // Adjust the index of the Player
        // If full loop, reset to 0
        CurrPlayer.SideIndex += (CurrPlayer.SideIndex == CurrWall.NumSides-1)? -(CurrWall.NumSides-1) : 1;
    }

    // Game-restart mechanics
    private void EndLevel() {
    	GameMult = 0f;
    	Playing = false;
    }

}