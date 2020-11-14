using UnityEngine;
using Assets.Scripts.Actors;
using System.Collections.Generic;
using Assets.Scripts.Controllers;
using System.Linq;

namespace Assets.Scripts.Environment
{
    public class Room : MonoBehaviour
    {
        [SerializeField]
        private List<Actor> _actorsInRoom;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("agent enter");
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

        private void OnTriggerExit2D(Collider2D collision)
        {
            var actor = collision.gameObject.transform.parent?.GetComponent<Actor>();
            if (actor != null)
            {
               if (_actorsInRoom == null)
               {
                   _actorsInRoom = new List<Actor>();
                   return;
               }
               //_actorsInRoom.ForEach(x => x.NewActorInCurrentRoom(actor));
               foreach (var act in _actorsInRoom)
               {
                   if (act.ActorType == Enums.ActorType.Playable)
                   {
                       continue;
                   }

                   var actorController = act.gameObject.GetComponent<NonPlayableZombieActorController>();
                   actorController.NotifyAboutActorLeftTheRoom(actor);
               }

               _actorsInRoom.Remove(actor);
               return;
            }
        }
    }
}
