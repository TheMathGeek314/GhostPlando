using System.Collections;
using UnityEngine;
using ItemChanger.Extensions;

namespace GhostPlando {
    internal class ParagonDeathEffects: MonoBehaviour {
        public static ParagonDeathEffects instance;
        public static ParticleSystem particleSystem;
        public Transform hero = HeroController.instance.transform;
        public static float jerk = 80;
        public static float radius = 2;
        public static float finalVelocity = 2000;
        public static float startDecreaseMultiplier = -10;
        private float acceleration = 0;

        private ParticleSystem.Particle[] particles;

        public ParagonDeathEffects() {
            instance = this;
        }

        void Update() {
            if(acceleration == 0)
                return;
            int count = particleSystem.particleCount;
            if(particles == null || particles.Length < count)
                particles = new ParticleSystem.Particle[count];
            count = particleSystem.GetParticles(particles);
            Vector3 heroPosition = hero.position;
            for(int i = 0; i < count; i++) {
                Vector3 direction = heroPosition - particles[i].position;
                Vector3 dnt = direction.normalized * Time.deltaTime;
                if(direction.magnitude < radius)
                    particles[i].velocity = dnt * finalVelocity;
                else
                    particles[i].velocity += dnt * acceleration;
            }
            particleSystem.SetParticles(particles, count);
        }

        public IEnumerator BeginEffects() {
            bool completedIntro = false;
            acceleration = 0;
            ParticleSystem.MainModule main = particleSystem.main;
            float prefocusTime = 0;
            bool hinted = false;
            while(!completedIntro) {
                while(!HeroController.instance.cState.focusing) {
                    prefocusTime += Time.deltaTime;
                    if(!hinted && prefocusTime > 5) {
                        GameCameras.instance.hudCamera.gameObject.FindChild(["Prompts", "Control Reminder"]).LocateMyFSM("Display").SendEvent("REMINDER FOCUS");
                        hinted = true;
                    }
                    yield return null;
                }
                prefocusTime = 0;
                float startTime = Time.time;
                float timeDiff = 0;
                while(timeDiff < 10) {
                    if(!HeroController.instance.cState.focusing) {
                        acceleration = 0;
                        main.startSpeedMultiplier = 110;
                        break;
                    }
                    acceleration += jerk * Time.deltaTime;
                    main.startSpeedMultiplier = timeDiff * startDecreaseMultiplier + 100;
                    timeDiff = Time.time - startTime;
                    yield return null;
                }
                if(timeDiff >= 10)
                    completedIntro = true;
            }
            GameManager.instance.LoadScene("Cinematic_Ending_C");
        }
    }
}
