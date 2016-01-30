using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CAVS.Recording.Testing {

	public class MenuBehavior : MonoBehaviour {

		[SerializeField]
		private RecordingServiceBehavior recordingService;

		[SerializeField]
		private Text fileNameForRecording;

		[SerializeField]
		private Text buttonRecorderText;

		private GameObject actor;

		public void toggleRecording(){
			
			if (recordingService.currentelyRecording ()) {
				
				recordingService.stopAndSaveRecording ();
				Destroy (actor);
				buttonRecorderText.text = "Start";

			} else {

				if (fileNameForRecording.text == "") {
					Debug.LogError ("You need to enter a filename!");
					return;
				}

				actor = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				actor.AddComponent<SimpleBehavior> ();
				actor.AddComponent<ActorBehavior> ();
				actor.transform.position = new Vector3 (0,3,0);
				recordingService.startRecording (fileNameForRecording.text);
				buttonRecorderText.text = "Stop";

			}

		}

	}

}