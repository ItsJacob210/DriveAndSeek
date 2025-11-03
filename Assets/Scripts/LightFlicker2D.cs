using UnityEngine;
using UnityEngine.Rendering.Universal;
//adds a simple occasional full-off flicker to 2d lights

public class LightFlicker2D : MonoBehaviour
{
	//lights to flicker; if empty and autoFindChildLights is true, finds child light2d components
	[SerializeField] private Light2D[] lights;
	[SerializeField] private bool autoFindChildLights = true;

	//random wait between flickers
	[SerializeField] private float minIntervalSeconds = 8f;
	[SerializeField] private float maxIntervalSeconds = 25f;
	//off/on durations per flicker
	[SerializeField] private float offDurationSeconds = 0.08f;
	[SerializeField] private float onDurationSeconds = 0.06f;
	//number of quick off/on blips per burst
	[SerializeField] private int minFlashes = 2;
	[SerializeField] private int maxFlashes = 3;

	private float[] baseIntensities;
	private bool running;

	private void Reset()
	{
		if (autoFindChildLights) lights = GetComponentsInChildren<Light2D>(true);
	}

	private void Awake()
	{
		if ((lights == null || lights.Length == 0) && autoFindChildLights)
			lights = GetComponentsInChildren<Light2D>(true);
		CacheBaseIntensities();
	}

	private void OnEnable()
	{
		if (!running) StartCoroutine(FlickerLoop());
	}

	private void OnDisable()
	{
		RestoreBase();
		running = false;
	}

	private void CacheBaseIntensities()
	{
		if (lights == null) return;
		baseIntensities = new float[lights.Length];
		for (int i = 0; i < lights.Length; i++)
			baseIntensities[i] = lights[i] != null ? lights[i].intensity : 0f;
	}

	private System.Collections.IEnumerator FlickerLoop()
	{
		running = true;
		var rand = new System.Random(gameObject.GetInstanceID());
		while (enabled)
		{
			float wait = Mathf.Lerp(minIntervalSeconds, maxIntervalSeconds, (float)rand.NextDouble());
			yield return new WaitForSecondsRealtime(wait);

			int flashes = Mathf.Clamp(Random.Range(minFlashes, maxFlashes + 1), 1, 8);
			for (int f = 0; f < flashes; f++)
			{
				//off
				for (int i = 0; i < lights.Length; i++)
				{
					if (lights[i] == null) continue;
					lights[i].intensity = 0f;
				}
				yield return new WaitForSecondsRealtime(offDurationSeconds);

				//on (restore)
				for (int i = 0; i < lights.Length; i++)
				{
					if (lights[i] == null) continue;
					lights[i].intensity = (baseIntensities != null && i < baseIntensities.Length) ? baseIntensities[i] : lights[i].intensity;
				}
				yield return new WaitForSecondsRealtime(onDurationSeconds);
			}
		}
	}

	private void RestoreBase()
	{
		if (lights == null || baseIntensities == null) return;
		for (int i = 0; i < lights.Length; i++)
		{
			if (lights[i] == null) continue;
			lights[i].intensity = baseIntensities[i];
		}
	}
}


