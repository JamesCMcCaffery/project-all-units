using ProjectAllUnits.Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

namespace ProjectAllUnits.Player
{
    /// <summary>Shows the character model that matches the player's role, and hides your own body from your first-person camera.</summary>
    [RequireComponent(typeof(PlayerRoleState))]
    public class PlayerAppearance : NetworkBehaviour
    {
        [SerializeField] GameObject officerModel;
        [SerializeField] GameObject dispatcherModel;

        PlayerRoleState roleState;

        void Awake() => roleState = GetComponent<PlayerRoleState>();

        public override void OnNetworkSpawn()
        {
            roleState.RoleChanged += ShowModelFor;
            ShowModelFor(roleState.Role);
        }

        public override void OnNetworkDespawn() => roleState.RoleChanged -= ShowModelFor;

        void ShowModelFor(PlayerRole role)
        {
            officerModel.SetActive(role == PlayerRole.Officer);
            dispatcherModel.SetActive(role == PlayerRole.Dispatcher);
            if (IsOwner) HideOwnBody();
        }

        // Shadows-only keeps your shadow on the floor without the camera clipping through your own head.
        void HideOwnBody()
        {
            foreach (var model in new[] { officerModel, dispatcherModel })
            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
                renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }
}
