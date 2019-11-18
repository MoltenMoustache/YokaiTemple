using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerAnimations : MonoBehaviour
{

    public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (XCI.GetButtonDown(XboxButton.X))
        {
            anim.SetBool("isAttacking", true);
        }
        else
        {
            anim.SetBool("isAttacking", false);
        }

        //if (currentHealth > 0)
        //{
          //  anim.SetBool("isDeath", true);
       // }
       // else
       // {
          //  anim.SetBool("isDeath", false);
        //}
    }
}
