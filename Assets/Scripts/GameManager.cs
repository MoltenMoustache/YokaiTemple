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
    [Header("Progression")]
    public int ritualProgress = 0;
    public int maxRitual;
    [SerializeField] Checkpoint[] checkpoints;
    GameObject[] checkpointObjects;
    List<GameObject> players = new List<GameObject>();
    GameObject[] currentPlayers;
    int currentPlayerCount;

    [Header("References")]
    public GameObject playerPrefab;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject endingCanvas;
    bool gameWon = false;

    // Start is called before the first frame update
    void Start()
    {
        InitializePlayers();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckGameOver())
            StartCoroutine(EndGame(gameWon));
    }

    void InitializePlayers()
    {
        // Gets the number of connected controllers
        int playersConnected = XCI.GetNumPluggedCtrlrs();

        ////DEBUG
        //if (playersConnected == 0)
        //{
        //    playersConnected = 4;
        //}

        Debug.Log("Players Connected: " + playersConnected);
        // Loops through the connected controllers and adds the player to the game
        for (int i = 0; i < playersConnected; i++)
        {
            // Instantiates player object
            GameObject playerObj = Instantiate(playerPrefab, new Vector3(1, 0, 5), Quaternion.identity);
            // Creates reference to the PlayerController component on the object
            PlayerController playerCont = playerObj.GetComponent<PlayerController>();

            // Finds player number
            int playerNumber = i + 1;
            string hudTag = ("P" + playerNumber + "HUD");
            Debug.Log(hudTag);

            // Finds the HUD for the corresponding player number
            GameObject hud = GameObject.FindGameObjectWithTag(hudTag);

            // Assigns player number and HUD to player
            playerCont.InitializePlayer(hud, (XboxController)playerNumber);

            // Adds player to list of players
            players.Add(playerObj);

            // Adds 50 to the maximum ritual requisite.
            maxRitual += 50;
        }

        if (playersConnected < 4)
        {
            for (int i = 4; i > playersConnected; i--)
            {
                Debug.Log("P" + i + "HUD" + " disabled");
                GameObject.FindGameObjectWithTag(("P" + i + "HUD")).SetActive(false);
            }
        }
    }

    bool CheckGameOver()
    {
        // Checks if all players are downed, if so game is over
        if (CheckAllPlayersDowned())
            return true;

        // Checks if ritual is complete, if so game is over
        if (ritualProgress >= maxRitual)
        {
            gameWon = true;
            return true;
        }

        return false;
    }

    bool CheckAllPlayersDowned()
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

    public bool CheckForDownedPlayers()
    {
        foreach (var player in players)
        {
            if (player.GetComponent<PlayerController>().isDown)
            {
                //Debug.Log("Played downed!");
                return true;
            }
        }

        //Debug.Log("No players downed");
        return false;
    }

    public void IncrementProgress(int a_amount = 1)
    {
        ritualProgress += a_amount;

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
            endingCanvas.SetActive(true);
            gameCanvas.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            endingCanvas.SetActive(true);
            gameCanvas.SetActive(false);
        }
    }

    public void DecrementProgress(int a_amount = 1)
    {
        ritualProgress -= a_amount;
        if (ritualProgress < 0)
            ritualProgress = 0;
    }

    void ToggleProgressObjects(GameObject[] a_objects, Color a_color)
    {
        foreach (GameObject currentObject in a_objects)
        {
            currentObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", a_color);
        }
    }
}
