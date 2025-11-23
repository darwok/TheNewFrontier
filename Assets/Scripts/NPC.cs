using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("Player & Range")]
    public Transform player;
    public float interactionDistance = 3f;

    [Header("UI References")]
    public GameObject interact;
    public GameObject options;
    public Button keyButton;
    public Button byeButton;
    public TMPro.TextMeshProUGUI feedbackText;
    [SerializeField] private float feedbackDuration = 2f;
    private Coroutine _clearFeedbackRoutine;
    public bool playerInRange;
    [SerializeField] private PlayerController playerController;

    void Start()
    {
        interact.SetActive(false);
        options.SetActive(false);
        feedbackText.text = "";

        keyButton.onClick.AddListener(OnAskForKey);
        byeButton.onClick.AddListener(OnSayBye);
    }

    void Update()
    {
        float dist = Vector3.Distance(player.position, transform.position);
        playerInRange = dist <= interactionDistance;
        interact.SetActive(playerInRange && !options.activeSelf);
        if (playerInRange && !options.activeSelf && Input.GetKeyDown(KeyCode.F))
            OpenOptions();
    }

    void OpenOptions()
    {
        EnableCursor();
        interact.SetActive(false);
        options.SetActive(true);
    }

    public void CloseDialogue()
    {
        options.SetActive(false);
    }

    void OnAskForKey()
    {
        var pc = player.GetComponent<PlayerController>();
        if (pc == null)
            return;

        // only give Key1 if the player already has Key0
        if (pc.hasKey0 && !pc.hasKey1)
        {
            pc.hasKey1 = true;
            ShowFeedback("Here’s the key to the Main Area!");
        }
        else if (!pc.hasKey0)
        {
            ShowFeedback("You need to clear the other room first");
        }
        else
        {
            ShowFeedback("You already have the key");
        }
        DisableCursor();
        Invoke(nameof(CloseDialogue), 5f);
    }

    private void ShowFeedback(string msg)
    {
        if (_clearFeedbackRoutine != null) StopCoroutine(_clearFeedbackRoutine);
        feedbackText.text = msg;
        _clearFeedbackRoutine = StartCoroutine(ClearFeedbackAfterDelay());
    }

    private IEnumerator ClearFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.text = "";
        _clearFeedbackRoutine = null;
    }

    void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnSayBye()
    {
        CloseDialogue();
        DisableCursor();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}