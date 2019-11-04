using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class ButtonManager : MonoBehaviour
{
    public bool IsDisabled;
    public List<GameObject> button;
    public int ChoiceIndex = 0;
    int buttonTotal;
    int buttonIndex;
    public GameObject MainMenuCanvas;
    public GameObject OptionCanvas;
    public int MenuIndicatorSpeed = 2000;
    float ButtonTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            if (transform.GetChild(i).GetComponent<ButtonBehavior>() != null)
            {
                button.Add(transform.GetChild(i).gameObject);
            }
        }

        if (button.Count > 0)
        {
            button[0].GetComponent<ButtonBehavior>().HighLighted = true;
        }

        buttonIndex = 0;
        buttonTotal = button.Count;
    }

    // Update is called once per frame
    void Update()
    {
        ButtonTimer += Time.deltaTime * 5.0f;

        // If menu is enabled
        if (!IsDisabled)
        {
            // Main Menu Only
            if (button[buttonIndex].GetComponent<ButtonBehavior>().IsMainMenu)
            {
                // Menu Up
                if (XCI.GetDPadUp(XboxDPad.Down))
                {
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                    buttonIndex++;
                    if (buttonIndex > buttonTotal - 1)
                    {
                        buttonIndex = 0;
                    }
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                }

                // Menu Down
                if (XCI.GetDPadUp(XboxDPad.Up))
                {
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                    buttonIndex--;
                    if (buttonIndex < 0)
                    {
                        buttonIndex = buttonTotal - 1;
                    }
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                }
            }

            // Indicate button was pressed
            if (XCI.GetButtonUp(XboxButton.A) && !button[buttonIndex].GetComponent<ButtonBehavior>().IsMultiChoice && !button[buttonIndex].GetComponent<ButtonBehavior>().IsFunctionButton && ButtonTimer > 1.0f)
            {

                button[buttonIndex].GetComponent<ButtonBehavior>().Pressed = true;

                if (button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled)
                    button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled = false;
                else
                    button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled = true;

               ButtonTimer = 0.0f;
            }

            // If Button Has a function
            else if (XCI.GetButtonUp(XboxButton.A) && !button[buttonIndex].GetComponent<ButtonBehavior>().IsMultiChoice && ButtonTimer > 1.0f)
            {
                ButtonFunction(button[buttonIndex].GetComponent<ButtonBehavior>().ButtonName);
                ButtonTimer = 0.0f;

            }


            // Options Only
            if (button[buttonIndex].GetComponent<ButtonBehavior>().IsOption)
            {
                // MultiChoice
                if (button[buttonIndex].GetComponent<ButtonBehavior>().IsMultiChoice)
                {
                    if (XCI.GetDPadUp(XboxDPad.Right))
                    {
                        ChoiceIndex++;
                        if (ChoiceIndex > button[buttonIndex].GetComponent<ButtonBehavior>().Choices.Length - 1)
                        {
                            ChoiceIndex = 0;
                        }
                        button[buttonIndex].GetComponent<ButtonBehavior>().CurrentChoice = button[buttonIndex].GetComponent<ButtonBehavior>().Choices[ChoiceIndex];
                        button[buttonIndex].GetComponent<ButtonBehavior>().ChoiceDisplay.text = button[buttonIndex].GetComponent<ButtonBehavior>().CurrentChoice;
                    }

                    else if (XCI.GetDPadUp(XboxDPad.Left))
                    {
                        ChoiceIndex--;
                        if (ChoiceIndex < 0)
                        {
                            ChoiceIndex = button[buttonIndex].GetComponent<ButtonBehavior>().Choices.Length - 1;
                        }
                        button[buttonIndex].GetComponent<ButtonBehavior>().CurrentChoice = button[buttonIndex].GetComponent<ButtonBehavior>().Choices[ChoiceIndex];
                        button[buttonIndex].GetComponent<ButtonBehavior>().ChoiceDisplay.text = button[buttonIndex].GetComponent<ButtonBehavior>().CurrentChoice;
                    }

                    else if (XCI.GetDPadUp(XboxDPad.Down) && buttonIndex == 6)
                    {
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                        buttonIndex++;
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                    }

                    else if (XCI.GetDPadUp(XboxDPad.Up) && buttonIndex == 6)
                    {
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                        buttonIndex = 2;
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                    }
                }
                // Non MultiCoice
                else
                {
                    if (XCI.GetDPadUp(XboxDPad.Left))
                    {
                        if (buttonIndex == 3 || buttonIndex == 4 || buttonIndex == 5)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex -= 3;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                        else if (buttonIndex == 8)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex--;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                    }

                    else if (XCI.GetDPadUp(XboxDPad.Right))
                    {
                        if (buttonIndex == 0 || buttonIndex == 1 || buttonIndex == 2)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex += 3;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                        else if (buttonIndex == 7)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex++;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                    }

                    else if (XCI.GetDPadUp(XboxDPad.Down))
                    {
                        if (buttonIndex == 2 || buttonIndex == 5)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex = 6;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                        else if (buttonIndex == 8)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex = 3;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                        else if (buttonIndex == 7)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex = 0;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                        else if (buttonIndex == 0 || buttonIndex == 1 || buttonIndex == 3 || buttonIndex == 4)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex++;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                    }

                    else if (XCI.GetDPadUp(XboxDPad.Up))
                    {
                        if (buttonIndex == 6)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex = 2;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                        else if (buttonIndex == 8 || buttonIndex == 7)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex = 6;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                        else if (buttonIndex == 0)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex = 7;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                        else if (buttonIndex == 3)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex = 8;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                        else if (buttonIndex == 1 || buttonIndex == 2 || buttonIndex == 4 || buttonIndex == 5)
                        {
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                            buttonIndex--;
                            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                        }
                    }
                }
            }


        }

    }


    void ButtonFunction(string buttonName)
    {
        // Main Menu
        if (buttonName == "Play")
        {

                SceneManager.LoadSceneAsync(1);



        }

        if (buttonName == "Options")
        {
            OptionCanvas.SetActive(true);
            IsDisabled = true;
        }

        if (buttonName == "Exit")
        {
            Application.Quit();
        }

        // Options
        if (buttonName == "Close")
        {
            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
            buttonIndex = 0;
            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
            MainMenuCanvas.GetComponentInChildren<ButtonManager>().IsDisabled = false;
            transform.parent.gameObject.SetActive(false);
        }

    }
}
