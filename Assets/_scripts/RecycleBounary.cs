using UnityEngine;
using System.Collections;

public class RecycleBounary : MonoBehaviour {

	public Transform player;

	void FixedUpdate(){
		// follow the player
		gameObject.transform.position = player.transform.position;
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag ("Horse")) {
			// recycle this horse... TODO: use objectpooler
			Destroy(other.gameObject);
		}
	}
}
