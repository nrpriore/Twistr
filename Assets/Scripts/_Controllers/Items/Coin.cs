/// <summary>
/// Contols the functions of coins
/// </summary>
public class Coin : TokenController {

	// What happens when the player collects a coin
	public override void Collide() {
		// Add to coins collected
		_gc.LC.CoinsCollected += (_gc.DoubleCoins)? 2 : 1;
		// Run base method
		base.Collide();
	}
}
