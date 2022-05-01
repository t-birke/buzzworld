using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class baseFloat : MonoBehaviour
{
    [SerializeField] private float _duration = 1f;

    [SerializeField] private Ease _easeType = Ease.OutSine;

    [SerializeField] private Vector3 _targetMove = new Vector3(0.1f, 0.1f, -0.1f);
    [SerializeField] private Quaternion _targetRotate = Quaternion.Euler(-2, -2, -2);

    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalMove(_targetMove, _duration).SetEase(_easeType).SetLoops(-1, LoopType.Yoyo);
        Quaternion endRotation = transform.rotation;
        endRotation *= _targetRotate;
        transform.DORotateQuaternion(endRotation, _duration).SetEase(_easeType).SetLoops(-1, LoopType.Yoyo);
    }

}
