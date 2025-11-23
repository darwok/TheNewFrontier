using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private Transform teleportDestination;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerHP = other.GetComponent<PlayerController>();
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;  // disable to prevent move conflict
                other.transform.position = teleportDestination.position;
                controller.enabled = true;   // re-enable after teleport
                playerHP.TakeDamage();
            }
        }
    }
}