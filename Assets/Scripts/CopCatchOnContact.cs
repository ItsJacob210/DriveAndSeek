using UnityEngine;
//detects physical collision between cop and robber rigidbodies to trigger a win

public class CopCatchOnContact : MonoBehaviour
{
	public GameObject targetCar;
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

    //check both enter and stay so a catch also triggers when colliders remain in contact
    private void OnCollisionEnter2D(Collision2D collision) { TryCatch(collision); }
    private void OnCollisionStay2D(Collision2D collision) { TryCatch(collision); }

    private void TryCatch(Collision2D collision)
    {
        if (targetRb == null) return;
        if (collision == null) return;
        //only accept collisions with the robber's specific rigidbody
        if (collision.rigidbody != targetRb) return;

        if (minRelativeSpeed > 0f)
        {
            if (selfRb != null && targetRb != null)
            {
                float rel = (selfRb.linearVelocity - targetRb.linearVelocity).magnitude;
                if (rel < minRelativeSpeed) return;
            }
        }

        //signal win via game manager
        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.CopWins();
    }
}


