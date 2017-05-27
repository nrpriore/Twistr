using UnityEngine;

/// <summary>
/// Contols the CoinMagnet gameobject
/// </summary>
public class CoinMagnet : TokenController {

	// What happens when the player collects
	public override void Collide() {
		// Activate powerup
		GameObject igpu = GameObject.Find("ActivateCoinMagnet");
		if(igpu != null) {
			igpu.GetComponent<InGamePowerup>().Active = true;
		}
		// Run base method
		base.Collide();
	}
}
