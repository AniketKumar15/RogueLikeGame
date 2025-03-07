using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    public enum FireMode { SemiAuto, Auto, Sniper }
    [Header("Gun Settings")]
    public FireMode fireMode;
    public float fireRate = 0.1f;
    public float damage = 20f;
    public float range = 100f;
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadTime = 2f;
    public bool isReloading = false;

    [Header("Recoil Settings")]
    public Transform camTransform;
    public float recoilAmount = 2f;
    public float recoilSpeed = 5f;

    [Header("Crosshair Settings")]
    public Image crosshair;
    public float baseCrosshairSize = 50f;
    public float moveExpand = 20f;
    public float shootExpand = 15f;
    private float crosshairSize;

    [Header("Reload Animation Settings")]
    public Transform gunTransform;  // Assign your gun's transform here
    public float reloadSpinSpeed = 200f;  // Rotation speed while reloading

    [Header("References")]
    public Transform muzzlePoint;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    private float nextFireTime = 0f;
    private Rigidbody playerRb;
    private Quaternion originalRotation;

    void Start()
    {
        currentAmmo = maxAmmo;
        playerRb = FindObjectOfType<Rigidbody>();
        crosshairSize = baseCrosshairSize;
        originalRotation = gunTransform.localRotation;  // Store initial rotation
    }

    void Update()
    {
        if (isReloading)
        {
            // Rotate gun during reload
            gunTransform.Rotate(0, 0, reloadSpinSpeed * Time.deltaTime);
            return;
        }

        // Shooting input based on fire mode
        if (fireMode == FireMode.Auto && Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
        }
        else if (fireMode != FireMode.Auto && Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
        }

        // Reload
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }

        // Update crosshair dynamically
        UpdateCrosshair();
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        muzzleFlash.Play();

        // Recoil effect
        camTransform.localRotation = Quaternion.Euler(-recoilAmount, 0, 0) * camTransform.localRotation;

        // Raycast for hit detection
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            if (hit.collider.GetComponent<EnemyHealth>())
            {
                hit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        // Stop rotation and reset gun position
        gunTransform.localRotation = originalRotation;
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void UpdateCrosshair()
    {
        float movementFactor = playerRb.velocity.magnitude > 0.1f ? moveExpand : 0;
        float shootingFactor = Input.GetButton("Fire1") ? shootExpand : 0;
        float newSize = baseCrosshairSize + movementFactor + shootingFactor;

        crosshair.rectTransform.sizeDelta = Vector2.Lerp(
            crosshair.rectTransform.sizeDelta,
            new Vector2(newSize, newSize),
            Time.deltaTime * 5f
        );
    }
}
