using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PhotoDataStack", menuName = "Game/Photo/PhotoDataStack")]
public class PhotoDataStack : PhotoData
{
    [SerializeField, InfoBox("能够显示一系列的照片")]  private GameObject[] photoStack;
    [SerializeField] private bool isRandomOrder = false;
    public override GameObject GetPhoto()
    {
        throw new System.NotImplementedException();
    }
}
