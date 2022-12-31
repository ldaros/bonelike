using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bone
{
    public class PlayerManager : MonoBehaviour
    {

        PlayerLocomotion playerLocomotion;
        Animator animator;

        [Header("Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;

        private CameraHandler _cameraHandler;
        private InputHandler _inputHandler;

        void Start()
        {
            playerLocomotion = GetComponent<PlayerLocomotion>();
            animator = GetComponentInChildren<Animator>();
            _cameraHandler = GetComponent<CameraHandler>();
            _inputHandler = GetComponent<InputHandler>();

            _cameraHandler.EnableCursorLock();
        }

        void Update()
        {
            float delta = Time.deltaTime;
            
            isInteracting = animator.GetBool("isInteracting");
            
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        }

        void LateUpdate()
        {
            _inputHandler.sprintFlag = false;
            _inputHandler.rollFlag = false;
            isSprinting = _inputHandler.B_Input;

            if (isInAir) { playerLocomotion.inAirTimer += Time.deltaTime; }
        }

    }

}