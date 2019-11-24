using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Pathfinding
    [Header("Pathfinding")]
    public float speed;
    [SerializeField] protected float stoppingDistance;
    protected GameObject targetPlayer;
    protected NavMeshAgent navMeshAgent;
    protected bool isTracking = true;

    // Attacking
    [Header("Attacking")]
    public int damage;
    protected bool canAttack = true;
    [SerializeField] protected float attackCooldown;
    protected Animator animator;

    private void Start()
    {
        InitializeEnemy();
    }

    GameObject GetTarget()
    {
        float distance = Mathf.Infinity;
        GameObject closestTarget = null;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (!player.GetComponent<PlayerController>().isDown)
            {
                float playerDistance = Vector3.Distance(transform.position, player.transform.position);

                if (playerDistance < distance)
                {
                    closestTarget = player;
                    distance = playerDistance;
                }
            }
        }

        if (isTracking)
            return closestTarget;
        else
            return null;
    }

    void LookAtTarget()
    {
        //Transform targetTransform = null;
        //targetTransform.position = new Vector3(targetPlayer.transform.position.x, 0, targetPlayer.transform.position.z);
        transform.LookAt(targetPlayer.transform);
    }

    protected virtual void Update()
    {
        targetPlayer = GetTarget();

        if (targetPlayer)
        {
            LookAtTarget();
            navMeshAgent.SetDestination(targetPlayer.transform.position);
            AttackCheck();
        }
        //else
        //Debug.Log("All players downed or not in game");
    }

    void AttackCheck()
    {
        if (canAttack)
        {
            // Check if we've reached the destination
            if (!navMeshAgent.pathPending)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        StartCoroutine(PlayAttackAnimation());
                        StartCoroutine(Attack());
                    }
                }
            }
        }
    }

    protected virtual IEnumerator PlayAttackAnimation()
    {
        animator.SetBool("isAttacking", true);
        AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];
        yield return new WaitForSeconds(clip.length);
        animator.SetBool("isAttacking", false);
    }
    protected virtual IEnumerator Attack()
    {
        // Disable attack ability for attack cooldown
        canAttack = false;

        // Gets the player's PlayerController
        PlayerController playerController = targetPlayer.GetComponent<PlayerController>();

        // Attack player
        playerController.TakeDamage(damage);

        // Goes on cooldown for 'attackCooldown' seconds and then re-enables attacking
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(PlayerController a_attacker)
    {
        a_attacker.attackCone.enemiesInRange.Remove(this);
        Destroy(gameObject);
    }

    void InitializeEnemy()
    {
        // Adds and configures Rigidbody to enemy object
        if (!GetComponent<Rigidbody>())
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 99999999;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        // Adds and configures NavMeshAgent to enemy object
        if (!GetComponent<NavMeshAgent>())
        {
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = speed;
            navMeshAgent.stoppingDistance = stoppingDistance;
        }
        else
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        animator = GetComponent<Animator>();
        // Sets tag
        tag = "Enemy";
    }
}
