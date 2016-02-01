using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CAVS.Recording.Testing {

	public class MenuBehavior : MonoBehaviour {

		[SerializeField]
		private RecordingServiceBehavior recordingService;

		[SerializeField]
		private PlaybackServiceBehavior playbackService;

		[SerializeField]
		private Text fileNameForRecording;

		[SerializeField]
		private Text buttonRecorderText;

		[SerializeField]
		private GameObject recordPanel;

		[SerializeField]
		private GameObject playbackPanel;

		[SerializeField]
		private GameObject playbackFileList;

		[SerializeField]
		private GameObject playbackFileButtonPrefab;

		[SerializeField]
		private GameObject playbackControlsPanel;

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

		public void toggleRecordPanel(){
		
			recordPanel.SetActive (!recordPanel.activeSelf);
			playbackPanel.SetActive (false);
			playbackControlsPanel.SetActive (false);
		
		}

		public void toggleLoadPanel(){

			playbackPanel.SetActive (!playbackPanel.activeSelf);
			recordPanel.SetActive (false);
			playbackControlsPanel.SetActive (false);

			// If we're displaying the panel this toggle
			if(playbackPanel.activeSelf){


				// Delete old files
				while(playbackFileList.transform.childCount > 0){

					Transform c = playbackFileList.transform.GetChild (0);
					c.SetParent (null);
					Destroy (c.gameObject);

				}


				// Create a button for every file
				for(int i = 0; i < playbackService.getAllRecordingsFileNames().Length; i ++){

					GameObject file = (GameObject)Instantiate (playbackFileButtonPrefab);
					string fileName = playbackService.getAllRecordingsFileNames () [i];
					file.transform.SetParent (playbackFileList.transform);
					file.GetComponentInChildren<Text> ().text = fileName;
					file.GetComponent<Button> ().onClick.AddListener (() => loadFile(fileName));
				}

			}

		}


		public void loadFile(string buttonPressed){
			
			if (playbackService.loadRecording (buttonPressed)) {
				playbackControlsPanel.SetActive (true);
			}

		}


		public void playLoadedRecording(){
			playbackService.playLoadedRecording ();
		}


		public void pauseLoadedRecording(){
			playbackService.pauseLoadedRecording ();
		}


	}

}