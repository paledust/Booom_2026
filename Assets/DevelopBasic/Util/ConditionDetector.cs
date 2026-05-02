using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConditionDetector
{
    [SerializeField] private float detectionTime;
    private float detectTimer = 0;
    public delegate void ConditionDelegate();
    private ConditionDelegate onCompleteFunc;
    private ConditionDelegate onResetFunc;
    private ConditionDelegate onConditionMeetFunc;
    
    public void OnComplete(ConditionDelegate func)=>onCompleteFunc = func;
    public void OnReset(ConditionDelegate func)=>onResetFunc = func;
    public void OnConditionMeet(ConditionDelegate func)=>onConditionMeetFunc = func;
    public float DetectUpdate(bool condition)
    {
        if(condition){
            if(Mathf.Approximately(detectTimer, 0f)){
                onConditionMeetFunc?.Invoke();
            }
            if(detectTimer<detectionTime){
                detectTimer += Time.deltaTime;
                if(detectTimer >= detectionTime){
                    onCompleteFunc?.Invoke();
                }
            }
        }
        else{
            if(detectTimer!=0){
                onResetFunc?.Invoke();
                detectTimer = 0;
            }
        }
        return detectTimer/detectionTime;
    }
}