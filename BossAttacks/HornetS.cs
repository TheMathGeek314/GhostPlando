using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class HornetS {
        internal static GameObject hornetPrefab;
        internal static GameObject barbRegionPrefab;
        internal static GameObject barbPrefab;
        internal static Object barbAudio;

        public static void Hook(PlayMakerFSM wosk) {
            barbAudio = hornetPrefab.LocateMyFSM("Control").GetState("Barb Antic").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            barbPrefab = barbRegionPrefab.GetComponent<PlayMakerFSM>().GetState("Spawn 1").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
        }

        public static FsmStateAction[] SpawnBarb(PlayMakerFSM self) {
            FsmFloat wx = self.FsmVariables.GetFsmFloat("wx");
            FsmFloat wy = self.FsmVariables.GetFsmFloat("wy");
            return [
                new GetPosition {
                    gameObject = self.FsmGOToOwner("Wosk"),
                    vector = Vector3.zero,
                    x = wx,
                    y = wy,
                    z = 0,
                    space = Space.World,
                    everyFrame = false
                },
                new Lambda(() => {
                    Vector3 rPosition = new Vector3(wx.Value + Random.Range(-3f, 3f), wy.Value + Random.Range(-1f, 5f), 0);
                    GameObject barb = GameObject.Instantiate(barbPrefab, rPosition, Quaternion.identity, null);
                    barb.SetActive(true);
                    self.FsmVariables.GetFsmGameObject("Globarb Instance").Value = barb;
                }),
                new NextFrameEvent {
                    sendEvent = FsmEvent.Finished
                }
            ];
        }

        public static FsmStateAction[] TriggerBarb(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("Globarb Instance"),
                    sendEvent = "BARB READY",
                    delay = 0,
                    everyFrame = false
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Wosk"),
                    audioClip = barbAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store") 
                },
                new Wait {
                    time = 0.5f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }
    }
}
