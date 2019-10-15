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

    List<PlayerController> playersInRange = new List<PlayerController>();

    PlayerController castingPlayer;

    // Update is called once per frame
    void Update()
    {

        QueryButton();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().IsInRitual(true, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().IsInRitual(false, this);
        }
    }

    public bool PlayerCasting()
    {
        if (castingPlayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool StartRitual(PlayerController a_player)
    {
        if (castingPlayer == null)
        {
            Debug.Log("Ritual Started");
            castingPlayer = a_player;

            // Moves player to center of ritual site
            castingPlayer.transform.position = new Vector3(transform.position.x, castingPlayer.transform.position.y, transform.position.z);

            buttonPrompt.enabled = true;
            imageColour.a = 1;
            currentButton = baseKeys[Random.Range(0, baseKeys.Count)];

            Vector3 offset = new Vector3(0f, 100f, 0f);
            buttonPrompt.transform.position = Camera.main.WorldToScreenPoint(castingPlayer.transform.position);
            buttonPrompt.transform.position += offset;
            buttonPrompt.rectTransform.sizeDelta = new Vector2(100, 100);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StopRitual()
    {
        currentButton = null;
        buttonPrompt.sprite = null;
        buttonPrompt.enabled = false;
        imageColour.a -= 0;
        castingPlayer = null;
    }

    void PressButton()
    {
        if (currentButton != null)
        {
            // Right button
            if (XCI.GetButtonDown(currentButton.button, castingPlayer.player))
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
                        if (XCI.GetButtonDown(XboxButton.B, castingPlayer.player) || XCI.GetButtonDown(XboxButton.X, castingPlayer.player) || XCI.GetButtonDown(XboxButton.Y, castingPlayer.player))
                        {
                            WrongButton();
                        }
                        break;
                    case XboxButton.B:
                        if (XCI.GetButtonDown(XboxButton.A, castingPlayer.player) || XCI.GetButtonDown(XboxButton.X, castingPlayer.player) || XCI.GetButtonDown(XboxButton.Y, castingPlayer.player))
                        {
                            WrongButton();
                        }
                        break;
                    case XboxButton.X:
                        if (XCI.GetButtonDown(XboxButton.B, castingPlayer.player) || XCI.GetButtonDown(XboxButton.A, castingPlayer.player) || XCI.GetButtonDown(XboxButton.Y, castingPlayer.player))
                        {
                            WrongButton();
                        }
                        break;
                    case XboxButton.Y:
                        if (XCI.GetButtonDown(XboxButton.B, castingPlayer.player) || XCI.GetButtonDown(XboxButton.X, castingPlayer.player) || XCI.GetButtonDown(XboxButton.A, castingPlayer.player))
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
        castingPlayer.IncrementProgress(ritualIncrement);
        imageColour.a = 1;
        currentButton = baseKeys[Random.Range(0, baseKeys.Count)];
    }

    void WrongButton()
    {
        castingPlayer.DecrementProgress(ritualDecrement);
        imageColour.a = 1;
        currentButton = baseKeys[Random.Range(0, baseKeys.Count)];
    }

    void QueryButton()
    {
        // Checks if there is a player in range
        if (castingPlayer != null)
        {
            // If there is a player in range, check if it has finished the ritual
            if (!castingPlayer.hasFinishedRitual)
            {
                // Displays the current button prompt
                DisplayButton();
                // Fades the button
                imageColour.a -= 0.0175f;
                if (imageColour.a <= 0 && castingPlayer.isCasting)
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
