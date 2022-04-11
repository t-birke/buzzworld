using System;
using System.Collections;
using System.Collections.Generic;
using Hands;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class colorChange : MonoBehaviour
{
    [SerializeField] private Renderer _button;
    [SerializeField] private float _buttonPressLightIndicatorTime = 0.3f;
    private Color _baseColor;
    private HandButton _interactable;
    private bool pressed = false;

    private Color _hoverColor = new Color(0.8f,0.92f,0.07f);
    private void Awake()
    {
        _baseColor = _button.material.GetColor("_Color");
        _interactable = GetComponent<HandButton>();
        _interactable.hoverEntered.AddListener(SetHoverColor);
        _interactable.hoverExited.AddListener(SetOriginalColor);
        _interactable.OnPress.AddListener(SetPressColor);
    }

    private void SetOriginalColor(HoverExitEventArgs arg0)
    {
        if (pressed) return;
        _button.material.SetColor("_Color", _baseColor);
    }

    private void SetHoverColor(HoverEnterEventArgs arg0)
    {
        if (pressed) return;
        _button.material.SetColor("_Color", _hoverColor);
    }
    
    private void SetPressColor()
    {
        pressed = true;
        _button.material.SetColor("_Color", Color.green);
        StartCoroutine(ResetColorAfterPressColor());
    }

    private IEnumerator ResetColorAfterPressColor()
    {
        yield return new WaitForSeconds(_buttonPressLightIndicatorTime);
        pressed = false;
        if (_interactable.isHovered)
        {
            SetHoverColor(null);
        }
        else
        {
            SetOriginalColor(null);
        }
    }
}
