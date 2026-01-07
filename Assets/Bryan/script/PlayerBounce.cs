using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    public float bounceStrength = 50f;

    private CharacterController controller;
    private Vector3 bounceVelocity;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Barrier")) return;

        Vector3 bounceDir = hit.normal;
        bounceVelocity = bounceDir * bounceStrength;
    }

    void Update()
    {
        if (bounceVelocity.magnitude > 0.1f)
        {
            bounceVelocity = Vector3.Lerp(bounceVelocity, Vector3.zero, Time.deltaTime * 6f);
            GetComponent<CharacterController>().Move(bounceVelocity * Time.deltaTime);
        }
    }
}
