using UnityEngine;
using System.Collections;

public class HorseMovement : MonoBehaviour {

	public float rotateFactor = 0.5f;
	public float RunSpeed = 0.0f;
	public float RideSpeed = 0.0f;

	private int MaxRotateDeg = 50;
	private RDHorseController hc;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		hc = GetComponent<RDHorseController> ();
		rb = GetComponent<Rigidbody> ();
	}

	public void OnBeenRide()
	{
		EasyTouch.On_Swipe += onSwipe;
		EasyTouch.On_Drag += onSwipe;
	}

	public void OnNotBeenRide()
	{
		EasyTouch.On_Swipe -= onSwipe;
		EasyTouch.On_Drag -= onSwipe;
	}

	// control the horse rotation...
	void onSwipe (Gesture gesture) {
		if (hc != null && hc.BeenRide){
			Vector3 deltaVect = gesture.deltaPosition;
			Vector3 curVect = transform.rotation.eulerAngles;
			Vector3 rotateVect = curVect + new Vector3 (0.0f, deltaVect.x * rotateFactor, 0.0f);
			//Debug.Log (rotateVect.y);
			if (rotateVect.y >= 180)
				rotateVect.y = Mathf.Clamp (rotateVect.y, 360-MaxRotateDeg, 360);
			else if (rotateVect.y < 180 && rotateVect.y >= 0)
				rotateVect.y = Mathf.Clamp (rotateVect.y, 0, MaxRotateDeg);
			else if(rotateVect.y < 0)
				rotateVect.y = Mathf.Clamp (rotateVect.y, -MaxRotateDeg, 0);
			transform.rotation = Quaternion.Euler(rotateVect);
		}
	}
		

	void FixedUpdate(){
		if (hc.Run) {
			rb.velocity = Vector3.zero;
			float speed = hc.BeenRide ? RideSpeed : RunSpeed;
			//rb.MovePosition (transform.position + transform.forward * speed * Time.deltaTime);
			transform.position += transform.forward * speed * Time.deltaTime;
		}
		if (!hc.BeenRide){
			// keep the raw horse run forward
			Vector3 angle = transform.rotation.eulerAngles;
			angle.y =  Mathf.Lerp (angle.y, 0, Time.deltaTime * 2);
			rb.MoveRotation(Quaternion.Euler(angle));
		}
	}
}
