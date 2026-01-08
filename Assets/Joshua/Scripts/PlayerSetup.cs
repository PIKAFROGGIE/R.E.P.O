using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Renderer BodyRenderer;

    public GameObject playerUIPrefab;
    private PlayerController playerController;

    public Camera FPSCamera;

    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();

        if (photonView.IsMine)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (playerUIPrefab != null)
            {
                GameObject playerUIGameobject = Instantiate(playerUIPrefab);
            }
           
        }
        else
        {
            foreach (Camera cam in playerController.GetComponentsInChildren<Camera>())
            {
                if(cam != null)
                    cam.gameObject.SetActive(false);
            }
            playerController.enabled = false;
            animator.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
