using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Base class that controls creation of walls
/// Specific types of walls should inherit this
/// </summary>
public class Wall {

    // Declare private vars
    private int     	_numSides;   	// Number of sides
    private float[] 	_holeChance;    // Chance for each hole in wall
    private int[]   	_holeIndex;     // Index of each hole
    private UnityEngine.Object  _token; // The Prefab to spawn
    private Wall 		_prevWall;		// The previous wall
    private float       _tokenZ;        // Starting Z value of tokens



    #region privatevars
    // Returns the number of sides of the wall
    public int NumSides {
        get {return _numSides;}
        set {_numSides = value;}
    }
    // Returns the chance for number of holes
    public float[] HoleChance {
        get {return _holeChance;}
        set {_holeChance = value;}
    }
    // Returns the index for the holes
    public int[] HoleIndex {
        get {return _holeIndex;}
        set {_holeIndex = value;}
    }
    // Returns the prefab to be created
    public UnityEngine.Object Token {
    	get {return _token;}
    	set {_token = value;}
    }
    // Returns the previous wall
    public Wall PrevWall {
    	get {return _prevWall;}
    }
    #endregion

    // Overriden function in each specific wall script
    public virtual void CreateWall(Wall prevWall) {
        _tokenZ = LevelController.TOKEN_START_Z;
    }

    // Determine how many holes and which indexes for the newly-generated wall
    // The input is the previous wall's holes
    public int[] GetHoleIndex(int[] holeIndex) {
        // Init vars
        int[] holes = null;
        float chance = UnityEngine.Random.value;
        int cnt;
        int ind;

        // Determine how many holes based on HoleChance int[]
        // The length of hole chance is the maximum number of holes
        // Each subsequent index is the percentage chance for there being another hole
        for(cnt = 0;cnt < HoleChance.Length;cnt++) {
            if(chance <= HoleChance[cnt]) {
                holes = new int[cnt + 1];
                break;
            }
        }

        // Determine index of holes.
        for(ind = 0;ind < holes.Length;ind++) {
            // Add 1 because Array.Contains doesn't work with 0
            // Will return to 0 index below
            int tempind = NextIndex() + 1;
            if(Error(tempind)) {
                return null;
            }
            // Ensure the same hole isn't chosen twice
            while(holes.Contains(tempind)) {
                tempind = NextIndex();
                if(Error(tempind)) {
                    return null;
                }
            }
            holes[ind] = tempind;
        }
        // Bring holes array back to 0 index
        for(ind = 0;ind < holes.Length;ind++) {
            holes[ind] -= 1;
        }

        // Sort and return final array.
        Array.Sort(holes);
        return holes;
    }

    // Gets index of next hole.
    private int NextIndex() {
        // Currently there is an equal chance for any index of the wall to be a hole.
        // Return cnt + 1 because Array.Contains doesn't work with 0
        float indhole = UnityEngine.Random.value;
        for(int cnt = 0;cnt < NumSides;cnt++) {
            if(indhole <= (float)(cnt+1) / NumSides) {
                return cnt;
            }
        }
        // If an int hasn't been returned yet, check NumSides of the Wall. Return -1 for error handling.
        return -1;
    }

    // Create the game objects
    // token is the type of wall prefab
    // hole is the array of hole indexes
    public void InstantiateWall(){
    	// If first wall, pass in null
    	HoleIndex = (PrevWall != null)? GetHoleIndex(PrevWall.HoleIndex) : GetHoleIndex(null);
        for(int cnt = 0; cnt < NumSides; cnt++) {
        	if(!HoleIndex.Contains(cnt)) {
        		// Instantiate token and set tc to TokenController script to set vars
	        	TokenController tc = (MonoBehaviour.Instantiate(
	        		Token,
	        		new Vector3(0,0,_tokenZ),
	        		Quaternion.Euler(0,0, (float)(cnt) * 360f / (float)NumSides)
	        	) as GameObject).GetComponent<TokenController>();
	        	// Set token vars
	        	tc.SideIndex = cnt;		// Set index of token
	        }
        }
    }

    // Eror handling. In this case, errors in 'int' returning functions return -1. This centralizes that check.
    private bool Error(int er) {
        return er == -1;
    }

}
