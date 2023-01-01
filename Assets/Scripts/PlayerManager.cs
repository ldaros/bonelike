using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bone
{
    public class PlayerManager : MonoBehaviour
    {
        Animator animator;

        [Header("Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isJumping;
        public bool isInAir;
        public bool isGrounded;

        private CameraHandler _cameraHandler;
        private InputHandler _inputHandler;
        private PlayerLocomotion _playerLocomotion;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            _playerLocomotion = GetComponent<PlayerLocomotion>();
            _cameraHandler = GetComponent<CameraHandler>();
            _inputHandler = GetComponent<InputHandler>();
        }

        private void Start()
        {
            _cameraHandler.EnableCursorLock();
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            isInteracting = animator.GetBool("isInteracting");

            _playerLocomotion.HandleMovement(delta);
            _playerLocomotion.HandleFalling(delta);
            _playerLocomotion.HandleRolling(delta);
            _playerLocomotion.HandleJumping(delta);
        }

        private void LateUpdate()
        {
            _inputHandler.sprintFlag = false;
            _inputHandler.rollFlag = false;
            _inputHandler.jumpFlag = false;
            isSprinting = _inputHandler.B_Input;
        }

    }

}