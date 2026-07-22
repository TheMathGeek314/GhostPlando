using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class WDefender {
        internal static GameObject defenderPrefab;
        internal static GameObject pillarObject;
        internal static Object bounceAudio;

        public static void Hook(PlayMakerFSM nosk) {
            FsmState pillarState = defenderPrefab.LocateMyFSM("Dung Defender").GetState("Pillar");
            bounceAudio = pillarState.GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            pillarObject = pillarState.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            
            PlayMakerFSM pillarFsm = nosk.gameObject.AddComponent<PlayMakerFSM>();
            pillarFsm.FsmName = "White Defender Pillars";
            FsmState firstState = pillarFsm.GetState(pillarFsm.ActiveStateName);
            FsmState setupState = pillarFsm.AddState("Setup");
            FsmState spawnState = pillarFsm.AddState("Spawn");
            FsmState incrementState = pillarFsm.AddState("Increment");
            pillarFsm.Fsm.Events = [new FsmEvent("TRIGGER"), new FsmEvent("END"), new FsmEvent("FINISHED")];
            firstState.AddTransition("TRIGGER", "Setup");
            setupState.AddTransition("FINISHED", "Spawn");
            spawnState.AddTransition("FINISHED", "Increment");
            spawnState.AddTransition("END", firstState.Name);
            incrementState.AddTransition("FINISHED", "Spawn");
            pillarFsm.AddFsmGameObject("Self", nosk.gameObject);
            pillarFsm.AddFsmGameObject("WD Pillar", null);
            pillarFsm.AddFsmFloat("Pillar X L", 0f);
            pillarFsm.AddFsmFloat("Pillar X R", 0f);
            pillarFsm.AddFsmInt("Pillars Remaining", 15);

            setupState.Actions = [
                new Lambda(() => {
                    float posX = pillarFsm.FsmVariables.GetFsmGameObject("Self").Value.transform.position.x;
                    FsmVariables fv = pillarFsm.FsmVariables;
                    fv.GetFsmFloat("Pillar X L").Value = posX - 1;
                    fv.GetFsmFloat("Pillar X R").Value = posX + 1;
                    fv.GetFsmInt("Pillars Remaining").Value = 15;
                })
            ];
            spawnState.Actions = [
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = pillarFsm.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = bounceAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = nosk.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = pillarObject,
                    spawnPoint = pillarFsm.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    storeObject = pillarFsm.FsmVariables.GetFsmGameObject("WD Pillar")
                },
                new SetDamageHeroAmount {
                    target = pillarFsm.FsmGOToOwner("WD Pillar"),
                    damageDealt = 1
                },
                new SetScale {
                    gameObject = pillarFsm.FsmGOToOwner("WD Pillar"),
                    vector = Vector3.zero,
                    x = -1,
                    y = 1,
                    z = 0,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SetPosition {
                    gameObject = pillarFsm.FsmGOToOwner("WD Pillar"),
                    vector = Vector3.zero,
                    x = pillarFsm.FsmVariables.GetFsmFloat("Pillar X R"),
                    y = 7.68f,
                    z = 0,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = pillarObject,
                    spawnPoint = pillarFsm.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    storeObject = pillarFsm.FsmVariables.GetFsmGameObject("WD Pillar")
                },
                new SetDamageHeroAmount {
                    target = pillarFsm.FsmGOToOwner("WD Pillar"),
                    damageDealt = 1
                },
                new SetScale {
                    gameObject = pillarFsm.FsmGOToOwner("WD Pillar"),
                    vector = Vector3.zero,
                    x = 1,
                    y = 1,
                    z = 0,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SetPosition {
                    gameObject = pillarFsm.FsmGOToOwner("WD Pillar"),
                    vector = Vector3.zero,
                    x = pillarFsm.FsmVariables.GetFsmFloat("Pillar X L"),
                    y = 7.68f,
                    z = 0,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new IntOperator {
                    integer1 = pillarFsm.FsmVariables.GetFsmInt("Pillars Remaining"),
                    integer2 = 1,
                    operation = IntOperator.Operation.Subtract,
                    storeResult = pillarFsm.FsmVariables.GetFsmInt("Pillars Remaining"),
                    everyFrame = false
                },
                new IntCompare {
                    integer1 = pillarFsm.FsmVariables.GetFsmInt("Pillars Remaining"),
                    integer2 = 0,
                    equal = FsmEvent.GetFsmEvent("END"),
                    lessThan = FsmEvent.GetFsmEvent("END"),
                    greaterThan = null,
                    everyFrame = false
                },
                new Wait {
                    time = 0.125f,
                    finishEvent = FsmEvent.GetFsmEvent("FINISHED"),
                    realTime = false
                }
            ];
            incrementState.Actions = [
                new FloatAdd {
                    floatVariable = pillarFsm.FsmVariables.GetFsmFloat("Pillar X L"),
                    add = -2.5f,
                    everyFrame = false,
                    perSecond = false
                },
                new FloatAdd {
                    floatVariable = pillarFsm.FsmVariables.GetFsmFloat("Pillar X R"),
                    add = 2.5f,
                    everyFrame = false,
                    perSecond = false
                }
            ];
        }

        public static FsmStateAction[] RunShockwave(PlayMakerFSM self) {
            return [
                new BoolTest {
                    boolVariable = self.FsmVariables.GetFsmBool("Falling from Ceiling"),
                    isFalse = FsmEvent.GetFsmEvent("FINISHED"),
                    everyFrame = false
                },
                new Lambda(() => {
                    self.gameObject.LocateMyFSM("White Defender Pillars").SendEvent("TRIGGER");
                })
            ];
        }
    }
}
