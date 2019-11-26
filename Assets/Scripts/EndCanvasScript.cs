using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class EndCanvasScript : MonoBehaviour
{
    public GameObject[] ButtonObjects;
    bool FirstFrame = true;
    int index = 0;
    public void Awake()
    {
        ButtonObjects[0].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Replay);
        ButtonObjects[1].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(MainMenu);
    }

    public void Replay()
    {
        SceneManager.LoadScene(1); 
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Clicked()
    {
        if (index == 0)
            Replay();
        else
            MainMenu();
    }
    private void Update()
    {



        if (XCI.GetButton(XboxButton.A, XboxController.All))
        {
            Replay();
        }

        if (XCI.GetButton(XboxButton.B, XboxController.All))
        {
            MainMenu();
        }
    }
}
