using System;
using Data;
using UnityEngine;
using UnityEngine.Video;

namespace Video
{
    public class VideoTrigger : MonoBehaviour
    {

        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private AppController _appController;
        [SerializeField] private GameObject _videoActiveLight;
        [SerializeField] private GameObject _videoReadyLight;
        private void OnTriggerEnter(Collider other)
        {
            _videoPlayer.Play();
            _appController.setIsPaused(true);
            _videoActiveLight.SetActive(true);
            _videoReadyLight.SetActive(false);
        }
        private void OnTriggerExit(Collider other)
        {
            _videoPlayer.Pause();
            _appController.setIsPaused(false);
            _videoActiveLight.SetActive(false);
            _videoReadyLight.SetActive(true);
        }
        
    }
}
