using UnityEngine;

public class InGamePowerup : MonoBehaviour {

	private PowerupController 	_pc;

	private int 		_powerupID;
	private int[] 		_level;

	private bool		_active;


#region privatevars
    // Returns the _active flag
    public bool Active {
        get {return _active;}
        set {_active = value;}
    }
#endregion

	void Start() {
		_active = false;
	}

	void OnMouseDown() {
		if(Active){
			StartPowerup();
		}
	}

	public void AssignPowerup(string powerup, int[] level) {
		name = "Activate" + powerup;
		switch(powerup) {
			case "TimeWarp":
            	_powerupID = 0;
                break;
            case "CoinMagnet":
            	_powerupID = 1;
                break;
            case "Shield":
            	_powerupID = 2;
                break;
            case "Powerup4":
            	// _powerupID = 3;
                break;
            case "Powerup5":
            	// _powerupID = 4;
                break;
		}
		_level = level;
	}

	private void StartPowerup() {
		_pc = GameObject.Find("Game Controller").GetComponent<PowerupController>();
		switch(_powerupID) {
            case 0:
            	_pc.InitTimeWarp(_level[0]);
                break;
            case 1:
            	_pc.InitCoinMagnet(_level[0],_level[1]);
                break;
            case 2:
            	_pc.InitShield();
                break;
            case 3:
            	// _pc.InitPowerup4(1);
                break;
            case 4:
            	// _pc.InitPowerup5(1);
                break;
        }
        Active = false;
	}
}
