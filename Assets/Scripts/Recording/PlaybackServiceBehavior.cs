using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CAVS.Recording {

	public class PlaybackServiceBehavior : MonoBehaviour {

		enum PlaybackState {
			Stopped,
			Paused,
			Playing
		}


		private Recording currrentLoadedRecording = null;


		private PlaybackState currentPlaybackState = PlaybackState.Stopped;

		private float timePlaybackStarted = 0;

		private float timeFrameStarted = 0;

		private int frameCurrentelyOn = 0;


		private Dictionary<int, GameObject> actors;

		/// <summary>
		/// Gets all recordings file names found in our designated recordings folder
		/// </summary>
		/// <returns>All Files found in recordings folder.</returns>
		public string[] getAllRecordingsFileNames(){

			string[] recordings = Directory.GetFiles ("Recordings");

			// Clean the name
			for (int i = 0; i < recordings.Length; i++) {
				recordings[i] = Path.GetFileName (recordings[i]);
			}

			return recordings;

		}


		public bool loadRecording(string recordingname){

			XmlSerializer serializer = new XmlSerializer (typeof(Recording));

			using (FileStream fileStream = new FileStream("Recordings/"+recordingname,FileMode.Open)) 
			{
				Recording result = (Recording) serializer.Deserialize(fileStream );
				Debug.Log (result.getDuration());
				currrentLoadedRecording = result;
				return true;
			}

			return false;
		}


		public void playLoadedRecording(){

			if (currrentLoadedRecording == null) {
				Debug.LogError ("Trying to play a recording when one isn't loaded yet!");
				return;
			}

			actors = new Dictionary<int, GameObject> ();
			currentPlaybackState = PlaybackState.Playing;
			frameCurrentelyOn = 0;
			timePlaybackStarted = Time.time;
			timeFrameStarted = Time.time;

			for (int i = 0; i < currrentLoadedRecording.ActorIds.Length; i++) {

				int id = currrentLoadedRecording.ActorIds [i];
				actors [id] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				actors [id].transform.position = currrentLoadedRecording.Frames [0].getPositionOfActor(id);
				actors [id].transform.rotation = Quaternion.Euler(currrentLoadedRecording.Frames [0].getRotationOfActor(id));

			}

		}


		public void pauseLoadedRecording(){
			
			if (currrentLoadedRecording == null) {
				Debug.LogError ("Trying to pause a recording when one isn't loaded yet!");
				return;
			}

			currentPlaybackState = PlaybackState.Paused;

		}


		void Update(){

			if (currentPlaybackState == PlaybackState.Playing) {

				if(frameCurrentelyOn == currrentLoadedRecording.Frames.Length -1){

					if(getTimeThroughRecording() >= currrentLoadedRecording.getDuration()){
						frameCurrentelyOn = 0;
						timeFrameStarted = Time.time;
						timePlaybackStarted = Time.time;
					}

				} else if(getTimeThroughRecording() >= currrentLoadedRecording.Frames [frameCurrentelyOn + 1].TimeStamp -currrentLoadedRecording.Frames [0].TimeStamp){
					frameCurrentelyOn++;
					timeFrameStarted = Time.time;
				}

				foreach(KeyValuePair<int, GameObject> actor in actors){

					float currentPercentageThroughFrame = (currrentLoadedRecording.Frames [frameCurrentelyOn].TimeStamp -currrentLoadedRecording.Frames [0].TimeStamp)/getTimeThroughRecording();

					// Lerp through frames so no matter what the frame rate is it'll appear smooth
					actor.Value.transform.position = Vector3.Slerp (actor.Value.transform.position, currrentLoadedRecording.Frames[frameCurrentelyOn].getPositionOfActor(actor.Key), currentPercentageThroughFrame);
					actor.Value.transform.rotation = Quaternion.Euler(Vector3.Slerp (actor.Value.transform.rotation.eulerAngles, currrentLoadedRecording.Frames[frameCurrentelyOn].getRotationOfActor(actor.Key), currentPercentageThroughFrame));

				}

			}

		}

		private float getTimeThroughRecording(){
			return Time.time - timePlaybackStarted;
		}


	}

}