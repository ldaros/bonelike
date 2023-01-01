using UnityEngine;

namespace Bone
{
    public class RigidBodyPush : MonoBehaviour
    {
        public float force = 2f;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Pushable"))
            {
                Rigidbody body = hit.collider.attachedRigidbody;

                if (body == null || body.isKinematic) { return; }
                if (hit.moveDirection.y < -0.3f) { return; }

                Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                body.velocity = pushDirection * force;
            }
        }

    }

}