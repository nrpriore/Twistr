using UnityEngine;

/// <summary>
/// Contols properties of the Heptagon wall token
/// </summary>
public class WallHept:Wall {

    // Handles heptagon wall creation
    public override void CreateWall(Wall prevWall) {
    	base.CreateWall(prevWall);

        // Set variables
        NumSides = 7;
        HoleChance = new float[] { .7f,1f }; // End with "1" (100%)

        // Create Wall tokens based on vars
        Token = Resources.Load("Prefabs/Walls/Heptagon");
        Token.name = "Wall";
        InstantiateWall();
    }

}
