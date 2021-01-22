using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheFirstPerson
{
    /*
    The base Extension class

    Extensions should inherit from this instead of MonoBehaviour
    if you want to use a function in your extension you will need to override it in your script

    Find some examples in the ExampleExtensions folder and an example of extension use in
    the JetpackPlayer prefab in the Prefabs folder.

    More event based functions to be added in future.
    */

    public class TFPExtension : MonoBehaviour
    {

        //executes at the end of the Start function in FPSController.cs
        public virtual void ExStart(ref TFPData data, TFPInfo info) { }

        //executes at the start of the Update function in FPSController.cs
        public virtual void ExPreUpdate(ref TFPData data, TFPInfo info) { }

        //executes at the end of the Update function in FPSController.cs
        public virtual void ExPostUpdate(ref TFPData data, TFPInfo info) { }

        //executes at the start of the FixedUpdate function in FPSController.cs. Use for physics interactions
        public virtual void ExPreFixedUpdate(ref TFPData data, TFPInfo info) { }

        //executes at the end of the FixedUpdate function in FPSController.cs. Use for physics interactions
        public virtual void ExPostFixedUpdate(ref TFPData data, TFPInfo info) { }

        //Executes before movement is calculated but after input is processed. This is useful if you want to modify variables that will be used in movement calculation
        public virtual void ExPreMoveCalc(ref TFPData data, TFPInfo info) { }
        /*
        executes before the controller.Move function is called in FPSController.cs
        YVel is applied to current move after this
        This means that YVel represents the vertical movement
        and currentmove represents the horizontal movement

        use this if you want to add custom movement functionality
        */
        public virtual void ExPreMove(ref TFPData data, TFPInfo info) { }

        //Executes at the end of the move function at this point all movement for the frame has been handled
        public virtual void ExPostMove(ref TFPData data, TFPInfo info) { }

        //executes after input is retrieved, allows you to override input behaviour, useful for things like disabling jumping or running
        public virtual void ExPostInput(ref TFPData data, TFPInfo info) { }
    }
}
