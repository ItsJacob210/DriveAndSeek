using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
	//start button to disable after first click (optional but recommended)
	[SerializeField] private Button startButton;
	//optional: groups to hide when countdown starts (e.g., controls panels and the start button root)
	[SerializeField] private GameObject[] hideOnStart;
	//countdown ui: you can reuse the title text instead of a separate panel
	[SerializeField] private GameObject countdownRoot; //optional panel; if null we keep the existing layout
	[SerializeField] private TextMeshProUGUI countdownText; //big center text (can be your title \"drive & seek\")
	[SerializeField] private string driveText = "Drive";
	private bool isStarting;

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
		//if no separate countdown root, reuse current layout and only drive the assigned text
		if (countdownRoot != null) countdownRoot.SetActive(false);
		//fallback: if no explicit countdownText is assigned, try to find a TMP on this object
		if (countdownText == null) countdownText = GetComponentInChildren<TextMeshProUGUI>(true);
	}

	public void StartGame()
	{
		//prevent multiple presses
		if (isStarting) return;
		isStarting = true;
		if (startButton != null) startButton.interactable = false;
		//block further input on the menu while countdown happens
		if (overlay != null) overlay.interactable = false;
		//hide configured groups (title stays visible to show numbers if countdownText uses it)
		if (hideOnStart != null)
		{
			for (int i = 0; i < hideOnStart.Length; i++)
			{
				if (hideOnStart[i] != null) hideOnStart[i].SetActive(false);
			}
		}

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
		//show countdown overlay
		if (countdownRoot != null) countdownRoot.SetActive(true);
		for (int i = seconds; i > 0; i--)
		{
			if (countdownBeepClip != null) audioSource.PlayOneShot(countdownBeepClip);
			if (countdownText != null) countdownText.text = i.ToString();
			yield return new WaitForSecondsRealtime(1f);
		}
		//display final drive text briefly
		if (countdownText != null) countdownText.text = driveText;
		yield return new WaitForSecondsRealtime(0.35f);
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
		//hide countdown overlay
		if (countdownRoot != null) countdownRoot.SetActive(false);
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


