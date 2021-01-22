using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheFirstPerson;
using UnityEngine;

public class DebugLine : TFPExtension
{

    public Vector3[] pos;
    public int historyLength = 100;
    public float decayTime = 10;
    public Color col = Color.red;

    public override void ExStart(ref TFPData data, TFPInfo info)
    {
        pos = new Vector3[] {transform.position};
    }

    public override void ExPreMove(ref TFPData data, TFPInfo info)
    {
        UpdateLine();
    }

    public override void ExPostMove(ref TFPData data, TFPInfo info)
    {
        UpdateLine();
    }

    void UpdateLine()
    {
        if (pos[0] != transform.position)
        {
            DrawLine(pos[0], transform.position);
            pos = pos.Prepend(transform.position).ToArray();
            if (pos.Length > historyLength)
            {
                pos = pos.Take(historyLength).ToArray();
            }
        }
    }
    
    void DrawLine(Vector3 a, Vector3 b)
    {
        Debug.DrawLine(a, b, col, decayTime);
    }
}
