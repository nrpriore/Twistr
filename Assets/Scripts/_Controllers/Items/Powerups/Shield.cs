using UnityEngine;

/// <summary>
/// Contols the Shield gameobject
/// </summary>
public class Shield : TokenController {

	// What happens when the player collects
	public override void Collide() {
		// Activate powerup
		GameObject igpu = GameObject.Find("ActivateShield");
		if(igpu != null) {
			igpu.GetComponent<InGamePowerup>().Active = true;
		}
		// Run base method
		base.Collide();
	}
}
