using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Interactions
{
    public class EnterVehicleTrigger : MonoBehaviour
    {
        [SerializeField] private Transform sittingPoint;
        [SerializeField] private GameObject enterVehicleIndicator;
        [SerializeField] private GameObject xrRig;
        [SerializeField] private InputActionReference triggerAction;
        [SerializeField] private ActionBasedContinuousMoveProvider _moveProvider;
        
        [Header("Animation")] 
        [SerializeField] private float scaleFactor;
        [SerializeField] private float scaleDuration;
        [SerializeField] private Ease scaleEasing;


        private Boolean active = false;
        private List<Collider> inTrigger = new List<Collider>();

        private GameManager _gameManager;
        // Start is called before the first frame update
        void Start()
        {
            enterVehicleIndicator.SetActive(false);
            xrRig = FindObjectOfType<XROrigin>().gameObject;
            _moveProvider = xrRig.gameObject.GetComponentInChildren<ActionBasedContinuousMoveProvider>();
            _gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        private void OnTriggerEnter(Collider other)
        {
            inTrigger.Add(other);
            if (inTrigger.Count > 1) return;
            enterVehicleIndicator.SetActive(true);
            enterVehicleIndicator.transform.DOScale(enterVehicleIndicator.transform.localScale * scaleFactor, scaleDuration).SetEase(scaleEasing).SetLoops(-1, LoopType.Yoyo);
            triggerAction.action.performed += enterVehicle;
        }

        private void enterVehicle(InputAction.CallbackContext obj)
        {
            xrRig.transform.SetParent(sittingPoint);
            xrRig.transform.localPosition = Vector3.zero;
            xrRig.transform.localRotation = Quaternion.identity;
            xrRig.GetComponentInChildren<Camera>().transform.rotation = Quaternion.identity;
            _moveProvider.enabled = false;
            StartCoroutine(_gameManager.sendSalesforceMessage("vr-enter-vehicle", ""));
        }

        private void OnTriggerExit(Collider other)
        {
            inTrigger.Remove(other);
            if (inTrigger.Count > 0) return;
            enterVehicleIndicator.SetActive(false);
            triggerAction.action.performed -= enterVehicle;
        }
    }
}
