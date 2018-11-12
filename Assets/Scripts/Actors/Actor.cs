using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Items;
using UnityEngine;
namespace Assets.Scripts.Actors
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Actor : MonoBehaviour
    {
        #region Private variables
        [SerializeField]
        private bool _isSelected;

        [SerializeField]
        private int _healthPoints;

        [SerializeField]
        private bool _isAlive;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private string _name;

        [SerializeField]
        private Team _team;

        [SerializeField]
        private GameObject _weaponObject;

        [SerializeField]
        private ActorType _actorType;

        [SerializeField]
        private Vector2 _targetPosition;

        [SerializeField]
        private bool _isInfected;

        private GameObject _actorDisplayer;
        private Rigidbody2D _rigidbody2D;
        private LineRenderer _lineRenderer;

        #endregion

        #region Accesors
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (!_isAlive)
                {
                    return;
                }
                _isSelected = value;
                if (_rigidbody2D == null)
                {
                    _rigidbody2D = GetComponent<Rigidbody2D>();
                }
                _rigidbody2D.bodyType = _isSelected ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;

                if(_lineRenderer != null)
                {
                    _lineRenderer.SetPosition(1, transform.position);
                    _lineRenderer.SetPosition(0, transform.position);
                }
                if(_actorDisplayer != null)
                {
                    _actorDisplayer.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
        }

        public int HealthPoints
        {
            get
            {
                return _healthPoints;
            }

            set
            {
                if(value <= 0)
                {
                    KillActor();
                }
                _healthPoints = value;
            }
        }

        private void KillActor()
        {
            _isAlive = false;
            _isSelected = false;
            _speed = 0;
        }

        public bool IsAlive
        {
            get
            {
                return _isAlive;
            }
        }

        public Team Team
        {
            get
            {
                return _team;
            }

            set
            {
                _team = value;
            }
        }

        public Weapon Weapon { get; set; }

        public ActorType ActorType
        {
            get
            {
                return _actorType;
            }

            set
            {
                _actorType = value;
            }
        }

        public bool IsInfected
        {
            get
            {
                return _isInfected;
            }

            set
            {
                _isInfected = value;
            }
        }
        #endregion

        #region Methods
        private void Start()
        {
            Weapon = _weaponObject.GetComponent<Weapon>();

            if(_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }
            _actorDisplayer = transform.GetChild(0).gameObject;

            _lineRenderer = _actorDisplayer.GetComponent<LineRenderer>();
            _lineRenderer.SetPosition(0, transform.position);
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _isSelected = false;

            if (_healthPoints > 0)
            {
                _isAlive = true;
            }
        }

        public bool GetCommand(Commands command)
        {
            if(!_isAlive || !_isSelected)
            {
                return false;
            }
            var spd = _speed * Time.deltaTime;
            _lineRenderer.SetPosition(0, transform.position);
            switch (command)
            {
                case Commands.Up:
                    transform.Translate(Vector2.up * spd);
                    break;
                case Commands.Down:
                    transform.Translate(Vector2.down * spd);
                    break;
                case Commands.Right:
                    transform.Translate(Vector2.right * spd);
                    break;
                case Commands.Left:
                    transform.Translate(Vector2.left * spd);
                    break;
                case Commands.Shoot:
                    Weapon.Shoot(_actorDisplayer.transform.localRotation);
                    break;
            }
            return true;
        }

        public void LookAt(Vector2 position)
        {
            _actorDisplayer.transform.localRotation = Quaternion.Euler(0f, 0f, AngleBetweenTwoPoints(transform.position, position));
            _lineRenderer.SetPosition(1, position);
        }

        public void MoveTowards(Vector2 position)
        {
            if(_actorType != ActorType.NonPlayable)
            {
                return;
            }
            _targetPosition = position;
        }

        private float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }

        private void Update()
        {
            if(_actorType != ActorType.NonPlayable || !_isSelected)
            {
                return;
            }
            var spd = Time.deltaTime * _speed;
            if(_targetPosition != null)
            {
                _lineRenderer.SetPosition(0, transform.position);
                transform.position = Vector2.MoveTowards(transform.position, _targetPosition, spd);
            }
        }
    }
    #endregion
}

