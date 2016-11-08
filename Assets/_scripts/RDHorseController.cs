using UnityEngine;
using System.Collections;

public class RDHorseController : MonoBehaviour {

    public GameObject riderParent;

    private Animator anim;
    public float rotateFactor = 0.5f;

    private int RideSpeedHash = Animator.StringToHash("RideSpeed");

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    void OnEnable()
    {
        EasyTouch.On_TouchStart += onTouchDown;
        EasyTouch.On_Swipe += onSwipe;
        EasyTouch.On_Drag += onSwipe;
        EasyTouch.On_TouchUp += onTouchUp;
    }

    void OnDestory()
    {
        EasyTouch.On_TouchStart -= onTouchDown;
        EasyTouch.On_Swipe -= onSwipe;
        EasyTouch.On_Drag -= onSwipe;
        EasyTouch.On_TouchUp -= onTouchUp;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void onTouchDown(Gesture gesture)
    {
        //Debug.Log("onTouchDown");
        anim.SetFloat(RideSpeedHash, 1.0f);
    }

    void onSwipe(Gesture gesture)
    {
        //Debug.Log("onSwipe");
        Vector3 rotateVect = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotateVect + new Vector3(0.0f, gesture.deltaPosition.x * rotateFactor, 0.0f));
    }

    void onTouchUp(Gesture gesture)
    {
        //Debug.Log("onTouchUp");
        anim.SetFloat(RideSpeedHash, 0.0f);
    }
}
