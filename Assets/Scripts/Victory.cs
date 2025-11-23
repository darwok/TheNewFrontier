using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTrigger = "Open"; 
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 3f;

    [Header("Options")]
    [Tooltip("Tag used for enemies in the scene")]
    [SerializeField] private string enemyTag = "Enemy";

    // Ensure we only open once.
    //private bool doorOpened = false;

    private void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        bool inRange = distance < detectionRange;
        //if (!doorOpened && AllEnemiesAreDead())
        //OpenDoor();
        if (AllEnemiesAreDead())
        {
            doorAnimator.SetBool("HasKey", true);
            //doorAnimator.SetBool("Open", inRange);
            if (inRange)
            {
                doorAnimator.SetTrigger("isOpen");
            }
        }
    }

    private bool AllEnemiesAreDead()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        return enemies.Length == 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Win");
        }
    }

    private void OpenDoor()
    {
        //doorOpened = true;
        Debug.Log("All enemies have been killed. The door is now open!");
        if (doorAnimator != null) doorAnimator.SetTrigger(openTrigger);
        else gameObject.SetActive(false);
    }

    
}