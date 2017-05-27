using UnityEngine;

/// <summary>
/// Contols properties of the Pentagon wall token
/// </summary>
public class WallPent:Wall {

    // Handles pentagon wall creation
    public override void CreateWall(Wall prevWall) {
    	base.CreateWall(prevWall);

        // Set variables
        NumSides = 5;
        HoleChance = new float[] { 0.2f,0.7f,1f }; // End with "1" (100%)

        // Create Wall tokens based on vars
        Token = Resources.Load("Prefabs/Walls/Pentagon");
        Token.name = "Wall";
        InstantiateWall();
    }

}
