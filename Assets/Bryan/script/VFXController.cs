using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VFXController : MonoBehaviour
{
    public static VFXController Instance;
    public Animator playerAnim;
    bool isRunning = false;
    Coroutine smokeCoroutine;
    public Transform spawnPosition, stunPosition;
    public GameObject runningSmoke, hitEffect, stunEffect;
    CharacterController CC;
    // Start is called before the first frame update

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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

    //[PunRPC]
    public void RPC_StunEffect()
    {
        //PhotonNetwork.Instantiate("hitEffect", spawnPosition.position, Quaternion.identity);
        Instantiate(hitEffect, spawnPosition.position, Quaternion.identity);
        //PhotonNetwork.Instantiate("stunEffect", stunPosition.position, Quaternion.identity);
        Instantiate(stunEffect, stunPosition.position, Quaternion.identity);
    }
}
