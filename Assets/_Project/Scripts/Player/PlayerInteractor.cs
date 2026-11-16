using ProjectAllUnits.Interaction;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectAllUnits.Player
{
    /// <summary>Lets the local player use whatever interactable they are looking at by pressing E.</summary>
    public class PlayerInteractor : NetworkBehaviour
    {
        [SerializeField] PlayerController controller;
        [SerializeField] float reach = 2.5f;
        [SerializeField] LayerMask interactableLayers = ~0;

        Seat currentSeat;

        public PlayerController Controller => controller;
        public IInteractable Target { get; private set; }

        void Update()
        {
            if (!IsOwner) return;
            Target = currentSeat == null ? FindTarget() : null;

            if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame) return;
            if (currentSeat != null) currentSeat.Leave(this);
            else Target?.Interact(this);
        }

        public void OnSeated(Seat seat) => currentSeat = seat;
        public void OnLeftSeat() => currentSeat = null;

        IInteractable FindTarget()
        {
            Transform eye = controller.Camera.transform;
            var raycastCollided = Physics.Raycast(eye.position, eye.forward, out RaycastHit hit, reach, interactableLayers);
            if (!raycastCollided)
                return null;
            return hit.collider.GetComponentInParent<IInteractable>();
        }
    }
}
