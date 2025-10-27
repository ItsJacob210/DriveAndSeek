using UnityEngine;
//manages looping chase music and proximity based siren loudness between cop and robber (robber sided mech)

public class ChaseAudioManager : MonoBehaviour
{
	[Header("Targets")]
	public Transform cop;
	public Transform robber;

	[Header("Clips")]
	public AudioClip chaseMusicLoop; //2 min
	public AudioClip sirenLoop; //1 sec

	[Header("Siren Distance")]
	public float sirenStartDistance = 40f;  //siren fades in when closer than this
	public float sirenMaxDistance = 2.5f;   //within this, siren at max volume

	[Header("Volumes")]
	[Range(0f, 1f)] public float musicVolume = 0.6f;
	[Range(0f, 1f)] public float sirenMaxVolume = 0.9f;

	[Header("Smoothing")]
	public float volumeLerpSpeed = 8f;

	//separate audio sources for music and siren
	private AudioSource musicSource;
	private AudioSource sirenSource;

	private void Awake()
	{
		//create two 2d audio sources
		musicSource = gameObject.AddComponent<AudioSource>();
		sirenSource = gameObject.AddComponent<AudioSource>();

		musicSource.loop = true;
		sirenSource.loop = true;
        sirenSource.spatialBlend = 0f; //2d mix so it plays uniformly across split-screen
		musicSource.spatialBlend = 0f;
	}

	private void Start()
	{
		//kick off music loop immediately
		if (chaseMusicLoop != null)
		{
			musicSource.clip = chaseMusicLoop;
			musicSource.volume = musicVolume;
			musicSource.Play();
		}
		//start siren loop muted; volume is driven by distance
		if (sirenLoop != null)
		{
			sirenSource.clip = sirenLoop;
			sirenSource.volume = 0f;
			sirenSource.Play();
		}
	}

	private void Update()
	{
		if (cop == null || robber == null || sirenSource == null) return;

		float d = Vector3.Distance(cop.position, robber.position);
		float t = 0f;
		if (d <= sirenStartDistance)
		{
			// Map distance to 0..1 where 0 = far (no siren), 1 = close (max siren)
			float range = Mathf.Max(0.0001f, sirenStartDistance - sirenMaxDistance);
			t = Mathf.Clamp01((sirenStartDistance - d) / range);
		}
		float targetVol = t * sirenMaxVolume;
		sirenSource.volume = Mathf.Lerp(sirenSource.volume, targetVol, volumeLerpSpeed * Time.deltaTime);

		if (musicSource != null)
		{
            musicSource.volume = musicVolume; //keep stable unless you want ducking
		}
	}
}


