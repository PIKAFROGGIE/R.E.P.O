using UnityEngine;
using Photon.Pun;

public class BananaProjectile : MonoBehaviour
{
    public float speed = 10f;
    public GameObject bananaPeelPrefab;

    Vector3 startPos;
    Vector3 moveDir;
    public float groundOffset = 0.04f;
    float flyDistance;

    PhotonView ownerPV;

    public void Init(PhotonView owner, Vector3 dir, float distance)
    {
        ownerPV = owner;
        moveDir = dir.normalized;
        flyDistance = distance;
        startPos = transform.position;
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) >= flyDistance)
        {
            SpawnPeel();
        }
    }

    void SpawnPeel()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 3f))
        {
            Vector3 forwardOnGround = Vector3.ProjectOnPlane(moveDir, hit.normal).normalized;
            if (forwardOnGround.sqrMagnitude < 0.001f)
                forwardOnGround = Vector3.ProjectOnPlane(Vector3.forward, hit.normal).normalized;

            Quaternion rot = Quaternion.LookRotation(forwardOnGround, hit.normal);

            Collider peelCol = bananaPeelPrefab.GetComponent<Collider>();
            float heightOffset = 0.05f;

            if (peelCol != null)
            {
                heightOffset = peelCol.bounds.extents.y;
            }

            Vector3 spawnPos = hit.point + hit.normal * (heightOffset + groundOffset);

            Instantiate(bananaPeelPrefab, spawnPos, rot);
        }

        Destroy(gameObject);
    }

}
