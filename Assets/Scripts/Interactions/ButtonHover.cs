using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Events;

public class ButtonHover : MonoBehaviour
{
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private float activationTime = 1f;
    [SerializeField] private GameObject _waitPrefab;
    public AppController _appController;
    public UnityEvent buttonPressed;
    
    private Material originalMaterial;
    private List<GameObject> inTrigger = new List<GameObject>();

    private bool buttonHoverActive = false;

    private float activationTimer = 0f;
    private GameObject _progressBar;
    private Vector3 _plasmaSize;
    private GameObject _plasma;
    private Material _progressBarMat;
    private GameObject waitTimerObject;
    //Function to call on activation
    public delegate void buttonAction();
    public buttonAction m_buttonAction;
    
    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonHoverActive)
        {
            activationTimer += Time.deltaTime;
            _progressBarMat.SetFloat("_Progress", activationTimer / activationTime);
            if (activationTimer >= activationTime)
            {
                //buttonpressed
                buttonPressed.Invoke();
                if(m_buttonAction != null) m_buttonAction();
                deactivateButton();
                //remove button
                destroyButton();
            }
        }
        
        
    }

    private void destroyButton()
    {
        //todo - if there are more buttons, destroy these as well. Probably they are in the appManager
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inTrigger.Count == 0)
        {
            //check if another button is already activated, if so, skip activation of this button
            if (!_appController.choiceButtonActive)
            {
                activateButton();
            }
        };
        inTrigger.Add(other.gameObject);
    }
    
    private void OnTriggerExit(Collider other)
    {
        inTrigger.Remove(other.gameObject);
        if (inTrigger.Count > 0) return;
        deactivateButton();

    }

    private void activateButton()
    {
        InstantiatePlaceHolder();
        GetComponent<Renderer>().material = hoverMaterial;
        buttonHoverActive = true;
        _appController.choiceButtonActive = true;
    }
    
    private void deactivateButton()
    {
        GetComponent<Renderer>().material = originalMaterial;
        buttonHoverActive = false;
        activationTimer = 0f;
        _appController.choiceButtonActive = false;
        _progressBarMat.SetFloat("_Progress", 0f);
        if (_plasma != null)
        {
            _plasma.transform.localScale = _plasmaSize;
        }
    }
    
    private void InstantiatePlaceHolder()
    {
        var cs = GetComponentsInChildren<Transform>();
        foreach (var c in cs)
        {
            if (c.CompareTag("ProgressBar"))
            {
                _progressBar = c.gameObject;
                _progressBarMat = _progressBar.GetComponentInChildren<Renderer>().material;
            }
            else if(c.CompareTag("plasma"))
            {
                _plasma = c.gameObject;
                _plasmaSize = c.localScale;
                c.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            }
        }
            
    }
}
