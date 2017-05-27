using UnityEngine;

/// <summary>
/// Object that controls Player variables and methods
/// </summary>
public class Player : MonoBehaviour {

	// Declare private vars
	private int 		_sideIndex;		// Index the player is on



#region privatevars
	// Returns the index the player is on
    public int SideIndex {
        get {return _sideIndex;}
        set {_sideIndex = value;}
    }
#endregion



    // Initialize vars
	void Start () {
		SideIndex = 0;
	}

	// Runs every frame
	void Update () {

	}
}
