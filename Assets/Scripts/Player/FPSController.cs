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
    
    [Header("Jump Settings")]
    [ConditionalHide("jumpEnabled",true)]
    public float jumpSpeed;
    [ConditionalHide("jumpEnabled",true)]
    public bool variableHeight;
    [ConditionalHide("jumpEnabled",true)]
    public float coyoteTime;
    [ConditionalHide("jumpEnabled",true)]
    public float bunnyhopTolerance;
    [ConditionalHide("jumpEnabled",true)]
    public float jumpGravity;
    [ConditionalHide(new string[]{"jumpEnabled","variableHeight"},true,false)]
    public float postJumpGravity;

    [Header("Gravity Settings")]
    public float gravity;
    public float gravityAcceleration;
	public float groundforce; 

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
    [ConditionalHide("verticalLook",true)]
    public float verticalLookLimit;

    [Header("CrouchSettings")]
    [ConditionalHide("crouchEnabled",true)]
    public float crouchColliderHeight;
    [ConditionalHide("crouchEnabled",true)]
    public float crouchCameraHeight;
    [ConditionalHide("crouchEnabled",true)]
    public float crouchMult;

    //Input
    bool moving;
    bool jumpHeld;
    bool crouching;
    bool running;
    float jumpPressed;
    float xIn;
    float yIn;
    float xMouse;
    float yMouse;
    
    //Vertical Movement
    bool jumping;
    bool grounded;
    float yVel;
    

    //General movement
	Vector3 currentMove;
	Vector3 forward;
	Vector3 side;
	Vector3 facing;

    //sliding
    bool slide;
	Vector3 hitNormal;

    void Start(){
        controller = GetComponent<CharacterController>();

        //get the transform of a child with a camera component
        cam = transform.GetComponentInChildren<Camera>().transform;
    }

    void Update(){
        
    }

}
