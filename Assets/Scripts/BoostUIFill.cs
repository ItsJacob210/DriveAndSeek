using UnityEngine;
using UnityEngine.UI;

public class BoostUIFill : MonoBehaviour
{
	[SerializeField] private MovePlayer movePlayer;
	[SerializeField] private Image fillImage; // UI Image with Fill Method = Horizontal or Radial

	private void Reset()
	{
		if (fillImage == null) fillImage = GetComponent<Image>();
		if (movePlayer == null) movePlayer = GetComponentInParent<MovePlayer>();
	}

	private void Awake()
	{
		if (fillImage == null) fillImage = GetComponent<Image>();
	}

	private void Update()
	{
		if (movePlayer == null || fillImage == null) return;
		fillImage.fillAmount = movePlayer.BoostFraction;
	}
}
