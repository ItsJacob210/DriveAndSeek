using UnityEngine;

public class CopCatchZone : MonoBehaviour
{
	[Tooltip("Assign the collider of the cop that should trigger a catch when touching the target car.")]
	public Collider2D copCollider;
	[Tooltip("The car GameObject that the cop is trying to catch.")]
	public GameObject targetCar;

	[Header("Catch Settings")]
	public float minRelativeSpeed = 0.5f; // optional: require some motion

	private void Reset()
	{
		// Try to pick our own collider by default
		copCollider = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (targetCar == null) return;
		if (other == null) return;
		if (other.gameObject != targetCar) return;

		// Optional relative speed check
		var rbCop = copCollider != null ? copCollider.attachedRigidbody : null;
		var rbTarget = other.attachedRigidbody;
		if (rbCop != null && rbTarget != null)
		{
			float rel = (rbCop.linearVelocity - rbTarget.linearVelocity).magnitude;
			if (rel < minRelativeSpeed) return;
		}

		// Signal win
		var gm = FindFirstObjectByType<GameManager>();
		if (gm != null)
		{
			gm.CopWins();
		}
	}
}


