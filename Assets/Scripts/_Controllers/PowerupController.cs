using UnityEngine;

public class PowerupController : MonoBehaviour {

	// ----------
	private float 		_timeWarpSlow;
	private float 		_timeWarpDuration;
	// ----------
	private int 		_coinMagnetPower;
	private int[]		_coinMagnetArray;
	private float 		_coinMagnetDuration;
	// ----------
	private int 		_shieldStartCharges;
	private int 		_shieldCurrCharges;

	// ----------
	private float 		_timeWarpTimer;
	private float 		_coinMagnetTimer;
	// ----------
	private bool		_timeWarpActive;
	private bool 		_coinMagnetActive;
	private bool 		_shieldActive;
	// ----------



#region privatevars
    // Returns TimeWarp vars
    public bool TimeWarpActive {
        get {return _timeWarpActive;}
    }
    public float TimeWarpSlow {
    	get {return _timeWarpSlow;}
    }
    // Returns CoinMagnet vars
    public bool CoinMagnetActive {
    	get {return _coinMagnetActive;}
    }
    public int[] CoinMagnetArray {
    	get {
    		_coinMagnetArray = new int[2 * _coinMagnetPower];
			int index; int temp;
			LevelController _lc = gameObject.GetComponent<LevelController>();
			for(int x = -_coinMagnetPower, y = 0; x < _coinMagnetPower; x++, y++) {
				index = (x < 0)? x : x + 1;
				temp = (_lc.CurrPlayer.SideIndex + index + _lc.CurrWall.NumSides) % _lc.CurrWall.NumSides;
				_coinMagnetArray[y] = temp;
			}
			return _coinMagnetArray;}
    }
    public bool ShieldActive {
    	get {return _shieldActive;}
    	set {_shieldActive = value;}
    }
    public int ShieldCurrCharges {
    	get {return _shieldCurrCharges;}
    	set {_shieldCurrCharges = value;}
    }
#endregion



	void Start() {
		// ----------
		_timeWarpSlow			= 0.1f;
		_timeWarpDuration		= 2f;
		// ----------
		_coinMagnetPower 		= 1;
		_coinMagnetDuration 	= 10f;
		// ----------
		_shieldStartCharges 	= 1;
	}

	// Update is called once per frame
	// Don't multiply timers by GameMult as these timers should be independent of game speed
	void Update() {
		if(_timeWarpActive) {
			_timeWarpTimer -= Time.deltaTime;
			if(_timeWarpTimer <= 0f) {
				EndTimeWarp();
			}
		}
		if(_coinMagnetActive) {
			_coinMagnetTimer -= Time.deltaTime;
			if(_coinMagnetTimer <= 0f) {
				EndCoinMagnet();
			}
		}
	}

	public void InitCoinMagnet(int durLev, int powLev) {
		_coinMagnetPower = powLev;
		_coinMagnetTimer = _coinMagnetDuration + (float)(durLev-1) * 1f;
		_coinMagnetActive = true;
	}

	private void EndCoinMagnet() {
		_coinMagnetTimer = 0f;
		_coinMagnetActive = false;
	}

	public void InitTimeWarp(int durLev) {
		_timeWarpTimer = _timeWarpDuration + (float)(durLev-1) * 1f;
		_timeWarpActive = true;
	}
	private void EndTimeWarp() {
		_timeWarpTimer = 0f;
		_timeWarpActive = false;
	}
	public void InitShield() {
		_shieldCurrCharges = _shieldStartCharges;
		_shieldActive = true;
	}
	public void EndShield() {
		_shieldActive = false;
	}

	/*public static string PickPowerup(){
		string powerup = "TimeWarp";
		return "Prefabs/Powerups/" + powerup;
	}*/
}
