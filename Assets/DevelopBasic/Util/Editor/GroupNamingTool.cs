using UnityEngine;
using UnityEditor;

public class GroupNamingTool : EditorWindow
{
    protected const string NAME_TAG = "<name>";
    protected const string NUM_TAG = "<num>";
    protected string Naming = "<name>";
    protected int StartIndex = 0;
    [MenuItem("Tools/Others/Group Naming Tool")]
    public static void ShowWindow(){
        GetWindow<GroupNamingTool>("Group Naming Tool");
    }
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Group Name");
            Naming = GUILayout.TextField(Naming);
            StartIndex = EditorGUILayout.IntField("StartIndex",StartIndex);
        }
        GUILayout.EndHorizontal();
        if(GUILayout.Button("Renaming")){
            GameObject[] objects = Selection.gameObjects;
            if(objects==null || objects.Length==0){
                Debug.Log("No Objects Selected");
                return;
            }
            int minIndex = objects[0].transform.GetSiblingIndex();
            for(int i=0; i<objects.Length; i++){
                if(minIndex > objects[i].transform.GetSiblingIndex()){
                    minIndex = objects[i].transform.GetSiblingIndex();
                }
            }
            
            Undo.RecordObjects(objects, "Change Name");
            for(int i=0; i<objects.Length; i++){
                string _objName = Naming;
                _objName = _objName.Replace(NAME_TAG, objects[i].name);
                _objName = _objName.Replace(NUM_TAG, (objects[i].transform.GetSiblingIndex()-minIndex+StartIndex).ToString());
                objects[i].name = _objName;

                EditorUtility.SetDirty(objects[i]);
            }
        }
    }
}
