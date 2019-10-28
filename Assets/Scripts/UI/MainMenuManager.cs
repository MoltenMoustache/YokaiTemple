using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public Camera MainCamera;
    public ButtonManager[] ButtonSet;

    public bool PlayerSelect = false;
    public Vector3 DefaultPosition;
    public Quaternion DefaultRotation;
    public Vector3 PlayerSelectPosition;
    public Quaternion PlayerSelectRotation;
    public GameObject PlayerSelectPositionObject;
    public float CameraSpeed = 0.3f;
    private float StartTime = 0.0f;
    private float PlayerSelectionJourneyLength = 0.0f;
    bool FirstFrameTime = false;
    public TextMeshProUGUI Title;
    Vector3 TitleDefaultPos;
    public float PlatformSpeed = 10f;
    public GameObject PlatForm;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
        DefaultPosition = MainCamera.transform.position;
        DefaultRotation = MainCamera.transform.rotation;
        PlayerSelectPosition = new Vector3(PlayerSelectPositionObject.transform.position.x, PlayerSelectPositionObject.transform.position.y, PlayerSelectPositionObject.transform.position.z);
        PlayerSelectRotation = PlayerSelectPositionObject.transform.rotation;

        PlayerSelectionJourneyLength = Vector3.Distance(DefaultPosition, PlayerSelectPosition);
        TitleDefaultPos = Title.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (ButtonSet[0].PlayerSelection)
        {
            PlayerSelect = true;
            Title.text = "Player Selection";
            Title.transform.localPosition = new Vector3(0, TitleDefaultPos.y + 150, 0);

            if (PlatForm.transform.position.y < -0.5)
            {
                PlatForm.transform.position = new Vector3(PlatForm.transform.position.x, PlatForm.transform.position.y + PlatformSpeed * Time.deltaTime, PlatForm.transform.position.z);
            }

        }
        else
        {
            PlayerSelect = false;
            Title.text = "UI TESTING";
            Title.transform.localPosition = TitleDefaultPos;

            if (PlatForm.transform.position.y > -15)
            {
                PlatForm.transform.position = new Vector3(PlatForm.transform.position.x, PlatForm.transform.position.y - PlatformSpeed * Time.deltaTime, PlatForm.transform.position.z);
            }
            else
            {
                if (PlatForm.transform.position.y < -15)
                {
                    PlatForm.transform.position = new Vector3(PlatForm.transform.position.x, -15, PlatForm.transform.position.z);
                }
            }
        }

        if (PlayerSelect)
        {
            if (!FirstFrameTime)
            {
                StartTime = Time.time;
            }
            FirstFrameTime = true;
            float distCovered = (Time.time - StartTime) * CameraSpeed;
            float fractionOfJourney = distCovered / PlayerSelectionJourneyLength;

            MainCamera.transform.position = Vector3.Lerp(DefaultPosition, PlayerSelectPosition, fractionOfJourney);
            MainCamera.transform.rotation = Quaternion.Lerp(DefaultRotation, PlayerSelectRotation, fractionOfJourney);


        }
        else
        {
            if (FirstFrameTime)
            {
                StartTime = Time.time;
            }
            FirstFrameTime = false;
            float distCovered = (Time.time - StartTime) * CameraSpeed;
            float fractionOfJourney = distCovered / PlayerSelectionJourneyLength;

            MainCamera.transform.position = Vector3.Lerp(PlayerSelectPosition, DefaultPosition, fractionOfJourney);
            MainCamera.transform.rotation = Quaternion.Lerp(PlayerSelectRotation, DefaultRotation, fractionOfJourney);

        }
    }
}
