using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonBehavior : MonoBehaviour
{
    public string ButtonName;
    public bool IsMainMenu = false;
    public bool IsOption = false;
    public bool HighLighted = false;
    public bool Pressed = false;
    public bool IsFunctionButton = false;

    // Options
    public bool IsMultiChoice = false;
    public Color DefaultColor = Color.white;
    public float ButtonTimer = -10.0f;
    public string[] Choices = null;
    public string CurrentChoice = "";
    public TextMeshProUGUI ChoiceDisplay;
    public bool OptionEnabled = false;
    public Image Indicator = null;
    public Vector3 DefaultIndicatorSize;

    // Start is called before the first frame update
    void Start()
    {
        DefaultColor = GetComponent<Image>().color;

        if (transform.GetChild(0).GetComponent<TextMeshProUGUI>() != null)
        {
            ButtonName = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        }


        if (ButtonName == "Resolution")
        {
            Choices = new string[3] {"1920 x 1080", "480 x 720", "999 x 111" };
        }

        if (IsMultiChoice)
        {
            CurrentChoice = Choices[0];
        }

        if (Indicator != null)
            DefaultIndicatorSize = Indicator.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (ButtonTimer > 0)
            ButtonTimer -= Time.deltaTime * 5;

        if (Pressed)
        {
            GetComponent<Image>().color = new Color(0.3f,0.3f,0.3f);
            if (ButtonTimer < 0.1f)
            {
                Pressed = false;
            }
        }
        else
        {
            GetComponent<Image>().color = DefaultColor;
        }

        if (Indicator != null)
        {
            if (OptionEnabled)
            {
                Indicator.transform.localPosition = new Vector3(2.5f, -2.5f, 0.0f);
                Indicator.transform.localScale = new Vector3(DefaultIndicatorSize.x - 0.1f, DefaultIndicatorSize.y - 0.1f, DefaultIndicatorSize.z - 0.1f);
            }
            else
            {
                Indicator.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                Indicator.transform.localScale = DefaultIndicatorSize;
            }
        }



        // For the main menu buttons
        if (IsMainMenu)
        {
            if (HighLighted && !Pressed)
            {
                if (transform.localPosition.x < -250)
                {
                    transform.position = new Vector3(transform.position.x + 500 * Time.deltaTime, transform.position.y, transform.position.z);
                }
            }
            else if(!Pressed)
            {
                if (transform.localPosition.x > -380)
                {
                    transform.position = new Vector3(transform.position.x - 500 * Time.deltaTime, transform.position.y, transform.position.z);
                }
                else
                {
                    if (transform.localPosition.x < -380)
                    {
                        transform.localPosition = new Vector3(-380, transform.localPosition.y, transform.localPosition.z);
                    }
                }
            }
        }

        // For Options
        if (IsOption)
        {
            if (HighLighted && !Pressed)
            {
                GetComponent<Image>().color = Color.grey;
            }

            if (OptionEnabled && Indicator != null)
            {
                Indicator.color = Color.green;
            }
            else if (Indicator != null)
            {
                Indicator.color = Color.red;
            }

        }





    }
}
