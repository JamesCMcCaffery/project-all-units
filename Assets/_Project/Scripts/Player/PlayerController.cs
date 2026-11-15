using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectAllUnits.Player
{
    /// <summary>First-person walking and looking, shared by every role.</summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] Camera playerCamera;
        [SerializeField] float walkSpeed = 4f;
        [SerializeField] float sprintSpeed = 7f;
        [SerializeField] float lookSensitivity = 0.1f;
        [SerializeField] float gravity = -20f;

        CharacterController body;
        float pitch;
        float verticalVelocity;

        public Camera Camera => playerCamera;
        public bool IsSeated { get; private set; }

        void Awake() => body = GetComponent<CharacterController>();

        public override void OnNetworkSpawn()
        {
            playerCamera.gameObject.SetActive(IsOwner);
            if (IsOwner) Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            if (!IsOwner) return;
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null) return;

            if (keyboard.escapeKey.wasPressedThisFrame) ToggleCursorLock();
            if (Cursor.lockState == CursorLockMode.Locked) Look(mouse);
            if (!IsSeated) Move(keyboard);
        }

        public void SitAt(Transform seatPoint)
        {
            IsSeated = true;
            body.enabled = false;
            transform.SetPositionAndRotation(seatPoint.position, seatPoint.rotation);
            pitch = 0f;
        }

        public void StandUp(Transform standPoint)
        {
            transform.SetPositionAndRotation(standPoint.position, standPoint.rotation);
            body.enabled = true;
            IsSeated = false;
        }

        void ToggleCursorLock()
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                ? CursorLockMode.None
                : CursorLockMode.Locked;
        }

        void Look(Mouse mouse)
        {
            Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;
            transform.Rotate(0f, delta.x, 0f);
            pitch = Mathf.Clamp(pitch - delta.y, -85f, 85f);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }

        void Move(Keyboard keyboard)
        {
            Vector2 input = new(
                (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
            float speed = keyboard.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;
            Vector3 velocity = (transform.right * input.x + transform.forward * input.y).normalized * speed;

            // A small constant downward pull keeps isGrounded reliable on slopes and steps.
            verticalVelocity = body.isGrounded ? -1f : verticalVelocity + gravity * Time.deltaTime;
            velocity.y = verticalVelocity;
            body.Move(velocity * Time.deltaTime);
        }
    }
}
