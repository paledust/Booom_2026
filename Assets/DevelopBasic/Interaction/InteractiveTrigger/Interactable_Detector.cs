using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Detector : MonoBehaviour
{
    private List<BasicInteractable_Trigger> interactables;
    BasicInteractable_Trigger currentInteractable;
    void Awake(){interactables = new List<BasicInteractable_Trigger>();}
    /// <summary>
    /// Register an interactable into the list
    /// And judge if we should switch current Interactable to this interactable
    /// </summary>
    /// <param name="_interactable"></param>
    public void RegisterInteractable(BasicInteractable_Trigger _interactable){
        Debug.Log("Register an interactable");
        if(!interactables.Contains(_interactable)){
            interactables.Add(_interactable);
            TryAssignNewInteractable(_interactable);
        }
    }
    /// <summary>
    /// Unregister this interactable from the list
    /// And find a new interactable as current interactable
    /// </summary>
    /// <param name="_interactable"></param>
    public void UnregisterInteractable(BasicInteractable_Trigger _interactable){
        Debug.Log("Unregister an interactable");
        if(interactables.Contains(_interactable)){
            interactables.Remove(_interactable);
            TryAssignNewInteractable();
        }
    }
    /// <summary>
    /// Compare the current interactable to this interactable,
    /// And check if we should switch current interactable to the new one.
    /// keep the current interactable if this interactable is null.
    /// </summary>
    /// <param name="_interactable"></param>
    void TryAssignNewInteractable(BasicInteractable_Trigger _interactable){
        if(_interactable==null){
            return;
        }
        else{
            if(currentInteractable==null) {
                AssignANewInteractable(_interactable);
            }
            else if(Vector3.Distance(_interactable.transform.position,transform.position) < Vector3.Distance(currentInteractable.transform.position,transform.position)){
                AssignANewInteractable(_interactable);
            }
        }
    }
    /// <summary>
    /// Assign the closest interactable as current interactable,
    /// If not find any registered interactable, set current interactable to null.
    /// </summary>
    /// <param name="_interactable"></param>
    void TryAssignNewInteractable(){
        float distance = Mathf.Infinity;
        BasicInteractable_Trigger result = null;
        foreach(BasicInteractable_Trigger interactable in interactables){
            if(Vector3.Distance(transform.position, interactable.transform.position) < distance){
                distance = Vector3.Distance(transform.position, interactable.transform.position);
                result = interactable;
            }
        }
        AssignANewInteractable(result);
    }
    void AssignANewInteractable(BasicInteractable_Trigger _interactable){
        currentInteractable = _interactable;
    }
}
