using UnityEngine;

namespace TheFirstPerson{
    public struct TFPInfo{
        public CharacterController controller;
        public Transform cam;
        public bool extensionsEnabled;
        public bool slopeSlideEnabled;
        public bool sprintEnabled;
        public bool momentumEnabled;
        public bool crouchEnabled;
        public bool jumpEnabled;
        public bool verticalLookEnabled;
        public bool customInputNames;
        public float airControl;
        public bool airSprintEnabled;
        public float jumpSpeed;
        public bool variableHeight;
        public float coyoteTime;
        public float bunnyhopTolerance;
        public float jumpGravityMult;
        public float postJumpGravityMult;
        public bool jumpWhileSliding;
        public float slopeJumpKickbackSpeed;
        public float gravity;
        public float baseGroundForce; 
        public float maxGroundForce; 
        public float gravityCap;
        public float baseFallVelocity;
        public float airResistance;
        public float airMoveSpeed;
        public float airStrafeMult;
        public float airBackwardMult;
        public float airSprintMult;
        public float moveSpeed;
        public float slopeSlideSpeed;
        public float acceleration;
        public float deceleration;
        public float sprintMult;
        public float strafeMult;
        public float backwardMult;
        public float sensitivity;
        public float verticalLookLimit;
        public bool crouchToggleStyle;
        public float crouchColliderHeight;
        public float crouchMult;
        public float crouchTransitionSpeed;
        public LayerMask crouchHeadHitLayerMask;
        public string jumpBtn;
        public string crouchBtn;
        public string runBtn;
        public string unlockMouseBtn;
        public string xInName;
        public string yInName;
        public string xMouseName;
        public string yMouseName;

        public TFPInfo(CharacterController controller, Transform cam,
            bool extensionsEnabled, bool slopeSlideEnabled, bool sprintEnabled, bool momentumEnabled, bool crouchEnabled, bool jumpEnabled,
            bool verticalLookEnabled, bool customInputNames, float airControl, bool airSprintEnabled,
            float jumpSpeed, bool variableHeight, float coyoteTime, float bunnyhopTolerance, float jumpGravityMult, float postJumpGravityMult,
            bool jumpWhileSliding, float slopeJumpKickbackSpeed,
            float gravity, float baseGroundForce, float maxGroundForce, float gravityCap, float baseFallVelocity,
            float airResistance, float airMoveSpeed, float airStrafeMult, float airBackwardMult, float airSprintMult,
            float moveSpeed, float slopeSlideSpeed, float acceleration, float deceleration, float sprintMult, float strafeMult, float backwardMult,
            float sensitivity, float verticalLookLimit,
            bool crouchToggleStyle, float crouchColliderHeight, float crouchMult, float crouchTransitionSpeed, LayerMask crouchHeadHitLayerMask,
            string jumpBtn, string crouchBtn, string runBtn, string unlockMouseBtn, string xInName, string yInName, string xMouseName, string yMouseName){
                
            this.controller = controller;
            this.cam = cam;
            this.extensionsEnabled = extensionsEnabled;
            this.slopeSlideEnabled = slopeSlideEnabled;
            this.sprintEnabled = sprintEnabled;
            this.momentumEnabled = momentumEnabled;
            this.crouchEnabled = crouchEnabled;
            this.jumpEnabled = jumpEnabled;
            this.verticalLookEnabled = verticalLookEnabled;
            this.customInputNames = customInputNames;
            this.airControl = airControl;
            this.airSprintEnabled = airSprintEnabled;
            this.jumpSpeed = jumpSpeed;
            this.variableHeight = variableHeight;
            this.coyoteTime = coyoteTime;
            this.bunnyhopTolerance = bunnyhopTolerance;
            this.jumpGravityMult = jumpGravityMult;
            this.postJumpGravityMult = postJumpGravityMult;
            this.jumpWhileSliding = jumpWhileSliding;
            this.slopeJumpKickbackSpeed = slopeJumpKickbackSpeed;
            this.gravity = gravity;
            this.baseGroundForce = baseGroundForce;
            this.maxGroundForce = maxGroundForce;
            this.gravityCap = gravityCap;
            this.baseFallVelocity = baseFallVelocity;
            this.airResistance = airResistance;
            this.airMoveSpeed = airMoveSpeed;
            this.airStrafeMult = airStrafeMult;
            this.airBackwardMult = airBackwardMult;
            this.airSprintMult = airSprintMult;
            this.moveSpeed = moveSpeed;
            this.slopeSlideSpeed = slopeSlideSpeed;
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            this.sprintMult = sprintMult;
            this.strafeMult = strafeMult;
            this.backwardMult = backwardMult;
            this.sensitivity = sensitivity;
            this.verticalLookLimit = verticalLookLimit;
            this.crouchToggleStyle = crouchToggleStyle;
            this.crouchColliderHeight = crouchColliderHeight;
            this.crouchMult = crouchMult;
            this.crouchTransitionSpeed = crouchTransitionSpeed;
            this.crouchHeadHitLayerMask = crouchHeadHitLayerMask;
            this.jumpBtn = jumpBtn;
            this.crouchBtn = crouchBtn;
            this.runBtn = runBtn;
            this.unlockMouseBtn = unlockMouseBtn;
            this.xInName = xInName;
            this.yInName = yInName;
            this.xMouseName = xMouseName;
            this.yMouseName = yMouseName;
        }
    }
}
