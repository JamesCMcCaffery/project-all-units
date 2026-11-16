using UnityEngine;

namespace ProjectAllUnits.Player
{
    /// <summary>Drives a character model's Animator from how fast it is actually moving, so remote players animate without any extra network data.</summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimator : MonoBehaviour
    {
        static readonly int SpeedParam = Animator.StringToHash("Speed");

        [SerializeField] float speedDampTime = 0.1f;
        [Tooltip("Ignore one-frame jumps faster than this, such as snapping into a seat.")]
        [SerializeField] float maxPlausibleSpeed = 12f;

        Animator animator;
        Vector3 lastPosition;

        void Awake()
        {
            animator = GetComponent<Animator>();
            animator.applyRootMotion = false;
        }

        void OnEnable() => lastPosition = transform.position;

        void Update()
        {
            Vector3 delta = transform.position - lastPosition;
            lastPosition = transform.position;
            delta.y = 0f;

            float speed = Time.deltaTime > 0f ? delta.magnitude / Time.deltaTime : 0f;
            if (speed > maxPlausibleSpeed) speed = 0f;
            animator.SetFloat(SpeedParam, speed, speedDampTime, Time.deltaTime);
        }
    }
}
