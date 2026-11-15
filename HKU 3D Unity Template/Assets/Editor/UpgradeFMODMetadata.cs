using UnityEditor;
using UnityEngine;

public static class UpgradeFMODMetadata
{
    [MenuItem("Tools/Upgrade FMOD Metadata")]
    public static void Upgrade()
    {
        AssetDatabase.ForceReserializeAssets(new[] { "Assets/Plugins/FMOD" }, ForceReserializeAssetsOptions.ReserializeMetadata);

        Debug.Log("FMOD metadata upgrade completed.");
    }
}