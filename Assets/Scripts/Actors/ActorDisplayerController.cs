using UnityEngine;

namespace Assets.Scripts.Actors
{
    [RequireComponent(typeof(Animator))]
    public class ActorDisplayerController : MonoBehaviour
    {
        private Animator _animator;
        private float _defaultSpeed;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _defaultSpeed = _animator.speed;

        }

        public void SetAnimationState(bool isOn)
        {
            _animator.speed = isOn ? _defaultSpeed : 0f;
        }
    }
}
