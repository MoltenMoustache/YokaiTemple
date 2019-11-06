using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PounceAI : EnemyAI
{

    protected override IEnumerator Attack()
    {
        // Gets the player's PlayerController
        PlayerController playerController = targetPlayer.GetComponent<PlayerController>();
        
        // USE COOLDOWN FOR THE PAUSE BEFORE POUNCE

        // POUNCE

        // STUN ENEMY IF MISSED POUNCE

        // STUN PLAYER IF HIT POUNCE

        // Disable attack ability for attack cooldown
        canAttack = false;

        // Goes on cooldown for 'attackCooldown' seconds and then re-enables attacking
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
