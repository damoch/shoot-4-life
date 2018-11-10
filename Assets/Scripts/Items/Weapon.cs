using UnityEngine;

namespace Assets.Scripts.Items
{
    public class Weapon : Item
    {
        #region Private variables
        [SerializeField]
        private GameObject _ammunitionObject;

        [SerializeField]
        private Ammunition _ammunition;

        [SerializeField]
        private float _cooldownTimeInSeconds;

        [SerializeField]
        private int _nuberOfRounds;

        private float _elapsedSeconds;
        private bool _isCoolingDown;
        #endregion

        #region Accessors
        public Ammunition Ammunition
        {
            get
            {
                return _ammunition;
            }

            set
            {
                _ammunition = value;
            }
        }

        public float CooldownTime
        {
            get
            {
                return _cooldownTimeInSeconds;
            }

            set
            {
                _cooldownTimeInSeconds = value;
            }
        }
        #endregion

        #region Methods
        private void Start()
        {
            if(_ammunition == null)
            {
                _ammunition = _ammunitionObject.GetComponent<Ammunition>();
            }

            _elapsedSeconds = 0;
            _isCoolingDown = false;
        }

        public void Shoot(Quaternion direction)
        {
            if (_isCoolingDown || _nuberOfRounds < 1) return;
            Instantiate(_ammunitionObject, transform.position, direction);//change to pooling later on
            _nuberOfRounds--;
            _isCoolingDown = true;
        }

        private void Update()
        {
            if (_isCoolingDown)
            {
                _elapsedSeconds += Time.deltaTime;

                if(_elapsedSeconds >= _cooldownTimeInSeconds)
                {
                    _elapsedSeconds = 0;
                    _isCoolingDown = false;
                }
            }
        }
        #endregion
    }
}
