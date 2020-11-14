///<summary>A Pixel Perfect Helper</summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixalHelper {
    ///<summary>
    /// Timer is used to set timers for registered actions
    /// If timer > 0, we call it locked
    /// If timer <= 0, we call it unlocked
    /// lock an action means the action is being performed right now.
    /// You can look up if an action is locked and decide
    /// what to do based on the action status
    /// You can decide to lock an action until you unlock it,
    /// the timer for this action will not count down.
    /// You can decide to lock an action for a certain amount of time
    /// When the time is reached, the lock will be unlocked.
    /// You can decide to unlock an action at any time.
    ///</summary>

    ///<example>
    /// Initiate the timer class
    ///   <c>var myTimer = new Timer();</c>
    /// To register a manual action, it is initially unlocked
    ///   <c>myTimer.RegisterAction("Jump", Timer.type.Manual);</c>
    /// To lock a manual action, no need to pass a lock time
    ///   <c>myTimer.LockAction("Jump")</c>
    /// To unlock a manual action
    ///   <c>myTimer.UnlockAction("Jump")</c>
    /// To check if an action is unlocked
    ///   <c>myTimer.IsUnlock("Jump")</c>
    ///</example>

    ///<example>
    /// Initiate the timer class
    ///   <c>var myTimer = new Timer();</c>
    /// To register an auto action, it is initially unlocked
    ///   <c>myTimer.RegisterAction("Shoot", Timer.type.Auto);</c>
    /// To lock an auto action, you need to provide a lock time
    ///   <c>myTimer.LockAction("Shoot", 1f)</c>
    /// To force-unlock an auto action regardless of remaining time.
    ///   <c>myTimer.UnlockAction("Shoot")</c>
    /// To check if an action is unlocked
    ///   <c>myTimer.IsUnlock("Shoot")</c>
    ///</example>
    public class Timer : MonoBehaviour {
        // public float timer;
        public enum types { Auto, Manual }

        public Dictionary<string, ActionData> timers = new Dictionary<string, ActionData> ();

        private void Update () {
            foreach (string action in new List<string> (timers.Keys)) {
                var actionData = timers[action];

                if (actionData.type == types.Auto) {
                    timers[action].lockTime -= Time.deltaTime;
                }
            }
        }

        public void RegisterAction (string action, types type, float lockTime = 0) {
            timers.Add (action, new ActionData (type, lockTime));
        }

        public void LockAction (string action, float lockTime = 1f) {
            if (timers.ContainsKey (action)) {
                timers[action].lockTime = lockTime;
                return;
            }

            throw new System.ArgumentException (
                $"Provided Action[{action}] Not Found. Please Register."
            );
        }

        public void UnlockAction (string action) {
            if (timers.ContainsKey (action)) {
                timers[action].lockTime = 0;
                return;
            }

            throw new System.ArgumentException (
                $"Provided Action[{action}] Not Found. Please Register."
            );
        }

        public bool IsUnlock (string action) {
            if (timers.ContainsKey (action)) {
                return timers[action].lockTime <= 0;
            }

            throw new System.ArgumentException (
                $"Provided Action[{action}] Not Found. Please Register."
            );
        }

        public bool IsLock (string action) {
            if (timers.ContainsKey (action)) {
                return timers[action].lockTime > 0;
            }

            throw new System.ArgumentException (
                $"Provided Actionp[{action}] Not Found. Please Register."
            );
        }
    }

    public class ActionData {
        public Timer.types type;
        public float lockTime;
        public ActionData (Timer.types myType, float myLockTime) {
            type = myType;
            lockTime = myLockTime;
        }
    }
}