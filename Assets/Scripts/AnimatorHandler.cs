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
            animator.SetFloat(_verticalHash, verticalMovement, 0.1f, Time.deltaTime);
            animator.SetFloat(_horizontalHash, horizontalMovement, 0.1f, Time.deltaTime);
        }

        public void EnableRotation() { canRotate = true; }

        public void DisableRotation() { canRotate = false; }

    }
}