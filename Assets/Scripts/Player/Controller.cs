using System.Collections;
using System.Collections.Generic;
using PixalHelper;
using UnityEngine;

namespace Player {
    [RequireComponent (typeof (Actor))]
    [RequireComponent (typeof (Timer))]
    [RequireComponent (typeof (FSMManager))]
    [RequireComponent (typeof (Animator))]

    public class Controller : MonoBehaviour {
        private Vector2 velocity;
        public LayerMask solidLayer;
        public string idleAnimation;
        public string runAnimation;
        public string jumpAnimation;
        public string fallAnimation;
        public string dashAnimation;
        public string direction;
        public string bodyState;
        [HideInInspector] public Actor myActor;
        [HideInInspector] public Timer myTimer;
        [HideInInspector] public FSMManager myFsmManager;
        [HideInInspector] public Animator myAnimator;
        [HideInInspector] public Damager myDamager;
        [HideInInspector] public Shooter myShooter;
        [Range (0, 1000)] public float maxFall;
        [Range (0, 1000)] public float maxRun;
        [Range (0, 1000)] public float jumpSpeed;
        [Range (0, 1000)] public float dashSpeed;
        [Range (0, 3000)] public float runAccelerate;
        [Range (0, 1000)] public float gravity;
        [Range (0, 1)] public float dashDurationTime;
        [Range (0, 2)] public float dashLockTime;
        [Range (0, 1)] public float attackDurationTime;
        [Range (0, 2)] public float attackLockTime;
        [Range (0, 2)] public float shootLockTime;
        private void Reset () {
            maxFall = 300f;
            maxRun = 300f;
            jumpSpeed = 400f;
            dashSpeed = 600f;
            runAccelerate = 2000f;
            gravity = 1000f;
            dashDurationTime = 0.1f;
            dashLockTime = 0.5f;
            attackDurationTime = 0.3f;
            attackLockTime = 0.4f;
        }

        private void Awake () {
            myActor = GetComponent<Actor> ();
            myTimer = GetComponent<Timer> ();
            myFsmManager = GetComponent<FSMManager> ();
            myAnimator = GetComponent<Animator> ();
            myDamager = GetComponentInChildren<Damager> ();
            myShooter = GetComponentInChildren<Shooter> ();

            myActor.solidLayer = solidLayer;
            myActor.maxFall = maxFall;
            myActor.gravity = gravity;
            myFsmManager.RegisterState ("Direction", "Left");
            myFsmManager.RegisterState ("Direction", "Right", true, true);

            myFsmManager.RegisterState ("FullBody", "Jump");
            myFsmManager.RegisterState ("FullBody", "Fall");
            myFsmManager.RegisterState ("FullBody", "Dash");
            myFsmManager.RegisterState ("FullBody", "Idle", true, true);
            myFsmManager.RegisterState ("FullBody", "Run");
            myFsmManager.RegisterState ("Action", "Attack", false);
            myFsmManager.RegisterState ("Action", "Shoot");

            myTimer.RegisterAction ("DashDuration", Timer.types.Auto);
            myTimer.RegisterAction ("DashLock", Timer.types.Auto);
            myTimer.RegisterAction ("AttackDuration", Timer.types.Auto);
            myTimer.RegisterAction ("AttackLock", Timer.types.Auto);
            myTimer.RegisterAction ("ShootLock", Timer.types.Auto);
            // We can have several action cold down. Can cold down AttackLock/DashLock... -> parallel mode
            // we can only have one action is performed at the moment. Can only Jump/Fall/Dash -> sequential mode. Transfer to the other state when the action is performed and allowed.
            // we can only either have left or right -> sequential mode
            // Maybe Manual mode becomes useless...
        }

        private void Update () {
            /// <summary>Status: Move Horizontally when input X is detected</summary>
            float inputX = Input.GetAxisRaw ("Horizontal");
            float inputY = Input.GetAxisRaw ("Vertical");
            bodyState = myFsmManager.GetState ("FullBody");
            direction = myFsmManager.GetState ("Direction");

            if (myActor.IsGrounded ()) {
                if (myActor.velocity.x < 0) {
                    myFsmManager.TurnOn ("Direction", "Left");

                    myFsmManager.TurnOn ("FullBody", "Run");
                } else if (myActor.velocity.x > 0) {
                    myFsmManager.TurnOn ("Direction", "Right");

                    myFsmManager.TurnOn ("FullBody", "Run");
                }

                if (myActor.velocity.x == 0 &&
                    myActor.velocity.y == 0
                )
                    myFsmManager.TurnOn ("FullBody", "Idle");
            }

            if (myFsmManager.IsOn ("Direction", "Left")) transform.localScale = new Vector3 (-1f, transform.localScale.y, transform.localScale.z);
            else transform.localScale = new Vector3 (1f, transform.localScale.y, transform.localScale.z);

            // If the character is Dashing, keep the dashing speed until the dash action is unlocked.
            if (myTimer.IsUnlock ("DashDuration")) {
                // back to the normal speed when dashing is unlocked.
                myActor.velocity.x = Math.Approach (myActor.velocity.x, maxRun * inputX, runAccelerate * Time.deltaTime);
            }
            /// </summary>End Moving Horizontally when input X is detected</summary>

            /// <summary>Status: Move Vertical when input X is detected</summary>
            if (myTimer.IsUnlock ("DashDuration")) {
                myActor.gravity = gravity;
            } else {
                myActor.velocity.y = 0;
                myActor.gravity = 0;
            }
            /// </summary>End Vertical Horizontally when input X is detected</summary>

            /// <summary>Status: Handle Falling</summary>
            if (myActor.velocity.y < 0) {
                myFsmManager.TurnOn ("FullBody", "Fall");
            }
            /// </summary>End Falling</summary>

            /// <summary>Event: Handle Jumping</summary>
            if (Input.GetButtonDown ("Jump") && myActor.IsGrounded ()) {
                myActor.velocity.y = jumpSpeed;
                myFsmManager.TurnOn ("FullBody", "Jump");
            }
            // if (myActor.velocity.y < 0) myTimer.UnlockAction ("Jump");
            /// </summary>End Jumping</summary>

            /// <summary>Event: Handle Dashing</summary>
            if (Input.GetKeyDown (KeyCode.Z) && myTimer.IsUnlock ("DashLock")) {
                myTimer.LockAction ("DashDuration", dashDurationTime);
                myTimer.LockAction ("DashLock", dashLockTime);
                myFsmManager.TurnOn ("FullBody", "Dash");

                if (myFsmManager.IsOn ("Direction", "Left")) myActor.velocity.x = -dashSpeed;
                else myActor.velocity.x = dashSpeed;
            }
            /// </summary>End Dashing</summary>

            /// <summary>Event: Handle Attack</summary>
            if (Input.GetKeyDown (KeyCode.X) && isEnabledDamager () &&
                myTimer.IsUnlock ("AttackLock")) {

                myFsmManager.TurnOn ("Action", "Attack");
                myTimer.LockAction ("AttackDuration", attackDurationTime);
                myTimer.LockAction ("AttackLock", attackLockTime);

                Debug.Log (myDamager.IfOverlapDamageBox ());
            }

            if (myTimer.IsUnlock ("AttackLock") &&
                myFsmManager.IsOn ("Action", "Attack")) {
                myFsmManager.TurnOff ("Action");
            }
            /// </summary>End Attack</summary>

            /// <summary>Event: Handle Shoot</summary>
            if (Input.GetKey (KeyCode.C) && isEnabledShooter () &&
                myTimer.IsUnlock ("ShootLock")) {

                myFsmManager.TurnOn ("Action", "Shoot");
                myTimer.LockAction ("ShootLock", shootLockTime);

                if (myFsmManager.IsOn ("Direction", "Left")) {
                    myShooter.Fire (Vector3.left);
                } else {
                    myShooter.Fire (Vector3.right);
                }

            }

            if (Input.GetKeyUp (KeyCode.C) &&
                myFsmManager.IsOn ("Action", "Shoot")) {
                myFsmManager.TurnOff ("Action");
            }
            /// </summary>End Shoot</summary>

            /// <summary>Animator</summary>
            if (myFsmManager.GetState ("Action") == null) {
                // If no action is performing, only show pure fullbody animation
                if (myFsmManager.IsOn ("FullBody", "Idle"))
                    SetAnimation (idleAnimation);

                if (myFsmManager.IsOn ("FullBody", "Run"))
                    SetAnimation (runAnimation);

                if (myFsmManager.IsOn ("FullBody", "Jump"))
                    SetAnimation (jumpAnimation);

                if (myFsmManager.IsOn ("FullBody", "Fall"))
                    SetAnimation (fallAnimation);

                if (myFsmManager.IsOn ("FullBody", "Dash"))
                    SetAnimation (dashAnimation);
            } else {
                switch (myFsmManager.GetState ("Action")) {
                    case "Attack":
                        if (myFsmManager.IsOn ("FullBody", "Idle"))
                            SetAnimation ("Attack");

                        if (myFsmManager.IsOn ("FullBody", "Run"))
                            SetAnimation ("Attack");

                        if (myFsmManager.IsOn ("FullBody", "Jump"))
                            SetAnimation ("Attack");

                        if (myFsmManager.IsOn ("FullBody", "Fall"))
                            SetAnimation ("Attack");

                        if (myFsmManager.IsOn ("FullBody", "Dash"))
                            SetAnimation ("Attack");
                        break;
                    case "Shoot":
                        Debug.Log ("shooting");
                        break;
                }
            }
        }
        /// </summary>Animator</summary>

        private void SetAnimation (string name) {
            if (!myAnimator.GetCurrentAnimatorStateInfo (0).IsName (name))
                myAnimator.Play (name);
        }

        private void dash (float distance, float time) {
            float speed = distance / time;
            // set the velocity to the speed
            // and let it be for time s.
            // for this duration, the status is dash status. also set the mytimer.
            // can also easily control the distance.
        }

        private bool isEnabledDamager () {
            return myDamager != null;
        }

        private bool isEnabledShooter () {
            return myShooter != null;
        }
    }
}

// TODO: USE ANIMATION TO TRIGGER THE FUNCTIONS


// TODO: Controller cannot assume the name of attack animation, which is state name.