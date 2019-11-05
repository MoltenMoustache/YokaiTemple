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
    [Header("Movement/Dash")]
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
    ParticleSystem dashParticle;

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

    [Header("Health")]
    public int maxHealth;     // public for the ^ header
    public int currentHealth;
    GameObject healthIconParent;
    [SerializeField] List<GameObject> healthIcons = new List<GameObject>();

    // Combat Variables
    [Header("Attacking")]
    public float hitSpeed;
    AttackCone attackCone;
    [SerializeField] float attackDelay;
    public bool canAttack;

    // Other References
    Rigidbody rb;

    // UI
    [SerializeField] GameObject hudObject;
    Image interactPrompt;

    bool isGodMode;
    [SerializeField] float godmodeDuration;

    RitualSite ritualSite;

    // Start is called before the first frame update
    void Start()
    {
        // Gets the Rigidbody component and stores it in 'rb'
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Reference to Animator component
        animator = GetComponent<Animator>();

        attackCone = transform.Find("Attack Cone").GetComponent<AttackCone>();

        currentHealth = maxHealth;
    }

    public void InitializePlayer(GameObject a_hudObject, XboxController a_player)
    {
        player = a_player;
        hudObject = a_hudObject;

        if (hudObject)
        {
            hudObject.SetActive(true);
            ritualBar = hudObject.transform.Find("Progress Slider").GetComponent<Slider>();
            reviveBar = hudObject.transform.Find("Revive Bar").GetComponent<Slider>();
            interactPrompt = hudObject.transform.Find("Interact Prompt").GetComponent<Image>();
            healthIconParent = hudObject.transform.Find("Health Icons").gameObject;

            Transform[] childIcons = healthIconParent.GetComponentsInChildren<Transform>();
            foreach (Transform child in childIcons)
            {
                healthIcons.Add(child.gameObject);
                healthIcons.Remove(healthIconParent);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // https://www.youtube.com/watch?v=dQw4w9WgXcQ
        // Followed this tutorial ^


        #region Debug Controls
        if (XCI.GetButtonDown(XboxButton.LeftBumper, player) || Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Hello!");
            TakeDamage(5);
        }

        if (XCI.GetButtonDown(XboxButton.RightBumper, player) || Input.GetKeyDown(KeyCode.E))
        {
            RevivePlayer(true);
        }

        if (XCI.GetButtonDown(XboxButton.DPadDown, player) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            TakeDamage(1);
        }

        if (XCI.GetButtonDown(XboxButton.DPadLeft, player))
        {
            TakeDamage(2);
        }

        if (XCI.GetButtonDown(XboxButton.DPadUp, XboxController.First) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            HealDamage(1);
        }
        #endregion

        // Gets the axis
        verticalAxis = XCI.GetAxis(XboxAxis.LeftStickY, player);
        horizontalAxis = XCI.GetAxis(XboxAxis.LeftStickX, player);

        // Rotate's the player according to axis (if the player is not downed or casting)
        if (!isDown && !isCasting)
            Rotate();


        if (reviveBar)
        {
            ReviveAction();
            DecayBar();
        }

        // Ritual
        if (ritualSite != null)
        {
            // If the player presses X in the ritual and theres no enemies in their attack cone, start casting the ritual.
            if (XCI.GetButtonDown(XboxButton.X, player) && !isCasting && canAttack)
            {
                CheckForNullEnemies();

                if (attackCone.enemiesInRange.Count == 0)
                {
                    Debug.LogWarning("No enemies in range and can attack");
                    if (ritualSite.StartRitual(this))
                    {
                        ToggleInteractPrompt(false);
                        isCasting = true;
                        GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
                else
                {
                    Debug.LogWarning("Enemies in range or cant attack...");
                }
            }

            if (isCasting)
            {
                // If the player is downed, stop casting the ritual.
                if (isDown)
                {
                    StopCasting(false);
                }

                // If player moves joystick, stop casting the ritual.
                if (XCI.GetAxis(XboxAxis.LeftStickX, player) > 0.5 || XCI.GetAxis(XboxAxis.LeftStickY, player) > 0.5 ||
                        XCI.GetAxis(XboxAxis.LeftStickX, player) < -0.4 || XCI.GetAxis(XboxAxis.LeftStickY, player) < -0.4)
                {
                    StopCasting(true);
                    GetComponent<Rigidbody>().isKinematic = false;
                }
            }
            
            // UI
            if (interactPrompt.color != Color.clear)
            {
                MoveInteractPrompt();
            }

        }

        // Allows revival
        ReviveAction();
        DecayBar();

        // Dash & Attack
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
                CheckForNullEnemies();

                // Attack
                StartCoroutine(Attack());
            }
        }
    }

    private void FixedUpdate()
    {
        // If the player is not casting the ritual or downed, allow movement
        if (!isDown && !isCasting)
            Move();
    }

    void MoveInteractPrompt()
    {
        if (!isCasting)
        {
            Vector3 offset = new Vector3(0f, 60f, 0f);
            interactPrompt.transform.position = Camera.main.WorldToScreenPoint(transform.position);
            interactPrompt.transform.position += offset;
            interactPrompt.rectTransform.sizeDelta = new Vector2(60, 60);
        }
    }

    public void IsInRitual(bool a_active, RitualSite a_site)
    {
        if (a_active)
        {
            ritualSite = a_site;
            ToggleInteractPrompt(!ritualSite.PlayerCasting());
        }
        else
        {
            ritualSite = null;
            ToggleInteractPrompt(false);
        }

    }

    void ToggleInteractPrompt(bool a_active)
    {
        if (a_active)
        {
            interactPrompt.color = Color.white;
        }
        else
        {
            interactPrompt.color = Color.clear;
        }
    }

    void StopCasting(bool a_togglePrompt)
    {
        ritualSite.StopRitual();
        isCasting = false;
        ToggleInteractPrompt(a_togglePrompt);
    }

    #region Joining/Leaving Game

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

        //Vector3 eulerRotation = new Vector3(dashParticle.transform.eulerAngles.x, transform.eulerAngles.y + 180, dashParticle.transform.eulerAngles.z);
        //dashParticle.transform.rotation = Quaternion.Euler(eulerRotation);
        //dashParticle.Play();

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

            // Disable kinematic on Rigidbody to allow movement
            rb.isKinematic = false;
            GetComponent<Rigidbody>().isKinematic = false;

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
                if (bar.value <= bar.minValue)
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
                Debug.Log("Downed player entered range");
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
        if (!isGodMode)
        {
            currentHealth -= a_dmg;
            Debug.Log("Damage taken!" + a_dmg);
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
    }

    public void TakeDamage(Enemy a_attacker, int a_dmg = 1)
    {
        if (!isGodMode)
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
            StartCoroutine(FlashGodmode());
        }
    }

    IEnumerator FlashGodmode()
    {
        isGodMode = true;
        yield return new WaitForSeconds(godmodeDuration);
        isGodMode = false;
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

    void CheckForNullEnemies()
    {
        // 
        List<Enemy> nullEnemies = new List<Enemy>();

        // Iterates through all enemies to check for null references...
        foreach (var enemy in attackCone.enemiesInRange)
        {
            if (enemy == null)
            {
                // Adds null reference to list
                nullEnemies.Add(enemy);
            }
        }

        if (nullEnemies.Count > 0)
        {
            // Iterates through all found null references and removes them from the enemiesInRange list
            foreach (var enemy in nullEnemies)
            {
                attackCone.enemiesInRange.Remove(enemy);
            }

            // Clear the null reference list
            nullEnemies.Clear();
        }
    }

    IEnumerator Attack()
    {
        // The player can't attack until...
        canAttack = false;


        // Check if enemies are within the cone...
        if (attackCone.enemiesInRange.Count > 0)
        {
            // Set target enemy and look at enemy
            Enemy targetEnemy = attackCone.enemiesInRange[0];
            transform.LookAt(targetEnemy.transform);

            // Remove enemy from list and kill enemy
            attackCone.enemiesInRange.Remove(targetEnemy);
            targetEnemy.TakeDamage(10);

            //transform.position = transform.position + transform.forward;

            // CHECK IF HEALTH IS LESS THAN 10
        }

        // If projectiles are within the cone...
        if (attackCone.projectilesInRange.Count > 0)
        {
            // Reflect the projectile in the direction the player is facing
            foreach (GameObject proj in attackCone.projectilesInRange)
            {
                if (proj != null)
                {
                    proj.transform.rotation = transform.rotation;
                    proj.GetComponent<Rigidbody>().velocity = transform.forward * 3;
                }
                else
                {
                    attackCone.projectilesInRange.Remove(proj);
                }
            }
        }

        yield return new WaitForSeconds(0.25f);
        canAttack = true;

    }

}
