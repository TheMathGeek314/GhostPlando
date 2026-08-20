using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class TraitorLord {
        internal static GameObject tLordPrefab;
        internal static GameObject shockwaveObject;
        internal static GameObject sickleObject;
        internal static AudioClip roarAudio;
        internal static AudioClip sickleAudio;

        public static void HookNosk(PlayMakerFSM nosk) {
            PlayMakerFSM tlFsm = tLordPrefab.LocateMyFSM("Mantis");
            shockwaveObject = tlFsm.GetState("Waves").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            roarAudio = tlFsm.GetState("Slam Antic").GetFirstActionOfType<AudioPlayerOneShot>().audioClips[0];
            nosk.AddFsmGameObject("Traitor Wave", null);
            for(int i = 0; i < 2; i++)
                shockwaveObject.Spawn();
            shockwaveObject.RecycleAll();
        }

        public static void HookWosk(PlayMakerFSM wosk) {
            PlayMakerFSM tlFsm = tLordPrefab.LocateMyFSM("Mantis");
            FsmState sickleThrowState = tlFsm.GetState("Sickle Throw");
            sickleObject = sickleThrowState.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            sickleAudio = sickleThrowState.GetFirstActionOfType<AudioPlayerOneShot>().audioClips[0];
            wosk.AddFsmFloat("Sickle Scale", 0);
            wosk.AddFsmFloat("Sickle Speed", 0);
            wosk.AddFsmGameObject("Sickle Projectile", null);
            for(int i = 0; i < 2; i++)
                sickleObject.Spawn();
            sickleObject.RecycleAll();
        }

        public static FsmStateAction[] RunShockwave(PlayMakerFSM self) {
            return [
                new BoolTest {
                    boolVariable = self.FsmVariables.GetFsmBool("Falling from Ceiling"),
                    isFalse = FsmEvent.Finished,
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

        public static FsmStateAction[] RunSickles(PlayMakerFSM self) {
            return [
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = sickleAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store"),
                },
                new GetScale {
                    gameObject = new FsmOwnerDefault { OwnerOption = OwnerDefaultOption.UseOwner },
                    vector = Vector3.zero,
                    xScale = self.FsmVariables.GetFsmFloat("Sickle Scale"),
                    yScale = 0,
                    zScale = 0,
                    space = Space.World,
                    everyFrame = false
                },
                new FloatOperator {
                    float1 = -8,
                    float2 = self.FsmVariables.GetFsmFloat("Sickle Scale"),
                    operation = FloatOperator.Operation.Multiply,
                    storeResult = self.FsmVariables.GetFsmFloat("Sickle Speed"),
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = sickleObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, -5, 0),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Sickle Projectile")
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Sickle Projectile"),
                    vector = Vector2.zero,
                    x = self.FsmVariables.GetFsmFloat("Sickle Speed"),
                    y = 0,
                    everyFrame = false
                },
                new FloatMultiply {
                    floatVariable = self.FsmVariables.GetFsmFloat("Sickle Speed"),
                    multiplyBy = 2.25f,
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = sickleObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, -5, 0),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Sickle Projectile")
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Sickle Projectile"),
                    vector = Vector2.zero,
                    x = self.FsmVariables.GetFsmFloat("Sickle Speed"),
                    y = 0,
                    everyFrame = false
                },
                new SetVelocity2d {
                    gameObject = new FsmOwnerDefault { OwnerOption = OwnerDefaultOption.UseOwner },
                    vector = Vector2.zero,
                    x = 0,
                    y = 0,
                    everyFrame = false
                }
            ];
        }
    }
}
