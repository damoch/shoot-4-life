using UnityEngine;

namespace Assets.Scripts.Items
{
    public class Ammunition : Item
    {
        [SerializeField]
        private float _speed;

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
    }
}
