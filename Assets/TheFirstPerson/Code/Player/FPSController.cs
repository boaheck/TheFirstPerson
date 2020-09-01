using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheFirstPerson
{
    public enum ExtFunc
    {
        Start,
        PreUpdate,
        PostUpdate,
        PreFixedUpdate,
        PostFixedUpdate,
        PreMove,
        PostMove,
        PostInput
    }

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
        [Tooltip("1 is full air control exactly how you control on the ground. 0 is none, you will have no control in the air.")]
        public float airControl = 0.5f;
        public bool airSprintEnabled = true;
        [Tooltip("This will put a limit of 1 on the magnitude of the horizontal movement input.")]
        public bool normaliseMoveInput = false;
        public bool moveInFixedUpdate = false;

        [Header("Jump Settings")]
        [ConditionalHide("jumpEnabled", true)]
        public bool definedByHeight = false;
        [ConditionalHide("jumpEnabled", true)]
        public bool variableHeight = true;
        [ConditionalHide("jumpEnabled", true)]
        [Tooltip("Time in seconds after you leave an edge that you can still jump.")]
        public float coyoteTime = 0.1f;
        [ConditionalHide("jumpEnabled", true)]
        [Tooltip("Time in seconds before you hit the ground where you can press jump to jump when you land.")]
        public float bunnyhopTolerance = 0.1f;
        [ConditionalHide(new string[] { "jumpEnabled", "definedByHeight" }, new bool[] { false, true }, true, false)]
        [Tooltip("Initial in units per second upward velocity of your jump.")]
        public float jumpSpeed = 9;
        [ConditionalHide(new string[] { "jumpEnabled", "definedByHeight" }, new bool[] { false, true }, true, false)]
        [Tooltip("Gravity that is applied on the upward section of your jump. This multiplies the base gravity variable.")]
        public float jumpGravityMult = 0.6f;
        [ConditionalHide(new string[] { "jumpEnabled", "definedByHeight" }, true, false)]
        [Tooltip("Maximum height in units that your jump will reach.")]
        public float maxJumpHeight = 4;
        [ConditionalHide(new string[] { "jumpEnabled", "definedByHeight" }, true, false)]
        [Tooltip("Maximum length of time in seconds your jump will last.")]
        public float maxJumpTime = 1;
        [ConditionalHide(new string[] { "jumpEnabled", "variableHeight", "definedByHeight" }, new bool[]{ false, false, true }, true, false)]
        [Tooltip("Gravity that is applied while you are traveling upwards after you have let go of the jump button. This multiplies the base gravity variable.")]
        public float postJumpGravityMult = 3;
        [ConditionalHide(new string[] { "jumpEnabled", "variableHeight", "definedByHeight" }, new bool[] { false, false, false }, true, false)]
        [Tooltip("Maximum height in units that your jump will reach if you let go of the jump button early.")]
        public float minJumpHeight = 1;
        [ConditionalHide(new string[] { "jumpEnabled", "slopeSlideEnabled" }, true, false)]
        public bool jumpWhileSliding = false;
        [ConditionalHide(new string[] { "jumpEnabled", "slopeSlideEnabled", "jumpWhileSliding" }, true, false)]
        [Tooltip("Force in units per second to be applied to the controller away from the surface of a slope too steep to traverse.")]
        public float slopeJumpKickbackSpeed = 10;

        [Header("Gravity Settings")]
        [Tooltip("Gravity variable this is the change in units per second that will be applied to your y velocity.")]
        public float gravity = 15;
        [Tooltip("Minimum force in units per second pushing you downwards while grounded. This applies on flat ground.")]
        public float baseGroundForce = 3;
        [Tooltip("Maximum force in units per second pushing you downwards while grounded. This applies on steep slopes.")]
        public float maxGroundForce = 30;
        [Tooltip("Maximum downwards velocity in units per second.")]
        public float gravityCap = 50;
        [Tooltip("Base downward velocity applied after walking off an edge in units per second.")]
        public float baseFallVelocity = 5;

        [Header("Air Control Settings")]
        [Tooltip("Speed in units per second that your horizontal velocity returns to 0 in air without any input.")]
        public float airResistance = 4;
        [Tooltip("Speed in units per second that you move forward in the air.")]
        public float airMoveSpeed = 6;
        [Tooltip("Speed that you strafe in the air relative to Air Move Speed.")]
        public float airStrafeMult = 0.8f;
        [Tooltip("Speed that you move backwards in the air relative to Air Move Speed.")]
        public float airBackwardMult = 0.6f;
        [ConditionalHide("airSprintEnabled", true)]
        [Tooltip("Speed that you sprint in the air relative to Air Move Speed.")]
        public float airSprintMult = 2;

        [Header("Speed Settings")]
        [Tooltip("Base forward speed in units per second.")]
        public float moveSpeed = 5;
        [ConditionalHide("slopeSlideEnabled", true)]
        [Tooltip("Horizontal speed while sliding in units per second.")]
        public float slopeSlideSpeed = 10;
        [ConditionalHide("momentumEnabled", true)]
        [Tooltip("Horizontal acceleration in units per second towards target horizontal speed if it is greater than current speed.")]
        public float acceleration = 50;
        [ConditionalHide("momentumEnabled", true)]
        [Tooltip("Horizontal deceleration in units per second towards target horizontal speed if it is less than current speed.")]
        public float deceleration = 40;
        [ConditionalHide("sprintEnabled", true)]
        public bool sprintToggleStyle = false;
        [ConditionalHide("sprintEnabled", true)]
        public bool sprintByDefault = false;
        [ConditionalHide("sprintEnabled", true)]
        [Tooltip("Speed that you sprint relative to Move Speed.")]
        public float sprintMult = 2;
        [Tooltip("Speed that you strafe relative to Move Speed.")]
        public float strafeMult = 0.8f;
        [Tooltip("Speed that you move backwards relative to Move Speed.")]
        public float backwardMult = 0.6f;

        [Header("Mouse Look Settings")]
        [Tooltip("Mouse sensitivity.")]
        public float sensitivity = 10;
        [Tooltip("In editor this may not work correctly but it will in build")]
        public bool mouseLockToggleEnabled = true;
        public bool startMouseLock = true;
        [ConditionalHide("verticalLookEnabled", true)]
        [Tooltip("Maximum upward or downward angle of the mouselook camera.")]
        public float verticalLookLimit = 80;
        [ConditionalHide("customCameraTransform", true)]
        public Transform cam;

        [Header("CrouchSettings")]
        [ConditionalHide("crouchEnabled", true)]
        public bool crouchToggleStyle = false;
        [ConditionalHide("crouchEnabled", true)]
        [Tooltip("Height of the crouch collider.")]
        public float crouchColliderHeight = 0.6f;
        [ConditionalHide("crouchEnabled", true)]
        [Tooltip("Horizontal speed when crouched relative to Move Speed.")]
        public float crouchMult = 0.5f;
        [ConditionalHide("crouchEnabled", true)]
        [Tooltip("Speed of transition to and from crouch in units per second.")]
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
        public string unlockMouseBtnCustom = "Cancel";
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
        bool mouseLocked;
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
        float originalMaxJH;
        float originalMinJH;
        float originalJT;
        float minJumpTime;

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

            if(sprintByDefault)
            {
                running = true;
            }

            if (definedByHeight)
            {
                RecalculateJumpValues();
            }

            mouseLocked = startMouseLock;

            controllerInfo = GetInfo();
            ExecuteExtension(ExtFunc.Start);
        }

        void Update()
        {
            if(definedByHeight && (originalJT != maxJumpTime || originalMaxJH != maxJumpHeight || originalMinJH != minJumpHeight))
            {
                RecalculateJumpValues();
            }


            ExecuteExtension(ExtFunc.PreUpdate);
            UpdateInput();
            UpdateMouseLock();
            ExecuteExtension(ExtFunc.PostInput);
            if (mouseLocked)
            {
                MouseLook();
            }
            if (!moveInFixedUpdate)
            {
                UpdateMovement(Time.deltaTime);
            }
            ExecuteExtension(ExtFunc.PostUpdate);
        }

        void FixedUpdate()
        {
            ExecuteExtension(ExtFunc.PreFixedUpdate);
            if (moveInFixedUpdate)
            {
                UpdateMovement(Time.deltaTime);
            }
            ExecuteExtension(ExtFunc.PostFixedUpdate);
        }

        void UpdateMovement(float dt)
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
                    controller.height = Mathf.MoveTowards(controller.height, crouchColliderHeight, dt * crouchTransitionSpeed);
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
                        controller.height = Mathf.MoveTowards(controller.height, standingHeight, dt * crouchTransitionSpeed);
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
                        currentMove = Vector3.MoveTowards(lastMoveH, targetMove, dt * deceleration);
                    }
                    else
                    {
                        currentMove = Vector3.MoveTowards(lastMoveH, targetMove, dt * acceleration);
                    }
                }
                else
                {
                    currentMove = targetMove;
                }
                var targetYVel = -baseGroundForce + (-maxGroundForce * (groundAngle / 90.0f));
                if (lastMove.y < 0)
                {
                    yVel = Mathf.Lerp(lastMove.y, targetYVel, gravity * dt);
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
                        jumpPressed -= dt;
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
                    currentMove = Vector3.MoveTowards(lastMoveH, targetMove, airResistance * dt);
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

                yVel -= gravity * gravMult * dt;
                if (yVel < -gravityCap)
                {
                    yVel = -gravityCap;
                }

                timeSinceGrounded += dt;
                if (jumpPressed > 0)
                {
                    jumpPressed -= dt;
                }
            }

            ExecuteExtension(ExtFunc.PreMove);

            currentMove += Vector3.up * yVel;
            moveDelta = transform.transform.position;
            controller.Move(currentMove * dt);
            moveDelta = transform.position - moveDelta;
            lastMove = moveDelta * 1 / dt;
            instantMomentumChange = false;
            ExecuteExtension(ExtFunc.PostMove);
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
                    gravMult = jumpGravityMult;
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
            if (mouseLockToggleEnabled)
            {
                if (Input.GetButtonDown(unlockMouseBtn))
                {
                    mouseLocked = false;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    mouseLocked = true;
                }
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
                float horizontalLook = transform.localEulerAngles.y;
                float verticalLook = cam.localEulerAngles.x;

                horizontalLook += xMouse * sensitivity;

                if (verticalLookEnabled)
                {
                    verticalLook -= yMouse * sensitivity;
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
            }
        }

        void UpdateInput()
        {
            xIn = Input.GetAxisRaw(xInName);
            yIn = Input.GetAxisRaw(yInName);
            if (normaliseMoveInput)
            {
                Vector2 normalised = Vector2.ClampMagnitude(new Vector2(xIn, yIn), 1.0f);
                xIn = normalised.x;
                yIn = normalised.y;
            }
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
            if (sprintToggleStyle)
            {
                if (Input.GetButtonDown(runBtn))
                {
                    running = !running;
                }
            }
            else if (sprintByDefault)
            {
                running = !Input.GetButton(runBtn);
            }
            else
            {
                running = Input.GetButton(runBtn);
            }
            jumpHeld = Input.GetButton(jumpBtn);
            if (Input.GetButtonDown(jumpBtn))
            {
                jumpPressed = coyoteTime;
                if (moveInFixedUpdate)
                {
                    jumpPressed += Time.fixedDeltaTime;
                }
            }
        }

        void RecalculateJumpValues()
        {
            jumpGravityMult = ((2 * maxJumpHeight) / Mathf.Pow(maxJumpTime, 2)) / gravity;
            jumpSpeed = (2 * maxJumpHeight) / maxJumpTime;
            minJumpTime = (2 * minJumpHeight) / jumpSpeed;
            postJumpGravityMult = ((2 * minJumpHeight) / Mathf.Pow(minJumpTime, 2)) / gravity;
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
            return new TFPInfo(controller, cam, extensionsEnabled, slopeSlideEnabled,
                sprintEnabled, momentumEnabled, crouchEnabled, jumpEnabled, verticalLookEnabled,
                customInputNames, airControl, airSprintEnabled, jumpSpeed, variableHeight, coyoteTime,
                bunnyhopTolerance, jumpGravityMult, postJumpGravityMult, jumpWhileSliding, slopeJumpKickbackSpeed,
                gravity, baseGroundForce, maxGroundForce, gravityCap, baseFallVelocity, airResistance,
                airMoveSpeed, airStrafeMult, airBackwardMult, airSprintMult, moveSpeed, slopeSlideSpeed,
                acceleration, deceleration, sprintMult, strafeMult, backwardMult, sensitivity, verticalLookLimit,
                crouchToggleStyle, crouchColliderHeight, crouchMult, crouchTransitionSpeed, crouchHeadHitLayerMask,
                jumpBtn, crouchBtn, runBtn, unlockMouseBtn, xInName, yInName, xMouseName, yMouseName,
                mouseLookEnabled, customCameraTransform, normaliseMoveInput, moveInFixedUpdate, definedByHeight,
                maxJumpHeight, maxJumpTime, minJumpHeight, sprintToggleStyle, sprintByDefault, mouseLockToggleEnabled,
                startMouseLock);
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

        void ExecuteExtension(ExtFunc command)
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
                    case ExtFunc.Start:
                        extension.ExStart(ref data, controllerInfo);
                        break;
                    case ExtFunc.PreUpdate:
                        extension.ExPreUpdate(ref data, controllerInfo);
                        break;
                    case ExtFunc.PostUpdate:
                        extension.ExPostUpdate(ref data, controllerInfo);
                        break;
                    case ExtFunc.PreFixedUpdate:
                        extension.ExPreFixedUpdate(ref data, controllerInfo);
                        break;
                    case ExtFunc.PostFixedUpdate:
                        extension.ExPostFixedUpdate(ref data, controllerInfo);
                        break;
                    case ExtFunc.PreMove:
                        extension.ExPreMove(ref data, controllerInfo);
                        break;
                    case ExtFunc.PostMove:
                        extension.ExPostMove(ref data, controllerInfo);
                        break;
                    case ExtFunc.PostInput:
                        extension.ExPostInput(ref data, controllerInfo);
                        break;
                    default:
                        break;
                }

            }
            SetData(data);
        }

    }
}
