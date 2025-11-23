using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private Bow bow;
    [SerializeField] private TeslaGun teslaGun;
    [SerializeField] private GameObject[] weapons;  // 0 = Bow, 1 = TeslaGun
    [SerializeField] private GameObject[] ammoUI;   // ammo panels for each weapon
    [SerializeField] private Animator anim;

    [Header("Weapon Switch Keys")]
    [SerializeField] private KeyCode weapon1Key = KeyCode.Alpha1;
    [SerializeField] private KeyCode weapon2Key = KeyCode.Alpha2;


    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private int maxJumps = 2;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 5f;

    [Header("Damage")]
    [SerializeField] private float hitDamage = 10f;
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private AudioSource hitSound;

    [Header("Teleport")]
    [SerializeField] private Transform teleportDestination;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference sprint;
    [SerializeField] private InputActionReference dash;
    [SerializeField] private InputActionReference attack;

    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 velocity;
    private int jumpCount;
    private float lastDashTime = -Mathf.Infinity;
    private bool isGrounded;

    private Vector3 moveDirection;
    private bool isSprinting;

    private enum PlayerState { Normal, Hurt, Dashing, Dead }
    private PlayerState state = PlayerState.Normal;

    private MoveCommand moveCommand;
    private JumpCommand jumpCommand;
    private DashCommand dashCommand;
    private AttackCommand attackCommand;

    private PlayerStatsSubject stats;
    private KeyInventory keyInventory;

    public int playerhp => stats != null ? Mathf.RoundToInt(stats.CurrentHealth) : 0;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main != null ? Camera.main.transform : transform;

        stats = GetComponent<PlayerStatsSubject>();
        if (stats == null)
        {
            stats = gameObject.AddComponent<PlayerStatsSubject>();
        }

        keyInventory = GetComponent<KeyInventory>();
        if (keyInventory == null)
        {
            keyInventory = gameObject.AddComponent<KeyInventory>();
        }

        // Commands
        moveCommand = new MoveCommand(this, move, sprint);
        jumpCommand = new JumpCommand(this, jump);
        dashCommand = new DashCommand(this, dash);
        attackCommand = new AttackCommand(this, attack);
    }

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        sprint.action.Enable();
        dash.action.Enable();
        attack.action.Enable();
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        sprint.action.Disable();
        dash.action.Disable();
        attack.action.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ActivateWeapon(0);
    }

    private void Update()
    {
        if (state == PlayerState.Dead)
            return;
        // Weapon switching input
        HandleWeaponSwitchInput();

        // First handle commands based on state
        if (state == PlayerState.Normal)
        {
            moveCommand.Execute();
            jumpCommand.Execute();
            dashCommand.Execute();
            attackCommand.Execute();
        }

        // Gravity and vertical movement
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            jumpCount = 0;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Horizontal movement only in allowed states
        if (state == PlayerState.Normal || state == PlayerState.Hurt)
        {
            if (moveDirection.sqrMagnitude > 0.001f)
            {
                controller.Move(moveDirection * Time.deltaTime);
                // Rotar hacia la dirección de movimiento
                Vector3 lookDir = new Vector3(moveDirection.x, 0f, moveDirection.z);
                if (lookDir.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(lookDir),
                        15f * Time.deltaTime
                    );
                }
            }
        }
    }

    // === Methods used by Commands ===
    public void HandleMoveInput(Vector2 input, bool sprinting)
    {
        if (state != PlayerState.Normal) return;

        isSprinting = sprinting;
        float speed = moveSpeed + (isSprinting ? sprintSpeed : 0f);

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desired = forward * input.y + right * input.x;
        if (desired.sqrMagnitude > 1f) desired.Normalize();

        moveDirection = desired * speed;

        // Animations, only basics, I placed the others in their respective methods, had to do some testing for adjusting timing.
        bool isMoving = input.sqrMagnitude > 0.01f;
        anim.SetBool("Walking", isMoving && !isSprinting);
        anim.SetBool("Sprinting", isMoving && isSprinting);
    }

    public void HandleJumpInput()
    {
        if (state != PlayerState.Normal) return;
        if (!controller.isGrounded && jumpCount >= maxJumps) return;

        jumpCount++;

        if (jumpCount == 1)
            anim.SetTrigger("Jump");
        else
            anim.SetTrigger("DoubleJump");

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void HandleDashInput()
    {
        if (state != PlayerState.Normal) return;
        if (Time.time < lastDashTime + dashCooldown) return;

        StartCoroutine(DashRoutine());
    }

    public void HandleAttackInput()
    {
        if (state != PlayerState.Normal) return;

        // Attack with the current weapon
        if (bow != null && teslaGun != null)
        {
            if (weapons != null && weapons.Length > 0)
            {
                // 0 = Bow, 1 = Tesla
                if (weapons[0].activeSelf)
                {
                    bow.TryShoot();
                }
                else if (weapons.Length > 1 && weapons[1].activeSelf)
                {
                    teslaGun.TryShoot();
                }
            }
        }

        //anim.SetTrigger("Shoot");
    }

    // === FSM for damage / dash / death ===
    public void TakeDamage()
    {
        TakeDamage(hitDamage);
    }

    public void TakeDamage(float amount)
    {
        if (state == PlayerState.Dead) return;

        if (stats != null)
            stats.TakeDamage(amount);

        if (hitParticles != null) hitParticles.Play();
        if (hitSound != null) hitSound.Play();

        if (stats != null && stats.CurrentHealth <= 0f)
        {
            StartCoroutine(DeathRoutine());
        }
        else
        {
            StartCoroutine(HurtRoutine());
        }
    }

    private IEnumerator HurtRoutine()
    {
        state = PlayerState.Hurt;
        anim.SetTrigger("Hit");
        yield return new WaitForSeconds(0.5f);
        state = PlayerState.Normal;
    }

    private IEnumerator DeathRoutine()
    {
        state = PlayerState.Dead;
        anim.SetTrigger("Death");
        controller.enabled = false;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Death");
    }

    private IEnumerator DashRoutine()
    {
        state = PlayerState.Dashing;
        lastDashTime = Time.time;

        float end = Time.time + dashDuration;
        while (Time.time < end)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }

        state = PlayerState.Normal;
    }

    // === Weapons / switching ===

    private void HandleWeaponSwitchInput()
    {
        if (Input.GetKeyDown(weapon1Key))
        {
            ActivateWeapon(0);  // Bow
        }

        if (Input.GetKeyDown(weapon2Key))
        {
            ActivateWeapon(1);  // TeslaGun
        }
    }


    public void ActivateWeaponOld(int index)
    {
        if (weapons == null || weapons.Length == 0) return;
        if (ammoUI == null || ammoUI.Length < weapons.Length) return;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
            ammoUI[i].SetActive(i == index);
        }
    }

    public void ActivateWeapon(int index)
    {
        if (weapons == null || weapons.Length == 0) return;
        if (index < 0 || index >= weapons.Length) return;

        // Activar/desactivar armas
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                weapons[i].SetActive(i == index);
        }

        // Activar/desactivar UI de ammo SOLO si existe
        if (ammoUI != null && ammoUI.Length > 0)
        {
            for (int i = 0; i < ammoUI.Length; i++)
            {
                if (ammoUI[i] != null)
                    ammoUI[i].SetActive(i == index);
            }
        }

        // Debug log to confirm switch and test key input
        Debug.Log($"Weapon switched to index {index}");
    }


    public void SwitchWeapon(int index)
    {
        ActivateWeapon(index);
    }

    // Teleport helper (if I need it to work with other traps or mechanics, actually useful for testing)
    public void TeleportTo(Transform destination)
    {
        if (destination == null) return;
        controller.enabled = false;
        transform.position = destination.position;
        controller.enabled = true;
    }
}