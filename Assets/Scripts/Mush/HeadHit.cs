using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace Mush
{
    public class HeadHit : MonoBehaviour
    {
        [SerializeField] private Sprite hitSprite;
        [SerializeField] private Sprite defultSprite;
        [SerializeField] private SpriteRenderer mushSpriteRenderer;


        private bool inRange = false;
        private float timeExitedRange;
        private float timeEnteredRange;
        private bool willTrigger;
        
        private void Start()
        {
        }

        private void Update()
        {
            if (willTrigger)
            {
                DoOnTrigger();
            }
        }

        private void DoOnTrigger()
        {
            willTrigger = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //if player enters sets the enter time
            if (other.CompareTag("Player"))
            {
                timeEnteredRange = Time.time;
                inRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            //if player exits sets the exit time
            if (other.CompareTag("Player"))
            {
                timeExitedRange = Time.time;
                inRange = false;
            }
        }

        public void CalculateChance()
        {
            //randomly chooses a number and if it has not exited it sets the exit time to current
            int chance = Random.Range(2, 12);
            if (timeExitedRange ==0)
            {
                timeExitedRange = Time.time;
            }
            //checks if the person has entered and if he has stayed in range for more than 3 sec
            //if he combines the randomly number plus the time spend if its > 10 it says that it can be triggered
            if (timeEnteredRange !=0 && (timeExitedRange - timeEnteredRange) >= 3)
            {
                float result = chance + (timeExitedRange - timeEnteredRange);
                if (result >= 10)
                {
                    willTrigger = true;
                }
            }
            //checks if it has exited if not , resets the time entered to current time , else to 0
            //also sets exit time to 0 either way
            timeExitedRange = 0;
            if (inRange)
            {
                timeEnteredRange = Time.time;
            }
            else
            {
                timeEnteredRange = 0;
            }
        }
    }
}