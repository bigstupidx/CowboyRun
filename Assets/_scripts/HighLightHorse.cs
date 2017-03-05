using UnityEngine;
using System.Collections;

public class HighLightHorse : MonoBehaviour {
	public Material[] origiMat;
	public Material[] highLightMat;
	public GameObject[] highLightGo;

	private bool HighLighing = false;

	public void MakeHighLight(bool make){
		if (HighLighing == make){
			return;
		}
		HighLighing = make;
		Material[] replaceMat = make ? highLightMat : origiMat;
		for (int i = 0 ; i < highLightGo.Length; i++){
			GameObject go = highLightGo [i];
			go.GetComponent<SkinnedMeshRenderer> ().material = replaceMat [i];
		}
	}


}
