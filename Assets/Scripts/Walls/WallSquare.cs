using UnityEngine;

/// <summary>
/// Contols properties of the Square wall token
/// </summary>
public class WallSquare:Wall {

    // Handles square wall creation
    public override void CreateWall(Wall prevWall) {
    	base.CreateWall(prevWall);

        // Set variables
        NumSides = 4;
        HoleChance = new float[] { .7f,1f }; // End with "1" (100%)

        // Create Wall tokens based on vars
        Token = Resources.Load("Prefabs/Walls/Square");
        Token.name = "Wall";
        InstantiateWall();
    }

}
