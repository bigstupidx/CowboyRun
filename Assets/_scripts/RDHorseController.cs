using UnityEngine;
using System.Collections;

public class RDHorseController : MonoBehaviour {

    public GameObject riderParent;
	public int NeighTime = 10;
	public SkinnedMeshRenderer[] skins;
	public GameObject HorseCanvasPrefab;
	[HideInInspector]public drawcircle circle;

    private Animator anim;
    private int RideSpeedHash = Animator.StringToHash("RideSpeed");
	private int DeadHash = Animator.StringToHash("dead");
	private int NeighHash = Animator.StringToHash("Neigh");
	private bool beenRide = false;
	private bool run = false;
	private RiderController riderController;
	private HorseMovement hm;
	private bool Angleing = false;
	private bool beenFound = false;
	private float AngryTime = 3.5f;

	private GameObject horseCanvas;
	private Animator horseAnimator;

	[HideInInspector]
	public bool BeenRide{
		get{
			return beenRide;
		}
	}

	[HideInInspector]
	public bool Run{
		get{
			return run;
		}
		set{
			run = value;
			anim.SetFloat (RideSpeedHash, run ? 1.0f : 0.0f);
		}
	}

	// Use this for initialization
	void Start () {
		
	}

	void LateUpdate(){
		if (run && beenRide && !Angleing){
			Angleing = !Angleing;
			StartCoroutine (Angry ());
			StartCoroutine (AngryEffect ());
		}
		if (CompareTag("Horse")){
			float radio = circle.radius;
			bool flag = false;
			if (  Vector3.Distance(transform.position, circle.gameObject.transform.position) <= radio ){
				flag = true;
			}
			BeenFound(flag);
		}
	}

	void Awake(){
		anim = GetComponent<Animator>();
		hm = GetComponent<HorseMovement>();
	}

	public void onCowboyRide(RiderController rc){
		beenRide = true;
		gameObject.tag = "Player";
		riderController = rc;
		hm.OnBeenRide ();
		BeenFound (false);
	}


	public void onCowboyLeave(){
		beenRide = false;
		gameObject.tag = "Horse";
		riderController = null;
		hm.OnNotBeenRide ();
	}

	void OnCollisionEnter(Collision collision) {
		// should die
		if (beenRide){
			Debug.Log (collision.gameObject.tag);
			if (riderController != null && gameObject.CompareTag("Player")){
				anim.SetBool (DeadHash, true);
				run = false;
				riderController.OnHorseBlock();
				GetComponent<Rigidbody> ().velocity = Vector3.zero;
			}
		}
	}

	IEnumerator Angry(){
		yield return new WaitForSeconds (NeighTime);
		if (riderController != null){
			run = false;
			anim.SetBool (NeighHash, true);
			StartCoroutine (TimeToJump ());
		}
	}

	IEnumerator AngryEffect(){
		yield return new WaitForSeconds (NeighTime - AngryTime);
		if (riderController != null){
			AddHorseCanvas ();
			horseAnimator.SetBool (Animator.StringToHash ("found"), false);
			horseAnimator.SetBool (Animator.StringToHash ("neigh"), true);
		}
	}

	IEnumerator TimeToJump(){
		horseAnimator.SetBool (Animator.StringToHash ("neigh"), false);
		yield return new WaitForSeconds(1.5f);
		if (riderController != null && !riderController.Dead){
			riderController.Jump ();
		}
	}

	void BeenFound(bool flag){
		if (beenFound == flag)
			return;
		beenFound = flag;
		//if (beenFound){
			AddHorseCanvas ();
			horseAnimator.SetBool (Animator.StringToHash ("found"), flag);
		//}
	}

	void AddHorseCanvas(){
		if(horseCanvas == null){
			GameObject canvas = Instantiate (HorseCanvasPrefab);
			horseCanvas = canvas;
			horseAnimator = canvas.GetComponent<Animator> ();
			canvas.transform.SetParent (gameObject.transform);
			canvas.transform.localPosition = Vector3.zero;
		}
	}
}
