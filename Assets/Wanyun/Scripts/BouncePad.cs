using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float bounceForce = 10f;
    public float upwardForce = 6f;

    void OnTriggerEnter(Collider other)
    {
        PlayerKnockback player = other.GetComponentInParent<PlayerKnockback>();
        if (player == null) return;

        // 弹开方向：从垫子中心往外
        Vector3 dir = (other.transform.position - transform.position);
        dir.y = 0f;

        player.ApplyBounce(dir, bounceForce, upwardForce);
    }
}
