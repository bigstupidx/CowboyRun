using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class RiderController : MonoBehaviour {

    public GameObject horse;
    public GameObject circle;
	public float searchRadio = 10f;
	public GameObject Smoke;

    private Animator anim;
    private Rigidbody rigidBody;
    private int RideSpeedHash = Animator.StringToHash("RideSpeed");
    private int JumpHash = Animator.StringToHash("jump");
    private int DeadHash = Animator.StringToHash("dead");
    private bool m_jumping = false;
    private bool dead = false;
	private GameObject lastFindHorse;
    private int jumpCount; //跳的次数，光圈随次数衰减
	private bool FallDowning = true;

	[HideInInspector]
	public bool Jumping{
		set{
			m_jumping = value;
		}
		get{
			return m_jumping;
		}
	}

	[HideInInspector]
	public bool Dead{
		set{
			dead = value;
		}
		get{
			return dead;
		}
	}

	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_jumping && !dead)
        {
            circle.transform.position = new Vector3( transform.position.x, 0 , transform.position.z);
//			GameObject FindHorse = searchHrose ();
//			//Material[] mats = FindHorse.GetComponentInChildren<SkinnedMeshRenderer> ().materials;
////			for (int i= 0; i< mats.Length; i++){
////				// replace the maters...
////				//mats [i] = xx;
////			}
//			if (FindHorse != null) {
//				if (!FindHorse.Equals(lastFindHorse)){
//					if (lastFindHorse != null){
//						lastFindHorse.GetComponent<HighLightHorse> ().MakeHighLight (false);
//					}
//					lastFindHorse = horse;
//				}
//				FindHorse.GetComponent<HighLightHorse> ().MakeHighLight (true);
//			}
        }
	}

	public void Jump()
    {
        if (!m_jumping)
        {
            m_jumping = true;
            circle.GetComponent<drawcircle> ().draw(jumpCount);
            jumpCount ++;
			Time.timeScale = 0.5f;
            anim.SetBool(JumpHash,true);

			Vector3 force = transform.up * 8 + horse.transform.forward * 12;
			LeaveHorse (force);
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Terrain"))
        {
            Debug.Log("OnCollisionEnter  " + collisionInfo.gameObject.tag);
            if (!dead)
            {
				Die ();
            }
		}
    }

	public void RideHorse(RDHorseController horse, bool run)
    {
		ResetCircle ();
		if (horse != null)
        {
            m_jumping = false;
			anim.SetBool(JumpHash,false);
			Time.timeScale = 1;
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
			rigidBody.detectCollisions = false;  //屏蔽rigidbody
			transform.SetParent (horse.riderParent.transform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.Euler(new Vector3(0, -90, -90));
			if (this.horse) {
				this.horse.GetComponent<RDHorseController> ().onCowboyLeave ();
			}
			this.horse = horse.gameObject;
			horse.onCowboyRide (this);
			if (run){
				horse.Run  = true;
			}
        }
    }

	public void LeaveHorse(Vector3 force){
		transform.parent = null;
		rigidBody.useGravity = true;
		rigidBody.isKinematic = false;
		rigidBody.detectCollisions = true;
		Vector3 v = transform.forward.normalized;
		transform.rotation = horse.transform.rotation;
		rigidBody.AddForce(force, ForceMode.Impulse);
		horse.GetComponent<RDHorseController> ().onCowboyLeave ();
		Destroy(horse);
		horse = null;
	}

	public GameObject searchHrose(){
		float radio = circle.GetComponent<drawcircle> ().radius;
		Collider[] colliders = Physics.OverlapSphere(circle.transform.position, radio);
		for (int i = 1; i< colliders.Length; i++){
			if (colliders[i].gameObject.CompareTag("Horse")){
				return colliders [i].gameObject;
			}
		}
		return null;
	}

	void ResetCircle(){
		circle.transform.position = new Vector3(0,0,-100);
	}

	void OnGameStart(){
		FallDowning = false;
		if (horse != null){
			horse.GetComponent<RDHorseController> ().Run = true;
		}
		anim.SetFloat(RideSpeedHash, 1.0f);
		Camera.main.GetComponent<fixedCam> ().enabled = true;
		Vector3 pos = horse.transform.position;
		pos.y += 0.5f;
		Instantiate (Smoke, pos, Smoke.transform.rotation);
	}

	private void onFallDown(){
		FallDowning = true;
		Vector3 orgin = horse.transform.position;
		orgin.y = 0;
		horse.transform.position = new Vector3 (orgin.x, 15, orgin.z);
		Tween tween = horse.transform.DOMove (orgin, 0.7f);
		tween.SetEase (Ease.InQuart);
		tween.OnComplete (OnGameStart);
	}

	//the horse contact the blocks
	public void OnHorseBlock(){
		m_jumping = false;
		dead = true;
	}

	private void Die(){
		dead = true;
		m_jumping = true;
		anim.SetBool(DeadHash, dead);
		ResetCircle ();
		GameObject smoke = Instantiate (Smoke);
		Vector3 pos = gameObject.transform.position;
		pos.y += 0.5f;
		smoke.transform.position = pos;
	}
}
