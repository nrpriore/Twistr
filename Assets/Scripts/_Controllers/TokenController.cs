using UnityEngine;
using System;

/// <summary>
/// Base class that controls Token variables and methods
/// GameObjects such as walls/powerups/coins should inherit this
/// </summary>
public class TokenController:MonoBehaviour {

    // Declare public vars
    public  GameController  _gc;                // Game Controller script

    // Declare private vars
    private int         _sideIndex;             // Index the token is on
    private bool        _checkedForCollision;   // Have we checked the token-player collision?
    private bool        _crashOnCollision;      // Does this token crash the player?



#region privatevars
    // Returns the index the token is on
    public int SideIndex {
        get {return _sideIndex;}
        set {_sideIndex = value;}
    }
    // Returns if this token crashes the player
    public bool CrashOnCollision {
        get {return _crashOnCollision;}
        set {_crashOnCollision = value;}
    }
    // Returns the Powerup controller
    public PowerupController PC {
        get {return _gc.LC.PC;}
    }
#endregion



    // Initialize vars
    public virtual void Awake() {
        _gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        name = name.Substring(0, name.Length - 7);
        _checkedForCollision = false;
    }

    public void SetToken(int index) {
        SideIndex = index;
    }

    // Runs every frame
    void Update() {
        // Move token toward player by ModGameSpeed
		gameObject.transform.Translate(Vector3.back * _gc.LC.ModGameSpeed * SpeedAlgorithm());
        // Check collision if token z = player z and we haven't checked collision yet
        // Have to extrapolate where token will be later to make it not go through the coin
        // Hence the 5 * _gc...
		if(gameObject.transform.position.z - 5 * _gc.LC.ModGameSpeed * SpeedAlgorithm() <= _gc.LC.CurrPlayer.gameObject.transform.position.z && !_checkedForCollision) {
			_checkedForCollision = true;
            // If collision, call Collide method
			if(SideIndex == _gc.LC.CurrPlayer.SideIndex) {
                Collide();
                return;
			}
            // Coin Magnet powerup!
            // Don't need else-if because previous if 'returns' when true
            if(name == "Coin" && _gc.gameObject.GetComponent<PowerupController>().CoinMagnetActive) {
                if(Array.IndexOf(_gc.gameObject.GetComponent<PowerupController>().CoinMagnetArray, SideIndex) != -1) {
                    Collide();
                    return;
                }
            }
		}
        // Destroy token if behind camera
		if(gameObject.transform.position.z < -Camera.main.transform.position.z) {
			Destroy(gameObject);
            return;
		}
    }

    // Calculate speed algorithm multiplier
    private float SpeedAlgorithm() {
        return Mathf.Log10(gameObject.transform.position.z + 3f);
    }

    // Base method for each child Collide method
    public virtual void Collide() {
        // If shield is active and token is of type that crashes player, destroy token and decrement/remove shield
        if(_crashOnCollision) {
            if(_gc.LC.PC.ShieldActive) {
                Destroy(gameObject);
                _gc.LC.PC.ShieldCurrCharges--;
                if(_gc.LC.PC.ShieldCurrCharges == 0) {
                    _gc.LC.PC.EndShield();
                }
                return;
            }
            // If shield didn't stop collision, set crashed variable to stop level
            _gc.LC.Crashed = true;
        }else{
            Destroy(gameObject);
        }
    }

}


