using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XboxCtrlrInput;

public class PlayerSelectBehavior : MonoBehaviour
{
    public int playerNum = 0;
    // Selction Buttons
    public Image ImageButtonLeft;
    public Image ImageButtonRight;
    // ---
    public UnityEngine.UI.Button LeftButton;
    public UnityEngine.UI.Button RightButton;
    public UnityEngine.UI.Button ConfirmButton;

    float timer = 0.0f;

    // Avatar
    [SerializeField]
    Sprite[] Avatar;
    public Image ImageAvatar;
    int TotalAvatar;
    int AvatarIndex;

    // Confirmation vars
    public bool Confirm = false;
    public bool PlayerConnected = true;

    public Sprite[] Avatar1 { get => Avatar; set => Avatar = value; }

    // Start is called before the first frame update
    void Start()
    {
        AvatarIndex = 0;
        TotalAvatar = Avatar1.Length;
        ImageAvatar.sprite = Avatar1[0];

        // Get button input
        LeftButton.onClick.AddListener(LeftButtonPressed);
        RightButton.onClick.AddListener(RightButtonPressed);
        ConfirmButton.onClick.AddListener(ConfirmButtonPressed);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * 2;

        PlayerConnected = (XCI.IsPluggedIn(playerNum) ? true : false);

        if (!PlayerConnected)
        {
            Color leftColor = ImageButtonLeft.color;
            Color rightColor = ImageButtonRight.color;
            Color confirmColor = ConfirmButton.image.color;
            Color avatarColor = ImageAvatar.color;

            leftColor.a = 0.1f;
            rightColor.a = 0.1f;
            confirmColor.a = 0.1f;
            avatarColor.a = 0.5f;

            ImageButtonLeft.color = leftColor;
            ImageButtonRight.color = rightColor;
            ConfirmButton.image.color = confirmColor;
            ImageAvatar.color = avatarColor;

            LeftButton.interactable = false;
            RightButton.interactable = false;
            ConfirmButton.interactable = false;
            Confirm = false;
        }
        else
        {
            Color leftColor = ImageButtonLeft.color;
            Color rightColor = ImageButtonRight.color;
            Color confirmColor = ConfirmButton.image.color;
            Color avatarColor = ImageAvatar.color;

            leftColor.a = 1.0f;
            rightColor.a = 1.0f;
            confirmColor.a = 1.0f;
            avatarColor.a = 1.0f;

            ImageButtonLeft.color = leftColor;
            ImageButtonRight.color = rightColor;
            ConfirmButton.image.color = confirmColor;
            ImageAvatar.color = avatarColor;

            LeftButton.interactable = true;
            RightButton.interactable = true;
            ConfirmButton.interactable = true;

        }

    }

    public void LeftButtonPressed()
    {
        if (!Confirm)
        {
            ImageAvatar.sprite = Avatar1[AvatarIndex];
            AvatarIndex++;

            if (AvatarIndex > TotalAvatar - 1)
                AvatarIndex = 0;
        }
    }

    public void RightButtonPressed()
    {
        if (!Confirm)
        {
            ImageAvatar.sprite = Avatar1[AvatarIndex];
            AvatarIndex--;

            if (AvatarIndex < 0)
                AvatarIndex = TotalAvatar - 1;
        }
    }

    public void ConfirmButtonPressed()
    {
        Confirm = (Confirm ? false : true);

        ConfirmButton.image.color = (!Confirm ? Color.white : Color.green);
    }
}
