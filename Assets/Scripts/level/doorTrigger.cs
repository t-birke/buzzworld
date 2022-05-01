using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorTrigger : MonoBehaviour
{
    [SerializeField] private Animator _doorAnimator;
//todo: make private
    public List<GameObject> inTrigger;
    // Start is called before the first frame update
    void Start()
    {
        closeDoor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(inTrigger.Count == 0){openDoor();};
        inTrigger.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger.Remove(other.gameObject);
        if (inTrigger.Count > 0) return;
        closeDoor();
        {
            
        }
    }

    private void closeDoor()
    {
        _doorAnimator.SetTrigger("Close");
    }
    
    private void openDoor()
    {
        _doorAnimator.SetTrigger("Open");
    }
    
}
