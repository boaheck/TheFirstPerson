using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheFirstPerson.Helper
{
    public class FPSLimiter : MonoBehaviour
    {
        [Header("For development testing purposes only")]
        public bool limitFPS = false;
        public int targetFPS = 60;

        void Start()
        {
            if (limitFPS)
            {
                Application.targetFrameRate = targetFPS;
            }
        }
    }
}
