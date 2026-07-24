using System.Linq;
using UnityEngine;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;

namespace GhostPlando {
    public static class GhostFsmUtil {
        public static FsmFloat AddFsmFloat(this PlayMakerFSM fsm, string name, float value) {
            FsmFloat ff = new FsmFloat {
                Name = name,
                Value = value
            };

            FsmFloat[] floats = new FsmFloat[fsm.FsmVariables.FloatVariables.Length + 1];
            fsm.FsmVariables.FloatVariables.CopyTo(floats, 0);
            floats[floats.Length - 1] = ff;
            fsm.FsmVariables.FloatVariables = floats;

            return ff;
        }

        public static FsmOwnerDefault FsmGOToOwner(this PlayMakerFSM fsm, string name) {
            return new FsmOwnerDefault {
                GameObject = fsm.FsmVariables.GetFsmGameObject(name),
                OwnerOption = OwnerDefaultOption.SpecifyGameObject
            };
        }

        public static FsmEventTarget FsmGOToEvent(this PlayMakerFSM fsm, string name) {
            return new FsmEventTarget {
                target = FsmEventTarget.EventTarget.GameObject,
                gameObject = fsm.FsmGOToOwner(name)
            };
        }

        public static FsmOwnerDefault GameObjectToOwner(this GameObject go) {
            return new FsmOwnerDefault {
                GameObject = new FsmGameObject { Value = go },
                OwnerOption = OwnerDefaultOption.SpecifyGameObject
            };
        }

        public static FsmEventTarget GameObjectToEventTarget(this GameObject go) {
            return new FsmEventTarget {
                target = FsmEventTarget.EventTarget.GameObject,
                gameObject = go.GameObjectToOwner()
            };
        }

        public static void AddFirstActions(this FsmState state, FsmStateAction[] actions) {
            for(int i = 0; i < actions.Length; i++) {
                state.InsertAction(actions[i], i);
            }
        }
        
        public static void AddLastActions(this FsmState state, FsmStateAction[] actions) {
            foreach(FsmStateAction action in actions) {
                state.AddLastAction(action);
            }
        }

        public static Vector2[] AssignFromTuples2(params (float, float)[] values) {
            return values.Select(tuple => new Vector2(tuple.Item1, tuple.Item2)).ToArray();
        }

        public static Vector3[] AssignFromTuples3(params (float, float)[] values) {
            return values.Select(tuple => new Vector3(tuple.Item1, tuple.Item2)).ToArray();
        }

        public static FsmState ChainFsmStates(this FsmState state, params (string, FsmStateAction[])[] next) {
            FsmState previousState = state;
            foreach((string name, FsmStateAction[] actions) in next) {
                FsmState newState = state.Fsm.FsmComponent.AddState(name);
                newState.Actions = actions;
                previousState.AddTransition("FINISHED", newState);
                previousState = newState;
            }
            return previousState;
        }
    }
}
