using UnityEngine;

namespace Bone
{
    public class PlayerLocomotion : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float fallSpeed = 45;

        [Header("Ground and Air")]
        [SerializeField]
        float groundDetectionRayStart = 0.3f;
        [SerializeField]
        float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField]
        float groundDirectionRayDistance = 0.2f;
        [SerializeField]
        LayerMask ignoreForGroundCheck;
        [SerializeField]
        public float timeToLand = 0.5f;

        public float inAirTimer;
        public Vector3 moveDirection;
        public new Rigidbody rigidbody;

        private Transform _cameraObject;
        private InputHandler _inputHandler;
        private Transform _transform;
        private AnimatorHandler _animatorHandler;
        private PlayerManager _playerManager;

        private Vector3 _normalVector;
        private Vector3 _targetPosition;

        private void Awake()
        {
            // Set up references to other components
            rigidbody = GetComponent<Rigidbody>();
            _playerManager = GetComponent<PlayerManager>();
            _inputHandler = GetComponent<InputHandler>();
            _animatorHandler = GetComponentInChildren<AnimatorHandler>();
            _cameraObject = Camera.main.transform;
            _transform = transform;
        }

        private void Start()
        {
            _playerManager.isGrounded = true;

        }

        public void HandleMovement(float delta)
        {
            if (_inputHandler.rollFlag) return;

            if (_playerManager.isInteracting) return;

            // Calculate the player's movement direction based on input and the camera's forward and right vectors
            moveDirection = _cameraObject.forward * _inputHandler.Vertical;
            moveDirection += _cameraObject.right * _inputHandler.Horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            // Calculate the player's velocity based on the movement direction and movement speed
            if (_inputHandler.sprintFlag)
            {
                _playerManager.isSprinting = true;
                moveDirection *= sprintSpeed;
            }
            else
            {
                moveDirection *= movementSpeed;
            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, _normalVector);
            rigidbody.velocity = projectedVelocity;

            // Update the AnimatorHandler component with the playe r's movement amount
            _animatorHandler.UpdateAnimatorValues(_inputHandler.MoveAmount, 0, _playerManager.isSprinting, delta);

            // Rotate the player's character model towards the input direction if allowed by the AnimatorHandler component
            if (_animatorHandler.canRotate) { HandleRotation(delta); }
        }

        private void HandleRotation(float delta)
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
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, rotationSpeed * delta);
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (_playerManager.isInteracting || !_inputHandler.rollFlag) { return; }

            moveDirection = _cameraObject.forward * _inputHandler.Vertical;
            moveDirection += _cameraObject.right * _inputHandler.Horizontal;

            bool isMoving = _inputHandler.MoveAmount > 0;

            if (isMoving)
            {
                _animatorHandler.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                _transform.rotation = rollRotation;
            }
            else
            {
                _animatorHandler.UpdateAnimatorValues(300, 0, false, delta);
                _animatorHandler.PlayTargetAnimation("Backstep", true);
            }

            _inputHandler.rollFlag = false;
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            _playerManager.isGrounded = false;

            RaycastHit hit;
            Vector3 origin = _transform.position;
            origin.y = groundDetectionRayStart;

            if (Physics.Raycast(origin, _transform.forward, out hit))
            {
                moveDirection = Vector3.zero;
            }

            if (_playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallSpeed);
                rigidbody.AddForce(moveDirection * fallSpeed / 5f);
            }

            Vector3 direction = moveDirection;
            direction.Normalize();
            origin += direction * groundDirectionRayDistance;

            _targetPosition = _transform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);

            bool grounded = Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ~ignoreForGroundCheck);

            if (grounded)
            {
                _normalVector = hit.normal;
                Vector3 tp = hit.point;
                _playerManager.isGrounded = true;
                _targetPosition.y = tp.y;

                bool shouldLand = inAirTimer > timeToLand;

                if (shouldLand) { _animatorHandler.PlayTargetAnimation("Land", true); }
                else { _animatorHandler.PlayTargetAnimation("Locomotion", false); }

                inAirTimer = 0;
                _playerManager.isInAir = false;
            }
            else
            {
                if (_playerManager.isGrounded) { _playerManager.isGrounded = false; }
                if (!_playerManager.isInAir)
                {
                    if (!_playerManager.isInteracting) { _animatorHandler.PlayTargetAnimation("Falling", true); }
                    Vector3 velocity = rigidbody.velocity;
                    velocity.Normalize();
                    rigidbody.velocity = velocity * (movementSpeed / 2);
                    _playerManager.isInAir = true;
                }
            }

            if (_playerManager.isGrounded)
            {
                if (_playerManager.isInteracting || _inputHandler.MoveAmount > 0)
                {
                    _transform.position = Vector3.Lerp(_transform.position, _targetPosition, delta);
                }
                else { _transform.position = _targetPosition; }
            }

        }
    }
}