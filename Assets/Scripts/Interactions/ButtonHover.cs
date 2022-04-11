using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class ButtonHover : MonoBehaviour
{
    [SerializeField] private Material hoverMaterial;
    [SerializeField] private float activationTime = 1f;
    [SerializeField] private GameObject _waitPrefab;
    [SerializeField] private float buttonTouchHapticStrength = 0.1f;
    [SerializeField] private float buttonPressHapticStrength = 0.8f;
    public UnityEvent buttonPressed;
    
    private Material originalMaterial;
    private List<GameObject> inTrigger = new List<GameObject>();

    private bool buttonHoverActive = false;

    private float activationTimer = 0f;
    private GameObject _progressBar;
    private Material _progressBarMat;
    private GameObject waitTimerObject;
    
    public delegate void buttonAction();
    public buttonAction m_buttonAction;
    
    private InputDevice _inputDevice;
    // Start is called before the first frame update
    private void Awake()
    {
        _waitPrefab.SetActive(false);
    }

    void Start()
    {
        originalMaterial = GetComponent<Renderer>().material;
        _inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonHoverActive)
        {
            activationTimer += Time.deltaTime;
            //haptics
            _inputDevice.SendHapticImpulse(0u, buttonTouchHapticStrength, Time.deltaTime);
            _progressBarMat.SetFloat("_Progress", activationTimer / activationTime);
            if (activationTimer >= activationTime)
            {
                //buttonpressed
                buttonPressed.Invoke();
                if (m_buttonAction != null)
                {
                    m_buttonAction();
                }
                //haptics
                _inputDevice.SendHapticImpulse(0u, buttonPressHapticStrength, 0.1f);
                Debug.Log("orb activated");
                deactivateButton();
            }
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inTrigger.Count == 0)
        {
            
                activateButton();
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
    }
    
    private void deactivateButton()
    {
        GetComponent<Renderer>().material = originalMaterial;
        buttonHoverActive = false;
        activationTimer = 0f;
        _progressBarMat.SetFloat("_Progress", 0f);
        _waitPrefab.SetActive(false);
    }
    
    private void InstantiatePlaceHolder()
    {
        _waitPrefab.SetActive(true);
        _progressBarMat = _waitPrefab.GetComponent<Renderer>().material;
    }
}
