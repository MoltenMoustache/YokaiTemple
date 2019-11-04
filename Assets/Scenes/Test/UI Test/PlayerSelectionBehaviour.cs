using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
public class PlayerSelectionBehaviour : MonoBehaviour
{
    public int PlayerNum = 0;
    public int HairstyleIndex = 0;
    public GameObject[] Hairstyle;
    public Animator animator;
    public bool PlayerReady;
    public float WaitTime = 2.0f;
    float timer = 0.0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * 2.0f;
        if (!PlayerReady)
        {
            if (XCI.GetAxis(XboxAxis.LeftStickX, (XboxController)PlayerNum) > 0 && timer > WaitTime)
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
            else if (XCI.GetAxis(XboxAxis.LeftStickX, (XboxController)PlayerNum) < 0 && timer > WaitTime)
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
}
