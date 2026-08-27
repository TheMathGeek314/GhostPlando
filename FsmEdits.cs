using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando {
    internal static class FsmEdits {
        public static FsmStateAction[] RoarEndActions(PlayMakerFSM self, GameObject title, GameObject fader, FsmState trans1) {
            return [
                new Wait {
                    time = 2.5f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                },
                new Lambda(() => {
                    Vector3 titlePosition = GameCameras.instance.mainCamera.transform.position + new Vector3(-0.4479f, 4.8225f, 0);
                    title.transform.position = new Vector3(titlePosition.x, titlePosition.y, -1.695f);
                    fader.transform.position = title.transform.position + new Vector3(0.5f, -2.67f, -8.93f);
                    fader.transform.localScale = new Vector3(37.6446f, 26.62126f, 1);
                    GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("OUT INSTANT");
                }),
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Nosk Title Fader"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Nosk Title"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new TransitionToAudioSnapshot {
                    snapshot = trans1.GetFirstActionOfType<TransitionToAudioSnapshot>().snapshot,
                    transitionTime = 0
                },
                new ApplyMusicCue {
                    musicCue = trans1.GetFirstActionOfType<ApplyMusicCue>().musicCue,
                    delayTime = 0,
                    transitionTime = 0
                }
            ];
        }

        public static FsmStateAction[] RoarInitActions(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("Nosk Title", true),
                    sendEvent = "DOWN",
                    delay = 0,
                    everyFrame = false
                },
                new iTweenFadeTo {
                    gameObject = self.FsmGOToOwner("Nosk Title Fader"),
                    alpha = 0,
                    includeChildren = true,
                    namedValueColor = "_Color",
                    time = 0.1f,
                    delay = 0,
                    easeType = iTween.EaseType.linear,
                    loopType = iTween.LoopType.none,
                    finishEvent = FsmEvent.Finished,
                    realTime = false,
                    stopOnExit = true,
                    loopDontFinish = true
                },
                new Lambda(() => {
                    GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("IN");
                })
            ];
        }

        public static FsmStateAction[] PhaseDecisionActions(PlayMakerFSM self) {
            return [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 1-2 HP"),
                    greaterThan = FsmEvent.GetFsmEvent("ONE"),
                    everyFrame = false
                },
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 2-3 HP"),
                    greaterThan = FsmEvent.GetFsmEvent("TWO"),
                    everyFrame = false
                },
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 3-4 HP"),
                    equal = FsmEvent.GetFsmEvent("FOUR"),
                    lessThan = FsmEvent.GetFsmEvent("FOUR"),
                    greaterThan = FsmEvent.GetFsmEvent("THREE"),
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction PhaseDecisionActions2(PlayMakerFSM self) {
            return new IntCompare {
                integer1 = self.FsmVariables.GetFsmInt("HP"),
                integer2 = self.FsmVariables.GetFsmInt("Phase 5-6 HP"),
                greaterThan = FsmEvent.GetFsmEvent("FIVE"),
                equal = FsmEvent.GetFsmEvent("SIX"),
                lessThan = FsmEvent.GetFsmEvent("SIX"),
                everyFrame = false
            };
        }

        public static FsmStateAction[] HoveringActions(PlayMakerFSM self) {
            List<FsmStateAction> actions = [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 3-4 HP"),
                    greaterThan = FsmEvent.Finished,
                    everyFrame = false
                },
                new SetGravity2dScale {
                    gameObject = self.FsmGOToOwner("Self"),
                    gravityScale = 0
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Self"),
                    vector = Vector2.zero,
                    x = 0,
                    y = 0,
                    everyFrame = false
                }
            ];
            actions.AddRange(BossAttacks.HornetP.RunThreadstorm(self));
            actions.AddRange(BossAttacks.Grimm.RunFlames(self));
            actions.Add(new Wait {
                time = 1f,
                finishEvent = FsmEvent.Finished,
                realTime = false
            });
            return actions.ToArray();
        }

        public static FsmStateAction[] RevertCeilingFall(PlayMakerFSM self) {
            return [
                new SetBoolValue {
                    boolVariable = self.FsmVariables.GetFsmBool("Falling from Ceiling"),
                    boolValue = false,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] AfterFallActions(PlayMakerFSM self) {
            return [
                new Wait {
                    time = self.FsmVariables.GetFsmFloat("Wait After Falling"),
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] ResetFallWait(PlayMakerFSM self) {
            return [
                new SetFloatValue {
                    floatVariable = self.FsmVariables.GetFsmFloat("Wait After Falling"),
                    floatValue = 0,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] ReduceJumpForce(PlayMakerFSM self) {
            return [
                new Lambda(() => {
                    FsmVariables fv = self.FsmVariables;
                    if(fv.GetFsmInt("HP").Value <= fv.GetFsmInt("Phase 3-4 HP").Value)
                        fv.GetFsmFloat("Jump Force").Value = 41;
                })
            ];
        }

        public static FsmStateAction[] IdleActions(PlayMakerFSM self) {
            return [
                new GetHP {
                    target = self.FsmGOToOwner("Self"),
                    storeValue = self.FsmVariables.GetFsmInt("HP")
                },
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 4-5 HP"),
                    lessThan = FsmEvent.GetFsmEvent("WINGIFY"),
                    equal = FsmEvent.GetFsmEvent("WINGIFY"),
                    everyFrame = false
                },
                new CheckPhaseChange {
                    ChangeEvent = FsmEvent.GetFsmEvent("PHASE ROOF")
                }
            ];
        }

        public static FsmStateAction[] MutateAnticActions(PlayMakerFSM self) {
            return [
                new Tk2dPlayAnimationWithEvents {
                    gameObject = self.FsmGOToOwner("Self"),
                    clipName = "Antic",
                    animationCompleteEvent = FsmEvent.Finished
                },
                new FaceObject {
                    objectA = self.FsmVariables.GetFsmGameObject("Self"),
                    objectB = HeroController.instance.gameObject,
                    spriteFacesRight = false,
                    playNewAnimation = false,
                    resetFrame = false,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] MutateJumpActions(PlayMakerFSM self) {
            return [
                new SetGravity2dScale {
                    gameObject = self.FsmGOToOwner("Self"),
                    gravityScale = 0
                },
                new AudioPlaySimple {
                    gameObject = self.FsmGOToOwner("Self"),
                    volume = 1,
                    oneShotClip = self.GetState("RS Jump").GetFirstActionOfType<AudioPlaySimple>().oneShotClip
                },
                new Tk2dPlayAnimation {
                    gameObject = self.FsmGOToOwner("Self"),
                    animLibName = "",
                    clipName = "Jump"
                },
                new GetPosition {
                    gameObject = self.FsmGOToOwner("Self"),
                    vector = Vector3.zero,
                    x = 0,
                    y = self.FsmVariables.GetFsmFloat("Self Y"),
                    z = 0,
                    space = Space.World,
                    everyFrame = true
                },
                new FloatCompare {
                    float1 = self.FsmVariables.GetFsmFloat("Self Y"),
                    float2 = self.FsmVariables.GetFsmFloat("Roof Y"),
                    tolerance = 0,
                    greaterThan = FsmEvent.GetFsmEvent("LAND"),
                    everyFrame = true
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Self"),
                    vector = Vector2.zero,
                    x = 0,
                    y = 55,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] MutateWaitActions(PlayMakerFSM self) {
            return [
                new Lambda(() => {
                    self.gameObject.GetComponent<HealthManager>().IsInvincible = true;
                }),
                new Wait {
                    time = 1,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] MutateSwitchActions(PlayMakerFSM self, int hp) {
            return [
                new Lambda(() => {
                    GameObject wosk = self.FsmVariables.GetFsmGameObject("Winged Nosk").Value;
                    wosk.GetComponent<HealthManager>().hp = hp;
                    wosk.SetActive(true);
                }),
                new DestroySelf()
            ];
        }

        public static FsmStateAction ZeroVelocity(PlayMakerFSM self) {
            return new SetVelocity2d {
                gameObject = new FsmOwnerDefault { OwnerOption = OwnerDefaultOption.UseOwner },
                vector = Vector2.zero,
                x = 0,
                y = 0,
                everyFrame = false
            };
        }

        public static FsmStateAction[] UpdateSpinners(PlayMakerFSM self) {
            FsmVariables fv = self.FsmVariables;
            return [
                ..BossAttacks.SoulMaster.StopSpinner(self),
                new IntCompare {
                    integer1 = fv.GetFsmInt("HP"),
                    integer2 = fv.GetFsmInt("Phase 5-6 HP"),
                    greaterThan = FsmEvent.Finished,
                    everyFrame = false
                },
                new IntCompare {
                    integer1 = fv.GetFsmInt("Current Boss Phase"),
                    integer2 = 6,
                    equal = FsmEvent.Finished,
                    everyFrame = false
                },
                .. BossAttacks.Markoth.RunShields(self)
            ];
        }

        public static FsmStateAction[] SwoopStartActions(PlayMakerFSM self) {
           return [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 5-6 HP"),
                    lessThan = FsmEvent.Finished,
                    equal = FsmEvent.Finished,
                    everyFrame = false
                },
               .. BossAttacks.HiveKnight.RunGlobarbs(self)
            ];
        }

        public static FsmStateAction[] DoFireSwoop(PlayMakerFSM self) {
            return [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 5-6 HP"),
                    lessThan = FsmEvent.GetFsmEvent("WITH"),
                    equal = FsmEvent.GetFsmEvent("WITH"),
                    greaterThan = FsmEvent.GetFsmEvent("WITHOUT"),
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] GlobsActions(PlayMakerFSM self) {
            return [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 5-6 HP"),
                    lessThan = FsmEvent.GetFsmEvent("SIX"),
                    equal = FsmEvent.GetFsmEvent("SIX"),
                    everyFrame = false
                },
                .. BossAttacks.Sheo.RunPurplePaint(self),
                .. BossAttacks.HiveKnight.RunSwarm(self)
            ];
        }

        public static FsmStateAction[] SummonBalloonActions(PlayMakerFSM self, GameObject balloon) {
            return [
                new SpawnObjectFromGlobalPool {
                    gameObject = balloon,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Zero Origin"),
                    position = self.FsmVariables.GetFsmVector3("Spawn Vector"),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Spawned Enemy")
                },
                new EnemyPusherIgnore {
                    target = new FsmOwnerDefault { OwnerOption = OwnerDefaultOption.UseOwner },
                    other = self.FsmVariables.GetFsmGameObject("Spawned Enemy")
                }
            ];
        }

        public static FsmStateAction[] SummonChoice(PlayMakerFSM self) {
            return [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 5-6 HP"),
                    greaterThan = FsmEvent.GetFsmEvent("Balloon"),
                    everyFrame = false
                },
                new SendRandomEvent {
                    events = new List<string> { "Balloon", "Vengefly", "Fluke", "Folly", "Zote" }.Select(s => FsmEvent.GetFsmEvent(s)).ToArray(),
                    weights = [0.4f, 1f, 1f, 1f, 1f],
                    delay = 0
                }
            ];
        }

        public static FsmStateAction[] BossDeathActions(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = new FsmEventTarget { target = FsmEventTarget.EventTarget.BroadcastAll },
                    sendEvent = "GHOST DEATH",
                    delay = 0,
                    everyFrame = false
                },
                new SetFsmBool {
                    gameObject = HeroController.instance.gameObject.GameObjectToOwner(),
                    fsmName = "Spell Control",
                    variableName = "Dream Focus",
                    setValue = true,
                    everyFrame = false
                },
                new Lambda(() => {
                    ParagonDeathEffects.particleSystem = self.GetState("Steam").GetFirstActionOfType<SetParticleEmissionRate>().gameObject.GameObject.Value.GetComponent<ParticleSystem>();
                    GameManager.instance.StartCoroutine(ParagonDeathEffects.instance.BeginEffects());
                })
            ];
        }
    }

    public class CheckPhaseChange: FsmStateAction {
        private static List<int> hpPhases;
        public FsmEvent ChangeEvent;

        public static void SetupThresholds(int[] hpHits, int multiplier) {
            int hp = 0;
            hpPhases = [0];
            for(int i = hpHits.Length - 1; i >= 0; i--) {
                hp += multiplier * hpHits[i];
                hpPhases.Insert(0, hp);
            }
        }

        public override void OnEnter() {
            FsmVariables fv = Fsm.FsmComponent.FsmVariables;
            int hp = fv.GetFsmInt("HP").Value;
            FsmInt phase = fv.GetFsmInt("Current Boss Phase");
            if(hp <= hpPhases[phase.Value]) {
                phase.Value++;
                base.Fsm.Event(ChangeEvent);
            }
            Finish();
        }
    }
}
