using UnityEngine;
using System.Collections;

namespace CAVS.Recording.Testing {

	public class SimpleBehavior : MonoBehaviour {

		// Update is called once per frame
		void Update () {
			transform.Translate (Mathf.Cos(Time.time) * 1 *Time.deltaTime, Mathf.Sin(Time.time) * 1 *Time.deltaTime,0);
		}

	}

}