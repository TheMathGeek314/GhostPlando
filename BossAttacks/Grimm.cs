using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class Grimm {
        internal static GameObject grimmPrefab;
        internal static GameObject spikeHolderPrefab;
        internal static GameObject flameObject;
        internal static GameObject batObject;
        internal static GameObject batPillarObject;
        internal static Object flameAudio;
        internal static Object batAudio;

        public static void Hook(PlayMakerFSM nosk) {
            PlayMakerFSM grimmFsm = grimmPrefab.LocateMyFSM("Control");
            FsmState explodeState = grimmFsm.GetState("UP Explode");
            flameObject = explodeState.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            flameAudio = explodeState.GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            FsmState firebatState = grimmFsm.GetState("Firebat 1");
            batAudio = firebatState.GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            SpawnObjectFromGlobalPool[] spawnActions = firebatState.GetActionsOfType<SpawnObjectFromGlobalPool>();
            batObject = spawnActions[0].gameObject.Value;
            batPillarObject = spawnActions[1].gameObject.Value;

            nosk.AddFsmGameObject("Grimm Projectile", null);
            nosk.AddFsmFloat("Firebat speed", 0);

            GameObject spikeHolder = GameObject.Instantiate(spikeHolderPrefab, new Vector3(71.3f, 1.34f, 0), Quaternion.identity);
            spikeHolder.SetActive(true);
            spikeHolder.GetComponent<PlayMakerFSM>().GetState("Ready").GetFirstActionOfType<SetPosition>().Enabled = false;
            nosk.AddFsmGameObject("Grimm Spike Holder", spikeHolder);
            batObject.Spawn().Recycle();
        }

        public static FsmStateAction[] RunFlames(PlayMakerFSM self) {
            return [
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = flameAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = flameObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Grimm Projectile")
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Grimm Projectile"),
                    vector = new Vector2(0, 1),
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = flameObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Grimm Projectile")
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Grimm Projectile"),
                    vector = new Vector2(9, 1),
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = flameObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Grimm Projectile")
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Grimm Projectile"),
                    vector = new Vector2(-9, 1),
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = flameObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Grimm Projectile")
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Grimm Projectile"),
                    vector = new Vector2(20, 1),
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = flameObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Grimm Projectile")
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Grimm Projectile"),
                    vector = new Vector2(-20, 1),
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] RunSpikes(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("Grimm Spike Holder"),
                    sendEvent = "SPIKE ATTACK",
                    delay = 0,
                    everyFrame = false
                },
                new Wait {
                    time = 0.4f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunFirebats(PlayMakerFSM self, float waitTime, float pitch, float height, bool isSlower) {
            return [
                new Lambda(() => {
                    self.FsmVariables.GetFsmFloat("Firebat speed").Value = (HeroController.instance.transform.position.x >= self.FsmVariables.GetFsmGameObject("Self").Value.transform.position.x ? 25 : -25) * (isSlower ? 0.75f : 1);
                }),
                new SpawnObjectFromGlobalPool {
                    gameObject = batObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, height, 0),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Grimm Projectile")
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = batPillarObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, 1.5f, 0),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Grimm Projectile"),
                    vector = Vector2.zero,
                    x = self.FsmVariables.GetFsmFloat("Firebat speed"),
                    y = 0,
                    everyFrame = false
                },
                new SetFsmFloat {
                    gameObject = self.FsmGOToOwner("Grimm Projectile"),
                    fsmName = "Control",
                    variableName = "X Velocity",
                    setValue = self.FsmVariables.GetFsmFloat("Firebat speed"),
                    everyFrame = false
                },
                new Wait {
                    time = waitTime,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = batAudio,
                    pitchMin = pitch,
                    pitchMax = pitch,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                }
            ];
        }
    }
}
