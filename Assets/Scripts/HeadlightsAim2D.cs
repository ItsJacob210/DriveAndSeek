using UnityEngine;
//orients and positions headlight transforms to match moveplayer facing, with per-orientation offsets

public class HeadlightsAim2D : MonoBehaviour
{
	[SerializeField] private MovePlayer movePlayer;
	[SerializeField] private Transform[] headlightRoots; //assign the two headlight cone objects
	[SerializeField] private float rotationLerp = 18f;
	[SerializeField] private float extraBiasDeg = 0f; //optional tiny offset
	[SerializeField] private float fallbackAngleDeg = -90f; //default forward if no input (up = -90)

	[Header("Positioning")]
	[SerializeField] private Transform carRoot; //optional; defaults to moveplayer.transform
	[SerializeField] private bool updatePosition = true;
	[SerializeField] private float forwardOffset = 0.5f; //base offset if per-orientation is off
	[SerializeField] private float lateralSeparation = 0.3f; //half-distance between left/right lights
	[SerializeField] private float zOffset = 0f; //keep lights on same z plane if needed

	[Header("Per-Orientation Offsets (optional, overrides base forwardOffset)")]
	[SerializeField] private bool usePerOrientationOffsets = true;
	[SerializeField] private float offsetUp = 1.3f;
	[SerializeField] private float offsetDown = 0.95f;
	[SerializeField] private float offsetSide = 1.6f; //a or d only
	[SerializeField] private float offsetUpDiagonal = 1.55f; //w+a or w+d
	[SerializeField] private float offsetDownDiagonal = 1.1f; //a+s or s+d
	[SerializeField] private float extraForwardDriftUp = 0f; //fine-tune drift up
	[SerializeField] private float extraForwardDriftDown = 0f; //fine-tune drift down

	private float currentAngle;
	private float lastEffectiveForwardOffset;

	private void Reset()
	{
		movePlayer = GetComponentInParent<MovePlayer>() ?? GetComponent<MovePlayer>();
	}

	private void Awake()
	{
		if (movePlayer == null)
		{
			movePlayer = GetComponentInParent<MovePlayer>() ?? GetComponent<MovePlayer>();
		}
	}

	private void LateUpdate()
	{
		if (movePlayer == null || headlightRoots == null || headlightRoots.Length == 0) return;

		Vector2 f = movePlayer.CurrentFacingDir;
		float targetAngle = fallbackAngleDeg;
		if (f.sqrMagnitude > 0.0001f)
		{
			targetAngle = Mathf.Atan2(f.y, f.x) * Mathf.Rad2Deg - 90f + extraBiasDeg;
		}

		currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationLerp * Time.deltaTime);
		Quaternion rot = Quaternion.Euler(0f, 0f, currentAngle);
		for (int i = 0; i < headlightRoots.Length; i++)
		{
			if (headlightRoots[i] == null) continue;
				headlightRoots[i].rotation = rot; //world-space rotation to follow facing even if parent doesn't rotate
		}

		if (updatePosition)
		{
			Transform root = carRoot != null ? carRoot : (movePlayer != null ? movePlayer.transform : null);
			if (root != null)
			{
				Vector3 carPos = root.position;
				Vector2 forward = f.sqrMagnitude > 0.0001f ? f.normalized : Vector2.up;
				Vector2 right = new Vector2(forward.y, -forward.x);
				bool hasInput = Input.GetKey(movePlayer.upKey) || Input.GetKey(movePlayer.downKey) || Input.GetKey(movePlayer.leftKey) || Input.GetKey(movePlayer.rightKey);
				float effForward = ComputeEffectiveForwardOffset(f, hasInput);
				Vector3 baseFront = carPos + (Vector3)(forward * effForward);
				float z = carPos.z + zOffset;

				if (headlightRoots.Length == 1)
				{
					headlightRoots[0].position = new Vector3(baseFront.x, baseFront.y, z);
				}
				else
				{
					Vector3 leftPos = baseFront + (Vector3)(-right * lateralSeparation);
					Vector3 rightPos = baseFront + (Vector3)(right * lateralSeparation);
					//convention: index 0 = left, index 1 = right
					if (headlightRoots[0] != null) headlightRoots[0].position = new Vector3(leftPos.x, leftPos.y, z);
					if (headlightRoots[1] != null) headlightRoots[1].position = new Vector3(rightPos.x, rightPos.y, z);
				}
			}
		}
	}

	private float ComputeEffectiveForwardOffset(Vector2 facing, bool hasInput)
	{
		if (!usePerOrientationOffsets || movePlayer == null)
		{
			lastEffectiveForwardOffset = forwardOffset;
			return lastEffectiveForwardOffset;
		}

		if (hasInput)
		{
			bool up = Input.GetKey(movePlayer.upKey);
			bool down = Input.GetKey(movePlayer.downKey);
			bool left = Input.GetKey(movePlayer.leftKey);
			bool right = Input.GetKey(movePlayer.rightKey);
			bool drift = Input.GetKey(movePlayer.driftKey);

			bool side = left ^ right; // exactly one
			bool vert = up ^ down;    // exactly one

			if (vert && side)
			{
				lastEffectiveForwardOffset = up ? (offsetUpDiagonal + (drift ? extraForwardDriftUp : 0f))
					: (offsetDownDiagonal + (drift ? extraForwardDriftDown : 0f));
				return lastEffectiveForwardOffset;
			}
			if (vert)
			{
				lastEffectiveForwardOffset = up ? (offsetUp + (drift ? extraForwardDriftUp : 0f))
					: (offsetDown + (drift ? extraForwardDriftDown : 0f));
				return lastEffectiveForwardOffset;
			}
			if (side)
			{
				lastEffectiveForwardOffset = offsetSide;
				return lastEffectiveForwardOffset;
			}
		}

		// No input: infer from facing vector; persist result so it stays stable while idle
		const float eps = 0.001f;
		if (facing.sqrMagnitude < eps * eps)
		{
			return lastEffectiveForwardOffset > 0f ? lastEffectiveForwardOffset : forwardOffset;
		}
		Vector2 n = facing.normalized;
		float ax = Mathf.Abs(n.x);
		float ay = Mathf.Abs(n.y);
		if (ax < 0.2f && ay >= 0.2f)
		{
			lastEffectiveForwardOffset = n.y > 0 ? offsetUp : offsetDown;
			return lastEffectiveForwardOffset;
		}
		if (ay < 0.2f && ax >= 0.2f)
		{
			lastEffectiveForwardOffset = offsetSide;
			return lastEffectiveForwardOffset;
		}
		// Diagonals: choose by vertical sign
		lastEffectiveForwardOffset = n.y >= 0 ? offsetUpDiagonal : offsetDownDiagonal;
		return lastEffectiveForwardOffset;
	}
}


