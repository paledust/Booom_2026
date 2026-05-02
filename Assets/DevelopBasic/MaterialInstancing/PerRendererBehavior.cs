using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class PerRendererBehavior : MonoBehaviour
{
    [SerializeField] protected bool isUpdating = true;
    [SerializeField] protected Renderer meshRenderer;

    public Renderer m_Renderer{get{return meshRenderer;}}

#region CommonShaderProperties
    protected static readonly int COLOR_ID = Shader.PropertyToID("_Color");
    protected static readonly int OPACITY_ID = Shader.PropertyToID("_Opacity");
	protected static readonly int STENCIL_COMP_PARAM_ID = Shader.PropertyToID("_StencilComp");
	protected const UnityEngine.Rendering.CompareFunction STENCIL_COMP_MASKINTERACTION_NONE = UnityEngine.Rendering.CompareFunction.Always;
	protected const UnityEngine.Rendering.CompareFunction STENCIL_COMP_MASKINTERACTION_VISIBLE_INSIDE = UnityEngine.Rendering.CompareFunction.LessEqual;
	protected const UnityEngine.Rendering.CompareFunction STENCIL_COMP_MASKINTERACTION_VISIBLE_OUTSIDE = UnityEngine.Rendering.CompareFunction.Greater;
#endregion

    protected MaterialPropertyBlock mpb;
    void Reset(){
        meshRenderer = GetComponent<Renderer>();
    }
    void Start(){
		mpb = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(mpb);
        InitProperties();
        UpdateProperties();
        meshRenderer.SetPropertyBlock(mpb);
    }
    void OnEnable()=>Start();
    void Update()
    {
    //Enable Editing in editor whenever it's not playing
    #if UNITY_EDITOR
        if(!UnityEditor.EditorApplication.isPlaying){
            meshRenderer.GetPropertyBlock(mpb);
            UpdateProperties();
            meshRenderer.SetPropertyBlock(mpb);
        }
        else if(isUpdating){
            meshRenderer.GetPropertyBlock(mpb);
            UpdateProperties();
            meshRenderer.SetPropertyBlock(mpb);
        }
    #else
        if(isUpdating){
            meshRenderer.GetPropertyBlock(mpb);
            UpdateProperties();
            meshRenderer.SetPropertyBlock(mpb);
        }
    #endif
    }
    protected virtual void UpdateProperties(){}//Excuted on initiation and Updating.
    protected virtual void InitProperties(){}//Only excuted on initation.
}