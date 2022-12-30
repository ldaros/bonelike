using UnityEngine;

namespace Bone
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;

        [SerializeField]
        float rotationSpeed = 10;

        private Transform _cameraObject;
        private InputHandler _inputHandler;
        private Vector3 _moveDirection;
        public new Rigidbody rigidbody;

        private Transform _transform;
        private AnimatorHandler _animatorHandler;
        private Animator _animator;
        private GameObject _normalCamera;

        private void Awake()
        {
            // Set up references to other components
            rigidbody = GetComponent<Rigidbody>();
            _inputHandler = GetComponent<InputHandler>();
            _animatorHandler = GetComponentInChildren<AnimatorHandler>();
            _animator = GetComponentInChildren<Animator>();
            _cameraObject = Camera.main.transform;
            _transform = transform;
        }

        private void Update()
        {
            HandleMovement();
            HandleRollingAndSprinting();
        }

        private void HandleMovement()
        {
            // Calculate the player's movement direction based on input and the camera's forward and right vectors
            _moveDirection = _cameraObject.forward * _inputHandler.Vertical;
            _moveDirection += _cameraObject.right * _inputHandler.Horizontal;
            _moveDirection.Normalize();
            _moveDirection.y = 0;

            // Calculate the player's velocity based on the movement direction and movement speed
            _moveDirection *= movementSpeed;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, Vector3.up);
            rigidbody.velocity = projectedVelocity;

            // Update the AnimatorHandler component with the player's movement amount
            _animatorHandler.UpdateAnimatorValues(_inputHandler.MoveAmount, 0);

            // Rotate the player's character model towards the input direction if allowed by the AnimatorHandler component
            if (_animatorHandler.canRotate) { HandleRotation(); }
        }

        private void HandleRotation()
        {
            // Calculate the target direction based on input
            Vector3 targetDirection = Vector3.zero;
            targetDirection = _cameraObject.forward * _inputHandler.Vertical;
            targetDirection += _cameraObject.right * _inputHandler.Horizontal;

            // Normalize the target direction and set its y component to 0 to prevent the character from tilting
            targetDirection.Normalize();
            targetDirection.y = 0;

            // If the target direction is zero, use the current forward direction instead
            if (targetDirection == Vector3.zero) { targetDirection = _transform.forward; }

            // Calculate the target rotation based on the target direction and smoothly interpolate towards it
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        private void HandleRollingAndSprinting()
        {
            if (_animator.GetBool("isInteracting") || !_inputHandler.rollFlag) { return; }

            _moveDirection = _cameraObject.forward * _inputHandler.Vertical;
            _moveDirection += _cameraObject.right * _inputHandler.Horizontal;

            bool isMoving = _inputHandler.MoveAmount > 0;
                
            if (isMoving)
            {
                _animatorHandler.PlayTargetAnimation("Rolling", true);
                _moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(_moveDirection);
                _transform.rotation = rollRotation;
            }
            else
            {
                _animatorHandler.PlayTargetAnimation("Backstep", true);
            }

            _inputHandler.rollFlag = false;
        }
    }
}