/// <summary>
/// Contols the functions of mines
/// </summary>
public class Mine : TokenController {

	public override void Awake() {
		base.Awake();
		CrashOnCollision = true;
	}

	// What happens when the player hits a mine
	public override void Collide() {
		// Run base method
		base.Collide();
	}
}
