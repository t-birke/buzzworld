using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HipMenu : MonoBehaviour
{
    [SerializeField] private Transform _CameraRotation;
    [SerializeField] private float _visibilityThreshold = 40f;
    [SerializeField] private GameObject _container;
    public float xangle;
    private bool _menuVisible = false;
    
    //TODO: make fade in and out gently
    // Start is called before the first frame update
    void Start()
    {
        _container.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        xangle = _CameraRotation.eulerAngles.x;
        if (!_menuVisible &&  xangle > _visibilityThreshold && xangle < 180f)
        {
            _container.SetActive(true);
            _menuVisible = true;
        }
        if (_menuVisible && xangle < _visibilityThreshold)
        {
            _container.SetActive(false);
            _menuVisible = false;
        }
    }
}
