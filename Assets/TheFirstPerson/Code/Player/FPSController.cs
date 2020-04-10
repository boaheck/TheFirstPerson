using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheFirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {

        CharacterController controller;


        [Header("Options")]
        public bool extensionsEnabled = false;
        public bool slopeSlideEnabled = true;
        public bool sprintEnabled = true;
        public bool momentumEnabled = true;
        public bool crouchEnabled = true;
        public bool jumpEnabled = true;
        public bool mouseLookEnabled = true;
        public bool verticalLookEnabled = true;
        public bool customCameraTransform = false;
        public bool customInputNames = false;
        [Range(0f, 1f)]
        public float airControl = 0.5f;
        public bool airSprintEnabled = true;

        [Header("Jump Settings")]
        [ConditionalHide("jumpEnabled", true)]
        public float jumpSpeed = 9;
        [ConditionalHide("jumpEnabled", true)]
        public bool variableHeight = true;
        [ConditionalHide("jumpEnabled", true)]
        public float coyoteTime = 0.1f;
        [ConditionalHide("jumpEnabled", true)]
        public float bunnyhopTolerance = 0.1f;
        [ConditionalHide(new string[] { "jumpEnabled", "variableHeight" }, true, false)]
        public float jumpGravityMult = 0.6f;
        [ConditionalHide(new string[] { "jumpEnabled", "variableHeight" }, true, false)]
        public float postJumpGravityMult = 3;
        [ConditionalHide(new string[] { "jumpEnabled", "slopeSlideEnabled" }, true, false)]
        public bool jumpWhileSliding = false;
        [ConditionalHide(new string[] { "jumpEnabled", "slopeSlideEnabled", "jumpWhileSliding" }, true, false)]
        public float slopeJumpKickbackSpeed = 10;

        [Header("Gravity Settings")]
        public float gravity = 15;
        public float baseGroundForce = 3;
        public float maxGroundForce = 30;
        public float gravityCap = 50;
        public float baseFallVelocity = 5;

        [Header("Air Control Settings")]
        public float airResistance = 4;
        public float airMoveSpeed = 6;
        public float airStrafeMult = 0.8f;
        public float airBackwardMult = 0.6f;
        [ConditionalHide("airSprintEnabled", true)]
        public float airSprintMult = 2;

        [Header("Speed Settings")]
        public float moveSpeed = 5;
        [ConditionalHide("slopeSlideEnabled", true)]
        public float slopeSlideSpeed = 10;
        [ConditionalHide("momentumEnabled", true)]
        public float acceleration = 50;
        [ConditionalHide("momentumEnabled", true)]
        public float deceleration = 40;
        [ConditionalHide("sprintEnabled", true)]
        public float sprintMult = 2;
        public float strafeMult = 0.8f;
        public float backwardMult = 0.6f;

        [Header("Mouse Look Settings")]
        public float sensitivity = 180;
        [ConditionalHide("verticalLookEnabled", true)]
        public float verticalLookLimit = 80;
        [ConditionalHide("customCameraTransform", true)]
        public Transform cam;

        [Header("CrouchSettings")]
        [ConditionalHide("crouchEnabled", true)]
        public bool crouchToggleStyle = false;
        [ConditionalHide("crouchEnabled", true)]
        public float crouchColliderHeight = 0.6f;
        [ConditionalHide("crouchEnabled", true)]
        public float crouchMult = 0.5f;
        [ConditionalHide("crouchEnabled", true)]
        public float crouchTransitionSpeed = 6;
        [ConditionalHide("crouchEnabled", true)]
        public LayerMask crouchHeadHitLayerMask;

        [Header("Input Names")]
        [ConditionalHide("customInputNames", true)]
        public string jumpBtnCustom = "Jump";
        [ConditionalHide("customInputNames", true)]
        public string crouchBtnCustom = "Fire1";
        [ConditionalHide("customInputNames", true)]
        public string runBtnCustom = "Fire3";
        [ConditionalHide("customInputNames", true)]
        string unlockMouseBtnCustom = "Cancel";
        [ConditionalHide("customInputNames", true)]
        public string xInNameCustom = "Horizontal";
        [ConditionalHide("customInputNames", true)]
        public string yInNameCustom = "Vertical";
        [ConditionalHide("customInputNames", true)]
        public string xMouseNameCustom = "Mouse X";
        [ConditionalHide("customInputNames", true)]
        public string yMouseNameCustom = "Mouse Y";

        public TFPExtension[] Extensions;

        //Input
        bool moving;
        bool jumpHeld;
        bool crouching;
        bool running;
        bool mouseLocked = true;
        float jumpPressed;
        float xIn;
        float yIn;
        float xMouse;
        float yMouse;

        //Vertical Movement
        bool jumping;
        bool grounded;
        float timeSinceGrounded;
        float yVel;
        float gravMult = 1;


        //General movement
        Vector3 lastMove;
        Vector3 currentMove;
        Vector3 forward;
        Vector3 side;
        Vector3 moveDelta;
        float currentStrafeMult;
        float currentBackwardMult;
        float currentMoveSpeed;
        float groundAngle;
        bool instantMomentumChange = false;

        //sliding
        bool slide;
        Vector3 hitNormal;
        Vector3 hitPoint;
        Vector3 slideMove;
        RaycastHit ledgeCheck;

        //crouching
        float standingHeight;
        float cameraOffset;
        RaycastHit headHit;

        //Input Name Defaults (assuming default unity axes are set up)
        string jumpBtn = "Jump";
        string crouchBtn = "Fire1";
        string runBtn = "Fire3";
        string unlockMouseBtn = "Cancel";
        string xInName = "Horizontal";
        string yInName = "Vertical";
        string xMouseName = "Mouse X";
        string yMouseName = "Mouse Y";

        TFPInfo controllerInfo;

        void Start()
        {
            controller = GetComponent<CharacterController>();

            //get the transform of a child with a camera component
            if (!customCameraTransform)
            {
                cam = transform.GetComponentInChildren<Camera>().transform;
            }

            standingHeight = controller.height;
            cameraOffset = standingHeight - cam.localPosition.y;

            //Handle custom input names
            if (customInputNames)
            {
                jumpBtn = jumpBtnCustom;
                crouchBtn = crouchBtnCustom;
                runBtn = runBtnCustom;
                unlockMouseBtn = unlockMouseBtnCustom;
                xInName = xInNameCustom;
                yInName = yInNameCustom;
                xMouseName = xMouseNameCustom;
                yMouseName = yMouseNameCustom;
            }
            controllerInfo = GetInfo();
            ExecuteExtension("Start");
        }

        void Update()
        {
            ExecuteExtension("PreUpdate");
            UpdateInput();
            UpdateMouseLock();
            if (mouseLocked)
            {
                MouseLook();
            }
            UpdateMovement();
            ExecuteExtension("PostUpdate");
        }

        void FixedUpdate()
        {
            ExecuteExtension("FixedUpdate");
        }

        void UpdateMovement()
        {
            forward = transform.forward;
            side = transform.right;
            currentMove = Vector3.zero;
            grounded = controller.isGrounded;
            slideMove = Vector3.zero;
            Vector3 lastMoveH = Vector3.Scale(lastMove, new Vector3(1, 0, 1));

            if (crouchEnabled && crouching)
            {
                if (controller.height > crouchColliderHeight)
                {
                    controller.height = Mathf.MoveTowards(controller.height, crouchColliderHeight, Time.deltaTime * crouchTransitionSpeed);
                }
            }
            else
            {
                if (controller.height < standingHeight)
                {
                    if (Physics.SphereCast(transform.position + (Vector3.up * 0.01f), controller.radius, Vector3.up, out headHit, standingHeight, crouchHeadHitLayerMask.value))
                    {
                        crouching = true;
                    }
                    else
                    {
                        controller.height = Mathf.MoveTowards(controller.height, standingHeight, Time.deltaTime * crouchTransitionSpeed);
                    }
                }
            }

            setCurrentMoveVars();
            Vector3 targetMove = GetHorizontalMove();
            controller.center = new Vector3(0, controller.height / 2.0f, 0);
            cam.localPosition = new Vector3(cam.localPosition.x, controller.height - cameraOffset, cam.localPosition.z);

            if (grounded && groundAngle >= controller.slopeLimit && slopeSlideEnabled)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out ledgeCheck, 0.1f + (controller.radius * 2)))
                {
                    if (Vector3.Angle(ledgeCheck.normal, Vector3.up) >= controller.slopeLimit)
                    {
                        slide = true;
                    }
                    else
                    {
                        slide = false;
                    }
                }
                else if (Physics.Raycast(transform.position + (controller.radius * Vector3.up), new Vector3(hitPoint.x, transform.position.y, hitPoint.z) - transform.position, out ledgeCheck, 0.1f + (controller.radius * 2)))
                {
                    if (Vector3.Angle(ledgeCheck.normal, Vector3.up) >= controller.slopeLimit)
                    {
                        slide = true;
                    }
                    else
                    {
                        slide = false;
                    }
                }
                else
                {
                    slide = false;
                    groundAngle = 0;
                }
            }
            else
            {
                slide = false;
            }

            if (slide)
            {
                slideMove = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref slideMove);
                Vector3 slideMoveh = Vector3.Scale(slideMove, new Vector3(1, 0, 1)).normalized * slopeSlideSpeed;
                if (Vector3.Angle(targetMove, slideMoveh) > 100)
                {
                    targetMove = slideMoveh;
                }
                else
                {
                    targetMove += slideMoveh;
                }
                if (jumpEnabled && jumpWhileSliding)
                {
                    if (jumpPressed > 0)
                    {
                        targetMove = slideMoveh.normalized * slopeJumpKickbackSpeed;
                        instantMomentumChange = true;
                    }
                }
            }

            if (grounded)
            {
                if (momentumEnabled && !instantMomentumChange)
                {
                    if (moving || targetMove.magnitude < lastMoveH.magnitude)
                    {
                        currentMove = Vector3.MoveTowards(lastMoveH, targetMove, Time.deltaTime * deceleration);
                    }
                    else
                    {
                        currentMove = Vector3.MoveTowards(lastMoveH, targetMove, Time.deltaTime * acceleration);
                    }
                }
                else
                {
                    currentMove = targetMove;
                }
                var targetYVel = -baseGroundForce + (-maxGroundForce * (groundAngle / 90.0f));
                if (lastMove.y < 0)
                {
                    yVel = Mathf.Lerp(lastMove.y, targetYVel, gravity * Time.deltaTime);
                }
                else
                {
                    yVel = targetYVel;
                }
                timeSinceGrounded = 0;
                jumping = false;
                if (jumpEnabled && (jumpWhileSliding || !slide))
                {
                    if (jumpPressed > 0)
                    {
                        Jump();
                    }
                }
                else if (slide)
                {
                    if (jumpPressed > 0)
                    {
                        jumpPressed -= Time.deltaTime;
                    }
                }
            }
            else
            {
                if ((controller.collisionFlags & CollisionFlags.Above) != 0 && yVel > 0)
                {
                    yVel = 0;
                }
                if (moving)
                {
                    currentMove = Vector3.Lerp(lastMoveH, targetMove, airControl);
                }
                else
                {
                    currentMove = Vector3.MoveTowards(lastMoveH, targetMove, airResistance * Time.deltaTime);
                }
                if (timeSinceGrounded <= 0 && !jumping)
                {
                    if (lastMove.y < -baseFallVelocity)
                    {
                        yVel = lastMove.y;
                    }
                    else if (yVel <= 0)
                    {
                        yVel = -baseFallVelocity;
                    }
                }
                if (jumpEnabled && timeSinceGrounded < coyoteTime && (jumpWhileSliding || !slide))
                {
                    if (jumpPressed > 0 && !jumping)
                    {
                        Jump();
                    }
                }

                yVel -= gravity * gravMult * Time.deltaTime;
                if (yVel < -gravityCap)
                {
                    yVel = -gravityCap;
                }

                timeSinceGrounded += Time.deltaTime;
                if (jumpPressed > 0)
                {
                    jumpPressed -= Time.deltaTime;
                }
            }

            ExecuteExtension("PreMove");

            currentMove += Vector3.up * yVel;
            moveDelta = transform.transform.position;
            controller.Move(currentMove * Time.deltaTime);
            moveDelta = transform.position - moveDelta;
            lastMove = moveDelta * 1 / Time.deltaTime;
            instantMomentumChange = false;
            ExecuteExtension("PostMove");
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            hitNormal = hit.normal;
            groundAngle = Vector3.Angle(hitNormal, Vector3.up);
            hitPoint = hit.point;
        }

        void setCurrentMoveVars()
        {
            if (grounded)
            {
                currentMoveSpeed = moveSpeed;
                currentStrafeMult = strafeMult;
                currentBackwardMult = backwardMult;
                if (crouching && crouchEnabled)
                {
                    currentMoveSpeed *= crouchMult;
                }
                else if (running && sprintEnabled)
                {
                    currentMoveSpeed *= sprintMult;
                }
            }
            else
            {
                currentMoveSpeed = airMoveSpeed;
                currentStrafeMult = airStrafeMult;
                currentBackwardMult = airBackwardMult;
                if (running && airSprintEnabled && !(crouching && crouchEnabled))
                {
                    currentMoveSpeed *= airSprintMult;
                }

                if (jumpHeld && jumping)
                {
                    if (variableHeight)
                    {
                        gravMult = jumpGravityMult;
                    }
                    if (yVel < 0)
                    {
                        jumping = false;
                        gravMult = 1.0f;
                    }
                }
                else if (jumping)
                {
                    jumping = false;
                    if (variableHeight)
                    {
                        gravMult = postJumpGravityMult;
                    }
                }
                else if (yVel > 0)
                {
                    if (variableHeight)
                    {
                        gravMult = postJumpGravityMult;
                    }
                }
                else
                {
                    gravMult = 1.0f;
                }
            }
        }

        Vector3 GetHorizontalMove()
        {
            Vector3 targetMove = Vector3.zero;
            if (moving)
            {
                targetMove += forward * currentMoveSpeed * yIn;
                if (yIn < 0)
                {
                    targetMove *= currentBackwardMult;
                }
                targetMove += side * currentMoveSpeed * xIn * currentStrafeMult;
            }
            return targetMove;
        }

        void Jump()
        {
            jumping = true;
            yVel = jumpSpeed;
            jumpPressed = 0;
        }

        void UpdateMouseLock()
        {
            if (Input.GetButtonDown(unlockMouseBtn))
            {
                mouseLocked = false;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                mouseLocked = true;
            }

            if (mouseLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

        }

        void MouseLook()
        {
            if (mouseLookEnabled)
            {
                float horizontalLook = transform.eulerAngles.y;
                float verticalLook = cam.localEulerAngles.x;

                horizontalLook += xMouse * sensitivity * Time.deltaTime;

                if (verticalLookEnabled)
                {
                    verticalLook -= yMouse * sensitivity * Time.deltaTime;
                    if (verticalLook > verticalLookLimit && verticalLook < 180)
                    {
                        verticalLook = verticalLookLimit;
                    }
                    else if (verticalLook > 180 && verticalLook < 360 - verticalLookLimit)
                    {
                        verticalLook = 360 - verticalLookLimit;
                    }

                    cam.localEulerAngles = new Vector3(verticalLook, 0, 0);
                }

                transform.localEulerAngles = new Vector3(0, horizontalLook, 0);
                print("lookenabled");
            }
            else
            {
                print("lookdisabled");
            }
        }

        void UpdateInput()
        {
            xIn = Input.GetAxisRaw(xInName);
            yIn = Input.GetAxisRaw(yInName);
            xMouse = Input.GetAxis(xMouseName);
            yMouse = Input.GetAxis(yMouseName);
            moving = Mathf.Abs(xIn) > 0.1 || Mathf.Abs(yIn) > 0.1;
            if (crouchToggleStyle)
            {
                if (Input.GetButtonDown(crouchBtn))
                {
                    crouching = !crouching;
                }
            }
            else
            {
                crouching = Input.GetButton(crouchBtn);
            }
            running = Input.GetButton(runBtn);
            jumpHeld = Input.GetButton(jumpBtn);
            if (Input.GetButtonDown(jumpBtn))
            {
                jumpPressed = coyoteTime;
            }
        }

        TFPData GetData()
        {
            return new TFPData(moving, jumpHeld, crouching, running, mouseLocked, mouseLookEnabled, jumpPressed, xIn, yIn, xMouse, yMouse,
                jumping, grounded, timeSinceGrounded, yVel, slide,
                gravMult, currentStrafeMult, currentBackwardMult, currentMoveSpeed, groundAngle,
                lastMove, currentMove, forward, side, moveDelta, hitNormal, hitPoint, slideMove,
                standingHeight, cameraOffset);
        }

        TFPInfo GetInfo()
        {
            return new TFPInfo(controller, cam,
                extensionsEnabled, slopeSlideEnabled, sprintEnabled, momentumEnabled, crouchEnabled, jumpEnabled,
                verticalLookEnabled, customInputNames, airControl, airSprintEnabled,
                jumpSpeed, variableHeight, coyoteTime, bunnyhopTolerance, jumpGravityMult, postJumpGravityMult,
                jumpWhileSliding, slopeJumpKickbackSpeed,
                gravity, baseGroundForce, maxGroundForce, gravityCap, baseFallVelocity,
                airResistance, airMoveSpeed, airStrafeMult, airBackwardMult, airSprintMult,
                moveSpeed, slopeSlideSpeed, acceleration, deceleration, sprintMult, strafeMult, backwardMult,
                sensitivity, verticalLookLimit,
                crouchToggleStyle, crouchColliderHeight, crouchMult, crouchTransitionSpeed, crouchHeadHitLayerMask,
                jumpBtn, crouchBtn, runBtn, unlockMouseBtn, xInName, yInName, xMouseName, yMouseName);
        }

        void SetData(TFPData newData)
        {
            moving = newData.moving;
            jumpHeld = newData.jumpHeld;
            crouching = newData.crouching;
            running = newData.running;
            mouseLocked = newData.mouseLocked;
            mouseLookEnabled = newData.mouseLookEnabled;
            jumpPressed = newData.jumpPressed;
            xIn = newData.xIn;
            yIn = newData.yIn;
            xMouse = newData.xMouse;
            yMouse = newData.yMouse;
            jumping = newData.jumping;
            grounded = newData.grounded;
            timeSinceGrounded = newData.timeSinceGrounded;
            yVel = newData.yVel;
            slide = newData.slide;
            gravMult = newData.gravMult;
            currentStrafeMult = newData.currentStrafeMult;
            currentBackwardMult = newData.currentBackwardMult;
            currentMoveSpeed = newData.currentMoveSpeed;
            groundAngle = newData.groundAngle;
            lastMove = newData.lastMove;
            currentMove = newData.currentMove;
            forward = newData.forward;
            side = newData.side;
            moveDelta = newData.moveDelta;
            hitNormal = newData.hitNormal;
            hitPoint = newData.hitPoint;
            slideMove = newData.slideMove;
            standingHeight = newData.standingHeight;
            cameraOffset = newData.cameraOffset;
        }

        void ExecuteExtension(string command)
        {
            if (!extensionsEnabled)
            {
                return;
            }
            TFPData data = GetData();
            foreach (TFPExtension extension in Extensions)
            {
                switch (command)
                {
                    case "Start":
                        extension.ExStart(ref data, controllerInfo);
                        break;
                    case "PreUpdate":
                        extension.ExPreUpdate(ref data, controllerInfo);
                        break;
                    case "PostUpdate":
                        extension.ExPostUpdate(ref data, controllerInfo);
                        break;
                    case "FixedUpdate":
                        extension.ExFixedUpdate(ref data, controllerInfo);
                        break;
                    case "PreMove":
                        extension.ExPreMove(ref data, controllerInfo);
                        break;
                    default:
                        break;
                }

            }
            SetData(data);
        }

    }
}
