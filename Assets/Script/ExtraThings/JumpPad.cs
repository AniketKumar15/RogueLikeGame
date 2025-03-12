using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public int forceAmount = 10;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody Rb =  collision.gameObject.GetComponent<Rigidbody>();
            Rb.velocity = new Vector3(0, forceAmount, 0);
        }
    }
}
