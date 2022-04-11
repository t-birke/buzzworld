using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringWheel : MonoBehaviour
{
    [SerializeField] private Transform _attachPoint;
    [SerializeField] private InputActionReference triggerAction;
    [SerializeField] private InputActionReference driveAction;
    [SerializeField] private float accelerationFactor = 0.01f;
    [SerializeField] private Rigidbody motorRB;
    [SerializeField] private float fwdSpeed = 100f;
    [SerializeField] private float revSpeed = 50f;
    [SerializeField] private float turnSpeed = 50f;
    private Boolean active = false;
    private List<Collider> inTrigger = new List<Collider>();
    private GameObject _steeringWheelColliders;
    private float acceleration;
    
    private GameManager _gameManager;

    private float currentVelocity = 0f;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        foreach (var c in GetComponentsInChildren<Transform>())
        {
            Debug.Log("child name = " + c.name);
            if (c.name == "steeringWheelCollider") _steeringWheelColliders = c.gameObject;
        }
        //detach Motor from car
        motorRB.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            acceleration  = driveAction.action.ReadValue<Vector2>().y;
            acceleration *= acceleration > 0 ? fwdSpeed : revSpeed;
            _gameManager.vehicle.transform.position = motorRB.transform.position;

            
            //set rotation
            if(motorRB.velocity.magnitude > 0.01f)
            {
                float newRotation = _gameManager.steeringAngle / 180 * turnSpeed * Time.deltaTime;
                //check if we are moving forward or backward
                var localVel = _gameManager.vehicle.transform.InverseTransformDirection(motorRB.velocity);
                newRotation *= localVel.x > 0 ? 1 : -1;
                _gameManager.vehicle.transform.Rotate(0,newRotation,0, Space.World);
            }
            
            
            /*
            _gameManager.vehicle.transform.Translate(-acceleration * accelerationFactor,0,0,Space.World);
            Debug.Log("moving by " + acceleration * accelerationFactor + "m/frame");
            */
            //update speed
            if (_gameManager.NFT != null)
            {
                var needle = _gameManager.NFT.GetNamedChild("Layer6").transform;
                needle.localEulerAngles = new Vector3(0,motorRB.velocity.magnitude * 5,0);
            }
        }
    }

    private void FixedUpdate()
    {
        motorRB.AddForce(-_gameManager.vehicle.transform.right * acceleration, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        inTrigger.Add(other);
        if (inTrigger.Count > 1) return;
        triggerAction.action.started += grabSteeringWheel;
        
    }

    private void grabSteeringWheel(InputAction.CallbackContext obj)
    {
        Debug.Log("grabSteeringWheel called");
        active = true;
        //disable all colliders
        _steeringWheelColliders.SetActive(false);
        //TODO: use both hands
        _gameManager.LaunchSteeringWheelMode(GameManager.HandSide.left, _attachPoint.GetComponent<Rigidbody>());
        triggerAction.action.started -= grabSteeringWheel;
        triggerAction.action.started += releaseSteeringWheel;
    }
    
    private void releaseSteeringWheel(InputAction.CallbackContext obj)
    {
        
        Debug.Log("release steering wheel");
        active = false;
        //enable all colliders
        _steeringWheelColliders.SetActive(true);
        //release Steering Wheel
        //TODO: both hands
        _gameManager.EndSteeringWheelMode(GameManager.HandSide.left);
        triggerAction.action.started -= releaseSteeringWheel;
        triggerAction.action.started += grabSteeringWheel;
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger.Remove(other);
        if (inTrigger.Count > 0) return;
        triggerAction.action.started -= grabSteeringWheel;
    }
}
