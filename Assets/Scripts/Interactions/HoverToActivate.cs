using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Events;

public class HoverToActivate : MonoBehaviour
{
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private GameObject _waitPrefab;
    [SerializeField] private GameObject[] _objectsToActivate;
    public AppController _appController;
    public UnityEvent buttonPressed;
    
    private Material originalMaterial;
    
    private bool buttonHoverActive = false;

    private Vector3 _plasmaSize;
    private GameObject _plasma;
    
    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = GetComponent<Renderer>().material;
        //deactivate info panels
        foreach (var o in _objectsToActivate)
        {
            o.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void destroyButton()
    {
        //todo - if there are more buttons, destroy these as well. Probably they are in the appManager
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(buttonHoverActive) return;
        activateButton();
    }
    
    private void OnTriggerExit(Collider other)
    {
       
    }

    private void activateButton()
    {
        InstantiatePlaceHolder();
        GetComponent<Renderer>().material = hoverMaterial;
        buttonHoverActive = true;
        _appController.choiceButtonActive = true;
        //activate info panels
        foreach (var o in _objectsToActivate)
        {
            o.SetActive(true);
        }
        
    }
    
   
    
    private void InstantiatePlaceHolder()
    {
        var cs = GetComponentsInChildren<Transform>();
        foreach (var c in cs)
        {
            if(c.CompareTag("plasma"))
            {
                _plasma = c.gameObject;
                _plasmaSize = c.localScale;
                c.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }
        }
            
    }

    public void Deactivate()
    {
        buttonHoverActive = false;
        if (_plasma != null)
        {
            _plasma.transform.localScale = _plasmaSize;
        }
        //deactivate info panels
        foreach (var o in _objectsToActivate)
        {
            o.SetActive(false);
        }
        
        //todo
        destroyButton();
    }
}
