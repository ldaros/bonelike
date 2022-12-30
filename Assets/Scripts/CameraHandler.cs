using UnityEngine;
using Cinemachine;

namespace Bone
{
    public class CameraHandler : MonoBehaviour
    {
        public CinemachineFreeLook freeLookCam;

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