using UnityEngine;

public class BoostPickup2D : MonoBehaviour
{
	public float boostAmount = 25f;
	public bool destroyOnPickup = true;
	public string targetTag = "Player"; // optional: restrict by tag; leave empty to accept any MovePlayer

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other == null) return;
		if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
		{
			// continue if parent/root has the tag
			var root = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
			if (!root.CompareTag(targetTag)) return;
		}

		var move = other.attachedRigidbody != null
			? other.attachedRigidbody.GetComponent<MovePlayer>()
			: other.GetComponentInParent<MovePlayer>();
		if (move == null) return;

		move.AddBoost(boostAmount);
		if (destroyOnPickup) Destroy(gameObject);
	}
}


