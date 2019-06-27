using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

public class PrintPostUpdate : TFPExtension{
    public override void ExPostUpdate(ref TFPData data, TFPInfo info){
        print("Post Updated extension!");
    }
}
