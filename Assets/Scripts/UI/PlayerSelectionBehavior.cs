using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionBehavior : MonoBehaviour
{
    public GameObject[] Player;
    MeshFilter[] PlayerModel;

    public Mesh[] Model;


    // Start is called before the first frame update
    void Start()
    {
        PlayerModel = new MeshFilter[4];
        for (int i = 0; i < 4; i++)
        {
            PlayerModel[i] = Player[i].GetComponent<MeshFilter>();
            PlayerModel[i].mesh = Model[i];
        }

        
    }

    // Update is called once per frame
    void Update()
    {
       // Rotate models
        for (int i = 0; i < 4; i++)
        {
            PlayerModel[i].transform.Rotate(new Vector3(0, 5.0f * Time.deltaTime, 0));
        }
    }
}
