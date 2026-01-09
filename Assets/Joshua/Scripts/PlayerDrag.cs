using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerDrag : MonoBehaviourPunCallbacks
{
    public float dragRange = 2f;

    DragPlayer currentTarget;

    public float dragHoldTime = 1f;

    public float dragDistance = 1.2f;
    public float dragFollowSpeed = 8f;

    float dragHoldTimer = 0f;
    bool isHoldingDrag = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragHoldTimer = 0f;
            isHoldingDrag = true;
        }

        if (Input.GetMouseButton(0) && isHoldingDrag && currentTarget == null)
        {
            dragHoldTimer += Time.deltaTime;

            if (dragHoldTimer >= dragHoldTime)
            {
                TryStartDrag();
                isHoldingDrag = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHoldingDrag = false;
            dragHoldTimer = 0f;

            if (currentTarget != null)
            {
                StopDrag();
            }
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (currentTarget == null) return;

        Transform targetTransform = currentTarget.transform;

        Vector3 desiredPos =
            transform.position - transform.forward * dragDistance;

        targetTransform.position = Vector3.Lerp(
            targetTransform.position,
            desiredPos,
            Time.fixedDeltaTime * dragFollowSpeed
        );
    }


    void TryStartDrag()
    {
        if (Physics.Raycast(transform.position + Vector3.up,transform.forward,out RaycastHit hit,dragRange))
        {
            DragPlayer target = hit.collider.GetComponent<DragPlayer>();
            Debug.Log("found");
            if (target == null) return;

            StartDrag(target);
        }
    }


    void StartDrag(DragPlayer target)
    {
        currentTarget = target;

        //target.photonView.RPC("RPC_SetDragged",RpcTarget.All,true);
        //target.RPC_SetDragged(true);
    }

    void StopDrag()
    {
        if (currentTarget == null) return;

        currentTarget.photonView.RPC("RPC_SetDragged", RpcTarget.All, false);
        //currentTarget.RPC_SetDragged(false);
        currentTarget = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position + Vector3.up;
        Vector3 dir = transform.forward * dragRange;

        Gizmos.DrawLine(origin, origin + dir);
    }
}
