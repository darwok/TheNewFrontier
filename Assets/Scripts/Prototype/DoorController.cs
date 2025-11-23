using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public KeyPrototype requiredKey;
    public float detectionRange = 3f;
    public Animator animator;

    private bool isOpen;

    private void Update()
    {
        if (player == null || requiredKey == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= detectionRange)
        {
            var inventory = player.GetComponent<KeyInventory>();
            if (inventory != null && inventory.HasKey(requiredKey))
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
        else
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;
        if (animator != null) animator.SetBool("isOpen", true);
    }

    void CloseDoor()
    {
        if (!isOpen) return;
        isOpen = false;
        if (animator != null) animator.SetBool("isOpen", false);
    }
}