using UnityEngine;
using System.Collections;
using UnityEditor;

public class BuildAsset {

	[MenuItem ("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles ()
	{
		BuildPipeline.BuildAssetBundles ("Assets/AssetBundles",BuildAssetBundleOptions.ChunkBasedCompression,BuildTarget.iOS);
	}
}
