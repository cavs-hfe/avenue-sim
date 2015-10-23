using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour {

	public bool lightsOn = false;

	private bool areLightsOnNow = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
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
