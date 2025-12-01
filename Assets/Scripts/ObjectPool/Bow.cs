using System.Collections;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public string weaponName;

    [SerializeField] private ObjectPool arrowPool;
    [SerializeField] private Transform muzzle;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private int currAmmo = 10;
    [SerializeField] public int maxMags = 3;
    [SerializeField] public int currMag = 3;
    [SerializeField] private TMPro.TextMeshProUGUI ammoText;
    [SerializeField] private TMPro.TextMeshProUGUI magsText;

    [Header("Shoot")]
    [SerializeField] private float shootCooldown = 0.5f;
    [SerializeField] private ParticleSystem shootParticles;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private NPC npc; // para no disparar si hablo con el NPC y evitar bugs

    private float lastShotTime;
    public bool isShooting { get; private set; }

    private void Awake()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponentInParent<Animator>();
    }

    private void Start()
    {
        currAmmo = maxAmmo;
        currMag = maxMags;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ammoText != null)
            ammoText.text = currAmmo.ToString() + "/" + maxAmmo;

        if (magsText != null)
            magsText.text = currMag.ToString() + "/" + maxMags;
    }

    public void TryShoot()
    {
        if (isShooting) return;
        if (currAmmo <= 0)
        {
            Debug.Log("No ammo, need to reload");
            return;
        }

        if (Time.time < lastShotTime + shootCooldown) return;
        if (npc != null && npc.playerInRange) return;

        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        if (playerAnimator != null)
            playerAnimator.SetTrigger("ArrowShot");

        yield return new WaitForSeconds(0.5f);

        if (shootParticles != null)
            shootParticles.Play();

        currAmmo--;
        lastShotTime = Time.time;
        UpdateUI();

        GameObject arrowObj = arrowPool != null ? arrowPool.Get() : null;
        if (arrowObj != null)
        {
            arrowObj.transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);
            Arrow arrow = arrowObj.GetComponent<Arrow>();
            if (arrow != null)
                arrow.Init(arrowPool);
        }

        isShooting = false;
    }

    // Restock de mags (llamado por pickups)
    public void RestockAmmo()
    {
        currMag++;
        currMag = Mathf.Clamp(currMag, 0, maxMags);
        UpdateUI();
    }

    // Recarga un mag completo, si es que hay mags disponibles obvio...
    public void Reload()
    {
        if (currMag <= 0)
        {
            Debug.Log("No more mags left");
            return;
        }

        currMag--;
        currAmmo = maxAmmo;
        UpdateUI();
    }

    private void OnEnable()
    {
        // Por si el arma se re-activa en medio de un disparo cancelado
        isShooting = false;
        UpdateUI();
    }

    private void OnDisable()
    {
        // Cancelamos cualquier “estado de disparo” atascado
        isShooting = false;
    }
}