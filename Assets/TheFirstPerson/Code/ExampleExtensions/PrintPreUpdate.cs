using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

public class PrintPreUpdate : TFPExtension{
    public override void ExPreUpdate(ref TFPData data){
        print("pre updated extension!");
    }
}
