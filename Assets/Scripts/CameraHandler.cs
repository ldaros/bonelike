using UnityEngine;
using Cinemachine;

namespace Bone
{
    public class CameraHandler : MonoBehaviour
    {
        public CinemachineFreeLook thirdPersonCamera;
        public Transform mainCamera;

        public void EnableCursorLock()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        public void DisableCursorLock()
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }
}