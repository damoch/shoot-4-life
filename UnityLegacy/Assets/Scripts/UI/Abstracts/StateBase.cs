using UnityEngine;

namespace Assets.Scripts.UI.Abstracts
{
    public abstract class StateBase : MonoBehaviour, IMenuState
    {
        [SerializeField]
        protected string _stateID;

        public virtual string StateID
        {
            get
            {
                return _stateID;
            }
        }

        public virtual void SetStateActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
