using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

public class TFPExtension : MonoBehaviour{
    public virtual void ExStart(ref TFPData data){}
    public virtual void ExPreUpdate(ref TFPData data){}
    public virtual void ExPostUpdate(ref TFPData data){}
    public virtual void ExPreMove(ref TFPData data){}
}
