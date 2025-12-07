using UnityEngine;
using UnityEngine.UI;
using TMPro; // para TextMeshProUGUI

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
    public TextMeshProUGUI feedbackText;
    [SerializeField] private float feedbackDuration = 2f;

    [Header("Keys")]
    [SerializeField] private KeyGiverNPC keyGiver;

    [HideInInspector] public bool playerInRange;

    private Coroutine feedbackRoutine;

    private void Start()
    {
        // Ocultar UI al inicio
        if (interact != null)
            interact.SetActive(false);

        if (options != null)
            options.SetActive(false);

        if (feedbackText != null)
        {
            feedbackText.text = string.Empty;
            feedbackText.gameObject.SetActive(false);
        }

        if (keyButton != null)
            keyButton.onClick.AddListener(OnAskForKey);

        if (byeButton != null)
            byeButton.onClick.AddListener(OnSayBye);
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        bool isNowInRange = dist <= interactionDistance;

        // Si el jugador sale del rango, cerrar diálogo y limpiar UI
        if (!isNowInRange && playerInRange)
        {
            playerInRange = false;
            CloseDialogue();
            return;
        }

        playerInRange = isNowInRange;

        // Mostrar "interact" sólo si está en rango y el menú no está abierto
        if (interact != null)
            interact.SetActive(playerInRange && (options != null && !options.activeSelf));

        // Abrir menú al presionar F estando en rango
        if (playerInRange && options != null && !options.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            OpenOptions();
        }
    }

    private void OpenOptions()
    {
        EnableCursor();

        if (interact != null)
            interact.SetActive(false);

        if (options != null)
            options.SetActive(true);
    }

    private void CloseDialogue()
    {
        if (options != null)
            options.SetActive(false);

        if (interact != null)
            interact.SetActive(false);

        // detener feedback si estaba corriendo
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }

        if (feedbackText != null)
        {
            feedbackText.text = string.Empty;
            feedbackText.gameObject.SetActive(false);
        }

        DisableCursor();
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
        var inventory = player != null ? player.GetComponent<KeyInventory>() : null;
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
    }

    private void ShowFeedback(string text)
    {
        if (feedbackText == null) return;

        feedbackText.text = text;
        feedbackText.gameObject.SetActive(true);

        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        feedbackRoutine = StartCoroutine(ClearFeedbackRoutine());
    }

    private System.Collections.IEnumerator ClearFeedbackRoutine()
    {
        yield return new WaitForSeconds(feedbackDuration);

        if (feedbackText != null)
        {
            feedbackText.text = string.Empty;
            feedbackText.gameObject.SetActive(false);
        }

        feedbackRoutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}