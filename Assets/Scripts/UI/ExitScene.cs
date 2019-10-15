using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class ExitScene : MonoBehaviour
{
    UnityEngine.UI.Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<UnityEngine.UI.Button>();

        button.onClick.AddListener(onButtonClick);
    }

    // Update is called once per frame
    void onButtonClick()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (XCI.GetButtonDown(XboxCtrlrInput.XboxButton.B))
        {
            onButtonClick();
        }
    }
}
