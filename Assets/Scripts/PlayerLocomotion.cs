using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bone
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerLocomotion : MonoBehaviour
    {
        [Header("Stats")]
        public float movementSpeed = 5;
        public float sprintSpeed = 7;
        public float rotationSpeed = 10;
        public float turnSmoothTime = 0.1f;

        [Header("Physics")]
        public float fallSpeed = 45;
        public float distanceToFall = 1f;
        public LayerMask groundLayers;
        public float airTime = 0f;

        [Header("Actions")]
        public float rollVelocity = 0.1f;
        public float rollDuration = 0.5f;
        public float backstepVelocity = 0.1f;
        public float backstepDuration = 0.3f;
        public float jumpHeight = 0.5f;
        public float jumpTime = 0.5f;

        private Transform _transform;
        private InputHandler _inputHandler;
        private AnimatorHandler _animatorHandler;
        private PlayerManager _playerManager;
        private CameraHandler _cameraHandler;
        private CharacterController _characterController;
        private float _turnSmoothVelocity;
        private Vector3 _moveDirection;

        private void Awake()
        {
            _playerManager = GetComponent<PlayerManager>();
            _inputHandler = GetComponent<InputHandler>();
            _cameraHandler = GetComponent<CameraHandler>();
            _characterController = GetComponent<CharacterController>();
            _animatorHandler = GetComponentInChildren<AnimatorHandler>();
            _transform = transform;
        }
        public void HandleMovement(float delta)
        {
            if (_playerManager.isInteracting) return;
            if (_inputHandler.rollFlag) return;

            float horizontal = _inputHandler.Horizontal;
            float vertical = _inputHandler.Vertical;

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            bool isMoving = direction.magnitude >= 0.1f;

            float speed;

            if (_inputHandler.sprintFlag)
            {
                _playerManager.isSprinting = true;
                speed = sprintSpeed;
            }
            else { speed = movementSpeed; }

            if (isMoving)
            {
                float cameraYRotation = _cameraHandler.mainCamera.eulerAngles.y;

                // Calculate the target angle for the character to face, and rotate towards it
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraYRotation;

                float angle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
                _transform.rotation = Quaternion.Euler(0f, angle, 0f);


                // Calculate the move direction based on the camera y-rotation, and the input direction
                _moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                _characterController.Move(_moveDirection.normalized * speed * delta);
            }

            _animatorHandler.UpdateAnimatorValues(_inputHandler.MoveAmount, 0, _playerManager.isSprinting, delta);
        }

        public void HandleFalling(float delta)
        {
            bool isGrounded = Physics.Raycast(_transform.position, Vector3.down, 0.1f, groundLayers);
            _playerManager.isGrounded = isGrounded;

            if (!isGrounded)
            {
                if (airTime >= distanceToFall) { _animatorHandler.PlayTargetAnimation("Falling", true); }
                _playerManager.isInAir = true;
                airTime += delta;

                _characterController.Move(_moveDirection.normalized * movementSpeed * delta + Vector3.down * fallSpeed * delta);
            }
            else
            {
                if (airTime >= distanceToFall)
                {
                    _animatorHandler.PlayTargetAnimation("Land", true);
                }
                _playerManager.isInAir = false;
                airTime = 0f;
            }

            Color rayColor = isGrounded ? Color.green : Color.red;
            Debug.DrawRay(_transform.position, Vector3.down, rayColor, distanceToFall);
        }


        public void HandleRolling(float delta)
        {
            if (_playerManager.isInteracting || !_inputHandler.rollFlag) { return; }

            bool isMoving = _inputHandler.MoveAmount > 0;

            if (isMoving) { StartCoroutine(RollingCo(delta)); }
            else { StartCoroutine(BackstepCo(delta)); }

            _inputHandler.rollFlag = false;
        }


        private IEnumerator RollingCo(float delta)
        {
            _animatorHandler.PlayTargetAnimation("Rolling", true);
            float startTime = delta;

            while (delta < startTime + rollDuration)
            {
                delta += Time.deltaTime;
                _characterController.Move(_moveDirection.normalized * rollVelocity * delta);
                yield return null;
            }
        }

        private IEnumerator BackstepCo(float delta)
        {
            _animatorHandler.PlayTargetAnimation("Backstep", true);
            float startTime = delta;

            while (delta < startTime + backstepDuration)
            {
                delta += Time.deltaTime;
                _characterController.Move(-_moveDirection.normalized * backstepVelocity * delta);
                yield return null;
            }

        }

        public void HandleJumping(float delta)
        {
            if (_playerManager.isInteracting || !_inputHandler.jumpFlag || !_playerManager.isGrounded) return;

            StartCoroutine(JumpingCo(delta));
            _inputHandler.jumpFlag = false;
        }

        private IEnumerator JumpingCo(float delta)
        {
            _animatorHandler.PlayTargetAnimation("Jumping", true);
            float startTime = delta;

            while (delta < startTime + jumpTime)
            {
                delta += Time.deltaTime;
                Vector3 jumpDirection =  Vector3.up * jumpHeight * delta;
                _characterController.Move(jumpDirection);
                yield return null;
            }
        }

    }
}