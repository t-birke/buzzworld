using System;
using DG.Tweening;
using identifier;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Interactions
{
    public class configurator : MonoBehaviour
    {
        [SerializeField] private Transform _playerHeadPosition;

        [SerializeField] private GameObject configuratorPrefab;
        [SerializeField] private GameObject configuratorStepThreePrefab;
        [SerializeField] private float _rotationOffset = 15f;
        [SerializeField] private Vector3 _configuratorSpawnOffset;
        
        
        

        private GameManager _gameManager;
        private GameObject _fakeCargoBuzz;
        private Vector3 _fakeCargoBuzzOriginalLocalPosition;
        public GameObject configuratorContainer;
        // Start is called before the first frame update
        void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void showConfigurator()
        {
            if (_gameManager && _gameManager.showConfigurator)
            {
                configuratorContainer = Instantiate(new GameObject(), _playerHeadPosition.position,
                    Quaternion.Euler(0,_playerHeadPosition.rotation.eulerAngles.y,0));
            
                configuratorContainer.transform.Rotate(Vector3.up, _rotationOffset);
                GameObject configurator = Instantiate(configuratorPrefab,configuratorContainer.transform);
                configurator.transform.localPosition = _configuratorSpawnOffset;
                _fakeCargoBuzz = configurator.GetNamedChild("idbuzz-cargo-mini");
                _fakeCargoBuzzOriginalLocalPosition = _fakeCargoBuzz.transform.localPosition;
            }
            
        }

        public void hideConfigurator()
        {
            //todo, add some hide effect
            configuratorContainer.SetActive(false);
        }
        public void unhideConfigurator(int nextStep)
        {
            Debug.Log("unhideConfigurator");
            //todo, add some show effect
            configuratorContainer.SetActive(true);
            //reset cargo buzz position
            _fakeCargoBuzz.transform.SetParent(configuratorContainer.GetNamedChild("cargo").transform);
            _fakeCargoBuzz.transform.localPosition = _fakeCargoBuzzOriginalLocalPosition;

            switch (nextStep)
            {
                case 3:
                    launchStepThree();
                    break;
                default:
                    break;
            }
        }
        public void resetConfiguratorPosition()
        {
            configuratorContainer.transform.position = _playerHeadPosition.position;
            configuratorContainer.transform.rotation =
                Quaternion.Euler(0, _playerHeadPosition.rotation.eulerAngles.y, 0);
            configuratorContainer.transform.Rotate(Vector3.up, _rotationOffset);
        }

        public void launchStepThree()
        {
            var stepOne = configuratorContainer.GetComponentInChildren<configuratorStepOne>();
            Instantiate(configuratorStepThreePrefab, stepOne.transform.position, stepOne.transform.rotation,
                configuratorContainer.transform);
            Destroy(stepOne.gameObject);
        }

    }
}
