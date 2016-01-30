using UnityEngine;
using System.Collections;

namespace CAVS.Recording {

	public class ActorBehavior : MonoBehaviour {

		/// <summary>
		/// For storing minimal data, instead of saving everything about the GameObject like the
		/// components attatched and their values, when it comes time to view our recording we'll
		/// represent them as some basic object.
		/// 
		/// Eventually this should be replaced with our own enum for the objects we care about such
		/// as player, car, plane, robot, etc.
		/// </summary>
		[SerializeField]
		private PrimitiveType objToRepresentActorInPlayback = PrimitiveType.Cube;


		/// <summary>
		/// The name of actor in the recording
		/// </summary>
		[SerializeField]
		private string nameForRecording;


		/// <summary>
		/// Returns the name we'd like to use to represent us for any recordings
		/// </summary>
		/// <returns>The name for recording.</returns>
		public string getNameForRecording(){
			
			if (nameForRecording == null || nameForRecording == "") {
				return transform.name;
			}

			return nameForRecording;

		}


		/// <summary>
		/// returns the representation we'd like the actor to assume when playing the recording
		/// </summary>
		/// <returns>The object to represent actor.</returns>
		public PrimitiveType getObjToRepresentActor(){
			return objToRepresentActorInPlayback;
		}

	}

}