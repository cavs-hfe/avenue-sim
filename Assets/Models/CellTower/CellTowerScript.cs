using UnityEngine;
using System.Collections;

public class CellTowerScript : MonoBehaviour {

	private Light towerLight;
	public GameObject towerLightCylinder;
	private MeshRenderer towerLightRenderer;
	public Material CellTowerLight;
	public Material CellTowerOff;

	// Use this for initialization
	void Start () {
		towerLight = GetComponentInChildren<Light> ();
		towerLightRenderer = towerLightCylinder.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if ((int)Time.time % 3 == 0) {
			towerLight.enabled = true;
			towerLightRenderer.material = CellTowerLight;
		} else {
			towerLight.enabled = false;
			towerLightRenderer.material = CellTowerOff;
		}
	}
}
