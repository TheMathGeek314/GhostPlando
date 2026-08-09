using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class CGuardian {
        internal static GameObject turretPrefab;
        internal static GameObject cgPrefab;
        internal static Object laserPrepAudio;
        internal static Object laserBurstAudio;

        public static void HookNosk(PlayMakerFSM nosk) {
            for(float f = 78f; f < 115; f += 4f)
                GameObject.Instantiate(turretPrefab, new Vector3(f, 19.5f, 0.006f), Quaternion.identity).SetActive(true);
        }

        public static void HookWosk(PlayMakerFSM wosk) {
            GameObject cg = GameObject.Instantiate(cgPrefab);
            GameObject emptyParent = new("CG Beam Parent");
            foreach(string obj in new string[] { "Beam", "Beam Ball", "Beam Impact" }) {
                GameObject go = GameObject.Instantiate(cg.FindChild(obj), wosk.transform.position, Quaternion.identity, emptyParent.transform);
                go.name = obj;
                wosk.AddFsmGameObject("CG " + obj, go);
            }
            foreach(string cgFloat in new string[] { "Ray Vector X", "Ray Vector Y", "Angle", "Ray Distance" })
                wosk.AddFsmFloat("CG " + cgFloat, 0);
            foreach(string cgVec in new string[] { "Ray Vector", "Ray Hit Point" })
                wosk.AddFsmVector2("CG " + cgVec, Vector2.zero);
            foreach(string emptyObj in new string[] { "Init Particle", "Beam Glow", "Impact Particle", "Sound Player" })
                wosk.AddFsmGameObject("CG " + emptyObj, null);
            PlayMakerFSM cgfsm = cgPrefab.LocateMyFSM("Beam Miner");
            laserPrepAudio = cgfsm.GetState("Beam Antic").GetFirstActionOfType<AudioPlaySimple>().oneShotClip.Value;
            laserBurstAudio = cgfsm.GetState("Beam").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            GameObject.Destroy(cg);
        }

        public static FsmStateAction[] RunCeilingLasers(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = new FsmEventTarget { target = FsmEventTarget.EventTarget.BroadcastAll },
                    sendEvent = "LASER SHOOT",
                    delay = 0f,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] InitDirectLaser(PlayMakerFSM self) {
            return [
                new SetScale {
                    gameObject = self.FsmGOToOwner("CG Beam Ball"),
                    vector = Vector3.zero,
                    x = 1,
                    y = 1,
                    z = 0,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SetScale {
                    gameObject = self.FsmGOToOwner("CG Beam Impact"),
                    vector = Vector3.zero,
                    x = 1,
                    y = 1,
                    z = 0,
                    everyFrame = false,
                    lateUpdate = false
                },
                new FindChild {
                    gameObject = self.FsmGOToOwner("CG Beam Impact"),
                    childName = "Init Particle",
                    storeResult = self.FsmVariables.GetFsmGameObject("CG Init Particle")
                },
                new FindChild {
                    gameObject = self.FsmGOToOwner("CG Beam"),
                    childName = "Beam Glow",
                    storeResult = self.FsmVariables.GetFsmGameObject("CG Beam Glow")
                },
                new FindChild {
                    gameObject = self.FsmGOToOwner("CG Beam Impact"),
                    childName = "Impact Particle",
                    storeResult = self.FsmVariables.GetFsmGameObject("CG Impact Particle")
                }
            ];
        }

        public static FsmStateAction[] PrepDirectLaser(PlayMakerFSM self) {
            FsmVariables fv = self.FsmVariables;
            FsmFloat selfX = fv.GetFsmFloat("Self X");
            FsmFloat selfY = fv.GetFsmFloat("Self Y");
            FsmFloat rayVX = fv.GetFsmFloat("CG Ray Vector X");
            FsmFloat rayVY = fv.GetFsmFloat("CG Ray Vector Y");
            FsmFloat angle = fv.GetFsmFloat("CG Angle");
            FsmOwnerDefault cgBeam = self.FsmGOToOwner("CG Beam");
            return [
                new SetVelocity2d {
                    gameObject = new FsmOwnerDefault { OwnerOption = OwnerDefaultOption.UseOwner },
                    vector = Vector2.zero,
                    x = 0,
                    y = 0,
                    everyFrame = false
                },
               new GetPosition {
                    gameObject = self.FsmGOToOwner("Hero"),
                    vector = Vector3.zero,
                    x = rayVX,
                    y = rayVY,
                    z = 0,
                    space = Space.World,
                    everyFrame = false
                },
                new GetPosition {
                    gameObject = self.FsmGOToOwner("Self"),
                    vector = Vector3.zero,
                    x = selfX,
                    y = selfY,
                    z = 0,
                    space = Space.World,
                    everyFrame = false
                },
                new SetPosition {
                    gameObject = self.FsmGOToOwner("CG Beam Ball"),
                    vector = Vector3.zero,
                    x = selfX,
                    y = selfY,
                    z = 0,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SetPosition {
                    gameObject = cgBeam,
                    vector = Vector3.zero,
                    x = selfX,
                    y = selfY,
                    z = 0,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new FloatSubtract {
                    floatVariable = rayVX,
                    subtract = selfX,
                    everyFrame = false,
                    perSecond = false
                },
                new FloatSubtract {
                    floatVariable = rayVY,
                    subtract = selfY,
                    everyFrame = false,
                    perSecond = false
                },
                new SetVector2XY {
                    vector2Variable = fv.GetFsmVector2("CG Ray Vector"),
                    vector2Value = Vector2.zero,
                    x = rayVX,
                    y = rayVY,
                    everyFrame = false
                },
                new GetAngleToTarget2D {
                    gameObject = cgBeam,
                    target = fv.GetFsmGameObject("Hero"),
                    offsetX = 0,
                    offsetY = 0,
                    storeAngle = angle,
                    everyFrame = false
                },
                new RayCast2d {
                    fromGameObject = cgBeam,
                    fromPosition = Vector2.zero,
                    direction = fv.GetFsmVector2("CG Ray Vector"),
                    space = Space.World,
                    distance = 100,
                    minDepth = 0,
                    maxDepth = 0,
                    storeDidHit = false,
                    storeHitPoint = fv.GetFsmVector2("CG Ray Hit Point"),
                    storeHitNormal = Vector2.zero,
                    storeHitDistance = fv.GetFsmFloat("CG Ray Distance"),
                    storeHitFraction = 0,
                    repeatInterval = 1,
                    layerMask = [8],
                    invertMask = false,
                    debug = false
                },
                new SetRotation {
                    gameObject = self.FsmGOToOwner("CG Beam Impact"),
                    quaternion = Quaternion.identity,
                    vector = Vector3.zero,
                    xAngle = 0,
                    yAngle = 0,
                    zAngle = angle,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SetRotation {
                    gameObject = self.FsmGOToOwner("CG Beam Ball"),
                    quaternion = Quaternion.identity,
                    vector = Vector3.zero,
                    xAngle = 0,
                    yAngle = 0,
                    zAngle = angle,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new AudioPlaySimple {
                    gameObject = new FsmOwnerDefault { OwnerOption = OwnerDefaultOption.UseOwner },
                    volume = 1,
                    oneShotClip = laserPrepAudio
                },
                new SetMeshRenderer {
                    gameObject = self.FsmGOToOwner("CG Beam Ball"),
                    active = true
                },
                new Lambda(() => {
                    Vector2 hitPoint = fv.GetFsmVector2("CG Ray Hit Point").Value;
                    fv.GetFsmGameObject("CG Beam Impact").Value.transform.position = new Vector3(hitPoint.x, hitPoint.y, -0.004f);
                }),
                new Tk2dPlayAnimation {
                    gameObject = self.FsmGOToOwner("CG Beam Ball"),
                    animLibName = "",
                    clipName = "Ball Antic"
                },
                new SetRotation {
                    gameObject = cgBeam,
                    quaternion = Quaternion.identity,
                    vector = Vector3.zero,
                    xAngle = 0,
                    yAngle = 0,
                    zAngle = angle,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SetMeshRenderer {
                    gameObject = cgBeam,
                    active = true
                },
                new ActivateGameObject {
                    gameObject = cgBeam,
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new Tk2dPlayAnimation {
                    gameObject = cgBeam,
                    animLibName = "",
                    clipName = "Beam Antic"
                },
                new FloatDivide {
                    floatVariable = fv.GetFsmFloat("CG Ray Distance"),
                    divideBy = 1.75f,
                    everyFrame = false
                },
                new SetScale {
                    gameObject = cgBeam,
                    vector = Vector3.zero,
                    x = fv.GetFsmFloat("CG Ray Distance"),
                    y = 1,
                    z = 0,
                    everyFrame = false,
                    lateUpdate = false
                },
                new PlayParticleEmitter {
                    gameObject = self.FsmGOToOwner("CG Init Particle"),
                    emit = 0
                },
                new Wait {
                    time = 0.7f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] FireDirectLaser(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("CG Beam Glow"),
                    sendEvent = "UP",
                    delay = 0,
                    everyFrame = false
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = laserBurstAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("CG Sound Player")
                },
                new Tk2dPlayAnimation {
                    gameObject = self.FsmGOToOwner("CG Beam"),
                    animLibName = "",
                    clipName = "Beam Shoot"
                },
                new Tk2dPlayAnimation {
                    gameObject = self.FsmGOToOwner("CG Beam Ball"),
                    animLibName = "",
                    clipName = "Ball Shoot"
                },
                new Wait {
                    time = 0.5f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                },
                new SetMeshRenderer {
                    gameObject = self.FsmGOToOwner("CG Beam Impact"),
                    active = true
                },
                new PlayParticleEmitter {
                    gameObject = self.FsmGOToOwner("CG Impact Particle"),
                    emit = 0
                },
                new StopParticleEmitter {
                    gameObject = self.FsmGOToOwner("CG Init Particle")
                }
            ];
        }

        public static FsmStateAction[] EndDirectLaser(PlayMakerFSM self) {
            return [
                new FadeAudio {
                    gameObject = self.FsmGOToOwner("CG Sound Player"),
                    startVolume = 1,
                    endVolume = 0,
                    time = 0.2f
                },
                new Tk2dPlayAnimationWithEvents {
                    gameObject = self.FsmGOToOwner("CG Beam"),
                    clipName = "Beam End",
                    animationCompleteEvent = FsmEvent.Finished
                },
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("CG Beam Glow"),
                    sendEvent = "DOWN",
                    delay = 0,
                    everyFrame = false
                },
                new Tk2dPlayAnimation {
                    gameObject = self.FsmGOToOwner("CG Beam Ball"),
                    animLibName = "",
                    clipName = "Ball End"
                },
                new SetMeshRenderer {
                    gameObject = self.FsmGOToOwner("CG Beam Impact"),
                    active = false
                },
                new StopParticleEmitter {
                    gameObject = self.FsmGOToOwner("CG Impact Particle")
                },
                new SetMeshRenderer {
                    gameObject = self.FsmGOToOwner("CG Beam Ball"),
                    active = false
                },
                new SetMeshRenderer {
                    gameObject = self.FsmGOToOwner("CG Beam"),
                    active = false
                }
            ];
        }
    }
}
