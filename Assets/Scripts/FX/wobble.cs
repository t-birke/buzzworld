using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class wobble : MonoBehaviour
{
    [SerializeField] private Vector3 _targetMove;
    [SerializeField] private float _duration;
    [SerializeField] private Ease _easeType;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalMove(_targetMove, _duration).SetEase(_easeType).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
