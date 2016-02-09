using UnityEngine;
using System.Collections;

namespace CAVS.Recording {

	/// <summary>
	/// Any monobehavior class that needs be disabled while a playback is happening of a 
	/// recording should extend from this class to the playback service knows what to
	/// disable.
	/// </summary>
	public abstract class PlaybackInterferenceBehavior: MonoBehaviour {

		[SerializeField]
		private bool disableOnPlayback = true;

		public bool shouldDisableOnPlayback(){
			return disableOnPlayback;
		}

	}

}