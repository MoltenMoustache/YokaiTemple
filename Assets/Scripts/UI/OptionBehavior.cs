using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionBehavior : MonoBehaviour
{
    public UnityEngine.UI.Button button;
    public TMPro.TextMeshProUGUI resultText;
    public TMPro.TextMeshProUGUI optionNameText;
    Image image;

    bool isClicked = false;
    public bool isRes = false;
    string[] res;
    int resIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        
        isRes = (optionNameText.text == "Resolution" ? true : false);

            image = button.GetComponent<Image>();

        if (!isRes)
            image.color = (isClicked == true ? image.color = Color.green : image.color = Color.red);

            button.onClick.AddListener(ButtonOnClicked);

        if (isRes)
        {
            res = new string[3] {"1920 x 1080","960 x 720","640 x 480"};
            resultText.text = res[resIndex];
            resIndex++;
        }
    }

    void ButtonOnClicked()
    {
        if (!isRes)
        {
            isClicked = (isClicked ? false : true);
            image.color = (isClicked == true ? image.color = Color.green : image.color = Color.red);
        }
        else
        {
            resIndex++;
            resIndex = (resIndex >= res.Length ? 0 : resIndex);
            resultText.text = res[resIndex];
        }
    }

    public void SetClicked(bool value)
    {
        isClicked = value;
        image.color = (isClicked == true ? image.color = Color.green : image.color = Color.red);
    }

    public bool GetBool()
    {
        return isClicked;
    }

    public int GetRes()
    {
        return resIndex;
    }

    public void SetRes(int value)
    {
        resIndex = value;
        resultText.text = res[resIndex];
    }
}
