using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    bool FirstFrame = true;

    [Header("How To Play")]
    public GameObject[] HowToPlayImages;
    public GameObject ImageHolder;
    public int ImageIndex = 0;

    [Header("Audio")]
    public AudioClip ClickSound;
    public AudioClip NavSound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                if (XCI.GetDPadUp(XboxDPad.Down) || ((XCI.GetAxis(XboxAxis.RightStickY) < 0 || XCI.GetAxis(XboxAxis.LeftStickY) < 0) && ButtonTimer > 1.0f))
                {
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                    buttonIndex++;
                    if (buttonIndex > buttonTotal - 1)
                    {
                        buttonIndex = 0;
                    }
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                    ButtonTimer = 0.0f;
                    PlayNavSound();
                }

                // Menu Down
                if (XCI.GetDPadUp(XboxDPad.Up) || ((XCI.GetAxis(XboxAxis.RightStickY) > 0 || XCI.GetAxis(XboxAxis.LeftStickY) > 0) && ButtonTimer > 1.0f))
                {
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                    buttonIndex--;
                    if (buttonIndex < 0)
                    {
                        buttonIndex = buttonTotal - 1;
                    }
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                    ButtonTimer = 0.0f;
                    PlayNavSound();
                }
            }

            // Indicate button was pressed
            if (XCI.GetButtonUp(XboxButton.A) && !button[buttonIndex].GetComponent<ButtonBehavior>().IsFunctionButton && ButtonTimer > 1.0f)
            {

                button[buttonIndex].GetComponent<ButtonBehavior>().Pressed = true;

                button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled = button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled ? false : true;

                ButtonTimer = 0.0f;
            }

            // If Button Has a function
            else if (XCI.GetButtonUp(XboxButton.A) && ButtonTimer > 1.0f)
            {
                ButtonFunction(button[buttonIndex].GetComponent<ButtonBehavior>().ButtonName);
                ButtonTimer = 0.0f;
            }


            // Options Only
            if (button[buttonIndex].GetComponent<ButtonBehavior>().IsOption)
            {
                if (FirstFrame)
                {
                    ImageHolder.GetComponent<Image>().sprite = HowToPlayImages[ImageIndex].GetComponent<Image>().sprite;
                    ImageHolder.GetComponent<Animator>().runtimeAnimatorController = HowToPlayImages[ImageIndex].GetComponent<Animator>().runtimeAnimatorController;
                    FirstFrame = false;
                }

                if (XCI.GetButtonUp(XboxButton.B))
                {
                    OptionCanvas.SetActive(OptionCanvas.activeSelf == false ? true : false);
                    button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled = button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled ? false : true;
                    button[buttonIndex].GetComponent<ButtonBehavior>().Pressed = false;
                    ButtonFunction("Close");
                }

                // Indicate button was pressed
                if (XCI.GetButtonUp(XboxButton.A) && button[buttonIndex].GetComponent<ButtonBehavior>().IsFunctionButton && ButtonTimer > 1.0f)
                {

                    button[buttonIndex].GetComponent<ButtonBehavior>().Pressed = true;

                    button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled = button[buttonIndex].GetComponent<ButtonBehavior>().OptionEnabled ? false : true;
                    ButtonFunction(button[buttonIndex].GetComponent<ButtonBehavior>().ButtonName);
                    ButtonTimer = 0.0f;
                }

                if (XCI.GetDPadUp(XboxDPad.Left) || ((XCI.GetAxis(XboxAxis.RightStickX) < 0 || XCI.GetAxis(XboxAxis.LeftStickX) < 0) && ButtonTimer > 1.0f))
                {
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                        buttonIndex--;
                        buttonIndex = Mathf.Clamp(buttonIndex, 0, 2);
                        button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                    PlayNavSound();
                    ButtonTimer = 0.0f;
                }

                if (XCI.GetDPadUp(XboxDPad.Right) || ((XCI.GetAxis(XboxAxis.RightStickX) > 0 || XCI.GetAxis(XboxAxis.LeftStickX) > 0) && ButtonTimer > 1.0f))
                {
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
                    buttonIndex++;
                    buttonIndex = Mathf.Clamp(buttonIndex, 0, 2);
                    button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
                    PlayNavSound();
                    ButtonTimer = 0.0f;

                }

                button[0].GetComponent<ButtonBehavior>().IsDisabled = (ImageIndex == 0 ? true : false);
                button[2].GetComponent<ButtonBehavior>().IsDisabled = (ImageIndex == HowToPlayImages.Length - 1 ? true : false);
            }


        }

    }

    private void PlayNavSound()
    {
        audioSource.clip = NavSound;
        audioSource.Play();
    }
    void ButtonFunction(string buttonName)
    {
        audioSource.clip = ClickSound;
        audioSource.Play();


        // Main Menu
        if (buttonName == "Play")
        {
           SceneManager.LoadSceneAsync(1);
        }

        if (buttonName == "How To Play")
        {
            OptionCanvas.SetActive(true);
            ImageIndex = 0;
            IsDisabled = true;
        }

        if (buttonName == "Exit")
        {
            Application.Quit();
        }

        if (buttonName == "Next")
        {
            ImageIndex++;
            ImageIndex = Mathf.Clamp(ImageIndex, 0, HowToPlayImages.Length - 1);
            ImageHolder.GetComponent<Image>().sprite = HowToPlayImages[ImageIndex].GetComponent<Image>().sprite;
            ImageHolder.GetComponent<Animator>().runtimeAnimatorController = HowToPlayImages[ImageIndex].GetComponent<Animator>().runtimeAnimatorController;
        }

        if (buttonName == "Previous")
        {
            ImageIndex--;
            ImageIndex = Mathf.Clamp(ImageIndex, 0, HowToPlayImages.Length - 1);
            ImageHolder.GetComponent<Image>().sprite = HowToPlayImages[ImageIndex].GetComponent<Image>().sprite;
            ImageHolder.GetComponent<Animator>().runtimeAnimatorController = HowToPlayImages[ImageIndex].GetComponent<Animator>().runtimeAnimatorController;
        }

        // Options
        if (buttonName == "Close")
        {
            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = false;
            buttonIndex = 0;
            ImageIndex = 0;
            button[buttonIndex].GetComponent<ButtonBehavior>().HighLighted = true;
            MainMenuCanvas.GetComponentInChildren<ButtonManager>().IsDisabled = false;
            transform.parent.gameObject.SetActive(false);
            FirstFrame = true;
        }

    }
}
