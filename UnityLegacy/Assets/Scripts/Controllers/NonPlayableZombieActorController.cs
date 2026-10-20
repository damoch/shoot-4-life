using Assets.Scripts.Actors;
using Assets.Scripts.Enums;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using Assets.Scripts.Environment;

namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(Actor))]
    public class NonPlayableZombieActorController : MonoBehaviour
    {
        [SerializeField]
        private Actor _actor;

        [SerializeField]
        private Team _team;

        private Actor _target;
        private List<Actor> _potentialTargets;

        public Team Team { get => _team; }

        private void Start()
        {
            _actor = GetComponent<Actor>();
            _actor.MoveTowards(Vector2.zero);
            _actor.Team = _team;
            _potentialTargets = new List<Actor>();
        }

        private void Update()
        {
            if (!_actor.IsAlive)
            {
                return;
            }

            if (!_actor.IsSelected)
            {
                _actor.IsSelected = true;
            }

            if (_target == null)
            {
                if(!FindNewTarget())
                {
                    return;
                }
            }

            if (!_target.IsAlive)
            {
                if(!FindNewTarget())
                {
                    _actor.MoveTowards(Vector2.zero);
                    return;
                }
            }

            _actor.LookAt(_target.transform.position);
            _actor.MoveTowards(_target.transform.position);

            if(_actor.Weapon.IsAttackPossible(_target.transform.position))
            {
                _actor.GetCommand(Commands.Shoot);
            }
        }

        internal void NotifyAboutNewActorInTheRoom(Actor actor)
        {
            if(actor.Team != _team)
            {
                if(_target == null) 
                {
                    _target = actor;
                    return;
                }
                if (!_potentialTargets.Contains(actor))
                {
                    _potentialTargets.Add(actor);
                }
            }
        }

        internal void NotifyAboutActorLeftTheRoom(Actor actor, Room leftTo)
        {
            if(_potentialTargets.Contains(actor))
            {
                _potentialTargets.Remove(actor);
            }
            if(_target == actor)
            {
                FindNewTarget();
            }
        }

        private bool FindNewTarget()
        {
            _target = _potentialTargets.FirstOrDefault(x => x.IsAlive);

            if(_target == null)
            {
                _potentialTargets.Clear();
                return false;
            }
            return true;            
        }
    }
}
