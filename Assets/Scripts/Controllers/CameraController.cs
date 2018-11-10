﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        #region Private variables
        [SerializeField]
        private Vector3 _offset;

        [SerializeField]
        private float _smoothTime;

        [SerializeField]
        private float _maxTransitionSpeed;

        private Camera _camera;
        private Vector3 _velocity;
        #endregion

        #region Accessors
        public List<Transform> TargetsToFollow  { get; set; }
        #endregion

        #region Methods
        private void Start()
        {
            _camera = Camera.main;
            TargetsToFollow = new List<Transform>();
        }

        private void LateUpdate()
        {
            if (TargetsToFollow.Count() == 0)
            {
                return;
            }

            var target = GetCenterPointOfSelectedActors() + _offset;
            _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, target, ref _velocity, _smoothTime, _maxTransitionSpeed, Time.deltaTime);
        }

        private Vector3 GetCenterPointOfSelectedActors()
        {
            if (TargetsToFollow.Count() == 1)
            {
                return TargetsToFollow[0].position;
            }

            var bounds = new Bounds(TargetsToFollow[0].position, Vector3.zero);

            for (var i = 1; i < TargetsToFollow.Count(); i++)
            {
                bounds.Encapsulate(TargetsToFollow[i].position);
            }

            var result = bounds.center;
            result.z = _camera.transform.position.z;
            return result;
        }
        #endregion
    }
}
