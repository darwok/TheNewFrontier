using UnityEngine;

public class DoorController : MonoBehaviour
{
    public enum KeyType
    {
        Key0,
        Key1,
        Key2,
        Key3
    }

    [Header("Setup")]
    public Transform player;
    public KeyType requiredKey;
    public float detectionRange = 3f;
    private PlayerController playerController;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        bool inRange = distance < detectionRange;

        //if (playerController.hasKey0)
        //{
        //    animator.SetBool("hasKey", true);
        //    if(distance < detectionRange)
        //    {
        //        OpenDoor();
        //    }
        //    else
        //    {
        //        CloseDoor();
        //    }
        //}
        if (PlayerHasRequiredKey())
        {
            animator.SetBool("hasKey", true);
            animator.SetBool("isOpen", inRange);
        }
    }

    private bool PlayerHasRequiredKey()
    {
        switch (requiredKey)
        {
            case KeyType.Key0:
                return playerController.hasKey0;
            case KeyType.Key1:
                return playerController.hasKey1;
            case KeyType.Key2:
                return playerController.hasKey2;
            case KeyType.Key3:
                return playerController.hasKey3;
            default:
                return false;
        }
    }

    void OpenDoor()
    {
        animator.SetBool("isOpen", true);
    }

    void CloseDoor()
    {
        animator.SetBool("isOpen", false);
    }

}
