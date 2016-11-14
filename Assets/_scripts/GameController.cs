using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public GameObject player;
	public GameObject[] instanableHorse;

	// Use this for initialization
	void Start () {
		GameObject horse =  AddHorse (new Vector3(20,0,20));
		player.GetComponent<RiderController> ().RideHorse (horse);
	}

	void Awake(){
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject AddHorse(Vector3 pos){
		if (pos.Equals(Vector3.zero)) {
			pos = new Vector3 (2,0,2) + player.transform.position;
		}
		System.Random r = new System.Random ();
		int t = r.Next (0, instanableHorse.Length);
		GameObject newHorse = Instantiate (instanableHorse [t], pos, Quaternion.identity) as GameObject;
		RideRunGameData.Instance ().AddHorse (newHorse);
		return newHorse;
	}
}
