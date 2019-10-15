using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class PlayerReadyBehavior : MonoBehaviour
{
    public GameObject[] gameObjectPlayer;

    public PlayerSelectBehavior[] behaviorPlayer;

    float[] timer;
    int playersConnected = 0;
    int playersReady = 0;

    bool allPlayersReady = false;

    // Start is called before the first frame update
    void Start()
    {
        timer = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
        playersReady = 0;
        behaviorPlayer[0] = gameObjectPlayer[0].GetComponent<PlayerSelectBehavior>();
        behaviorPlayer[1] = gameObjectPlayer[1].GetComponent<PlayerSelectBehavior>();
        behaviorPlayer[2] = gameObjectPlayer[2].GetComponent<PlayerSelectBehavior>();
        behaviorPlayer[3] = gameObjectPlayer[3].GetComponent<PlayerSelectBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < playersConnected; i++)
        {
            timer[i] += Time.deltaTime * 5;
        }

        playersConnected = XCI.GetNumPluggedCtrlrs();

        allPlayersReady = (playersConnected == playersReady && playersConnected != 0);
        //Debug.Log(allPlayersReady);

        if (allPlayersReady)
        {
            SceneManager.LoadScene(0);
        }

        CheckInput(XboxController.First);
        CheckInput(XboxController.Second);
        CheckInput(XboxController.Third);
        CheckInput(XboxController.Fourth);

    }


    void CheckReady(XboxController pressedController)
    {
        behaviorPlayer[(int)pressedController].ConfirmButtonPressed();

        if (behaviorPlayer[(int)pressedController].Confirm)
        {
            playersReady++;
        }
        else
        {
            playersReady--;
        }
    }

    void LeftPressed(XboxController pressedController)
    {
        behaviorPlayer[(int)pressedController].LeftButtonPressed();
    }

    void RightPressed(XboxController pressedController)
    {
        behaviorPlayer[(int)pressedController].RightButtonPressed();
    }

    void CheckInput(XboxController currentController)
    {

        if (XCI.GetButtonDown(XboxCtrlrInput.XboxButton.A, currentController) && timer[(int)currentController - 1] > 1)
        {
            CheckReady(currentController - 1);
            timer[(int)currentController - 1] = 0.0f;
        }

        if (XCI.GetAxis(XboxAxis.LeftStickX, currentController) > 0 && timer[(int)currentController - 1] > 1)
        {
            LeftPressed(currentController - 1);
            timer[(int)currentController - 1] = 0.0f;
        }

        if (XCI.GetAxis(XboxAxis.LeftStickX, currentController) < 0 && timer[(int)currentController - 1] > 1)
        {
            RightPressed(currentController - 1);
            timer[(int)currentController - 1] = 0.0f;
        }

        if (XCI.GetButtonDown(XboxCtrlrInput.XboxButton.B, currentController) && timer[(int)currentController - 1] > 1)
        {
            if (behaviorPlayer[(int)currentController - 1].Confirm)
                CheckReady(currentController - 1);
            else
                SceneManager.LoadScene(0);

            timer[(int)currentController - 1] = 0.0f;
        }
    }
}
