using System;
using System.Collections;
using UnityEngine;

namespace SceneChange
{
    public class AreaEntrance : AreaTransition
    {
        private void Start()
        {
            StartCoroutine(CheckAllEntrances());
        }

        protected override void OnCameraDisabled() { }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // When the player physically walks out of the trigger, 
                // we definitely want to switch to Exit mode.
                DisableEntrance();
            }
        }

        private void DisableEntrance()
        {
            cam.enabled = true;

            // Enable all exits in the scene
            AreaExit[] exits = FindObjectsByType<AreaExit>(FindObjectsInactive.Include, sortMode: FindObjectsSortMode.InstanceID);
            foreach (AreaExit exit in exits)
            {
                exit.enabled = true;
                exit.gameObject.SetActive(true);
            }

            // Disable all entrances
            AreaEntrance[] entrances = FindObjectsByType<AreaEntrance>(FindObjectsInactive.Include, sortMode: FindObjectsSortMode.InstanceID);
            foreach (AreaEntrance entrance in entrances)
            {
                entrance.isActive = false;
                entrance.gameObject.SetActive(false);
            }
        }

        private IEnumerator CheckAllEntrances()
        {
            // Wait for physics/player spawning to settle
            yield return new WaitForEndOfFrame();

            AreaEntrance[] allEntrances = FindObjectsByType<AreaEntrance>(FindObjectsInactive.Include, sortMode: FindObjectsSortMode.InstanceID);
            
            bool playerIsEnteringAnywhere = false;

            foreach (AreaEntrance ent in allEntrances)
            {
                if (ent.IsPlayerAtThisEntrance())
                {
                    playerIsEnteringAnywhere = true;
                    break; 
                }
            }

            // ONLY if the player is not entering through ANY door do we disable the system
            if (!playerIsEnteringAnywhere)
            {
                DisableEntrance();
            }
            else
            {
                // If the player IS at THIS specific entrance, set up the camera
                if (IsPlayerAtThisEntrance())
                {
                    cam.enabled = false;
                    isActive = true;
                }
            }
        }

        // Helper method to see if the player is currently inside this specific entrance logic
        private bool IsPlayerAtThisEntrance()
        {
            bool isPlayerLeft = player.gameObject.transform.position.x < transform.position.x;
            bool isPlayerInRange = player.transform.position.y > transform.position.y - 7 &&
                                   player.transform.position.y < transform.position.y + 7;

            // Logic: Is the player within Y bounds AND is their side consistent with the entrance direction?
            return isPlayerInRange && isPlayerLeft != toMoveLeft;
        }
    }
}