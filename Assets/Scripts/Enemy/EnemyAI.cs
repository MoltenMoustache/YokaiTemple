using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Health
    [SerializeField] int maxHealth;
    int currentHealth;

    // Attack
    [SerializeField] int attackDamage;

    Vector3 targetPos;
    GameObject[] players;
    GameObject targetPlayer;
    [SerializeField] float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");   
    }

    // Update is called once per frame
    void Update()
    {
        GetClosestPlayer();
        FollowPlayer();
    }
    
    void GetClosestPlayer()
    {
        float distance = Mathf.Infinity;
        foreach (GameObject player in players)
        {
            float playerDistance = Vector3.Distance(transform.position, player.transform.position);
            if(playerDistance < distance)
            {
                targetPlayer = player;
                distance = playerDistance;
            }

        }

        targetPos = targetPlayer.transform.position;
    }

    void FollowPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(attackDamage);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int a_dmg = 1)
    {
        currentHealth -= a_dmg;
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
