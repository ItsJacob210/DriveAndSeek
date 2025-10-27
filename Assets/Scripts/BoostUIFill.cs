using UnityEngine;
using UnityEngine.UI;
//updates a ui image fillAmount to reflect the player's boost
public class BoostUIFill : MonoBehaviour
{
	[SerializeField] private MovePlayer movePlayer;
	[SerializeField] private Image fillImage; //ui image with type=filled

	private void Reset()
	{
		//autowire references when added
		if (fillImage == null) fillImage = GetComponent<Image>();
		if (movePlayer == null) movePlayer = GetComponentInParent<MovePlayer>();
	}

	private void Awake()
	{
		//ensure we have an image reference
		if (fillImage == null) fillImage = GetComponent<Image>();
	}

	private void Update()
	{
		//drive fill by boost fraction (0...1)
		if (movePlayer == null || fillImage == null) return;
		fillImage.fillAmount = movePlayer.BoostFraction;
	}
}
