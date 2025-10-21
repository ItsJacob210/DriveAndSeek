using UnityEngine;

public class CarCrashPhysics2D : MonoBehaviour
{
	[Header("Crash/Collision Settings")]
	public float minImpactSpeedForStun = 4f;
	public float stunDurationSeconds = 0.6f;
	public float bounceMultiplier = 0.9f; // scales physics impulse response
	public float angularImpulseScale = 0.5f; // torque from side hits

	[Header("Damage (optional)")]
	public float impactDamageScale = 1f; // expose if you later add health

	private Rigidbody2D rb2d;
	private MovePlayer move;
	private float stunUntilTime;

	private void Awake()
	{
		rb2d = GetComponent<Rigidbody2D>();
		move = GetComponent<MovePlayer>();
	}

	private void Update()
	{
		if (move != null)
		{
			bool isStunned = Time.time < stunUntilTime;
			move.SetExternalStun(isStunned);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (rb2d == null || collision == null) return;

		// Calculate relative speed at impact
		float relativeSpeed = collision.relativeVelocity.magnitude;
		if (relativeSpeed < 0.01f) return;

		// Apply a bounce impulse manually to exaggerate feel if needed
		Vector2 averageNormal = Vector2.zero;
		for (int i = 0; i < collision.contactCount; i++)
		{
			averageNormal += collision.GetContact(i).normal;
		}
		if (collision.contactCount > 0)
		{
			averageNormal /= collision.contactCount;
			Vector2 reflect = Vector2.Reflect(rb2d.linearVelocity, averageNormal);
			rb2d.linearVelocity = Vector2.Lerp(rb2d.linearVelocity, reflect, Mathf.Clamp01(bounceMultiplier));
			// Add angular spin from side impacts
			rb2d.angularVelocity += relativeSpeed * angularImpulseScale * Mathf.Sign(Vector2.SignedAngle(averageNormal, rb2d.linearVelocity));
		}

		// Stun control so player can't instantly counter-steer the crash
		if (relativeSpeed >= minImpactSpeedForStun)
		{
			stunUntilTime = Time.time + stunDurationSeconds;
		}

		// Hook for damage systems later using impactDamageScale
	}
}


