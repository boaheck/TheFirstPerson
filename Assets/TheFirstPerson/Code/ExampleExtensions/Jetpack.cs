using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

public class Jetpack : TFPExtension{

    public float jetPower;
    public bool limitFuel;
    public float fuel;
    float fuelLeft;

    public override void ExStart(ref TFPData data){
        fuelLeft = fuel;
    }

    public override void ExPreMove(ref TFPData data){
        if((!data.grounded || data.slide) && !data.jumping){
            if(data.jumpPressed > 0){
                data.timeSinceGrounded = 1;
                data.jumpPressed = 0;
            }
            if(data.jumpHeld && fuelLeft > 0){
                print("Jetting");
                data.yVel = jetPower;
                if(limitFuel){
                    fuelLeft -= Time.deltaTime;
                }
            }
        }else if(data.grounded){
            fuelLeft = fuel;
        }
    }
}
