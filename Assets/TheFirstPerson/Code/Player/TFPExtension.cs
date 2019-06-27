using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheFirstPerson;

/*
The base Extension class

Extensions should inherit from this instead of MonoBehaviour
if you want to use a function in your extension you will need to override it in your script

Find some examples in the ExampleExtensions folder and an example of extension use in
the JetpackPlayer prefab in the Prefabs folder.

More event based functions to be added in future.
*/

public class TFPExtension : MonoBehaviour{

    //executes at the end of the Start function in FPSController.cs
    public virtual void ExStart(ref TFPData data, TFPInfo info){}

    //executes at the start of the Update function in FPSController.cs
    public virtual void ExPreUpdate(ref TFPData data, TFPInfo info){}

    //executes at the end of the Update function in FPSController.cs
    public virtual void ExPostUpdate(ref TFPData data, TFPInfo info){}

    //executes during the FixedUpdate function in FPSController.cs. Use for physics interactions
    public virtual void ExFixedUpdate(ref TFPData data, TFPInfo info){}

    /*
    executes before the controller.Move function is called in FPSController.cs
    YVel is applied to current move after this
    This means that YVel represents the vertical movement
    and currentmove represents the horizontal movement

    use this if you want to add custom movement functionality
    */
    public virtual void ExPreMove(ref TFPData data, TFPInfo info){}
}
