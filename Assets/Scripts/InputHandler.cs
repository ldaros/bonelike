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
        public bool sprintFlag;
        public float rollInputTimer;

        private PlayerControls _inputActions;
        private Vector2 _movementInput;
        private Vector2 _cameraInput;

        private void Awake()
        {
            _inputActions = new PlayerControls();

            _inputActions.PlayerMovement.Movement.performed += inputActions => _movementInput = inputActions.ReadValue<Vector2>();
            _inputActions.PlayerMovement.Camera.performed += i => _cameraInput = i.ReadValue<Vector2>();
        }

        private void OnEnable() { _inputActions.Enable(); }

        private void OnDisable() { _inputActions.Disable(); }

        private void Update()
        {
            UpdateInputValues();
            handleBInput();
        }

        private void UpdateInputValues()
        {
            Horizontal = _movementInput.x;
            Vertical = _movementInput.y;
            MoveAmount = Mathf.Clamp01(Mathf.Abs(Horizontal) + Mathf.Abs(Vertical));
            MouseX = _cameraInput.x;
            MouseY = _cameraInput.y;
        }

        private void handleBInput()
        {
            B_Input = _inputActions.PlayerActions.Roll.IsPressed();
            bool quickPress = rollInputTimer > 0 && rollInputTimer < 0.2f;

            if (B_Input)
            {
                rollInputTimer += Time.deltaTime;
                sprintFlag = true;
            }
            else
            {
                if (quickPress) { rollFlag = true; }
                rollInputTimer = 0;
                sprintFlag = false;
            }
        }
    }
}