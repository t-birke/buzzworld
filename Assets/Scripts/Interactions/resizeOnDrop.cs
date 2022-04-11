using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class resizeOnDrop : MonoBehaviour
{
    [SerializeField] private Vector3 _targetScale;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Drop()
    {
        var i = GetComponent<XRSocketInteractor>().firstInteractableSelected;
        i.transform.localScale = _targetScale;
        //reset grabState
        //i.transform.GetComponent<grabStateHandler>().setState(false);
    }
}
