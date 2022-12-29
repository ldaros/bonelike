using UnityEngine;

namespace Bone
{
    public class AnimatorHandler : MonoBehaviour
    {
        // Public variables
        public Animator animator;
        public bool canRotate;

        // Private variables
        private int _verticalHash;
        private int _horizontalHash;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            _verticalHash = Animator.StringToHash("Vertical");
            _horizontalHash = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            float clampedVerticalMovement = ClampMovement(verticalMovement);
            float clampedHorizontalMovement = ClampMovement(horizontalMovement);

            animator.SetFloat(_verticalHash, clampedVerticalMovement, 0.1f, Time.deltaTime);
            animator.SetFloat(_horizontalHash, clampedHorizontalMovement, 0.1f, Time.deltaTime);
        }

        private float ClampMovement(float movement)
        {
            // Clamp the movement value to the range -1 to 1, with a special case for values between -0.55 and 0.55
            if (movement > 0 && movement < 0.55f) return 0.5f;
            if (movement > 0.55f) return 1;
            if (movement < 0 && movement > -0.55f) return -0.5f;
            if (movement < -0.55f) return -1;

            return 0;
        }

        public void EnableRotation() { canRotate = true; }

        public void DisableRotation() { canRotate = false; }

    }
}