using UnityEngine;
//controls the start overlay visibility and start action

public class StartOverlay : MonoBehaviour
{
	//canvasgroup on the overlay canvas (required to fade/hide and block input)
	[SerializeField] private CanvasGroup overlay;
	//pause the game while overlay is visible
	[SerializeField] private bool pauseAtStart = false;

	//audio
	[SerializeField] private AudioClip startLoopClip; //ambient/menu loop while overlay shown
	[SerializeField] private AudioClip buttonClickClip; //click sfx when pressing start
	[SerializeField] private AudioClip countdownBeepClip; //played each second of countdown
	[SerializeField] private bool useCountdown = true;
	[SerializeField] private int countdownSeconds = 3;
	[SerializeField] private float startMusicVolume = 0.6f;
	private AudioSource audioSource;
	[SerializeField] private ChaseAudioManager chaseAudioManager; //optional: start gameplay loops on finish

	private void Awake()
	{
		if (overlay == null) overlay = GetComponent<CanvasGroup>();
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.spatialBlend = 0f;
		if (pauseAtStart) Time.timeScale = 0f;
		Show(true);
		//play start loop if provided
		if (startLoopClip != null)
		{
			audioSource.loop = true;
			audioSource.clip = startLoopClip;
			audioSource.volume = startMusicVolume;
			audioSource.Play();
		}
	}

	public void StartGame()
	{
		if (buttonClickClip != null) audioSource.PlayOneShot(buttonClickClip);
		if (useCountdown && countdownSeconds > 0)
		{
			StartCoroutine(BeginCountdownThenStart());
		}
		else
		{
			FinishStart();
		}
	}

	private System.Collections.IEnumerator BeginCountdownThenStart()
	{
		int seconds = Mathf.Max(1, countdownSeconds);
		for (int i = seconds; i > 0; i--)
		{
			if (countdownBeepClip != null) audioSource.PlayOneShot(countdownBeepClip);
			yield return new WaitForSecondsRealtime(1f);
		}
		FinishStart();
	}

	private void FinishStart()
	{
		//stop start loop
		if (audioSource != null && audioSource.loop)
		{
			audioSource.loop = false;
			audioSource.Stop();
		}
		//kick off gameplay audio if assigned
		if (chaseAudioManager != null) chaseAudioManager.BeginGameplay();
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


