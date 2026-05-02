using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererOpacity : PerRendererBehavior
{
    [Range(0, 1f)] public float opacity = 1;
    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(OPACITY_ID, opacity);
    }
}
