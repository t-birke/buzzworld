using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class volumeControl : MonoBehaviour
    {
        [SerializeField] private int volumeStep;
        [SerializeField] private Renderer dashboardRenderer;
        [Header("textures")] 
        [SerializeField] private Texture volume1tex;
        [SerializeField] private Texture volume2tex;
        [SerializeField] private Texture volume3tex;
        
        private GameManager _gameManager;
        private List<Collider> inTrigger = new List<Collider>();
        private Texture originalTex;
        void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
            originalTex = dashboardRenderer.material.mainTexture;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        private void OnTriggerEnter(Collider other)
        {
            inTrigger.Add(other);
            if (inTrigger.Count > 1) return;
            switch (volumeStep)
            {
                case 1: 
                    dashboardRenderer.material.mainTexture = volume1tex;
                    break;
                case 2:
                    dashboardRenderer.material.mainTexture = volume2tex;
                    break;
                case 3:
                    dashboardRenderer.material.mainTexture = volume3tex;
                    break;
            }
            _gameManager.hideVolumeInfoPanel(resetTexture, volumeStep);
            
        }

        private void OnTriggerExit(Collider other)
        {
            inTrigger.Remove(other);
            if (inTrigger.Count > 0) return;
            
        }

        private void resetTexture()
        {
            dashboardRenderer.material.mainTexture = originalTex;
        }
    }
}
