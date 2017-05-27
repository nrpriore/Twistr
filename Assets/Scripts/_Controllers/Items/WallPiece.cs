/// <summary>
/// Contols the functions of wall tokens
/// </summary>
public class WallPiece : TokenController {

	public override void Awake() {
		base.Awake();
		CrashOnCollision = true;
	}

	// What happens when the player hits a wall
	public override void Collide() {
		// Run base method
		base.Collide();
	}
}
