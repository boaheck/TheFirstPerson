using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

public class PrintPreUpdate : TFPExtension{
    public override void ExPreUpdate(ref TFPData data, TFPInfo info){
        print("pre updated extension!");
    }
}
