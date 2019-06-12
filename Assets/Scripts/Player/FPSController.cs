using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour{
    
    CharacterController controller;
    Transform cam;
    
    [Header("Options")]
    public bool slopeSlideEnabled;
    public bool sprintEnabled;
    public bool momentumEnabled;
    public bool crouchEnabled;
    public bool jumpEnabled;
    public bool verticalLookEnabled;
    public bool customInputNames;
    [Range(0f,1f)]
    public float airControl = 0.5f;
    public bool airSprintEnabled;
    
    [Header("Jump Settings")]
    [ConditionalHide("jumpEnabled",true)]
    public float jumpSpeed;
    [ConditionalHide("jumpEnabled",true)]
    public bool variableHeight;
    [ConditionalHide("jumpEnabled",true)]
    public float coyoteTime;
    [ConditionalHide("jumpEnabled",true)]
    public float bunnyhopTolerance;
    [ConditionalHide(new string[]{"jumpEnabled","variableHeight"},true,false)]
    public float jumpGravityMult;
    [ConditionalHide(new string[]{"jumpEnabled","variableHeight"},true,false)]
    public float postJumpGravityMult;
    [ConditionalHide(new string[]{"jumpEnabled","slopeSlideEnabled"},true,false)]
    public float slopeJumpKickbackSpeed;

    [Header("Gravity Settings")]
    public float gravity;
	public float groundforce; 
    public float gravityCap;
    public float baseFallVelocity;

    [Header("Air Control Settings")]
    public float airResistance;
    public float airMoveSpeed;
    public float airStrafeMult;
    public float airBackwardMult;
    [ConditionalHide("airSprintEnabled",true)]
    public float airSprintMult;

    [Header("Speed Settings")]
    public float moveSpeed;
    [ConditionalHide("slopeSlideEnabled",true)]
    public float slopeSlideSpeed;
    [ConditionalHide("momentumEnabled",true)]
    public float acceleration;
    [ConditionalHide("momentumEnabled",true)]
    public float deceleration;
    [ConditionalHide("sprintEnabled",true)]
    public float sprintMult;
    public float strafeMult;
    public float backwardMult;

    [Header("Mouse Look Settings")]
    public float sensitivity;
    [ConditionalHide("verticalLookEnabled",true)]
    public float verticalLookLimit;

    [Header("CrouchSettings")]
    [ConditionalHide("crouchEnabled",true)]
    public float crouchColliderHeight;
    [ConditionalHide("crouchEnabled",true)]
    public float crouchCameraHeight;
    [ConditionalHide("crouchEnabled",true)]
    public float crouchMult;

    [Header("Input Names")]
    [ConditionalHide("customInputNames",true)]
    public string jumpBtnCustom = "Jump";
    [ConditionalHide("customInputNames",true)]
    public string crouchBtnCustom = "Fire1";
    [ConditionalHide("customInputNames",true)]
    public string runBtnCustom = "Fire3";
    [ConditionalHide("customInputNames",true)]
    string unlockMouseBtnCustom = "Cancel";
    [ConditionalHide("customInputNames",true)]
    public string xInNameCustom = "Horizontal";
    [ConditionalHide("customInputNames",true)]
    public string yInNameCustom = "Vertical";
    [ConditionalHide("customInputNames",true)]
    public string xMouseNameCustom = "Mouse X";
    [ConditionalHide("customInputNames",true)]
    public string yMouseNameCustom = "Mouse Y";

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
    bool instantMomentumChange = false;

    //sliding
    bool slide;
	Vector3 hitNormal;
    Vector3 slideMove;

    //crouching
    float standingHeight;
    float standingCameraHeight;

    //Input Name Defaults (assuming default unity axes are set up)
    string jumpBtn = "Jump";
    string crouchBtn = "Fire1";
    string runBtn = "Fire3";
    string unlockMouseBtn = "Cancel";
    string xInName = "Horizontal";
    string yInName = "Vertical";
    string xMouseName = "Mouse X";
    string yMouseName = "Mouse Y";

    void Start(){
        controller = GetComponent<CharacterController>();

        //get the transform of a child with a camera component
        cam = transform.GetComponentInChildren<Camera>().transform;

        if(customInputNames){
            jumpBtn = jumpBtnCustom;
            crouchBtn = crouchBtnCustom;
            runBtn = runBtnCustom;
            unlockMouseBtn = unlockMouseBtnCustom;
            xInName = xInNameCustom;
            yInName = yInNameCustom;
            xMouseName = xMouseNameCustom;
            yMouseName = yMouseNameCustom;
        }
    }

    void Update(){
        UpdateInput();
        UpdateMouseLock();
        if(mouseLocked){
            MouseLook();
        }
        UpdateMovement();
    }

    void UpdateMovement(){
        forward = transform.forward;
        side = transform.right;
        currentMove = Vector3.zero;
        grounded = controller.isGrounded;
        slideMove = Vector3.zero;
        Vector3 lastMoveH = Vector3.Scale(lastMove, new Vector3(1,0,1));
        setCurrentMoveVars();
        Vector3 targetMove = GetHorizontalMove();

        if(slide && slopeSlideEnabled){
            slideMove = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
            Vector3.OrthoNormalize(ref hitNormal,ref slideMove);
            slideMove *= slopeSlideSpeed;
            Vector3 slideMoveh = Vector3.Scale(slideMove,new Vector3(1,0,1));
            if(Vector3.Angle(targetMove,slideMove) > 100){
                targetMove = slideMoveh;
            }else{
                targetMove += slideMoveh;
            }
            if(jumpEnabled){
                if(jumpPressed > 0){
                    targetMove = slideMoveh.normalized * slopeJumpKickbackSpeed;
                    instantMomentumChange = true;
                }
            }
        }

        if(grounded){
            if(momentumEnabled && !instantMomentumChange){
                if(moving || targetMove.magnitude < lastMoveH.magnitude){
                    currentMove = Vector3.MoveTowards(lastMoveH,targetMove,Time.deltaTime * deceleration);
                }else{
                    currentMove = Vector3.MoveTowards(lastMoveH,targetMove,Time.deltaTime * acceleration);
                }
            }else{
                currentMove = targetMove;
            }
            yVel = Mathf.Min(-groundforce,slideMove.y);
            timeSinceGrounded = 0;
            jumping = false;
            if(jumpEnabled){
                if(jumpPressed > 0){
                    Jump();
                }
            }
        }else{
            slide = false;
            if ((controller.collisionFlags & CollisionFlags.Above) != 0 && yVel > 0){
                yVel = 0;
            }
            if(moving){
                currentMove = Vector3.Lerp(lastMoveH,targetMove,airControl);
            }else{
                currentMove = Vector3.MoveTowards(lastMoveH,targetMove,airResistance*Time.deltaTime);
            }
            if(timeSinceGrounded <= 0 && !jumping){
                yVel = -baseFallVelocity;
            }
            if(jumpEnabled && timeSinceGrounded < coyoteTime){
                if(jumpPressed > 0 && !jumping){
                    Jump();
                }
            }

            yVel -= gravity * gravMult * Time.deltaTime;
            if(yVel < -gravityCap){
                yVel = -gravityCap;
            }

            timeSinceGrounded += Time.deltaTime;
            if(jumpPressed > 0){
                jumpPressed -= Time.deltaTime;
            }
        }
        
        currentMove += Vector3.up * yVel;
        moveDelta = transform.transform.position;
        controller.Move(currentMove * Time.deltaTime);
        moveDelta = transform.position - moveDelta;
        lastMove = moveDelta * 1/Time.deltaTime;
        instantMomentumChange = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit){
        if ((controller.collisionFlags & CollisionFlags.Below) != 0 && Vector3.Angle(hit.normal,Vector3.up) > controller.slopeLimit){
            print("sliding!");
            slide = true;
            hitNormal = hit.normal;
        }else{
            slide = false;
        }
    }

    void setCurrentMoveVars(){
        if(grounded){
            currentMoveSpeed = moveSpeed;
            currentStrafeMult = strafeMult;
            currentBackwardMult = backwardMult;
            if(running && sprintEnabled){
                currentMoveSpeed *= sprintMult;
            }
        }else{
            currentMoveSpeed = airMoveSpeed;
            currentStrafeMult = airStrafeMult;
            currentBackwardMult = airBackwardMult;
            if(running && airSprintEnabled){
                currentMoveSpeed *= airSprintMult;
            }

            if(jumpHeld){
                if(jumping){
                    if(variableHeight){
                        gravMult = jumpGravityMult;
                    }
                    if(yVel < 0){
                        jumping = false;
                        gravMult = 1.0f;
                    }
                }
            }else if(jumping){
                jumping = false;
                if(variableHeight){
                    gravMult = postJumpGravityMult;
                }
            }else if(yVel > 0){
                if(variableHeight){
                    gravMult = postJumpGravityMult;
                }
            }else{
                gravMult = 1.0f;
            }

        }
    }

    Vector3 GetHorizontalMove(){
        Vector3 targetMove = Vector3.zero;
        if(moving){
            targetMove += forward * currentMoveSpeed * yIn;
            if(yIn < 0){
                targetMove *= currentBackwardMult;
            }
            targetMove += side * currentMoveSpeed * xIn * currentStrafeMult;
        }
        return targetMove;
    }

    void Jump(){
        jumping = true;
        yVel = jumpSpeed;
        jumpPressed = 0;
    }

    void UpdateMouseLock(){
        if(Input.GetButtonDown(unlockMouseBtn)){
            mouseLocked = false;
        }else if(Input.GetMouseButtonDown(0)){
            mouseLocked = true;
        }

        if(mouseLocked){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }else{
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    void MouseLook(){
        float horizontalLook = transform.eulerAngles.y;
        float verticalLook = cam.localEulerAngles.x;

        horizontalLook += xMouse * sensitivity * Time.deltaTime;

        if(verticalLookEnabled){
            verticalLook -= yMouse * sensitivity * Time.deltaTime;
            if(verticalLook > verticalLookLimit && verticalLook < 180){
                verticalLook = verticalLookLimit;
            }else if(verticalLook > 180 && verticalLook < 360 - verticalLookLimit){
                verticalLook = 360 - verticalLookLimit;
            }

            cam.localEulerAngles = new Vector3(verticalLook,0,0);
        }

        transform.localEulerAngles = new Vector3(0,horizontalLook,0);

    }

    void UpdateInput(){
        xIn = Input.GetAxisRaw(xInName);
        yIn = Input.GetAxisRaw(yInName);
        xMouse = Input.GetAxis(xMouseName);
        yMouse = Input.GetAxis(yMouseName);
        moving = Mathf.Abs(xIn) > 0.1 || Mathf.Abs(yIn) > 0.1;
        crouching = Input.GetButton(crouchBtn);
        running = Input.GetButton(runBtn);
        jumpHeld = Input.GetButton(jumpBtn);
        if(Input.GetButtonDown(jumpBtn)){
            jumpPressed = coyoteTime;
        }
    }

}
