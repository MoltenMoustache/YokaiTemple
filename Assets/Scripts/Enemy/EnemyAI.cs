using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Pathfinding
    protected GameObject targetPlayer;
    protected NavMeshAgent navMeshAgent;


    // Attacking
    [Header("Attacking")]
    protected bool canAttack = true;
    public int damage;
    [SerializeField] protected float attackCooldown;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
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

        return closestTarget;
    }

    void LookAtTarget()
    {
        //Transform targetTransform = null;
        //targetTransform.position = new Vector3(targetPlayer.transform.position.x, 0, targetPlayer.transform.position.z);
        transform.LookAt(targetPlayer.transform);
    }

    private void Update()
    {
        targetPlayer = GetTarget();

        if (targetPlayer)
        {
            LookAtTarget();
            navMeshAgent.SetDestination(targetPlayer.transform.position);
            AttackCheck();
        }
        else
            Debug.Log("All players downed or not in game");
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
                        StartCoroutine(Attack());
                    }
                }
            }
        }
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
}
