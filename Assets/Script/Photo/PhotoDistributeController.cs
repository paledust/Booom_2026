using System.Collections.Generic;
using UnityEngine;

public class PhotoDistributeController : MonoBehaviour
{
    private Dictionary<string, PhotoDistributor> dictDistributor;
    void Awake()
    {
        dictDistributor = new Dictionary<string, PhotoDistributor>();
    }
    public PhotoDistributor GetPhotoDistributor(PhotoDistributorData data)
    {
        if(!dictDistributor.TryGetValue(data.name, out var distributor))
        {
            distributor = data.GetPhotoInstance();
            dictDistributor.Add(data.name, distributor);
        }
        return distributor;
    }
}