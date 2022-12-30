using UnityEngine;

namespace Bone
{
    public class InputHandler : MonoBehaviour
    {
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        public float MoveAmount { get; private set; }
        public float MouseX { get; private set; }
        public float MouseY { get; private set; }
        public bool B_Input { get; private set; }

        public bool rollFlag;
        public bool isInteracting;

        private PlayerControls _inputActions;
        private CameraHandler _cameraHandler;
        private Vector2 _movementInput;
        private Vector2 _cameraInput;

        private void Awake()
        {
            _inputActions = new PlayerControls();
            _cameraHandler = GetComponent<CameraHandler>();

            _inputActions.PlayerMovement.Movement.performed += inputActions => _movementInput = inputActions.ReadValue<Vector2>();
            _inputActions.PlayerMovement.Camera.performed += i => _cameraInput = i.ReadValue<Vector2>();

            _cameraHandler.EnableCursorLock();
        }

        private void OnEnable() { _inputActions.Enable(); }

        private void OnDisable() { _inputActions.Disable(); }

        private void Update()
        {
            UpdateInputValues();
            handleRollInput();
        }

        private void UpdateInputValues()
        {
            Horizontal = _movementInput.x;
            Vertical = _movementInput.y;
            MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
            MouseX = _cameraInput.x;
            MouseY = _cameraInput.y;
        }

        private void handleRollInput()
        {
            B_Input = _inputActions.PlayerActions.Roll.triggered;
            if (B_Input) { rollFlag = true; }
        }
    }
}