using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public GunSystem gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;
    public GameObject pickUpText;

    private void Start()
    {
        if (!equipped)
        {
            DisableHand();
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        else
        {
            EquipGun();
        }
    }

    private void Update()
    {

        // Pickup gun when player is in range and presses "E"
        if (!equipped && Vector3.Distance(player.position, transform.position) <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull)
        {
            PickUp();

        }
        // Drop gun when pressing "Q"
        if (equipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.position, fpsCam.forward, out hit, pickUpRange))
        {
            if (hit.collider.CompareTag("Gun") && !equipped && !slotFull)
            {
                pickUpText.SetActive(true);
            }
            else
            {
                pickUpText.SetActive(false);
            }
        }
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        // Make weapon a child of the gun container
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = new Vector3(2, 2, 2);

        EquipGun();
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        // Unparent gun
        transform.SetParent(null);

        //Diabble hand if gun is not equpied by the player
        DisableHand();

        // 🔴 RE-ENABLE Rigidbody by ADDING IT BACK
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.mass = 1f;
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;

        // Enable Collider
        coll.enabled = true;
        coll.isTrigger = false;

        rb.isKinematic = false;
        rb.useGravity = true;
        // Transfer momentum from player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        // Add force to throw the gun
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

        // Add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        // Disable gun script
        gunScript.enabled = false;
    }


    private void EquipGun()
    {
        // ✅ Completely REMOVE Rigidbody
        Destroy(rb);
        rb = null;

        //Enable hand if gun is equpied by the player
        EnableHand();
        // Disable Collider to avoid collisions
        coll.enabled = false;

        // Enable gun script
        gunScript.enabled = true;
    }

    public void DisableHand()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    public void EnableHand()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
