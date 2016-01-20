using UnityEngine;
using System.Collections;

public class CarAlarmTriggerScript : MonoBehaviour {

	public GameObject trigger;
	
	private bool triggered = false;

	private AudioSource audioSource;
	
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (triggered) {
			audioSource.Play();
			triggered = false;
		}
	}
	
	void OnTriggerEnter(Collider other) {
		
		if (other.gameObject.Equals (trigger)) {
			triggered = true;
		}
	}
}
