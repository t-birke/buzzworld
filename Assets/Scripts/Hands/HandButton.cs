using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace Hands
{
    public class HandButton : XRBaseInteractable
    {

        public UnityEvent OnPress = null;
        
        [SerializeField] private float _wiggleRoom = 0.1f;
        
        
        private float yMin = 0.0f;
        private float yMax = 0.0f;
        private bool previousPress = false;
        private float previousHandHeight = 0.0f;
        private XRBaseInteractor hoverInteractor = null;

        protected override void Awake()
        {
            base.Awake();
            hoverEntered.AddListener(StartPress);
            hoverExited.AddListener(EndPress);
        }

        private void Start()
        {
            SetMinMax();
        }

        private void SetMinMax()
        {
            Collider collider = GetComponent<Collider>();
            yMin = transform.localPosition.y - (collider.bounds.size.y * 0.5f);
            yMax = transform.localPosition.y;
        }

        private void EndPress(HoverExitEventArgs arg0)
        {
            hoverInteractor = null;
            previousHandHeight = 0.0f;

            previousPress = false;
            SetYPosition(yMax);
        }

        private void StartPress(HoverEnterEventArgs arg0)
        {
            hoverInteractor = arg0.interactor;
            previousHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            hoverEntered.RemoveListener(StartPress);
            hoverExited.RemoveListener(EndPress);
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (hoverInteractor)
            {
                float newHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
                float handDifference = previousHandHeight - newHandHeight;
                previousHandHeight = newHandHeight;

                float newPosition = transform.localPosition.y - handDifference;
                SetYPosition(newPosition);
                
                CheckPress();
            }
        }

        private float GetLocalYPosition(Vector3 position)
        {
            Vector3 localPosition = transform.root.InverseTransformPoint(position);
            return localPosition.y;
        }

        private void SetYPosition(float position)
        {
            Vector3 newPosition = transform.localPosition;
            newPosition.y = Mathf.Clamp(position, yMin, yMax);
            transform.localPosition = newPosition;
        }

        private void CheckPress()
        {
            bool inPosition = InPosition();
            if (inPosition && !previousPress)
            {
                Debug.Log("Push it real good!");
                previousPress = true;
                SelectEnterEventArgs sargs = new SelectEnterEventArgs();
                sargs.interactable = this;
                sargs.interactor = hoverInteractor;
                selectEntered.Invoke(sargs);
                OnPress.Invoke();
            }
            
        }

        private bool InPosition()
        {
            float inRange = Mathf.Clamp(transform.localPosition.y, yMin, yMin + _wiggleRoom);
            return transform.localPosition.y == inRange;
        }
    }
}
