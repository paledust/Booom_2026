using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_ObjectPool<T> : MonoBehaviour where T: MonoBehaviour
{
    [SerializeField] private GameObject poolPrefab;
    [SerializeField] private int MaxAmount = 15;

    private List<T> pools;
    private int neededAmount = 0;

    public static Action<T> E_OnThisRecycle;
    public static void Call_OnThisRecycle(T target)=>E_OnThisRecycle?.Invoke(target);
    
    void Awake(){
        pools = new List<T>();
        neededAmount = 0;
    }
    void OnDestroy(){
        CleanUp();
    }
    T GetObjFromPool(Predicate<T> condition){
        var pendingObj = pools.Find(x=>condition(x));
        if(pendingObj!=null){
            PrepareTarget(pendingObj);
            return pendingObj;
        }
        else{
            if(pools.Count<MaxAmount){
                var obj = SpawnTarget();
                pools.Add(obj);
                return obj;
            }
            else{
                neededAmount ++;
                return null;
            }
        }
    }
    protected T SpawnTarget(){
        var target = Instantiate(poolPrefab, transform).GetComponent<T>();
        PrepareTarget(target);
        return target;
    }
    protected void CleanUpPools(){
        foreach(var obj in pools){
            Destroy(obj.gameObject);
        }
        pools.Clear();        
    }
    void RecycleTarget(T target){
        if(neededAmount > 0){
            PrepareTarget(target);
            neededAmount --;
        }
    }
    /// <summary>
    /// This will be called during spawn and recycle,
    /// Usually the initiation of the objects.
    /// </summary>
    /// <param name="target">The Prepared target</param> <summary>
    protected virtual void PrepareTarget(T target){}
    /// <summary>
    /// This will be called after the pool destroied
    /// </summary>
    protected virtual void CleanUp()=>CleanUpPools();
}