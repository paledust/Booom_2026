using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum EdgeType
{
    None = 0,
    Top = 1<<1,
    Bottom = 1<<2,
    Left = 1<<3,
    Right = 1<<4
}
public class EdgeHandler : MonoBehaviour
{
    [SerializeField] private GameObject edgePrefab;
    private EdgeType activeEdgeTypes;
    private Rect activeRect;
    private Dictionary<EdgeType, GameObject> dictEdge;
    public bool m_hasEdge => activeEdgeTypes != EdgeType.None;
    
    public Vector2 GetEdgeCorrectPoint(Vector2 startPoint, Vector2 endPoint, out EdgeType alignEdge)
    {
        //起始点在内部，直接返回
        if(activeRect.Contains(startPoint))
        {
            alignEdge = EdgeType.None;
            return endPoint;
        }
        
        Vector2 min = new Vector2(Mathf.Min(startPoint.x, endPoint.x), Mathf.Min(startPoint.y, endPoint.y));
        Vector2 max = new Vector2(Mathf.Max(startPoint.x, endPoint.x), Mathf.Max(startPoint.y, endPoint.y));
        var rect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        if(!activeRect.Overlaps(rect))
        {
            alignEdge = EdgeType.None;
            return endPoint;
        }

        Vector2 dir = endPoint - startPoint;
        if(startPoint.x > activeRect.xMin && startPoint.x < activeRect.xMax)
        {
            if(startPoint.y > activeRect.yMax && (activeEdgeTypes & EdgeType.Top)!=0)
                return AlignToTop(endPoint, out alignEdge);
            if(startPoint.y < activeRect.yMin && (activeEdgeTypes & EdgeType.Bottom)!=0)
                return AlignToBottom(endPoint, out alignEdge);
        }
        if(startPoint.y > activeRect.yMin && startPoint.y < activeRect.yMax)
        {
            if(startPoint.x > activeRect.xMax && (activeEdgeTypes & EdgeType.Right)!=0)
                return AlignToRight(endPoint, out alignEdge);
            if(startPoint.x < activeRect.xMin && (activeEdgeTypes & EdgeType.Left)!=0)
                return AlignToLeft(endPoint, out alignEdge);
        }
        //左上
        if(startPoint.x < activeRect.xMin && startPoint.y > activeRect.yMax)
        {
            Vector2 corner = new Vector2(activeRect.xMin, activeRect.yMax);
            Vector2 diff = endPoint - corner;
            if(Mathf.Abs(diff.y)>Mathf.Abs(diff.x))
                return AlignToLeft(endPoint, out alignEdge);
            else
                return AlignToTop(endPoint, out alignEdge);
        }
        //右下
        if(startPoint.x > activeRect.xMax && startPoint.y < activeRect.yMin)
        {
            Vector2 corner = new Vector2(activeRect.xMax, activeRect.yMin);
            Vector2 diff = endPoint - corner;
            if(Mathf.Abs(diff.y)>Mathf.Abs(diff.x))
                return AlignToRight(endPoint, out alignEdge);
            else
                return AlignToBottom(endPoint, out alignEdge);
        }
        //左下
        if(startPoint.x < activeRect.xMin && startPoint.y < activeRect.yMin)
        {
            Vector2 corner = new Vector2(activeRect.xMin, activeRect.yMin);
            Vector2 diff = endPoint - corner;
            if(Mathf.Abs(diff.y)>Mathf.Abs(diff.x))
                return AlignToLeft(endPoint, out alignEdge);
            else
                return AlignToBottom(endPoint, out alignEdge);
        }
        //右上
        if(startPoint.x > activeRect.xMax && startPoint.y > activeRect.yMax)
        {
            Vector2 corner = new Vector2(activeRect.xMin, activeRect.yMax);
            Vector2 diff = endPoint - corner;
            if(Mathf.Abs(diff.y)>Mathf.Abs(diff.x))
                return AlignToRight(endPoint, out alignEdge);
            else
                return AlignToTop(endPoint, out alignEdge);
        }
        alignEdge = EdgeType.None;
        return endPoint;
    }
    public void CompleteEdge(EdgeType edgeType)
    {
        activeEdgeTypes = activeEdgeTypes &~ edgeType;
        if(dictEdge.TryGetValue(edgeType, out var edgeObj))
        {
            Destroy(edgeObj);
            dictEdge.Remove(edgeType);
        }
    }
    Vector2 AlignToTop(Vector2 point, out EdgeType alignEdge)
    {
        Vector2 intersect = point + Vector2.up * (activeRect.yMax - point.y);
        alignEdge = EdgeType.Top;
        return intersect;
    }
    Vector2 AlignToBottom(Vector2 point, out EdgeType alignEdge)
    {
        Vector2 intersect = point + Vector2.down * (point.y - activeRect.yMin);
        alignEdge = EdgeType.Bottom;

        return intersect;
    }
    Vector2 AlignToLeft(Vector2 point, out EdgeType alignEdge)
    {
        Vector2 intersect = point + Vector2.left * (point.x - activeRect.xMin);
        alignEdge = EdgeType.Left;
        return intersect;
    }
    Vector2 AlignToRight(Vector2 point, out EdgeType alignEdge)
    {
        Vector2 intersect = point + Vector2.right * (activeRect.xMax - point.x);
        alignEdge = EdgeType.Right;
        return intersect;
    }
    public void CreateConstraintRect(Rect rect, EdgeType edgeType)
    {
        activeRect = rect;
        activeEdgeTypes = edgeType;
        DrawEdge(rect, EdgeType.Top);
        DrawEdge(rect, EdgeType.Bottom);
        DrawEdge(rect, EdgeType.Left);
        DrawEdge(rect, EdgeType.Right);
    }
    public void DrawEdge(Rect rect, EdgeType edgeType)
    {
        if(dictEdge == null)
            dictEdge = new Dictionary<EdgeType, GameObject>();
        if((activeEdgeTypes & edgeType)!=0)
        {
            Vector2 start, end;
            switch(edgeType)
            {
                case EdgeType.Top:
                    start = rect.min + Vector2.up * rect.height;
                    end = rect.max;
                    break;
                case EdgeType.Bottom:
                    start = rect.min;
                    end = rect.max + Vector2.down * rect.height;
                    break;
                case EdgeType.Left:
                    start = rect.min;
                    end = rect.min + Vector2.up * rect.height;
                    break;
                case EdgeType.Right:
                    start = rect.max + Vector2.down * rect.height;
                    end = rect.max;
                    break;
                default:
                    start = Vector2.zero;
                    end = Vector2.right;
                    break;
            }
            var edgeObj = Instantiate(edgePrefab);
            edgeObj.transform.localScale = new Vector3(Vector2.Distance(start, end), edgeObj.transform.localScale.y, 0);
            edgeObj.transform.rotation = Quaternion.FromToRotation(Vector3.right, end - start);
            edgeObj.transform.position = (start + end) * 0.5f;
            dictEdge.Add(edgeType, edgeObj);
        }
    }
}
