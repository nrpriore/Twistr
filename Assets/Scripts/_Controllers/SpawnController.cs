using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Controls all item spawning
/// Determines which, when, and where items spawn
/// </summary>
public class SpawnController : MonoBehaviour {

	// Declare constants
	private const float OFFSET_START	= 0.7f;
	private const float OFFSET_END		= 1f;
	private const int 	NUM_ITEMS		= 10;

	// Declare private vars
	private LevelController	_lc;		// Level Controller Script
    private float       _wallTimer;		// Time since last wall spawned
	private float 		_itemTimer;
	private float 		_itemTime;
	private float 		_wallTime;

	private bool 		_chunkStarted;
	private int 		_itemIndex;
	private float       _tokenZ;        // Starting Z value of tokens
	// Potentially not needed
//	private float 		_tokenY;		// Starting Y value of tokens

	private GameObject 	_items;			// Parent GameObject that holds all items
	private int 		_wallID;		// WallID to spawn

	private static int[] _openIndicies; // Valid locations to spawn a coin, powerup, etc.
	private bool        _noPowerupThisChunk; // True if there was a powerup spawned this chunk
	private bool        _earnedPowerupThisChunk; // True if player got every coin in previous chunk
	private int         _coinsEarnedBeforeChunk; // Number of coins the player collected this chunk

#region privatevars
    // Returns the index the token is on
    public GameObject Items {
        get {return _items;}
        set {_items = value;}
	}
    public static int Num_Items{
    	get {return NUM_ITEMS;}

    }
#endregion



	// Runs when added to the scene
	void Start () {
		// Initialize items GameObject
		Items = new GameObject("Items");

		// Link Level Controller script
		_lc 			= gameObject.GetComponent<LevelController>();
		_wallTime 		= LevelController.WALL_TIME;
		_itemTime 		= (_wallTime - (OFFSET_START + OFFSET_END)) / (float)NUM_ITEMS;
		_tokenZ 		= LevelController.TOKEN_START_Z;

    	_wallTimer 		= 0f;				// Init WallTimer
    	_wallID 		= _lc.WallID;
    	_openIndicies   =  new int[_lc.WallID + 3];
    	for(int i = 0; i<_lc.WallID + 3; i++){
    		_openIndicies[i] = i;
    	}
    	_noPowerupThisChunk = true;
    	_earnedPowerupThisChunk = false;
    	_coinsEarnedBeforeChunk = 0;
	}

	// Runs every frame
	void Update () {
		if(_lc.Playing) {
			// Timing Mechanics //
        	// Adjust WallTimer
            _wallTimer -= _lc.ModDeltaTime;
            // Spawn new wall when WallTimer hits 0
            if(_wallTimer <= 0f){
	        	SpawnNextWall(_wallID);
	        	_wallTimer = _wallTime;
	        }
			// If the chunk has started:
			if(_chunkStarted) {
				// Reduce item timer
				_itemTimer -= _lc.ModDeltaTime;
				// (Potentially) spawn item when 0
				if(_itemTimer <= 0f) {
					// If indexes left, spawn item
					if(_itemIndex > 0) {
						// Spawn Item
						ArrayList openIndicies = new ArrayList(_openIndicies);
						int coinIndex = CoinSpawner.CoinIndex(openIndicies);
						if(_itemIndex == 1 && _earnedPowerupThisChunk){
							SpawnItem(coinIndex, PickPowerup());
						}
						else{
							SpawnItem(coinIndex, "Prefabs/Coins/Coin");
						}
						openIndicies.Remove(coinIndex);

						if(UnityEngine.Random.value < 0.05 && _noPowerupThisChunk){
							SpawnItem(Random.PickIndex(openIndicies), PickPowerup());
							_noPowerupThisChunk = false;
						}
						// Decrease item index
						_itemIndex--;
						_itemTimer = _itemTime;
					}
					// If no indexes left, end chunk
					else {
						_chunkStarted = false;
						_earnedPowerupThisChunk =
							_lc.CoinsCollected - _coinsEarnedBeforeChunk == CoinSpawner.CoinsThisChunk;
						_coinsEarnedBeforeChunk = _lc.CoinsCollected;
					}
				}
			}
			// If it hasn't started, check if we should start it
			else if(!_chunkStarted && WithinChunk()) {
				_chunkStarted = true;
				_itemTimer = _itemTime;
				_itemIndex = NUM_ITEMS;
				_noPowerupThisChunk = true;
			}
		}
	}

	// Governs what to spawn
	private void SpawnItem(int index, String prefabName) {
		TokenController tc = (Instantiate(
			Resources.Load(prefabName),
			new Vector3(xStart(index),yStart(index),_tokenZ),
			Quaternion.Euler(0,0, (float)index * 360f / (float)_lc.CurrWall.NumSides)
		) as GameObject).GetComponent<TokenController>();
		tc.SideIndex = index;
	}

	private string PickPowerup() {
		InGamePowerup igpu = _lc.SelectedPowerups[Mathf.FloorToInt((float)_lc.SelectedPowerups.Length * 0.99f * UnityEngine.Random.value)];
		return "Prefabs/Powerups/" + igpu.name.Substring(8, igpu.name.Length - 8);
	}

	// Check if we should start chunk
	private bool WithinChunk() {
		return _wallTimer < _wallTime - OFFSET_START && _wallTimer > OFFSET_END;
	}

	private void SpawnNextWall(int wallID) {
        Wall newWall = null;
        // Determine which wall to spawn
        switch(wallID) {
            case 1:
                newWall = new WallSquare();
                break;
            case 2:
                newWall = new WallPent();
                break;
            case 3:
                newWall = new WallHex();
                break;
            case 4:
                newWall = new WallHept();
                break;
            case 5:
                newWall = new WallOct();
                break;
        }
        // Create the wall
        if(newWall != null) {
            newWall.CreateWall(_lc.CurrWall);
        }
        // Set last spawned wall
        _lc.CurrWall = newWall;
    }

    private float xStart(int index){
    	return (float)Math.Sin(((float)index/(float)_lc.CurrWall.NumSides)*2*Math.PI);
    }
    private float yStart(int index){
    	return -(float)Math.Cos(((float)index/(float)_lc.CurrWall.NumSides)*2*Math.PI);
    }
}
