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
    public bool IsDisabled = false;

    // Options
    public Color DefaultColor = Color.white;
    public float ButtonTimer = -10.0f;
    public bool OptionEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        DefaultColor = GetComponent<Image>().color;

        if (transform.GetChild(0).GetComponent<TextMeshProUGUI>() != null)
        {
            ButtonName = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        }
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
            if (HighLighted)
            {
                GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f);
            }

            if (IsDisabled)
            {
                GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            }
        }





    }
}
