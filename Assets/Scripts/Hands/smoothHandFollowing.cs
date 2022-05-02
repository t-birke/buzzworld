using DG.Tweening;
using UnityEngine;

namespace Hands
{
    public class smoothHandFollowing : MonoBehaviour
    {
        [SerializeField] private Transform handPosition;
        [SerializeField] private float threshold;
        [SerializeField] private float speed;
        private Vector3 lastMenuPosition;

        // Start is called before the first frame update
        void Start()
        {
            lastMenuPosition = handPosition.transform.position;
            transform.position = handPosition.transform.position;
            transform.rotation = handPosition.transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(handPosition.position, lastMenuPosition) > threshold)
            {
                lastMenuPosition = handPosition.position;
                transform.DOMove(handPosition.position, speed).SetEase(Ease.InQuad);
                transform.DORotateQuaternion(handPosition.rotation, speed).SetEase(Ease.InQuad);
            }
        }
    }
}
