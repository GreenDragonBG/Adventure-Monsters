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

        [SerializeField] private GameObject mushrooms;
        private Light2D[] mushroomLights;
        private float lightPeriod;
        private float warningStart;
        private bool isWarning;
        private float warningCounter;
        
        private void Start()
        {
            //finds the mushroomLights and sets lightPeriod
            mushroomLights = mushrooms.GetComponentsInChildren<Light2D>();
            lightPeriod = 0.2f;
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
            //if isWarning and it has waited long enough it flashes warning lights on and off
            if (isWarning && Time.time - warningStart >= lightPeriod)
            {
                if (warningCounter %2 == 0)
                {
                    LightWarningOff();
                }
                else
                {
                    LightWarningOn();
                }
                warningCounter++;
                warningStart = Time.time;
                
                if (warningCounter>=7)
                {
                    warningCounter = 0;
                    isWarning = false;
                    PlyaParticles();
                }
            }

            //if it's calculated to be triggered it sets isWarning to True
            if (willTrigger)
            {
                isWarning = true;
                warningStart= Time.time;
                willTrigger = false;
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
            // 1 out of 3 chance
            int roll = Random.Range(0, 3); // returns 0, 1, or 2
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