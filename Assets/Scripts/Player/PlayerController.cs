// PlayerController.cs
// Last edited 24/9/2019
// Created by Josh Moten


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public XboxController player;
    [HideInInspector] public bool hasJoined;

    // Movement Variables
    [SerializeField] float moveSpeed;
    float verticalAxis;
    float horizontalAxis;

    // Animation
    Animator animator;

    // Dashing Variables
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    bool canDash = true;

    // Ritual Variables
    public int ritualContribution;
    Slider ritualBar;
    [HideInInspector] public bool isCasting;
    [HideInInspector] public bool hasFinishedRitual;

    // Reviving Variables
    [HideInInspector] public bool isDown;
    [HideInInspector] public PlayerController downedPlayer;
    Slider reviveBar;

    [HideInInspector] public PlayerController revivingPlayer;
    
    // Health Variables
    int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] GameObject[] healthIcons;

    // Combat Variables
    [SerializeField] Transform handPosition;
    [SerializeField] GameObject weaponObject;
    [SerializeField] GameObject testWep;
    AttackCone attackCone;
    [SerializeField] float hitSpeed;
    [SerializeField] float attackDelay;
    bool canAttack;

    // Other References
    Rigidbody rb;
    [SerializeField] GameObject hudObject;

    // Model References (TEMPORARY)
    GameObject deadModel;
    GameObject rootObject;
    GameObject samuraiObject;


    // Start is called before the first frame update
    void Start()
    {
        // Gets the Rigidbody component and stores it in 'rb'
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Reference to Animator component
        animator = GetComponent<Animator>();

        // Gets both the ritual bar and revive bar references
        ritualBar = hudObject.transform.Find("Progress Bar").GetComponent<Slider>();
        reviveBar = hudObject.transform.Find("Revive Bar").GetComponent<Slider>();

        attackCone = transform.Find("Attack Cone").GetComponent<AttackCone>();

        currentHealth = maxHealth;

        // Get model references
        deadModel = transform.Find("Dead Model").gameObject;
        rootObject = transform.Find("Root").gameObject;
        samuraiObject = transform.Find("Samurai").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // https://www.youtube.com/watch?v=dQw4w9WgXcQ
        // Followed this tutorial ^


        // Gets the axis
        verticalAxis = XCI.GetAxis(XboxAxis.LeftStickY, player);
        horizontalAxis = XCI.GetAxis(XboxAxis.LeftStickX, player);
        

        animator.SetFloat("Movement", Mathf.Abs(verticalAxis + horizontalAxis));

        // Rotate's the player according to axis (if the player is not downed)
        if (!isDown)
            Rotate();

        // Dash
        if (!isCasting)
        {

            if (XCI.GetButtonDown(XboxButton.A, player))
            {
                // If the cooldown is at 0, allow a dash
                if (canDash)
                    StartCoroutine(Dash());
            }

            if (XCI.GetButtonDown(XboxButton.X, player))
            {
                StartCoroutine(Attack());
            }
        }

        // If the controller is not plugged in, leave the game
        if (!XCI.IsPluggedIn((int)player) && gameObject.activeSelf)
        {
            LeaveGame();
        }
        else if (!hasJoined)
        {
            JoinGame();
        }

        // Allows revival
        ReviveAction();

        #region Debug Controls
        if (XCI.GetButtonDown(XboxButton.LeftBumper, player))
        {
            DownPlayer();
        }

        if (XCI.GetButtonDown(XboxButton.RightBumper, player))
        {
            RevivePlayer(true);
        }

        if (XCI.GetButtonDown(XboxButton.DPadDown, player))
        {
            TakeDamage(1);
        }

        if (XCI.GetButtonDown(XboxButton.DPadLeft, player))
        {
            TakeDamage(2);
        }

        if (XCI.GetButtonDown(XboxButton.DPadUp, XboxController.First))
        {
            HealDamage(1);
        }

        if (XCI.GetButtonDown(XboxButton.DPadRight, player))
        {
            EquipWeapon(testWep);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        // If the player is not casting the ritual or downed, allow movement
        if (!isCasting && !isDown)
            Move();
    }

    #region Joining/Leaving Game
    public void JoinGame()
    {
        // If the player has not already joined the game...
        if (!hasJoined)
        {
            // Enable the hud object and the player object itself
            hudObject.SetActive(true);
            gameObject.SetActive(true);
        }
    }

    void LeaveGame()
    {
        // Disable the hud object and the player object
        hudObject.SetActive(false);
        ritualBar.value = ritualBar.minValue;
        hasFinishedRitual = false;
        isCasting = false;
        GameManager.instance.DecrementProgress(ritualContribution);
        ritualContribution = 0;
        gameObject.SetActive(false);
        // Subtracts the current player's progress from the overall ritual progress
        // Update the player count in the game manager
        GameManager.instance.UpdatePlayerCount();
    }
    #endregion

    #region Movement/Rotation
    void Move()
    {
        // Moves the object in the direction of the joystick
        rb.velocity = new Vector3(horizontalAxis * moveSpeed * Time.deltaTime, 0, verticalAxis * moveSpeed * Time.deltaTime);
    }

    void Rotate()
    {
        // Actually rotate the player
        Vector3 lookDirection = new Vector3(horizontalAxis, 0, verticalAxis);
        if (lookDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public IEnumerator Dash()
    {
        // Exponentially increase movement speed (dash)
        moveSpeed += dashForce;
        canDash = false;

        // For dashDuration seconds.
        yield return new WaitForSeconds(dashDuration);

        // Decrease movement speed (end dash)
        moveSpeed -= dashForce;

        // Dash cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    #endregion

    #region Ritual Increment/Decrement
    public void IncrementProgress(int a_amount = 1)
    {
        // Adds a_amount to the ritual contribution
        ritualContribution += a_amount;

        // Updates the progress bar
        if (ritualContribution >= 50)
        {
            ritualBar.value = ritualBar.maxValue;
            hasFinishedRitual = true;
            Debug.Log(player.ToString() + " player has completed their ritual!");
        }
        ritualBar.value = ritualContribution;

        // Updates GameManager
        GameManager.instance.IncrementProgress(a_amount);
    }

    public void DecrementProgress(int a_amount = 1)
    {
        // Deducts a_amount from the ritual contribution
        ritualContribution -= a_amount;

        // Updates the progress bar
        if (ritualContribution < 0)
        {
            ritualContribution = (int)ritualBar.minValue;
            ritualBar.value = ritualBar.minValue;
        }
        else
        {
            // Updates GameManager
            GameManager.instance.DecrementProgress(a_amount);
        }
        ritualBar.value = ritualContribution;

    }
    #endregion

    #region Incapacitation/Reviving
    public void DownPlayer()
    {
        // If the player is not already downed
        if (!isDown)
        {
            // Set downed to true
            isDown = true;
            // Enables the trigger volume
            GetComponent<SphereCollider>().enabled = true;
            // Enable the revive bar object
            reviveBar.gameObject.SetActive(true);

            // Enable dead model / disable live model
            deadModel.SetActive(true);
            rootObject.SetActive(false);
            samuraiObject.SetActive(false);

            // Make the bar follow the player
            Vector3 offset = new Vector3(0f, 30f, 0f);
            reviveBar.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            reviveBar.transform.position += offset;

            // Enable kinematic on Rigidbody to prevent movement
            rb.isKinematic = true;

            if (currentHealth > 0)
            {
                TakeDamage(5);
            }
        }
    }

    public void RevivePlayer(bool a_fullHealth = false)
    {
        // If the player is already downed
        if (isDown)
        {
            // Set downed to false
            isDown = false;
            // Disables the trigger volume
            GetComponent<SphereCollider>().enabled = false;
            // Disable the revive bar object
            reviveBar.gameObject.SetActive(false);

            // Disable dead model / Enable live model
            deadModel.SetActive(false);
            rootObject.SetActive(true);
            samuraiObject.SetActive(true);

            // Disable kinematic on Rigidbody to allow movement
            rb.isKinematic = false;

            if (currentHealth < maxHealth && a_fullHealth)
                HealDamage(maxHealth);
        }
    }

    void ReviveAction()
    {
        // If the downed player is not equal to null
        if (revivingPlayer != null)
        {
            // Gets reference to downed player's revive bar
            Slider bar = reviveBar;

            // If the player holds the X button on the downed player...
            if (XCI.GetButton(XboxButton.X, revivingPlayer.player))
            {

                // Slowly increase the bar
                bar.value += 0.005f;
                if (bar.value >= bar.maxValue)
                {
                    // If the bar is filled, the player is revived and the bar is reset
                    RevivePlayer(true);
                    bar.value = bar.minValue;
                    revivingPlayer = null;
                }
            }
            else
            {
                // If the button is let go, the progress is reset
                bar.value -= 0.0025f;
                if(bar.value <= bar.minValue)
                {
                    bar.value = bar.minValue;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponent<SphereCollider>().enabled)
        {
            if (other.tag == "Player" && isDown)
            {
                revivingPlayer = other.GetComponent<PlayerController>();
            }
        }
        // else if (ConeCollider)
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().downedPlayer = null;
        }
    }
    #endregion

    #region Equip/Unequip
    public void EquipWeapon(GameObject a_weapon)
    {
        weaponObject = a_weapon;
        weaponObject.transform.parent = handPosition;

    }
    #endregion

    #region Heal/Damage
    public void TakeDamage(int a_dmg = 1)
    {
        currentHealth -= a_dmg;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            DownPlayer();
        }

        Debug.Log("Damage taken:" + a_dmg.ToString());
        Debug.Log("Health remaining:" + currentHealth.ToString());

        foreach (GameObject icon in healthIcons)
        {
            icon.SetActive(false);
        }

        for (int i = 0; i < currentHealth; i++)
        {
            healthIcons[i].SetActive(true);
        }
    }

    public void TakeDamage(Enemy a_attacker, int a_dmg = 1)
    {
        currentHealth -= a_dmg;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            DownPlayer();
            a_attacker.ClearTarget();
        }

        Debug.Log("Damage taken:" + a_dmg.ToString());
        Debug.Log("Health remaining:" + currentHealth.ToString());

        foreach (GameObject icon in healthIcons)
        {
            icon.SetActive(false);
        }

        for (int i = 0; i < currentHealth; i++)
        {
            healthIcons[i].SetActive(true);
        }
    }

    void HealDamage(int a_heal = 1)
    {
        currentHealth += a_heal;
        if (currentHealth > 0)
        {
            RevivePlayer();
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        for (int i = 0; i < currentHealth; i++)
        {
            healthIcons[i].SetActive(true);
        }
    }
    #endregion\

    IEnumerator Attack()
    {
        // Check if enemies are within the cone...
        if (attackCone.enemiesInRange.Count > 0)
        {
            // Deal damage to each enemy
            foreach (Enemy enemy in attackCone.enemiesInRange)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(Random.Range(1, 10));
                }
            }
        }

        // If projectiles are within the cone...
        if (attackCone.projectilesInRange.Count > 0)
        {
            // Reflect the projectile in the direction the player is facing
            foreach (Projectile proj in attackCone.projectilesInRange)
            {
                proj.IsHit();
                proj.transform.rotation = transform.rotation;
                proj.moveSpeed += hitSpeed;
            }
        }

        // The player can't attack until...
        canAttack = false;
        animator.SetBool("isAttacking", !canAttack);

        // the cooldown has finished
        yield return new WaitForSeconds(attackDelay);

        // the player can now attack
        canAttack = true;
        animator.SetBool("isAttacking", !canAttack);

    }
}
