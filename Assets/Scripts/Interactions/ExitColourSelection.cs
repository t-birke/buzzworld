using UnityEngine;

namespace Interactions
{
    public class ExitColourSelection : MonoBehaviour
    {
        
        private GameManager _gameManager;
        
        // Start is called before the first frame update
        void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void CloseAndExit()
        {
            //send message to Salesforce
            StartCoroutine(_gameManager.sendSalesforceMessage("vr-color-selected",_gameManager.selectedUpperBodyColour + ";" + _gameManager.selectedLowerBodyColour));
            //TODO: properly close the configurator
            FindObjectOfType<configurator>().configuratorContainer.SetActive(false);
            if(_gameManager.showGarage) FindObjectOfType<garage>().closeGarage();
        }
    }
}
