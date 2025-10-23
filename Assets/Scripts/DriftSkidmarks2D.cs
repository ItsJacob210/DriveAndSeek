using UnityEngine;

public class DriftSkidmarks2D : MonoBehaviour
{
	[SerializeField] private MovePlayer movePlayer;
	[SerializeField] private TrailRenderer leftTrail;   // rear-left trail
	[SerializeField] private TrailRenderer rightTrail;  // rear-right trail

	[SerializeField] private float minSpeedForMarks = 2f;
	[SerializeField] private float minEmitSeconds = 0.05f; // prevent flicker when tapping

	private float lastEmitOnTime;

	private void Reset()
	{
		movePlayer = GetComponent<MovePlayer>();
	}

	private void Awake()
	{
		if (movePlayer == null) movePlayer = GetComponent<MovePlayer>();
	}

	private void Update()
	{
		if (movePlayer == null || leftTrail == null || rightTrail == null) return;

		bool up = Input.GetKey(movePlayer.upKey);
		bool down = Input.GetKey(movePlayer.downKey);
		bool left = Input.GetKey(movePlayer.leftKey);
		bool right = Input.GetKey(movePlayer.rightKey);
		bool drift = Input.GetKey(movePlayer.driftKey);

		bool side = left ^ right;         // exactly one of left/right
		bool vert = up || down;           // any of up/down
		bool driftCombo = drift && side && vert;

		float speed = (movePlayer.rb2d != null) ? movePlayer.rb2d.linearVelocity.magnitude : 0f;
		bool fastEnough = speed >= minSpeedForMarks;

		bool shouldEmit = driftCombo && fastEnough;
		if (shouldEmit) lastEmitOnTime = Time.time;

		bool emitNow = shouldEmit || (Time.time - lastEmitOnTime) < minEmitSeconds;
		if (leftTrail.emitting != emitNow) leftTrail.emitting = emitNow;
		if (rightTrail.emitting != emitNow) rightTrail.emitting = emitNow;
	}
}


