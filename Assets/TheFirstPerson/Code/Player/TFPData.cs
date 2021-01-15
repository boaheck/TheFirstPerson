using UnityEngine;

namespace TheFirstPerson
{
    public struct TFPData
    {
        public bool movementEnabled;
        public bool moving;
        public bool jumpHeld;
        public bool crouching;
        public bool running;
        public bool mouseLocked;
        public bool mouseLookEnabled;
        public float jumpPressed;
        public float xIn;
        public float yIn;
        public float xMouse;
        public float yMouse;
        public bool jumping;
        public bool grounded;
        public float timeSinceGrounded;
        public float yVel;
        public bool slide;
        public float gravMult;
        public float currentStrafeMult;
        public float currentBackwardMult;
        public float currentMoveSpeed;
        public float groundAngle;
        public Vector3 lastMove;
        public Vector3 currentMove;
        public Vector3 forward;
        public Vector3 side;
        public Vector3 moveDelta;
        public Vector3 hitNormal;
        public Vector3 hitPoint;
        public Vector3 slideMove;
        public float standingHeight;
        public float cameraOffset;

        public TFPData(bool movementEnabled,bool moving, bool jumpHeld, bool crouching, bool running, bool mouseLocked, bool mouseLookEnabled, float jumpPressed, float xIn, float yIn, float xMouse, float yMouse,
            bool jumping, bool grounded, float timeSinceGrounded, float yVel, bool slide,
            float gravMult, float currentStrafeMult, float currentBackwardMult, float currentMoveSpeed, float groundAngle,
            Vector3 lastMove, Vector3 currentMove, Vector3 forward, Vector3 side, Vector3 moveDelta, Vector3 hitNormal, Vector3 hitPoint, Vector3 slideMove,
            float standingHeight, float cameraOffset)
        {
            this.movementEnabled = movementEnabled;
            this.moving = moving;
            this.jumpHeld = jumpHeld;
            this.crouching = crouching;
            this.running = running;
            this.mouseLocked = mouseLocked;
            this.mouseLookEnabled = mouseLookEnabled;
            this.jumpPressed = jumpPressed;
            this.xIn = xIn;
            this.yIn = yIn;
            this.xMouse = xMouse;
            this.yMouse = yMouse;
            this.jumping = jumping;
            this.grounded = grounded;
            this.timeSinceGrounded = timeSinceGrounded;
            this.yVel = yVel;
            this.slide = slide;
            this.gravMult = gravMult;
            this.currentStrafeMult = currentStrafeMult;
            this.currentBackwardMult = currentBackwardMult;
            this.currentMoveSpeed = currentMoveSpeed;
            this.groundAngle = groundAngle;
            this.lastMove = lastMove;
            this.currentMove = currentMove;
            this.forward = forward;
            this.side = side;
            this.moveDelta = moveDelta;
            this.hitNormal = hitNormal;
            this.hitPoint = hitPoint;
            this.slideMove = slideMove;
            this.standingHeight = standingHeight;
            this.cameraOffset = cameraOffset;
        }
    }
}
