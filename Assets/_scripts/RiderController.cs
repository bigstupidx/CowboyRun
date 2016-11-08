using UnityEngine;
using System.Collections;

public class RiderController : MonoBehaviour {

    public GameObject horse;
    public GameObject circle;

    private Animator anim;
    private Rigidbody rigidBody;
    private int RideSpeedHash = Animator.StringToHash("RideSpeed");
    private int JumpHash = Animator.StringToHash("jump");
    private int DeadHash = Animator.StringToHash("dead");
    private bool jumping = false;
    private bool dead = false;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
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
        if (jumping)
        {
            circle.transform.position = new Vector3( transform.position.x, 0 , transform.position.z);
        }
	}

    void FixedUpdate()
    {
        
    }

    void onTouchDown(Gesture gesture)
    {
        Debug.Log("onTouchDown");
        if (jumping)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5.0f);
            if (hitColliders.Length > 0)
            {
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    Debug.Log(hitColliders[i].gameObject.tag);
                }
            }
        }
        anim.SetFloat(RideSpeedHash, 1.0f);
    }

    void onSwipe(Gesture gesture)
    {
        Debug.Log("onSwipe");
        
    }

    void onTouchUp(Gesture gesture)
    {
        Debug.Log("onTouchUp");
        anim.SetFloat(RideSpeedHash, 0.0f);
        Jump();
    }

    void Jump()
    {
        if (!jumping)
        {
            jumping = true;
            transform.parent = null;
            anim.SetBool(JumpHash,true);

            rigidBody.useGravity = true;
            rigidBody.isKinematic = false;
            Vector3 v = transform.forward.normalized;
            transform.rotation = horse.transform.rotation;
            //rigidBody.AddForce(transform.right + transform.up * 8 + transform.forward*10, ForceMode.Impulse);
            rigidBody.AddForce(transform.up * 8 + horse.transform.forward * 10, ForceMode.Impulse);

            //horse.SetActive(false);
            Destroy(horse);
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Terrain"))
        {
            Debug.Log("OnCollisionEnter  " + collisionInfo.gameObject.tag);
            if (!dead)
            {
                dead = true;
                anim.SetBool(DeadHash, dead);
            }
        }
    }

    void RideHorse(GameObject horse)
    {
        RDHorseController ctrl = horse.GetComponent<RDHorseController>();
        if (ctrl != null)
        {
            jumping = false;
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            transform.parent = ctrl.riderParent.transform;
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, -90));
        }
    }

    //void OnCollisionStay(Collision collisionInfo)
    //{
    //    Debug.Log("OnCollisionStay  " + collisionInfo.gameObject.tag);
    //}
}
