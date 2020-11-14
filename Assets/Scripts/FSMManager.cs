using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixalHelper {
    ///<problem>
    /// for now, 
    /// never modify the same layer from different components or asyncly in one component
    /// it may active two states at the same time
    /// to fix this bug, go #[1]#
    /// One time can only register one with default active
    ///</problem>
    public class FSMManager : MonoBehaviour {
        public Dictionary<string, FSM> fsmManager = new Dictionary<string, FSM> ();

        // Each new state needs register.
        // if preemptible parameter is not provided, default value is true
        // if actived parameter is not provided, default value is false;
        // layer and state is required parameters.
        public void RegisterState (string layer, string state, bool preemptible = true, bool actived = false) {
            if (!fsmManager.ContainsKey (layer)) {
                fsmManager.Add (layer, new FSM ());
            }

            fsmManager[layer].AddState (state, actived, preemptible);
        }

        // Turn on a state
        // If the current state is preemptive, the current state will implicitly turn off when a new state turns on.
        // If the current state is unpreemptive, the current state keeps on, the new state will remain off. 
        public void TurnOn (string layer, string state) {
            if (fsmManager.ContainsKey (layer)) {
                FSM fsm = fsmManager[layer];

                if (fsm.Contains (state)) {
                    fsm.TurnOn (state);
                    return;
                }

                throw new System.ArgumentException (
                    $"Provided State[{state}] Not Found. Please Add State First with AddState(layer, state)."
                );
            }
            throw new System.ArgumentException (
                $"Provided Layer[{layer}] Not Found. Please Add Layer First with AddState(layer, state)."
            );
        }

        // Turn off an layer.
        // No actived state in the layer.
        public void TurnOff (string layer) {
            if (fsmManager.ContainsKey (layer)) {
                FSM fsm = fsmManager[layer];

                fsm.TurnOff ();
                return;
            }
            throw new System.ArgumentException (
                $"Provided Layer[{layer}] Not Found. Please Add Layer First with AddState(layer, state)."
            );
        }

        // Return true if a provided state is actived.
        public bool IsOn (string layer, string state) {
            if (fsmManager.ContainsKey (layer)) {
                FSM fsm = fsmManager[layer];

                return fsm.IsActived (state);
            }
            throw new System.ArgumentException (
                $"Provided Layer[{layer}] Not Found. Please Add Layer First with AddState(layer, state)."
            );
        }

        // Return the current state in the given layer.
        public string GetState (string layer) {
            if (fsmManager.ContainsKey (layer))
                return fsmManager[layer].on;

            throw new System.ArgumentException (
                $"Provided Layer[{layer}] Not Found. Please Add Layer First with AddState(layer, state)."
            );
        }
    }

    //  
    public class FSM {
        public Dictionary<string, StateData> fsm = new Dictionary<string, StateData> ();
        public string on;

        public void AddState (string state, bool actived, bool preemptible) {
            if (actived) on = state;
            fsm.Add (state, new StateData (actived, preemptible));
        }

        public bool Contains (string state) {
            return fsm.ContainsKey (state);
        }

        public void TurnOn (string state) {
            /// #[1]# To fix:
            /// Iterate fsm, turn off all states before turn on.
            /// note: the property `on` is causing the problem.
            ///       but avoided a for loop.
            /// Or use some way to deny the illegal operation.
            if (on != null) {
                if (!fsm[on].preemptible) return;
                fsm[on].actived = false;
            }

            fsm[state].actived = true;
            on = state;
        }

        public void TurnOff () {
            if (on != null) {
                fsm[on].actived = false;
                on = null;
            }

        }

        public bool IsActived (string state) {
            return fsm[state].actived;
        }

        public string GetState () {
            return on;
        }
    }

    public class StateData {
        public bool actived;
        // If preemptible is true, other states can interrupt a previous state
        // If preemptible is false, other states cannot iterrupt a previous state
        // An unpreemptible state, can only unlocked by explicitly turnning off.
        public bool preemptible;

        public StateData (bool newActived, bool newPreemptible) {
            actived = newActived;
            preemptible = newPreemptible;

        }
    }
}