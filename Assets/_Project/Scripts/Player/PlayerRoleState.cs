using ProjectAllUnits.Core;
using Unity.Netcode;

namespace ProjectAllUnits.Player
{
    /// <summary>Holds a player's role, set by the server and readable by everyone.</summary>
    public class PlayerRoleState : NetworkBehaviour
    {
        readonly NetworkVariable<PlayerRole> role = new(PlayerRole.Officer);

        public PlayerRole Role => role.Value;

        public void AssignRole(PlayerRole newRole)
        {
            if (IsServer) role.Value = newRole;
        }
    }
}
