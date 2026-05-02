using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PerRendererColor : PerRendererBehavior
{
    public Color tint = Color.white;
    [SerializeField] protected string ColorName = "_Color";
    protected override void UpdateProperties()
    {
        mpb.SetColor(ColorName, tint);
    }
}
