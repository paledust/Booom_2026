using UnityEditor;
using UnityEngine;
using System.IO;

public class FileNamingTool : EditorWindow
{
    protected const string NAME_TAG = "<name>";
    protected const string NUM_TAG = "<num>";

    private bool checkReplace = false;
    private string replaceTag = "";
    private string targetTag = "";
    private string naming = "NewName_";
    private int startIndex = 0;

    [MenuItem("Tools/Others/File Rename Tool")]
    public static void ShowWindow()
    {
        GetWindow<FileNamingTool>("File Rename Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Rename Selected Assets", EditorStyles.boldLabel);
        EditorGUILayout.TextArea("Use Cheat sheet:\n"+
        "1.<name> - 文件本身名字\n" +
        "2.<num> - 文件索引编号\n"+
        "e.g: pic_<num>会按照所有选取的文件按照顺序命名为pic_1,pic_2,pic_3....\n"+
        "e.g: unit_<name>会将所有选取的文件添加unit前缀");

        naming = EditorGUILayout.TextField("Prefix:", naming);
        startIndex = EditorGUILayout.IntField("Start Index:", startIndex);
        EditorGUILayout.Space(20);
        checkReplace = EditorGUILayout.Toggle("Replace String", checkReplace);
        targetTag = EditorGUILayout.TextField("Target Tag:", targetTag);
        replaceTag = EditorGUILayout.TextField("Replace Tag:", replaceTag);

        if (GUILayout.Button("Rename Assets"))
        {
            Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

            for (int i = 0; i < selectedObjects.Length; i++)
            {
                Object obj = selectedObjects[i];
                string oldPath = AssetDatabase.GetAssetPath(obj);
                string fileName = Path.GetFileNameWithoutExtension(oldPath);
                string extension = Path.GetExtension(oldPath);
                Debug.Log(fileName);
                // Construct the new name
                string newName = $"{naming}{extension}";
                newName = newName.Replace(NAME_TAG, fileName);
                newName = newName.Replace(NUM_TAG, (startIndex + i).ToString());

                if (checkReplace)
                {
                    newName = newName.Replace(targetTag, replaceTag);
                }

                // Rename the asset
                AssetDatabase.RenameAsset(oldPath, newName);
            }
            AssetDatabase.Refresh(); // Refresh the Asset Database to reflect changes
        }
    }
}