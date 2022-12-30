using UnityEngine;

namespace Bone
{
    public class AnimatorHandler : MonoBehaviour
    {
        private const float AnimationTransitionDuration = 0.2f;
        private const float AnimationDampTime = 0.1f;

        // Public variables
        public Animator animator;
        public InputHandler inputHandler;
        public PlayerLocomotion playerLocomotion;
        public bool canRotate;

        // Private variables
        private int _verticalHash;
        private int _horizontalHash;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            _verticalHash = Animator.StringToHash("Vertical");
            _horizontalHash = Animator.StringToHash("Horizontal");
        }

        public void PlayTargetAnimation(string animation, bool isInteracting)
        {
            animator.applyRootMotion = isInteracting;
            animator.SetBool("isInteracting", isInteracting);
            animator.CrossFade(animation, AnimationTransitionDuration);
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            animator.SetFloat(_verticalHash, verticalMovement, AnimationDampTime, Time.deltaTime);
            animator.SetFloat(_horizontalHash, horizontalMovement, AnimationDampTime, Time.deltaTime);
        }

        public void EnableRotation() { canRotate = true; }

        public void DisableRotation() { canRotate = false; }

        private void OnAnimatorMove()
        {
            if (!inputHandler.isInteracting) return;
            
            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
        
            Vector3 velocity = deltaPosition / Time.deltaTime;
            playerLocomotion.rigidbody.velocity = velocity;
        }
    }   
}