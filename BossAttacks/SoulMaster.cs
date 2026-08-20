using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class SoulMaster {
        internal static GameObject mageLordPrefab;
        internal static GameObject shockwavePoolObject;
        internal static GameObject orbObject;
        internal static Object quakeAudio;
        internal static Object chargeAudio;
        internal static Object shootAudio;
        internal const string hitName = "Soul Master Hit";
        internal const string blastName = "Soul Master Blast";
        internal const string pillarName = "Soul Master Pillar";
        internal const string flashName = "Soul Master Flash";
        internal const string particlesName = "Soul Master Particles";

        public static void HookNosk(PlayMakerFSM nosk) {
            foreach((string fieldName, string childName, Vector3 offset) in new (string, string, Vector3)[] {
                (hitName, "Quake Hit", Vector3.zero),
                (blastName, "Quake Blast", new Vector3(0.47f, 0.66f, 0)),
                (pillarName, "Quake Pillar", new Vector3(-0.18f, 7.04f, 0)),
                (flashName, "White Flash", new Vector3(-0.14f, -0.14f, 0.01f)),
                (particlesName, "Quake Particles", new Vector3(0, -3.1f, -0.001f))
            }) {
                nosk.AddFsmGameObject(fieldName, GameObject.Instantiate(mageLordPrefab.FindChild(childName), nosk.transform.position + offset, Quaternion.identity, nosk.transform));
            }
            nosk.AddFsmGameObject("Quake Shockwave", null);
            PlayMakerFSM mageLordFsm = mageLordPrefab.LocateMyFSM("Mage Lord");
            shockwavePoolObject = mageLordFsm.GetState("Quake Waves").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            quakeAudio = mageLordFsm.GetState("Quake Land").GetFirstActionOfType<AudioPlaySimple>().oneShotClip.Value;
        }

        public static void HookWosk(PlayMakerFSM wosk) {
            PlayMakerFSM mageLordFsm = mageLordPrefab.LocateMyFSM("Mage Lord");
            chargeAudio = mageLordFsm.GetState("HS Summon").GetFirstActionOfType<AudioPlaySimple>().oneShotClip.Value;
            On.PlayMakerFSM.OnEnable += ReduceSpinnerRange;
            wosk.AddFsmGameObject("Soul Master Spinner", GameObject.Instantiate(mageLordPrefab.FindChild("Orb Spinner"), wosk.transform));
            FsmState shotState = mageLordFsm.GetState("Shot");
            orbObject = shotState.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            shootAudio = shotState.GetFirstActionOfType<AudioPlaySimple>().oneShotClip.Value;
            wosk.AddFsmFloat("SM Orb X", 0);
            wosk.AddFsmFloat("SM Orb Y", 0);
            wosk.AddFsmVector3("SM Orb Vector", Vector3.zero);
            wosk.AddFsmGameObject("SM Orb Projectile", null);
            wosk.AddFsmFloat("SM Orb Angle", 0);
            wosk.AddFsmFloat("SM Orb Looper", 0);
        }

        public static FsmStateAction[] RunQuake(PlayMakerFSM self) {
            return [
                new Lambda(() => {
                    FsmBool fallingBool = self.FsmVariables.GetFsmBool("Falling from Ceiling");
                    if(!fallingBool.Value)
                        self.SendEvent("FINISHED");
                    else {
                        self.FsmVariables.GetFsmGameObject(hitName).Value.SetActive(true);
                        self.FsmVariables.GetFsmGameObject(blastName).Value.SetActive(true);
                        self.FsmVariables.GetFsmGameObject(pillarName).Value.SetActive(true);
                        self.FsmVariables.GetFsmGameObject(flashName).Value.SetActive(true);
                    }
                }),
                new SendEventByName {
                    eventTarget = GameCameras.instance.mainCamera.gameObject.GameObjectToEventTarget(),
                    sendEvent = "BigShake",
                    delay = 0,
                    everyFrame = false
                },
                new PlayParticleEmitter {
                    gameObject = self.FsmGOToOwner(particlesName),
                    emit = 0
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = quakeAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = shockwavePoolObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(1.5f, 0, 0),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Quake Shockwave")
                },
                new Lambda(() => {
                    GameObject shockwave = self.FsmVariables.GetFsmGameObject("Quake Shockwave").Value;
                    shockwave.transform.position = new Vector3(self.transform.position.x, 2.82f, 0);
                    shockwave.transform.localScale = new Vector3(1.6f, 0, 0);
                    FsmVariables waveFV = shockwave.GetComponent<PlayMakerFSM>().FsmVariables;
                    waveFV.GetFsmBool("Facing Right").Value = true;
                    waveFV.GetFsmFloat("Speed").Value = 25;
                }),
                new SpawnObjectFromGlobalPool {
                    gameObject = shockwavePoolObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(-1.5f, 0, 0),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Quake Shockwave")
                },
                new Lambda(() => {
                    GameObject shockwave = self.FsmVariables.GetFsmGameObject("Quake Shockwave").Value;
                    shockwave.transform.position = new Vector3(self.transform.position.x, 2.82f, 0);
                    shockwave.transform.localScale = new Vector3(1.6f, 0, 0);
                    FsmVariables waveFV = shockwave.GetComponent<PlayMakerFSM>().FsmVariables;
                    waveFV.GetFsmBool("Facing Right").Value = false;
                    waveFV.GetFsmFloat("Speed").Value = 25;
                }),
            ];
        }

        public static FsmStateAction[] RunSpinner(PlayMakerFSM self) {
            return [
                new AudioPlaySimple {
                    gameObject = new FsmOwnerDefault { OwnerOption = OwnerDefaultOption.UseOwner },
                    volume = 1,
                    oneShotClip = chargeAudio
                },
                new SetFsmFloat {
                    gameObject = self.FsmGOToOwner("Soul Master Spinner"),
                    fsmName = "Spin Control",
                    variableName = "Spin Speed",
                    setValue = -75,
                    everyFrame = false
                },
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("Soul Master Spinner", true),
                    sendEvent = "SPINNER SUMMON",
                    delay = 0,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] StopSpinner(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("Soul Master Spinner", true),
                    sendEvent = "DISSIPATE",
                    delay = 0,
                    everyFrame = false
                }
            ];
        }

        internal static void ReduceSpinnerRange(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(GameManager.instance.sceneName == "Deepnest_32" && self.FsmName == "Summon Orbs") {
                foreach(iTweenMoveBy tween in self.GetState("Spawn").GetActionsOfType<iTweenMoveBy>()) {
                    tween.vector.Value *= 0.7f;
                }
            }
        }

        public static FsmStateAction[] PrepOrb(PlayMakerFSM self) {
            return [
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = chargeAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new SetFloatValue {
                    floatVariable = self.FsmVariables.GetFsmFloat("SM Orb Looper"),
                    floatValue = 0,
                    everyFrame = false
                },
                new Wait {
                    time = 0.7f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] FireOrb(PlayMakerFSM self) {
            FsmVariables fv = self.FsmVariables;
            return [
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = fv.GetFsmGameObject("Self"),
                    audioClip = shootAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = fv.GetFsmGameObject("Empty Store")
                },
                new RandomFloat {
                    min = -3,
                    max = 3,
                    storeResult = fv.GetFsmFloat("SM Orb X")
                },
                new RandomFloat {
                    min = -3,
                    max = 3,
                    storeResult = fv.GetFsmFloat("SM Orb Y")
                },
                new SetVector3XYZ {
                    vector3Variable = fv.GetFsmVector3("SM Orb Vector"),
                    vector3Value = Vector3.zero,
                    x = fv.GetFsmFloat("SM Orb X"),
                    y = fv.GetFsmFloat("SM Orb Y"),
                    z = 0,
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = orbObject,
                    spawnPoint = fv.GetFsmGameObject("Self"),
                    position = fv.GetFsmVector3("SM Orb Vector"),
                    rotation = Vector3.zero,
                    storeObject = fv.GetFsmGameObject("SM Orb Projectile")
                },
                new SetIsKinematic2d {
                    gameObject = self.FsmGOToOwner("SM Orb Projectile"),
                    isKinematic = false
                },
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("SM Orb Projectile"),
                    sendEvent = "FIRE",
                    delay = 0,
                    everyFrame = false
                },
                new GetAngleToTarget2D {
                    gameObject = self.FsmGOToOwner("SM Orb Projectile"),
                    target = fv.GetFsmGameObject("Hero"),
                    offsetX = 0,
                    offsetY = -0.5f,
                    storeAngle = fv.GetFsmFloat("SM Orb Angle"),
                    everyFrame = false
                },
                new SetVelocityAsAngle {
                    gameObject = self.FsmGOToOwner("SM Orb Projectile"),
                    angle = fv.GetFsmFloat("SM Orb Angle"),
                    speed = 20,
                    everyFrame = false
                },
                new SendRandomEvent {
                    events = [ FsmEvent.GetFsmEvent("LOOP"), FsmEvent.Finished ],
                    weights = [ 9, fv.GetFsmFloat("SM Orb Looper") ],
                    delay = 0
                }
            ];
        }

        public static FsmStateAction[] LoopOrb(PlayMakerFSM self) {
            return [
                new FloatAdd {
                    floatVariable = self.FsmVariables.GetFsmFloat("SM Orb Looper"),
                    add = 1,
                    perSecond = false,
                    everyFrame = false
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = chargeAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new Wait {
                    time = 0.6f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }
    }
}