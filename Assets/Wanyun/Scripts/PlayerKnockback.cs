using UnityEngine;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerKnockback : MonoBehaviourPun
{
    public float knockbackDuration = 0.25f;
    public float gravity = -20f;

    CharacterController cc;
    Vector3 knockbackVelocity;
    float knockbackTimer;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;

            cc.Move(knockbackVelocity * Time.deltaTime);

            knockbackVelocity.y += gravity * Time.deltaTime;
        }
    }

    public void ApplyKnockback(Vector3 direction, float force, float upwardForce)
    {
        knockbackVelocity =
            direction.normalized * force +
            Vector3.up * upwardForce;

        knockbackTimer = knockbackDuration;
    }

    public void ApplyBounce(Vector3 direction, float force, float upwardForce)
    {
        knockbackVelocity =
            direction.normalized * force +
            Vector3.up * upwardForce;

        knockbackTimer = 0.15f; 
    }
}
