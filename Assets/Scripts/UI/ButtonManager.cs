using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public bool IsDisabled;
    public List<GameObject> button;
    public Vector3 DefaultPos;
    public int ChoiceIndex = 0;
    int buttonTotal;
    int buttonIndex;
    public GameObject MainMenuCanvas;
    public GameObject OptionCanvas;
    public bool PlayerSelection = false;
    public int MenuIndicatorSpeed = 2000;

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

        DefaultPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // If menu is enabled
        if (!IsDisabled)
        {




            // Main Menu Only
            if (button[buttonIndex].GetComponent<ButtonBehavior>().IsMainMenu)
            {
                if (PlayerSelection)
                {
                    if (transform.localPosition.x > -1200)
                    {
                        transform.position = new Vector3(transform.position.x - MenuIndicatorSpeed * Time.deltaTime, transform.position.y, transform.position.z);
                    }
                }
                else if (!PlayerSelection)
                {
                    if (transform.localPosition.x < DefaultPos.x)
                    {
                        transform.position = new Vector3(transform.position.x + MenuIndicatorSpeed * Time.deltaTime, transform.position.y, transform.position.z);
                    }
                    else
                    {
                        if (transform.localPosition.x > DefaultPos.x + 20)
                        {
                            transform.localPosition = new Vector3(DefaultPos.x + 5, transform.localPosition.y, transform.localPosition.z);
                        }
                    }
                }


                // Menu Up
                if (Input.GetKeyUp(KeyCode.DownArrow))
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
                if (Input.GetKeyUp(KeyCode.UpArrow))
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
            if (Input.GetKeyUp(KeyCode.Space) && !button[buttonIndex].GetComponent<ButtonBehavior>().IsMultiChoice && !button[buttonIndex].GetComponent<ButtonBehavior>().IsFunctionButton)
            {
                button[buttonIndex].GetComponent<ButtonBehavior>().Pressed = true;

                if (button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled)
                    button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled = false;
                else
                    button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled = true;

                button[buttonIndex].GetComponent<ButtonBehavior>().ButtonTimer = 1.0f;
            }
            
            // If Button Has a function
            else if (Input.GetKeyUp(KeyCode.Space) && !button[buttonIndex].GetComponent<ButtonBehavior>().IsMultiChoice)
            {                                        
                ButtonFunction(button[buttonIndex].GetComponent<ButtonBehavior>().ButtonName);
            }


            // Options Only
            if (button[buttonIndex].GetComponent<ButtonBehavior>().IsOption)
            {
                // MultiChoice
                if (button[buttonIndex].GetComponent<ButtonBehavior>().IsMultiChoice)
                {
                    if (Input.GetKeyUp(KeyCode.RightArrow))
                    {
                        ChoiceIndex++;
                        if (ChoiceIndex > button[buttonIndex].GetComponent<ButtonBehavior>().Choices.Length - 1)
                        {
                            ChoiceIndex = 0;
                        }
                        button[buttonIndex].GetComponent<ButtonBehavior>().CurrentChoice = button[buttonIndex].GetComponent<ButtonBehavior>().Choices[ChoiceIndex];
                        button[buttonIndex].GetComponent<ButtonBehavior>().ChoiceDisplay.text = button[buttonIndex].GetComponent<ButtonBehavior>().CurrentChoice;
                    }

                    else if (Input.GetKeyUp(KeyCode.LeftArrow))
                    {
                        ChoiceIndex--;
                        if (ChoiceIndex < 0)
                        {
                            ChoiceIndex = button[buttonIndex].GetComponent<ButtonBehavior>().Choices.Length - 1;
                        }
                        button[buttonIndex].GetComponent<ButtonBehavior>().CurrentChoice = button[buttonIndex].GetComponent<ButtonBehavior>().Choices[ChoiceIndex];
                        button[buttonIndex].GetComponent<ButtonBehavior>().ChoiceDisplay.text = button[buttonIndex].GetComponent<ButtonBehavior>().CurrentChoice;
                    }

                    else if (Input.GetKeyUp(KeyCode.DownArrow) && buttonIndex == 6)
                    {
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                        buttonIndex++;
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                    }

                    else if (Input.GetKeyUp(KeyCode.UpArrow) && buttonIndex == 6)
                    {
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                        buttonIndex = 2;
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                    }
                }
                // Non MultiCoice
                else
                {
                    if (Input.GetKeyUp(KeyCode.LeftArrow))
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

                    else if (Input.GetKeyUp(KeyCode.RightArrow))
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

                    else if (Input.GetKeyUp(KeyCode.DownArrow))
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

                    else if (Input.GetKeyUp(KeyCode.UpArrow))
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
            PlayerSelection = (PlayerSelection ? false : true);
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
