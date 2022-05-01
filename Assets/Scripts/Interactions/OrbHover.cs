using Data;
using UnityEngine;
using UnityEngine.Events;

namespace Interactions
{
    public class OrbHover : MonoBehaviour
    {
        public UnityEvent buttonPressed;
        public delegate void buttonAction();
        public buttonAction m_buttonAction;

 
        private void destroyButton()
        {
            //todo - if there are more buttons, destroy these as well. Probably they are in the appManager
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            //buttonpressed
            buttonPressed.Invoke();
            if(m_buttonAction != null) m_buttonAction();
            //remove button
            destroyButton();
        }


    }
}
