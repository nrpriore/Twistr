using UnityEngine;
using System.Collections;

/// <summary>
/// Handles all decisions the game must make based on
/// random behavior.
/// </summary>
public static class Random{
	public static int PickIndex(ArrayList openIndicies){
		return (int)openIndicies[Mathf.FloorToInt((float)openIndicies.Count * 0.99f * UnityEngine.Random.value)];
	}
}