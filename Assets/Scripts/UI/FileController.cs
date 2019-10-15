using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FileController : MonoBehaviour
{
    struct options
    {
        public bool bloom;
        public bool fullScreen;
        public bool VSync;
        public bool AimAssist;
        public bool XRay;

        public int resolutionX;
        public int resolutionY;
    }

    public List<GameObject> option;
    OptionBehavior[] optionBehaviors;
    string optionPath = "options.txt";
    options currentOptions;

    void Awake()
    {
        SceneManager.sceneUnloaded += SceneUnloaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        do
        {
            option.Add(gameObject.transform.GetChild(i).gameObject);
            i++;
        }
        while (gameObject.transform.childCount >= i + 1);

        optionBehaviors = new OptionBehavior[i];

        for (int x = 0; x < i; x++)
        {
            optionBehaviors[x] = option[x].GetComponent<OptionBehavior>();
        }

        if (File.Exists(optionPath))
        {
            using (var reader = new BinaryReader(File.Open(optionPath, FileMode.OpenOrCreate)))
            {
                for (int z = 0; z < optionBehaviors.Length; z++)
                {
                    int current = reader.ReadInt32();

                    if (optionBehaviors[z].isRes)
                        optionBehaviors[z].SetRes(current);
                    else
                        optionBehaviors[z].SetClicked(Convert.ToBoolean(current));
                }
            }
        }
        currentOptions = new options();
        currentOptions.bloom = optionBehaviors[0].GetBool();
        currentOptions.fullScreen = optionBehaviors[1].GetBool();
        currentOptions.VSync = optionBehaviors[2].GetBool();
        currentOptions.AimAssist = optionBehaviors[3].GetBool();
        currentOptions.XRay = optionBehaviors[4].GetBool();

        switch (optionBehaviors[5].GetRes())
        {
            case 0:
                currentOptions.resolutionX = 1920;
                currentOptions.resolutionY = 1080;
                break;
            case 1:
                currentOptions.resolutionX = 960;
                currentOptions.resolutionY = 720;
                break;
            case 2:
                currentOptions.resolutionX = 640;
                currentOptions.resolutionY = 480;
                break;
            default:
                break;
        }

        optionBehaviors[0].button.Select();

    }


    void SceneUnloaded<Scene>(Scene scene)
    {
        using (var writer = new BinaryWriter(File.Open(optionPath, FileMode.Create)))
        {
            for (int i = 0; i < optionBehaviors.Length; i++)
            {
                if (optionBehaviors[i].isRes)
                    writer.Write(optionBehaviors[i].GetRes());
                else
                    writer.Write(Convert.ToInt32(optionBehaviors[i].GetBool()));
            }
        }

    }
}
