using UnityEngine;

public class SpectatorScript : MonoBehaviour
{
    public float flySpeed = 10f;
    public float mouseSensitivity = 2f;
    public GameObject camera;

    void Update()
    {
        // Simple WASD Fly Movement
        float x = Input.GetAxis("Horizontal") * flySpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * flySpeed * Time.deltaTime;

        // E and Q to go Up and Down
        float y = 0;
        if (Input.GetKey(KeyCode.E)) y = flySpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q)) y = -flySpeed * Time.deltaTime;

        camera.transform.Translate(new Vector3(x, y, z));

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        Camera.main.transform.Rotate(Vector3.left * mouseY); 
    }
}