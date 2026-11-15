using Unity.Netcode.Components;
using UnityEngine;

namespace ProjectAllUnits.Networking
{
    /// <summary>
    /// Owner-authoritative NetworkTransform: the owning client moves its own body
    /// and the result is replicated to everyone else. Responsive, and what most
    /// co-op games (Lethal Company included) do for player movement.
    /// </summary>
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
