using Assets.Scripts.Actors;
using Assets.Scripts.Enums;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(Actor))]
    public class NonPlayableActorController : MonoBehaviour
    {
        [SerializeField]
        private Actor _actor;

        [SerializeField]
        private Team _team;

        private Actor _target;

        private void Start()
        {
            _actor = GetComponent<Actor>();
            _actor.Team = _team;
            _actor.IsSelected = true;
        }

        private void Update()
        {
            if (!_actor.IsAlive)
            {
                return;
            }

            if(_target == null || !_target.IsAlive)
            {
                FindNewTarget();
            }

            _actor.LookAt(_target.transform.position);
            _actor.MoveTowards(_target.transform.position);
        }

        private void FindNewTarget()
        {
            _target = FindObjectsOfType<Actor>().FirstOrDefault(x => x.Team != _team && x.IsAlive);
        }
    }
}
