using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    public string weaponName;
    [SerializeField] private GameObject arrowPrefab;
    List<GameObject> arrowsPool = new List<GameObject>();
    [SerializeField] int maxArrows;
    [SerializeField] Transform muzzle;
    [SerializeField] InputActionReference shoot;
    [SerializeField] InputActionReference reload;
    [SerializeField] private int currAmmo;
    [SerializeField] private int maxAmmo;
    [SerializeField] public int maxMags;
    [SerializeField] public int currMag;
    [SerializeField] private TMPro.TextMeshProUGUI ammoText;
    [SerializeField] private TMPro.TextMeshProUGUI magsText;
    [SerializeField] private float lastShotTime;
    [SerializeField] private int shootCD;
    [SerializeField] private ParticleSystem shootParticles;
    [SerializeField] private Animator playerAnimator;
    public bool isShooting = false;
    [SerializeField] private NPC npc;

    private void OnEnable()
    {
        shoot.action.Enable();
        reload.action.Enable();
    }

    private void OnDisable()
    {
        shoot.action.Disable();
        reload.action.Disable();
    }

    private void Awake()
    {
        playerAnimator = GetComponentInParent<Animator>();
    }
    void Start()
    {
        // Set initial ammo and mags
        currAmmo = maxAmmo;
        currMag = maxMags;
        ammoText.text = currAmmo.ToString() + "/" + maxAmmo;
        magsText.text = currMag.ToString() + "/" + maxMags;
        // Create the arrows pool
        for (int i = 0; i < maxArrows; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            arrowsPool.Add(arrow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Shoot and Reload actions
        if (shoot.action.triggered && Time.time >= lastShotTime + shootCD && !npc.playerInRange)
        {
            Debug.Log("Shooting with: " + weaponName);
            Shoot();
        }
        if (reload.action.triggered && currAmmo == 0 || shoot.action.triggered && currAmmo == 0)
        {
            if (currMag > 0) Reload(maxAmmo);
            else if (currMag == 0) Debug.Log("No more mags left, look for more Arrows");
        }
    }

    public void Shoot()
    {
        if (isShooting || currAmmo <= 0 || currMag < 0) return;
        // Check if we can shoot
        else if (currAmmo > 0 && currMag >= 0)
        {
            isShooting = true;
            //shootParticles.Play();
            playerAnimator.SetTrigger("ArrowShot");
            StartCoroutine(ShootRoutine());
            //currAmmo--;
            //ammoText.text = currAmmo.ToString() + "/" + maxAmmo;
            //GameObject currArrow = GetArrow();
            //if (currArrow != null )
            //{
            //    currArrow.transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);
            //    currArrow.SetActive(true);
            //}
            //lastShotTime = Time.time;
        }
        else if (currMag <= 0)
        {
            Debug.Log("No arrows");
        }
    }

    private IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        shootParticles.Play();
        currAmmo--;
        ammoText.text = currAmmo.ToString() + "/" + maxAmmo;
        GameObject currArrow = GetArrow();
        if (currArrow != null)
        {
            currArrow.transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);
            currArrow.SetActive(true);
        }
        lastShotTime = Time.time;
        isShooting = false;
    }

    // If the player has no arrows, we can get a new arrow from the pool or instantiate a new one
    public GameObject GetArrow()
    {
        foreach (GameObject a in arrowsPool) if (!a.activeInHierarchy) return a;
        GameObject clone = Instantiate(arrowPrefab, Vector3.zero, Quaternion.identity);
        clone.SetActive(false);
        arrowsPool.Add (clone);
        return clone;
    }

    // I mean... Restock Ammo is for restocking ammo... I guess
    public void RestockAmmo()
    {
        currMag++;
        currMag = Mathf.Clamp(currMag, 0, maxMags);
        Debug.Log("Restocked ammo, current mag: " + currMag);
        magsText.text = currMag.ToString() + "/" + maxMags;
    }

    // Well... Reload is for reloading the bow, duh
    public void Reload(int gunAmmo)
    {
        currMag--;
        magsText.text = currMag.ToString() + "/" + maxMags;
        currAmmo = gunAmmo;
        ammoText.text = currAmmo.ToString() + "/" + maxAmmo;
    }
}
