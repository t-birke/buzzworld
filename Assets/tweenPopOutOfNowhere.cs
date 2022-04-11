using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

public class tweenPopOutOfNowhere : MonoBehaviour
{
    [Header("tween parameters")] [SerializeField]
    private float _delay;
    [SerializeField] private float _appearDuration;
    [SerializeField] private Ease _scaleEase = Ease.Linear;
    [SerializeField] private Ease _posEase = Ease.Linear;
    [SerializeField] private float _stayTime = 10f;
    [SerializeField] private Ease _outEase = Ease.OutElastic;

    [SerializeField] private float _outTime = 2f;

    [Header("Material Parameters")] 
    [SerializeField] private Material[] _mats;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startAppearTween());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator startAppearTween()
    {
        yield return new WaitForSeconds (_delay);
        transform.DOScale(1f, _appearDuration).SetEase(_scaleEase);
        transform.DOMoveY(1.894f, _appearDuration).SetEase(_posEase);
        yield return new WaitForSeconds(_stayTime);
        yield return applyTransparentMaterials();
        yield return fadeTween();
        yield return new WaitForSeconds(_outTime);
        Destroy(gameObject);
    }

    private IEnumerator fadeTween()
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            foreach (var m in r.materials)
            {
                m.DOFade(0, _outTime).SetEase(_outEase);
            }
        }
        yield return true;
    }

    private IEnumerator applyTransparentMaterials()
    {
        int i;
        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            i = 0;
            var originalMaterials = r.materials;
            foreach (var m in originalMaterials)
            {
                originalMaterials.SetValue(_mats[i], i);
                i++;
            }

            r.materials = originalMaterials;
        }
        yield return true;
    }
}
