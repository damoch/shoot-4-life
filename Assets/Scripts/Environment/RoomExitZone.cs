using Assets.Scripts.Actors;
using UnityEngine;
namespace Assets.Scripts.Environment
{
    public class RoomExitZone : MonoBehaviour
    {
        private Room _currentRoom;

        [SerializeField]
        private Room _leadsTo;

        private void Start()
        {
            _currentRoom = GetComponentInParent<Room>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var actor = col.gameObject.GetComponentInParent<Actor>();
            if(actor)
            {
                _currentRoom.ActorIsLeavingTheRoomTo(actor, _leadsTo);
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            var actor = col.gameObject.GetComponentInParent<Actor>();
            if (actor)
            {
                _currentRoom.ActorLeftTheExitZone(actor);
            }
        }
    }
}