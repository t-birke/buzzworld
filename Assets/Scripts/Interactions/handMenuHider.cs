using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class handMenuHider : MonoBehaviour
{

    [SerializeField] private GameObject _handMenu; 
    [SerializeField] private float _enterMenuZone = 45f;
    [SerializeField] private float _exitMenuZone = 115f;

    [SerializeField] private Renderer menuBackgroundRenderer;

    [SerializeField] private Renderer[] buttonBackgroundRenderer;
    [SerializeField] private TextMeshPro[] textElements;
    [SerializeField] private float _fadeOutTime = 0.2f;
    [SerializeField] private float _fadeInTime = 0.2f;
    // Start is called before the first frame update
    private physicsButton[] _menuButtons;
    private GameManager _gameManager;
    private Boolean prevshowHandMenuStatus = true;
    private Vector4 menuBackgroundOriginalColor;
    private Vector4 buttonOriginalColor;
    private Boolean inTheZone = false;
    void Start()
    {
        _handMenu.SetActive(false);
        _gameManager = FindObjectOfType<GameManager>();
        _menuButtons = _handMenu.GetComponentsInChildren<physicsButton>(true);
        menuBackgroundOriginalColor = menuBackgroundRenderer.material.GetColor("_Color");
        buttonOriginalColor = buttonBackgroundRenderer[0].material.GetColor("_Color");
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager && _gameManager.showHandMenu)
        {
            if (!prevshowHandMenuStatus)
            {
                //track changes in status
                prevshowHandMenuStatus = true;
            }
            if (transform.rotation.eulerAngles.z > _enterMenuZone && transform.rotation.eulerAngles.z < _exitMenuZone)
            {
                if (!inTheZone)
                {
                    _handMenu.SetActive(true);
                    fadeIn();
                    inTheZone = true;
                }
            }
            else
            {
                if (inTheZone)
                {
                    fadeOut();
                    inTheZone = false;
                    foreach (var button in _menuButtons)
                    {
                        button.inTrigger.Clear();
                        button.resetButton();
                    }
                }
            }
        }
        else
        {
            if(prevshowHandMenuStatus)
            {
                //cover the case if show HandMenu just switched
                prevshowHandMenuStatus = false;
                fadeOut();
                inTheZone = false;
                foreach (var button in _menuButtons)
                {
                    button.inTrigger.Clear();
                    button.resetButton();
                }
            }
        }
        
    }

    void fadeOut()
    {
        menuBackgroundRenderer.material.DOFade(0, _fadeOutTime).OnComplete(hideMenu);
        foreach (var button in buttonBackgroundRenderer)
        {
            button.material.DOFade(0, _fadeOutTime);
        }
        foreach (var text in textElements)
        {
            text.DOFade(0, _fadeOutTime);
        }
    }

    void fadeIn()
    {
        menuBackgroundRenderer.material.DOFade(menuBackgroundOriginalColor[3], _fadeInTime);
        foreach (var button in buttonBackgroundRenderer)
        {
            button.material.DOFade(buttonOriginalColor[3], _fadeInTime);
        }
        foreach (var text in textElements)
        {
            text.DOFade(1, _fadeInTime);
        }
    }

    void hideMenu()
    {
        _handMenu.SetActive(false);
    }
}
