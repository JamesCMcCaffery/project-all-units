using Unity.Netcode;
using UnityEngine;

namespace ProjectAllUnits.Dispatch
{
    /// <summary>
    /// The dispatcher's player object. Seated at the desk: no walking, mouse is free
    /// for the UI. Step 2 grows this into calls, lookups, the map and camera feeds.
    /// Prefab: NetworkObject + this, with a child Camera framing the desk monitors
    /// and a child Canvas (deskUI) for the dispatch screens.
    /// </summary>
    public class DispatcherStation : NetworkBehaviour
    {
        [SerializeField] Camera deskCamera;
        [SerializeField] GameObject deskUI;

        public override void OnNetworkSpawn()
        {
            deskCamera.gameObject.SetActive(IsOwner);
            if (deskUI != null) deskUI.SetActive(IsOwner);
            if (IsOwner) Cursor.lockState = CursorLockMode.None;
        }
    }
}
