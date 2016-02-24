using UnityEngine;
using System.Collections;

namespace CAVS.Recording.Testing {

	public class SimpleBehavior : CAVS.Recording.PlaybackActorBehavior {


		public override void handleEvent(string name, string contents){
			Debug.Log (name + " : " + contents);
		}


		// Update is called once per frame
		void Update () {
			transform.Translate (Mathf.Cos(Time.time) * 1 *Time.deltaTime, Mathf.Sin(Time.time) * 1 *Time.deltaTime, 0);
			transform.Rotate (new Vector3(0,10*Time.deltaTime,0));

			if (Input.GetKeyDown (KeyCode.A)) {
				RecordingServiceBehavior service = GameObject.FindObjectOfType<RecordingServiceBehavior> ();
				service.logEventToRecording ("Input","A");
				Debug.Log ("Hit A");
			}

		}


	}

}