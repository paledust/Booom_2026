using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInteractable_Trigger : MonoBehaviour
{
    [SerializeField]
    private Collider m_triggerbox;
    void OnTriggerEnter(Collider other){
        Interactable_Detector detector = other.GetComponent<Interactable_Detector>();
        if(detector != null){
            detector.RegisterInteractable(this);
        }
    }
    void OnTriggerExit(Collider other){
        Interactable_Detector detector = other.GetComponent<Interactable_Detector>();
        if(detector != null){
            detector.UnregisterInteractable(this);
        }
    }
    public void OnInteract(){}
    public void DisableTrigger(){m_triggerbox.enabled = false;}
    public void EnableTrigger(){m_triggerbox.enabled = true;}
}
