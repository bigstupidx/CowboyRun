using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

	public Camera mainCamera;

	// Update is called once per frame
	void Update () {
		if (mainCamera == null){
			mainCamera = Camera.main;
		}
		transform.LookAt (mainCamera.transform);
	}
}
