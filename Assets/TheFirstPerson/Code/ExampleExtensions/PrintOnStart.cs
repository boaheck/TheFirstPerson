using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

public class PrintOnStart : TFPExtension{
    public override void ExStart(ref TFPData data, TFPInfo info){
        print("started new extension!");
    }
}
