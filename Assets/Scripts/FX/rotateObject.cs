using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class rotateObject : MonoBehaviour
{
    [SerializeField] private float _duration = 6f;

    [SerializeField] private Ease _easeType = Ease.Linear;

    [SerializeField] private Vector3 _targetRotate = new Vector3(0, 360, 0);

    private Tween rotationTween;
    // Start is called before the first frame update
    void Start()
    {
        rotationTween = transform.DORotate(_targetRotate, _duration,RotateMode.LocalAxisAdd).SetEase(_easeType).SetLoops(-1, LoopType.Incremental);
    }

    public void StopRotation()
    {
        rotationTween.Kill();
    }
    
    public void RestartRotation()
    {
        rotationTween.Play();
    }

}
