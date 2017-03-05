using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using XLua;

public class UpdateMgr : MonoBehaviour {
	public GameObject UpdatePanel;
	public GameObject Pop;
	public Animator animator;
	public Text text;

	private static LuaEnv luaenv = new LuaEnv();
	private static readonly string CDN = "http://stzb321.github.io/AssetUpdate/CowBoyRun/ios/V1/";
	private static readonly string LuaAssetURL = CDN + "AssetBundles";
	private static int Version = 1;

	// Use this for initialization
	void Start () {
		if (!RideRunGameData.Instance ().IsUpdated) {
			Pop.SetActive (true);
		}else{
			UpdatePanel.SetActive (false);
		}
	}

	public void StartDownLoad(){
		if (!RideRunGameData.Instance().IsUpdated){
			animator.SetBool (Animator.StringToHash ("download"), true);
			text.text = "下载中。。。";
			StartCoroutine(LoadLuaAssets());
			RideRunGameData.Instance ().IsUpdated = true;
		}
	}

	public void Close(){
		UpdatePanel.SetActive (false);
	}

	IEnumerator LoadLuaAssets(){
		bool succ = false;
		WWW mwww = WWW.LoadFromCacheOrDownload (LuaAssetURL, Version);
		Debug.Log (LuaAssetURL);
		yield return mwww;
		if( string.IsNullOrEmpty( mwww.error )){
			AssetBundle ab = mwww.assetBundle;
			AssetBundleManifest manifest = ab.LoadAsset ("AssetBundleManifest") as AssetBundleManifest;

			// get all depends
			string[] depends = manifest.GetAllDependencies("luaasset");
			AssetBundle[] dependAssets = new AssetBundle[depends.Length];

			for (int i = 0; i < depends.Length; i++) {
				// load all depends...
				WWW dwww = WWW.LoadFromCacheOrDownload (CDN + depends [i], Version);
				yield return dwww;
				dependAssets [i] = dwww.assetBundle;
			}

			WWW luawww = WWW.LoadFromCacheOrDownload(CDN + "luaasset", Version);
			Debug.Log (CDN + "luaasset");
			yield return luawww;
			if (string.IsNullOrEmpty(luawww.error)){
				AssetBundle Luaasset = luawww.assetBundle;
				string[] names = Luaasset.GetAllAssetNames ();
				for (int i = 0; i < names.Length; i++) {
					TextAsset luatxt = Luaasset.LoadAsset (names[i]) as TextAsset;
					if (luatxt != null) {
						// finally, we do the lua string...
						Debug.Log(luatxt.text);
						luaenv.DoString (luatxt.text);
					}
				}
				succ = true;
				Luaasset.Unload (false);
			}else{
				Debug.Log (luawww.error);
			}

			// unload the depend assets
			for (int i = 0; i < dependAssets.Length; i++) {
				dependAssets [i].Unload (false);
			}

			ab.Unload (false);
		}else{
			Debug.Log (mwww.error);
		}

		if(succ){
			text.text = "更新安装成功！";
		}else{
			text.text = "更新下载失败。";
		}
	}
}
