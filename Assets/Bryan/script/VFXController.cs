using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VFXController : MonoBehaviour
{
    public Animator playerAnim;
    bool isRunning = false;
    Coroutine smokeCoroutine;
    public Transform spawnPosition;
    public GameObject runningSmoke;
    CharacterController CC;
    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAnim.GetFloat("Blend") > 0.75f && CC.isGrounded)
            isRunning = true;
        else
            isRunning = false;

        if(isRunning && smokeCoroutine == null)
        {
            smokeCoroutine = StartCoroutine(EmitSmoke());
        }
        else if (!isRunning && smokeCoroutine != null)
        {
            StopCoroutine(smokeCoroutine);
            smokeCoroutine = null;
        }
    }

    IEnumerator EmitSmoke()
    {
        while(isRunning)
        {
            //PhotonNetwork.Instantiate("runningSmoke", spawnPosition.position, Quaternion.identity);
            Instantiate(runningSmoke, spawnPosition.position, Quaternion.identity);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
