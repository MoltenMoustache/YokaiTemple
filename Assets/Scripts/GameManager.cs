using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XboxCtrlrInput;

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
    ConnectionManager connectionManager;

    public int ritualProgress = 0;
    public int maxRitual;
    [SerializeField] Slider progressBar;
    [SerializeField] Checkpoint[] checkpoints;
    GameObject[] checkpointObjects;
    [SerializeField] GameObject[] players;
    GameObject[] currentPlayers;
    int currentPlayerCount;

    // Pause Menu
    [SerializeField] GameObject pausePanel;

    // Start is called before the first frame update
    void Start()
    {
        connectionManager = new ConnectionManager();
        InitializePlayers();
    }

    // Update is called once per frame
    void Update()
    {

        //if (XCI.GetNumPluggedCtrlrs() != currentPlayerCount)
        //{
        //    currentPlayers = GameObject.FindGameObjectsWithTag("Player");
        //    for (int i = 0; i < XCI.GetNumPluggedCtrlrs(); i++)
        //    {
        //        players[i].GetComponent<PlayerController>().JoinGame();
        //    }

        //    UpdatePlayerCount();
        //    currentPlayerCount = XCI.GetNumPluggedCtrlrs();
        //}

        if (XCI.GetButtonDown(XboxButton.Start, XboxController.All))
        {
            PauseGame(!pausePanel.activeSelf);
        }
    }

    void CheckCheckpointProgression()
    {

    }

    public ConnectionManager GetConnectionManager()
    {
        return connectionManager;
    }

    void InitializePlayers()
    {
        int playersConnected = XCI.GetNumPluggedCtrlrs();
        for (int i = 0; i < playersConnected; i++)
        {
            players[i].GetComponent<PlayerController>().JoinGame();
        }
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
            EndGame(true);
        }
    }

    void EndGame(bool a_didWin)
    {
        if (a_didWin)
        {
            Time.timeScale = 0;
            GameObject[] yokai = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject yoke in yokai)
            {
                Destroy(yoke);
            }
            Debug.Log("Game Complete.");
        }
    }

    public void DecrementProgress(int a_amount = 1)
    {
        ritualProgress -= a_amount;
        if (ritualProgress < 0)
            ritualProgress = 0;

        progressBar.value = ritualProgress;
    }

    void PauseGame(bool a_pause)
    {
        if (a_pause)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
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
