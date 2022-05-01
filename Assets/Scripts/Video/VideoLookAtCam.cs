using UnityEngine;

namespace Video
{
    public class VideoLookAtCam : MonoBehaviour
    {
        [SerializeField] private Transform _userCamera;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(new Vector3(_userCamera.position.x, transform.position.y, _userCamera.position.z));
        }
    }
}
