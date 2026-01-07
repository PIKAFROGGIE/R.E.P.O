using UnityEngine;

public class WindForce : MonoBehaviour
{
    public float windStrength = 5f;

    void OnTriggerStay(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null) return;

        Vector3 windDir = transform.up;
        cc.Move(windDir * windStrength * Time.deltaTime);
    }
}


