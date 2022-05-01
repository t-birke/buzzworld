using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class quickAndDirty : MonoBehaviour
{
    [SerializeField] private GameObject _avatarBase;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveAvatarOne()
    {
        _avatarBase.transform.DOMove(new Vector3(-1.88f,1.6f,0.488000005f),
            2f).SetEase(Ease.InOutSine);
    }
}
