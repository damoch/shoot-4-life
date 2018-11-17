using UnityEngine;

namespace Assets.Scripts.Actors
{
    public class ActorDisplayerController : MonoBehaviour
    {
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayMovementClip()
        {

        }
    }
}
