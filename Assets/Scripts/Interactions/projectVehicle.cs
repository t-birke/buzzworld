using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interactions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class projectVehicle : MonoBehaviour
{
    [SerializeField] private XRRayInteractor _rayInteractor;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private GameObject vehiclePrefab;
    [SerializeField] private Vector3 reticleTargetRotation;
    [SerializeField] private InputActionReference joystick;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 _pivotOffset = new Vector3(0,0.5f,0);
    
    private XRDirectInteractor _interactor;

    private Boolean placementModeActivated = false;
    private Vector3 raycastTarget;
    private Vector3 targetNormal;
    private Boolean targetValid;
    private int targetSegment;
    private GameObject ghost;
    private configurator _configurator;

    public GameObject placedVehicle;
    private void Awake()
    {
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        _interactor = GetComponent<XRDirectInteractor>();
        _rayInteractor.gameObject.SetActive(false);
        _configurator = FindObjectOfType<configurator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (placementModeActivated)
        {
            if (ghost == null)
            {
                ghost = Instantiate(ghostPrefab, _rayInteractor.transform);
            }
            //activate LineRenderer
            _rayInteractor.gameObject.SetActive(true);
            _rayInteractor.TryGetHitInfo(out raycastTarget, out targetNormal, out targetSegment, out targetValid);
            ghost.transform.position = raycastTarget;
            ghost.transform.rotation = Quaternion.Euler(reticleTargetRotation);
            if (joystick.action.inProgress)
            {
                float rotation = joystick.action.ReadValue<Vector2>().y;
                ghost.transform.Rotate(Vector3.up, rotation * rotationSpeed);
                reticleTargetRotation.y += rotation;
                if (reticleTargetRotation.y > 360) reticleTargetRotation.y = 0;
                if (reticleTargetRotation.y < 0) reticleTargetRotation.y = 360;
            }
            
        }
    }

    public void startVehiclePlacement()
    {
        //limit to id buzz
        //todo: make independent of layer name
        if (_interactor.firstInteractableSelected.transform.name == "idbuzz-cargo-mini")
        {
            placementModeActivated = true;
            //hide configurator
            _configurator.hideConfigurator();
        }
        
    }

    public void placeVehicle()
    {
        if (placementModeActivated)
        {
            _rayInteractor.gameObject.SetActive(false);
            placementModeActivated = false;
            placedVehicle = Instantiate(vehiclePrefab, ghost.transform.position, ghost.transform.rotation);
            placedVehicle.transform.Translate(_pivotOffset);
            //save reference in game manager
            var gm = FindObjectOfType<GameManager>();
            gm.vehicle = placedVehicle;
            Destroy(ghost);
            //send message to Salesforce
            //TODO: use real price from dataset
            StartCoroutine(gm.sendSalesforceMessage("vr-select-vehicle","€ 36,820"));
            //show configurator and adjust position
            _configurator.resetConfiguratorPosition();
            _configurator.unhideConfigurator(3);
        }
    }

}







