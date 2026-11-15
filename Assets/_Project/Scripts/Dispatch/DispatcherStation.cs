using Unity.Netcode;
using UnityEngine;

namespace ProjectAllUnits.Dispatch
{
    /// <summary>The dispatcher's seated player object, showing the desk camera and UI to its owner.</summary>
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
