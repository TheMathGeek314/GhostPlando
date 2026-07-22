using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class HornetP {
        internal static GameObject hornetPrefab;
        internal static Object throwAudio;
        internal static Object throwAndReturnAudio;
        internal const string ballName = "Hornet Protector Ball";
        internal const string needleName = "Hornet Protector Needle";
        internal const string tinkName = "Hornet Protector Tink";
        internal const string angleName = "Hornet Protector Angle";

        public static void Hook(PlayMakerFSM nosk) {
            GameObject hornet = GameObject.Instantiate(hornetPrefab);
            PlayMakerFSM hornetFsm = hornetPrefab.LocateMyFSM("Control");
            throwAudio = hornetFsm.GetState("Sphere A").GetFirstActionOfType<AudioPlaySimple>().oneShotClip.Value;
            throwAndReturnAudio = hornetFsm.GetState("Throw").GetFirstActionOfType<AudioPlaySimple>().oneShotClip.Value;
            //threadstorm
            GameObject ball = GameObject.Instantiate(hornet.FindChild("Sphere Ball"), nosk.transform.position, Quaternion.identity, nosk.transform);
            PlayMakerFSM growFsm = ball.GetComponent<PlayMakerFSM>();
            FsmState growState = growFsm.GetState("Grow");
            FsmState waitState = growFsm.AddState("Delay");
            FsmState destroyState = growFsm.AddState("Destroy");
            iTweenScaleTo tween = growState.GetFirstActionOfType<iTweenScaleTo>();
            tween.finishEvent = new FsmEvent("FINISHED");
            growState.AddFirstAction(new Lambda(() => {
                if(growFsm.gameObject.TryGetComponent(out iTweenFSMEvents itfe)) {
                    GameObject.Destroy(itfe);
                }
            }));
            growState.AddTransition("FINISHED", "Delay");
            waitState.AddFirstAction(new Wait() {
                time = 1f,
                finishEvent = FsmEvent.GetFsmEvent("FINISHED"),
                realTime = false
            });
            waitState.AddTransition("FINISHED", "Destroy");
            destroyState.AddFirstAction(new ActivateGameObject {
                gameObject = ball.GameObjectToOwner(),
                activate = false,
                recursive = false,
                resetOnExit = false,
                everyFrame = false
            });
            nosk.AddFsmGameObject(ballName, ball);

            //needle
            GameObject needle = GameObject.Instantiate(hornet.FindChild("Needle"));
            needle.GetComponent<MeshRenderer>().enabled = true;
            nosk.AddFsmFloat(angleName, 0);
            nosk.AddFsmGameObject(needleName, needle);
            //needle tink
            GameObject tink = needle.FindChild("Needle Tink");
            PlayMakerFSM tinkFsm = tink.GetComponent<PlayMakerFSM>();
            tinkFsm.GetState("Setup").GetFirstActionOfType<SetFsmGameObject>().Enabled = false;
            tink.SetActive(true);
            nosk.AddFsmGameObject(tinkName, tink);
            needle.GetComponent<PlayMakerFSM>().GetState("Notify").AddFirstAction(new Lambda(() => {
                tink.GetComponent<BoxCollider2D>().enabled = false;
                needle.SetActive(false);
            }));
            GameObject.Destroy(hornet);
        }

        public static FsmStateAction[] RunThreadstorm(PlayMakerFSM self) {
            return [
                new Lambda(() => self.FsmVariables.GetFsmGameObject(ballName).Value.GetComponent<PlayMakerFSM>().SetState("Grow")),
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner(ballName),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = throwAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                }
            ];
        }

        public static FsmStateAction[] RunNeedle(PlayMakerFSM self) {
            return [
                new BoolTest {
                    boolVariable = self.FsmVariables.GetFsmBool("Falling from Ceiling"),
                    isTrue = FsmEvent.GetFsmEvent("FINISHED"),
                    everyFrame = false
                },
                new GetAngleToTarget2D {
                    gameObject = self.gameObject.GameObjectToOwner(),
                    target = HeroController.instance.gameObject,
                    offsetX = 0,
                    offsetY = 0,
                    storeAngle = self.FsmVariables.GetFsmFloat(angleName),
                    everyFrame = false
                },
                new Lambda(() => {
                    GameObject needle = self.FsmVariables.GetFsmGameObject(needleName).Value;
                    needle.transform.position = self.transform.position + new Vector3(0, -1.5f, 0);
                    needle.SetActive(true);
                    self.FsmVariables.GetFsmGameObject(tinkName).Value.GetComponent<BoxCollider2D>().enabled = true;
                    FsmFloat angleFF = self.FsmVariables.GetFsmFloat(angleName);
                    angleFF.Value = (angleFF.Value > 90 && angleFF.Value <= 270) ? 180 : 0;
                }),
                new SetVelocityAsAngle {
                    gameObject = self.FsmGOToOwner(needleName),
                    angle = self.FsmVariables.GetFsmFloat(angleName),
                    speed = 38,
                    everyFrame = false
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = throwAndReturnAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
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