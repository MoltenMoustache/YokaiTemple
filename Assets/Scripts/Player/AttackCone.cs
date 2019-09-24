using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCone : MonoBehaviour
{
    PlayerController parentPlayer;
    public List<Enemy> enemiesInRange = new List<Enemy>();
    public List<Projectile> projectilesInRange = new List<Projectile>();

    // Start is called before the first frame update
    void Start()
    {
        parentPlayer = transform.parent.GetComponent<PlayerController>();
        if (!parentPlayer)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            enemiesInRange.Add(other.GetComponent<Enemy>());
        }

        if(other.tag == "Projectile")
        {
            projectilesInRange.Add(other.GetComponent<Projectile>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Enemy enemyLeaving = other.GetComponent<Enemy>();
            if (enemiesInRange.Contains(enemyLeaving))
            {
                enemiesInRange.Remove(enemyLeaving);
            }
        }

        if(other.tag == "Projectile")
        {
            Projectile projectileLeaving = other.GetComponent<Projectile>();
            if (projectilesInRange.Contains(projectileLeaving))
            {
                projectilesInRange.Remove(projectileLeaving);
            }
        }
    }
}
