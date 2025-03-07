using UnityEngine;
using TMPro;
using System.Collections;

public class GunSystem : MonoBehaviour
{
    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletsLeft, bulletsShot;

    //Bools 
    private bool shooting, readyToShoot, reloading;

    //References
    public Camera fpsCam;
    public Transform attackPoint;
    public LayerMask hitMask; // Now supports all layers

    //Graphics
    public GameObject muzzleFlashPrefab;
    public GameObject bulletHolePrefab;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI ammoText;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        HandleInput();
        UpdateAmmoText();
    }

    private void HandleInput()
    {
        shooting = allowButtonHold ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            StartCoroutine(Reload());

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Spread Calculation
        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);
        float spreadZ = Random.Range(-spread, spread);

        Vector3 spreadDirection = fpsCam.transform.forward + new Vector3(spreadX, spreadY, spreadZ);

        //Raycast
        if (Physics.Raycast(fpsCam.transform.position, spreadDirection, out RaycastHit hit, range, hitMask))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // ✅ Apply damage only if the object has an EnemyHealth script
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // ✅ Spawn bullet impact only if an object was hit
            if (bulletHolePrefab != null)
            {
                GameObject impact = Instantiate(bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                impact.transform.SetParent(hit.collider.transform);
                Destroy(impact, 5f);
            }
        }

        // ✅ Show muzzle flash
        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, attackPoint.position, attackPoint.rotation);
            flash.transform.SetParent(attackPoint); // Attach to gun tip
            Destroy(flash, 0.1f);
        }

        // ✅ Apply Camera Shake
        if (camShake != null)
        {
            camShake.Shake(camShakeDuration, camShakeMagnitude);
        }

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private IEnumerator Reload()
    {
        reloading = true;

        // ✅ Rotate gun during reload
        float elapsedTime = 0f;
        Transform gunTransform = transform;
        Quaternion startRotation = gunTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(gunTransform.eulerAngles.x, gunTransform.eulerAngles.y, gunTransform.eulerAngles.z + 360);

        while (elapsedTime < reloadTime)
        {
            gunTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / reloadTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gunTransform.rotation = startRotation; // Reset rotation after reload
        bulletsLeft = magazineSize;
        reloading = false;
    }

    private void UpdateAmmoText()
    {
        if (ammoText != null)
            ammoText.text = $"{bulletsLeft} / {magazineSize}";
    }
}
