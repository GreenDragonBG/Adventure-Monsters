using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Mush
{
    public class MushSpores : MonoBehaviour
    {
        private ParticleSystem[] particles;
        private bool willTrigger;

        [Header("Mushroom lights")]
        [SerializeField] private GameObject mushrooms;
        private Light2D[] mushroomLights;
        
        [Header("Warning Effect")]
        private float lightPeriod;
        private float warningStart;
        private bool isWarning;
        private float warningCounter;
        
        private void Start()
        {
            //finds the mushroomLights and sets lightPeriod
            mushroomLights = mushrooms.GetComponentsInChildren<Light2D>();
            lightPeriod = 0.3f;
            //finds the particleSystems and stops them at the start
            particles = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in particles)
            {
                particle.Stop();
            }
            warningCounter = 0;
        }

        private void Update()
        {
            //if it's calculated to be triggered it sets isWarning to True
            if (willTrigger)
            {
                warningStart= Time.time;
                willTrigger = false;
                isWarning = true;
                MushPlatform.IsActive = true;
            }
            
            
            //if isWarning and it has waited long enough it flashes warning lights on and off
            if (isWarning && Time.time - warningStart >= lightPeriod)
            {
                if (warningCounter %2 == 0)
                {
                    LightWarningOff();
                    warningCounter++;
                }
                else
                {
                    LightWarningOn();
                    warningCounter++;
                }
                warningStart = Time.time;
                
                if (warningCounter>=7)
                {
                    warningCounter = 0;
                    PlyaParticles();
                    isWarning = false;
                }
            }
            else if (particles[0].isPlaying==false && !isWarning && !willTrigger) 
            {
                MushPlatform.IsActive = false;
            }
        }

        private void PlyaParticles()
        {
            //plays every ParticleSystem
            foreach (ParticleSystem particle in particles)
            {
                particle.Play();
            }
        }
        
        //Sets the warning lights on
        private void LightWarningOn()
        {
            foreach (Light2D mushroom in mushroomLights)
            {
                mushroom.color = new Color(0.41f, 1f, 0f, 1f);
            }
        }
        
        //Sets the warning lights off
        private void LightWarningOff()
        {
            foreach (Light2D mushroom in mushroomLights)
            {
               mushroom.color = new Color(1f, 0.36f, 0.886f, 1f);
            }
        }

        public void CalculateChance()
        {
            if (particles[0].isPlaying || isWarning)
            {
                return;
            }

            //1 out of 5 chance
            int roll = Random.Range(0, 5); // returns 0, 1, 2, 3 or 4
            if (roll == 0)
            {
                willTrigger = true;
            }
            else
            {
                willTrigger = false;
            }
        }
    }
}