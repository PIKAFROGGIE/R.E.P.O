using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController : MonoBehaviour
{
    bool collapseBase = true, collapseMiddle = true;
    public GameObject UFOBase, UFOMiddle;
    Rigidbody rbBase, rbMiddle;
    public Animator baseAnim, middleAnim;
    public PlayerUI timer;
    // Start is called before the first frame update
    void Start()
    {
        rbBase = UFOBase.GetComponent<Rigidbody>();
        rbMiddle = UFOMiddle.GetComponent<Rigidbody>();
        rbBase.isKinematic = true;
        rbMiddle.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.GetCurrentTime() < 90f && collapseBase)
        {
            collapseBase = false;
            StartCoroutine(Collapse(baseAnim, rbBase));
        }
        if (timer.GetCurrentTime() < 50f && collapseMiddle)
        {
            collapseMiddle = false;
            StartCoroutine(Collapse(middleAnim, rbMiddle));
        }
    }

    IEnumerator Collapse(Animator anim, Rigidbody rb)
    {
        anim.SetTrigger("collapse");
        yield return new WaitForSeconds(10f);
        anim.SetTrigger("fall");
        EnableRigidbody(rb);
    }

    void EnableRigidbody(Rigidbody rb)
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.WakeUp();
    }
}
