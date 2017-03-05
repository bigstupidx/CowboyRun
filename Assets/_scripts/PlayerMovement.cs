using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour {

	private RiderController rc;

	// Use this for initialization
	void Start () {
		rc = GetComponent<RiderController> ();
	}

	void OnEnable()
	{
		EasyTouch.On_TouchStart += onTouchDown;
		EasyTouch.On_TouchUp += onTouchUp;
	}

	void OnDisable()
	{
		EasyTouch.On_TouchStart -= onTouchDown;
		EasyTouch.On_TouchUp -= onTouchUp;
	}

	void onTouchDown(Gesture gesture)
	{
		if (rc.Jumping && !rc.Dead){
			// find a horse...
			GameObject horse = rc.searchHrose();
			if (horse != null){
				Time.timeScale = 0.1f;
				Vector3 pos = horse.transform.position;
				pos.y = pos.y + 0.7f;
				pos.z = pos.z + 1.0f;
				Tween tw = gameObject.transform.DOMove (pos, 0.2f);
				tw.OnComplete(() => {
					Time.timeScale = 1f;
					rc.RideHorse (horse.GetComponent<RDHorseController>(), true);
				});
				tw.SetUpdate (true);
				tw.timeScale = 0.5f;
				GetComponent<Rigidbody>().detectCollisions = true;
			}
		}
	}

	void onTouchUp(Gesture gesture)
	{
		// should Jump...
		if (!rc.Jumping && !rc.Jumping && !rc.Dead)//&& !FallDowning)
			rc.Jump();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
