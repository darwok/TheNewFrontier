using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public KeyPrototype requiredKey;
    public float detectionRange = 3f;

    private KeyInventory keyInventory;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (player != null)
        {
            keyInventory = player.GetComponent<KeyInventory>();
            if (keyInventory == null)
            {
                Debug.LogWarning("[DoorController] El player no tiene KeyInventory.");
            }
        }
        else
        {
            Debug.LogWarning("[DoorController] Player no asignado en el inspector.");
        }
    }

    private void Update()
    {
        if (player == null || animator == null || requiredKey == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool inRange = distance < detectionRange;

        if (PlayerHasRequiredKey())
        {
            animator.SetBool("hasKey", true);
            animator.SetBool("isOpen", inRange);
        }
        else
        {
            animator.SetBool("hasKey", false);
            animator.SetBool("isOpen", false);
        }
    }

    private bool PlayerHasRequiredKey()
    {
        if (keyInventory == null || requiredKey == null)
            return false;

        bool hasKey = keyInventory.HasKey(requiredKey);
        // Debug.Log($"[DoorController] PlayerHasRequiredKey({requiredKey.id}) = {hasKey}");
        return hasKey;
    }
}