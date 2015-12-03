using UnityEngine;
using System.Collections;

public class AndromedaControlScript : MonoBehaviour {

	private Animator myAnimator;

	private int lastSecond = 0;
	private int nextActionHash = Animator.StringToHash("NextAction");

	// Use this for initialization
	void Start () {
		myAnimator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if ((int)Time.time > lastSecond) {
			myAnimator.SetInteger (nextActionHash, Random.Range (0, 4));
			lastSecond = (int)Time.time;
		}
	}
}
