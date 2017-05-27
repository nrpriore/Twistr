using UnityEngine;

/// <summary>
/// Object that controls game variables and functions.
/// Linked to GameController object in Unity, and runs when the program starts.
/// Inherits MonoBehavior because it is referenced from a unity game object
/// </summary>
public class GameController:MonoBehaviour {

    // Declare private vars
    private LevelController     _lc;    // Controls level variables and functions


    // Dev - set WallID in Unity Inspector
    public int WallID;
    private bool _doubleCoins;

#region privatevars
    // Returns the level controller script
    public LevelController LC {
        get {return _lc;}
        set {_lc = value;}
    }
    public bool DoubleCoins {
        get {return _doubleCoins;}
        set {_doubleCoins = value;}
    }
#endregion



    // Runs on game start
    void Start() {
        // Set initial variables
        InitiateVars();

        // For Dev - start level with WallID (type of wall)
        string[] selectedPowerups = {
            //"TimeWarp",
            //"CoinMagnet",
            "Shield"
        };
        LC.InitLevel(WallID, selectedPowerups);
    }

    // Sets initial variables
    private void InitiateVars() {
        DoubleCoins = true;

        // Add a LevelController to the scene
        LC = gameObject.AddComponent<LevelController>();
    }

}
