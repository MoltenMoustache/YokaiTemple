using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PounceAI : EnemyAI
{
    [Header("Pounce")]
    public float pounceSpeed;
    [SerializeField] float pouncePause;
    bool isPouncing = false;

    // NavMesh info

    float currentStoppingDistance;
    float currentSpeed;

    protected override IEnumerator Attack()
    {
        canAttack = false;

        isTracking = false;
        isPouncing = true;
        navMeshAgent.enabled = false;

        //Gets the player's position
        Vector3 pounceDestination = targetPlayer.transform.position;

        // Enemy pauses for a short moment before pouncing
        yield return new WaitForSeconds(pouncePause);

        // Enemy looks at the target position
        transform.LookAt(pounceDestination);

        // Adds a force to the enemy in the direction of where they were looking
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * pounceSpeed, ForceMode.VelocityChange);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    protected override void Update()
    {
        CheckPounceCompletion();
        base.Update();
    }

    void CheckPounceCompletion()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isPouncing)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            navMeshAgent.enabled = true;
            rb.isKinematic = false;
            isPouncing = false;
            isTracking = true;
        }

        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}
