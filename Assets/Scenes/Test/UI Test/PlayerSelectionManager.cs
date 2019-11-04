using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    public GameObject[] Player;
    public PlayerSelectionBehaviour[] PBehaviour;
    public int ReadyPlayers = 0;
    public TextMeshProUGUI CountDownText;
    public int CountDownTimer = 5;
    float timer = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        Player = new GameObject[4];
        
        PBehaviour = new PlayerSelectionBehaviour[4];
        for (int i = 0; i < 4; i++)
        {
            Player[i] = transform.GetChild(i).gameObject;
            PBehaviour[i] = Player[i].GetComponent<PlayerSelectionBehaviour>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ReadyPlayers != XCI.GetNumPluggedCtrlrs())
        {
            timer = 5.0f;
            CountDownText.gameObject.SetActive(false);
        }
        else
        {
            CountDownText.gameObject.SetActive(true);

            timer -= Time.deltaTime * 1.0f;
        }

        Player[3].SetActive(XCI.GetNumPluggedCtrlrs() > 3);
        Player[2].SetActive(XCI.GetNumPluggedCtrlrs() > 2);
        Player[1].SetActive(XCI.GetNumPluggedCtrlrs() > 1);
        Player[0].SetActive(XCI.GetNumPluggedCtrlrs() > 0);

        ReadyPlayers = 0;

        for (int i = 0; i < 4; i++)
        {
            ReadyPlayers += (PBehaviour[i].PlayerReady ? 1 : 0);
        }

        if (ReadyPlayers == XCI.GetNumPluggedCtrlrs())
        {
            CountDownTimer = System.Convert.ToInt32(timer);
            CountDownText.text = System.Convert.ToString(CountDownTimer);
            if (CountDownTimer < 0)
            {
                Debug.Log("Load Game Scene");
            }
        }
    }
}
