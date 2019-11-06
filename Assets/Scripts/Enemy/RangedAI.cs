using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAI : EnemyAI
{
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileForce;
    GameObject projectileSpawn;

    protected override IEnumerator Attack()
    {
        // Disable attack ability for attack cooldown
        canAttack = false;

        // Gets the player's PlayerController
        PlayerController playerController = targetPlayer.GetComponent<PlayerController>();

        // Attack player

        // Finds the spawn position for the projectile
        projectileSpawn = transform.Find("Projectile Spawn").gameObject;

        // Instantiates the projectile
        GameObject projectileObj = Instantiate(projectile, projectileSpawn.transform.position, transform.rotation);

        // Initializes the projectile with the damage
        projectileObj.GetComponent<LanternProjectile>().InitializeProjectile(damage);

        // Adds force to projectile
        projectileObj.GetComponent<Rigidbody>().AddForce((transform.forward * projectileForce), ForceMode.VelocityChange);

        // Goes on cooldown for 'attackCooldown' seconds and then re-enables attacking
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
