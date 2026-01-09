using UnityEngine;
using Photon.Pun;


public class PlungerProjectile : MonoBehaviour
{
    public float speed;
    public float lifeTime;

    public Collider hitBox;

    PhotonView ownerPV;
    float pullForce;

    Vector3 moveDir;

    bool hasHit = false;

    public void Init(PhotonView owner, float force, Vector3 fireDir)
    {
        ownerPV = owner;
        pullForce = force;
        moveDir = fireDir.normalized;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (hasHit) return;
        transform.position += moveDir * speed * Time.deltaTime;
    }


    void OnTriggerEnter(Collider other)
    {
        if (ownerPV == null) return;

        PhotonView targetPV = other.GetComponentInParent<PhotonView>();
        PlayerKnockback1 knockback = other.GetComponentInParent<PlayerKnockback1>();

        if (targetPV == null || knockback == null) return;
        if (targetPV == ownerPV) return;

        Debug.Log($"Plunger hit {targetPV.Owner?.NickName}");

        Vector3 pullTarget =
            ownerPV.transform.position +
            ownerPV.transform.forward * 1.2f;

        targetPV.RPC(
            nameof(PlayerKnockback1.RPC_PullToPosition),
            RpcTarget.All,
            pullTarget
        );

        PlungerSkill skill = ownerPV.GetComponent<PlungerSkill>();
        if (skill != null)
        {
            skill.photonView.RPC(
                nameof(PlungerSkill.RPC_PlayHitSFX),
                RpcTarget.All
            );
        }

        hasHit = true;

        if (hitBox != null)
            hitBox.enabled = false;

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Destroy(gameObject);
    }


}
