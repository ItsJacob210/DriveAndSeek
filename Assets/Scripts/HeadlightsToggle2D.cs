using UnityEngine;
using UnityEngine.Rendering.Universal; //light2D
//toggles an array of 2d headlights on/off with smoothing

public class HeadlightsToggle2D : MonoBehaviour
{
	[Header("Lights to control (assign both headlight cones)")]
	public Light2D[] lights;

	[Header("Toggle Settings")]
	public KeyCode toggleKey = KeyCode.H;
	public bool lightsOnAtStart = true;
	public float toggleLerpSpeed = 12f; // smoothing when toggling

	[Header("Intensity (scale existing per-light intensities)")]
	public float onScale = 1f;
	public float offScale = 0f;
	private bool isOn;
	private float[] baseIntensities;

	private void Awake()
	{
		isOn = lightsOnAtStart;
		CacheBaseIntensities();
		//snap to initial state
		ApplyScaleImmediate(isOn ? onScale : offScale);
	}

	private void CacheBaseIntensities()
	{
		if (lights == null) return;
		baseIntensities = new float[lights.Length];
		for (int i = 0; i < lights.Length; i++)
		{
			baseIntensities[i] = lights[i] != null ? lights[i].intensity : 0f;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(toggleKey))
		{
			isOn = !isOn;
		}

		float targetScale = isOn ? onScale : offScale;
		LerpToScale(targetScale);
	}

	private void LerpToScale(float targetScale)
	{
		if (lights == null || baseIntensities == null) return;
		for (int i = 0; i < lights.Length; i++)
		{
			var l = lights[i];
			if (l == null) continue;
			float targetIntensity = baseIntensities[i] * targetScale;
			l.intensity = Mathf.Lerp(l.intensity, targetIntensity, toggleLerpSpeed * Time.deltaTime);
		}
	}

	private void ApplyScaleImmediate(float scale)
	{
		if (lights == null || baseIntensities == null) return;
		for (int i = 0; i < lights.Length; i++)
		{
			var l = lights[i];
			if (l == null) continue;
			l.intensity = baseIntensities[i] * scale;
		}
	}
}


