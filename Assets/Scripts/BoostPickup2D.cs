using UnityEngine;
//handles boost pickups that grant boost to cars on trigger contact w/ prefab asset
public class BoostPickup2D : MonoBehaviour
{
    public float boostAmount = 25f;
    public bool destroyOnPickup = true;
    public string targetTag = "Player";

	private void OnTriggerEnter2D(Collider2D other)
	{
        if (other == null) return;
        //filter by tag if configured; also check rigidbody root
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
		{
            //continue if parent/root has the tag
			var root = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
			if (!root.CompareTag(targetTag)) return;
		}

        //locate moveplayer on the contacting rigidbody or parent
		var move = other.attachedRigidbody != null
			? other.attachedRigidbody.GetComponent<MovePlayer>()
			: other.GetComponentInParent<MovePlayer>();
		if (move == null) return;

        //apply boost and optionally remove pickup
		move.AddBoost(boostAmount);
		if (destroyOnPickup) Destroy(gameObject);
	}
}


