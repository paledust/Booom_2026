using UnityEngine;

[CreateAssetMenu(fileName = "PhotoData", menuName = "Game/Photo/PhotoData")]
public class PhotoDataSingle : PhotoData
{
    [SerializeField] private GameObject photoPrefab;
    public override GameObject GetPhoto() => photoPrefab;
}