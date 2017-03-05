using UnityEngine;
using System.Collections;


public class fixedCam : MonoBehaviour {
    public Transform target;  //the target what the camare look for
    public float smooth;

	private Vector3 offset;

	void Start(){
		offset = transform.position - target.transform.position;
	}

	void LateUpdate () {
        Vector3 targetPos = target.transform.position;
		targetPos.y = 0;
		//transform.position = Vector3.Lerp (transform.position, targetPos + offset, smooth * Time.deltaTime);
		transform.position = targetPos + offset;
	}

}
