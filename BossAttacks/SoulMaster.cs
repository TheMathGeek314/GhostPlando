using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class SoulMaster {
        internal static GameObject mageLordPrefab;
        internal static Object quakeAudio;
        internal static GameObject shockwavePoolObject;
        internal const string hitName = "Soul Master Hit";
        internal const string blastName = "Soul Master Blast";
        internal const string pillarName = "Soul Master Pillar";
        internal const string flashName = "Soul Master Flash";
        internal const string particlesName = "Soul Master Particles";

        public static void Hook(PlayMakerFSM nosk) {
            GameObject mage = GameObject.Instantiate(mageLordPrefab);
            //shockwave
            foreach((string fieldName, string childName, Vector3 offset) in new (string, string, Vector3)[] {
                (hitName, "Quake Hit", Vector3.zero),
                (blastName, "Quake Blast", new Vector3(0.47f, 0.66f, 0)),
                (pillarName, "Quake Pillar", new Vector3(-0.18f, 7.04f, 0)),
                (flashName, "White Flash", new Vector3(-0.14f, -0.14f, 0.01f)),
                (particlesName, "Quake Particles", new Vector3(0, -3.1f, -0.001f))
            }) {
                nosk.AddFsmGameObject(fieldName, GameObject.Instantiate(mage.FindChild(childName), nosk.transform.position + offset, Quaternion.identity, nosk.transform));
            }
            nosk.AddFsmGameObject("Quake Shockwave", null);
            PlayMakerFSM mageLordFsm = mageLordPrefab.LocateMyFSM("Mage Lord");
            quakeAudio = mageLordFsm.GetState("Quake Land").GetFirstActionOfType<AudioPlaySimple>().oneShotClip.Value;
            shockwavePoolObject = mageLordFsm.GetState("Quake Waves").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            GameObject.Destroy(mage);
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
    }
}