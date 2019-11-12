using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XboxCtrlrInput;
public class PlayerSelectionBehaviour : MonoBehaviour
{
    public int PlayerNum = 0;
    public int HairstyleIndex = 0;
    public int ColourIndex = 0;
    public int[] SubColourIndex;
    public GameObject[] Hairstyle;
    public Animator animator;
    public bool PlayerReady;
    public float WaitTime = 2.0f;
    public GameObject PlayerMeshObject;
    float timer = 0.0f;
    float Ctimer = 0.0f;
    public Color[] colors;
    public GameObject SelectedColorObject;
    public GameObject[] ColorObjects;
    private void Start()
    {
        SubColourIndex = new int[4];
        int index = 0;
        colors = new Color[5];

        for (double i = 0; i < 3.6; i += 0.2f)
        {
            Color c = Color.HSVToRGB((float)i, 1, 1);
            if (index == 5) break;
            colors[index] = new Color(c.r,c.g,c.b);
            index++;
        }
        ColourIndex = 2;

        UpdateIndex();

        SelectedColorObject.GetComponent<Image>().color = colors[ColourIndex];

        ColorObjects[0].GetComponent<Image>().color = colors[SubColourIndex[0]];
        ColorObjects[1].GetComponent<Image>().color = colors[SubColourIndex[1]];
        ColorObjects[2].GetComponent<Image>().color = colors[ColourIndex];
        ColorObjects[3].GetComponent<Image>().color = colors[SubColourIndex[2]];
        ColorObjects[4].GetComponent<Image>().color = colors[SubColourIndex[3]];

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * 2.0f;
        Ctimer += Time.deltaTime * 2.0f;
        if (!PlayerReady)
        {
            if (XCI.GetAxis(XboxAxis.RightStickX, (XboxController)PlayerNum) > 0 && timer > WaitTime)
            {
                Hairstyle[HairstyleIndex].SetActive(false);
                HairstyleIndex++;
                if (HairstyleIndex >= Hairstyle.Length)
                {
                    HairstyleIndex = 0;
                }
                Hairstyle[HairstyleIndex].SetActive(true);
                timer = 0.0f;
            }
            else if (XCI.GetAxis(XboxAxis.RightStickX, (XboxController)PlayerNum) < 0 && timer > WaitTime)
            {
                Hairstyle[HairstyleIndex].SetActive(false);
                HairstyleIndex--;
                if (HairstyleIndex < 0)
                {
                    HairstyleIndex = Hairstyle.Length - 1;
                }
                Hairstyle[HairstyleIndex].SetActive(true);
                timer = 0.0f;
            }

            if (XCI.GetAxis(XboxAxis.LeftStickY, (XboxController)PlayerNum) > 0 && Ctimer > WaitTime)
            {
                
                ColourIndex++;
                if (ColourIndex >= colors.Length)
                {
                    ColourIndex = 0;
                }

                UpdateIndex();

                SelectedColorObject.GetComponent<Image>().color = colors[ColourIndex];
                ColorObjects[0].GetComponent<Image>().color = colors[SubColourIndex[0]];
                ColorObjects[1].GetComponent<Image>().color = colors[SubColourIndex[1]];
                ColorObjects[2].GetComponent<Image>().color = colors[ColourIndex];
                ColorObjects[3].GetComponent<Image>().color = colors[SubColourIndex[2]];
                ColorObjects[4].GetComponent<Image>().color = colors[SubColourIndex[3]];


                Hairstyle[HairstyleIndex].GetComponent<Renderer>().material.color = colors[ColourIndex];

                Ctimer = 0.0f;
            }
            else if (XCI.GetAxis(XboxAxis.LeftStickY, (XboxController)PlayerNum) < 0 && Ctimer > WaitTime)
            {
                ColourIndex--;
                if (ColourIndex < 0)
                {
                    ColourIndex = colors.Length - 1;
                }

                UpdateIndex();
                SelectedColorObject.GetComponent<Image>().color = colors[ColourIndex];
                ColorObjects[0].GetComponent<Image>().color = colors[SubColourIndex[0]];
                ColorObjects[1].GetComponent<Image>().color = colors[SubColourIndex[1]];
                ColorObjects[2].GetComponent<Image>().color = colors[ColourIndex];
                ColorObjects[3].GetComponent<Image>().color = colors[SubColourIndex[2]];
                ColorObjects[4].GetComponent<Image>().color = colors[SubColourIndex[3]];

                Hairstyle[HairstyleIndex].GetComponent<Renderer>().material.color = colors[ColourIndex];
                
                Ctimer = 0.0f;
            }
        }

        if (XCI.GetButton(XboxButton.A, (XboxController)PlayerNum) && timer > WaitTime)
        {
            PlayerReady = (PlayerReady ? false : true);
            animator.SetBool("Ready", PlayerReady);

            timer = 0.0f;
        }
        else if (XCI.GetButton(XboxButton.B, (XboxController)PlayerNum) && timer > WaitTime)
        {
            if (PlayerReady)
            {
                PlayerReady = (PlayerReady ? false : true);
                animator.SetBool("Ready", PlayerReady);
                timer = 0.0f;
            }

        }
    }

    void UpdateIndex()
    {
        SubColourIndex[0] = (ColourIndex + 2 > colors.Length - 1 ? ColourIndex + 2 - colors.Length : ColourIndex + 2);
        SubColourIndex[1] = (ColourIndex + 1 > colors.Length - 1 ? ColourIndex + 1 - colors.Length : ColourIndex + 1);

        SubColourIndex[2] = (ColourIndex - 1 < 0 ? ColourIndex - 1 + colors.Length : ColourIndex - 1);
        SubColourIndex[3] = (ColourIndex - 2 < 0 ? ColourIndex - 2 + colors.Length : ColourIndex - 2);

        for (int i = 0; i < Hairstyle.Length; i++)
        {
            Hairstyle[i].GetComponent<Renderer>().material.color = colors[ColourIndex];
        }

    }

}
