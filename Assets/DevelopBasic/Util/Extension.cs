using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Extension
public static class ExtensionMethods{
    public static Vector3 GetPositionOnPlaneFromScreenPoint(this Camera camera, Vector3 screenPoint, Vector3 planePoint, Vector3 planeNormal){
        Ray ray = camera.ScreenPointToRay(screenPoint);

        if(Vector3.Dot(ray.direction, planeNormal) == 0) return Vector3.zero;

        Vector3 offset = planePoint - ray.origin;
        float height = Vector3.Dot(offset, planeNormal);
        float scale  = height/Vector3.Dot(ray.direction, planeNormal);
        Vector3 planeOffset = ray.direction*scale - offset;
        
        return planeOffset + planePoint;
    }
    public static float GetRndValueInVector2Range(this Vector2 range){return Random.Range(range.x, range.y);}
    public static int GetRndValueInVector2Range(this Vector2Int range){return Random.Range(range.x, range.y);}
}
#endregion
