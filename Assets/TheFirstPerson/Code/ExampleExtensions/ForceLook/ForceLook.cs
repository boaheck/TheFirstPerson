using System.Collections;
using System.Collections.Generic;
using TheFirstPerson;
using UnityEngine;

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
        if(disablePlayerLook && forceLook){
            data.mouseLookEnabled = false;
        }else{
            data.mouseLookEnabled = true;
        }
    }

    public override void ExPreMove(ref TFPData data, TFPInfo info)
    {
        if(forceLook)
        {
            GetLastLook(data,info);

            if(isAngle)
            {
                lookAngle = lookValue;
            }else
            {
                lookAngle = Quaternion.LookRotation(lookValue-info.cam.position).eulerAngles;
            }

            float newHorLook = Mathf.MoveTowardsAngle(lastHorLook,lookAngle.y,lookSpeed*Time.deltaTime);
            float newVerLook = Mathf.MoveTowardsAngle(lastVerLook,lookAngle.x,lookSpeed*Time.deltaTime);
            Vector3 newLook = new Vector3(newVerLook,newHorLook,0.0f);
            transform.eulerAngles = new Vector3(0.0f,newLook.y,0.0f);
            info.cam.localEulerAngles = new Vector3(newLook.x,0.0f,0.0f);
        }
    }

    void GetLastLook(TFPData data, TFPInfo info)
    {
        lastHorLook = transform.eulerAngles.y;
        lastVerLook = info.cam.localEulerAngles.x;
        lastLook = new Vector3(lastVerLook,lastHorLook,0.0f);
    }
}
