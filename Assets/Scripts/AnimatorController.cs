﻿#if USE_JOYSTICK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    //public DualStickShooterCharaMotor charaMotor;
    public Animator animator;

    public string
        forwardBackwardParamName = "Vertical",
        strafeParamName = "Horizontal",
        speedParamName = "Speed";

    // ------------------
    void OnEnable()
    {
        if (this.animator == null)
            this.animator = this.GetComponent<Animator>();

        if (this.charaMotor == null)
            this.charaMotor = this.GetComponent<DualStickShooterCharaMotor>();
    }


    // -----------------	
    void Update()
    {
        if ((this.animator == null) || (this.charaMotor == null))
            return;

        Vector2 v = new Vector2(this.charaMotor.GetLocalDir().x, this.charaMotor.GetLocalDir().z);


        if (!string.IsNullOrEmpty(this.speedParamName))
            this.animator.SetFloat(this.speedParamName, v.magnitude);

        if (!string.IsNullOrEmpty(this.forwardBackwardParamName))
            this.animator.SetFloat(this.forwardBackwardParamName, v.y);
        if (!string.IsNullOrEmpty(this.strafeParamName))
            this.animator.SetFloat(this.strafeParamName, v.x);
    }
}
#endif