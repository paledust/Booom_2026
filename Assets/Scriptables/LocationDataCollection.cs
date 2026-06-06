using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "LocationDataCollection", menuName = "Game/Location/LocationDataCollection")]
public class LocationDataCollection : ScriptableObject
{
    public List<LocationData> allLocations;
#if UNITY_EDITOR
    [Button("Data Validation")]
    public void DataValidation()
    {
        string path = AssetDatabase.GetAssetPath(this);
        path = Path.GetDirectoryName(path);
        allLocations = FindAllAssetsOfAllSubFolders<LocationData>(path);
        foreach(var location in allLocations)
        {
            location.Validate();
        }
        EditorUtility.SetDirty(this);
    }
    public static List<T> FindAllAssetsOfAllSubFolders<T>(string folderPath, List<string> excludedFolders = null) where T : ScriptableObject
    {
        List<T> foundAssets = new List<T>();
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("文件夹未找到: " + folderPath);
            return foundAssets;
        }

        // 包含根目录和子文件夹中的文件
        GetAssetFromTheFolder(folderPath, foundAssets);

        // 使用 SearchOption.AllDirectories 来包含所有子文件夹
        string[] subfolders = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);
        foreach (var subfolder in subfolders)
        {
            // 如果子文件夹在排除列表中，跳过
            if (excludedFolders != null && excludedFolders.Exists(excluded => subfolder.Contains(excluded)))
            {
                continue;
            }

            GetAssetFromTheFolder(subfolder, foundAssets);
        }

        return foundAssets;
    }
    private static void GetAssetFromTheFolder<T>(string folderPath, List<T> foundAssets) where T : ScriptableObject
    {
        foreach (var file in Directory.GetFiles(folderPath, "*.asset"))
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(file.Replace("\\", "/").Replace(Application.dataPath, "Assets"));
            if (asset != null) foundAssets.Add(asset);
        }
    }
#endif
}