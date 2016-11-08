using UnityEngine;
using System.Collections;

public class HorseRunForward : MonoBehaviour {

    public float runSpeed = 1.0f;
    private Animator anim;
    private int RideSpeedHash = Animator.StringToHash("RideSpeed");

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    void OnEnable()
    {
        anim.SetFloat(RideSpeedHash, runSpeed);
    }

    //void OnDisable()
    //{
    //    anim.SetFloat(RideSpeedHash, 0.0f);
    //}

    void SetSpeed(float speed)
    {
        runSpeed = speed;
        anim.SetFloat(RideSpeedHash, runSpeed);
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
