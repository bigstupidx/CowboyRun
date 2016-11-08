using UnityEngine;
using System.Collections;

public class fixedCam : MonoBehaviour {
    public Transform target;  //the target what the camare look for
    public Vector3 normalDirection = new Vector3(0.5f,0.5f,0.5f);
    public float distance;

	// Use this for initialization
	void Start () {
		
	}

	void OnEnable(){
		
	}

	void OnDestroy(){
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetPos = target.transform.position;
        Vector3 newPos = new Vector3();
		newPos = targetPos + normalDirection.normalized * distance;
        transform.position = newPos;
		transform.LookAt (target.position);
	}

}
