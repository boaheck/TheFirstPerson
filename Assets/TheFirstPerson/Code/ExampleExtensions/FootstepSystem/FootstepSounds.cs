using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

//A dynamic footstep system. due to how the terrain texture is retrieved this will not function in versions earlier than 2018.3
//this should be placed on the same object as FPSController.cs
//leftFoot and rightFoot can be the same audio source if you don't want to have seperate sources for the feet

public class FootstepSounds : TFPExtension
{

    public float distanceBetweenFootsteps;
    public AudioSource leftFoot, rightFoot;
    public FootstepGroup[] footsteps;

    public FootstepGroup defaultFootsteps;
    float leftDistance, rightDistance;

    RaycastHit groundTypeCheck;

    public override void ExStart(ref TFPData data, TFPInfo info)
    {
        leftDistance = 0;
        rightDistance = distanceBetweenFootsteps;
    }

    public override void ExPostUpdate(ref TFPData data, TFPInfo info)
    {
        if (data.moving && data.grounded)
        {
            float deltaMove = Vector3.Scale(data.lastMove, new Vector3(1, 0, 1)).magnitude * Time.deltaTime;
            leftDistance -= deltaMove;
            rightDistance -= deltaMove;
            if (leftDistance <= 0)
            {
                leftDistance += distanceBetweenFootsteps * 2;
                leftFoot.PlayOneShot(GetClip(info));
            }
            if (rightDistance <= 0)
            {
                rightDistance += distanceBetweenFootsteps * 2;
                rightFoot.PlayOneShot(GetClip(info));

            }
        }
        else
        {
            leftDistance = 0;
            rightDistance = distanceBetweenFootsteps;
        }

    }

    AudioClip GetClip(TFPInfo info)
    {
        if (Physics.SphereCast(transform.position + (Vector3.up * info.controller.radius), info.controller.radius, Vector3.down, out groundTypeCheck, info.crouchHeadHitLayerMask.value))
        {
            TerrainCollider hitTerrain = groundTypeCheck.transform.GetComponent<TerrainCollider>();
            MeshRenderer hitMesh = groundTypeCheck.transform.GetComponent<MeshRenderer>();
            Texture2D hitTexture;
            if (hitTerrain != null)
            {
                hitTexture = TerrainSurface.GetMainTexture(groundTypeCheck.transform.GetComponent<Terrain>(), transform.position);
            }
            else if (hitMesh != null)
            {
                hitTexture = hitMesh.material.mainTexture as Texture2D;
            }
            else
            {
                return defaultFootsteps.footSounds[Random.Range(0, defaultFootsteps.footSounds.Length)];
            }
            foreach (FootstepGroup fsGroup in footsteps)
            {
                foreach (Texture2D tex in fsGroup.textures)
                {
                    if (hitTexture == tex)
                    {
                        return fsGroup.footSounds[Random.Range(0, fsGroup.footSounds.Length)];
                    }
                }
            }
        }
        return defaultFootsteps.footSounds[Random.Range(0, defaultFootsteps.footSounds.Length)];
    }

}
