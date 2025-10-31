using UnityEngine;
//controls the start overlay visibility and start action

public class StartOverlay : MonoBehaviour
{
	//canvasgroup on the overlay canvas (required to fade/hide and block input)
	[SerializeField] private CanvasGroup overlay;
	//pause the game while overlay is visible
	[SerializeField] private bool pauseAtStart = false;

	private void Awake()
	{
		if (overlay == null) overlay = GetComponent<CanvasGroup>();
		if (pauseAtStart) Time.timeScale = 0f;
		Show(true);
	}

	public void StartGame()
	{
		Show(false);
		if (pauseAtStart) Time.timeScale = 1f;
	}

	private void Show(bool show)
	{
		if (overlay == null) return;
		overlay.alpha = show ? 1f : 0f;
		overlay.interactable = show;
		overlay.blocksRaycasts = show;
	}
}


