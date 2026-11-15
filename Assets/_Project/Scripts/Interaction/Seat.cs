using ProjectAllUnits.Player;
using UnityEngine;

namespace ProjectAllUnits.Interaction
{
    /// <summary>A chair any player can sit in and stand up from.</summary>
    public class Seat : MonoBehaviour, IInteractable
    {
        [SerializeField] Transform seatPoint;
        [SerializeField] Transform standPoint;

        public virtual string Prompt => "Sit";

        public virtual void Interact(PlayerInteractor player)
        {
            player.Controller.SitAt(seatPoint);
            player.OnSeated(this);
            OnSeated(player);
        }

        public void Leave(PlayerInteractor player)
        {
            OnLeaving(player);
            player.Controller.StandUp(standPoint);
            player.OnLeftSeat();
        }

        protected virtual void OnSeated(PlayerInteractor player) { }
        protected virtual void OnLeaving(PlayerInteractor player) { }
    }
}
