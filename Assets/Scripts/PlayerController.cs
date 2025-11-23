using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Bow Bow;
    [SerializeField] private TeslaGun TeslaGun;
    private Transform Cameramain;

    [Header("Player Build")]
    private CharacterController characterController;
    private bool groundPlayer;
    private Vector3 PlayerVelocity;
    private int jumps = 0;
    [SerializeField] private float currHP;
    [SerializeField] private float maxHP;
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference sprint;
    [SerializeField] private InputActionReference dash;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float timejump;
    [SerializeField] private Animator anim;
    [SerializeField] private Slider lifeSlider;
    [SerializeField] private int maxJumps;
    [SerializeField] private ParticleSystem hitP;
    [SerializeField] private AudioSource hitS;
    [SerializeField] private float hitDamage;
    [SerializeField] private float healing;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform teleportDestination;
    private float tempSpeed;
    private bool canMove = true;
    private bool isHit;
    private bool isDead = false;
    public int playerhp { get { return (int)currHP; } }


    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 5f;

    private float lastDashTime = -Mathf.Infinity;
    private bool isDashing = false;


    [Header("Keys")]
    public bool hasKey0;
    public bool hasKey1;
    public bool hasKey2;
    public bool hasKey3;

    [Header("UI")]

    [Header("Weapons")]
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private int currWeapon = 0;
    [SerializeField] private GameObject[] ammo;
    private bool isSwitching = false;
    private int requestedWeapon = -1;
    //public Bow bowComponent;


    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        sprint.action.Enable();
        dash.action.Enable();
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        sprint.action.Disable();
        dash.action.Disable();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        hasKey0 = false;
        hasKey1 = false;
        hasKey2 = false;
        hasKey3 = false;
        characterController = GetComponent<CharacterController>();
        Cameramain = Camera.main.transform;
        currHP = maxHP;
        lifeSlider.maxValue = maxHP;
        lifeSlider.value = currHP;
        SwitchWeaponRoutine(currWeapon);
        tempSpeed = playerSpeed;
        scoreManager.instance.AddScore(0);
    }

    void Update()
    {
        if (isDead) return;
        // Dash
        if (dash.action.triggered && Time.time >= lastDashTime + dashCooldown && !isDashing && canMove)
        {
            StartCoroutine(DashRoutine());
        }
        if (isDashing) return;

        // Grounded check
        groundPlayer = characterController.isGrounded;
        if (groundPlayer && PlayerVelocity.y < 0)
        {
            PlayerVelocity.y = -2.0f;
            jumps = 0;
        }

        // Weapon switching
        if (!isSwitching)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && currWeapon != 0) StartCoroutine(SwitchWeaponRoutine(0));
            else if (Input.GetKeyDown(KeyCode.Alpha2) && currWeapon != 1) StartCoroutine(SwitchWeaponRoutine(1));
        }

        Vector2 movement = move.action.ReadValue<Vector2>();
        Vector3 moveInput = new Vector3(movement.x,0,movement.y);
        moveInput = Cameramain.forward * moveInput.z + Cameramain.right * moveInput.x;
        moveInput.y = 0;

        // No movement if hit
        isHit = anim.GetCurrentAnimatorStateInfo(0).IsName("Hit");
        canMove = !isHit;
        //canMove = Bow.isShooting ? false : canMove;
        //canMove = TeslaGun.isShooting ? false : canMove;
        playerSpeed = canMove ? tempSpeed : 0;

        // Walking and sprinting animations and conditinos
        if (moveInput.x != 0 && canMove && !sprint.action.IsPressed()) anim.SetBool("Walking", true);
        else if(moveInput.x == 0) anim.SetBool("Walking", false);
        if (moveInput.x !=0 && canMove && sprint.action.IsPressed())
        {
            anim.SetBool("Walking", false);
            anim.SetBool("Sprinting", true);
            playerSpeed = tempSpeed + sprintSpeed; // Change speed temporarily when sprinting
        }
        else
        {
            playerSpeed = tempSpeed;
            anim.SetBool("Sprinting", false);
        }
        if (moveInput.magnitude > 0 && canMove) // Only rotate if moving
        {
            characterController.Move(playerSpeed * Time.deltaTime * moveInput);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveInput),
                20f * Time.deltaTime // Smooth rotation speed
            );
        }

        // Jump
        if (jump.action.triggered && jumps < maxJumps)
        {
            if (jumps == 0)
            {
                anim.SetTrigger("Jump");
                PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
            }
            else if (jumps == 1)
            {
                anim.SetTrigger("DoubleJump");
                PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
            }
            jumps++;
        }
        PlayerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(PlayerVelocity * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttackPoint"))
        {
            TakeDamage();
        }
        if(other.CompareTag("Heal"))
        {
            if (currHP >= 100)
                return;
            else if (currHP < 100)
            {
                currHP += healing;
                if (currHP >= 100)
                    currHP = 100;
                lifeSlider.value = currHP;
                other.gameObject.SetActive(false);
            }
        }
        // Check for keys and activate them
        switch (other.gameObject.name)
        {
            case "Key0":
                hasKey0 = true;
                Debug.Log("Picked up Key0");
                other.gameObject.SetActive(false);
                break;

            case "Key1":
                hasKey1 = true;
                Debug.Log("Picked up Key1");
                other.gameObject.SetActive(false);
                break;

            case "Key2":
                hasKey2 = true;
                Debug.Log("Picked up Key2");
                other.gameObject.SetActive(false);
                break;
            case "Key3":
                hasKey3 = true;
                Debug.Log("Picked up Key3");
                other.gameObject.SetActive(false);
                break;
        }

        if (other.CompareTag("Fire"))
        {
            TakeDamage();
        }

        // Restock ammo for weapons
        if (other.gameObject.CompareTag("ArrowsAmmo") && Bow.currMag < Bow.maxMags)
        {
            Bow.RestockAmmo();
            other.gameObject.SetActive(false);
        }
        if (other.gameObject.name == "Core")
        {
            TeslaGun.RestockAmmo();
            other.gameObject.SetActive(false);
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;
        hitP.Play();
        currHP -= hitDamage;
        if (lifeSlider != null) lifeSlider.value = currHP;
        if (currHP <= 0) StartCoroutine(Death());
        //currHP = 0;
        else
        {
            //HealthUI.instance.UpdateHP(hitDamage);
            anim.SetTrigger("Hit");
        }
    }

    private IEnumerator Death()
    {
        isDead = true;
        canMove = false;
        GetComponent<CharacterController>().enabled = false;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(3f);
        //while (anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))//&& anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        //{
        //    yield return null;
        //}
        // When the death animation ends, loads Game Over scene
        SceneManager.LoadScene("Death");
    }

    void ActivateWeapon(int weapon)
    {
        for (int i = 0; i < 2; i++)
        {
            weapons[i].SetActive(false);
            ammo[i].SetActive(false);
        }
        weapons[weapon].SetActive(true);
        ammo[weapon].SetActive(true);
        currWeapon = weapon;
    }

    private IEnumerator SwitchWeaponRoutine(int weapon)
    {
        isSwitching = true;
        requestedWeapon = weapon;
        if (weapon == 0) anim.SetTrigger("EquipBow");
        else if (weapon == 1) anim.SetTrigger("DisarmBow");
        yield return new WaitForSeconds(0.5f); // Wait for the animation to finish
        ActivateWeapon(requestedWeapon);
        isSwitching = false;
        requestedWeapon = -1; // Reset requested weapon
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float end = Time.time + dashDuration;
        while (Time.time < end)
        {
            // Move forward every frame
            characterController.Move(transform.forward * dashSpeed * Time.deltaTime);
            yield return null;
        }
        isDashing = false;
    }
}
