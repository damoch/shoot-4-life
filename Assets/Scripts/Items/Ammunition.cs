using Assets.Scripts.Actors;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class Ammunition : Item
    {
        #region Private variables
        [SerializeField]
        private float _speed;

        [SerializeField]
        private int _damageValue;

        [SerializeField]
        private AmmunitionType _ammunitionType;

        [SerializeField]
        private bool _isPenetrator;

        [SerializeField]
        private float _meeleAttackLength;

        private float _step;
        #endregion

        #region Accessors
        public float Speed
        {
            get
            {
                return _speed;
            }

            set
            {
                _speed = value;
            }
        }

        public int DamageValue
        {
            get
            {
                return _damageValue;
            }

            set
            {
                _damageValue = value;
            }
        }

        public AmmunitionType AmmunitionType
        {
            get
            {
                return _ammunitionType;
            }

            set
            {
                _ammunitionType = value;
            }
        }

        public bool IsPenetrator
        {
            get
            {
                return _isPenetrator;
            }

            set
            {
                _isPenetrator = value;
            }
        }
        #endregion

        #region Methods
        private void Start()
        {
            if(_ammunitionType == AmmunitionType.Meele)
            {
                _speed = 0;
            }
        }

        private void Update()
        {
            if(_ammunitionType == AmmunitionType.Meele && _meeleAttackLength > 0)
            {
                _meeleAttackLength -= Time.deltaTime;
                if(_meeleAttackLength <= 0)
                {
                    Destroy(gameObject);
                }
                return;
            }
            _step = _speed * Time.deltaTime;
            transform.Translate(Vector2.left * _step);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var gObject = collision.gameObject;

            var actor = gObject.GetComponent<Actor>();
            if (actor != null && actor.IsAlive)
            {
                actor.HealthPoints -= _damageValue;
            }

            if (!_isPenetrator && actor.IsAlive)
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}
