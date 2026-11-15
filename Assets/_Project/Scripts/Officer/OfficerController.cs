using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectAllUnits.Officer
{
    /// <summary>
    /// First-person officer movement. Owner-driven; pair with ClientNetworkTransform.
    /// Prefab: CharacterController + NetworkObject + ClientNetworkTransform + this,
    /// with a child Camera (assigned to playerCamera) at eye height.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class OfficerController : NetworkBehaviour
    {
        [SerializeField] Camera playerCamera;
        [SerializeField] float walkSpeed = 4f;
        [SerializeField] float sprintSpeed = 7f;
        [SerializeField] float lookSensitivity = 0.1f;
        [SerializeField] float gravity = -20f;

        CharacterController body;
        float pitch;
        float verticalVelocity;

        void Awake() => body = GetComponent<CharacterController>();

        public override void OnNetworkSpawn()
        {
            // Only the local player's camera renders; remote officers are just bodies.
            playerCamera.gameObject.SetActive(IsOwner);
            if (IsOwner) Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (!IsOwner) return;
            var kb = Keyboard.current;
            var mouse = Mouse.current;
            if (kb == null || mouse == null) return;

            if (kb.escapeKey.wasPressedThisFrame)
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                    ? CursorLockMode.None : CursorLockMode.Locked;

            // Look
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Vector2 look = mouse.delta.ReadValue() * lookSensitivity;
                transform.Rotate(0f, look.x, 0f);
                pitch = Mathf.Clamp(pitch - look.y, -85f, 85f);
                playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
            }

            // Move
            Vector2 input = new(
                (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0),
                (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0));
            float speed = kb.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;
            Vector3 move = (transform.right * input.x + transform.forward * input.y).normalized * speed;

            verticalVelocity = body.isGrounded ? -1f : verticalVelocity + gravity * Time.deltaTime;
            move.y = verticalVelocity;
            body.Move(move * Time.deltaTime);
        }
    }
}
