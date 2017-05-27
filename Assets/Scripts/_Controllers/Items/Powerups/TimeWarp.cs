using UnityEngine;

/// <summary>
/// Contols the TimeWarp gameobject
/// </summary>
public class TimeWarp : TokenController {

	// What happens when the player collects
	public override void Collide() {
		// Activate powerup
		GameObject igpu = GameObject.Find("ActivateTimeWarp");
		if(igpu != null) {
			igpu.GetComponent<InGamePowerup>().Active = true;
		}
		// Run base method
		base.Collide();
	}
}
