using UnityEngine;
using System.Collections;

public class AssetHelper : MonoBehaviour {

	public static GameObject Tile;
	private static AssetBundle TileAsset;

	public static void LoadAssetBundle(){
		ReadTile ();
	}

	static void ReadTile(){
		TileAsset = AssetBundle.LoadFromFile (Application.streamingAssetsPath + "/terrain.tile");
		Tile = TileAsset.LoadAsset ("tile") as GameObject;
		Debug.Log ("aaa");
	}

	public static void UnloadAssets(){
		TileAsset.Unload (true);
		Resources.UnloadUnusedAssets ();
	}
}
