using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class grabStateHandler : MonoBehaviour
{
    public enum pose
    {
        NFT = 0,
        Vehicle = 1
    };

    [SerializeField] private pose _thisPose;
    
    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setState(Boolean state)
    {
        //check if the interactor is not a socket interactor
        var i = GetComponent<XRGrabInteractable>().firstInteractorSelecting;
        if (i.transform.GetComponentInChildren<XRSocketInteractor>() == null)
        {
            String triggerName = null;
            switch (_thisPose)
            {
                case pose.NFT :
                    triggerName = "NFTgrabPose";
                    break;
                case pose.Vehicle :
                    triggerName = "VehiclegrabPose";
                    break;
            }
            if (triggerName != null)
            {
                _gameManager.setAnimatorFlag(triggerName, state);
            }
        }
        else
        {
            if (_thisPose == pose.NFT && state)
            {
                //if the NFT is placed in it's socket, assign reference to Game Manager
                _gameManager.NFT = gameObject;
                StartCoroutine(_gameManager.sendSalesforceMessage("vr-transfer-nft", ""));
            }
        }
        
        
    }
}
