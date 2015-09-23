using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {

	// Use this for initialization
	public Transform goal;
	
	void Start () {
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.destination = goal.position; 
	}
}
