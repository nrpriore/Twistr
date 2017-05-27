using UnityEngine;

/// <summary>
/// Contols properties of the Hexagon wall token
/// </summary>
public class WallHex:Wall {

    // Handles hexagon wall creation
    public override void CreateWall(Wall prevWall) {
    	base.CreateWall(prevWall);

        // Set variables
        NumSides = 6;
        HoleChance = new float[] { .7f,1f }; // End with "1" (100%)

        // Create Wall tokens based on vars
        Token = Resources.Load("Prefabs/Walls/Hexagon");
        Token.name = "Wall";
        InstantiateWall();
    }

}
