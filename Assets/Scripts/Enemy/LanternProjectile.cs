using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternProjectile : MonoBehaviour
{
    int damage;

    public void InitializeProjectile(int a_damage)
    {
        damage = a_damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the projectile hits the player, deal damage
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }

        // Destroys projectile on-hit
        Destroy(gameObject);
    }
}
