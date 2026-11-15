using Unity.Netcode.Components;
using UnityEngine;

namespace ProjectAllUnits.Networking
{
    /// <summary>NetworkTransform where the owning client, not the server, has authority.</summary>
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
