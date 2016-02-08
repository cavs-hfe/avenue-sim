using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace CAVS.Recording.UI {

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

		[SerializeField]
		private Toggle shouldAutoIncrementTrialRecordings;

		[SerializeField]
		private GameObject errorPanel;

		public void toggleRecording(){
			
			if (recordingService.currentelyRecording ()) {
				
				recordingService.stopAndSaveRecording ();
				buttonRecorderText.text = "Start";

			} else {

				if (fileNameForRecording.text == "") {
					Debug.LogError ("You need to enter a filename!");
					return;
				}

				string fileName = fileNameForRecording.text;

				string pattern = @"\d+$";
				string replacement = "";
				Regex rgx = new Regex(pattern);

				int numOfFilesWithSameName = 1;

				// We need to figure out what number to put on the end of it
				if (shouldAutoIncrementTrialRecordings.isOn) {

					string[] allCurrentFileNames = PlaybackServiceBehavior.getAllRecordingsFileNames ();

					for (int i = 0; i < allCurrentFileNames.Length; i++) {

						// Remove .xml extensions
						allCurrentFileNames [i] = allCurrentFileNames [i].Substring (0, allCurrentFileNames [i].Length - 4);

						// Remove numbers at end of file
						allCurrentFileNames [i] = rgx.Replace (allCurrentFileNames [i], replacement);

						// If file names match
						if (allCurrentFileNames [i].Equals (fileName)) {
							numOfFilesWithSameName++;
						}

					}

					fileName += numOfFilesWithSameName;

				} 
				// Check if any names already saved match a file already saved
				else {

					string[] allCurrentFileNames = PlaybackServiceBehavior.getAllRecordingsFileNames ();

					for (int i = 0; i < allCurrentFileNames.Length; i++) {

						// Remove .xml extensions
						allCurrentFileNames [i] = allCurrentFileNames [i].Substring (0, allCurrentFileNames [i].Length - 4);

						// If file names match
						if (allCurrentFileNames [i].Equals (fileName)) {

							// Show an error on the screen preventing an overwrite
							GameObject errorInstance = Instantiate(errorPanel);
							errorInstance.transform.SetParent(playbackPanel.transform.parent,false);
							Destroy(errorInstance,3f);

							return;

						}

					}

				}

				recordingService.startRecording (fileName);
				buttonRecorderText.text = "Stop";

			}

		}

		public void toggleRecordPanel(){
		
			recordPanel.SetActive (!recordPanel.activeSelf);
			playbackPanel.SetActive (false);
			playbackControlsPanel.SetActive (false);
			playbackService.clearCurrentLoadedRecording ();

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
				for(int i = 0; i < PlaybackServiceBehavior.getAllRecordingsFileNames().Length; i ++){

					GameObject file = (GameObject)Instantiate (playbackFileButtonPrefab);
					string fileName = PlaybackServiceBehavior.getAllRecordingsFileNames () [i];
					file.transform.SetParent (playbackFileList.transform);
					file.GetComponentInChildren<Text> ().text = fileName;
					file.GetComponent<Button> ().onClick.AddListener (() => loadFile(fileName));

				}

			}

		}


		public void loadFile(string buttonPressed){
			
			if (playbackService.loadRecording (buttonPressed) != null) {
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