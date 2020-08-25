using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;
using System.Linq;

/*
UnwalkableTags

this allows you to prevent the controller from moving forward if it detects it'll pass over a collider with one of the Unwalkable tags.
collisionResolution is the distance away that the script checks points for walkable ground
collisionAngle is the angle by which it rotates this checked point in iterations until it finds walkable ground
 */

public class UnwalkableColliders : TFPExtension
{
    RaycastHit hit;
    public string[] Unwalkable;
    public float collisionResolution = 0.1f;
    public int collisionStepAngle = 5;
    public override void ExPreMove(ref TFPData data, TFPInfo info)
    {
        Vector3 dir = data.currentMove * Time.deltaTime;
        Vector3 checkPos = transform.position + dir;
        if (Physics.Raycast(checkPos + Vector3.up, Vector3.down, out hit))
        {
            string tag = hit.collider.tag;
            Vector3 step = -dir.normalized * collisionResolution;
            Vector3 nonTaggedPos = transform.position;
            if (Unwalkable.Contains(tag))
            {
                for (int a = -90; a < 90; a += collisionStepAngle)
                {
                    checkPos = (Quaternion.Euler(0, a, 0) * dir) + transform.position;
                    Physics.Raycast(checkPos + Vector3.up, Vector3.down, out hit);
                    tag = hit.collider.tag;
                    if (!Unwalkable.Contains(tag))
                    {
                        nonTaggedPos = checkPos;
                        if (a > 0)
                        {
                            break;
                        }
                    }
                }
                data.currentMove = nonTaggedPos - transform.position;
                data.currentMove *= 1 / Time.deltaTime;
            }
        }
    }
}
