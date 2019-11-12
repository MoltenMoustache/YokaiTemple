using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
using TMPro;

public class PlayerSelectionManager : MonoBehaviour
{
    public GameObject[] Player;
    public PlayerSelectionBehaviour[] PBehaviour;
    public GameObject[] ColourPicker;
    public int ReadyPlayers = 0;
    public TextMeshProUGUI CountDownText;
    public TextMeshProUGUI[] PlayerNameText;
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
        if (ReadyPlayers != XCI.GetNumPluggedCtrlrs() || XCI.GetNumPluggedCtrlrs() == 0)
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
        PlayerNameText[3].text = (XCI.GetNumPluggedCtrlrs() > 3 ? (!PBehaviour[3].PlayerReady ? "Player Four" : "Ready") : "Connect Controller");
        ColourPicker[3].SetActive(XCI.GetNumPluggedCtrlrs() > 3 && !PBehaviour[3].PlayerReady);

        Player[2].SetActive(XCI.GetNumPluggedCtrlrs() > 2);
        PlayerNameText[2].text = (XCI.GetNumPluggedCtrlrs() > 2 ? (!PBehaviour[2].PlayerReady ? "Player Three" : "Ready") : "Connect Controller");
        ColourPicker[2].SetActive(XCI.GetNumPluggedCtrlrs() > 2 && !PBehaviour[2].PlayerReady);

        Player[1].SetActive(XCI.GetNumPluggedCtrlrs() > 1);
        PlayerNameText[1].text = (XCI.GetNumPluggedCtrlrs() > 1 ? (!PBehaviour[1].PlayerReady ? "Player Two" : "Ready") : "Connect Controller");
        ColourPicker[1].SetActive(XCI.GetNumPluggedCtrlrs() > 1 && !PBehaviour[1].PlayerReady);


        Player[0].SetActive(XCI.GetNumPluggedCtrlrs() > 0);
        PlayerNameText[0].text = (XCI.GetNumPluggedCtrlrs() > 0 ? (!PBehaviour[0].PlayerReady ? "Player One" : "Ready" ) : "Connect Controller");
        ColourPicker[0].SetActive(XCI.GetNumPluggedCtrlrs() > 0 && !PBehaviour[0].PlayerReady);

        ReadyPlayers = 0;

        for (int i = 0; i < 4; i++)
        {
            ReadyPlayers += (PBehaviour[i].PlayerReady ? 1 : 0);
        }

        if (ReadyPlayers == XCI.GetNumPluggedCtrlrs() && XCI.GetNumPluggedCtrlrs() > 0)
        {
            CountDownTimer = System.Convert.ToInt32(timer);
            CountDownText.text = System.Convert.ToString(CountDownTimer);
            if (CountDownTimer < 0 )
            {
                SceneManager.LoadScene(2);
            }
        }

    }

}
