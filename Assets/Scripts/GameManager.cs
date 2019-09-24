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

    public int ritualProgress = 0;
    public int maxRitual;
    [SerializeField] Slider progressBar;
    [SerializeField] Checkpoint[] checkpoints;
    GameObject[] checkpointObjects;
    [SerializeField] GameObject[] players;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (checkpoints.Length > 0)
        {
            foreach (Checkpoint check in checkpoints)
            {
                if (ritualProgress >= check.progress)
                {
                    checkpointObjects = GameObject.FindGameObjectsWithTag(check.tag);
                    DisableObjectArray(checkpointObjects, check.color);
                }
            }
        }

        if (XCI.GetNumPluggedCtrlrs() > 0)
        {
            for (int i = 0; i < XCI.GetNumPluggedCtrlrs(); i++)
            {
                players[i].GetComponent<PlayerController>().JoinGame();
            }

            UpdatePlayerCount();
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

        if (ritualProgress >= maxRitual)
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

    //public bool CheckWin()
    //{
    //    // Gets an array of all players current in the game
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    //    // Loops through each player
    //    foreach (GameObject player in players)
    //    {
    //        // If a player has not finished the ritual, the game is not complete and the function returns false.
    //        if (!player.GetComponent<PlayerController>().hasFinishedRitual)
    //        {
    //            return false;
    //        }
    //    }

    //    // Otherwise, the game is complete and the functon returns true.
    //    return true;
    //}

    void DisableObjectArray(GameObject[] a_objects, Color a_color)
    {
        foreach (GameObject currentObject in a_objects)
        {
            //currentObject.SetActive(false);
            currentObject.GetComponent<MeshRenderer>().material.color = a_color;
        }
    }
}
