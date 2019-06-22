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
    public bool jumpWhileSliding;
    [ConditionalHide(new string[]{"jumpEnabled","slopeSlideEnabled","jumpWhileSliding"},true,false)]
    public float slopeJumpKickbackSpeed;

    [Header("Gravity Settings")]
    public float gravity;
    public float baseGroundForce; 
	public float maxGroundForce; 
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
    public bool crouchToggleStyle;
    [ConditionalHide("crouchEnabled",true)]
    public float crouchColliderHeight;
    [ConditionalHide("crouchEnabled",true)]
    public float crouchMult;
    [ConditionalHide("crouchEnabled",true)]
    public float crouchTransitionSpeed;
    [ConditionalHide("crouchEnabled",true)]
    public LayerMask crouchHeadHitLayerMask;

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

    void Start(){
        controller = GetComponent<CharacterController>();

        //get the transform of a child with a camera component
        cam = transform.GetComponentInChildren<Camera>().transform;
        standingHeight = controller.height;
        cameraOffset = standingHeight - cam.position.y;
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

        if(crouchEnabled && crouching){
            if(controller.height > crouchColliderHeight){
                controller.height = Mathf.MoveTowards(controller.height,crouchColliderHeight,Time.deltaTime * crouchTransitionSpeed);
            }
        }else{
            if(controller.height < standingHeight){
                if(Physics.SphereCast(transform.position+(Vector3.up * 0.01f),controller.radius,Vector3.up,out headHit,standingHeight,crouchHeadHitLayerMask.value)){
                    crouching = true;
                }else{
                    controller.height = Mathf.MoveTowards(controller.height,standingHeight,Time.deltaTime * crouchTransitionSpeed);
                }
            }
        }
        
        setCurrentMoveVars();
        Vector3 targetMove = GetHorizontalMove();
        controller.center = new Vector3(0,controller.height/2.0f,0);
        cam.localPosition = new Vector3(cam.localPosition.x,controller.height - cameraOffset,cam.localPosition.z);
        
        if(grounded && groundAngle >= controller.slopeLimit && slopeSlideEnabled){
            if(Physics.Raycast(transform.position,Vector3.down,out ledgeCheck,0.1f+(controller.radius*2))){
                if(Vector3.Angle(ledgeCheck.normal,Vector3.up) >= controller.slopeLimit){
                    slide = true;
                }else{
                    slide = false;
                }
            }else if(Physics.Raycast(transform.position+(controller.radius * Vector3.up),new Vector3(hitPoint.x,transform.position.y,hitPoint.z) - transform.position,out ledgeCheck,0.1f+(controller.radius*2))){
                if(Vector3.Angle(ledgeCheck.normal,Vector3.up) >= controller.slopeLimit){
                    slide = true;
                }else{
                    slide = false;
                }
            }else{
                slide = false;
                groundAngle = 0;
            }
        }else{
            slide = false;
        }

        if(slide){
            slideMove = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
            Vector3.OrthoNormalize(ref hitNormal,ref slideMove);
            Vector3 slideMoveh = Vector3.Scale(slideMove,new Vector3(1,0,1)).normalized * slopeSlideSpeed;

            if(Vector3.Angle(targetMove,slideMoveh) > 100){
                targetMove = slideMoveh;
            }else{
                targetMove += slideMoveh;
            }
            if(jumpEnabled && jumpWhileSliding){
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
            yVel = -baseGroundForce + (-maxGroundForce * (groundAngle/90.0f));
            
            
            
            timeSinceGrounded = 0;
            jumping = false;
            if(jumpEnabled && (jumpWhileSliding || !slide)){
                if(jumpPressed > 0){
                    Jump();
                }
            }else if(slide){
                if(jumpPressed > 0){
                    jumpPressed -= Time.deltaTime;
                }
            }
        }else{
            if ((controller.collisionFlags & CollisionFlags.Above) != 0 && yVel > 0){
                yVel = 0;
            }
            if(moving){
                currentMove = Vector3.Lerp(lastMoveH,targetMove,airControl);
            }else{
                currentMove = Vector3.MoveTowards(lastMoveH,targetMove,airResistance*Time.deltaTime);
            }
            if(timeSinceGrounded <= 0 && !jumping){
                if(lastMove.y < -baseFallVelocity){
                    yVel = lastMove.y;
                }else if(yVel <= 0){
                    yVel = -baseFallVelocity;
                }
            }
            if(jumpEnabled && timeSinceGrounded < coyoteTime  && (jumpWhileSliding || !slide)){
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
        hitNormal = hit.normal;
        groundAngle = Vector3.Angle(hitNormal,Vector3.up);
        hitPoint = hit.point;
    }

    void setCurrentMoveVars(){
        if(grounded){
            currentMoveSpeed = moveSpeed;
            currentStrafeMult = strafeMult;
            currentBackwardMult = backwardMult;
            if(crouching && crouchEnabled){
                currentMoveSpeed *= crouchMult;
            }else if(running && sprintEnabled){
                currentMoveSpeed *= sprintMult;
            }
        }else{
            currentMoveSpeed = airMoveSpeed;
            currentStrafeMult = airStrafeMult;
            currentBackwardMult = airBackwardMult;
            if(running && airSprintEnabled && !(crouching && crouchEnabled)){
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
        print("JUMP! " + Time.frameCount + " - sliding :"+ slide + " - angle " + groundAngle + " - grounded :" + timeSinceGrounded);
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
        if(crouchToggleStyle){
            if(Input.GetButtonDown(crouchBtn)){
                crouching = !crouching;
            }
        }else{
            crouching = Input.GetButton(crouchBtn);
        }
        running = Input.GetButton(runBtn);
        jumpHeld = Input.GetButton(jumpBtn);
        if(Input.GetButtonDown(jumpBtn)){
            jumpPressed = coyoteTime;
        }
    }

}
