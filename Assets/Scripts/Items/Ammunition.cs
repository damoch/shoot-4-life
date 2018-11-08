using Assets.Scripts.Actors;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class Ammunition : Item
    {
        [SerializeField]
        private float _speed;

        [SerializeField]
        private int _damageValue;

        private float _step;

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

        private void Update()
        {
            _step = _speed * Time.deltaTime;
            transform.Translate(Vector2.left * _step);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var gObject = collision.gameObject;

            var actor = gObject.GetComponent<Actor>();
            if (actor != null)
            {
                actor.HealthPoints -= _damageValue;
            }

            Destroy(gameObject);
        }
    }
}
