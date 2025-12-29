using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController : MonoBehaviour
{
    public bool collapseBase, collapseMiddle;
    public GameObject UFOBase, UFOMiddle;
    Rigidbody rbBase, rbMiddle;
    public Animator baseAnim, middleAnim;
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
        if (collapseBase)
            StartCoroutine(Collapse(baseAnim, rbBase));
        if (collapseMiddle)
            StartCoroutine(Collapse(middleAnim, rbMiddle));
    }

    IEnumerator Collapse(Animator anim, Rigidbody rb)
    {
        anim.SetTrigger("collapse");
        yield return new WaitForSeconds(2f);
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
