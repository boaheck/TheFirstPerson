using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;
using UnityEngine.UI;

//A simple jetpack script, useful for testing vertical level design or just having a jetpack in a game
//Does not need to be on the same object as FPSController.cs

public class Jetpack : TFPExtension
{

    public float jetPower;
    public bool limitFuel;
    public float fuel;
    public float fuelRegain;
    public Text fuelGuage;
    public AudioSource jetSound;
    public float soundFadeSpeed;
    float fuelLeft;
    bool jetting;

    public override void ExStart(ref TFPData data, TFPInfo info)
    {
        fuelLeft = fuel;
    }

    public override void ExPreMove(ref TFPData data, TFPInfo info)
    {
        if ((!data.grounded || data.slide) && !data.jumping)
        {
            if (data.jumpPressed > 0)
            {
                jetting = true;
                data.timeSinceGrounded = info.coyoteTime + Time.deltaTime;
                data.jumpPressed = 0;
            }
            if (data.jumpHeld && fuelLeft > 0)
            {
                jetting = true;
                data.yVel = jetPower;
                if (limitFuel)
                {
                    fuelLeft -= Time.deltaTime;
                    if (fuelLeft < 0)
                    {
                        fuelLeft = 0;
                    }
                }
                jetSound.volume = Mathf.MoveTowards(jetSound.volume, 1.0f, soundFadeSpeed * Time.deltaTime);
            }
            else
            {
                jetting = false;
            }
        }
        else if (data.grounded)
        {
            jetting = false;
            fuelLeft = Mathf.MoveTowards(fuelLeft, fuel, fuelRegain * Time.deltaTime);
        }
        else
        {
            jetting = false;
        }
        if (!jetting)
        {
            jetSound.volume = Mathf.MoveTowards(jetSound.volume, 0.0f, soundFadeSpeed * Time.deltaTime);
        }
        if (fuelLeft < fuel)
        {
            fuelGuage.text = "FUEL: " + fuelLeft.ToString("F2") + " / " + fuel;
        }
        else
        {
            fuelGuage.text = "";
        }
    }
}
