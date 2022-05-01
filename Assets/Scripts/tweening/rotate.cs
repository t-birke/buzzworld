using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class rotate : MonoBehaviour
{
    [SerializeField] private float _duration = 10f;

    [SerializeField] private Ease _easeType = Ease.Linear;

    [SerializeField] private Vector3 _targetRotate = new Vector3(0, 360, 0);

    // Start is called before the first frame update
    void Start()
    {
        transform.DORotate(_targetRotate, _duration,RotateMode.LocalAxisAdd).SetEase(_easeType).SetLoops(-1, LoopType.Incremental);
    }

}