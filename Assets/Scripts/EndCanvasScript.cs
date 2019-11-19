using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndCanvasScript : MonoBehaviour
{
    public void GoToCharSelect()
    {
        SceneManager.LoadScene(1); 
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
