using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour {

	public bool lightsOn = false;
	public bool autoLights = false;
	public DayNightScript dayNightScript;

	private bool areLightsOnNow = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (autoLights) {
			if (dayNightScript.currentTimeOfDay < 0.27f || dayNightScript.currentTimeOfDay > 0.75f) {
				lightsOn = true;
			} else {
				lightsOn = false;
			}
		}

		if (lightsOn && !areLightsOnNow) {
			Light[] lights = GetComponentsInChildren<Light> ();
			foreach (Light l in lights) {
				l.enabled = true;
				areLightsOnNow = true;
			}
		} else if (!lightsOn && areLightsOnNow) {
			Light[] lights = GetComponentsInChildren<Light> ();
			foreach (Light l in lights) {
				l.enabled = false;
				areLightsOnNow = false;
			}
		}
	}
}
