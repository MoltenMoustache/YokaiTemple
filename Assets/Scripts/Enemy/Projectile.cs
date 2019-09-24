using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float moveSpeed;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //rb.velocity = Vector3.forward * moveSpeed;
        rb.AddForce(transform.forward * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
        rb.velocity = rb.velocity.normalized * moveSpeed;
    }

    public void IsHit()
    {
        rb.isKinematic = true;
        rb.isKinematic = false;
        transform.Rotate(new Vector3(0f, 180f, 0f));
    }
}
