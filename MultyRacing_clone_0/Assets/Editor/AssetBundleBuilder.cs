#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    [MenuItem("Tools/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        string bundleDir = "Assets/AssetBundles";
        if (!System.IO.Directory.Exists(bundleDir))
            System.IO.Directory.CreateDirectory(bundleDir);

        BuildPipeline.BuildAssetBundles(bundleDir,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows64); 
    }
}
#endif