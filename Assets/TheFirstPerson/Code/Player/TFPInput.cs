using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomInput", menuName = "TFP/CustomInput", order = 1)]
public class TFPInput : ScriptableObject
{
    public bool useFPSControllerAxisNames;
    public string jumpBtn;
    public string crouchBtn;
    public string runBtn;
    public string unlockMouseBtn;
    public string xInName;
    public string yInName;
    public string xMouseName;
    public string yMouseName;

    public virtual void SetAxisNames(string jumpBtn, string crouchBtn, 
        string runBtn, string unlockMouseBtn, string xInName, 
        string yInName, string xMouseName, string yMouseName)
    {
        this.jumpBtn = jumpBtn;
        this.crouchBtn = crouchBtn;
        this.runBtn = runBtn;
        this.unlockMouseBtn = unlockMouseBtn;
        this.xInName = xInName;
        this.yInName = yInName;
        this.xMouseName = xMouseName;
        this.yMouseName = yMouseName;
    }

    public virtual float XAxis()
    {
        return Input.GetAxisRaw(xInName);
    }

    public virtual float YAxis()
    {
        return Input.GetAxisRaw(yInName);
    }

    public virtual float XMouse()
    {
        return Input.GetAxis(xMouseName);
    }

    public virtual float YMouse()
    {
        return Input.GetAxis(yMouseName);
    }

    public virtual bool CrouchPressed()
    {
        return Input.GetButtonDown(crouchBtn);
    }

    public virtual bool CrouchHeld()
    {
        return Input.GetButton(crouchBtn);
    }

    public virtual bool RunPressed()
    {
        return Input.GetButtonDown(runBtn) || Input.GetAxisRaw(runBtn) > 0.1f;
    }

    public virtual bool RunHeld()
    {
        return Input.GetButton(runBtn) || Input.GetAxisRaw(runBtn) > 0.1f;
    }

    public virtual bool JumpHeld()
    {
        return Input.GetButton(jumpBtn);
    }

    public virtual bool JumpPressed()
    {
        return Input.GetButtonDown(jumpBtn);
    }

    public virtual bool UnlockMouseButton()
    {
        return Input.GetButtonDown(unlockMouseBtn);
    }
}
