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

    [Header("Keys")]
    [SerializeField] private KeyGiverNPC keyGiver;

    [HideInInspector] public bool playerInRange;

    private Coroutine feedbackRoutine;

    private void Start()
    {
        interact.SetActive(false);
        options.SetActive(false);

        keyButton.onClick.AddListener(OnAskForKey);
        byeButton.onClick.AddListener(OnSayBye);
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        playerInRange = dist <= interactionDistance;

        interact.SetActive(playerInRange && !options.activeSelf);

        if (playerInRange && !options.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            OpenOptions();
        }
    }

    private void OpenOptions()
    {
        EnableCursor();
        interact.SetActive(false);
        options.SetActive(true);
    }

    private void CloseDialogue()
    {
        options.SetActive(false);
        interact.SetActive(false);
    }

    private void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnAskForKey()
    {
        var inventory = player.GetComponent<KeyInventory>();
        if (keyGiver != null && inventory != null)
        {
            keyGiver.TryGiveKey(inventory);
            ShowFeedback("Revisando llaves...");
        }
        else
        {
            ShowFeedback("No puedo darte la llave ahora.");
        }
    }

    private void OnSayBye()
    {
        CloseDialogue();
        DisableCursor();
    }

    private void ShowFeedback(string text)
    {
        if (feedbackText == null) return;

        feedbackText.text = text;

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(ClearFeedbackRoutine());
    }

    private System.Collections.IEnumerator ClearFeedbackRoutine()
    {
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.text = string.Empty;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}