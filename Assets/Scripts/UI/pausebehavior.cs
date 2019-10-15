using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XboxCtrlrInput;

public class pausebehavior : MonoBehaviour
{
    public UnityEngine.UI.Button[] button;
    public UnityEngine.UI.Button resume;
    public UnityEngine.UI.Button mainMenu;

    buttonbehaviour[] buttonBehaviours;

    bool IsResume = false;
    bool IsMainMenu = false;

    float timer = 0.0f;
    int currentButton = 0; 

    void Start()
    {
        button = new UnityEngine.UI.Button[2] { resume, mainMenu };
        buttonBehaviours = new buttonbehaviour[2] { button[0].GetComponent<buttonbehaviour>(), button[1].GetComponent<buttonbehaviour>() };
    }

    void Update()
    {
        timer -= (timer > -1.0f ? Time.deltaTime * 5 : 0);

        if (timer <= 0 && IsResume)
        {
            this.gameObject.SetActive(false);
        }
        else if (timer <= 0 && IsMainMenu)
        {
            SceneManager.LoadScene(0);
        }

        if (button[0].GetComponent<buttonbehaviour>().isOn == false && button[1].GetComponent<buttonbehaviour>().isOn == false)
        {
            resume.GetComponent<buttonbehaviour>().isOn = true;
            currentButton = 0;
        }

        if (XCI.GetAxis(XboxAxis.LeftStickY) < 0 && timer <= 0)
        {
            timer = 1.0f;
            currentButton++;
        }

        if (XCI.GetAxis(XboxAxis.LeftStickY) > 0 && timer <= 0)
        {
            timer = 1.0f;
            currentButton--;
        }


        currentButton = (currentButton > 1 ? 0 : (currentButton < 0 ? 0 : currentButton));

        buttonBehaviours[0].isOn = (currentButton == 0 ? true : false);
        buttonBehaviours[1].isOn = (currentButton == 1 ? true : false);

        if (XCI.GetButtonDown(XboxButton.A) && timer <= 0)
        {
            timer = 1.0f;
            ButtonPressed(button[currentButton]);
        }

    }

    private void ButtonPressed(UnityEngine.UI.Button currentButton)
    {
        currentButton.GetComponent<buttonbehaviour>().Pressed();
        timer = 1.0f;

        IsResume = (currentButton == resume ? true : false);
        IsMainMenu = (currentButton == mainMenu ? true : false);
    }
}
