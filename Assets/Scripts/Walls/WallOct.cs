using UnityEngine;

/// <summary>
/// Contols properties of the Octagon wall token
/// </summary>
public class WallOct:Wall {

    // Handles octagon wall creation
    public override void CreateWall(Wall prevWall) {
    	base.CreateWall(prevWall);

        // Set variables
        NumSides = 8;
        HoleChance = new float[] { .7f,1f }; // End with "1" (100%)

        // Create Wall tokens based on vars
        Token = Resources.Load("Prefabs/Walls/Octagon");
        Token.name = "Wall";
        InstantiateWall();
    }

}
