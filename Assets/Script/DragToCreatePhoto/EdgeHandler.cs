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
    public struct EdgeLine
    {
        public readonly Vector2 start;
        public readonly Vector2 end;
        private readonly EdgeType edgeType;
        public EdgeLine(Vector2 start, Vector2 end, EdgeType edgeType)
        {
            this.start = start;
            this.end = end;
            this.edgeType = edgeType;
        }
        public Vector2 PointCorrection(Vector2 startPoint, Vector2 endPoint)
        {
            switch(edgeType)
            {
                case EdgeType.Top:
                    if(startPoint.y < start.y)
                        return endPoint;
                    else
                        return EdgeCorrect(startPoint, endPoint);
                case EdgeType.Bottom:
                    if(startPoint.y > start.y)
                        return endPoint;
                    else
                        return EdgeCorrect(startPoint, endPoint);
                case EdgeType.Left:
                    if(startPoint.x > start.x)
                        return endPoint;
                    else
                        return EdgeCorrect(startPoint, endPoint);
                case EdgeType.Right:
                    if(startPoint.x < start.x)
                        return endPoint;
                    else
                        return EdgeCorrect(startPoint, endPoint);
                default:
                    return endPoint;
            }
        }
        private Vector2 EdgeCorrect(Vector2 startPoint, Vector2 endPoint)
        {
            bool isVertical = edgeType == EdgeType.Left || edgeType == EdgeType.Right;
            if(isVertical)
            {
                if((startPoint.y - start.y) * (endPoint.y - start.y) > 0 && 
                    (startPoint.y - end.y) * (endPoint.y - end.y) > 0 &&
                    (start.y - startPoint.y) * (end.y - startPoint.y) > 0)
                    return endPoint;
                else
                    switch(edgeType)
                    {
                        case EdgeType.Left:
                            endPoint.x = Mathf.Min(endPoint.x, start.x);
                            return endPoint;
                        case EdgeType.Right:
                            endPoint.x = Mathf.Max(endPoint.x, start.x);
                            return endPoint;
                        default:
                            return endPoint;
                    }
            }
            else
            {
                if((startPoint.x - start.x) * (endPoint.x - start.x) > 0 && 
                    (startPoint.x - end.x) * (endPoint.x - end.x) > 0 &&
                    (start.x - startPoint.x) * (end.x - startPoint.x) > 0)
                    return endPoint;
                else
                    switch(edgeType)
                    {
                        case EdgeType.Top:
                            endPoint.y = Mathf.Max(endPoint.y, start.y);
                            return endPoint;
                        case EdgeType.Bottom:
                            endPoint.y = Mathf.Min(endPoint.y, start.y);
                            return endPoint;
                        default:
                            return endPoint;
                    }
            }
        }
    }
    [SerializeField] private GameObject edgePrefab;
    private List<EdgeLine> currentEdges = new List<EdgeLine>();
    public bool m_hasEdge => currentEdges!=null && currentEdges.Count > 0;

    public Vector2 GetEdgeCorrectPoint(Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 maxPoint = endPoint;
        foreach(var edge in currentEdges)
        {
            maxPoint = edge.PointCorrection(startPoint, maxPoint);
        }
        return maxPoint;
    }
    public EdgeLine AddEdgeToRect(Rect rect, EdgeType edgeType)
    {
        EdgeLine edge;
        switch(edgeType)
        {
            case EdgeType.Top:
                edge = new EdgeLine(rect.min + Vector2.up * rect.height, rect.max, EdgeType.Top);
                break;
            case EdgeType.Bottom:
                edge = new EdgeLine(rect.min, rect.max + Vector2.down * rect.height, EdgeType.Bottom);
                break;
            case EdgeType.Left:
                edge = new EdgeLine(rect.min, rect.min + Vector2.up * rect.height, EdgeType.Left);
                break;
            case EdgeType.Right:
                edge = new EdgeLine(rect.max + Vector2.down * rect.height, rect.max, EdgeType.Right);
                break;
            default:
                edge = new EdgeLine(Vector2.zero, Vector2.right, EdgeType.None);
                break;
        }
        var edgeObj = Instantiate(edgePrefab);
        edgeObj.transform.localScale = new Vector3(Vector2.Distance(edge.start, edge.end), edgeObj.transform.localScale.y, 0);
        edgeObj.transform.rotation = Quaternion.FromToRotation(Vector3.right, edge.end - edge.start);
        edgeObj.transform.position = (edge.start + edge.end) * 0.5f;
        currentEdges.Add(edge);
        return edge;
    }
}
