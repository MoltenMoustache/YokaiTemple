using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCanvasScript : MonoBehaviour
{
    public GameObject[] ButtonObjects;
    bool FirstFrame = true;
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

    private void Update()
    {
        if (FirstFrame)
        {
            ButtonObjects[0].GetComponent<UnityEngine.UI.Button>().Select();
            FirstFrame = false;
        }
    }
}
