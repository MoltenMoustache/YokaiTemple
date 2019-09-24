﻿
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [HideInInspector] PlayerController player;
    [HideInInspector] Camera cam;
    public float lookRadius = 10f;
    [HideInInspector] Transform target;
    public float attackDistance = 1.5f;
    public int damage = 1;
    public float attackSpeed = 1f;
    public float attackCooldown = 0f;

    // health Variables
    [SerializeField] int maxHealth;
    int currentHealth;

    public NavMeshAgent agent;

    Vector3 targetPos;
    GameObject[] players;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        players = GameObject.FindGameObjectsWithTag("Player");
        cam = Camera.main;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        GetClosestPlayer();
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (attackCooldown < 0)
        {
            attackCooldown = 0;
        }
        float distance = Vector3.Distance(target.position, transform.position);

        //if(Input.GetMouseButtonDown(0))
        //{

        //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        agent.SetDestination(hit.point);
        //    }
        //}

        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);
            if (distance <= agent.stoppingDistance)
            {
                FaceTarget();
            }
        }

        if (distance <= attackDistance)
        {
            if (attackCooldown == 0)
            {
                player.TakeDamage(damage);
                attackCooldown++;

            }

        }
    }

    public void TakeDamage(int a_dmg)
    {
        currentHealth -= a_dmg;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    void GetClosestPlayer()
    {
        float distance = Mathf.Infinity;
        foreach (GameObject play in players)
        {
            player = play.GetComponent<PlayerController>();
            float playerDistance = Vector3.Distance(transform.position, player.transform.position);
            if (playerDistance < distance)
            {
                target = play.transform;
                player = play.GetComponent<PlayerController>();
                distance = playerDistance;
            }

        }

        targetPos = target.transform.position;
    }

    void OnDrawGizmosSelected()
    {
        //shows the look radius of enemy
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        //shows attack distance of enemy
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
