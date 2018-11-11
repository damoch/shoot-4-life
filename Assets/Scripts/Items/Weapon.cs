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

        [SerializeField]
        private int _magazineCapacity;

        [SerializeField]
        private float _reloadLength;        

        private int _currentMagzineRounds;
        private float _elapsedCooldownSeconds;
        private float _elapsedReloadSeconds;
        private bool _isCoolingDown;
        private bool _isReloading;
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

            _elapsedCooldownSeconds = 0;
            _isCoolingDown = false;
            ReloadWeapon();
        }

        public void Shoot(Quaternion direction)
        {
            if (_isCoolingDown || _isReloading || _currentMagzineRounds < 1)
            {
                return;
            }
            Instantiate(_ammunitionObject, transform.position, direction);//change to pooling later on
            _currentMagzineRounds--;
            _isCoolingDown = true;
            _isReloading = _currentMagzineRounds < 1;
        }

        private void Update()
        {
            if (_isCoolingDown)
            {
                _elapsedCooldownSeconds += Time.deltaTime;

                if(_elapsedCooldownSeconds >= _cooldownTimeInSeconds)
                {
                    _elapsedCooldownSeconds = 0;
                    _isCoolingDown = false;
                }
            }

            if (_isReloading)
            {
                _elapsedReloadSeconds += Time.deltaTime;

                if(_elapsedReloadSeconds >= _reloadLength)
                {
                    ReloadWeapon();
                }
            }
        }

        private void ReloadWeapon()
        {
            _elapsedReloadSeconds = 0;
            _isReloading = false;

            if(_magazineCapacity > _nuberOfRounds)
            {
                _currentMagzineRounds = _nuberOfRounds;
                _nuberOfRounds = 0;
                return;
            }
            _nuberOfRounds -= _magazineCapacity;
            _currentMagzineRounds = _magazineCapacity;
        }
        #endregion
    }
}
