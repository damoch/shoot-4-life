using UnityEngine;
using Assets.Scripts.Actors;
using System.Collections.Generic;
using Assets.Scripts.Controllers;
using System.Linq;
using System;

namespace Assets.Scripts.Environment
{
    public class Room : MonoBehaviour
    {
        [SerializeField]
        private List<Actor> _actorsInRoom;

        [SerializeField]
        private List<Room> _neighbouringRooms;

        private Dictionary<Actor, Room> _actorsLeavingRoomTo;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var actor = collision.gameObject.transform.parent?.GetComponent<Actor>();
            if(actor != null)
            {
                if(_actorsInRoom == null)
                {
                    _actorsInRoom = new List<Actor> { actor };
                    return;
                }
                if(_actorsInRoom.Contains(actor))
                {
                    Debug.LogError($"??? ok, but like, you {actor.gameObject.name} already are in {gameObject.name}?");
                    return;
                }

                foreach(var act in _actorsInRoom)
                {
                    if(act.ActorType == Enums.ActorType.Playable)
                    {
                        continue;
                    }

                    var actorController = act.gameObject.GetComponent<NonPlayableZombieActorController>();
                    actorController.NotifyAboutNewActorInTheRoom(actor);
                }

                _actorsInRoom.Add(actor);
                Debug.Log($"{actor.gameObject.name} has entered room {gameObject.name}");
            }

            var zombieController = collision.gameObject.transform.parent?.GetComponent<NonPlayableZombieActorController>();
            if(zombieController != null)
            {
                var enemies = _actorsInRoom.Where(x => x.Team != zombieController.Team && x.IsAlive).ToList();//zombie will attack anyone, who is not in their team
                if(enemies.Count == 0)
                {
                    return;
                }
                foreach (var enemy in enemies)
                {
                    zombieController.NotifyAboutNewActorInTheRoom(enemy);                    
                }
            }
        }

        internal void ActorIsLeavingTheRoomTo(Actor actor, Room room)
        {
            if(_actorsLeavingRoomTo == null)
            {
                _actorsLeavingRoomTo = new Dictionary<Actor, Room>();
            }
            if(!_actorsInRoom.Contains(actor))
            {
                return;
            }
            if(_actorsLeavingRoomTo.ContainsKey(actor)){
                _actorsLeavingRoomTo[actor] = room;
                Debug.Log($"{actor.name} is in the exit zone of {gameObject.name}");
                return;
            }
            _actorsLeavingRoomTo.Add(actor, room);

            Debug.Log($"{actor.name} is in the exit zone of {gameObject.name}");
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var actor = collision.gameObject.transform.parent?.GetComponent<Actor>();
            if (actor != null)
            {
                //https://answers.unity.com/questions/410711/trigger-in-child-object-calls-ontriggerenter-in-pa.html
                if (_actorsInRoom == null)
                {
                    _actorsInRoom = new List<Actor>();
                    return;
                }
                if(!_actorsInRoom.Contains(actor))
                {
                    Debug.LogError($"What in the actual fuck, {actor.gameObject.name} you are NOT in this {gameObject.name}!");
                    return;

                }
                Room actorsNewRoom = null;
                if (!_actorsLeavingRoomTo.ContainsKey(actor))
                {
                    Debug.LogError($"Something is wrong, {actor.gameObject.name} should be in leaving {gameObject.name} actors list... IDK fix it");
                    return;
                }
                else
                {
                    actorsNewRoom = _actorsLeavingRoomTo[actor];
                }
                foreach (var act in _actorsInRoom)
                {
                    if (act.ActorType == Enums.ActorType.Playable)
                    {
                        continue;
                    }

                    var actorController = act.gameObject.GetComponent<NonPlayableZombieActorController>();
                    actorController.NotifyAboutActorLeftTheRoom(actor, actorsNewRoom);
                }


                if (_actorsLeavingRoomTo.ContainsKey(actor))
                {
                    _actorsLeavingRoomTo.Remove(actor);
                }
                _actorsInRoom.Remove(actor);
                Debug.Log($"Actor {actor.gameObject.name} left to {actorsNewRoom?.gameObject.name}");
                return;
            }
        }
    }
}
