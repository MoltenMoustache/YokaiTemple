using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;

[System.Serializable]
public class Button
{
    public XboxButton button;
    public Sprite icon;
}

public class RitualSite : MonoBehaviour
{
    [SerializeField] Image buttonPrompt;
    [SerializeField] List<Button> baseKeys = new List<Button>();
    Button currentButton;

    Color imageColour = new Color(1, 1, 1, 1);
    Color materialColour;

    [SerializeField] int ritualIncrement;
    [SerializeField] int ritualDecrement;

    PlayerController playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        //materialColour = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange != null)
        {
            if (!playerInRange.isCasting)
            {
                Vector3 offset = new Vector3(0f, 60f, 0f);
                buttonPrompt.transform.position = Camera.main.WorldToScreenPoint(playerInRange.transform.position);
                buttonPrompt.transform.position += offset;
                buttonPrompt.rectTransform.sizeDelta = new Vector2(60, 60);
            }

            if (XCI.GetButtonUp(XboxButton.X, playerInRange.player) && !playerInRange.isCasting)
            {
                StartRitual();
            }

            if (currentButton != null)
            {
                if (XCI.GetAxis(XboxAxis.LeftStickX, playerInRange.player) > 0.5 || XCI.GetAxis(XboxAxis.LeftStickY, playerInRange.player) > 0.5 ||
                    XCI.GetAxis(XboxAxis.LeftStickX, playerInRange.player) < -0.4 || XCI.GetAxis(XboxAxis.LeftStickY, playerInRange.player) < -0.4)
                {
                    StopRitual();
                }
            }
        }
        QueryButton();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && playerInRange == null)
        {
            buttonPrompt.sprite = baseKeys[2].icon;

            if (!other.GetComponent<PlayerController>().hasFinishedRitual)
            {
                playerInRange = other.GetComponent<PlayerController>();
                buttonPrompt.enabled = true;
                imageColour.a = 255;
                buttonPrompt.color = imageColour;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == playerInRange)
        {
            playerInRange = null;
            buttonPrompt.sprite = null;
            buttonPrompt.enabled = false;
            currentButton = null;
        }
    }

    void StartRitual()
    {
        // Moves player to center of ritual site
        playerInRange.transform.position = new Vector3(transform.position.x, playerInRange.transform.position.y, transform.position.z);
        playerInRange.isCasting = true;
        playerInRange.GetComponent<Rigidbody>().isKinematic = true;

        buttonPrompt.enabled = true;
        imageColour.a = 1;
        currentButton = baseKeys[Random.Range(0, baseKeys.Count)];

        Vector3 offset = new Vector3(0f, 100f, 0f);
        buttonPrompt.transform.position = Camera.main.WorldToScreenPoint(playerInRange.transform.position);
        buttonPrompt.transform.position += offset;
        buttonPrompt.rectTransform.sizeDelta = new Vector2(100, 100);
    }

    void StopRitual()
    {
        playerInRange.isCasting = false;
        playerInRange.GetComponent<Rigidbody>().isKinematic = false;

        GetComponent<Renderer>().material.color = materialColour;

        currentButton = null;
        buttonPrompt.sprite = null;
        buttonPrompt.enabled = false;
        imageColour.a -= 0;
    }

    void PressButton()
    {
        if (currentButton != null)
        {
            // Right button
            if (XCI.GetButtonDown(currentButton.button, playerInRange.player))
            {
                if (GameManager.instance.ritualProgress < GameManager.instance.maxRitual)
                {
                    CorrectButton();
                }
                else
                {
                    StopRitual();
                }
            }

            // Wrong button
            else
            {
                switch (currentButton.button)
                {
                    case XboxButton.A:
                        if (XCI.GetButtonDown(XboxButton.B, playerInRange.player) || XCI.GetButtonDown(XboxButton.X, playerInRange.player) || XCI.GetButtonDown(XboxButton.Y, playerInRange.player))
                        {
                            WrongButton();
                        }
                        break;
                    case XboxButton.B:
                        if (XCI.GetButtonDown(XboxButton.A, playerInRange.player) || XCI.GetButtonDown(XboxButton.X, playerInRange.player) || XCI.GetButtonDown(XboxButton.Y, playerInRange.player))
                        {
                            WrongButton();
                        }
                        break;
                    case XboxButton.X:
                        if (XCI.GetButtonDown(XboxButton.B, playerInRange.player) || XCI.GetButtonDown(XboxButton.A, playerInRange.player) || XCI.GetButtonDown(XboxButton.Y, playerInRange.player))
                        {
                            WrongButton();
                        }
                        break;
                    case XboxButton.Y:
                        if (XCI.GetButtonDown(XboxButton.B, playerInRange.player) || XCI.GetButtonDown(XboxButton.X, playerInRange.player) || XCI.GetButtonDown(XboxButton.A, playerInRange.player))
                        {
                            WrongButton();
                        }
                        break;
                }
            }
        }
    }

    void CorrectButton()
    {
        playerInRange.IncrementProgress(ritualIncrement);
        imageColour.a = 1;
        currentButton = baseKeys[Random.Range(0, baseKeys.Count)];

        GetComponent<Renderer>().material.color = materialColour;
    }

    void WrongButton()
    {
        playerInRange.DecrementProgress(ritualDecrement);
        imageColour.a = 1;
        currentButton = baseKeys[Random.Range(0, baseKeys.Count)];
    }

    void QueryButton()
    {
        // Checks if there is a player in range
        if (playerInRange != null)
        {
            // If there is a player in range, check if it has finished the ritual
            if (!playerInRange.hasFinishedRitual)
            {
                // Displays the current button prompt
                DisplayButton();
                // Fades the button
                imageColour.a -= 0.0175f;
                if(imageColour.a <= 0)
                {
                    WrongButton();
                }

                PressButton();
            }
            else
            {
                StopRitual();
            }
        }
    }

    void DisplayButton()
    {
        if (currentButton != null)
        {
            buttonPrompt.color = imageColour;
            buttonPrompt.sprite = currentButton.icon;
        }
        else
        {
            imageColour.a = 0;
        }
    }
}
