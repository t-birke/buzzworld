using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.XR;

public class physicsButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Material _buttonHoverMaterial;
    [SerializeField] private Material _buttonClickMaterial;
    [SerializeField] private GameObject _buttonVisual;
    [SerializeField] private float _clickThresholdPercentage = 0.9f;
    [SerializeField] private float buttonTouchHapticStrength = 0.1f;
    [SerializeField] private float buttonPressHapticStrength = 0.8f;
    [SerializeField] private Renderer _buttonRenderer;
    private float distance;
    public List<Collider> inTrigger = new List<Collider>(); 
    private Material _buttonOriginalMaterial;
    private BoxCollider _trigger;
    private Vector3 _buttonOriginalLocalPosition;
    private UnityEngine.XR.InputDevice _inputDevice;
    public UnityEvent onPress;
    private Boolean buttonPressed = false;
    
    public delegate void buttonAction();
    public buttonAction m_buttonAction;
        void Awake()
    {
        _buttonOriginalMaterial = _buttonRenderer.material;
        _buttonOriginalLocalPosition = _buttonVisual.transform.localPosition;
        _trigger = GetComponent<BoxCollider>();
        //todo: currently hardcoded to right hand, make flexible based on hand that is entering the trigger
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inTrigger.Count == 0)
        {
            //doo stuff that needs to be done once the first collider enters the trigger
            _buttonRenderer.material = _buttonHoverMaterial;
            _inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            //other.ClosestPoint()
        }
        inTrigger.Add(other);
    }

    private void Update()
    {
        if (inTrigger.Count > 0)
        {
            //haptics
            _inputDevice.SendHapticImpulse(0u, buttonTouchHapticStrength, Time.deltaTime);
            //todo: this calculates the distance to the center back of the trigger collider.
            //Actually it should calculate the distance to the point that is closest to the collider on the backplane of the trigger collider,
            //so that no matter where I press, the distance is always correct
            var closestColliderPoint =
                inTrigger[0].ClosestPoint(transform.TransformPoint(new Vector3(0, -_trigger.size.y, 0)));
            distance = transform.InverseTransformPoint(closestColliderPoint).y;
            _buttonVisual.transform.localPosition = _buttonOriginalLocalPosition + new Vector3(0,distance,0);
            if (!buttonPressed && -distance > _trigger.size.y * _clickThresholdPercentage)
            {
                ButtonPress();
                if (m_buttonAction != null)
                {
                    m_buttonAction();
                }
            }
        }
    }

    private void ButtonPress()
    {
        Debug.Log("button prssd");
        _buttonRenderer.material = _buttonClickMaterial;
        //haptics
        _inputDevice.SendHapticImpulse(0u, buttonPressHapticStrength, 0.1f);
        //event fire
        onPress.Invoke();
        //set buttonPressed to true to avoid multiple firing
        buttonPressed = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger.Remove(other);
        if (inTrigger.Count > 0) return;
        resetButton();
    }

    public void resetButton()
    {
        _buttonRenderer.material = _buttonOriginalMaterial;
        _buttonVisual.transform.DOLocalMove(_buttonOriginalLocalPosition, 0.4f, false).SetEase(Ease.OutBounce);
        buttonPressed = false;
    }
    /*
    private void OnDrawGizmos()
    {
        
        if (_trigger)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere( transform.TransformPoint(new Vector3(0,-_trigger.size.y,0)) , 0.0003f); 
            

        }
    }
    */
}
