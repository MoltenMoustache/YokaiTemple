using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class buttonbehaviour : MonoBehaviour , ISelectHandler, IDeselectHandler
{
    public UnityEngine.UI.Button button;
    public bool isOn = false;
    bool isdown = false;
    float timer = 0.0f;

    private void Start()
    {
        button.Select();
    }


    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        isOn = true;
    }

    public void OnDeselect(BaseEventData data)
    {
        isOn = false;
        isdown = false;
    }

    public void Pressed()
    {
        isdown = true;
        timer = 1.0f;
    }

    public void Update()
    {
        if (timer > -1.0f)
            timer -= Time.deltaTime * 5;

        if (isdown)
        {
            transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            if (timer <= 0)
                isdown = false;
        }
        else
        {
            if (isOn)
                transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            else
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }


    }
}
