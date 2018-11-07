﻿using UnityEngine;

namespace Assets.Scripts.Items
{
    public class Weapon : Item
    {
        [SerializeField]
        private GameObject _ammunitionObject;

        [SerializeField]
        private Ammunition _ammunition;

        [SerializeField]
        private float _cooldownTimeInSeconds;
        private float _elapsedSeconds;

        private bool _isCoolingDown;

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

        private void Start()
        {
            if(_ammunition == null)
            {
                _ammunition = _ammunitionObject.GetComponent<Ammunition>();
            }

            _elapsedSeconds = 0;
            _isCoolingDown = false;
        }

        public void Shoot()
        {
            if (_isCoolingDown) return;
            Debug.Log("BANG!");
            _isCoolingDown = true;



            //change to pooling later on
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
    }
}
