﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CAVS.IntersectionControl{

	public class IntersectionController : MonoBehaviour {

		[SerializeField]
		private List<StopLightBehavior> northSouthStopLights;


		[SerializeField]
		private List<StopLightBehavior> eastWestStopLights;


		/// <summary>
		/// Are the south/north lanes currentely green
		/// </summary>
		private bool snEnabled = true;


		/// <summary>
		/// Adds the stop light to the north/south category.
		/// </summary>
		/// <param name="stoplight">Stoplight.</param>
		public void addNorthSouthStopLight(StopLightBehavior stoplight){
			this.northSouthStopLights.Add (stoplight);
		}


		/// <summary>
		/// Adds the stop light to the east/west category.
		/// </summary>
		/// <param name="stoplight">Stoplight.</param>
		public void addEastWestStopLight(StopLightBehavior stoplight){
			this.eastWestStopLights.Add (stoplight);
		}


		/// <summary>
		/// Toggles what road is currentely green.
		/// </summary>
		public void toggle(){

			toggle (0);

		} 


		/// <summary>
		/// Toggle the specified delay.
		/// </summary>
		/// <param name="delay">Delay.</param>
		public void toggle(float delay){

			toggle (delay, 0);

		}


		/// <summary>
		/// Toggles which of the lanes are green.
		/// </summary>
		/// <param name="yellowLightDuration">how long the currentely green lane will stay in the yellow light before turning red.</param>
		/// <param name="greenLightduractionAfterRed">How long the originally red light will stay red after the green lane turning red.</param>
		public void toggle(float yellowLightDuration, float greenLightduractionAfterRed){

			if (snEnabled) {
				switchLightsToStop (northSouthStopLights.ToArray(), yellowLightDuration);
				switchLightsToGo (eastWestStopLights.ToArray(), yellowLightDuration+greenLightduractionAfterRed);
			} else {
				switchLightsToStop (eastWestStopLights.ToArray(), yellowLightDuration);
				switchLightsToGo (northSouthStopLights.ToArray(), yellowLightDuration+greenLightduractionAfterRed);
			}

			snEnabled = !snEnabled;

		}



		private void switchLightsToStop(StopLightBehavior[] lights, float delay){

			float sanatizedDelay = Mathf.Clamp (delay, 0, 100000);

			for (int i = 0; i < lights.Length; i++) {

				if (delay != 0) {

					lights [i].changeToYellow ();
					lights [i].changeToRed (sanatizedDelay);

				} else {

					lights [i].changeToRed ();

				}

			}

		}


		private void switchLightsToGo(StopLightBehavior[] lights, float delay){

			float sanatizedDelay = Mathf.Clamp (delay, 0, 100000);

			for (int i = 0; i < lights.Length; i++) {

				if (delay != 0) {

					lights [i].changeToGreen (delay);

				} else {

					lights [i].changeToGreen ();

				}

			}

		}


		private float lastDemoToggleTime = -1;

		/// <summary>
		/// Toggles lights after a short amount of time
		/// </summary>
		private void demo(){

			if(lastDemoToggleTime  +6f < Time.time){

				lastDemoToggleTime = Time.time;

				toggle (2, .5f);

			}

		}


		void Start(){

			if (northSouthStopLights == null) {
				northSouthStopLights = new List<StopLightBehavior> ();
			}

			if (eastWestStopLights == null) {
				eastWestStopLights = new List<StopLightBehavior> (); 
			}

			switchLightsToGo (northSouthStopLights.ToArray(), 0);
			switchLightsToStop (eastWestStopLights.ToArray(), 0);
			snEnabled = true;

		}



		// Update is called once per frame
		void Update () {
		
			demo ();

		}

	}

}