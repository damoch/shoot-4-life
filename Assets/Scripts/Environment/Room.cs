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
                //_actorsInRoom.ForEach(x => x.NewActorInCurrentRoom(actor));
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
                Debug.Log($"{actor.name} is leaving the room");
                return;
            }
            _actorsLeavingRoomTo.Add(actor, room);

            Debug.Log($"{actor.name} is leaving the room");
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var actor = collision.gameObject.transform.parent?.GetComponent<Actor>();
            if (actor != null)
            {
                Debug.Break();
                //https://answers.unity.com/questions/410711/trigger-in-child-object-calls-ontriggerenter-in-pa.html
                if (_actorsInRoom == null)
                {
                    _actorsInRoom = new List<Actor>();
                    return;
                }
                Room actorsNewRoom = null;
                if (!_actorsLeavingRoomTo.ContainsKey(actor))
                {
                    Debug.LogWarning($"Something is wrong, {actor.gameObject.name} should be in leaving actors list... IDK fix it");
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

                    Debug.Log($"Actor {actor.gameObject.name} left to {actorsNewRoom?.gameObject.name}");
                }

                if (_actorsLeavingRoomTo.ContainsKey(actor))
                {
                    _actorsLeavingRoomTo.Remove(actor);
                }
                _actorsInRoom.Remove(actor);
                return;
            }
        }
    }
}
