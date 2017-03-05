using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoTerrain : MonoBehaviour {

	public GameObject Tile;
	public GameObject[] Obstacles; //障碍物
	public int SignDistance; //距离提示
	public GameObject sign;
	public GameObject ground;

	private bool Trigger = false; //every tile trigger one time;

	//TODO make this propertity get set...
	public int TileIndex;
	public int SetileIndex {
		set {
			TileIndex = value;
			ResetPosition ();
		}
		get{
			return TileIndex;
		}
	}


	// Use this for initialization
	void Start () {
//		GenereObstacle ();
//		GenereSign ();
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player")){
			bool IsCowBoy = false;
			if (other.gameObject.GetComponent<RiderController> () != null){
				IsCowBoy = true;
			}

			RDHorseController rd = other.gameObject.GetComponent<RDHorseController> ();
			if (IsCowBoy || (!Trigger && rd != null && rd.BeenRide)){
				// should add a tile... 
				//TODO: use a objectPool
				Trigger = !Trigger;
				GameObject newTile = Instantiate(Tile);
				newTile.transform.SetParent (gameObject.transform.parent.transform);
				newTile.GetComponent<AutoTerrain> ().SetileIndex = ++TileIndex;
			}
		}
	}

	void ResetPosition(){
		var z = ground.GetComponent<MeshFilter> ().mesh.bounds.size.z * transform.localScale.z * TileIndex;
		transform.localPosition = new Vector3 (0,0, z);
	}

	void GenereObstacle(){
//		if(TileIndex == 0){
//			return;
//		}
//		int count = Random.Range (1, 5);
//		for (int i=0;i<count;i++){
//			int index = Random.Range (0, Obstacles.Length);
//			GameObject go = Instantiate (Obstacles [index]);
//			go.transform.SetParent (gameObject.transform);
//			go.transform.localPosition = new Vector3 ( Random.Range(TileW/3,2*TileW/3), 0, Random.Range(0,TileH));
//		}
	}

	void GenereSign(){
//		float low = Mathf.Floor(TileIndex * TileH / SignDistance);
//		float high = Mathf.Floor((TileIndex + 1) * TileH / SignDistance);
//
//		if ( high - low >= 1 ){
//			GameObject signTxt = Instantiate (sign,transform) as GameObject;
//			string str = SignDistance * TileIndex + "m";
//			str += "  " + str;
//			str += "  " + str;
//			signTxt.GetComponent<TextMesh>().text = str;
//			Vector3 pos = signTxt.transform.position;
//			pos.z = SignDistance * TileIndex;
//			sign.transform.position = pos;
//		}
	}
}
