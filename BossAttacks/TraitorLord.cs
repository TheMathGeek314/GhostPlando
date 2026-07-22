using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class TraitorLord {
        internal static GameObject tLordPrefab;
        internal static GameObject shockwaveObject;
        internal static AudioClip roarAudio;

        public static void Hook(PlayMakerFSM nosk) {
            PlayMakerFSM tlFsm = tLordPrefab.LocateMyFSM("Mantis");
            shockwaveObject = tlFsm.GetState("Waves").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            roarAudio = tlFsm.GetState("Slam Antic").GetFirstActionOfType<AudioPlayerOneShot>().audioClips[0];
            nosk.AddFsmGameObject("Traitor Wave", null);
            shockwaveObject.Spawn().Recycle();
        }

        public static FsmStateAction[] RunShockwave(PlayMakerFSM self) {
            return [
                new BoolTest {
                    boolVariable = self.FsmVariables.GetFsmBool("Falling from Ceiling"),
                    isFalse = FsmEvent.GetFsmEvent("FINISHED"),
                    everyFrame = false
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = roarAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store"),
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = shockwaveObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0.25f, -4.0413f, 6.1993f),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Traitor Wave")
                },
                new SetScale {
                    gameObject = self.FsmGOToOwner("Traitor Wave"),
                    vector = Vector3.zero,
                    x = 1,
                    y = 1,
                    z = 1,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Traitor Wave"),
                    vector = Vector2.zero,
                    x = 16,
                    y = 0,
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = shockwaveObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(-0.25f, -4.0413f, 6.1993f),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Traitor Wave")
                },
                new SetScale {
                    gameObject = self.FsmGOToOwner("Traitor Wave"),
                    vector = Vector3.zero,
                    x = -1,
                    y = 1,
                    z = 1,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Traitor Wave"),
                    vector = Vector2.zero,
                    x = -16,
                    y = 0,
                    everyFrame = false
                },
                new SetFloatValue {
                    floatVariable = self.FsmVariables.GetFsmFloat("Wait After Falling"),
                    floatValue = 0.5f,
                    everyFrame = false
                }
            ];
        }
    }
}
