
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [HideInInspector] protected PlayerController player;
    [HideInInspector] protected Camera cam;
    public float lookRadius = 10f;
    [HideInInspector] protected Transform target;
    public float attackDistance = 1.5f;
    public int damage = 1;
    public float attackSpeed = 1f;
    public float attackCooldown = 0f;

    // health Variables
    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    public NavMeshAgent agent;

    protected Vector3 targetPos;
    protected GameObject[] players;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }

    protected void OnUpdate()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        target = GetClosestPlayer();

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (attackCooldown < 0)
        {
            attackCooldown = 0;
        }

        float distance = Vector3.Distance(target.position, transform.position);

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
                player.TakeDamage(this, damage);
                attackCooldown++;
            }

        }
    }

    public void ClearTarget()
    {
        target = null;
        player = null;
    }

    public void TakeDamage(int a_dmg)
    {
        currentHealth -= a_dmg;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    protected void FaceTarget()
    {
        Transform targetTransform = null;
        targetTransform.position = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        transform.LookAt(targetTransform);
    }
    protected Transform GetClosestPlayer()
    {
        float distance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (GameObject play in players)
        {
            if (true)
            //if (!play.GetComponent<PlayerController>().isDown)
            {
                //Debug.Log("playerDistance: " + playerDistance);
                player = play.GetComponent<PlayerController>();
                float playerDistance = Vector3.Distance(transform.position, player.transform.position);

                if (playerDistance < distance)
                {
                    closestTarget = play.transform;
                    distance = playerDistance;
                }
            }
        }

        if (target != null)
        {
            targetPos = target.transform.position;
        }
        return closestTarget;
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
