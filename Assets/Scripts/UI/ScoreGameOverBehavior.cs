using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreGameOverBehavior : MonoBehaviour
{
    public struct PlayerScore
    {
        public string playerName;
        public int playerScore;
    }

    public GameObject[] scoreOBJ;
    TextMeshProUGUI[] scoreTXT;
    PlayerScore[] Player;


    // Start is called before the first frame update
    void Start()
    {
        Player = new PlayerScore[scoreOBJ.Length];
        scoreTXT = new TextMeshProUGUI[scoreOBJ.Length];

        Player[0].playerName = "One";
        Player[1].playerName = "Two";
        Player[2].playerName = "Three";
        Player[3].playerName = "Four";


        for (int i = 0; i < scoreOBJ.Length; i++)
        {
            scoreTXT[i] = scoreOBJ[i].GetComponent<TextMeshProUGUI>();

            Player[i].playerScore = i * (i + 1);

            scoreTXT[i].text = ("Player " + Player[i].playerName + ": " + Player[i].playerScore + "%");
        }

    }


}
