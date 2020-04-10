using System.Collections;
using System.Collections.Generic;
using TheFirstPerson;
using UnityEngine;

//this plugin will force the player to either look at a specific point in space or in a specific direction if isAngle is true
//disablePlayerLook can be set true to remove all player agency in wher they look, if it is false they will have to fight against the lookspeed to look away
//this script should be placed on the same object as FPSController.cs

public class ForceLook : TFPExtension
{

    public bool forceLook;
    public Vector3 lookValue;
    public bool isAngle;
    public bool disablePlayerLook;
    public float lookSpeed;

    float lastHorLook;
    float lastVerLook;
    Vector3 lastLook;
    Vector3 lookAngle;

    public override void ExPreUpdate(ref TFPData data, TFPInfo info)
    {
        if (disablePlayerLook && forceLook)
        {
            data.mouseLookEnabled = false;
        }
        else
        {
            data.mouseLookEnabled = true;
        }
    }

    public override void ExPreMove(ref TFPData data, TFPInfo info)
    {
        if (forceLook)
        {
            GetLastLook(data, info);

            if (isAngle)
            {
                lookAngle = lookValue;
            }
            else
            {
                lookAngle = Quaternion.LookRotation(lookValue - info.cam.position).eulerAngles;
            }

            float newHorLook = Mathf.MoveTowardsAngle(lastHorLook, lookAngle.y, lookSpeed * Time.deltaTime);
            float newVerLook = Mathf.MoveTowardsAngle(lastVerLook, lookAngle.x, lookSpeed * Time.deltaTime);
            Vector3 newLook = new Vector3(newVerLook, newHorLook, 0.0f);
            transform.eulerAngles = new Vector3(0.0f, newLook.y, 0.0f);
            info.cam.localEulerAngles = new Vector3(newLook.x, 0.0f, 0.0f);
        }
    }

    void GetLastLook(TFPData data, TFPInfo info)
    {
        lastHorLook = transform.eulerAngles.y;
        lastVerLook = info.cam.localEulerAngles.x;
        lastLook = new Vector3(lastVerLook, lastHorLook, 0.0f);
    }
}
