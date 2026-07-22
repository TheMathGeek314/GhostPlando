using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class BrokenVessel {
        internal static GameObject vesselPrefab;
        internal static GameObject projectileObject;
        public static ILHook JarHook;
        private static FieldInfo thisField;

        public static void Hook(PlayMakerFSM nosk) {
            projectileObject = vesselPrefab.LocateMyFSM("IK Control").GetState("Dstab Land").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            MethodInfo method = typeof(SpawnJarControl).GetMethod("Behaviour", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget();
            thisField = method.DeclaringType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First(f => f.FieldType == typeof(SpawnJarControl));
            JarHook = new ILHook(method, SpawnOnBreak);
        }

        private static void SpawnOnBreak(ILContext il) {
            ILCursor cursor = new ILCursor(il).Goto(0);
            cursor.GotoNext(i => i.MatchLdloc(1),
                            i => i.MatchLdfld<SpawnJarControl>("enemyToSpawn"));
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, thisField);
            cursor.EmitDelegate<Action<SpawnJarControl>>(jar => {
                foreach(float velocity in new float[] { -19f, -10f, 10f, 19f }) {
                    GameObject proj = GameObject.Instantiate(projectileObject, jar.gameObject.transform.position, Quaternion.identity);
                    proj.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity, 0);
                    proj.SetActive(true);
                }
            });
        }
    }
}
