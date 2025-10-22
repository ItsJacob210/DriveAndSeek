using UnityEngine;

public class CopCatchOnContact : MonoBehaviour
{
	[Tooltip("The robber/player car GameObject. Catch triggers when our non-trigger collider touches theirs.")]
	public GameObject targetCar;
	[Tooltip("Require this relative speed to count as a catch. Set 0 for any touch.")]
	public float minRelativeSpeed = 0f;

	private Rigidbody2D selfRb;
	private Rigidbody2D targetRb;

	private void Awake()
	{
		selfRb = GetComponent<Rigidbody2D>();
		if (targetCar != null) targetRb = targetCar.GetComponent<Rigidbody2D>();
	}

	public void SetTarget(GameObject target)
	{
		targetCar = target;
		targetRb = target != null ? target.GetComponent<Rigidbody2D>() : null;
	}

    private void OnCollisionEnter2D(Collision2D collision) { TryCatch(collision); }
    private void OnCollisionStay2D(Collision2D collision) { TryCatch(collision); }

    private void TryCatch(Collision2D collision)
    {
        if (targetRb == null) return;
        if (collision == null) return;
        if (collision.rigidbody != targetRb) return;

        if (minRelativeSpeed > 0f)
        {
            if (selfRb != null && targetRb != null)
            {
                float rel = (selfRb.linearVelocity - targetRb.linearVelocity).magnitude;
                if (rel < minRelativeSpeed) return;
            }
        }

        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.CopWins();
    }
}


