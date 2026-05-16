using System;
using UnityEngine;

public class InterestPointBasic : MonoBehaviour
{
    [SerializeField] private Sprite photo;
    [SerializeField] private Collider2D hitbox;
    [SerializeField] private EdgeType edgeType;
    public EdgeType m_edgeType => edgeType;

    public void RevealPoint()
    {
        hitbox.enabled = false;
    }
    public void HidePoint()
    {
        hitbox.enabled = true;
    }
    public Sprite GetNextPhoto()
    {
        Destroy(gameObject);
        return photo;
    }
}