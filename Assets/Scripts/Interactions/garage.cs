using System;
using DG.Tweening;
using UnityEngine;

namespace Interactions
{
    public class garage : MonoBehaviour
    {
        [SerializeField] private Transform _playerHeadPosition;

        [SerializeField] private GameObject vehicleBasePrefab;
        [SerializeField] private GameObject labelPrefab;
        [SerializeField] private GameObject vehiclePlaceholderPrefab;

        [SerializeField] private float _rotationOffset = 15f;
        [SerializeField] private Vector3 _baseSpawnOffset;
        [SerializeField] private Vector3 _labelSpawnOffset;
        [SerializeField] private Vector3 _vehicleSpawnOffset;

        [SerializeField] private Ease _garageOutEase;
        [SerializeField] private float _garageOutDuration;
        [SerializeField] private Vector3 _garageOutRotation;
        private GameManager _gameManager;

        private GameObject _garage;
        // Start is called before the first frame update
        void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void showGarage()
        {
            if (_gameManager && !_gameManager.showGarage)
            {
                _gameManager.showGarage = true;
                _gameManager.HideHandMenu();
                _garage = Instantiate(new GameObject(), _playerHeadPosition.position,
                    Quaternion.Euler(0,_playerHeadPosition.rotation.eulerAngles.y,0));
            
                _garage.transform.Rotate(Vector3.up, _rotationOffset);
                GameObject vehicleBase = Instantiate(vehicleBasePrefab,_garage.transform);
                vehicleBase.transform.localPosition = _baseSpawnOffset;
                //button.transform.LookAt(_playerHeadPosition.position);
                //button.transform.Rotate(_buttonRotationOffset,Space.Self);
                GameObject label = Instantiate(labelPrefab,_garage.transform);
                label.transform.localPosition = _labelSpawnOffset;
                //find close button and assign close action
                label.GetComponentInChildren<physicsButton>().m_buttonAction = closeGarage;
                GameObject vehiclePlaceholder = Instantiate(vehiclePlaceholderPrefab,_garage.transform);
                vehiclePlaceholder.transform.localPosition = _vehicleSpawnOffset;
                //set launch car configurator action
                vehiclePlaceholder.GetComponentInChildren<ButtonHover>().m_buttonAction = openVehicleConfigurator;
            }
            
        }

        public void closeGarage()
        {
            _gameManager.showGarage = false;
            _gameManager.UnhideHandMenu();
            if (_garage)
            {
                Destroy(_garage,0.3f);
            }
        }



        private void openVehicleConfigurator()
        {
            if (_garage)
            {
                _garage.transform.DOLocalRotate(_garageOutRotation, _garageOutDuration,RotateMode.LocalAxisAdd)
                    .SetEase(_garageOutEase).OnComplete(() => { _gameManager.ShowConfiguratorAction();});
            }
            
        }
        
    }
}
