using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCone : MonoBehaviour
{
    PlayerController parentPlayer;
    public List<Enemy> enemiesInRange = new List<Enemy>();
    public List<GameObject> projectilesInRange = new List<GameObject>();

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
        Debug.Log("Trigger entered.");
        if (other.tag == "Enemy")
        {
            Debug.Log("Enemy entered trigger.");
            enemiesInRange.Add(other.GetComponent<Enemy>());
        }

        if (other.tag == "Projectile")
        {
            projectilesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemyLeaving = other.GetComponent<Enemy>();
            if (enemiesInRange.Contains(enemyLeaving))
            {
                enemiesInRange.Remove(enemyLeaving);
            }
        }

        if (other.tag == "Projectile")
        {
            GameObject projectileLeaving = other.gameObject;
            if (projectilesInRange.Contains(projectileLeaving))
            {
                projectilesInRange.Remove(projectileLeaving);
            }
        }
    }
}
