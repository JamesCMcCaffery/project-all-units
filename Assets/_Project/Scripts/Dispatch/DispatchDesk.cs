using ProjectAllUnits.Core;
using ProjectAllUnits.Interaction;
using ProjectAllUnits.Player;
using UnityEngine;

namespace ProjectAllUnits.Dispatch
{
    /// <summary>The dispatch desk chair; when the dispatcher sits here they switch to the desk screens.</summary>
    public class DispatchDesk : Seat
    {
        [SerializeField] Camera deskCamera;
        [SerializeField] GameObject deskUI;

        public override string Prompt => "Sit at dispatch desk";

        void Awake() => ShowDesk(false);

        protected override void OnSeated(PlayerInteractor player)
        {
            if (!IsDispatcher(player)) return;
            player.Controller.Camera.enabled = false;
            ShowDesk(true);
            Cursor.lockState = CursorLockMode.None;
        }

        protected override void OnLeaving(PlayerInteractor player)
        {
            if (!IsDispatcher(player)) return;
            ShowDesk(false);
            player.Controller.Camera.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        static bool IsDispatcher(PlayerInteractor player) =>
            player.GetComponent<PlayerRoleState>().Role == PlayerRole.Dispatcher;

        void ShowDesk(bool visible)
        {
            deskCamera.gameObject.SetActive(visible);
            if (deskUI != null) deskUI.SetActive(visible);
        }
    }
}
