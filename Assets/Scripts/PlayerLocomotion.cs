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

        private Transform _myTransform;
        private AnimatorHandler _animatorHandler;
        private Rigidbody _rigidbody;
        private GameObject _normalCamera;

        private void Awake()
        {
            // Set up references to other components
            _rigidbody = GetComponent<Rigidbody>();
            _inputHandler = GetComponent<InputHandler>();
            _animatorHandler = GetComponentInChildren<AnimatorHandler>();
            _cameraObject = Camera.main.transform;
            _myTransform = transform;
        }

        private void Update()
        {
            // Calculate the player's movement direction based on input and the camera's forward and right vectors
            _moveDirection = _cameraObject.forward * _inputHandler.Vertical;
            _moveDirection += _cameraObject.right * _inputHandler.Horizontal;
            _moveDirection.Normalize();

            // Calculate the player's velocity based on the movement direction and movement speed
            _moveDirection *= movementSpeed;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, Vector3.up);
            _rigidbody.velocity = projectedVelocity;

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
            if (targetDirection == Vector3.zero)
            {
                targetDirection = _myTransform.forward;
            }

            // Calculate the target rotation based on the target direction and smoothly interpolate towards it
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            _myTransform.rotation = Quaternion.Slerp(_myTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}