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
    [HideInInspector] public int ritualContribution;
    Slider ritualBar;
    [HideInInspector] public bool isCasting;
    [HideInInspector] public bool hasFinishedRitual;

    // Reviving Variables
    [HideInInspector] public bool isDown;
    [HideInInspector] public PlayerController downedPlayer;
    Slider reviveBar;
    bool isBeingRevived;
    
    // Health Variables
    int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] GameObject[] healthIcons;

    // Combat Variables
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

        // Rotate's the player according to axis (if the player is not downed or casting)
        if (!isDown && !isCasting)
            Rotate();

        // Dash
        if (!isCasting)
        {
            // if the A button is pressed and the player can dash
            if (XCI.GetButtonDown(XboxButton.A, player) && canDash)
            {
                // If the cooldown is at 0, allow a dash
                StartCoroutine(Dash());
            }

            // If the X button is pressed and the player can attack
            if (XCI.GetButtonDown(XboxButton.X, player))
            {
                // Attack
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
        DecayBar();

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
        #endregion
    }

    private void FixedUpdate()
    {
        // If the player is not casting the ritual or downed, allow movement
        if (!isDown && !isCasting)
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
    // Moves the player according to left stick movement
    void Move()
    {
        // Moves the object in the direction of the joystick
        rb.velocity = new Vector3(horizontalAxis * moveSpeed * Time.deltaTime, 0, verticalAxis * moveSpeed * Time.deltaTime);
    }

    // Rotates the player according to left stick movement
    void Rotate()
    {
        // Actually rotate the player
        Vector3 lookDirection = new Vector3(horizontalAxis, 0, verticalAxis);
        if (lookDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    // Quickly moves the player forward a short distance
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
            isDown = true;
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

    void RevivePlayer(bool a_fullHealth = false)
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

    public void Revive()
    {
        isBeingRevived = true;

        // Slowly increase the bar
        reviveBar.value += 0.005f;
        if (reviveBar.value >= reviveBar.maxValue)
        {
            // If the bar is filled, the player is revived and the bar is reset
            RevivePlayer(true);
            reviveBar.value = reviveBar.minValue;
        }
    }

    public void StopReviving()
    {
        isBeingRevived = false;
    }

    // If there is a downed player in this player's range, begin reviving the player
    void ReviveAction()
    {
        // If the downed player is not equal to null
        if (downedPlayer != null)
        {
            // Gets reference to downed player's revive bar
            Slider bar = downedPlayer.reviveBar;

            // If the player holds the X button on the downed player...
            if (XCI.GetButton(XboxButton.X, player))
            {
                downedPlayer.Revive();
                // if the player is revived, lose the reference to the downed player
                if (reviveBar.value >= reviveBar.maxValue)
                {
                    downedPlayer = null;
                }
            }
            // If revive button is let go...
            else if (XCI.GetButtonUp(XboxButton.X, player))
            {
                // Stop reviving player
                downedPlayer.StopReviving();
                // If the button is let go, the progress is reset
                bar.value -= 0.0025f;
                if(bar.value <= bar.minValue)
                {
                    bar.value = bar.minValue;
                }
                // If the button is let go, the progress is reset
                bar.value -= 0.0025f;
                if (bar.value <= bar.minValue)
                {
                    bar.value = bar.minValue;
                }
            }
        }
    }

    // Decays the revive bar if the reviving action was stopped
    void DecayBar()
    {
        // If the player is no longer being revived but
        if (!isBeingRevived && reviveBar.value > 0)
        {
            reviveBar.value -= 0.0025f;
            if (reviveBar.value < reviveBar.minValue)
            {
                reviveBar.value = reviveBar.minValue;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the collider entering the trigger is tagged as "Player"
        if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerController>().isDown)
            {
                downedPlayer = other.GetComponent<PlayerController>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerController>().isDown)
            {
                downedPlayer.StopReviving();
                downedPlayer = null;
            }
        }
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
        // The player can't attack until...
        canAttack = false;
        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(0.25f);


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

        yield return new WaitForSeconds(0.25f);
        animator.SetBool("isAttacking", false);

        // the cooldown has finished
        yield return new WaitForSeconds(attackDelay);

        // the player can now attack
        canAttack = true;

    }
}
