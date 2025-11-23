using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeslaGun : MonoBehaviour
{
    [SerializeField] InputActionReference shoot;
    [SerializeField] InputActionReference reload;
    [SerializeField] private float laserRange = 50;
    [SerializeField] Transform muzzle;
    [SerializeField] LayerMask hitLayers;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float rayVisualTime = 0.1f;
    [SerializeField] float teslaDamage = 1;
    [SerializeField] private float maxAmmoTime = 10f;
    [SerializeField] private float currAmmoTime;
    [SerializeField] private int maxMags;
    [SerializeField] private int currMag;
    [SerializeField] private TMPro.TextMeshProUGUI ammoText;
    [SerializeField] private TMPro.TextMeshProUGUI magsText;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ParticleSystem shootParticles;
    public bool isShooting = false;
    [SerializeField] private NPC npc;

    PlayerController controllerScipt;

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

    void Start()
    {
        // Initialize variables so the player starts with full ammo and mags and update UI
        currAmmoTime = maxAmmoTime;
        currMag = maxMags;
        ammoText.text = currAmmoTime.ToString() + "/" + maxAmmoTime;
        magsText.text = currMag.ToString() + "/" + maxMags;

    }

    void Update()
    {
        // Shoot and Reload actions, just like the name says...
        if (shoot.action.IsPressed()) ShootLaser();
        if (reload.action.triggered)
        {
            if (currMag > 0 && currAmmoTime == 0) Reload(maxAmmoTime);
            else if (currMag == 0) Debug.Log("No more mags left, look for more Arrows");
        }
    }

    // Main method to shoot the laser (commented lines to test before deleting them)
    // Update: Test completed, removed comments
    private void ShootLaser()
    {
        if (currAmmoTime > 0 && currMag >= 0 && !npc.playerInRange)
        {
            {
                isShooting = true;
                playerAnimator.SetTrigger("LaserShot");
                StartCoroutine(ShootLaserRoutine());
            }
        }
        else if ( currMag <= 0)
        {
            Debug.Log("No ammo left, reload");
        }
    }

    // Coroutine to handle animation start and function execution
    private IEnumerator ShootLaserRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (!shoot.action.IsPressed())
        {
            isShooting = false;
            yield break;
        }
        RaycastHit hit;
        Vector3 laserEnd = muzzle.position + muzzle.forward * laserRange;

        if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, laserRange, hitLayers))
        {
            currAmmoTime -= Time.deltaTime;
            currAmmoTime = Mathf.Clamp(currAmmoTime, 0f, maxAmmoTime);
            ammoText.text = ((currAmmoTime / maxAmmoTime) * 100).ToString("F0") + "%";

            laserEnd = hit.point;
            Debug.Log("Hit detected with: " + hit.collider.name);

            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null) enemy.GetHit(teslaDamage);
        }

        // Draw laser visual effect
        DrawLaser(laserEnd);
        shootParticles.Play();
    }

    public void TryShoot()
    {
        if (currAmmoTime > 0 && currMag >= 0 && !npc.playerInRange)
        {
            if (!isShooting)
                StartCoroutine(ShootLaserRoutine());
        }
    }

    private void DrawLaser(Vector3 endPoint)
    {
        lineRenderer.SetPosition(0, muzzle.position);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.enabled = true;
        Invoke(nameof(HideLaser), rayVisualTime);
    }

    private void HideLaser()
    {
        lineRenderer.enabled = false;
        shootParticles.Stop();
        isShooting = false;
    }

    public void RestockAmmo()
    {
        currMag++;
        currMag = Mathf.Clamp(currMag, 0, maxMags);
        magsText.text = currMag.ToString() + maxMags;
    }

    public void Reload(float time)
    {
        currMag--;
        currAmmoTime = time;
        magsText.text = currMag.ToString() + "/" + maxMags;
        ammoText.text = currAmmoTime.ToString() + "/" + maxAmmoTime;
    }
}
