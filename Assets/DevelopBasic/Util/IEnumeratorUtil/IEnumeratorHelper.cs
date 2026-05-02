using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasingFunc;
using TMPro;
using UnityEngine.Rendering;

public static class CommonCoroutine{
    public static IEnumerator delayAction(Action action, float delay){
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    public static IEnumerator coroutineFadeSprite(SpriteRenderer m_sprite, float targetAlpha, float duration, float delay = 0){
        yield return new WaitForSeconds(delay);
        if(targetAlpha == 1) m_sprite.enabled = true;

        Color initColor = m_sprite.color;
        Color targetColor = initColor;
        targetColor.a = targetAlpha;

        yield return new WaitForLoop(duration, (t)=>{
            m_sprite.color = Color.Lerp(initColor, targetColor, Easing.SmoothInOut(t));
        });

        if(targetAlpha == 0) m_sprite.enabled = false;
    }
    public static IEnumerator coroutineFadeSpriteColor(SpriteRenderer m_sprite, Color targetColor, float duration){
        Color initColor = m_sprite.color;

        yield return new WaitForLoop(duration, (t)=>{
            m_sprite.color = Color.Lerp(initColor, targetColor, Easing.SmoothInOut(t));
        });
    }
    public static IEnumerator coroutineFadePerRenderColor(PerRendererColor m_renderer, float targetAlpha, float duration, float delay = 0){
        yield return new WaitForSeconds(delay);
        if(targetAlpha == 1) m_renderer.m_Renderer.enabled = true;

        Color initColor = m_renderer.tint;
        Color targetColor = initColor;
        targetColor.a = targetAlpha;

        yield return new WaitForLoop(duration, (t)=>
            m_renderer.tint = Color.Lerp(initColor, targetColor, Easing.SmoothInOut(t))
        );

        if(targetAlpha == 0) m_renderer.m_Renderer.enabled = false;
    }
    public static IEnumerator coroutineFadeUI(UnityEngine.UI.MaskableGraphic graphic, float targetAlpha, float duration){
        Color initColor, targetColor;
        initColor = graphic.color;
        targetColor = initColor;
        targetColor.a = targetAlpha;

        yield return new WaitForLoop(duration, (t)=>{
            graphic.color = Color.Lerp(initColor, targetColor, t);
        });
    }
    public static IEnumerator CoroutineFadeCharacter(TMP_CharacterInfo c, float targetAlpha, float duration, Easing.FunctionType easeType=Easing.FunctionType.QuadEaseOut){
        Color initColor = c.color;
        Color targetColor = initColor;
        targetColor.a = targetAlpha;

        string outlineSoftnessName = "_OutlineSoftness";
        c.material = Material.Instantiate(c.material);
        float initSoftness = c.material.GetFloat(outlineSoftnessName);
        float targetSoftness = 1-targetAlpha;

        var easeFunc = Easing.GetFunctionWithTypeEnum(easeType);
        yield return new WaitForLoop(duration, (t)=>{
            c.material.SetFloat(outlineSoftnessName, Mathf.Lerp(initSoftness, targetSoftness, easeFunc(Mathf.Clamp01(t*5))));
            c.color = Color.Lerp(initColor, targetColor, easeFunc(t));
        });
    }
    public static IEnumerator CoroutineDestroyDelay(float delay, GameObject gameObject){
        yield return new WaitForSeconds(delay);
        GameObject.Destroy(gameObject);
    }
    public static IEnumerator CoroutineMover(Transform trans, Vector3 targetPos, float duraiton, Easing.FunctionType ease, bool isLocal = false){
        var easeFunc = Easing.GetFunctionWithTypeEnum(ease);
        Vector3 initPos = isLocal?trans.localPosition:trans.position;
        yield return new WaitForLoop(duraiton, (t)=>{
            if(isLocal) trans.localPosition = Vector3.LerpUnclamped(initPos, targetPos, easeFunc(t));
            else trans.position = Vector3.LerpUnclamped(initPos, targetPos, easeFunc(t));
        });
    }
    public static IEnumerator CoroutineBlinkSprite(SpriteRenderer spriteRenderer, float duration, float blinkFreq){
        yield return new WaitForLoop(duration, (t)=>{
            spriteRenderer.enabled = Mathf.Cos(Mathf.PI*t*duration*blinkFreq)>0?true:false;
        });
        spriteRenderer.enabled = true;
    }
    public static IEnumerator CoroutineBlinkSprite(SpriteRenderer[] spriteRenderers, float duration, float blinkFreq){
        yield return new WaitForLoop(duration, (t)=>{
            for(int i=0; i<spriteRenderers.Length; i++){
                spriteRenderers[i].enabled = Mathf.Cos(Mathf.PI*t*duration*blinkFreq)>0?true:false;
            }
        });
        for(int i=0; i<spriteRenderers.Length; i++){
            spriteRenderers[i].enabled = true;
        }
    }
}

public class CoroutineExcuter
{
    MonoBehaviour initiator;
    IEnumerator coroutine;
    public CoroutineExcuter(MonoBehaviour _context)=>initiator = _context;
    public void Excute(IEnumerator go){
        if(coroutine!=null) initiator.StopCoroutine(coroutine);
        coroutine = go;
        initiator.StartCoroutine(go);
    }
    public void Abort(){
        if(coroutine!=null) initiator.StopCoroutine(coroutine);
    }
}

public class WaitForLoop: IEnumerator
{
    private IEnumerator m_coroutine;

    public WaitForLoop(float _duration, Action<float> _go){
        m_coroutine = ForLoopCoroutine(_duration, _go);
    }

    public object Current{get{return m_coroutine.Current;}}
    public bool MoveNext(){return m_coroutine.MoveNext();}
    public void Reset()=>m_coroutine.Reset();
    
    public static IEnumerator ForLoopCoroutine(float duration, Action<float> go){
        float speed = 1f/duration;
        for(float t=0; t<1; t+=Time.deltaTime*speed){
            go(t);
            yield return null;
        }
        go(1);
    }
}