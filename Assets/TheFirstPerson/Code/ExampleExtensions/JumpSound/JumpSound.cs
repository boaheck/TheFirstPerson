using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

//plays a sound when you jump. the simplest example plugin.

public class JumpSound : TFPExtension
{

    public AudioSource src;
    public AudioClip sound;

    public override void ExPostUpdate(ref TFPData data, TFPInfo info)
    {
        if (data.timeSinceGrounded <= Time.deltaTime && data.jumping)
        {
            src.PlayOneShot(sound);
        }
    }

}
