using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovementCheck player = other.GetComponent<PlayerMovementCheck>();
        if (player != null)
        {
            player.ReachFinish();
        }
    }
}
