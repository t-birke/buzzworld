using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colourTrigger : MonoBehaviour
{
    [SerializeField] private GameManager.VehicleArea thisArea = GameManager.VehicleArea.upperBody;
    [SerializeField] private GameManager.VehicleColour thisColor = GameManager.VehicleColour.LimeYellowMetallic;
    
    private GameManager _gameManager;
    private List<Collider> inTrigger = new List<Collider>();
    // Start is called before the first frame update
    void Start()
    {
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
        _gameManager.changeVehicleColour(thisColor, thisArea);
    }
    
    private void OnTriggerExit(Collider other)
    {
        inTrigger.Remove(other);
        if (inTrigger.Count > 0) return;
        //do stuff

    }
}
