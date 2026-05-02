using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDetector<T> : MonoBehaviour where T: MonoBehaviour
{
    [SerializeField] private Transform detectCenter;
    [SerializeField, ShowOnly] private List<T> pendingTargerts;
    [SerializeField, ShowOnly] private T currentTarget;
    
    public bool m_isDetecting{get; private set;} = true;
    public T m_currentTarget{get{return currentTarget;}}
    public List<T> m_pendingTargets{get{return pendingTargerts;}}

    public void StopDetecting(){
        m_isDetecting = false;
        SwapTarget(null);
    }
    public void StartDetecting(){
        m_isDetecting = true;
        TryAssignNewTarget();
    }
    public void RegisterPendingTarget(T pendingTarget){
        if(!pendingTargerts.Contains(pendingTarget)){
            pendingTargerts.Add(pendingTarget);

            TryAssignNewTarget(pendingTarget);
        }
    }
    public bool UnregisterPendingTarget(T pendingTarget){
        if(pendingTargerts.Contains(pendingTarget)){
            pendingTargerts.Remove(pendingTarget);
            TryAssignNewTarget();

            return true;
        }
        return false;
    }
    public T GetClosestTargetFromPending(){
        TryAssignNewTarget();
        return currentTarget;
    }
    public bool IsThisRegistered(T target){
        return pendingTargerts.Contains(target);
    }
    void TryAssignNewTarget(T testingTarget){
        if(!m_isDetecting) return;

        if(currentTarget==null) SwapTarget(testingTarget);
        else if(Vector3.Distance(testingTarget.transform.position, detectCenter.position) < Vector3.Distance(currentTarget.transform.position, detectCenter.position)){
            SwapTarget(testingTarget);
        }
    }
    void TryAssignNewTarget(){
        if(!m_isDetecting) return;

        float distance = Mathf.Infinity;
        T result = null;
        foreach(T target in pendingTargerts){
            if(Vector3.Distance(detectCenter.position, target.transform.position) < distance){
                distance = Vector3.Distance(detectCenter.position, target.transform.position);
                result = target;
            }
        }
        SwapTarget(result);
    }
    private void SwapTarget(T newTarget){
        currentTarget = newTarget;
    }
}