using UnityEngine;
using Photon.Pun;

public class EggProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 0.5f;

    public Collider hitBox;   // ⭐ 拖 HitBox 的 BoxCollider

    PhotonView ownerPV;
    Vector3 moveDir;

    public void Init(PhotonView owner, Vector3 fireDir)
    {
        ownerPV = owner;
        moveDir = fireDir.normalized;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (ownerPV == null) return;

        PhotonView targetPV = other.GetComponentInParent<PhotonView>();
        if (targetPV == null || targetPV == ownerPV) return;

        Debug.Log($"🥚 Egg hit {targetPV.Owner?.NickName}");

        PlayerEggEffect eggEffect =
            targetPV.GetComponent<PlayerEggEffect>();

        if (eggEffect != null)
        {
            targetPV.RPC(
                nameof(PlayerEggEffect.RPC_ShowEggEffect),
                RpcTarget.All
            );
        }

        EggSkill skill = ownerPV.GetComponent<EggSkill>();
        if (skill != null)
        {
            skill.photonView.RPC(
                nameof(EggSkill.RPC_PlayEggHitSFX),
                RpcTarget.All
            );
        }

        // ⭐ 立刻关闭 HitBox，防止多次触发
        if (hitBox != null)
            hitBox.enabled = false;

        Destroy(gameObject);
    }
}
