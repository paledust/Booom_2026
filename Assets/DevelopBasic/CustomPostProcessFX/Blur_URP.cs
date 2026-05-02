using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNIVERSAL_PIPELINE_CORE_INCLUDED 
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenuForRenderPipeline("Custom/Blur_URP", typeof(UniversalRenderPipeline))]
public class Blur_URP : VolumeComponent, IPostProcessComponent
{
    [Range(0, 0.1f)] public FloatParameter blurAmount = new FloatParameter(value : 0f);
    [Range(15,100)] public NoInterpIntParameter sampleAmount = new NoInterpIntParameter(value : 15);
    public bool IsActive()=>blurAmount.value > 0 && sampleAmount.value > 0;
    public bool IsTileCompatible() => true;
}

[System.Serializable]
public class Blur_URP_Pass : ScriptableRenderPass
{
    RenderTargetIdentifier src;
    RenderTargetIdentifier tempID_V;
    RenderTargetIdentifier tempID_H;

    private Material material;
    private int BlurRT_ID_V = Shader.PropertyToID("Blur_RT_Vertical");
    private int BlurRT_ID_H = Shader.PropertyToID("Blur_RT_Horizontal");
    public Blur_URP_Pass(){
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData){
        material = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Custom/Blur_URP"));

        src = renderingData.cameraData.renderer.cameraColorTarget;
        
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        cmd.GetTemporaryRT(BlurRT_ID_V, descriptor.width>>1, descriptor.height>>1, 0, FilterMode.Bilinear, descriptor.colorFormat);
        tempID_V = new RenderTargetIdentifier(BlurRT_ID_V);
        cmd.GetTemporaryRT(BlurRT_ID_H, descriptor.width>>1, descriptor.height>>1, 0, FilterMode.Bilinear, descriptor.colorFormat);
        tempID_H = new RenderTargetIdentifier(BlurRT_ID_H);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData){
        if(material == null){
            Debug.LogError("Blur effect Materials instance is null");
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get("Blur");
        cmd.Clear();
        
        var stack = VolumeManager.instance.stack;

        var blurEffect = stack.GetComponent<Blur_URP>();
        if(blurEffect.IsActive()){
            material.SetFloat("_BlurSize", blurEffect.blurAmount.value);
            material.SetInt("_Sample", blurEffect.sampleAmount.value);
        }
        else{
            return;
        }

        Blit(cmd, src, tempID_V, material, 0);
        Blit(cmd, tempID_V, tempID_H, material, 1);
        Blit(cmd, tempID_H, src);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    public override void OnCameraCleanup(CommandBuffer cmd){
        cmd.ReleaseTemporaryRT(BlurRT_ID_V);
        cmd.ReleaseTemporaryRT(BlurRT_ID_H);
    }
}
#endif