using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MenuNav : MonoBehaviour
{
    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button optionButton;
    public UnityEngine.UI.Button exitButton;


    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(playButtonClick);
        optionButton.onClick.AddListener(optionsButtonClick);
        exitButton.onClick.AddListener(exitButtonClick);
    }

    void playButtonClick()
    {
        SceneManager.LoadScene(2);
    }

    void optionsButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    void exitButtonClick()
    {
        Application.Quit();
    }
}
