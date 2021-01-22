using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TheFirstPerson.Helper
{
    public class SimpleThirdPersonCamera : MonoBehaviour
    {
        public enum UpdateType
        {
            Update,
            LateUpdate,
            FixedUpdate
        }

        public Transform camTarget;
        public Transform camObject;
        public Vector3 pivotOffset = Vector3.zero;
        public Vector3 lookOffset = Vector3.zero;
        public float lookSensitivity = 1;
        public float verticalLookLimit;
        public bool mouseLookEnabled = true;
        public UpdateType positionUpdateType;
        public UpdateType lookUpdateType;

        void Start()
        {
            if (camTarget != null)
            {
                transform.position = camTarget.TransformPoint(pivotOffset);
                if (camObject != null)
                {
                    camObject.LookAt(camTarget.TransformPoint(lookOffset));
                }
                else
                {
                    Debug.LogError("Please assign a Transform to the camObject Value of The SimpleThirdPersonCamera attached to " + gameObject.name);
                }
            }
            else
            {
                Debug.LogError("Please assign a Transform to the camTarget Value of The SimpleThirdPersonCamera attached to " + gameObject.name);
            }
        }

        void Update()
        {
            if (positionUpdateType == UpdateType.Update)
            {
                UpdatePosition(Time.deltaTime);
            }
            if (lookUpdateType == UpdateType.Update)
            {
                UpdateLook(Time.deltaTime);
            }
        }

        void LateUpdate()
        {
            if (positionUpdateType == UpdateType.LateUpdate)
            {
                UpdatePosition(Time.deltaTime);
            }
            if (lookUpdateType == UpdateType.LateUpdate)
            {
                UpdateLook(Time.deltaTime);
            }
        }

        void FixedUpdate()
        {
            if (positionUpdateType == UpdateType.FixedUpdate)
            {
                UpdatePosition(Time.fixedDeltaTime);
            }
            if (lookUpdateType == UpdateType.FixedUpdate)
            {
                UpdateLook(Time.fixedDeltaTime);
            }
        }

        //TODO: Implement position Smoothing
        void UpdatePosition(float dt)
        {
            if (camTarget != null)
            {
                transform.position = camTarget.TransformPoint(pivotOffset);
            }
        }

        //TODO: Implement look Smoothing
        void UpdateLook(float dt)
        {
            if (camTarget != null && camObject != null)
            {
                if (mouseLookEnabled)
                {
                    Vector2 lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                    float horizontalLook = transform.localEulerAngles.y;
                    float verticalLook = transform.localEulerAngles.x;

                    lookInput = lookInput * lookSensitivity;
                    horizontalLook += lookInput.x;
                    verticalLook -= lookInput.y;
                    if (verticalLook > verticalLookLimit && verticalLook < 180)
                    {
                        verticalLook = verticalLookLimit;
                    }
                    else if (verticalLook > 180 && verticalLook < 360 - verticalLookLimit)
                    {
                        verticalLook = 360 - verticalLookLimit;
                    }
                    transform.localEulerAngles = new Vector3(verticalLook, horizontalLook, 0);
                }

                camObject.LookAt(camTarget.TransformPoint(lookOffset));
            }
        }
    }
}
