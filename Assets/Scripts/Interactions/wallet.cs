using System;
using DG.Tweening;
using UnityEngine;

namespace Interactions
{
    public class wallet : MonoBehaviour
    {
        [SerializeField] private Transform _playerHeadPosition;

        [SerializeField] private GameObject vehicleBasePrefab;
        [SerializeField] private GameObject labelPrefab;

        [SerializeField] private float _rotationOffset = 15f;
        [SerializeField] private Vector3 _baseSpawnOffset;
        [SerializeField] private Vector3 _labelSpawnOffset;
        [SerializeField] private Vector3 _labelRotationOffset;

        [SerializeField] private Ease _garageOutEase;
        [SerializeField] private float _garageOutDuration;
        [SerializeField] private Vector3 _garageOutRotation;
        private GameManager _gameManager;

        private GameObject _wallet;
        // Start is called before the first frame update
        void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void showWallet()
        {
            if (_gameManager && !_gameManager.showGarage)
            {
                _gameManager.HideHandMenu();
                _wallet = Instantiate(new GameObject(), _playerHeadPosition.position,
                    Quaternion.Euler(0,_playerHeadPosition.rotation.eulerAngles.y,0));
            
                _wallet.transform.Rotate(Vector3.up, _rotationOffset);
                GameObject vehicleBase = Instantiate(vehicleBasePrefab,_wallet.transform);
                vehicleBase.transform.localPosition = _baseSpawnOffset;
                //button.transform.LookAt(_playerHeadPosition.position);
                //button.transform.Rotate(_buttonRotationOffset,Space.Self);
                GameObject label = Instantiate(labelPrefab,_wallet.transform);
                label.transform.localPosition = _labelSpawnOffset;
                label.transform.Rotate(_labelRotationOffset);
                //find close button and assign close action
                label.GetComponentInChildren<physicsButton>().m_buttonAction = closeWallet;
            }
            
        }

        public void closeWallet()
        {
            _gameManager.showWallet = false;
            _gameManager.UnhideHandMenu();
            if (_wallet)
            {
                Destroy(_wallet,0.3f);
            }
        }
        
    }
}
