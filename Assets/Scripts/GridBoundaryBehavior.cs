using UnityEngine;
using System.Collections;

public class GridBoundaryBehavior : MonoBehaviour {

	[SerializeField]
	private GameObject target = null;

	// Use this for initialization
	void Start () {
	
		gameObject.GetComponent<MeshRenderer> ().material.mainTexture = getTexture(1);

		// Set rendering mode to Fade
		gameObject.GetComponent<MeshRenderer> ().material.SetFloat ("_Mode",2f);
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(target == null){
			return;
		}

		float distanceFromCenterToEdge = transform.lossyScale.magnitude/3f;
		float str = (Vector3.Distance(transform.position, target.transform.position)/distanceFromCenterToEdge);
		gameObject.GetComponent<MeshRenderer> ().material.mainTexture = getTexture(str);

		float maxTiling = 30f;
		gameObject.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (maxTiling, maxTiling)*Mathf.Clamp(Mathf.Pow(str,3), 0.1f, 1f);
	
	}


	private Texture2D textureForMaterial = null;
	private float lastAlpha = 0;

	private Texture2D getTexture(float alph){

		float alpha = Mathf.Clamp01 (alph);

		if (textureForMaterial == null || lastAlpha  != alpha) {

			lastAlpha = alpha;

			textureForMaterial = new Texture2D (400,400);
		
			int borderWidth = 5;
			Color bordercolor = new Color(103f/255f, 230f/255f, 236f/255f, alpha);


			for (int x = 0; x < textureForMaterial.width; x++) {

				for (int y = 0; y < textureForMaterial.height; y++) {

					if ((x < borderWidth || x > textureForMaterial.width - borderWidth) || (y < borderWidth || y > textureForMaterial.height - borderWidth)) {
						textureForMaterial.SetPixel (x, y, bordercolor);
					} else {
						textureForMaterial.SetPixel (x, y, Color.clear);
					}

				}

			}

			textureForMaterial.Apply ();

		}

		return textureForMaterial;

	}

}