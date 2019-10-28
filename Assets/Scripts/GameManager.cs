using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XboxCtrlrInput;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [System.Serializable]
    class Checkpoint
    {
        public string tag;
        public int progress;
        public Color color;
    }

    // References
    [SerializeField] TextMeshProUGUI ritualText;

    // Connections
    //ConnectionManager connectionManager;

    public int ritualProgress = 0;
    public int maxRitual;
    [SerializeField] Slider progressBar;
    [SerializeField] Checkpoint[] checkpoints;
    GameObject[] checkpointObjects;
    [SerializeField] GameObject[] players;
    GameObject[] currentPlayers;
    int currentPlayerCount;
    [SerializeField] GameObject gameoverCanvas;
    [SerializeField] GameObject pauseCanvas;

    // Start is called before the first frame update
    void Start()
    {
       // connectionManager = new ConnectionManager();
        progressBar.maxValue = maxRitual;
        InitializePlayers();
    }

    // Update is called once per frame
    void Update()
    {
        //if (XCI.GetButtonDown(XboxButton.Start, XboxController.All))
        //{
        //    PauseGame(!pauseCanvas.activeSelf);
        //}

        if (CheckGameOver())
        {
            StartCoroutine(EndGame(false));
        }
    }

    //public ConnectionManager GetConnectionManager()
    //{
    //    return connectionManager;
    //}

    void InitializePlayers()
    {
        int playersConnected = XCI.GetNumPluggedCtrlrs();
        for (int i = 0; i < playersConnected; i++)
        {
            players[i].GetComponent<PlayerController>().JoinGame();
        }
    }

    bool CheckGameOver()
    {
        foreach (var player in players)
        {
            if (!player.GetComponent<PlayerController>().isDown)
            {
                return false;
            }
        }

        return true;
    }

    public void UpdatePlayerCount()
    {
        maxRitual = 50 * XCI.GetNumPluggedCtrlrs();
        if (maxRitual > 200)
        {
            maxRitual = 200;
        }
        else if (maxRitual < 100)
        {
            maxRitual = 100;
        }
        progressBar.maxValue = maxRitual;
    }

    public void IncrementProgress(int a_amount = 1)
    {
        ritualProgress += a_amount;
        progressBar.value = ritualProgress;

        if (checkpoints.Length > 0)
        {
            foreach (Checkpoint check in checkpoints)
            {
                if (ritualProgress >= check.progress)
                {
                    checkpointObjects = GameObject.FindGameObjectsWithTag(check.tag);
                    ToggleProgressObjects(checkpointObjects, check.color);
                }
            }
        }

        if (ritualProgress >= maxRitual)
        {
            StartCoroutine(EndGame(true));
        }
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public IEnumerator EndGame(bool a_didWin)
    {
        if (a_didWin)
        {
            GameObject[] yokai = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject yoke in yokai)
            {
                Destroy(yoke);
            }

            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadSceneAsync(0);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadSceneAsync(0);
        }
    }

    public void DecrementProgress(int a_amount = 1)
    {
        ritualProgress -= a_amount;
        if (ritualProgress < 0)
            ritualProgress = 0;

        progressBar.value = ritualProgress;
    }

    public void PauseGame(bool a_pause)
    {
        if (a_pause)
        {
            //Time.timeScale = 0.25f;
            pauseCanvas.SetActive(true);
        }
        else
        {
            //Time.timeScale = 1;
            pauseCanvas.SetActive(false);
        }
    }

    void ToggleProgressObjects(GameObject[] a_objects, Color a_color)
    {
        foreach (GameObject currentObject in a_objects)
        {
            currentObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", a_color);
        }
    }
}
