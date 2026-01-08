using UnityEngine;
using Photon.Pun;

public class ThunderItem : MonoBehaviour
{
    [Header("Effect")]
    public float radius = 6f;
    public float force = 12f;
    public float lifeTime = 1.2f;

    int ownerActorNumber;

    public void Activate(int owner)
    {
        ownerActorNumber = owner;
        Explode();
        Destroy(gameObject, lifeTime);
    }

    void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            PhotonView pv = hit.GetComponent<PhotonView>();
            if (pv == null) continue;

            // 不影响自己
            if (pv.OwnerActorNr == ownerActorNumber) continue;

            Vector3 dir = (hit.transform.position - transform.position).normalized;

            //只让“被击中的玩家自己”执行位移
            if (pv.IsMine)
            {
                PlayerKnockback kb = hit.GetComponent<PlayerKnockback>();
                if (kb != null)
                {
                    //kb.ApplyKnockback(dir * force);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
