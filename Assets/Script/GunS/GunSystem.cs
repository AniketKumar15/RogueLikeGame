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
    int bulletsLeft, bulletsShot;
    public int rotationSpeed = 500;

    //bools 
    bool shooting, readyToShoot, reloading;


    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;
    public float aimSmoothing = 10;
    //Graphics
    public ParticleSystem muzzleFlash;
    public GameObject bulletHoleGraphic;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI text;

    private Vector3 originalPosition;  // Store the original position
    public float recoilAmount = 0.1f;  // How much the gun moves back
    private Vector3 currentRecoilOffset = Vector3.zero;
    private Vector3 targetPosition;
    public int gunDefaultRotation = 105;
    public int aimRotation = 90;
    public Vector3 rotationAxis = new Vector3(0, 0, 1);



    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Start()
    {
        originalPosition = transform.localPosition;
    }
    private void Update()
    {
        MyInput();
        DetermineAim();

        if (reloading) RotateGun(rotationSpeed);
        else StopRotation();

        // Apply recoil offset to the gun position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + currentRecoilOffset, Time.deltaTime * aimSmoothing);

        // Update UI
        text.SetText(bulletsLeft + " / " + magazineSize);
    }
    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);


        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        if (bulletsLeft == 0 && !reloading) Reload();


        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
            ApplyRecoil();
        }
    }
    private void DetermineAim()
    {
        // Position Adjustment
        targetPosition = normalLocalPosition;
        float targetYRotation = gunDefaultRotation; // Default rotation when not aiming

        if (Input.GetMouseButton(1)) // Right Mouse Button for Aiming
        {
            targetPosition = aimingLocalPosition;
            targetYRotation = aimRotation; // Rotate to 90° when aiming
        }

        // Smoothly interpolate position and rotation
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * aimSmoothing);

        // Smoothly rotate only on the Y-axis
        Quaternion targetRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, targetYRotation, transform.localRotation.eulerAngles.z);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * aimSmoothing);
    }
    private void Shoot()
    {
        readyToShoot = false;

        muzzleFlash.Play();

        AudioManager.instance.Play("Fire");
        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        
        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

       
        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range))
        {

            if (rayHit.rigidbody != null)
            {
                rayHit.rigidbody.AddForce(-rayHit.normal * 200);
            }

            //if (rayHit.collider.CompareTag("Enemy"))
            //    rayHit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
        }


        //ShakeCamera
        //camShake.StartShake(camShakeDuration, camShakeMagnitude);


        //Graphics
        GameObject impact = Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
        impact.transform.SetParent(rayHit.collider.gameObject.transform);
        Destroy(impact, 5);


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
    private void Reload()
    {
        reloading = true;
        transform.GetChild(0).gameObject.SetActive(false);
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        transform.GetChild(0).gameObject.SetActive(true);
        reloading = false;
    }
    public void ApplyRecoil()
    {
        // Apply recoil relative to current position
        currentRecoilOffset += new Vector3(0, 0, -recoilAmount);

        // Smoothly return to aimed position
        StartCoroutine(ResetRecoil());
    }

    private IEnumerator ResetRecoil()
    {
        float elapsedTime = 0f;
        float recoilResetSpeed = 10f; // Adjust for smoothness

        Vector3 startOffset = currentRecoilOffset;

        while (elapsedTime < 0.1f)
        {
            currentRecoilOffset = Vector3.Lerp(startOffset, Vector3.zero, elapsedTime * recoilResetSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentRecoilOffset = Vector3.zero; // Ensure exact reset
    }
    void RotateGun(float rotationSpeed)
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
        
    }
    void StopRotation()
    {
        if (!Input.GetMouseButton(1))
            transform.localRotation = Quaternion.Euler(0, gunDefaultRotation, 0); // Reset rotation
    }

    public bool IsShooting()
    {
        return Input.GetMouseButton(0); // Shooting when left-click is held
    }

    public bool IsAiming()
    {
        return Input.GetMouseButton(1); // Aiming when right-click is held
    }
}
