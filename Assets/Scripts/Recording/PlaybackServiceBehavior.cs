using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CAVS.Recording {


	/// <summary>
	/// This is used for playing a single recording at a time inside the scene.
	/// </summary>
	public class PlaybackServiceBehavior : MonoBehaviour {


		enum PlaybackState {
			Stopped,
			Paused,
			Playing
		}


		/// <summary>
		/// The currrent loaded recording that we may or maynot be playing
		/// </summary>
		private Recording currrentLoadedRecording = null;


		/// <summary>
		/// The current state we're in that determines whether or not we're animating actors
		/// </summary>
		private PlaybackState currentPlaybackState = PlaybackState.Stopped;


		/// <summary>
		/// The time in the game that playback of a recording started.
		/// </summary>
		private float timePlaybackStarted = 0;


		/// <summary>
		/// The time the current frame we are on started
		/// </summary>
		private float timeFrameStarted = 0;


		/// <summary>
		/// The index that references the frame in the currentely loaded recording
		/// that we are currentely animating.
		/// </summary>
		private int frameCurrentelyOn = 0;


		/// <summary>
		/// How long we have spent paused during our playback
		/// </summary>
		private float timeSpentPaused = 0;


		/// <summary>
		/// The actors currentely being animated from the recording
		/// </summary>
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


		/// <summary>
		/// Clears the current loaded recording of all actors in the scene
		/// </summary>
		public void clearCurrentLoadedRecording(){

			if (actors != null) {
				foreach (KeyValuePair<int, GameObject> actor in actors) {
					Destroy (actor.Value);
				}
			}

			currentPlaybackState = PlaybackState.Stopped;
			currrentLoadedRecording = null;

		}


		/// <summary>
		/// Loads the recording located in the Recordings folder.
		/// Will return a recording object if the loading was succesful
		/// </summary>
		/// <returns>The recording.</returns>
		/// <param name="recordingname">Recordingname.</param>
		public Recording loadRecording(string recordingname){

			// Clear out any recording that might be loaded
			clearCurrentLoadedRecording();

			// Load in the new recording
			XmlSerializer serializer = new XmlSerializer (typeof(Recording));

			try{
				using (FileStream fileStream = new FileStream("Recordings/"+recordingname,FileMode.Open)) 
				{
					Recording result = (Recording) serializer.Deserialize(fileStream);
					Debug.Log (result.getDuration());
					currrentLoadedRecording = result;
					return currrentLoadedRecording;
				}
			}
			catch{
				return null;
			}

		}


		/// <summary>
		/// If a recording has been loaded, then this will spawn all actors in the scene and begin
		/// animating them.
		/// </summary>
		public void playLoadedRecording(){

			if (currrentLoadedRecording == null) {
				Debug.LogError ("Trying to play a recording when one isn't loaded yet!");
				return;
			}

			if (currentPlaybackState == PlaybackState.Paused) {
				currentPlaybackState = PlaybackState.Playing;
				return;
			}

			// Reset previous recording variables
			actors = new Dictionary<int, GameObject> ();
			currentPlaybackState = PlaybackState.Playing;
			frameCurrentelyOn = 0;
			timePlaybackStarted = Time.time;
			timeFrameStarted = Time.time;
			timeSpentPaused = 0f;
			velocitys = new Vector3[currrentLoadedRecording.ActorIds.Length];

			for (int i = 0; i < currrentLoadedRecording.ActorIds.Length; i++) {

				// Get the unique Id of the actor
				int id = currrentLoadedRecording.ActorIds [i];

				// Create the actor
				GameObject actorRef = (GameObject)Resources.Load(currrentLoadedRecording.getActorPreferedRepresentation(id));
				actors [id] = GameObject.Instantiate(actorRef);
				actors [id].transform.name = currrentLoadedRecording.getActorName (id);
				actors [id].transform.position = currrentLoadedRecording.Frames [0].getPositionOfActor(id);
				actors [id].transform.rotation = Quaternion.Euler(currrentLoadedRecording.Frames [0].getRotationOfActor(id));

			}

		}


		/// <summary>
		/// If a recording is playing, then it will become paused.
		/// </summary>
		public void pauseLoadedRecording(){
			
			if (currrentLoadedRecording == null) {
				Debug.LogError ("Trying to pause a recording when one isn't loaded yet!");
				return;
			}

			if (currentPlaybackState == PlaybackState.Playing) {
				currentPlaybackState = PlaybackState.Paused;
			}

		}


		/// <summary>
		/// The velocitys of the actors for smooth interpolation between frames
		/// </summary>
		private Vector3[] velocitys;

		void Update(){

			if (currentPlaybackState == PlaybackState.Playing) {

				// Determine whether or not it's time to go to the next frame

				if(frameCurrentelyOn == currrentLoadedRecording.Frames.Length -1){

					if(getTimeThroughRecording() >= currrentLoadedRecording.getDuration()){
						frameCurrentelyOn = 0;
						timeFrameStarted = Time.time;
						timePlaybackStarted = Time.time;
						timeSpentPaused = 0;
					}

				} else if(getTimeThroughRecording() >= currrentLoadedRecording.Frames [frameCurrentelyOn + 1].TimeStamp -currrentLoadedRecording.Frames [0].TimeStamp){
					frameCurrentelyOn++;
					timeFrameStarted = Time.time;
				}

				// Go through each actor and move them to their correct position
				int i = 0;
				foreach(KeyValuePair<int, GameObject> actor in actors){

					// Lerp through frames so no matter what the frame rate is it'll appear smooth
					actor.Value.transform.position = Vector3.SmoothDamp (actor.Value.transform.position, currrentLoadedRecording.Frames[frameCurrentelyOn].getPositionOfActor(actor.Key), ref velocitys[i], 1f/currrentLoadedRecording.FrameRateRecordedAt);
					actor.Value.transform.rotation = Quaternion.Euler(Vector3.Slerp (actor.Value.transform.rotation.eulerAngles, currrentLoadedRecording.Frames[frameCurrentelyOn].getRotationOfActor(actor.Key), .1f));

					i++;
				}

			}

			if (currentPlaybackState == PlaybackState.Paused) {
				timeSpentPaused += Time.deltaTime;
			}

		}


		/// <summary>
		/// The time in seconds of how long the recording has been playing.
		/// </summary>
		/// <returns>The time through recording.</returns>
		private float getTimeThroughRecording(){
			return Time.time - timePlaybackStarted-timeSpentPaused;
		}


	}

}