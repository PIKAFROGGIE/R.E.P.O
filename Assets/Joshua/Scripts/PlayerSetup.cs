using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] FPS_Hands_ChildGameobjects;
    public GameObject[] Soldier_ChildGameobjects;

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
            foreach (GameObject gameObject in Soldier_ChildGameobjects)
            {
                gameObject.SetActive(false);
            }
            if (playerUIPrefab != null)
            {
                GameObject playerUIGameobject = Instantiate(playerUIPrefab);
            }
            FPSCamera.enabled = true;
            if(animator != null)
            animator.SetBool("IsSoldier", false);
        }
        else
        {
            foreach (GameObject gameObject in Soldier_ChildGameobjects)
            {
                gameObject.SetActive(true);
            }

            playerController.enabled = false;
            foreach (Camera cam in GetComponentsInChildren<Camera>())
            {
                cam.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
